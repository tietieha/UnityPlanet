using System;
using System.Collections.Generic;

namespace UWALocal
{
	// Token: 0x02000021 RID: 33
	internal class EmptyScreenTrackManager : ScreenTrackManager
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000B914 File Offset: 0x00009B14
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			base.Enabled = false;
		}
	}
}
