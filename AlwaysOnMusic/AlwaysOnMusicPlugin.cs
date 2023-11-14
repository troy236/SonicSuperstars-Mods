using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;

namespace AlwaysOnMusic;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class AlwaysOnMusicPlugin : BasePlugin {
    public override void Load() {
        var harmony = new Harmony("Superstars.AlwaysOnMusic");
        var originalSoundManager_OnApplicationFocus = AccessTools.Method(typeof(SoundManager), "OnApplicationFocus");
        if (originalSoundManager_OnApplicationFocus == null) {
            Log.LogError("Failed to find SoundManager.OnApplicationFocus");
            return;
        }
        var postSoundManager_OnApplicationFocus = AccessTools.Method(typeof(AlwaysOnMusicPlugin), "HookSoundManager_OnApplicationFocus");
        harmony.Patch(originalSoundManager_OnApplicationFocus, prefix: new HarmonyMethod(postSoundManager_OnApplicationFocus));
    }

    public static bool HookSoundManager_OnApplicationFocus() {
        return false;
    }
}
