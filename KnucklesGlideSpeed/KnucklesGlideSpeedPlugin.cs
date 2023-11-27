using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriPlayerAction;

namespace KnucklesGlideSpeed;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class KnucklesGlideSpeedPlugin : BasePlugin {
    private static Harmony _harmony;
    private static float _targetX;
    private static KnucklesGlideSpeedConfig _config;

    public override void Load() {
        _harmony = new Harmony("Superstars.KnucklesGlideSpeed");
        _config = KnucklesGlideSpeedConfig.Load(Log);

        var originalPlActionJumpUniqueKnuckles_Init = AccessTools.Method(typeof(PlActionJumpUniqueKnuckles), "Init");
        var originalPlActionJumpUniqueKnuckles_FixedUpdate = AccessTools.Method(typeof(PlActionJumpUniqueKnuckles), "FixedUpdate");
        if (originalPlActionJumpUniqueKnuckles_Init == null) {
            Log.LogError("Failed to find PlActionJumpUniqueKnuckles.Init");
            return;
        }
        if (originalPlActionJumpUniqueKnuckles_FixedUpdate == null) {
            Log.LogError("Failed to find PlActionJumpUniqueKnuckles.FixedUpdate");
            return;
        }
        var pre_PlActionJumpUniqueKnuckles_Init = AccessTools.Method(typeof(KnucklesGlideSpeedPlugin), "Hook_PlActionJumpUniqueKnuckles_Init");
        var post_PlActionJumpUniqueKnuckles_FixedUpdate = AccessTools.Method(typeof(KnucklesGlideSpeedPlugin), "Hook_PlActionJumpUniqueKnuckles_FixedUpdate");
        _harmony.Patch(originalPlActionJumpUniqueKnuckles_Init, prefix: new HarmonyMethod(pre_PlActionJumpUniqueKnuckles_Init));
        _harmony.Patch(originalPlActionJumpUniqueKnuckles_FixedUpdate, postfix: new HarmonyMethod(post_PlActionJumpUniqueKnuckles_FixedUpdate));
    }

    public static void Hook_PlActionJumpUniqueKnuckles_Init(PlActionJumpUniqueKnuckles __instance) {
        if (_config.UseStartXSpeed) {
            _targetX = System.Math.Max(3f, __instance.OwnerPlyer.VelocityX);
        }
        else {
            _targetX = _config.GlideSpeed;
        }
    }

    public static void Hook_PlActionJumpUniqueKnuckles_FixedUpdate(PlActionJumpUniqueKnuckles __instance) {
        __instance.targetVelX = _targetX;
    }
}
