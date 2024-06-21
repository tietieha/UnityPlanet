using System;
using System.Runtime.InteropServices;

namespace UWACore.Util.NetWork
{
	// Token: 0x0200004C RID: 76
	[ComVisible(false)]
	public class Using_request
	{
		// Token: 0x06000335 RID: 821 RVA: 0x0001BEF4 File Offset: 0x0001A0F4
		public string Start(string datakey, int time, string device, int os)
		{
			return "";
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001BF14 File Offset: 0x0001A114
		public string OSS(string datakey, int platform = 0)
		{
			return "";
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001BF34 File Offset: 0x0001A134
		public string Upload(string datakey, string url)
		{
			return "";
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0001BF54 File Offset: 0x0001A154
		public string Submit(string datakey)
		{
			return "";
		}

		// Token: 0x04000244 RID: 580
		public static readonly Using_request Instance = new Using_request();
	}
}
