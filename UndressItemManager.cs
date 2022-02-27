using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using COM3D2.UndressUtil.Plugin.UndressItem;

namespace COM3D2.UndressUtil.Plugin
{
    class UndressItemManager: MonoBehaviour
    {
		private PartsData data;
		private UIButton button;
		private UITexture icon;

		private Dictionary<Maid, IUndressItem>
			maidUndressItemLookup = new Dictionary<Maid, IUndressItem>();

		private Dictionary<Maid, IUndressItem>
			maidHalfUndressItemLookup = new Dictionary<Maid, IUndressItem>();


		private Color defaultColor = Color.white;

		private UndressWindowManager undressWindowManager;
		private MaidSelectPanelManager maidSelectManager;
		private MaidTracker maidTracker;

		public void Awake()
        {
			button = base.GetComponent<UIButton>();
			icon = base.GetComponent<UITexture>();
		}

		public void Start()
        {
			undressWindowManager = this.gameObject.GetComponentInParent<UndressWindowManager>();
			Assert.IsNotNull(undressWindowManager.MaidSelectPanelManager, "Could not find MaidSelectManager");
			maidSelectManager = undressWindowManager.MaidSelectPanelManager;
			maidSelectManager.MaidSelected.AddListener(this.UpdateState);

			EventDelegate.Add(this.button.onClick, new EventDelegate.Callback(this.Toggle));

			maidTracker = gameObject.GetComponentInParent<MaidTracker>();
			Assert.IsNotNull(maidTracker, "Could not find MaidTracker");
			maidTracker.MaidActivated.AddListener(this.AddMaidData);
			undressWindowManager.UndressModeChangeEvent.AddListener(this.UpdateState);

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

		public bool IsEnable
		{
			get
			{
				return this.button.isEnabled;
			}
		}

		public void Init(MPN mpn, PartsData data)
		{
			this.data = data;
			this.icon.mainTexture = this.data.DefaultIcon;
		}

		public IUndressItem GetUndressItem()
        {
			var selectMaid = maidSelectManager.SelectedMaid;
			var lookup = maidUndressItemLookup;

			if (undressWindowManager.mode == UndressWindowManager.UndressMode.HALFUNDRESS)
            {
				lookup = maidHalfUndressItemLookup;
			}

			return lookup[selectMaid];
		}

		public void Dress()
        {
			var item = GetUndressItem();
			if (item != null)
            {
				item.Dress();
				UpdateState();
			}
		}

		public void Undress()
		{
			var item = GetUndressItem();
			if (item != null)
			{
				item.Undress();
				UpdateState();
			}
		}

		public void Toggle()
		{
			var item = GetUndressItem();
			if (item != null)
			{
				item.Toggle();
				UpdateState();
			}
		}

		public void AddMaidData(Maid maid)
        {
			this.AddMaidData(maid, false);
        }

		public void AddMaidData(Maid maid, bool forceUpdate)
		{
			if (!maid)
			{
				return;
			}

			if (forceUpdate || !maidUndressItemLookup.ContainsKey(maid))
            {
				this.maidUndressItemLookup[maid] = UndressItem.UndressItem.ForMaid(maid, this.data);
			}

			if (forceUpdate || !maidHalfUndressItemLookup.ContainsKey(maid))
            {
				if(!maid.IsCrcBody)
                {
					this.maidHalfUndressItemLookup[maid] = UndressItem.HalfUndressItem.ForMaid(maid, this.data);
				} else
                {
					this.maidHalfUndressItemLookup[maid] = CostumeItem.ForMaid(maid, this.data);
				}
				
			}
		}

		private void SetDisabled()
        {
			this.icon.mainTexture = this.data.DefaultIcon;
			this.button.isEnabled = false;
			this.button.defaultColor = this.defaultColor;
			this.gameObject.SetActive(false);
			undressWindowManager.RepositionItemGrid();
		}

		public virtual void UpdateState(Maid maid)
		{
			if (maid == null)
            {
				SetDisabled();
				return;
			}

			AddMaidData(maid);
			UpdateState();
			undressWindowManager.RepositionItemGrid();
		}

		public virtual void UpdateState()
        {
			var item = GetUndressItem();
			if (item != null)
			{
				this.icon.mainTexture = item.Icon ?? this.data.DefaultIcon;
				this.button.defaultColor = item.Color;
				this.button.isEnabled = true;
				this.button.UpdateColor(false);
				this.gameObject.SetActive(true);
			}
			else
			{
				SetDisabled();
			}
		}

	}
}
