using System;
using System.Net;
using System.Text;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000056 RID: 86
	internal class UwaWebsiteWebClient : WebClient
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x000219D4 File Offset: 0x0001FBD4
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x000219DC File Offset: 0x0001FBDC
		public CookieContainer CookieContainer { get; set; }

		// Token: 0x060003C2 RID: 962 RVA: 0x000219E8 File Offset: 0x0001FBE8
		public UwaWebsiteWebClient() : this(new CookieContainer(100))
		{
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x000219FC File Offset: 0x0001FBFC
		public UwaWebsiteWebClient(CookieContainer cookies)
		{
			this.CookieContainer = cookies;
			base.Encoding = Encoding.UTF8;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00021A1C File Offset: 0x0001FC1C
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			bool flag = webRequest is HttpWebRequest;
			if (flag)
			{
				(webRequest as HttpWebRequest).CookieContainer = this.CookieContainer;
			}
			return (HttpWebRequest)webRequest;
		}
	}
}
