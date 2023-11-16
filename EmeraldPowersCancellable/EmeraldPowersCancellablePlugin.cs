using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriPlayer;

namespace EmeraldPowersCancellable;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EmeraldPowersCancellablePlugin : BasePlugin {
    public override void Load() {
        var harmony = new Harmony("Superstars.EmeraldPowersCancellable");
        var originalPlayerMain2D_FixedUpdate = AccessTools.Method(typeof(PlayerMain2D), "FixedUpdate");
        if (originalPlayerMain2D_FixedUpdate == null) {
            Log.LogError("Failed to find PlayerMain2D.FixedUpdate");
            return;
        }
        var post_FixedUpdate = AccessTools.Method(typeof(EmeraldPowersCancellablePlugin), "Hook_FixedUpdate");
        harmony.Patch(originalPlayerMain2D_FixedUpdate, postfix: new HarmonyMethod(post_FixedUpdate));
    }

    public static void Hook_FixedUpdate(PlayerMain2D __instance) {
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
