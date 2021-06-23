using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin
{
    public class MaidIconClickEvent : UnityEvent<Maid> { };

    public class MaidIcon : MonoBehaviour
    {
        public Maid maid { get; private set; }
        UISprite frame;
        UIButton button;
        UITexture texture;

        public MaidIconClickEvent Click { get; private set; } = new MaidIconClickEvent();

        public void Setup(Maid maid)
        {
            this.maid = maid;
        }

        public void Awake() {
            Log.LogVerbose("MaidIcon.Awake {0}", this);

            var frameObj = gameObject.transform.Find("IconMask/Frame").gameObject;
            frame = frameObj.GetComponent<UISprite>();

            var iconObj = gameObject.transform.Find("IconMask/Icon").gameObject;
            button = iconObj.AddComponent<UIButton>();

            texture = iconObj.GetComponentInChildren<UITexture>();
            Assert.IsNotNull(texture, "Cannot find iconObj.UITexture");
        }

        public void Start()
        {
            Log.LogVerbose("MaidIcon.Start {0}", this);
            Assert.IsNotNull(this.maid, "maid not set");

            texture.mainTexture = maid.GetThumIcon();
            EventDelegate.Add(button.onClick, this.OnMaidIconClick);
        }

        public void OnMaidIconClick()
        {
            Click.Invoke(this.maid);
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                button.SetState(UIButtonColor.State.Disabled, false);
                frame.color = Color.yellow;
            }
            else
            {
                button.SetState(UIButtonColor.State.Normal, false);
                frame.color = Color.clear;
            }
        }
    }
}
