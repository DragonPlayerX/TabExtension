using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;

using TabExtension.Config;

namespace TabExtension.UI
{
    public class TabLayout : MonoBehaviour
    {
        public static TabLayout Instance;

        private GameObject quickMenu;
        private GameObject layout;
        private RectTransform tooltipRect;
        private RectTransform backgroundRect;
        private BoxCollider menuCollider;
        private List<RectTransform> uixObjects;

        private Dictionary<string, int> tabSorting;
        private List<string> defaultSorting;
        private MenuStateController menuStateController;

        private bool useStyletor;

        public TabLayout(IntPtr value) : base(value)
        {
            Instance = this;

            tabSorting = Configuration.Load();

            if (tabSorting == null)
                tabSorting = new Dictionary<string, int>();

            Configuration.TabSorting.OnValueChanged += new Action<bool, bool>((oldValue, newValue) =>
            {
                if (newValue)
                {
                    tabSorting = Configuration.Load();

                    if (tabSorting == null)
                        tabSorting = new Dictionary<string, int>();

                    ApplySorting();
                }
                else
                {
                    ApplySorting(true);
                }
            });

            Configuration.TabsPerRow.OnValueChanged += new Action<int, int>((oldValue, newValue) => MelonCoroutines.Start(RecalculateLayout()));

            quickMenu = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)").gameObject;
            layout = quickMenu.transform.Find("Container/Window/Page_Buttons_QM/HorizontalLayoutGroup").gameObject;
            tooltipRect = quickMenu.transform.Find("Container/Window/ToolTipPanel").GetComponent<RectTransform>();

            DestroyImmediate(layout.GetComponent<HorizontalLayoutGroup>());

            GameObject background = quickMenu.transform.Find("Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Background_QM_PagePanel").gameObject;
            background.SetActive(Configuration.TabBackground.Value);
            Configuration.TabBackground.OnValueChanged += new Action<bool, bool>((oldValue, newValue) => background.SetActive(newValue));

            if (MelonHandler.Mods.Any(mod => mod.Info.Name.Equals("Styletor")))
            {
                useStyletor = true;
                StyleElement styleElement = background.GetComponent<StyleElement>();
                styleElement.field_Public_String_1 = "TabBottom";

                TabExtensionMod.Logger.Msg("Found Styletor. Style tag was applied to the tab background.");
            }
            else
            {
                Sprite tabSprite = null;
                StyleEngine styleEngine = quickMenu.GetComponent<StyleEngine>();
                Il2CppSystem.Collections.Generic.List<StyleResource.Resource> resources = styleEngine.field_Public_StyleResource_0.resources;
                for (int i = 0; i < resources.Count; i++)
                {
                    if (resources[i].obj.name.Equals("Page_Tab_Backdrop") && resources[i].obj.GetIl2CppType() == UnhollowerRuntimeLib.Il2CppType.Of<Sprite>())
                        tabSprite = resources[i].obj.Cast<Sprite>();
                }

                if (tabSprite != null)
                    TabExtensionMod.Logger.Msg("Found sprite: " + tabSprite.name);
                else
                    TabExtensionMod.Logger.Warning("Unable to find the Page_Tab_Backdrop sprite.");

                Image image = background.GetComponent<Image>();
                image.sprite = tabSprite;
                image.color = new Color(1, 1, 1, 0.8f);
            }

            backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchoredPosition = new Vector2(0, -64);
            backgroundRect.sizeDelta = new Vector2(950, 128);

            menuStateController = quickMenu.GetComponent<MenuStateController>();
        }

        internal void OnEnable() => MelonCoroutines.Start(RecalculateLayout());

        [method: HideFromIl2Cpp]
        public IEnumerator RecalculateLayout()
        {
            if (uixObjects == null)
            {
                uixObjects = new List<RectTransform>();

                foreach (var t in quickMenu.transform.Find("Container").transform)
                {
                    Transform child = t.Cast<Transform>();
                    if (child.gameObject.name.StartsWith("QuickMenuExpandoRoot"))
                        uixObjects.Add(child.Find("Content").GetComponent<RectTransform>());
                }
            }

            if (menuCollider == null)
            {
                // Wait to avoid BoxCollider == null 
                yield return null;

                // Extra wait for late initialized tabs
                yield return null;
                yield return null;

                menuCollider = quickMenu.transform.Find("Container/Window/Page_Buttons_QM").GetComponent<BoxCollider>();

                foreach (var t in transform)
                {
                    Transform child = t.Cast<Transform>();

                    if (child.gameObject.name != "Background_QM_PagePanel")
                        child.gameObject.AddComponent<LayoutListener>();
                }
            }

            List<Transform> childs = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                Transform child = t.Cast<Transform>();

                if (child.gameObject.activeSelf && child.gameObject.name != "Background_QM_PagePanel")
                    childs.Add(child);
            }

            int pivotX = 0;

