using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006C RID: 108
	[ComVisible(false)]
	public enum TestOperation
	{
		// Token: 0x040002F2 RID: 754
		Initialising,
		// Token: 0x040002F3 RID: 755
		EntryHeader,
		// Token: 0x040002F4 RID: 756
		EntryData,
		// Token: 0x040002F5 RID: 757
		EntryComplete,
		// Token: 0x040002F6 RID: 758
		MiscellaneousTests,
		// Token: 0x040002F7 RID: 759
		Complete
	}
}
