using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriGmk;

namespace PressFactoryDisableDrone;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class PressFactoryDisableDronePlugin : BasePlugin {
    private static Harmony _harmony;

    public override void Load() {
        _harmony = new Harmony("Superstars.PressFactoryNoDrone");
        var originalGmkTicktackDrone_ActCountUp = AccessTools.Method(typeof(GmkTicktackDrone), "ActCountUp");
        if (originalGmkTicktackDrone_ActCountUp == null) {
            Log.LogError("Failed to find GmkTicktackDrone.ActCountUp");
            return;
        }
        var pre_ActCountUp = AccessTools.Method(typeof(PressFactoryDisableDronePlugin), "Hook_ActCountUp");
        _harmony.Patch(originalGmkTicktackDrone_ActCountUp, prefix: new HarmonyMethod(pre_ActCountUp));
    }

    public static bool Hook_ActCountUp() {
        return false;
    }

}
