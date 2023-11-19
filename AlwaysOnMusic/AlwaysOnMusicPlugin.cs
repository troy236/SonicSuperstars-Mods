using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;

namespace AlwaysOnMusic;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class AlwaysOnMusicPlugin : BasePlugin {
    private static Harmony _harmony;

    public override void Load() {
        _harmony = new Harmony("Superstars.AlwaysOnMusic");
        var originalSoundManager_OnApplicationFocus = AccessTools.Method(typeof(SoundManager), "OnApplicationFocus");
        var originalSysPauseManager_OnApplicationFocus = AccessTools.Method(typeof(SysPauseManager), "OnApplicationFocus");
        if (originalSoundManager_OnApplicationFocus == null) {
            Log.LogError("Failed to find SoundManager.OnApplicationFocus");
            return;
        }
        if (originalSysPauseManager_OnApplicationFocus == null) {
            Log.LogError("Failed to find SysPauseManager.OnApplicationFocus");
            return;
        }
        var pre_OnApplicationFocus = AccessTools.Method(typeof(AlwaysOnMusicPlugin), "Hook_OnApplicationFocus");
        _harmony.Patch(originalSoundManager_OnApplicationFocus, prefix: new HarmonyMethod(pre_OnApplicationFocus));
        _harmony.Patch(originalSysPauseManager_OnApplicationFocus, prefix: new HarmonyMethod(pre_OnApplicationFocus));
    }

    public static bool Hook_OnApplicationFocus() {
        return false;
    }
}
