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
        UIEventTrigger eventTrigger;
        bool visible = true;
        public UndressMode mode { get; private set; } = UndressMode.NORMAL;

        private Dictionary<UIWFTabButton, Maid> uiTabMaidLookup = new Dictionary<UIWFTabButton, Maid>();
        private Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        public MaidSelectPanelManager MaidSelectPanelManager { get; private set; }
        private MaidTracker maidTracker;
        private readonly List<UndressItemManager> undressItemManagers = new List<UndressItemManager>();

        public readonly UnityEvent UndressModeChangeEvent = new UnityEvent();

        public static UndressWindowManager Instance { get; private set; }

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
            if (Instance != null)
            {
                throw new Exception("Already initialized");
            }

            Instance = this;
            DontDestroyOnLoad(this);

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

            this.DoHideWindow(true);
        }

        public void Start()
        {
            Log.LogVerbose("UndressWindowManager.Start [{0}]", this.GetInstanceID());

            SetupComponents();
            SetupMaidIconList();
            SetupItemGrid();
            SetupHalfUndressButton();
            SetupRefreshButton();

            this.maidTracker.MaidActivated.AddListener(this.MaidActive);
            this.maidTracker.MaidDeactivated.AddListener(this.MaidInactive);
            this.maidTracker.MaidPropUpdated.AddListener(this.MaidPropUpdate);

            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAllCoroutines();
            this.DoHideWindow(true);
            ResetItemManagers();

            StartCoroutine(this.KeyboardCheckCoroutine());

            if (IsAutoShow && IsInSupportedLevel && !visible)
            {
                DelayedShowWindow();
            }
        }

        #endregion

        #region Component setup

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
            if (currentDelayedShow != null)
            {
                StopCoroutine(currentDelayedShow);
            }

            if (this.visible)
            {
                var duration = immediate ? 0f : 0.5f;
                TweenAlpha.Begin(this.gameObject, duration, 0);
                this.visible = false;
            }
        }

        Coroutine currentDelayedShow;

        public void DelayedShowWindow()
        {
            currentDelayedShow = StartCoroutine(DelayedShowWindowCoroutine());
        }

        IEnumerator DelayedShowWindowCoroutine()
        {
            Log.LogVerbose("Delayed show wait");
            yield return new WaitForSeconds(1);
            if (!visible && this.maidTracker.GetActiveMaids().Count() > 0)
            {
                Log.LogVerbose("Delayed show start");
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
                undressItem.Undress();
            }
        }

        public void AllDress()
        {
            foreach (var undressItem in undressItemManagers)
            {
                undressItem.Dress();
            }
        }

        public void HalfUndressMode()
        {
            this.mode = this.mode == UndressMode.HALFUNDRESS ? UndressMode.NORMAL : UndressMode.HALFUNDRESS;

            var uiBtn = halfUndressButton.GetComponent<UIButton>();
            uiBtn.defaultColor = this.mode == UndressMode.HALFUNDRESS ? Color.white : Color.gray;
            uiBtn.UpdateColor(false);

            this.Refresh();
        }

        public void Refresh()
        {
            UpdateItemManagers(MaidSelectPanelManager.SelectedMaid);
        }

        private IEnumerator KeyboardCheckCoroutine()
        {
            var shortcut = UndressUtilPlugin.Instance.Config.showShortcut.Value;
            while (true)
            {
                yield return null;

                if (shortcut.IsDown())
                {
                    ToggleWindow();
                }
            }
        }

        private void UpdateItemManagers(Maid maid)
        {
            foreach (var manager in undressItemManagers)
            {
                manager.SetMaid(maid, mode);
            }

            RepositionItemGrid();
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
                Log.LogVerbose("Skipping maid prop update because undress window is hidden");
                return;
            }

            if(maid != MaidSelectPanelManager.SelectedMaid)
            {
                Log.LogVerbose("Skipping maid prop update because it is not the selected maid");
                return;
            }

            if (!maidPropUpdateTriggered)
            {
                maidPropUpdateTriggered = true;
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
