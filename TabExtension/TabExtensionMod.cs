using System.Collections;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;

using TabExtension;
using TabExtension.UI;
using TabExtension.Config;

[assembly: MelonInfo(typeof(TabExtensionMod), "TabExtension", "1.3.0", "DragonPlayer", "https://github.com/DragonPlayerX/TabExtension")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonOptionalDependencies("UIExpansionKit")]

namespace TabExtension
{
    public class TabExtensionMod : MelonMod
    {
        public static readonly string Version = "1.3.0";

        public static TabExtensionMod Instance { get; private set; }
        public static MelonLogger.Instance Logger => Instance.LoggerInstance;

        public override void OnApplicationStart()
        {
            Instance = this;
            Logger.Msg("Initializing TabExtension " + Version + "...");

            Configuration.Init();

            ClassInjector.RegisterTypeInIl2Cpp<LayoutListener>();
            ClassInjector.RegisterTypeInIl2Cpp<TabLayout>();

            MelonCoroutines.Start(Init());
        }

        private IEnumerator Init()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;
            while (GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent") == null) yield return null;

            GameObject quickMenu = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)").gameObject;
            GameObject layout = quickMenu.transform.Find("Container/Window/Page_Buttons_QM/HorizontalLayoutGroup").gameObject;

            layout.AddComponent<TabLayout>();

            Logger.Msg("Running version " + Version + " of TabExtension.");
        }
    }
}
