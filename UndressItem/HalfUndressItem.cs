using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
	class HalfUndressItem : IUndressItem
	{
		public Texture Icon { get; private set; }

        public bool Active => true;

        public Color Color => Color.white;

		Maid maid;
		PartsData partsData;
		int currentPropIndex = 0;
		IList<string> props;

		public void Dress()
        {
			this.currentPropIndex = 0;
			SetProp(this.currentPropIndex);
        }

        public void Toggle()
        {
			this.currentPropIndex++;
			if (this.currentPropIndex >= this.props.Count())
			{
				this.Dress();
			} else
            {
				SetProp(this.currentPropIndex);
			}
		}

        public void Undress()
        {
			this.currentPropIndex = this.props.Count - 1;
			SetProp(this.currentPropIndex);
        }

		private void SetProp(int idx)
		{
			var filename = this.props[idx];
			Log.LogVerbose("Setting prop [{0}] for mpn [{1}]", filename, this.partsData.mpn);
			maid.SetProp(this.partsData.mpn, filename, filename.ToLower().GetHashCode(), true);
			maid.AllProcPropSeqStart();
		}


		public static IUndressItem ForMaid(Maid maid, PartsData part)
        {
			MaidProp prop = maid.GetProp(part.mpn);
			if (string.IsNullOrEmpty(prop.strFileName) || prop.strFileName.IndexOf("_del") >= 1)
			{
				return null;
			}

			var halfUndressProps = GetHalfUndressMenuItems(prop.strFileName);
			if (halfUndressProps.Count() <= 1)
            {
				return null;
            } 

			return new HalfUndressItem()
			{
				maid = maid,
				partsData = part,
				Icon = UndressItem.GetIcon(prop.strFileName),
				props = halfUndressProps
			};
        }

		private static IEnumerable<string> GetPororiFiles(string basename, string ext)
		{
			string[] postfixes = { "_porori" };
			foreach (var postfix in postfixes)
			{
				foreach (var name in GetPostfixFiles(
					basename, postfix, ext))
                {
					yield return name;
                }
			}
		}

		private static IEnumerable<string> GetPostfixFiles(string basename, string postfix, string ext)
		{
			// _porori1, porori2, etc
			int index = 1;
			while (true)
			{
				string undressItemName;
				if (index == 1)
				{
					undressItemName = basename + postfix + ext;
				}
				else
				{
					undressItemName = basename + postfix + index + ext;
				}

				if (GameUty.IsExistFile(undressItemName))
				{
					Log.LogVerbose("Found half undress item {0}", undressItemName);
					yield return undressItemName;
				}
				else
				{
					break;
				}
				index++;
			}

		}

		private static IEnumerable<string> GetIFiles(string basename, string ext)
		{
			if (!basename.EndsWith("_i_")) yield break;

			string[] infixes = { "_zurashi", "_mekure", "_mekure_back", "_po" };
			foreach (var infix in infixes)
			{
				string newname = basename.Insert(basename.Length - 3, infix) + ext;
				if (GameUty.IsExistFile(newname))
				{
					Log.LogVerbose("Found half undress item {0}", newname);
					yield return newname;
				}
			}
		}

		private static IList<string> GetHalfUndressMenuItems(string mainItem)
		{
            var items = new List<string>
            {
                mainItem
            };

            string baseName = Path.GetFileNameWithoutExtension(mainItem).ToLower();
			string ext = mainItem.EndsWith(".mod", true, null) ? ".mod" : ".menu";

			foreach (var name in GetIFiles(baseName, ext))
			{
				items.Add(name);
			}

			foreach (var name in GetPororiFiles(baseName, ext))
			{
				items.Add(name);
			}

			return items;
		}

	}
}
