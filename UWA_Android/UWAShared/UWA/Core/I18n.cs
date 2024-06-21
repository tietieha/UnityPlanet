using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UWA.Core
{
	// Token: 0x0200005D RID: 93
	[ComVisible(false)]
	public sealed class I18n
	{
		// Token: 0x06000422 RID: 1058 RVA: 0x000258BC File Offset: 0x00023ABC
		private I18n()
		{
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x000258C8 File Offset: 0x00023AC8
		public static I18n Instance
		{
			get
			{
				return I18n.instance;
			}
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x000258E8 File Offset: 0x00023AE8
		private static void InitConfig()
		{
			bool flag = I18n.locales.Contains(I18n._currentLocale);
			if (flag)
			{
				bool flag2 = I18n._getFileCb != null;
				string aJSON;
				if (flag2)
				{
					aJSON = I18n._getFileCb(I18n._currentLocale);
				}
				else
				{
					string text = I18n._localePath + I18n._currentLocale;
					TextAsset textAsset = Resources.Load(text) as TextAsset;
					aJSON = textAsset.text;
				}
				I18n.config = JSON.Parse(aJSON);
			}
			else
			{
				bool isLoggingMissing = I18n._isLoggingMissing;
				if (isLoggingMissing)
				{
					Debug.Log("Missing: locale [" + I18n._currentLocale + "] not found in supported list");
				}
			}
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0002599C File Offset: 0x00023B9C
		public static string GetLocale()
		{
			return I18n._currentLocale;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000259BC File Offset: 0x00023BBC
		public static void SetLocale(string newLocale = null)
		{
			bool flag = newLocale != null;
			if (flag)
			{
				I18n._currentLocale = newLocale;
				I18n.InitConfig();
			}
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x000259E8 File Offset: 0x00023BE8
		public static void SetPath(string localePath = null)
		{
			bool flag = localePath != null;
			if (flag)
			{
				I18n._localePath = localePath;
				I18n.InitConfig();
			}
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00025A14 File Offset: 0x00023C14
		public static void Configure(string localePath = null, string newLocale = null, bool logMissing = true, string[] overridelocals = null, I18n.GetLocaleFile callback = null)
		{
			bool flag = overridelocals != null;
			if (flag)
			{
				I18n.locales = overridelocals;
			}
			bool flag2 = callback != null;
			if (flag2)
			{
				I18n._getFileCb = callback;
			}
			I18n._isLoggingMissing = logMissing;
			I18n.SetPath(localePath);
			I18n.SetLocale(newLocale);
			I18n.InitConfig();
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00025A64 File Offset: 0x00023C64
		public string __(string key, params object[] args)
		{
			bool flag = I18n.config == null;
			if (flag)
			{
				I18n.InitConfig();
			}
			string text = key;
			bool flag2 = I18n.config[key] != null;
			if (flag2)
			{
				bool flag3 = I18n.config[key].Count == 0;
				if (flag3)
				{
					text = I18n.config[key];
				}
				else
				{
					text = this.FindSingularOrPlural(key, args);
				}
				bool flag4 = args.Length != 0;
				if (flag4)
				{
					text = string.Format(text, args);
				}
			}
			else
			{
				bool isLoggingMissing = I18n._isLoggingMissing;
				if (isLoggingMissing)
				{
					Debug.Log("Missing translation for:" + key);
				}
			}
			return text;
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00025B34 File Offset: 0x00023D34
		private string FindSingularOrPlural(string key, object[] args)
		{
			JSONClass asObject = I18n.config[key].AsObject;
			string result = key;
			int countAmount = this.GetCountAmount(args);
			int num = countAmount;
			string text;
			if (num != 0)
			{
				if (num != 1)
				{
					text = "other";
				}
				else
				{
					text = "one";
				}
			}
			else
			{
				text = "zero";
			}
			bool flag = asObject[text] != null;
			if (flag)
			{
				result = asObject[text];
			}
			else
			{
				bool isLoggingMissing = I18n._isLoggingMissing;
				if (isLoggingMissing)
				{
					Debug.Log("Missing singPlurKey:" + text + " for:" + key);
				}
			}
			return result;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00025BFC File Offset: 0x00023DFC
		private int GetCountAmount(object[] args)
		{
			int num = 0;
			bool flag = args.Length != 0 && this.IsNumeric(args[0]);
			if (flag)
			{
				num = Math.Abs(Convert.ToInt32(args[0]));
				bool flag2 = num == 1 && Math.Abs(Convert.ToDouble(args[0])) != 1.0;
				if (flag2)
				{
					num = 2;
				}
				else
				{
					bool flag3 = num == 0 && Math.Abs(Convert.ToDouble(args[0])) != 0.0;
					if (flag3)
					{
						num = 2;
					}
				}
			}
			return num;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00025CB4 File Offset: 0x00023EB4
		private bool IsNumeric(object Expression)
		{
			bool flag = Expression == null || Expression is DateTime;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = Expression is short || Expression is int || Expression is long || Expression is decimal || Expression is float || Expression is double || Expression is bool;
				result = flag2;
			}
			return result;
		}

		// Token: 0x040002A3 RID: 675
		private static JSONNode config = null;

		// Token: 0x040002A4 RID: 676
		private static readonly I18n instance = new I18n();

		// Token: 0x040002A5 RID: 677
		private static string[] locales = new string[]
		{
			"zh_CN",
			"en_US",
			"ja_JP",
			"fr_FR",
			"es_ES"
		};

		// Token: 0x040002A6 RID: 678
		private static string _currentLocale = "zh_CN";

		// Token: 0x040002A7 RID: 679
		private static string _localePath = "Locales/";

		// Token: 0x040002A8 RID: 680
		private static bool _isLoggingMissing = true;

		// Token: 0x040002A9 RID: 681
		private static I18n.GetLocaleFile _getFileCb = null;

		// Token: 0x02000140 RID: 320
		// (Invoke) Token: 0x06000A7F RID: 2687
		public delegate string GetLocaleFile(string locale);
	}
}
