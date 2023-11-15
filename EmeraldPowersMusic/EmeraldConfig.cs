using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace EmeraldPowersMusic;
public class EmeraldConfig {
    public bool AvatarMusic { get; set; }
    public bool BulletMusic { get; set; }
    public bool VisionMusic { get; set; }
    public bool WaterMusic { get; set; }
    public bool IvyMusic { get; set; }
    public bool SlowMusic { get; set; }
    public bool ExtraMusic { get; set; }
    public bool SuperMusic { get; set; }
    public bool AvatarStartupSound { get; set; }
    public bool BulletUsageSound { get; set; }
    public bool IvyUsageSound { get; set; }
    public bool SlowStartupSound { get; set; }
    public bool VisionStartupSound { get; set; }

    public static EmeraldConfig Load() {
        try {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EmeraldMusicConfig.txt");
            if (!File.Exists(path)) return new EmeraldConfig();
            return JsonSerializer.Deserialize<EmeraldConfig>(File.ReadAllBytes(path));
        }
        catch (Exception ex) {
            Console.WriteLine("Failed to read EmeraldMusicConfig.txt");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Using default values");
        }
        return new EmeraldConfig();
    }
}
