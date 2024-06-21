using System;

namespace UWASDK
{
	// Token: 0x02000013 RID: 19
	[Flags]
	public enum eDumpType
	{
		// Token: 0x04000079 RID: 121
		None = 0,
		// Token: 0x0400007A RID: 122
		ManagedHeap = 1,
		// Token: 0x0400007B RID: 123
		Lua = 2,
		// Token: 0x0400007C RID: 124
		Resources = 4,
		// Token: 0x0400007D RID: 125
		Overdraw = 8
	}
}
