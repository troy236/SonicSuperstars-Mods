using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriPlayer;
using OriPlayerAction;

namespace SpinDashFix;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SpinDashPlugin : BasePlugin {
    private static Harmony _harmony;
    private static bool _jumpTriggered;

    public override void Load() {
        _harmony = new Harmony("Superstars.SpinDashFix");
        var originalPlActionSpinDashCharge_FixedUpdate = AccessTools.Method(typeof(PlActionSpinDashCharge), "FixedUpdate");
        var originalPlayerMain2D_IsJumpTriggered = AccessTools.Method(typeof(PlayerMain2D), "IsJumpTriggered");
        if (originalPlActionSpinDashCharge_FixedUpdate == null) {
            Log.LogError("Failed to find PlActionSpinDashCharge.FixedUpdate");
            return;
        }
        if (originalPlayerMain2D_IsJumpTriggered == null) {
            Log.LogError("Failed to find PlayerMain2D.IsJumpTriggered");
            return;
        }
        var pre_FixedUpdate = AccessTools.Method(typeof(SpinDashPlugin), "Hook_FixedUpdate");
        var post_IsJumpTriggered2D = AccessTools.Method(typeof(SpinDashPlugin), "Hook_IsJumpTriggered2D");
        _harmony.Patch(originalPlActionSpinDashCharge_FixedUpdate, prefix: new HarmonyMethod(pre_FixedUpdate));
        _harmony.Patch(originalPlayerMain2D_IsJumpTriggered, postfix: new HarmonyMethod(post_IsJumpTriggered2D));
    }

    public static void Hook_FixedUpdate(PlActionSpinDashCharge __instance) {
        if (__instance.ownerPlyer.nextAction <= PlayerBase.EActionIndex.ActNone) {
            if (_jumpTriggered) {
                _jumpTriggered = false;
                __instance.addCharge = true;
            }
        }
    }

    public static void Hook_IsJumpTriggered2D(bool __result) {
        if (__result) {
            _jumpTriggered = true;
        }
    }
}
