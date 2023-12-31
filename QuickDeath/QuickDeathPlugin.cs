﻿using System.Linq;
using System.Reflection;
using arz.input;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using OriPlayer;

namespace QuickDeath;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class QuickDeathPlugin : BasePlugin {
    private static Harmony _harmony;
    private static QuickDeathConfig _config;
    private static bool _Triggered = false;
    private static System.Threading.Timer _timer;
    private static MethodInfo SetSandwichedDeadMethod;

    public override void Load() {
        _harmony = new Harmony("Superstars.QuickDeath");
        _config = QuickDeathConfig.Load(Log);
        if (_config.QuickDeathEnumCombo == PlatformPad.Button.None && _config.QuickRestartEnumCombo == PlatformPad.Button.None) {
            Log.LogInfo("No button combo assigned. Exiting");
            return;
        }
        if (_config.QuickDeathEnumCombo != PlatformPad.Button.None) {
            Log.LogInfo($"Quick Death combo assigned to: {_config.QuickDeathEnumCombo}");
        }
        if (_config.QuickRestartEnumCombo != PlatformPad.Button.None) {
            Log.LogInfo($"Quick Restart combo assigned to: {_config.QuickRestartEnumCombo}");
        }
        SetSandwichedDeadMethod = typeof(PlayerMain2D).GetMethod("SetSandwichedDead");
        var originalPlayerMain2D_LateUpdate = AccessTools.Method(typeof(PlayerMain2D), "LateUpdate");
        var originalPlayerBase3D_UpdateMove = AccessTools.Method(typeof(PlayerBase3D), "UpdateMove");
        if (originalPlayerMain2D_LateUpdate == null) {
            Log.LogError("Failed to find PlayerMain2D.LateUpdate");
            return;
        }
        if (originalPlayerBase3D_UpdateMove == null) {
            Log.LogError("Failed to find PlayerBase3D.UpdateMove");
            return;
        }
        var post_LateUpdate = AccessTools.Method(typeof(QuickDeathPlugin), "Hook_LateUpdate");
        var pre_UpdateMove = AccessTools.Method(typeof(QuickDeathPlugin), "Hook_UpdateMove");
        _harmony.Patch(originalPlayerMain2D_LateUpdate, postfix: new HarmonyMethod(post_LateUpdate));
        _harmony.Patch(originalPlayerBase3D_UpdateMove, prefix: new HarmonyMethod(pre_UpdateMove));
    }

    public static void Hook_LateUpdate(PlayerMain2D __instance) {
        if (__instance.playerID == -1) return;
        if (_Triggered) {
            return;
        }
        var inputPlayerController = __instance.inputPlyCtlr;
        if (inputPlayerController is null) return;
        var playerPad = inputPlayerController.playerPad;
        if (playerPad is null) return;
        var nowPad = playerPad.nowPad;
        if (nowPad is null) return;
        var onButton = nowPad.On;
        if (_config.QuickDeathEnumCombo != PlatformPad.Button.None) {
            if ((onButton & _config.QuickDeathEnumCombo) == _config.QuickDeathEnumCombo) {
                int parameterLength = SetSandwichedDeadMethod.GetParameters().Length;
                if (parameterLength == 0) {
                    SetSandwichedDeadMethod.Invoke(__instance, null);
                }
                else {
                    //Current Dec patch has 1 optional bool parameter. This could support more optional parameters in the future
                    SetSandwichedDeadMethod.Invoke(__instance, Enumerable.Repeat(System.Type.Missing, parameterLength).ToArray());
                }
                _Triggered = true;
                _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(3), System.Threading.Timeout.InfiniteTimeSpan);
            }
        }
        if (_config.QuickRestartEnumCombo != PlatformPad.Button.None) {
            if ((onButton & _config.QuickRestartEnumCombo) == _config.QuickRestartEnumCombo) {
                GameSceneControllerBase.ReStart();
                _Triggered = true;
                _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(3), System.Threading.Timeout.InfiniteTimeSpan);
            }
        }
    }

    public static void Hook_UpdateMove(PlayerBase3D __instance) {
        if (__instance.playerID == -1) return;
        if (_Triggered) {
            return;
        }
        var inputPlayerController = __instance.inputPlyCtlr;
        if (inputPlayerController is null) return;
        var playerPad = inputPlayerController.playerPad;
        if (playerPad is null) return;
        var nowPad = playerPad.nowPad;
        if (nowPad is null) return;
        var onButton = nowPad.On;
        if (_config.QuickDeathEnumCombo != PlatformPad.Button.None) {
            if ((onButton & _config.QuickDeathEnumCombo) == _config.QuickDeathEnumCombo) {
                __instance.crntAction = PlayerBase.EActionIndex.ActDead;
                _Triggered = true;
                _timer = new System.Threading.Timer(ResetTriggered, null, System.TimeSpan.FromSeconds(3), System.Threading.Timeout.InfiniteTimeSpan);
            }
        }
    }

    private static void ResetTriggered(object state) {
        _timer.Dispose();
        _Triggered = false;
    }
}
