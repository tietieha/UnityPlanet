using System;
using System.Collections.Generic;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003D RID: 61
	internal class EmptyScreenTrackManager : ScreenTrackManager
	{
		// Token: 0x060002BB RID: 699 RVA: 0x00013504 File Offset: 0x00011704
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			base.Enabled = false;
		}
	}
}
