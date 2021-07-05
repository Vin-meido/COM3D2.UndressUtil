using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
    class UndressItem : IUndressItem
    {
        public Texture Icon { get; private set;  }

        public bool Active { get; private set; }

        public Color Color => this.Active ? defaultColor : undressColor;

        static Color defaultColor = Color.white;
        static Color undressColor = Color.gray;
        
        Maid maid;
        PartsData partsData;

        public void Dress()
        {
            SetMaidMask(false);
            this.Active = true;
        }

        public void Toggle()
        {
            if (this.Active)
            {
                Undress();
            } else
            {
                Dress();
            }
        }

        public void Undress()
        {
            SetMaidMask(true);
            this.Active = false;
        }

        public static Texture GetIcon(string filename)
        {
            SceneEdit.SMenuItem smenuItem = new SceneEdit.SMenuItem();
            SceneEdit.InitMenuItemScript(smenuItem, filename, false);
            return smenuItem.m_texIconRef;
        }

        public static IUndressItem ForMaid(Maid maid, PartsData part)
        {
            MaidProp prop = maid.GetProp(part.mpn);
            if (string.IsNullOrEmpty(prop.strFileName) || prop.strFileName.IndexOf("_del") >= 1)
            {
                return null;
            }

            return new UndressItem()
            {
                maid = maid,
                partsData = part,
                Icon = GetIcon(prop.strFileName),
                Active = true,
            };
        }

        protected virtual void SetMaidMask(bool is_mask_on)
        {
            foreach (TBody.SlotID f_eSlot in this.partsData.SlotIDlist)
            {
                maid.body0.SetMask(f_eSlot, !is_mask_on);
            }
        }

    }
}
