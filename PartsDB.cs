using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{

	class PartsDB
    {
		public class PartsData
		{
			public MPN mpn;
			public List<TBody.SlotID> SlotIDlist = new List<TBody.SlotID>();
			public Texture DefaultIcon;
			public List<MPN> CrcMpnList = new List<MPN>();
		}


		private Dictionary<MPN, PartsData> mpnPartsData = new Dictionary<MPN, PartsData>();
		private MPN lastMPN;

		public PartsDB()
        {
			KasaiUtility.CsvRead("csv_rhythm_action/undress_parts_data.nei", this.ReadPartsData, 0, 1, null);
		}

		public IEnumerable<PartsData> GetPartsData()
        {
			return mpnPartsData.Values;
        }

		private void ReadPartsData(CsvParser csv, int cx, int cy)
		{
			switch (cx)
			{
				case 0:
					{
						MPN key = (MPN)Enum.Parse(typeof(MPN), csv.GetCellAsString(cx, cy));
						var part = new PartsData();
						part.mpn = key;
						this.mpnPartsData.Add(key, part);
						lastMPN = key;
						break;
					}
				case 1:
					foreach (string value in csv.GetCellAsString(cx, cy).Split(new char[]
					{
				','
					}))
					{
						mpnPartsData[lastMPN].SlotIDlist.Add((TBody.SlotID)Enum.Parse(typeof(TBody.SlotID), value));
					}
					break;
				case 2:
					{
						string text = csv.GetCellAsString(cx, cy);
						if (text.IndexOf(".tex") < 0)
						{
							text += ".tex";
						}
						if (!GameUty.FileSystem.IsExistentFile(text))
						{
							Log.LogError("Texture [{0}] does not exist", text);
						}
						Texture2D defaultIcon = ImportCM.CreateTexture(text);
						mpnPartsData[lastMPN].DefaultIcon = defaultIcon;
						break;
					}
				case 4:
					{
						bool flag = csv.GetCellAsString(cx, cy) == "○";
						if (Product.isPublic && !flag)
						{
							return;
						}
						break;
					}
				case 5:
					foreach (string value2 in csv.GetCellAsString(cx, cy).Split(new char[]
					{
				','
					}))
					{
						MPN mpn = (MPN)Enum.Parse(typeof(MPN), value2);
						if (mpn != MPN.null_mpn)
						{
							mpnPartsData[lastMPN].CrcMpnList.Add(mpn);
						}
					}
					break;
			}
		}
	}
}
