using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using MaidExtension;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
    class CostumeItem : IUndressItem
    {
        public Texture Icon { get; private set; }

        public bool Active { get; private set; }

        public Color Color => Color.white;
    
        List<string> costumes;
        PartsData partsData;
        Maid maid;
        int currentCostume = -1;

        public void Dress()
        {
            currentCostume = -1;
            UpdateCostume();
        }

        public void Toggle()
        {
            currentCostume += 1;
            if (currentCostume >= costumes.Count())
            {
                currentCostume = -1;
            }

            UpdateCostume();
        }

        public void Undress()
        {
            currentCostume = costumes.Count() - 1;
            UpdateCostume();
        }

        void UpdateCostume()
        {
            if (currentCostume == -1)
            {
                maid.ResetProp(partsData.mpn.ToString());
                Log.LogVerbose("Costume reset {0}", partsData.mpn);
            }
            else
            {
                var newCostume = costumes[currentCostume];
                maid.ItemChangeTemp(partsData.mpn.ToString(), newCostume);
                Log.LogVerbose("Costume change {0} => {1}", partsData.mpn, newCostume);
            }
            maid.AllProcProp();
        }

        static IEnumerable<T> GetEnumValues<T>()
        {
            foreach (var obj in Enum.GetValues(typeof(T)))
            {
                yield return (T)obj;
            }
        }

        public static IUndressItem ForMaid(Maid maid, PartsData part)
        {
            var costumes = new List<string>();

            foreach(var t in GetEnumValues<MaidCostumeChangeController.MaidCostumeChangeManager.CostumeType>())
            {
                string names = MaidCostumeChangeController.MaidCostumeChangeManager.GetItemChangeNames(t);
                if (maid.IsItemChangeEnabled(part.mpn.ToString(), names))
                {
                    Log.LogVerbose("Costume item {0} {1} [{2}]", maid, part.mpn, names);
                    costumes.Add(names);
                }
            }

            if (costumes.Count() > 0)
            {
                return new CostumeItem()
                {
                    maid = maid,
                    partsData = part,
                    costumes = costumes,
                    Icon = null,
                    Active = false,
                };

            }

            return null;
        }

    }
}
