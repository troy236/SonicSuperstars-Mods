using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using OriPlayerAction;

namespace SuperSonicDashNoRings;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SuperSonicDashNoRingsPlugin : BasePlugin {
    private static Harmony _harmony;

    public override void Load() {
        _harmony = new Harmony("Superstars.SuperSonicDashNoRings");
        var original_PlActionSuperAttack_Init = AccessTools.Method(typeof(PlActionSuperAttack), "Init");
        if (original_PlActionSuperAttack_Init == null) {
            Log.LogError("Failed to find PlActionSuperAttack.Init");
            return;
        }
        var pre_ActionSuperAttack_Init = AccessTools.Method(typeof(SuperSonicDashNoRingsPlugin), "Hook_ActionSuperAttack_Init");
        _harmony.Patch(original_PlActionSuperAttack_Init, prefix: new HarmonyMethod(pre_ActionSuperAttack_Init));
    }

    public static void Hook_ActionSuperAttack_Init() {
        var gameSceneControllerInstance = GameSceneController.Instance;
        if (gameSceneControllerInstance is null) return;
        var player = gameSceneControllerInstance.GetPlayerID(0);
        if (player is null) return;
        player.AddRingCnt(5);
    }
}
