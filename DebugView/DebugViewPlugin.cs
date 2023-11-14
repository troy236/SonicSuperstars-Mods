using BepInEx;
using BepInEx.Unity.IL2CPP;
using OriDebug;

namespace DebugView;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class DebugViewPlugin : BasePlugin {

    public override void Load() {
        DebugInfo.infomationEnable = true;
        Log.LogInfo("Debug view enabled");
    }
}
