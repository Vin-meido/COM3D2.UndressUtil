using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin
{
    public class MaidSelectPanelManager: MonoBehaviour
    {
        GameObject maidGrid;
        Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        MaidTracker maidTracker;
        public MaidSelectEvent MaidSelected { get; private set; } = new MaidSelectEvent();
        public Maid SelectedMaid { get; private set; }

        public class MaidSelectEvent : UnityEvent<Maid> { };


        public void Start()
        {
            maidTracker = this.gameObject.GetComponentInParent<MaidTracker>();
            maidTracker.MaidActivated.AddListener(this.OnMaidActivated);
            maidTracker.MaidDeactivated.AddListener(this.OnMaidDeactivated);
            maidGrid = gameObject;

            foreach (var maid in maidTracker.GetActiveMaids())
            {
                OnMaidActivated(maid);
            }
        }

        public void OnDestroy()
        {
            maidTracker.MaidActivated.RemoveListener(this.OnMaidActivated);
            maidTracker.MaidDeactivated.RemoveListener(this.OnMaidDeactivated);
        }

        public void RepositionIcons()
        {
            StartCoroutine(RepositionIconsCoroutine());
        }

        IEnumerator RepositionIconsCoroutine()
        {
            yield return null;
            maidGrid.GetComponent<UIGrid>().Reposition();
        }

        private void OnMaidActivated(Maid maid)
        {
            if (maidGameObjectLookup.ContainsKey(maid))
            {
                maidGameObjectLookup[maid].SetActive(true);
                return;
            }

            var gameObject = Prefabs.CreateMaidIcon(this.maidGrid);
            var maidIconComponent = gameObject.GetComponentInChildren<MaidIcon>();
            Assert.IsNotNull(maidIconComponent, "Could not get maidIconComponent for [{0}]", gameObject);
            maidIconComponent.Setup(maid);
            maidIconComponent.Click.AddListener(this.OnMaidIconClick);

            maidGameObjectLookup[maid] = gameObject;

            if (this.SelectedMaid == null)
            {
                OnMaidIconClick(maid);
            }

            RepositionIcons();
        }

        private void OnMaidDeactivated(Maid maid)
        {
            if (maidGameObjectLookup.ContainsKey(maid))
            {
                var obj = maidGameObjectLookup[maid];
                var component = obj.GetComponentInChildren<MaidIcon>();
                component.SetSelected(false);
                obj.SetActive(false);
                Destroy(obj);
                maidGameObjectLookup.Remove(maid);

                if (this.SelectedMaid == maid)
                {
                    SelectNextActiveMaid();
                }
            }
            RepositionIcons();
        }

        private void SelectNextActiveMaid()
        {
            this.SelectedMaid = maidTracker.GetActiveMaids().FirstOrDefault();
            if (SelectedMaid != null)
            {
                OnMaidIconClick(SelectedMaid);
            }
            else
            {
                MaidSelected.Invoke(SelectedMaid);
            }
        }

        private void OnMaidIconClick(Maid maid)
        {
            this.SelectedMaid = maid;
            foreach (var component in gameObject.GetComponentsInChildren<MaidIcon>())
            {
                component.SetSelected(component.maid == maid);
            }

            Log.LogVerbose("Maid selected event [{0}]", maid);
            MaidSelected.Invoke(maid);
        }
    }
}
