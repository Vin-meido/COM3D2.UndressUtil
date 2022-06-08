using COM3D2.UndressUtil.Plugin.Hooks;
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
    public class UndressWindowManager: MonoBehaviour
    {
        GameObject itemWindow;
        GameObject itemGrid;
        GameObject maidGrid;
        GameObject halfUndressButton;
        GameObject bg3;
        GameObject bg2;

        bool visible = true;
        public UndressMode Mode { get; private set; } = UndressMode.NORMAL;

        public MaidSelectPanelManager MaidSelectPanelManager { get; private set; }
        private MaidTracker maidTracker;
        private readonly List<UndressItemManager> undressItemManagers = new List<UndressItemManager>();

        public readonly UnityEvent UndressModeChangeEvent = new UnityEvent();

        private UndressUtilPlugin Plugin => UndressUtilPlugin.Instance;

        private bool IsAutoShow => Plugin.IsAutoShow;

        private bool IsAutoHide => Plugin.IsAutoHide;

        private bool IsInSupportedLevel => Plugin.IsInSupportedLevel;

        public enum UndressMode
        {
            NORMAL,
            HALFUNDRESS
        }

        #region Unity
        public void Awake()
        {
            Log.LogVerbose("UndressWindowManager.Awake");

            this.itemWindow = this.gameObject.transform.Find("ItemWindow")?.gameObject;
            Assert.IsNotNull(this.itemWindow, "Could not find item window");

            this.itemGrid = this.gameObject.transform.Find("ItemWindow/ItemGrid")?.gameObject;
            Assert.IsNotNull(this.itemGrid, "Could not find item grid");

            this.maidGrid = this.gameObject.transform.Find("ItemWindow/MaidIcon")?.gameObject;
            Assert.IsNotNull(this.maidGrid, "Could not find MaidIcon");

            this.bg2 = this.gameObject.transform.Find("ItemWindow/BG2")?.gameObject;
            Assert.IsNotNull(this.bg2, "Could not find BG2");

            this.maidTracker = gameObject.GetComponentInParent<MaidTracker>();
            Assert.IsNotNull(this.maidTracker, "Could not find MaidTracker component");


            this.gameObject.AddComponent<TweenAlpha>();

            this.DoHideWindow(true);
        }

        public void Start()
        {
            Log.LogInfo("UndressWindowManager[{0}] Startup", this.GetInstanceID());

            this.bg3 = this.gameObject.transform.Find("ItemWindow/BG3")?.gameObject;
            Assert.IsNotNull(this.bg3, "Could not find BG3");

            SetupComponents();
            SetupMaidIconList();
            SetupItemGrid();
            SetupHalfUndressButton();
            SetupRefreshButton();

            this.maidTracker.MaidActivated.AddListener(this.MaidActive);
            this.maidTracker.MaidDeactivated.AddListener(this.MaidInactive);
            this.maidTracker.MaidPropUpdated.AddListener(this.MaidPropUpdate);

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            YotogiManagerHooks.Init();

            YotogiManagerHooks.OnPreYotogiStart.AddListener(OnPreYotogiStart);
            YotogiManagerHooks.OnYotogiStart.AddListener(OnYotogiStart);
            Log.LogInfo("UndressWindowManager[{0}] Startup complete!", this.GetInstanceID());
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Log.LogInfo("Reinitializing due to scene load...");
            StopAllCoroutines();
            this.DoHideWindow(true);
            ResetItemManagers();
            Log.LogInfo("Reinitialization complete.");
        }

        #endregion

        #region Component setup

        private void SetupComponents()
        {
            // make window draggable
            var bg3 = this.gameObject.transform.Find("ItemWindow/BG3").gameObject;

            // setup button callbacks
            var bg = this.gameObject.transform.Find("ItemWindow/BG").gameObject;
            SetClickCallback(bg, "TitleBar/End", new EventDelegate.Callback(this.HideWindow));
            SetClickCallback(bg, "AllUndress", new EventDelegate.Callback(this.AllUndress));
            SetClickCallback(bg, "AllDress", new EventDelegate.Callback(this.AllDress));
        }

        private void SetupItemGrid()
        {
            var partsDB = new PartsDB();

            foreach (var data in partsDB.GetPartsData())
            {
                var gameObject = Prefabs.CreateItemIcon(this.itemGrid);
                gameObject.name = String.Format("ItemIcon_{0}", data.mpn);
                var manager = gameObject.GetComponent<UndressItemManager>();
                manager.Init(data.mpn, data);
                undressItemManagers.Add(manager);
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

        private void SetupHalfUndressButton()
        {
            halfUndressButton = gameObject.transform.Find("ItemWindow/HalfUndressButton").gameObject;
            var uiBtn = halfUndressButton.GetComponent<UIButton>();
            uiBtn.onClick.Clear();
            EventDelegate.Add(uiBtn.onClick, this.HalfUndressMode);
        }

        private void SetupRefreshButton()
        {
            var button = gameObject.transform.Find("ItemWindow/RefreshButton").gameObject;
            var uiBtn = button.GetComponent<UIButton>();
            uiBtn.onClick.Clear();
            EventDelegate.Add(uiBtn.onClick, this.Refresh);
        }

        private void SetupMaidIconList()
        {
            MaidSelectPanelManager = this.maidGrid.AddComponent<MaidSelectPanelManager>();
            MaidSelectPanelManager.MaidSelected.AddListener(this.MaidSelected);
        }

        #endregion

        public void RepositionItemGrid()
        {
            this.itemGrid.GetComponent<UIGrid>().Reposition();
        }

        public void HideWindow()

        {
            this.DoHideWindow(false);
        }

        public void DoHideWindow(bool immediate = false)
        {
            StopCoroutine(nameof(DelayedShowWindowCoroutine));
            if (this.visible)
            {
                var duration = immediate ? 0f : 0.5f;
                TweenAlpha.Begin(this.gameObject, duration, 0);
                this.visible = false;
            }
        }

        public void DelayedShowWindow()
        {
            StartCoroutine(nameof(DelayedShowWindowCoroutine));
        }

        IEnumerator DelayedShowWindowCoroutine()
        {
            yield return new WaitForSeconds(1);
            if (!visible && this.maidTracker.GetActiveMaids().Count() > 0)
            {
                this.ShowWindow();
            }
        }

        public void ShowWindow(bool immediate = false)
        {
            if (!this.visible)
            {
                this.Refresh();
                var duration = immediate ? 0f : 0.5f;
                TweenAlpha.Begin(this.gameObject, duration, 1);
                this.visible = true;
            }
        }

        public void ToggleWindow()
        {
            if (this.visible)
            {
                this.HideWindow();
            }
            else
            {
                this.ShowWindow();
            }
        }

        public void AllUndress()
        {
            foreach (var undressItem in undressItemManagers)
            {
                if (undressItem.Available)
                {
                    undressItem.Undress();
                }
            }
        }

        public void AllDress()
        {
            foreach (var undressItem in undressItemManagers)
            {
                if (undressItem.Available)
                {
                    undressItem.Dress();
                }
            }
        }

        public void HalfUndressMode()
        {
            this.Mode = this.Mode == UndressMode.HALFUNDRESS ? UndressMode.NORMAL : UndressMode.HALFUNDRESS;

            var uiBtn = halfUndressButton.GetComponent<UIButton>();
            uiBtn.defaultColor = this.Mode == UndressMode.HALFUNDRESS ? Color.white : Color.gray;
            uiBtn.UpdateColor(false);

            this.Refresh();
        }

        public void Refresh()
        {
            UpdateItemManagers(MaidSelectPanelManager.SelectedMaid);
        }

        public int Height
        {
            get => bg2.GetComponent<UISprite>().height;
            set
            {
                var bg3Sprite = bg3.GetComponent<UISprite>();
                var bg2Sprite = bg2.GetComponent<UISprite>();

                bg2Sprite.height = value;
                bg3Sprite.height = value + 20;

                var offset = (800 - value) / 2;
                bg2Sprite.gameObject.transform.localPosition = new Vector3(0, offset - 10);
                bg3Sprite.gameObject.transform.localPosition = new Vector3(0, offset);
                bg3Sprite.ResizeCollider();
            }
        }

        public void SetRows(int i)
        {
            var newHeight = 80 + (i * 80);
            this.Height = newHeight;
        }

        private void OnPreYotogiStart()
        {
            if (!Plugin.IsKeepYotogiDressState) return;

            Log.LogVerbose("Saving dressing state");
            foreach (var manager in undressItemManagers)
            {
                foreach (var maid in maidTracker.GetActiveMaids())
                {
                    manager.UpdateMaid(maid, UndressMode.NORMAL);
                }
            }
        }

        private void OnYotogiStart()
        {
            if (!Plugin.IsKeepYotogiDressState) return;

            Log.LogInfo("Restoring dressing state");
            foreach (var manager in undressItemManagers)
            {
                manager.ReapplyAllMaids(UndressMode.NORMAL);
            }
        }

        private void UpdateItemManagers(Maid maid)
        {
            int available = 0;
            foreach (var manager in undressItemManagers)
            {
                manager.SetMaid(maid, Mode);
                var item = manager.GetUndressItem();
                if (item != null && item.Available) available++;
            }

            RepositionItemGrid();
            var rows = Mathf.CeilToInt(available / 3f);
            rows = rows > 2 ? rows : 2;
            SetRows(rows);
        }

        private void ResetItemManagers()
        {
            foreach (var manager in undressItemManagers)
            {
                manager.Reset();
            }

            RepositionItemGrid();
        }

        #region Event callbacks

        private void MaidActive(Maid maid)
        {
            if (this.IsAutoShow && this.IsInSupportedLevel && !this.visible)
            {
                DelayedShowWindow();
            }
        }

        private void MaidInactive(Maid maid)
        {
            if (this.IsAutoHide && this.visible && this.maidTracker.GetActiveMaids().Count() == 0)
            {
                this.HideWindow();
            }
        }

        private void MaidSelected(Maid maid)
        {
            if(visible)
            {
                UpdateItemManagers(maid);
            }
        }

        private bool maidPropUpdateTriggered = false;
        private bool maidPropUpdateCoroutineStarted = false;

        private void MaidPropUpdate(Maid maid)
        {
            if (!visible)
            {
                return;
            }

            if(maid != MaidSelectPanelManager.SelectedMaid)
            {
                return;
            }

            if (!maidPropUpdateTriggered)
            {
                maidPropUpdateTriggered = true;
                Log.LogVerbose("Maid prop update trigger for maid {0}", maid);
            }

            if (!maidPropUpdateCoroutineStarted) { 
                maidPropUpdateCoroutineStarted = true;
                this.StartCoroutine(MaidPropUpdateCoroutine());
            }
        }

        private IEnumerator MaidPropUpdateCoroutine()
        {
            Log.LogVerbose("Waiting for maid prop updates to stop");

            while (maidPropUpdateTriggered)
            {
                maidPropUpdateTriggered = false;
                yield return new WaitForSeconds(1);
            }


            Log.LogVerbose("Maid prop updates ended, refreshing window.");
            this.Refresh();
            maidPropUpdateCoroutineStarted = false;
        }

        #endregion
    }
}
