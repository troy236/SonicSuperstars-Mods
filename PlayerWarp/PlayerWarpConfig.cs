using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using arz.input;

namespace PlayerWarp;
public class PlayerWarpConfig {
    public PlatformPad.Button WarpEnumCombo;
    public string WarpCombo { get; set; }
    public bool CycleThroughCheckpoints { get; set; }
    public bool ClearWarpOnRestartAct { get; set; }
    public bool ReloadConfigOnChange { get; set; }
    public string Bridge_Island_Act1_WarpPosition { get; set; }
    public string Bridge_Island_Act2_WarpPosition { get; set; }
    public string Speed_Jungle_Act1_WarpPosition { get; set; }
    public string Speed_Jungle_ActSonic_WarpPosition { get; set; }
    public string Speed_Jungle_Act2_WarpPosition { get; set; }
    public string Sky_Temple_Act1_WarpPosition { get; set; }
    public string Pinball_Carnival_Act1_WarpPosition { get; set; }
    public string Pinball_Carnival_Act2_WarpPosition { get; set; }
    public string Lagoon_City_Act1_WarpPosition { get; set; }
    public string Lagoon_City_ActAmy_WarpPosition { get; set; }
    public string Lagoon_City_Act2_WarpPosition { get; set; }
    public string Sand_Sanctuary_Act1_WarpPosition { get; set; }
    public string Press_Factory_Act1_WarpPosition { get; set; }
    public string Press_Factory_Act2_WarpPosition { get; set; }
    public string Golden_Capital_Act1_WarpPosition { get; set; }
    public string Golden_Capital_ActKnuckles_WarpPosition { get; set; }
    public string Golden_Capital_Act2_WarpPosition { get; set; }
    public string Cyber_Station_Act1_WarpPosition { get; set; }
    public string Frozen_Base_Act1_WarpPosition { get; set; }
    public string Frozen_Base_ActTails_WarpPosition { get; set; }
    public string Frozen_Base_Act2_WarpPosition { get; set; }
    public string Egg_Fortress_Act1_WarpPosition { get; set; }
    public string Egg_Fortress_Act2_WarpPosition { get; set; }
    public string Trip_Bridge_Island_Act1_WarpPosition { get; set; }
    public string Trip_Bridge_Island_Act2_WarpPosition { get; set; }
    public string Trip_Speed_Jungle_Act1_WarpPosition { get; set; }
    public string Trip_Speed_Jungle_Act2_WarpPosition { get; set; }
    public string Trip_Speed_Jungle_Act3_WarpPosition { get; set; }
    public string Trip_Sky_Temple_Act1_WarpPosition { get; set; }
    public string Trip_Pinball_Carnival_Act1_WarpPosition { get; set; }
    public string Trip_Pinball_Carnival_Act2_WarpPosition { get; set; }
    public string Trip_Lagoon_City_Act1_WarpPosition { get; set; }
    public string Trip_Lagoon_City_Act2_WarpPosition { get; set; }
    public string Trip_Lagoon_City_Act3_WarpPosition { get; set; }
    public string Trip_Sand_Sanctuary_Act1_WarpPosition { get; set; }
    public string Trip_Press_Factory_Act1_WarpPosition { get; set; }
    public string Trip_Press_Factory_Act2_WarpPosition { get; set; }
    public string Trip_Golden_Capital_Act1_WarpPosition { get; set; }
    public string Trip_Golden_Capital_Act2_WarpPosition { get; set; }
    public string Trip_Golden_Capital_Act3_WarpPosition { get; set; }
    public string Trip_Cyber_Station_Act1_WarpPosition { get; set; }
    public string Trip_Frozen_Base_Act1_WarpPosition { get; set; }
    public string Trip_Frozen_Base_Act2_WarpPosition { get; set; }
    public string Trip_Frozen_Base_Act3_WarpPosition { get; set; }
    public string Trip_Egg_Fortress_Act1_WarpPosition { get; set; }
    public string Trip_Egg_Fortress_Act2_WarpPosition { get; set; }
    public string Last_Story_WarpPosition { get; set; }

    public static PlayerWarpConfig Load(BepInEx.Logging.ManualLogSource log, out bool succeeded) {
        succeeded = false;
        try {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PlayerWarpConfig.txt");
            if (!File.Exists(path)) return GetDefault();
            var playerWarpConfig = JsonSerializer.Deserialize<PlayerWarpConfig>(File.ReadAllBytes(path), new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });
            if (!string.IsNullOrEmpty(playerWarpConfig.WarpCombo)) {
                foreach (var button in playerWarpConfig.WarpCombo.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                    if (Enum.TryParse<PlatformPad.Button>(button, true, out var platformPadButton)) {
                        playerWarpConfig.WarpEnumCombo |= platformPadButton;
                    }
                    else {
                        log.LogWarning($"Failed to parse {button} as a button. Ignoring");
                    }
                }
            }
            succeeded = true;
            return playerWarpConfig;
        }
        catch (Exception ex) {
            log.LogError("Failed to read PlayerWarpConfig.txt");
            log.LogError(ex.ToString());
            log.LogError("Using default values");
        }
        return GetDefault();
    }

    private static PlayerWarpConfig GetDefault() {
        var playerWarpConfig = new PlayerWarpConfig();
        playerWarpConfig.WarpEnumCombo = PlatformPad.Button.ZL | PlatformPad.Button.ZR | PlatformPad.Button.RUp;
        playerWarpConfig.CycleThroughCheckpoints = true;
        playerWarpConfig.ClearWarpOnRestartAct = false;
        playerWarpConfig.ReloadConfigOnChange = false;
        playerWarpConfig.Bridge_Island_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Bridge_Island_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Speed_Jungle_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Speed_Jungle_ActSonic_WarpPosition = "0,0,-1";
        playerWarpConfig.Speed_Jungle_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Sky_Temple_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Pinball_Carnival_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Pinball_Carnival_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Lagoon_City_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Lagoon_City_ActAmy_WarpPosition = "0,0,-1";
        playerWarpConfig.Lagoon_City_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Sand_Sanctuary_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Press_Factory_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Press_Factory_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Golden_Capital_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Golden_Capital_ActKnuckles_WarpPosition = "0,0,-1";
        playerWarpConfig.Golden_Capital_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Cyber_Station_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Frozen_Base_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Frozen_Base_ActTails_WarpPosition = "0,0,-1";
        playerWarpConfig.Frozen_Base_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Egg_Fortress_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Egg_Fortress_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Bridge_Island_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Bridge_Island_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Speed_Jungle_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Speed_Jungle_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Speed_Jungle_Act3_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Sky_Temple_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Pinball_Carnival_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Pinball_Carnival_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Lagoon_City_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Lagoon_City_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Lagoon_City_Act3_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Sand_Sanctuary_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Press_Factory_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Press_Factory_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Golden_Capital_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Golden_Capital_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Golden_Capital_Act3_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Cyber_Station_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Frozen_Base_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Frozen_Base_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Frozen_Base_Act3_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Egg_Fortress_Act1_WarpPosition = "0,0,-1";
        playerWarpConfig.Trip_Egg_Fortress_Act2_WarpPosition = "0,0,-1";
        playerWarpConfig.Last_Story_WarpPosition = "0,0,-1";
        return playerWarpConfig;
    }
}
