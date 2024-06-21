using System;
using System.Net;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000057 RID: 87
	internal struct UwaWebResponse
	{
		// Token: 0x04000275 RID: 629
		public int HttpStatusCode;

		// Token: 0x04000276 RID: 630
		public string HttpErrorMessage;

		// Token: 0x04000277 RID: 631
		public WebHeaderCollection HttpHeaders;

		// Token: 0x04000278 RID: 632
		public string data;

		// Token: 0x04000279 RID: 633
		public byte[] binData;

		// Token: 0x0400027A RID: 634
		public bool ok;
	}
}
