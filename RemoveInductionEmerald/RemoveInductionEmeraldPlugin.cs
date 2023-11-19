using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriPlayer;

namespace RemoveInductionEmerald;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class RemoveInductionEmeraldPlugin : BasePlugin {
    private static Harmony _harmony;

    public override void Load() {
        _harmony = new Harmony("Superstars.PressFactoryNoDrone");
        var originalPlayerMain2D_UpDateFixedInductionEmerald = AccessTools.Method(typeof(PlayerMain2D), "UpDateFixedInductionEmerald");
        if (originalPlayerMain2D_UpDateFixedInductionEmerald == null) {
            Log.LogError("Failed to find PlayerMain2D.UpDateFixedInductionEmerald");
            return;
        }
        var pre_UpDateFixedInductionEmerald = AccessTools.Method(typeof(RemoveInductionEmeraldPlugin), "Hook_UpDateFixedInductionEmerald");
        _harmony.Patch(originalPlayerMain2D_UpDateFixedInductionEmerald, prefix: new HarmonyMethod(pre_UpDateFixedInductionEmerald));
    }

    public static bool Hook_UpDateFixedInductionEmerald() {
        return false;
    }
}
