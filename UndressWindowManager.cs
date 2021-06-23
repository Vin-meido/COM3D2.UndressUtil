using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.UndressUtil.Plugin
{
    class UndressWindowManager: MonoBehaviour
    {
        GameObject itemWindow;
        GameObject itemGrid;
        GameObject maidGrid;
        UIEventTrigger eventTrigger;

        private Dictionary<UIWFTabButton, Maid> uiTabMaidLookup = new Dictionary<UIWFTabButton, Maid>();
        private Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        public MaidSelectManager maidSelectManager { get; private set; }
        private MaidTracker maidTracker;

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
            Log.LogVerbose("UndressWindowManager.Start");

            SetupHover();
            SetupComponents();
            SetupMaidIconList();
            SetupItemGrid();

            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }

        public void OnSceneUnloaded(Scene sceen)
        {
            Destroy(this.gameObject);
        }

        private void SetupHover()
        {
            EventDelegate.Add(eventTrigger.onHoverOver, this.OnHover);
            EventDelegate.Add(eventTrigger.onHoverOut, this.OnHoverOut);
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
        }

        private void SetupItemGrid()
        {
            var partsDB = new PartsDB();

            foreach (var data in partsDB.GetPartsData())
            {
                var gameObject = Prefabs.CreateItemIcon(this.itemGrid);
                gameObject.name = String.Format("ItemIcon_{0}", data.mpn);
                gameObject
                    .GetComponent<UndressItem>()
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
            maidSelectManager = this.maidGrid.AddComponent<MaidSelectManager>();
        }

        public void HideWindow()
        {
            TweenAlpha.Begin(this.gameObject, 0.5f, 0);
        }

        public void AllUndress()
        {
            foreach (UndressItem undressItem in this.gameObject.GetComponentsInChildren<UndressItem>())
            {
                undressItem.SetMaidMask(maidSelectManager.SelectedMaid, true);
            }
        }

        public void AllDress()
        {
            foreach (UndressItem undressItem in this.gameObject.GetComponentsInChildren<UndressItem>())
            {
                undressItem.SetMaidMask(maidSelectManager.SelectedMaid, false);
            }
        }

        private void OnHover()
        {
            Log.LogVerbose("UWM.OnHover");
        }

        private void OnHoverOut()
        {
            Log.LogVerbose("UWM.OnHoverOut");
        }
    }
}
