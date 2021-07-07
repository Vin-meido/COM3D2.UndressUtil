using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin
{
    public class MaidEvent : UnityEvent<Maid> { }

    class MaidTracker : MonoBehaviour
    {
        public MaidEvent MaidActivated { get; private set; } = new MaidEvent();
        public MaidEvent MaidDeactivated { get; private set; } = new MaidEvent();

        private Dictionary<Maid, bool> maidOldStatus = new Dictionary<Maid, bool>();
        private int pollRate = 1;

        public void Start()
        {
            
        }

        public void OnEnable()
        {
            StartCoroutine(this.Coroutine());
        }

        public IEnumerable<Maid> GetActiveMaids()
        {
            return maidOldStatus
                .Where(item => item.Value == true)
                .Select(item => item.Key)
                .ToList();
        }

        public IEnumerator Coroutine()
        {
            while (true)
            {
                var total_maids = GameMain.Instance.CharacterMgr.GetMaidCount();
                var checkedMaids = new List<Maid>();

                for (var i=0; i<total_maids; i++)
                {
                    var maid = GameMain.Instance.CharacterMgr.GetMaid(i);
                    if(maid != null)
                    {
                        var enabled = maid.isActiveAndEnabled;
                        var oldStatus = maidOldStatus.ContainsKey(maid) ? maidOldStatus[maid] : false;

                        maidOldStatus[maid] = enabled;

                        if (enabled != oldStatus)
                        {
                            if (enabled)
                            {
                                ActivateMaid(maid);
                            }
                            else
                            {
                                DeactivateMaid(maid);
                            }
                        }

                        checkedMaids.Add(maid);
                    }

                }

                IEnumerable<Maid> uncheckedMaids = maidOldStatus
                    .Where(item => !checkedMaids.Contains(item.Key) && item.Value)
                    .Select(item => item.Key)
                    .ToList();

                foreach (var maid in uncheckedMaids)
                {
                    maidOldStatus[maid] = false;
                    DeactivateMaid(maid);
                }

                yield return new WaitForSeconds(pollRate);
            }
        }

        private void ActivateMaid(Maid maid)
        {
            Log.LogVerbose("Activate maid: {0}", maid);
            MaidActivated.Invoke(maid);
        }

        private void DeactivateMaid(Maid maid)
        {
            Log.LogVerbose("Deactivate maid: {0}", maid);
            MaidDeactivated.Invoke(maid);
        }
    }
}
