using System;
using System.Runtime.InteropServices;

namespace UWALocal
{
	// Token: 0x0200001B RID: 27
	internal static class GPUGraphicsTools
	{
		// Token: 0x06000167 RID: 359
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_DisableDrawCalls();

		// Token: 0x06000168 RID: 360
		[DllImport("GPUGraphicsTools")]
		public static extern int unhook_DisableDrawCalls();

		// Token: 0x06000169 RID: 361
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_2T();

		// Token: 0x0600016A RID: 362
		[DllImport("GPUGraphicsTools")]
		public static extern int unhook_2T();

		// Token: 0x0600016B RID: 363
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_SR();

		// Token: 0x0600016C RID: 364
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_DTF();

		// Token: 0x0600016D RID: 365
		[DllImport("GPUGraphicsTools")]
		public static extern int unhook_DTF();

		// Token: 0x0600016E RID: 366
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_DisableAlphaBlend();

		// Token: 0x0600016F RID: 367
		[DllImport("GPUGraphicsTools")]
		public static extern int hook_DisableZTest();

		// Token: 0x06000170 RID: 368
		[DllImport("GPUGraphicsTools")]
		public static extern int unhook_DisableZTest();

		// Token: 0x06000171 RID: 369
		[DllImport("GPUGraphicsTools")]
		public static extern int unhook_Disable();

		// Token: 0x040000A7 RID: 167
		private const string DllName = "GPUGraphicsTools";
	}
}
