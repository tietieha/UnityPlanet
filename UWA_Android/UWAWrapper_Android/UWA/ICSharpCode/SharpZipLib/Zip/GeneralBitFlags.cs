using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200005E RID: 94
	[Flags]
	[ComVisible(false)]
	public enum GeneralBitFlags
	{
		// Token: 0x0400027C RID: 636
		Encrypted = 1,
		// Token: 0x0400027D RID: 637
		Method = 6,
		// Token: 0x0400027E RID: 638
		Descriptor = 8,
		// Token: 0x0400027F RID: 639
		ReservedPKware4 = 16,
		// Token: 0x04000280 RID: 640
		Patched = 32,
		// Token: 0x04000281 RID: 641
		StrongEncryption = 64,
		// Token: 0x04000282 RID: 642
		Unused7 = 128,
		// Token: 0x04000283 RID: 643
		Unused8 = 256,
		// Token: 0x04000284 RID: 644
		Unused9 = 512,
		// Token: 0x04000285 RID: 645
		Unused10 = 1024,
		// Token: 0x04000286 RID: 646
		UnicodeText = 2048,
		// Token: 0x04000287 RID: 647
		EnhancedCompress = 4096,
		// Token: 0x04000288 RID: 648
		HeaderMasked = 8192,
		// Token: 0x04000289 RID: 649
		ReservedPkware14 = 16384,
		// Token: 0x0400028A RID: 650
		ReservedPkware15 = 32768
	}
}
