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

		Maid selectedMaid;
		UndressWindowManager.UndressMode currentMode;

		void Awake()
        {
			button = base.GetComponent<UIButton>();
			icon = base.GetComponent<UITexture>();
		}

		void Start()
        {
			EventDelegate.Add(this.button.onClick, new EventDelegate.Callback(this.Toggle));
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

		public void SetMaid(Maid maid, UndressWindowManager.UndressMode mode)
        {
			selectedMaid = maid;
			currentMode = mode;

			if (maid != null)
            {
				if (mode == UndressWindowManager.UndressMode.NORMAL)
				{
					AddMaidUndressData(maid);
				}
				else
				{
					AddMaidHalfUndressData(maid);
				}
			}

			this.UpdateState();
		}

		public void Reset()
        {
			maidUndressItemLookup.Clear();
			maidHalfUndressItemLookup.Clear();
			selectedMaid = null;
			currentMode = UndressWindowManager.UndressMode.NORMAL;
        }

		public IUndressItem GetUndressItem()
        {
			if (selectedMaid == null) return null;

			var lookup = maidUndressItemLookup;

			if (currentMode == UndressWindowManager.UndressMode.HALFUNDRESS)
            {
				lookup = maidHalfUndressItemLookup;
			}

			return lookup[selectedMaid];
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

		void AddMaidUndressData(Maid maid)
        {
			if (!maidUndressItemLookup.ContainsKey(maid))
			{
				this.maidUndressItemLookup[maid] = UndressItem.UndressItem.ForMaid(maid, this.data);
			}
			else
			{
				this.maidUndressItemLookup[maid].Update();
			}

		}

		void AddMaidHalfUndressData(Maid maid)
        {
			if (!maidHalfUndressItemLookup.ContainsKey(maid))
			{
				if (!maid.IsCrcBody)
				{
					this.maidHalfUndressItemLookup[maid] = UndressItem.HalfUndressItem.ForMaid(maid, this.data);
				}
				else
				{
					this.maidHalfUndressItemLookup[maid] = CostumeItem.ForMaid(maid, this.data);
				}

			}
			else
			{
				this.maidHalfUndressItemLookup[maid].Update();
			}
		}

		private void SetDisabled()
        {
			this.icon.mainTexture = this.data.DefaultIcon;
			this.button.isEnabled = false;
			this.button.defaultColor = this.defaultColor;
			this.gameObject.SetActive(false);
		}

		void UpdateState()
        {
			var item = GetUndressItem();
			if (item != null && item.Available)
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
