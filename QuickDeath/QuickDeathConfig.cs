using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using arz.input;

namespace QuickDeath;
public class QuickDeathConfig {
    public PlatformPad.Button QuickDeathEnumCombo;
    public PlatformPad.Button QuickRestartEnumCombo;
    public string QuickDeathCombo { get; set; }
    public string QuickRestartCombo { get; set; }

    public static QuickDeathConfig Load(BepInEx.Logging.ManualLogSource log) {
        try {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "QuickDeathConfig.txt");
            if (!File.Exists(path)) return GetDefault();
            var quickDeathConfig = JsonSerializer.Deserialize<QuickDeathConfig>(File.ReadAllBytes(path), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
            if (!string.IsNullOrEmpty(quickDeathConfig.QuickDeathCombo)) {
                foreach (var button in quickDeathConfig.QuickDeathCombo.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                    if (Enum.TryParse<PlatformPad.Button>(button, true, out var platformPadButton)) {
                        quickDeathConfig.QuickDeathEnumCombo |= platformPadButton;
                    }
                    else {
                        log.LogWarning($"Failed to parse {button} as a button. Ignoring");
                    }
                }
            }
            if (!string.IsNullOrEmpty(quickDeathConfig.QuickRestartCombo)) {
                foreach (var button in quickDeathConfig.QuickRestartCombo.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                    if (Enum.TryParse<PlatformPad.Button>(button, true, out var platformPadButton)) {
                        quickDeathConfig.QuickRestartEnumCombo |= platformPadButton;
                    }
                    else {
                        log.LogWarning($"Failed to parse {button} as a button. Ignoring");
                    }
                }
            }
            return quickDeathConfig;
        }
        catch (Exception ex) {
            log.LogError("Failed to read QuickDeathConfig.txt");
            log.LogError(ex.ToString());
            log.LogError("Using default values");
        }
        return GetDefault();
    }

    private static QuickDeathConfig GetDefault() {
        var quickDeathConfig = new QuickDeathConfig();
        quickDeathConfig.QuickDeathEnumCombo = PlatformPad.Button.ZL | PlatformPad.Button.ZR | PlatformPad.Button.L | PlatformPad.Button.R;
        quickDeathConfig.QuickRestartEnumCombo = PlatformPad.Button.ZL | PlatformPad.Button.ZR | PlatformPad.Button.RDown | PlatformPad.Button.RLeft;
        return quickDeathConfig;
    }
}
