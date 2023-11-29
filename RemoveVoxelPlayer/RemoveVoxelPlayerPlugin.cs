using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using OriGmk;
using OriPlayer;

namespace RemoveVoxelPlayer;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class RemoveVoxelPlayerPlugin : BasePlugin {
    private static Harmony _harmony;
    private static RemoveVoxelPlayerConfig _config;

    public override void Load() {
        _harmony = new Harmony("Superstars.RemoveVoxelPlayer");
        _config = RemoveVoxelPlayerConfig.Load(Log);
        var originalPlayerMain2D_ChangePixelPlayer = AccessTools.Method(typeof(PlayerMain2D), "ChangePixelPlayer");
        var originalPlayerRabbit2D_ChangePixelPlayer = AccessTools.Method(typeof(PlayerRabbit2D), "ChangePixelPlayer");
        if (originalPlayerMain2D_ChangePixelPlayer == null) {
            Log.LogError("Failed to find PlayerMain2D.ChangePixelPlayer");
            return;
        }
        if (originalPlayerRabbit2D_ChangePixelPlayer == null) {
            Log.LogError("Failed to find PlayerRabbit2D.ChangePixelPlayer");
            return;
        }
        var pre_ChangePixelPlayer = AccessTools.Method(typeof(RemoveVoxelPlayerPlugin), "Hook_ChangePixelPlayer");
        _harmony.Patch(originalPlayerMain2D_ChangePixelPlayer, prefix: new HarmonyMethod(pre_ChangePixelPlayer));
        _harmony.Patch(originalPlayerRabbit2D_ChangePixelPlayer, prefix: new HarmonyMethod(pre_ChangePixelPlayer));
    }

    public static bool Hook_ChangePixelPlayer(ref GmkMorphZone.EMorphType type) {
        if (type == GmkMorphZone.EMorphType.PIXEL && _config.RemovePlayerPixelModel) {
            type = GmkMorphZone.EMorphType.NORMAL;
        }
        else if (type == GmkMorphZone.EMorphType.JELLYFISH && _config.RemoveJellyfishModel) {
            return false;
        }
        else if (type == GmkMorphZone.EMorphType.MOUSE && _config.RemoveMouseModel) {
            return false;
        }
        else if (type == GmkMorphZone.EMorphType.ROCKET && _config.RemoveRocketModel) {
            return false;
        }
        return true;
    }
}
