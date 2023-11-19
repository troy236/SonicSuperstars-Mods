using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriPlayer;

namespace EmeraldPowersCancellable;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EmeraldPowersCancellablePlugin : BasePlugin {
    private static Harmony _harmony;

    public override void Load() {
        _harmony = new Harmony("Superstars.EmeraldPowersCancellable");
        var originalPlayerBase_get_EmeraldActiveRemainTimeRate = AccessTools.Method(typeof(PlayerBase), "get_EmeraldActiveRemainTimeRate");
        if (originalPlayerBase_get_EmeraldActiveRemainTimeRate == null) {
            Log.LogError("Failed to find PlayerBase.get_EmeraldActiveRemainTimeRate");
            return;
        }
        var post_EmeraldActiveRemainTimeRate = AccessTools.Method(typeof(EmeraldPowersCancellablePlugin), "Hook_EmeraldActiveRemainTimeRate");
        _harmony.Patch(originalPlayerBase_get_EmeraldActiveRemainTimeRate, postfix: new HarmonyMethod(post_EmeraldActiveRemainTimeRate));
    }

    public static void Hook_EmeraldActiveRemainTimeRate(PlayerMain2D __instance) {
        if (__instance.playerID == -1) return;
        float emeraldActiveTime = __instance.emeraldActiveSec;
        if (emeraldActiveTime > 0 && emeraldActiveTime < 15) {
            if (__instance.IsEmeraldCancelTriggered()) {
                __instance.ResetSelectedEmeraldAll();
                __instance.emeraldActiveSec = 0;
            }
        }
    }
}
