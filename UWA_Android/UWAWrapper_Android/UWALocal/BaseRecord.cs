using System;

namespace UWALocal
{
	// Token: 0x02000022 RID: 34
	internal interface BaseRecord
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001D4 RID: 468
		bool isValid { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001D5 RID: 469
		int sampleBlockCountX { get; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001D6 RID: 470
		long elapsedNanosecondsX { get; }
	}
}
