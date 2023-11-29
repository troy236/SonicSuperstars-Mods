using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace RemoveVoxelPlayer;
public class RemoveVoxelPlayerConfig {
    public bool RemovePlayerPixelModel { get; set; }
    public bool RemoveJellyfishModel { get; set; }
    public bool RemoveMouseModel { get; set; }
    public bool RemoveRocketModel { get; set; }

    public static RemoveVoxelPlayerConfig Load(BepInEx.Logging.ManualLogSource log) {
        try {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "RemoveVoxelPlayerConfig.txt");
            if (!File.Exists(path)) return GetDefault();
            var removeVoxelPlayerConfig = JsonSerializer.Deserialize<RemoveVoxelPlayerConfig>(File.ReadAllBytes(path), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
            return removeVoxelPlayerConfig;
        }
        catch (Exception ex) {
            log.LogError("Failed to read RemoveVoxelPlayerConfig.txt");
            log.LogError(ex.ToString());
            log.LogError("Using default values");
        }
        return GetDefault();
    }

    private static RemoveVoxelPlayerConfig GetDefault() {
        var removeVoxelPlayerConfig = new RemoveVoxelPlayerConfig();
        removeVoxelPlayerConfig.RemovePlayerPixelModel = true;
        removeVoxelPlayerConfig.RemoveJellyfishModel = false;
        removeVoxelPlayerConfig.RemoveMouseModel = false;
        removeVoxelPlayerConfig.RemoveRocketModel = false;
        return removeVoxelPlayerConfig;
    }
}
