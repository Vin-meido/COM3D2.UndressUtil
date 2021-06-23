using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin
{
    class MaidSelectManager: MonoBehaviour
    {
        GameObject maidGrid;
        Dictionary<Maid, GameObject> maidGameObjectLookup = new Dictionary<Maid, GameObject>();
        MaidTracker maidTracker;
        public MaidSelectEvent MaidSelected { get; private set; } = new MaidSelectEvent();
        public Maid SelectedMaid { get; private set; }

        public class MaidSelectEvent : UnityEvent<Maid> { };
        public class MaidIconClickEvent : UnityEvent<Maid> { };

        public class MaidIcon: UIButton
        {
            public Maid maid { get; private set; }
            public MaidIconClickEvent Click { get; private set; } = new MaidIconClickEvent();

            public void Setup(Maid maid)
            {
                this.maid = maid;
                EventDelegate.Add(this.onClick, this.OnMaidIconClick);
            }

            public void OnMaidIconClick()
            {
                Click.Invoke(this.maid);
            }

            public void SetSelected(bool selected)
            {
                if (selected)
                {
                    SetState(UIButtonColor.State.Disabled, false);
                    var frame = gameObject.transform.parent.Find("Frame");
                    var sprite = frame.GetComponent<UISprite>();
                    sprite.color = Color.yellow;
                } else
                {
                    SetState(UIButtonColor.State.Normal, false);
                    var frame = gameObject.transform.parent.Find("Frame");
                    var sprite = frame.GetComponent<UISprite>();
                    sprite.color = Color.clear;
                }
            }
        }

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

        private void OnMaidActivated(Maid maid)
        {
            if (maidGameObjectLookup.ContainsKey(maid))
            {
                maidGameObjectLookup[maid].SetActive(true);
            }
            else
            {
                var gameObject = Prefabs.CreateMaidIcon(this.maidGrid);

                gameObject.GetComponentInChildren<UITexture>().mainTexture = maid.GetThumIcon();
                var maidIconComponent = gameObject.GetComponentInChildren<MaidIcon>();
                maidIconComponent.Setup(maid);
                maidIconComponent.Click.AddListener(this.OnMaidIconClick);

                maidGameObjectLookup[maid] = gameObject;
            }

            maidGrid.GetComponent<UIGrid>().Reposition();
            if (this.SelectedMaid == null)
            {
                OnMaidIconClick(maid);
            }
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
                maidGrid.GetComponent<UIGrid>().Reposition();
                
                if (this.SelectedMaid == maid)
                {
                    SelectNextActiveMaid();
                }
            }
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
