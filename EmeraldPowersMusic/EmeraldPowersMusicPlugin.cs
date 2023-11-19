using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using static Orion.SoundSourceSettings;

namespace EmeraldPowersMusic;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class EmeraldPowersMusicPlugin : BasePlugin {
    private static Harmony _harmony;
    private static bool _blockChangeVolume = false;
    private static EmeraldMusicConfig _config;

    public override void Load() {
        _harmony = new Harmony("Superstars.EmeraldPowersMusic");
        _config = EmeraldMusicConfig.Load();
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
        var preSoundManager_Play2 = AccessTools.Method(typeof(EmeraldPowersMusicPlugin), "HookSoundManager_Play");
        var preSoundManager_ChangeVolumeAll = AccessTools.Method(typeof(EmeraldPowersMusicPlugin), "HookSoundManager_ChangeVolumeAll");
        _harmony.Patch(originalSoundManager_Play, prefix: new HarmonyMethod(preSoundManager_Play2));
        _harmony.Patch(originalSoundManager_ChangeVolumeAll, prefix: new HarmonyMethod(preSoundManager_ChangeVolumeAll));
    }

    public static bool HookSoundManager_Play(SoundSourceTypes type, int cue) {
        #region Emerald Long Sound Effects
        if (type == SoundSourceTypes.Player) {
            switch (cue) {
                case 30011: //Avatar Sonic
                case 30111: //Avatar Tails
                case 30211: //Avatar Knuckles
                case 30311: //Avatar Amy
                case 30411: //Avatar Trip
                    return _config.AvatarStartupSound;
                case 30009: //Bullet Sonic
                case 30109: //Bullet Tails
                case 30209: //Bullet Knuckles
                case 30309: //Bullet Amy
                case 30409: //Bullet Trip
                    return _config.BulletUsageSound;
                case 30018: //Vision Sonic
                case 30118: //Vision Tails
                case 30218: //Vision Knuckles
                case 30318: //Vision Amy
                case 30418: //Vision Trip
                    return _config.VisionStartupSound;
                case 30022: //Ivy Sonic
                case 30122: //Ivy Tails
                case 30222: //Ivy Knuckles
                case 30322: //Ivy Amy
                case 30422: //Ivy Trip
                    return _config.IvyUsageSound;
                case 30024: //Slow Sonic
                case 30124: //Slow Tails
                case 30224: //Slow Knuckles
                case 30324: //Slow Amy
                case 30424: //Slow Trip
                    return _config.SlowStartupSound;
            }
        }
        #endregion
        if (type == SoundSourceTypes.BgmJingle) {
            switch (cue) {
                case 90039: //Avatar
                    if (_config.AvatarMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90038: //Bullet
                    if (_config.BulletMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90041: //Vision
                    if (_config.VisionMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90044: //Water
                    if (_config.WaterMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90040: //Ivy
                    if (_config.IvyMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90042: //Slow
                    if (_config.SlowMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90043: //Extra
                    if (_config.ExtraMusic) return true;
                    _blockChangeVolume = true;
                    return false;
                case 90022: //Super
                    if (_config.SuperMusic) return true;
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
