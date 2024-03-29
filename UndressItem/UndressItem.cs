﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

using COM3D2.UndressUtil.Plugin.Hooks;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
    class UndressItem : IUndressItem
    {
        public Texture Icon { get; private set;  }

        public bool Active { get; private set; }
        public bool Available { get; private set; }

        public Color Color => this.Active ? defaultColor : undressColor;

        static Color defaultColor = Color.white;
        static Color undressColor = Color.gray;
        static MethodInfo crcSetMaskMethod;
        static bool isCrcChecked = false;
        
        string currentPropFilename;
        Maid maid;
        PartsData partsData;

        public static void EnsureCrcChecked()
        {
            if(!isCrcChecked)
            {
                isCrcChecked = true;
                
                if (Product.isCREditSystemSupport)
                {
                    crcSetMaskMethod = AccessTools.Method(typeof(TBody), "SetMask", new Type[] { typeof(MPN), typeof(bool) });
                    if (crcSetMaskMethod == null)
                    {
                        Log.LogError("CRC SetMask not found, if you are using COM3D2.5, then this is not normal.");
                    }
                    else
                    {
                        Log.LogInfo("CRC SetMask found");
                    }
                }
            }
        }

        public void Dress()
        {
            this.Active = true;
            Apply();
        }

        public void Toggle()
        {
            this.Active = !this.Active;
            Apply();
        }

        public void Undress()
        {
            this.Active = false;
            Apply();
        }

        public void Apply()
        {
            SetMaidMask(!this.Active);
        }

        public static Texture GetIcon(string filename)
        {
            SceneEdit.SMenuItem smenuItem = new SceneEdit.SMenuItem();
            SceneEdit.InitMenuItemScript(smenuItem, filename, false);
            return smenuItem.m_texIconRef;
        }
        public static IUndressItem ForMaid(Maid maid, PartsData part)
        {

            var item = new UndressItem()
            {
                maid = maid,
                partsData = part,
            };
            item.Update();
            return item;
        }

        protected virtual void SetMaidMask(bool is_mask_on)
        {
            MaidHooks.Supress(() => SetMaidMaskProc(is_mask_on));
        }

        protected virtual void SetMaidMaskProc(bool is_mask_on)
        {
            foreach (TBody.SlotID f_eSlot in this.partsData.SlotIDlist)
            {
                maid.body0.SetMask(f_eSlot, !is_mask_on);
                Log.LogVerbose("Set slot mask {2} {0}: {1}", f_eSlot, !is_mask_on, maid);
            }


            return;

            // something's wrong with this code, not sure what yet
            // leaving this here for future reference
            EnsureCrcChecked();
            if (crcSetMaskMethod != null)
            {
                foreach (var mpn in this.partsData.CrcMpnList)
                {
                    crcSetMaskMethod.Invoke(maid.body0, new object[] { mpn, !is_mask_on });
                    Log.LogVerbose("Set crc mpn mask {2} {0}: {1}", mpn, !is_mask_on, maid);
                }
            }

        }

        public virtual void Update()
        {
            MaidProp prop = maid.GetProp(partsData.mpn);

            var filename = string.IsNullOrEmpty(prop.strTempFileName) ? prop.strFileName : prop.strTempFileName;

            currentPropFilename = filename;

            if (string.IsNullOrEmpty(filename) || filename.IndexOf("_del") >= 1)
            {
                Active = false;
                Available = false;
                Icon = partsData.DefaultIcon;
            }
            else
            {
                Log.LogVerbose("Undress item {0} {1} [{2}]", maid, partsData.mpn, filename);

                bool isActive = false;
                foreach (TBody.SlotID f_eSlot in partsData.SlotIDlist)
                {
                    isActive = maid.body0.GetMask(f_eSlot);
                    if (isActive) break;
                }

                Active = isActive;
                Available = true;
                Icon = GetIcon(filename);
            }
        }
    }
}
