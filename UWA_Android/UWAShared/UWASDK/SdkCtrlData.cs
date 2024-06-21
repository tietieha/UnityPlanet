using System;

namespace UWASDK
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public class SdkCtrlData
	{
		// Token: 0x04000095 RID: 149
		public eSdkMode SdkMode;

		// Token: 0x04000096 RID: 150
		public eGotMode GotMode;

		// Token: 0x04000097 RID: 151
		public eDumpType TryDump;

		// Token: 0x04000098 RID: 152
		public bool PocoConnected = false;

		// Token: 0x04000099 RID: 153
		public bool TryStart = false;

		// Token: 0x0400009A RID: 154
		public bool TryStop = false;

		// Token: 0x0400009B RID: 155
		public bool TryExit = false;

		// Token: 0x0400009C RID: 156
		public bool TryPA = false;

		// Token: 0x0400009D RID: 157
		public bool TryGOT = false;

		// Token: 0x0400009E RID: 158
		public bool GPMTest = false;

		// Token: 0x0400009F RID: 159
		public static readonly SdkCtrlData Instance = new SdkCtrlData();
	}
}
