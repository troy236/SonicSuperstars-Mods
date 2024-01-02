using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Orion;
using OriUI;
using UnityEngine.SceneManagement;

namespace ChangeCharacterBetweenActs;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ChangeCharacterBetweenActsPlugin : BasePlugin {
    private static Harmony _harmony;
    private static bool _allowSeamless;
    private static bool _getRealSeamlessResult;

    public override void Load() {
        _harmony = new Harmony("Superstars.ChangeCharacterBetweenActs");
        var Il2CppToMonoDelegateReferencetype = typeof(DelegateSupport).GetNestedType("Il2CppToMonoDelegateReference", System.Reflection.BindingFlags.NonPublic);
        if (Il2CppToMonoDelegateReferencetype != null) {
            ClassInjector.RegisterTypeInIl2Cpp(Il2CppToMonoDelegateReferencetype);
        }

        var originalUIGameMainResult_UpdateOpenResultMenu = AccessTools.Method(typeof(UIGameMainResult), "Update");
        var originalGameSceneControllerBase_IsSeamlessAct = AccessTools.Method(typeof(GameSceneControllerBase), "IsSeamlessAct");
        var originalUIGameMain_OpenMainResultUI = AccessTools.Method(typeof(UIGameMain), "OpenMainResultUI");
        var originalUIGameMainResult_SetNextActScene = AccessTools.Method(typeof(UIGameMainResult), "SetNextActScene");
        if (originalUIGameMainResult_UpdateOpenResultMenu == null) {
            Log.LogError("Failed to find UIGameMainResult.Update");
            return;
        }
        if (originalGameSceneControllerBase_IsSeamlessAct == null) {
            Log.LogError("Failed to find GameSceneControllerBase.IsSeamlessAct");
            return;
        }
        if (originalUIGameMain_OpenMainResultUI == null) {
            Log.LogError("Failed to find UIGameMain.OpenMainResultUI");
            return;
        }
        if (originalUIGameMainResult_SetNextActScene == null) {
            Log.LogError("Failed to find UIGameMainResult.SetNextActScene");
            return;
        }
        var pre_UpdateOpenResultMenu = AccessTools.Method(typeof(ChangeCharacterBetweenActsPlugin), "Hook_Update");
        var pre_IsSeamlessAct = AccessTools.Method(typeof(ChangeCharacterBetweenActsPlugin), "Hook_IsSeamlessAct");
        var pre_OpenMainResultUI = AccessTools.Method(typeof(ChangeCharacterBetweenActsPlugin), "Hook_OpenMainResultUI");
        var pre_SetNextActScene = AccessTools.Method(typeof(ChangeCharacterBetweenActsPlugin), "Hook_SetNextActScene");
        _harmony.Patch(originalUIGameMainResult_UpdateOpenResultMenu, prefix: new HarmonyMethod(pre_UpdateOpenResultMenu));
        _harmony.Patch(originalGameSceneControllerBase_IsSeamlessAct, postfix: new HarmonyMethod(pre_IsSeamlessAct));
        _harmony.Patch(originalUIGameMain_OpenMainResultUI, prefix: new HarmonyMethod(pre_OpenMainResultUI));
        _harmony.Patch(originalUIGameMainResult_SetNextActScene, prefix: new HarmonyMethod(pre_SetNextActScene));
    }

    public static void Hook_Update() {
        if (UIDefine.IsAnyPadTriggerButton(SysSaveDataKeyboardConfig.EAction.MenuCancel)) {
            if (SysGameManager.Instance.gameMode != SysGameManager.GameMode.NormalMode) return;
            if (UnityEngine.Object.FindAnyObjectByType<UICharacterSelectWindow>() is not null) return;
            SysDialogManager.Instance.CreateCharacterSelectWindow(SysSaveManager.Instance.CurrentSlotData.IsNotificationOpenTripMode,
                (Il2CppSystem.Action<int, UICharacterSelectWindow.TransferSelectdAllCharaData>)CharacterOnSelect);
        }
    }

    private static void CharacterOnSelect(int index, UICharacterSelectWindow.TransferSelectdAllCharaData characterData) {
        if (characterData is null || characterData.selectedData is null) return;
        var gameSceneControllerInstance = GameSceneController.Instance;
        var userControlDatas = gameSceneControllerInstance.PassingData.UserControlDatas;
        var activeGamePlayerNameList = gameSceneControllerInstance.activeGamePlayerNameList;
        for (int i = 0; i < characterData.selectedData.Length; i++) {
            if (characterData.selectedData[i].useCharacterID == -1) { continue; }
            if (userControlDatas.Length - 1 < i) { continue; }
            switch (characterData.selectedData[i].useCharacterID) {
                case 0:
                    userControlDatas[i].CharaID = 0;
                    activeGamePlayerNameList[i] = "Ply_Son_M00.prefab";
                    break;
                case 1:
                    userControlDatas[i].CharaID = 1;
                    activeGamePlayerNameList[i] = "Ply_Tai_M00.prefab";
                    break;
                case 2:
                    userControlDatas[i].CharaID = 2;
                    activeGamePlayerNameList[i] = "Ply_Knu_M00.prefab";
                    break;
                case 3:
                    userControlDatas[i].CharaID = 3;
                    activeGamePlayerNameList[i] = "Ply_Amy_M00.prefab";
                    break;
                case 4:
                    userControlDatas[i].CharaID = 4;
                    activeGamePlayerNameList[i] = "Ply_Bri_M00.prefab";
                    break;
                default:
                    break;
            }
        }
        _allowSeamless = false;
        Player_ResidentCache.PlayerLoad_Passing(gameSceneControllerInstance.PassingData);
        Player_ResidentCache.PlayerLoad_Start();
    }

    public static void Hook_IsSeamlessAct(ref bool __result) {
        if (_getRealSeamlessResult) return;
        if (__result) {
            __result = _allowSeamless;
        }
    }

    public static void Hook_OpenMainResultUI() {
        _allowSeamless = true;
    }

    public static void Hook_SetNextActScene(ref string nextActName) {
        if (nextActName == null && _allowSeamless == false) {
            //If nextActName is null then character/fruit dialog was not used and is using Act data to determine what Act to load next
            _getRealSeamlessResult = true;
            //Check if current Act is a Seamless Act
            bool isSeamless = GameSceneController.Instance.IsSeamlessAct();
            _getRealSeamlessResult = false;
            if (isSeamless) {
                //Seamless Acts seem to be missing data for what Act to go to after results if not using the Seamless transition.
                //The failsafe warps to World Map
                //The following tells the game which Act to load next
                var sceneName = SceneManager.GetActiveScene().name;
                //3 Known Seamless Acts currently
                switch (sceneName) {
                    case "Zone01_Act1":
                        nextActName = "Zone01_Act2";
                        break;
                    case "Zone01_Act1_Trip":
                        nextActName = "Zone01_Act2_Trip";
                        break;
                    case "Zone04_Act1":
                        nextActName = "Zone04_Act2";
                        break;
                    case "Zone04_Act1_Trip":
                        nextActName = "Zone04_Act2_Trip";
                        break;
                    case "Zone11_Act1":
                        nextActName = "Zone11_Act2";
                        break;
                    case "Zone11_Act1_Trip":
                        nextActName = "Zone11_Act2_Trip";
                        break;
                }
            }
        }
    }
}
