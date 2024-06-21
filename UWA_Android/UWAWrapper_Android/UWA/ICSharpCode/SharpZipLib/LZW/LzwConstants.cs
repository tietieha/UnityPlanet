using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x02000097 RID: 151
	[ComVisible(false)]
	public sealed class LzwConstants
	{
		// Token: 0x0600073A RID: 1850 RVA: 0x00030688 File Offset: 0x0002E888
		private LzwConstants()
		{
		}

		// Token: 0x04000466 RID: 1126
		public const int MAGIC = 8093;

		// Token: 0x04000467 RID: 1127
		public const int MAX_BITS = 16;

		// Token: 0x04000468 RID: 1128
		public const int BIT_MASK = 31;

		// Token: 0x04000469 RID: 1129
		public const int EXTENDED_MASK = 32;

		// Token: 0x0400046A RID: 1130
		public const int RESERVED_MASK = 96;

		// Token: 0x0400046B RID: 1131
		public const int BLOCK_MODE_MASK = 128;

		// Token: 0x0400046C RID: 1132
		public const int HDR_SIZE = 3;

		// Token: 0x0400046D RID: 1133
		public const int INIT_BITS = 9;
	}
}
