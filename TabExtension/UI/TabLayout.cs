using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;

namespace TabExtension.UI
{
    public class TabLayout : MonoBehaviour
    {

        private static readonly int TabsPerRow = 7;

        private GameObject quickMenu;
        private GameObject layout;
        private RectTransform tooltipRect;
        private RectTransform backgroundRect;
        private BoxCollider menuCollider;
        private List<RectTransform> uixObjects;

        private bool useStyletor;

        public TabLayout(IntPtr value) : base(value)
        {
            quickMenu = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)").gameObject;
            layout = quickMenu.transform.Find("Container/Window/Page_Buttons_QM/HorizontalLayoutGroup").gameObject;
            tooltipRect = quickMenu.transform.Find("Container/Window/ToolTipPanel").GetComponent<RectTransform>();

            DestroyImmediate(layout.GetComponent<HorizontalLayoutGroup>());

            GameObject background = quickMenu.transform.Find("Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Background_QM_PagePanel").gameObject;
            background.SetActive(true);

            if (MelonHandler.Mods.Any(mod => mod.Info.Name.Equals("Styletor")))
            {
                useStyletor = true;
                StyleElement styleElement = background.GetComponent<StyleElement>();
                styleElement.field_Public_String_1 = "TabBottom";
            }

            Image image = background.GetComponent<Image>();
            image.sprite = transform.Find("Page_Dashboard/Background").GetComponent<Image>().sprite;
            image.color = new Color(1, 1, 1, 0.6f);

            backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchoredPosition = new Vector2(0, -64);
            backgroundRect.sizeDelta = new Vector2(950, 128);
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
                        child.gameObject.AddComponent<LayoutListener>().OnLayoutUpdateRequested += new Action(() => MelonCoroutines.Start(RecalculateLayout()));
                }

                if (useStyletor)
                    MelonCoroutines.Start(UpdateBackgroundLater());
            }

            List<Transform> childs = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                Transform child = t.Cast<Transform>();
                if (child.gameObject.activeSelf && child.gameObject.name != "Background_QM_PagePanel")
                {
                    if (child.GetComponent<LayoutListener>() == null)
                        child.gameObject.AddComponent<LayoutListener>().OnLayoutUpdateRequested += new Action(() => MelonCoroutines.Start(RecalculateLayout()));

                    childs.Add(child);
                }
            }

            int pivotX = 0;

            for (int i = 0; i < childs.Count; i++)
            {
                int y = (i / TabsPerRow);
                int x = i - (y * TabsPerRow);

                if (x == 0)
                    pivotX = -(((childs.Count - i) >= TabsPerRow ? TabsPerRow : (childs.Count - i)) * 64) + 64;

                if (childs.Count > i)
                {
                    RectTransform rect = childs[i].transform.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(pivotX + x * 128, -(y * 128));
                    rect.pivot = new Vector2(0.5f, 1);
                }
            }

            menuCollider.size = new Vector3(900, 128 + ((childs.Count - 1) / TabsPerRow) * 128, 1);
            menuCollider.center = new Vector3(0, -64 - ((childs.Count - 1) / TabsPerRow) * 64, 0);
            tooltipRect.anchoredPosition = new Vector2(0, -140 - (((childs.Count - 1) / TabsPerRow) * 128));

            foreach (RectTransform transform in uixObjects)
                transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, -(((childs.Count - 1) / TabsPerRow) * (128 / 3)));

            if (useStyletor)
            {
                yield return null;
                backgroundRect.anchoredPosition = new Vector2(0, -64);
                backgroundRect.sizeDelta = new Vector2(950, 128);
            }
        }

        [method: HideFromIl2Cpp]
        public IEnumerator UpdateBackgroundLater()
        {
            yield return new WaitForSeconds(5);
            backgroundRect.anchoredPosition = new Vector2(0, -64);
            backgroundRect.sizeDelta = new Vector2(950, 128);
        }
    }
}
