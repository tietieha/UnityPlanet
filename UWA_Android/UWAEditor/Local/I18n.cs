using System;
using System.Linq;
using UnityEngine;

namespace UWAEditor.Local
{
	// Token: 0x02000006 RID: 6
	public sealed class I18n
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002E09 File Offset: 0x00001009
		private I18n()
		{
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002E14 File Offset: 0x00001014
		public static I18n Instance
		{
			get
			{
				return I18n.instance;
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002E2C File Offset: 0x0000102C
		private static void InitConfig()
		{
			bool flag = I18n.locales.Contains(I18n._currentLocale);
			if (flag)
			{
				bool flag2 = I18n._getFileCb != null;
				string text;
				if (flag2)
				{
					text = I18n._getFileCb(I18n._currentLocale);
				}
				else
				{
					string text2 = I18n._localePath + I18n._currentLocale;
					TextAsset textAsset = Resources.Load(text2) as TextAsset;
					text = textAsset.text;
				}
				bool flag3 = text != null;
				if (flag3)
				{
					I18n.config = JSON.Parse(text);
				}
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

		// Token: 0x06000026 RID: 38 RVA: 0x00002ED8 File Offset: 0x000010D8
		public static string GetLocale()
		{
			return I18n._currentLocale;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002EF0 File Offset: 0x000010F0
		public static void SetLocale(string newLocale = null)
		{
			bool flag = newLocale != null;
			if (flag)
			{
				I18n._currentLocale = newLocale;
				I18n.InitConfig();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F14 File Offset: 0x00001114
		public static void SetPath(string localePath = null)
		{
			bool flag = localePath != null;
			if (flag)
			{
				I18n._localePath = localePath;
				I18n.InitConfig();
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002F38 File Offset: 0x00001138
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

		// Token: 0x0600002A RID: 42 RVA: 0x00002F80 File Offset: 0x00001180
		public string __(string key, params object[] args)
		{
			bool flag = I18n.config == null;
			if (flag)
			{
				I18n.InitConfig();
			}
			bool flag2 = I18n.config == null;
			string result;
			if (flag2)
			{
				result = key;
			}
			else
			{
				string text = key;
				bool flag3 = I18n.config[key] != null;
				if (flag3)
				{
					bool flag4 = I18n.config[key].Count == 0;
					if (flag4)
					{
						text = I18n.config[key];
					}
					else
					{
						text = this.FindSingularOrPlural(key, args);
					}
					bool flag5 = args.Length != 0;
					if (flag5)
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
				result = text;
			}
			return result;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000304C File Offset: 0x0000124C
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

		// Token: 0x0600002C RID: 44 RVA: 0x000030EC File Offset: 0x000012EC
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

		// Token: 0x0600002D RID: 45 RVA: 0x0000317C File Offset: 0x0000137C
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

		// Token: 0x0400000D RID: 13
		private static JSONNode config = null;

		// Token: 0x0400000E RID: 14
		private static readonly I18n instance = new I18n();

		// Token: 0x0400000F RID: 15
		private static string[] locales = new string[]
		{
			"en_US",
			"fr_FR",
			"es_ES"
		};

		// Token: 0x04000010 RID: 16
		private static string _currentLocale = "en_US";

		// Token: 0x04000011 RID: 17
		private static string _localePath = "Locales/";

		// Token: 0x04000012 RID: 18
		private static bool _isLoggingMissing = true;

		// Token: 0x04000013 RID: 19
		private static I18n.GetLocaleFile _getFileCb = null;

		// Token: 0x02000012 RID: 18
		// (Invoke) Token: 0x060000B2 RID: 178
		public delegate string GetLocaleFile(string locale);
	}
}
