using System.IO;
using System.Linq;
using arz.input;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Moon.Scene;
using OriGmk;
using Orion;
using OriPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerWarp;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class PlayerWarpPlugin : BasePlugin {
    private static ManualLogSource _log;
    private static Harmony _harmony;
    private static PlayerWarpConfig _config;
    private static FileSystemWatcher _watcher;
    private static int _zoneNum = 0;
    private static int _actNum = 0;
    private static bool _isTripStory = false;
    private static System.Numerics.Vector3 _defaultPosition;
    private static bool _Triggered = false;
    private static bool _configChangeTriggered = false;
    private static int _checkPointCount = 0;
    public static System.Numerics.Vector3 _customWarpPosition;
    private static System.Threading.Timer _timer;

    public override void Load() {
        _log = Log;
        _harmony = new Harmony("Superstars.PlayerWarp");
        _config = PlayerWarpConfig.Load(Log, out _);
        if (_config.WarpEnumCombo == PlatformPad.Button.None) {
            Log.LogInfo("No button combo assigned. Exiting");
            return;
        }
        if (_config.WarpEnumCombo != PlatformPad.Button.None) {
            Log.LogInfo($"Player Warp combo assigned to: {_config.WarpEnumCombo}");
        }
        _customWarpPosition = new(0, 0, -1);
        _defaultPosition = new(0, 0, -1);
        var originalPlayerMain2D_LateUpdate = AccessTools.Method(typeof(PlayerMain2D), "LateUpdate");
        var originalScene_Manager_SceneLoaded = AccessTools.Method(typeof(Scene_Manager), "SceneLoaded");
        var originalGameSceneControllerBase_ReStart_1 = AccessTools.Method(typeof(GameSceneControllerBase), "ReStart", [typeof(DB_TransitionsEachTimeDefine.EAnimType)]);
        if (originalPlayerMain2D_LateUpdate == null) {
            Log.LogError("Failed to find PlayerMain2D.LateUpdate");
            return;
        }
        if (originalScene_Manager_SceneLoaded == null) {
            Log.LogError("Failed to find Scene_Manager.SceneLoaded");
            return;
        }
        if (originalGameSceneControllerBase_ReStart_1 == null) {
            _log.LogError("Failed to find GameSceneControllerBase.ReStart");
            return;
        }
        var post_LateUpdate = AccessTools.Method(typeof(PlayerWarpPlugin), "Hook_LateUpdate");
        var post_SceneLoaded = AccessTools.Method(typeof(PlayerWarpPlugin), "Hook_SceneLoaded");
        var post_ReStart = AccessTools.Method(typeof(PlayerWarpPlugin), "Hook_ReStart");
        _harmony.Patch(originalPlayerMain2D_LateUpdate, postfix: new HarmonyMethod(post_LateUpdate));
        _harmony.Patch(originalScene_Manager_SceneLoaded, postfix: new HarmonyMethod(post_SceneLoaded));
        _harmony.Patch(originalGameSceneControllerBase_ReStart_1, postfix: new HarmonyMethod(post_ReStart));

        if (_config.ReloadConfigOnChange) {
            _watcher = new FileSystemWatcher();
            _watcher.Path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _watcher.Filter = "PlayerWarpConfig.txt";
            _watcher.IncludeSubdirectories = false;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += ConfigChanged;
            _watcher.EnableRaisingEvents = true;
        }
    }

    private static void ConfigChanged(object sender, FileSystemEventArgs e) {
        if (!_config.ReloadConfigOnChange) return;
        if (_configChangeTriggered) {
            _log.LogInfo("Reloading config");
            var config = PlayerWarpConfig.Load(_log, out bool succeeded);
            if (succeeded) {
                _config = config;
            }
            else {
                _log.LogWarning("Failed to parse config. Keeping old settings");
            }
        }
        _configChangeTriggered = !_configChangeTriggered;
    }

    public static void Hook_LateUpdate(PlayerMain2D __instance) {
        if (__instance.playerID == -1) return;
        if (_Triggered) {
            return;
        }
        var inputPlayerController = __instance.inputPlyCtlr;
        if (inputPlayerController is null) return;
        var playerPad = inputPlayerController.playerPad;
        if (playerPad is null) return;
        var nowPad = playerPad.nowPad;
        if (nowPad is null) return;
        var onButton = nowPad.On;
        if ((onButton & _config.WarpEnumCombo) == _config.WarpEnumCombo) {
            if (_config.CycleThroughCheckpoints) {
                WarpToCheckpoint(__instance);
            }
            else {
                if (_customWarpPosition.X == 0 && _customWarpPosition.Y == 0 && _customWarpPosition.Z == -1) {
                    _log.LogInfo($"Setting Warp Position X: {__instance.transform.position.x} Y: {__instance.transform.position.y} Z: {__instance.transform.position.z}");
                    _customWarpPosition = new(__instance.transform.position.x, __instance.transform.position.y, __instance.transform.position.z);
                }
                else {
                    __instance.transform.position = new(_customWarpPosition.X, _customWarpPosition.Y, _customWarpPosition.Z);
                }
            }
            RestoreEmeralds();
            __instance.ForceResetInductionEmerald();
            bool zeroGravity = false;
            __instance.ApplyZeroGravity(ref zeroGravity);
            _Triggered = true;
            _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(1), System.Threading.Timeout.InfiniteTimeSpan);
        }
    }

    public static void WarpToCheckpoint(PlayerMain2D player) {
        var checkpoints = UnityEngine.Object.FindObjectsByType<GmkCheckPointWay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (checkpoints.Length == 0) {
            _log.LogWarning("No checkpoints found to warp to");
            return;
        }
        GmkCheckPointWay[] checkpointsOrdered;
        if (_zoneNum == 11 && _actNum == 2) {
            checkpointsOrdered = checkpoints.OrderByDescending(cp => cp.ExitPointPosition.x).ToArray();
        }
        else {
            checkpointsOrdered = checkpoints.OrderBy(cp => cp.ExitPointPosition.x).ToArray();
        }
        int checkPointIndex = _checkPointCount++;
        if (checkpoints.Length - 1 >= checkPointIndex) {
            _log.LogInfo($"Warping to X: {checkpointsOrdered[checkPointIndex].ExitPointPosition.x} Y: {checkpointsOrdered[checkPointIndex].ExitPointPosition.y}");
            player.transform.position = checkpointsOrdered[checkPointIndex].ExitPointPosition;
        }
        else {
            _checkPointCount = 1;
            checkPointIndex = 0;
            _log.LogInfo($"Warping to X: {checkpointsOrdered[checkPointIndex].ExitPointPosition.x} Y: {checkpointsOrdered[checkPointIndex].ExitPointPosition.y}");
            player.transform.position = checkpointsOrdered[checkPointIndex].ExitPointPosition;
        }
    }

    public static System.Numerics.Vector3 GetCustomPosition() {
        if (_isTripStory) {
            switch (_zoneNum) {
                case 1:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Bridge_Island_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Bridge_Island_Act2_WarpPosition);
                    }
                    break;
                case 2:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Speed_Jungle_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Speed_Jungle_Act2_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Trip_Speed_Jungle_Act3_WarpPosition);
                    }
                    break;
                case 3:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Sky_Temple_Act1_WarpPosition);
                    }
                    break;
                case 4:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Pinball_Carnival_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Pinball_Carnival_Act2_WarpPosition);
                    }
                    break;
                case 5:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Lagoon_City_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Lagoon_City_Act2_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Trip_Lagoon_City_Act3_WarpPosition);
                    }
                    break;
                case 6:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Sand_Sanctuary_Act1_WarpPosition);
                    }
                    break;
                case 7:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Press_Factory_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Press_Factory_Act2_WarpPosition);
                    }
                    break;
                case 8:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Golden_Capital_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Golden_Capital_Act2_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Trip_Golden_Capital_Act3_WarpPosition);
                    }
                    break;
                case 9:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Cyber_Station_Act1_WarpPosition);
                    }
                    break;
                case 10:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Frozen_Base_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Frozen_Base_Act2_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Trip_Frozen_Base_Act3_WarpPosition);
                    }
                    break;
                case 11:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Trip_Egg_Fortress_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Trip_Egg_Fortress_Act2_WarpPosition);
                    }
                    break;
            }
        }
        else {
            switch (_zoneNum) {
                case 1:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Bridge_Island_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Bridge_Island_Act2_WarpPosition);
                    }
                    break;
                case 2:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Speed_Jungle_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Speed_Jungle_ActSonic_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Speed_Jungle_Act2_WarpPosition);
                    }
                    break;
                case 3:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Sky_Temple_Act1_WarpPosition);
                    }
                    break;
                case 4:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Pinball_Carnival_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Pinball_Carnival_Act2_WarpPosition);
                    }
                    break;
                case 5:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Lagoon_City_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Lagoon_City_ActAmy_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Lagoon_City_Act2_WarpPosition);
                    }
                    break;
                case 6:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Sand_Sanctuary_Act1_WarpPosition);
                    }
                    break;
                case 7:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Press_Factory_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Press_Factory_Act2_WarpPosition);
                    }
                    break;
                case 8:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Golden_Capital_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Golden_Capital_ActKnuckles_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Golden_Capital_Act2_WarpPosition);
                    }
                    break;
                case 9:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Cyber_Station_Act1_WarpPosition);
                    }
                    break;
                case 10:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Frozen_Base_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Frozen_Base_ActTails_WarpPosition);
                    }
                    if (_actNum == 3) {
                        return ParsePosition(_config.Frozen_Base_Act2_WarpPosition);
                    }
                    break;
                case 11:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Egg_Fortress_Act1_WarpPosition);
                    }
                    if (_actNum == 2) {
                        return ParsePosition(_config.Egg_Fortress_Act2_WarpPosition);
                    }
                    break;
                case 12:
                    if (_actNum == 1) {
                        return ParsePosition(_config.Last_Story_WarpPosition);
                    }
                    break;
            }
        }
        return _defaultPosition;
    }

    public static System.Numerics.Vector3 ParsePosition(string configPosition) {
        if (!string.IsNullOrEmpty(configPosition)) {
            if (configPosition == "0,0,-1") {
                return _defaultPosition;
            }
            var configPositions = configPosition.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            if (configPositions.Length != 3) {
                _log.LogWarning($"Invalid length {configPositions.Length} for position while parsing {configPosition}. Expected 3 axis");
                return _defaultPosition;
            }
            System.Numerics.Vector3 vector = new();
            if (float.TryParse(configPositions[0], out float positionFloat)) {
                vector.X = positionFloat;
            }
            else {
                _log.LogWarning($"Failed to parse {configPositions[0]} as a float while parsing {configPosition}.");
                return _defaultPosition;
            }
            if (float.TryParse(configPositions[1], out positionFloat)) {
                vector.Y = positionFloat;
            }
            else {
                _log.LogWarning($"Failed to parse {configPositions[1]} as a float while parsing {configPosition}.");
                return _defaultPosition;
            }
            if (float.TryParse(configPositions[2], out positionFloat)) {
                vector.Z = positionFloat;
            }
            else {
                _log.LogWarning($"Failed to parse {configPositions[2]} as a float while parsing {configPosition}.");
                return _defaultPosition;
            }
            return vector;
        }
        return _defaultPosition;
    }

    public static void RestoreEmeralds() {
        int emeraldCount = GameSceneController.Instance.GetEmeraldNum();
        UIGameMain uIGameMainInstance = UIGameMain.Instance;
        switch (emeraldCount) {
            case 0:
                break;
            case 1:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                break;
            case 2:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                break;
            case 3:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Vision);
                break;
            case 4:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Vision);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Water);
                break;
            case 5:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Vision);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Water);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Ivy);
                break;
            case 6:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Vision);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Water);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Ivy);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Time);
                break;
            case 7:
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bomb);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Bullet);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Vision);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Water);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Ivy);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Time);
                uIGameMainInstance.ResetBossBattleChaosEmeraldActiveUI(PlayerBase.EEmerald.Extra);
                break;
        }
    }

    public static void Hook_ReStart() {
        if (_config.ClearWarpOnRestartAct) ResetWarpLocation();
    }

    public static void ResetWarpLocation() {
        if (_config.CycleThroughCheckpoints) {
            _checkPointCount = 0;
        }
        else {
            var customPosition = GetCustomPosition();
            if (customPosition == _defaultPosition) {
                _customWarpPosition.X = 0;
                _customWarpPosition.Y = 0;
                _customWarpPosition.Z = -1;
            }
            else {
                _customWarpPosition.X = customPosition.X;
                _customWarpPosition.Y = customPosition.Y;
                _customWarpPosition.Z = customPosition.Z;
            }
        }
    }

    public static void Hook_SceneLoaded(Scene nextScene) {
        string sceneName = nextScene.name;
        if (sceneName.StartsWith("Zone")) {
            var sceneNameSplit = sceneName.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
            if (sceneNameSplit.Length < 2) {
                _log.LogError($"Could not parse zone scene name. {sceneName}");
                return;
            }
            _zoneNum = int.Parse(sceneNameSplit[0][4..]);
            _actNum = int.Parse(sceneNameSplit[1][3..]);
            _isTripStory = sceneNameSplit.Length == 3;
            ResetWarpLocation();
        }
    }

    private static void ResetTriggered(object state) {
        _timer.Dispose();
        _Triggered = false;
    }
}
