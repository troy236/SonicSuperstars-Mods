using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;

namespace EmeraldPowers;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EmeraldPowersPlugin : BasePlugin {

    public override void Load() {
        var harmony = new Harmony("Superstars.EmeraldPowers");
        var originalPlayer_ForceResetInductionEmerald = AccessTools.Method(typeof(GameSceneController), "Player_ForceResetInductionEmerald");
        var originalPlayer_ResetSelectedEmeraldAll = AccessTools.Method(typeof(GameSceneController), "Player_ResetSelectedEmeraldAll");
        if (originalPlayer_ForceResetInductionEmerald == null) {
            Log.LogError("Failed to find GameSceneController.Player_ForceResetInductionEmerald");
            return;
        }
        if (originalPlayer_ResetSelectedEmeraldAll == null) {
            Log.LogError("Failed to find GameSceneController.Player_ResetSelectedEmeraldAll");
            return;
        }
        var postGameSceneController_Player_ForceResetInductionEmerald = AccessTools.Method(typeof(EmeraldPowersPlugin), "HookGameScene_Player");
        harmony.Patch(originalPlayer_ForceResetInductionEmerald, prefix: new HarmonyMethod(postGameSceneController_Player_ForceResetInductionEmerald));

        var postGameSceneController_Player_ResetSelectedEmeraldAll = AccessTools.Method(typeof(EmeraldPowersPlugin), "HookGameScene_Player");
        harmony.Patch(originalPlayer_ResetSelectedEmeraldAll, prefix: new HarmonyMethod(postGameSceneController_Player_ResetSelectedEmeraldAll));
    }

    public static bool HookGameScene_Player() {
        return false;
    }
}
