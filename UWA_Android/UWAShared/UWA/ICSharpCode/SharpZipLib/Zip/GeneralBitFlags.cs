using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006D RID: 109
	[Flags]
	[ComVisible(false)]
	public enum GeneralBitFlags
	{
		// Token: 0x040002EF RID: 751
		Encrypted = 1,
		// Token: 0x040002F0 RID: 752
		Method = 6,
		// Token: 0x040002F1 RID: 753
		Descriptor = 8,
		// Token: 0x040002F2 RID: 754
		ReservedPKware4 = 16,
		// Token: 0x040002F3 RID: 755
		Patched = 32,
		// Token: 0x040002F4 RID: 756
		StrongEncryption = 64,
		// Token: 0x040002F5 RID: 757
		Unused7 = 128,
		// Token: 0x040002F6 RID: 758
		Unused8 = 256,
		// Token: 0x040002F7 RID: 759
		Unused9 = 512,
		// Token: 0x040002F8 RID: 760
		Unused10 = 1024,
		// Token: 0x040002F9 RID: 761
		UnicodeText = 2048,
		// Token: 0x040002FA RID: 762
		EnhancedCompress = 4096,
		// Token: 0x040002FB RID: 763
		HeaderMasked = 8192,
		// Token: 0x040002FC RID: 764
		ReservedPkware14 = 16384,
		// Token: 0x040002FD RID: 765
		ReservedPkware15 = 32768
	}
}
