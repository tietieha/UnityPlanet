using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x020000A9 RID: 169
	[ComVisible(false)]
	public sealed class GZipConstants
	{
		// Token: 0x06000830 RID: 2096 RVA: 0x0003DE80 File Offset: 0x0003C080
		private GZipConstants()
		{
		}

		// Token: 0x040004FC RID: 1276
		public const int GZIP_MAGIC = 8075;

		// Token: 0x040004FD RID: 1277
		public const int FTEXT = 1;

		// Token: 0x040004FE RID: 1278
		public const int FHCRC = 2;

		// Token: 0x040004FF RID: 1279
		public const int FEXTRA = 4;

		// Token: 0x04000500 RID: 1280
		public const int FNAME = 8;

		// Token: 0x04000501 RID: 1281
		public const int FCOMMENT = 16;
	}
}
