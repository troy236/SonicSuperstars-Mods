using arz.input;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using OriPlayer;

namespace QuickDeath;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class QuickDeathPlugin : BasePlugin {
    private static QuickDeathConfig _config;
    private static bool _Triggered = false;
    private static System.Threading.Timer _timer;

    public override void Load() {
        var harmony = new Harmony("Superstars.QuickDeath");
        _config = QuickDeathConfig.Load(Log);
        if (_config.QuickDeathEnumCombo == PlatformPad.Button.None && _config.QuickRestartEnumCombo == PlatformPad.Button.None) {
            Log.LogInfo("No button combo assigned. Exiting");
            return;
        }
        if (_config.QuickDeathEnumCombo != PlatformPad.Button.None) {
            Log.LogInfo($"Quick Death combo assigned to: {_config.QuickDeathEnumCombo}");
        }
        if (_config.QuickRestartEnumCombo != PlatformPad.Button.None) {
            Log.LogInfo($"Quick Restart combo assigned to: {_config.QuickRestartEnumCombo}");
        }
        var originalPlayerMain2D_LateUpdate = AccessTools.Method(typeof(PlayerMain2D), "LateUpdate");
        if (originalPlayerMain2D_LateUpdate == null) {
            Log.LogError("Failed to find PlayerMain2D.LateUpdate");
            return;
        }
        var post_LateUpdate = AccessTools.Method(typeof(QuickDeathPlugin), "Hook_LateUpdate");
        harmony.Patch(originalPlayerMain2D_LateUpdate, postfix: new HarmonyMethod(post_LateUpdate));
    }

    public static void Hook_LateUpdate(PlayerMain2D __instance) {
        if (__instance.playerID == -1) return;
        if (_Triggered) {
            return;
        }
        var inputPlayerController = __instance.inputPlyCtlr;
        if (inputPlayerController == null) return;
        var playerPad = inputPlayerController.playerPad;
        if (playerPad == null) return;
        var nowPad = playerPad.nowPad;
        if (nowPad == null) return;
        var onButton = nowPad.On;
        if (_config.QuickDeathEnumCombo != PlatformPad.Button.None) {
            if ((onButton & _config.QuickDeathEnumCombo) == _config.QuickDeathEnumCombo) {
                __instance.SetSandwichedDead();
                _Triggered = true;
                _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(3), System.Threading.Timeout.InfiniteTimeSpan);
            }
        }
        if (_config.QuickRestartEnumCombo != PlatformPad.Button.None) {
            if ((onButton & _config.QuickRestartEnumCombo) == _config.QuickRestartEnumCombo) {
                GameSceneControllerBase.ReStart();
                _Triggered = true;
                _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(3), System.Threading.Timeout.InfiniteTimeSpan);
            }
        }
    }

    private static void ResetTriggered(object state) {
        _timer.Dispose();
        _Triggered = false;
    }
}
