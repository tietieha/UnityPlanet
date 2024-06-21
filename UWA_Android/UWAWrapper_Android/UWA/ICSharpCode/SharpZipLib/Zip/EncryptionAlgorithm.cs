using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200005D RID: 93
	[ComVisible(false)]
	public enum EncryptionAlgorithm
	{
		// Token: 0x0400026D RID: 621
		None,
		// Token: 0x0400026E RID: 622
		PkzipClassic,
		// Token: 0x0400026F RID: 623
		Des = 26113,
		// Token: 0x04000270 RID: 624
		RC2,
		// Token: 0x04000271 RID: 625
		TripleDes168,
		// Token: 0x04000272 RID: 626
		TripleDes112 = 26121,
		// Token: 0x04000273 RID: 627
		Aes128 = 26126,
		// Token: 0x04000274 RID: 628
		Aes192,
		// Token: 0x04000275 RID: 629
		Aes256,
		// Token: 0x04000276 RID: 630
		RC2Corrected = 26370,
		// Token: 0x04000277 RID: 631
		Blowfish = 26400,
		// Token: 0x04000278 RID: 632
		Twofish,
		// Token: 0x04000279 RID: 633
		RC4 = 26625,
		// Token: 0x0400027A RID: 634
		Unknown = 65535
	}
}
