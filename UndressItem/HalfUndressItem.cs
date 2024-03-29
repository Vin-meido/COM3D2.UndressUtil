﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using COM3D2.UndressUtil.Plugin.Hooks;
using System.Text.RegularExpressions;

namespace COM3D2.UndressUtil.Plugin.UndressItem
{
	class HalfUndressItem : IUndressItem
	{
		public Texture Icon { get; private set; }

        public bool Active => true;
		public bool Available { get; private set; }

        public Color Color => Color.white;

		Maid maid;
		PartsData partsData;
		int currentPropIndex = 0;
		IList<string> props;
		string currentMainPropFilename;


		public void Dress()
        {
			this.currentPropIndex = 0;
			Apply();
        }

        public void Toggle()
        {
			this.currentPropIndex++;
			if (this.currentPropIndex >= this.props.Count()) this.currentPropIndex = 0;
			Apply();
		}

        public void Undress()
        {
			this.currentPropIndex = this.props.Count - 1;
			Apply();
        }

		public void Apply()
        {
			SetProp(this.currentPropIndex);
		}

		private void SetProp(int idx)
		{
			var filename = this.props[idx];
			Log.LogVerbose("Setting prop [{0}] for mpn [{1}]", filename, this.partsData.mpn);

			MaidHooks.Supress(() =>
			{
				maid.SetProp(this.partsData.mpn, filename, filename.ToLower().GetHashCode(), true);
				maid.AllProcProp();
			});
		}


		public static IUndressItem ForMaid(Maid maid, PartsData part)
        {
			var item = new HalfUndressItem()
			{
				maid = maid,
				partsData = part,
			};

			item.Update();
			return item;
        }

		public void Update()
        {
			MaidProp prop = maid.GetProp(partsData.mpn);
			string propFilename = GetMaidPropBaseNonPororiFilename(prop);
			
			if (currentMainPropFilename == propFilename)
			{
				// same no need to update.
				return;
			}

			currentMainPropFilename = propFilename;
			Log.LogVerbose("Half undress item {0} {1}", partsData.mpn, currentMainPropFilename);

			if (string.IsNullOrEmpty(currentMainPropFilename) || currentMainPropFilename.IndexOf("_del") >= 1)
			{
				Available = false;
				Icon = partsData.DefaultIcon;
				return;
			}

			var halfUndressProps = GetHalfUndressMenuItems(currentMainPropFilename);
			if (halfUndressProps.Count() <= 1)
			{
				Available = false;
				Icon = partsData.DefaultIcon;
				return;
			}

			Available = true;
			Icon = UndressItem.GetIcon(currentMainPropFilename);
			props = halfUndressProps;
		}

		private static string GetMaidPropBaseNonPororiFilename(MaidProp prop)
        {
			string filename = prop.strTempFileName;
			if (string.IsNullOrEmpty(filename))
			{
				filename = prop.strFileName;
			}

			if (filename.Contains("_porori"))
            {
				filename = Regex.Replace(filename, "_porori[0-9]+", "");
			}

			string[] infixes = { "_zurashi", "_mekure_back", "_mekure", "_po" };
			foreach(var infix in infixes)
            {
				filename = filename.Replace(infix, "");
            }

			return filename;
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
