using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x0200009A RID: 154
	[ComVisible(false)]
	public sealed class GZipConstants
	{
		// Token: 0x06000754 RID: 1876 RVA: 0x000310C8 File Offset: 0x0002F2C8
		private GZipConstants()
		{
		}

		// Token: 0x04000489 RID: 1161
		public const int GZIP_MAGIC = 8075;

		// Token: 0x0400048A RID: 1162
		public const int FTEXT = 1;

		// Token: 0x0400048B RID: 1163
		public const int FHCRC = 2;

		// Token: 0x0400048C RID: 1164
		public const int FEXTRA = 4;

		// Token: 0x0400048D RID: 1165
		public const int FNAME = 8;

		// Token: 0x0400048E RID: 1166
		public const int FCOMMENT = 16;
	}
}