            for (int i = 0; i < childs.Count; i++)
            {
                int y = i / Configuration.TabsPerRow.Value;
                int x = i - (y * Configuration.TabsPerRow.Value);

                if (x == 0)
                {
                    if (Configuration.ParsedTabAlignment == Configuration.Alignment.Left)
                        pivotX = -(Configuration.TabsPerRow.DefaultValue * 64 - (Configuration.TabsPerRow.DefaultValue % 2 * 64));
                    else if (Configuration.ParsedTabAlignment == Configuration.Alignment.Right)
                        pivotX = Configuration.TabsPerRow.DefaultValue * 64 + (Configuration.TabsPerRow.DefaultValue % 2 * 64) - (((childs.Count - i) >= Configuration.TabsPerRow.Value ? Configuration.TabsPerRow.Value : (childs.Count - i)) * 128);
                    else
                        pivotX = -(((childs.Count - i) >= Configuration.TabsPerRow.Value ? Configuration.TabsPerRow.Value : (childs.Count - i)) * 64) + 64;
                }

                if (childs.Count > i)
                {
                    RectTransform rect = childs[i].transform.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(pivotX + x * 128, -(y * 128));
                    rect.pivot = new Vector2(0.5f, 1);
                }
            }

            int backgroundPivotX = 0;
            if (Configuration.ParsedTabAlignment == Configuration.Alignment.Left)
                backgroundPivotX -= (Configuration.TabsPerRow.DefaultValue - Configuration.TabsPerRow.Value) * 64;
            else if (Configuration.ParsedTabAlignment == Configuration.Alignment.Right)
                backgroundPivotX += (Configuration.TabsPerRow.DefaultValue - Configuration.TabsPerRow.Value) * 64;

            backgroundRect.anchoredPosition = new Vector2(backgroundPivotX, -64);
            backgroundRect.sizeDelta = new Vector2(Configuration.TabsPerRow.Value * 128 + 54, 128);

            menuCollider.size = new Vector3(900, 128 + (childs.Count - 1) / Configuration.TabsPerRow.Value * 128, 1);
            menuCollider.center = new Vector3(0, -64 - (childs.Count - 1) / Configuration.TabsPerRow.Value * 64, 0);
            tooltipRect.anchoredPosition = new Vector2(0, -140 - ((childs.Count - 1) / Configuration.TabsPerRow.Value * 128));

            foreach (RectTransform transform in uixObjects)
                transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, -((childs.Count - 1) / Configuration.TabsPerRow.Value * (128 / 3)));

            if (useStyletor)
            {
                yield return null;
                backgroundRect.anchoredPosition = new Vector2(0, -64);
                backgroundRect.sizeDelta = new Vector2(950, 128);
            }
        }

        [method: HideFromIl2Cpp]
        public void ApplySorting(bool applyDefault = false)
        {
            if (!applyDefault && !Configuration.TabSorting.Value)
                return;

            Dictionary<string, ValueTuple<Transform, UIPage>> tabs = new Dictionary<string, ValueTuple<Transform, UIPage>>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                Transform child = t.Cast<Transform>();

                if (child.gameObject.name != "Background_QM_PagePanel")
                {
                    string pageKey = child.gameObject.GetComponent<MenuTab>()?.field_Public_String_0;
                    if (menuStateController.field_Private_Dictionary_2_String_UIPage_0.ContainsKey(pageKey))
                    {
                        UIPage uiPage = menuStateController.field_Private_Dictionary_2_String_UIPage_0[pageKey];
                        tabs.Add(uiPage.field_Public_String_0, ValueTuple.Create(child, uiPage));
                    }
                    else
                    {
                        TabExtensionMod.Logger.Warning("Menu tab \"" + pageKey + "\" has no UIPage.");
                    }
                }
            }

            if (defaultSorting == null)
            {
                defaultSorting = new List<string>();
                defaultSorting.AddRange(tabs.Keys);
            }

            bool added = false;

            foreach (string tab in tabs.Keys)
            {
                if (!tabSorting.ContainsKey(tab))
                {
                    tabSorting.Add(tab, tabSorting.Count + 1);
                    added = true;
                }
            }

            if (added)
                Configuration.Save(tabSorting);

            List<string> sorting = applyDefault ? defaultSorting : tabSorting.OrderBy(x => x.Value).ToDictionary(k => k.Key, v => v.Value).Keys.ToList();

            int tabIndex = 0;
            for (int i = 0; i < sorting.Count; i++)
            {
                if (tabs.ContainsKey(sorting[i]))
                {
                    tabs[sorting[i]].Item1.SetSiblingIndex(tabIndex + 1);
                    SetPageIndex(tabs[sorting[i]].Item2, tabIndex);
                    tabIndex++;
                }
            }
        }

        [method: HideFromIl2Cpp]
        private void SetPageIndex(UIPage uiPage, int index)
        {
            Il2CppReferenceArray<UIPage> pages = menuStateController.field_Public_ArrayOf_UIPage_0;

            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].Equals(uiPage))
                    Switch(pages, i, index);
            }
        }

        [method: HideFromIl2Cpp]
        private static void Switch<T>(IList<T> array, int index, int newIndex)
        {
            T obj = array[index];
            array[index] = array[newIndex];
            array[newIndex] = obj;
        }
    }
}
