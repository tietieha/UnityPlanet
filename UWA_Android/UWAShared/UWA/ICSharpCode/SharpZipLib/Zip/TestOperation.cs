using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007B RID: 123
	[ComVisible(false)]
	public enum TestOperation
	{
		// Token: 0x04000365 RID: 869
		Initialising,
		// Token: 0x04000366 RID: 870
		EntryHeader,
		// Token: 0x04000367 RID: 871
		EntryData,
		// Token: 0x04000368 RID: 872
		EntryComplete,
		// Token: 0x04000369 RID: 873
		MiscellaneousTests,
		// Token: 0x0400036A RID: 874
		Complete
	}
}
