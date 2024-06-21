using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006B RID: 107
	[ComVisible(false)]
	public enum CompressionMethod
	{
		// Token: 0x040002DA RID: 730
		Stored,
		// Token: 0x040002DB RID: 731
		Deflated = 8,
		// Token: 0x040002DC RID: 732
		Deflate64,
		// Token: 0x040002DD RID: 733
		BZip2 = 11,
		// Token: 0x040002DE RID: 734
		WinZipAES = 99
	}
}
