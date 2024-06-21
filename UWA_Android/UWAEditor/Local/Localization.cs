using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace UWAEditor.Local
{
	// Token: 0x02000005 RID: 5
	public class Localization
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002C06 File Offset: 0x00000E06
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002C0E File Offset: 0x00000E0E
		public Localization.eWebSite WebSite { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002C17 File Offset: 0x00000E17
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002C1F File Offset: 0x00000E1F
		public Localization.eLocale Locale { get; private set; }

		// Token: 0x0600001B RID: 27 RVA: 0x00002C28 File Offset: 0x00000E28
		public static string StreamToString(Stream stream)
		{
			stream.Position = 0L;
			string result;
			using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002C74 File Offset: 0x00000E74
		private string LoadLocale(string locale)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("UWAEditor.Res.Locales." + locale + ".json");
			bool flag = manifestResourceStream != null;
			string result;
			if (flag)
			{
				string text = Localization.StreamToString(manifestResourceStream);
				result = text;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002CBC File Offset: 0x00000EBC
		private Localization()
		{
			this.Locale = Localization.eLocale.zh_CN;
			I18n instance = I18n.Instance;
			I18n.Configure("Locales/", this.Locale.ToString(), true, new string[]
			{
				"zh_CN",
				"en_US",
				"ja_JP"
			}, new I18n.GetLocaleFile(this.LoadLocale));
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002D29 File Offset: 0x00000F29
		public void SetWebSite(Localization.eWebSite site)
		{
			this.WebSite = site;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002D34 File Offset: 0x00000F34
		public void SetLocale(Localization.eLocale locale)
		{
			this.Locale = locale;
			I18n.SetLocale(locale.ToString());
			bool flag = this.OnLocalize != null;
			if (flag)
			{
				this.OnLocalize();
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002D78 File Offset: 0x00000F78
		public string Get(string key)
		{
			return I18n.Instance.__(key, new object[0]);
		}

		// Token: 0x04000009 RID: 9
		public Localization.GameVoidDelegate OnLocalize;

		// Token: 0x0400000A RID: 10
		public static readonly Localization Instance = new Localization();

		// Token: 0x0200000F RID: 15
		// (Invoke) Token: 0x060000AE RID: 174
		public delegate void GameVoidDelegate();

		// Token: 0x02000010 RID: 16
		public enum eWebSite
		{
			// Token: 0x04000026 RID: 38
			CN,
			// Token: 0x04000027 RID: 39
			US,
			// Token: 0x04000028 RID: 40
			JP
		}

		// Token: 0x02000011 RID: 17
		public enum eLocale
		{
			// Token: 0x0400002A RID: 42
			zh_CN,
			// Token: 0x0400002B RID: 43
			en_US,
			// Token: 0x0400002C RID: 44
			ja_JP
		}
	}
}
