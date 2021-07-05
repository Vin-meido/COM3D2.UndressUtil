using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace COM3D2.UndressUtil.Plugin
{
    class UndressWindowManager: MonoBehaviour
    {
        GameObject itemWindow;
        GameObject itemGrid;
        GameObject maidGrid;
        UIEventTrigger eventTrigger;
        bool visible = true;
        public UndressMode mode { get; private set; } = UndressMode.NORMAL;

        private Dictionary<UIWFTabButton, Maid> uiTabMaidLookup = new Dictionary<UIWFTabButton, Maid>();
        private Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        public MaidSelectPanelManager MaidSelectPanelManager { get; private set; }
        private MaidTracker maidTracker;

        public readonly UnityEvent UndressModeChangeEvent = new UnityEvent();

        public enum UndressMode
        {
            NORMAL,
            HALFUNDRESS
        }

        public void Awake()
        {
            Log.LogVerbose("UndressWindowManager.Awake");

            this.itemWindow = this.gameObject.transform.Find("ItemWindow").gameObject;
            Assert.IsNotNull(this.itemWindow, "Could not find item window");

            this.itemGrid = this.gameObject.transform.Find("ItemWindow/ItemGrid").gameObject;
            Assert.IsNotNull(this.itemGrid, "Could not find item grid");

            this.maidGrid = this.gameObject.transform.Find("ItemWindow/MaidIcon").gameObject;
            Assert.IsNotNull(this.maidGrid, "Could not find MaidIcon");

            this.maidTracker = gameObject.GetComponentInParent<MaidTracker>();
            Assert.IsNotNull(this.maidTracker, "Could not find MaidTracker component");

            this.eventTrigger = this.gameObject
                .transform
                .Find("TakeEvent")
                .gameObject
                .GetComponent<UIEventTrigger>();
            Assert.IsNotNull(this.eventTrigger, "Could not find event trigger");

            this.gameObject.AddComponent<TweenAlpha>();
        }

        public void Start()
        {
            Log.LogVerbose("UndressWindowManager.Start [{0}]", this.GetInstanceID());

            SetupComponents();
            SetupMaidIconList();
            SetupItemGrid();

            if ((!GameMain.Instance.VRMode)
                && (!UndressUtilPlugin.Instance.Config.autoShowInNonVr.Value))
            {
                Log.LogVerbose("Auto show disabled, hiding window");
                this.visible = false;
                this.gameObject.AddComponent<TweenAlpha>().to = 0;
            }

            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
        }

        public void OnEnable()
        {
            StartCoroutine(this.KeyboardCheckCoroutine());
        }


        public void OnDestroy()
        {
            Log.LogVerbose("UndressWindowManager.OnDestroy [{0}]", this.GetInstanceID());
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }

        public void OnSceneUnloaded(Scene sceen)
        {
            Destroy(this.gameObject);
        }

        private void SetupComponents()
        {
            // make window draggable
            var bg = this.gameObject.transform.Find("ItemWindow/BG").gameObject;
            var dragger = bg.AddComponent<PhotoWindowDragMove>();
            dragger.WindowTransform = this.gameObject.transform;

            // setup button callbacks
            SetClickCallback(bg, "TitleBar/End", new EventDelegate.Callback(this.HideWindow));
            SetClickCallback(bg, "AllUndress", new EventDelegate.Callback(this.AllUndress));
            SetClickCallback(bg, "AllDress", new EventDelegate.Callback(this.AllDress));
            SetClickCallback(this.gameObject, "ItemWindow/HalfUndressButton", new EventDelegate.Callback(this.HalfUndressMode));
        }

        private void SetupItemGrid()
        {
            var partsDB = new PartsDB();

            foreach (var data in partsDB.GetPartsData())
            {
                var gameObject = Prefabs.CreateItemIcon(this.itemGrid);
                gameObject.name = String.Format("ItemIcon_{0}", data.mpn);
                gameObject
                    .GetComponent<UndressItemManager>()
                    .Init(data.mpn, data);
            }

            this.itemGrid.GetComponent<UIGrid>().Reposition();
        }

        private void SetClickCallback(GameObject bg, string child, EventDelegate.Callback callback)
        {
            var btn = bg.transform.Find(child).gameObject;
            var uibtn = btn.GetComponent<UIButton>();
            uibtn.onClick.Clear();
            EventDelegate.Add(uibtn.onClick, callback);
        }

        private void SetupMaidIconList()
        {
            MaidSelectPanelManager = this.maidGrid.AddComponent<MaidSelectPanelManager>();
        }

        public void HideWindow()
        {
            if (this.visible)
            {
                TweenAlpha.Begin(this.gameObject, 0.5f, 0);
                this.visible = false;
            }
        }

        public void ShowWindow()
        {
            if (!this.visible)
            {
                TweenAlpha.Begin(this.gameObject, 0.5f, 1);
                this.visible = true;
            }
        }

        public void AllUndress()
        {
            foreach (UndressItemManager undressItem in this.gameObject.GetComponentsInChildren<UndressItemManager>())
            {
                undressItem.Undress();
            }
        }

        public void AllDress()
        {
            foreach (UndressItemManager undressItem in this.gameObject.GetComponentsInChildren<UndressItemManager>())
            {
                undressItem.Dress();
            }
        }

        public void HalfUndressMode()
        {
            this.mode = this.mode == UndressMode.HALFUNDRESS ? UndressMode.NORMAL : UndressMode.HALFUNDRESS;
            this.UndressModeChangeEvent.Invoke();
        }

        private IEnumerator KeyboardCheckCoroutine()
        {
            var shortcut = UndressUtilPlugin.Instance.Config.showShortcut.Value;
            Log.LogInfo("UndressWindow shortcut is [{0}]", shortcut);

            while (true)
            {
                yield return null;

                if (shortcut.IsDown())
                {
                    if (this.visible)
                    {
                        this.HideWindow();
                    } else
                    {
                        this.ShowWindow();
                    }
                }
            }
        }

    }
}
