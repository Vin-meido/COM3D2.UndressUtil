using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

using COM3D2.UndressUtil.Plugin.Hooks;

namespace COM3D2.UndressUtil.Plugin
{
    public class MaidEvent : UnityEvent<Maid> { }

    class MaidTracker : MonoBehaviour
    {
        public MaidEvent MaidActivated { get; private set; } = new MaidEvent();
        public MaidEvent MaidDeactivated { get; private set; } = new MaidEvent();
        public MaidEvent MaidPropUpdated { get; private set; } = new MaidEvent();

        private Dictionary<Maid, bool> maidOldStatus = new Dictionary<Maid, bool>();
        private int pollRate = 1;

        private UndressUtilConfig config => UndressUtilPlugin.Instance.Config;

        public void Awake()
        {
            Log.LogVerbose("MaidTracker.Awake [{0}]", this.GetInstanceID());
        }

        public void Start()
        {
            Log.LogVerbose("MaidTracker.Start [{0}]", this.GetInstanceID());
            this.Refresh();

            MaidHooks.Init();
            MaidHooks.OnMaidPropUpdate.AddListener(this.UpdateMaidProp);
        }

        public void Refresh()
        {
            maidOldStatus.Clear();

            foreach (var maid in GetUndressTargets(true))
            {
                this.ActivateMaid(maid);
            }
        }

        public IEnumerable<Maid> GetUndressTargets(bool verbose=false)
        {
            var total_maids = GameMain.Instance.CharacterMgr.GetMaidCount();
#if DEBUG
            if (verbose) Log.LogVerbose("Total maids: {0}", total_maids);
#endif

            for (var i = 0; i < total_maids; i++)
            {
                var maid = GameMain.Instance.CharacterMgr.GetMaid(i);

#if DEBUG
                if (verbose && maid != null) Log.LogVerbose("Maid:{0} CRC:{1} Active:{2}", maid, maid?.IsCrcBody, maid?.isActiveAndEnabled);
#endif

                if (maid != null && maid.isActiveAndEnabled)
                {
                    yield return maid;
                }
            }

            var total_man = GameMain.Instance.CharacterMgr.GetManCount();
#if DEBUG
            if (verbose) Log.LogVerbose("Total man: {0}", total_man);
#endif

            for (var i = 0; i < total_man; i++)
            {
                var man = GameMain.Instance.CharacterMgr.GetMan(i);
#if DEBUG
                if(verbose) Log.LogVerbose("Man:{0} CRC:{1} Active:{2}", man, man?.IsCrcBody, man?.isActiveAndEnabled);
#endif
                if (man != null && man.IsCrcBody && man.isActiveAndEnabled)
                {
                    yield return man;
                }
            }
        }

        public void OnEnable()
        {
            StopAllCoroutines();
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

                foreach (var maid in GetUndressTargets())
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
            maidOldStatus[maid] = true;
            MaidActivated.Invoke(maid);
        }

        private void DeactivateMaid(Maid maid)
        {
            Log.LogVerbose("Deactivate maid: {0}", maid);
            maidOldStatus[maid] = false;
            MaidDeactivated.Invoke(maid);
        }

        private void UpdateMaidProp(Maid maid)
        {
            //Log.LogVerbose("Update maid props: {0}", maid);
            MaidPropUpdated.Invoke(maid);
        }
    }
}
