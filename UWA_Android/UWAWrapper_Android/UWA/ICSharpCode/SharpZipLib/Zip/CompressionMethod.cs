using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200005C RID: 92
	[ComVisible(false)]
	public enum CompressionMethod
	{
		// Token: 0x04000267 RID: 615
		Stored,
		// Token: 0x04000268 RID: 616
		Deflated = 8,
		// Token: 0x04000269 RID: 617
		Deflate64,
		// Token: 0x0400026A RID: 618
		BZip2 = 11,
		// Token: 0x0400026B RID: 619
		WinZipAES = 99
	}
}
