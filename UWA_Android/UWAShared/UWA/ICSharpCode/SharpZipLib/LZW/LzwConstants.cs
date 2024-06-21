using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x020000A6 RID: 166
	[ComVisible(false)]
	public sealed class LzwConstants
	{
		// Token: 0x06000816 RID: 2070 RVA: 0x0003D440 File Offset: 0x0003B640
		private LzwConstants()
		{
		}

		// Token: 0x040004D9 RID: 1241
		public const int MAGIC = 8093;

		// Token: 0x040004DA RID: 1242
		public const int MAX_BITS = 16;

		// Token: 0x040004DB RID: 1243
		public const int BIT_MASK = 31;

		// Token: 0x040004DC RID: 1244
		public const int EXTENDED_MASK = 32;

		// Token: 0x040004DD RID: 1245
		public const int RESERVED_MASK = 96;

		// Token: 0x040004DE RID: 1246
		public const int BLOCK_MODE_MASK = 128;

		// Token: 0x040004DF RID: 1247
		public const int HDR_SIZE = 3;

		// Token: 0x040004E0 RID: 1248
		public const int INIT_BITS = 9;
	}
}
