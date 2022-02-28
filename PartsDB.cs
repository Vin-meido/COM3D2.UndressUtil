using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{
	public class PartsData
	{
		public MPN mpn;
		public List<TBody.SlotID> SlotIDlist = new List<TBody.SlotID>();
		public Texture DefaultIcon;
		public List<MPN> CrcMpnList = new List<MPN>();
	}

	public class PartsDB
    {
		private Dictionary<MPN, PartsData> mpnPartsData = new Dictionary<MPN, PartsData>();
		private MPN lastMPN;

		public PartsDB()
        {
			KasaiUtility.CsvRead("csv_rhythm_action/undress_parts_data.nei", this.ReadPartsData, 0, 1, null);
			AddCrcManParts();
		}

		void AddCrcManParts()
        {
			var defaultIcon = ImportCM.CreateTexture("customview_icon_tops.tex");
			if(Product.isCREditSystemSupport)
            {
				AddCrcManMpnPart("jacket");
				AddCrcManMpnPart("vest");
				AddCrcManMpnPart("shirt");
			}
		}


		void AddCrcManMpnPart(string part)
        {
			var mpn = EnumParse<MPN>(part, MPN.null_mpn);
			var slot = EnumParse<TBody.SlotID>(part);

			if(mpn != MPN.null_mpn)
            {
				mpnPartsData[mpn] = GetCustomPartDataFor(mpn, slot);
            }
        }

		static PartsData GetCustomPartDataFor(MPN mpn, TBody.SlotID slot)
        {
			return new PartsData()
			{
				mpn = mpn,
				DefaultIcon = ImportCM.CreateTexture("customview_icon_tops.tex"),
				SlotIDlist = new List<TBody.SlotID>() { slot },
				CrcMpnList = new List<MPN>() { mpn }
			};
		}


		static T EnumParse<T>(string value) where T: Enum
        {
			return (T)Enum.Parse(typeof(T), value);
        }

		static T EnumParse<T>(string value, T def) where T : Enum
		{
			try
            {
				return (T)Enum.Parse(typeof(T), value);
			}
			catch(ArgumentException)
            {
				return def;
            }
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
						MPN key = EnumParse<MPN>(csv.GetCellAsString(cx, cy));
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
						mpnPartsData[lastMPN].SlotIDlist.Add(EnumParse<TBody.SlotID>(value));
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
				case 6:
					foreach (string value2 in csv.GetCellAsString(cx, cy).Split(new char[]
					{
				','
					}))
					{
						MPN mpn = EnumParse(value2, MPN.null_mpn);
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
