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
        GameObject itemGrid;
        GameObject maidGrid;
        private Dictionary<UIWFTabButton, Maid> uiTabMaidLookup = new Dictionary<UIWFTabButton, Maid>();
        private Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        public MaidSelectManager maidSelectManager { get; private set; }
        private MaidTracker maidTracker;

        public void Start()
        {
            maidTracker = gameObject.GetComponentInParent<MaidTracker>();

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

        private void SetupComponents()
        {
            // make window draggable
            var bg = this.gameObject.transform.Find("ItemWindow/BG").gameObject;
            var dragger = bg.AddComponent<PhotoWindowDragMove>();
            dragger.WindowTransform = this.gameObject.transform;

            // setup button callbacks
            SetCallback(bg, "TitleBar/End", new EventDelegate.Callback(this.HideWindow));
            SetCallback(bg, "AllUndress", new EventDelegate.Callback(this.AllUndress));
            SetCallback(bg, "AllDress", new EventDelegate.Callback(this.AllDress));
        }

        private void SetupItemGrid()
        {
            this.itemGrid = this.gameObject.transform.Find("ItemWindow/ItemGrid").gameObject;
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

        private void SetCallback(GameObject bg, string child, EventDelegate.Callback callback)
        {
            var btn = bg.transform.Find(child).gameObject;
            var uibtn = btn.GetComponent<UIButton>();
            uibtn.onClick.Clear();
            EventDelegate.Add(uibtn.onClick, callback);
        }

        private void SetupMaidIconList()
        {
            this.maidGrid = this.gameObject.transform.Find("ItemWindow/MaidIcon").gameObject;
            Destroy(this.maidGrid.GetComponent<UIWFTabPanel>());
            maidSelectManager = this.maidGrid.AddComponent<MaidSelectManager>();
        }

        public void HideWindow()
        {
            this.gameObject.SetActive(false);
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
    }
}
