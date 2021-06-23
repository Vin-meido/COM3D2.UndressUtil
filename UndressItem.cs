using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{
    class UndressItem: MonoBehaviour
    {
		private PartsDB.PartsData data;
		private UIButton button;
		private UITexture icon;
		private MPN mpn;

		private Dictionary<Maid, MaidInfo> maidMaidInfoLookup = new Dictionary<Maid, MaidInfo>();

		private Color m_DefaultColor = Color.white;
		private Color m_UndressColor = Color.gray;

		private MaidSelectPanelManager maidSelectManager;
		private MaidTracker maidTracker;

		public void Awake()
        {
			button = base.GetComponent<UIButton>();
			icon = base.GetComponent<UITexture>();
		}

		public void Start()
        {
			var mgr = this.gameObject.GetComponentInParent<UndressWindowManager>();
			Assert.IsNotNull(mgr.MaidSelectPanelManager, "Could not find MaidSelectManager");
			maidSelectManager = mgr.MaidSelectPanelManager;
			maidSelectManager.MaidSelected.AddListener(this.UpdateState);

			EventDelegate.Add(this.button.onClick, new EventDelegate.Callback(this.SwitchMask));

			maidTracker = gameObject.GetComponentInParent<MaidTracker>();
			Assert.IsNotNull(maidTracker, "Could not find MaidTracker");
			maidTracker.MaidActivated.AddListener(this.AddMaidData);

			// Add maid data only when the scene is fully loaded
			// Avoids maids not having equiped slots due to early loading
			StartCoroutine(this.AddMaidDataCoroutine());
		}

		public void OnDestroy()
        {
			maidTracker.MaidActivated.RemoveListener(this.AddMaidData);
        }

		private IEnumerator AddMaidDataCoroutine()
		{
			yield return null;
			foreach (var maid in maidTracker.GetActiveMaids())
			{
				this.AddMaidData(maid);
			}

			UpdateState(maidSelectManager.SelectedMaid);
		}

		private class MaidInfo
		{
			public bool IsUndress;
			public Texture ItemIcon;
			public MaidInfo(Texture icon, bool is_undress)
			{
				this.ItemIcon = icon;
				this.IsUndress = is_undress;
			}
		}

		public bool IsEnable
		{
			get
			{
				return this.button.isEnabled;
			}
		}

		public void Init(MPN mpn, PartsDB.PartsData data)
		{
			this.data = data;
			this.mpn = mpn;
			this.icon.mainTexture = this.data.DefaultIcon;
		}

		public void SwitchMask()
		{
			var selectMaid = maidSelectManager.SelectedMaid;
			this.SetMaidMask(selectMaid, !this.maidMaidInfoLookup[selectMaid].IsUndress);
		}

		public void SetMaidMask(Maid maid, bool is_mask_on)
		{
			if (!this.IsEnable)
			{
				return;
			}
			if (!this.maidMaidInfoLookup.ContainsKey(maid))
			{
				return;
			}
			if (this.maidMaidInfoLookup[maid].IsUndress == is_mask_on)
			{
				return;
			}

			this.maidMaidInfoLookup[maid].IsUndress = is_mask_on;
			if (maid.IsCrcBody)
			{
				/*
				foreach (MPN parentMpn in this.data.CrcMpnList)
				{
					maid.body0.SetMask(parentMpn, !is_mask_on);
				}
				*/
			}
			else
			{
				foreach (TBody.SlotID f_eSlot in this.data.SlotIDlist)
				{
					maid.body0.SetMask(f_eSlot, !is_mask_on);
					this.button.defaultColor = (this.button.hover = ((!this.maidMaidInfoLookup[maid].IsUndress) ? this.m_DefaultColor : this.m_UndressColor));
				}
			}
		}


		public void AddMaidData(Maid maid)
		{
			if (!maid || maidMaidInfoLookup.ContainsKey(maid))
			{
				return;
			}
			if (maid.IsCrcBody)
			{
				this.maidMaidInfoLookup[maid] = new UndressItem.MaidInfo(null, false);
			}
			else
			{
				MaidProp prop = maid.GetProp(this.mpn);
				SceneEdit.SMenuItem smenuItem = new SceneEdit.SMenuItem();
				Assert.IsNotEmpty(prop.strFileName, "Prop [{0}] has no filename", prop);
				SceneEdit.InitMenuItemScript(smenuItem, prop.strFileName, false);
				if (smenuItem.m_texIconRef)
				{
					this.maidMaidInfoLookup[maid] = new UndressItem.MaidInfo(smenuItem.m_texIconRef, false);
				}
			}
		}

		public void UpdateState(Maid maid)
		{
			if (maid == null)
            {
				this.icon.mainTexture = this.data.DefaultIcon;
				this.button.isEnabled = false;
				this.button.defaultColor = this.m_DefaultColor;
				return;
			}

			AddMaidData(maid);

			if (maid.IsCrcBody)
			{
				this.button.isEnabled = false;
				foreach (MPN parentMpn in this.data.CrcMpnList)
				{
					/*
					if (maid.body0.GetSlotLoaded(parentMpn))
					{
						this.button.isEnabled = true;
						break;
					}
					*/
				}
				if (this.data.CrcMpnList.Count <= 0 && this.button.gameObject.activeSelf)
				{
					this.button.gameObject.SetActive(false);
					wf.Utility.ResetNGUI(this.button.transform.parent.GetComponent<UIGrid>());
				}
			}
			else
			{
				string strFileName = maid.GetProp(this.mpn).strFileName;
				this.button.isEnabled = (!string.IsNullOrEmpty(strFileName) && strFileName.IndexOf("_del") < 0 && this.maidMaidInfoLookup.ContainsKey(maid));
			}
			if (!this.button.isEnabled || maid.IsCrcBody)
			{
				this.icon.mainTexture = this.data.DefaultIcon;
				if (maid.IsCrcBody)
				{
					this.button.defaultColor = (this.button.hover = ((!this.maidMaidInfoLookup[maid].IsUndress) ? this.m_DefaultColor : this.m_UndressColor));
				}
			}
			else
			{
				this.icon.mainTexture = this.maidMaidInfoLookup[maid].ItemIcon;
				this.button.defaultColor = (this.button.hover = ((!this.maidMaidInfoLookup[maid].IsUndress) ? this.m_DefaultColor : this.m_UndressColor));
			}
		}
	}
}
