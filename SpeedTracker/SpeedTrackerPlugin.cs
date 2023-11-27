﻿using System.Threading;
using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Orion;
using OriDebug;
using OriPlayer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpeedTracker;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SpeedTrackerPlugin : BasePlugin {
    private static ManualLogSource _log;
    private static Harmony _harmony;
    private static bool _firstRun = true;
    private static PlayerBase _playerBase;
    private static Timer _timer;
    private static DebugPlayerSpeedValue _debugPlayerSpeedValue;

    public override void Load() {
        _log = Log;
        _timer = new Timer(UpdatePlayerBase, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2));
        _harmony = new Harmony("Superstars.SpeedTracker");
        var originalGameSceneController_Instance = AccessTools.Method(typeof(GameSceneController), "get_Instance");
        if (originalGameSceneController_Instance == null) {
            Log.LogError("Failed to find GameSceneController.Instance");
            return;
        }
        var postGameSceneController_Instance = AccessTools.Method(typeof(SpeedTrackerPlugin), "HookGameScene_Instance");
        _harmony.Patch(originalGameSceneController_Instance, postfix: new HarmonyMethod(postGameSceneController_Instance));
    }

    public static void UpdatePlayerBase(object state) {
        try {
            if (_debugPlayerSpeedValue is null) return;
            var gameSceneControllerInstance = GameSceneController.Instance;
            if (gameSceneControllerInstance is null) return;
            var playerBase = gameSceneControllerInstance.LeaderPlayer;
            if (playerBase is not null && (playerBase.WasCollected || playerBase != _playerBase)) {
                _playerBase = playerBase;
                _debugPlayerSpeedValue.targetPl = _playerBase;
                //_debugPlayerSpeedValue.rollSpeed = _playerBase.ActDB.RollTopSpeed;
                //_debugPlayerSpeedValue.topSpeed = _playerBase.ActDB.TopSpeed;
            }
        }
        catch (Exception ex) {
            _log.LogError($"Error: {ex.ToString()}");
        }
    }

    public static void HookGameScene_Instance(GameSceneController __result) {
        try {
            if (!_firstRun) return;
            if (__result is null) return;
            _firstRun = false;
            _playerBase = __result.GetPlayer(0);
            var go = new GameObject();
            _debugPlayerSpeedValue = go.AddComponent<DebugPlayerSpeedValue>();
            Object.DontDestroyOnLoad(go);
            _debugPlayerSpeedValue.targetPl = _playerBase;
            //Accessing ActDB fails. Value changes depending on speed shoes/super
            //_debugPlayerSpeedValue.rollSpeed = _playerBase.ActDB.RollTopSpeed;
            //_debugPlayerSpeedValue.topSpeed = _playerBase.ActDB.TopSpeed;

        }
        catch (Exception ex) {
            _log.LogError($"Error: {ex.ToString()}");
        }
    }
}
