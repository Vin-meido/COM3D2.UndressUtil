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

            var filename = string.IsNullOrEmpty(prop.strTempFileName) ? prop.strFileName : prop.strTempFileName;

            if (string.IsNullOrEmpty(filename) || filename.IndexOf("_del") >= 1)
            {
                return null;
            }

            Log.LogVerbose("Undress item {0} {1} [{2}]", maid, part.mpn, filename);

            bool isActive = false;
            foreach (TBody.SlotID f_eSlot in part.SlotIDlist)
            {
                isActive = maid.body0.GetMask(f_eSlot);
                if (isActive) break;
            }

            return new UndressItem()
            {
                maid = maid,
                partsData = part,
                Icon = GetIcon(filename),
                Active = isActive,
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
