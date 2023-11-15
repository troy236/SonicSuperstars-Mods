using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using static Orion.SoundSourceSettings;

namespace EmeraldPowersMusic;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EmeraldPowersMusicPlugin : BasePlugin {
    private static bool _blockChangeVolume = false;

    public override void Load() {
        var harmony = new Harmony("Superstars.EmeraldPowersMusic");
        var originalSoundManager_Play = AccessTools.Method(typeof(SoundManager), "Play", new[] { typeof(SoundSourceTypes), typeof(string), typeof(int), typeof(SoundSourceSettings) });
        var originalSoundManager_ChangeVolumeAll = AccessTools.Method(typeof(SoundManager), "ChangeVolumeAll");
        if (originalSoundManager_Play == null) {
            Log.LogError("Failed to find SoundManager.Play");
            return;
        }
        if (originalSoundManager_ChangeVolumeAll == null) {
            Log.LogError("Failed to find SoundManager.ChangeVolumeAll");
            return;
        }
        var postSoundManager_Play2 = AccessTools.Method(typeof(EmeraldPowersMusicPlugin), "HookSoundManager_Play");
        var postSoundManager_ChangeVolumeAll = AccessTools.Method(typeof(EmeraldPowersMusicPlugin), "HookSoundManager_ChangeVolumeAll");
        harmony.Patch(originalSoundManager_Play, prefix: new HarmonyMethod(postSoundManager_Play2));
        harmony.Patch(originalSoundManager_ChangeVolumeAll, prefix: new HarmonyMethod(postSoundManager_ChangeVolumeAll));
    }


    public static bool HookSoundManager_Play(SoundSourceTypes type, int cue) {
        //Comment this region out to have the sound effects play
        #region Emerald Long Sound Effects
        if (type == SoundSourceTypes.Player) {
            switch (cue) {
                case 30411: //Avatar
                    return false;
                case 30418: //Vision
                    return false;
                case 30422: //Ivy
                    return false;
                case 30424: //Slow
                    return false;
            }
        }
        #endregion
        if (type == SoundSourceTypes.BgmJingle) {
            switch (cue) {
                case 90039: //Avatar
                    _blockChangeVolume = true;
                    return false;
                case 90038: //Bullet
                    _blockChangeVolume = true;
                    return false;
                case 90041: //Vision
                    _blockChangeVolume = true;
                    return false;
                case 90044: //Water
                    _blockChangeVolume = true;
                    return false;
                case 90040: //Ivy
                    _blockChangeVolume = true;
                    return false;
                case 90042: //Slow
                    _blockChangeVolume = true;
                    return false;
                case 90043: //Extra
                    _blockChangeVolume = true;
                    return false;
                case 90022: //Super
                    _blockChangeVolume = true;
                    return false;
            }
        }
        return true;
    }

    public static bool HookSoundManager_ChangeVolumeAll() {
        if (_blockChangeVolume) {
            _blockChangeVolume = false;
            return false;
        }
        return true;
    }
}
