using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnucklesGlideSpeed;
public class KnucklesGlideSpeedConfig {
    public float GlideSpeed { get; set; }
    public bool UseStartXSpeed { get; set; }
    public bool GlideAcceleration { get; set; }

    public static KnucklesGlideSpeedConfig Load(BepInEx.Logging.ManualLogSource log) {
        try {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "KnucklesGlideSpeedConfig.txt");
            if (!File.Exists(path)) return GetDefault();
            return JsonSerializer.Deserialize<KnucklesGlideSpeedConfig>(File.ReadAllBytes(path), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
        }
        catch (Exception ex) {
            log.LogError("Failed to read KnucklesGlideSpeedConfig.txt");
            log.LogError(ex.ToString());
            log.LogError("Using default values");
        }
        return GetDefault();
    }

    private static KnucklesGlideSpeedConfig GetDefault() {
        var knucklesGlideSpeedConfig = new KnucklesGlideSpeedConfig();
        knucklesGlideSpeedConfig.GlideSpeed = 3f;
        knucklesGlideSpeedConfig.UseStartXSpeed = true;
        knucklesGlideSpeedConfig.GlideAcceleration = false;
        return knucklesGlideSpeedConfig;
    }
}
