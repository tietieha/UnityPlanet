using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006C RID: 108
	[ComVisible(false)]
	public enum EncryptionAlgorithm
	{
		// Token: 0x040002E0 RID: 736
		None,
		// Token: 0x040002E1 RID: 737
		PkzipClassic,
		// Token: 0x040002E2 RID: 738
		Des = 26113,
		// Token: 0x040002E3 RID: 739
		RC2,
		// Token: 0x040002E4 RID: 740
		TripleDes168,
		// Token: 0x040002E5 RID: 741
		TripleDes112 = 26121,
		// Token: 0x040002E6 RID: 742
		Aes128 = 26126,
		// Token: 0x040002E7 RID: 743
		Aes192,
		// Token: 0x040002E8 RID: 744
		Aes256,
		// Token: 0x040002E9 RID: 745
		RC2Corrected = 26370,
		// Token: 0x040002EA RID: 746
		Blowfish = 26400,
		// Token: 0x040002EB RID: 747
		Twofish,
		// Token: 0x040002EC RID: 748
		RC4 = 26625,
		// Token: 0x040002ED RID: 749
		Unknown = 65535
	}
}
