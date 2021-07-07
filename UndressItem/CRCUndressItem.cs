using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
    class CRCUndressItem : IUndressItem
    {
        public Texture Icon => partsData.DefaultIcon;

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
            }
            else
            {
                Dress();
            }
        }

        public void Undress()
        {
            SetMaidMask(true);
            this.Active = false;
        }

        public static IUndressItem ForMaid(Maid maid, PartsData part)
        {
            return new CRCUndressItem()
            {
                maid = maid,
                partsData = part,
            };

            throw new NotImplementedException();
        }

        protected virtual void SetMaidMask(bool is_mask_on)
        {
#if CRC_SUPPORT
            foreach (MPN parentMpn in this.partsData.CrcMpnList)
			{
				maid.body0.SetMask(parentMpn, !is_mask_on);
			}
#endif
        }

    }
}
