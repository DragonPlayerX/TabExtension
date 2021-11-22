using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace TabExtension.UI
{
    public class LayoutListener : MonoBehaviour
    {

        private bool lastState;

        [method: HideFromIl2Cpp]
        public event Action OnLayoutUpdateRequested;

        public LayoutListener(IntPtr value) : base(value) { }

        internal void Awake()
        {
            lastState = gameObject.activeSelf;
            OnLayoutUpdateRequested += new Action(() => MelonCoroutines.Start(TabLayout.Instance.RecalculateLayout()));
            OnLayoutUpdateRequested?.Invoke();
        }

        internal void OnEnable()
        {
            if (!lastState)
                OnLayoutUpdateRequested?.Invoke();

            lastState = gameObject.activeSelf;
        }

        internal void OnDisable()
        {
            if (lastState && transform.parent.gameObject.activeInHierarchy)
                OnLayoutUpdateRequested?.Invoke();

            lastState = gameObject.activeSelf;
        }
    }
}
