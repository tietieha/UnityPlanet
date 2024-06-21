using System;
using UnityEngine;
using UWA;

namespace UWALocal
{
	// Token: 0x02000018 RID: 24
	internal class DumpHelper
	{
		// Token: 0x0600014A RID: 330 RVA: 0x00008560 File Offset: 0x00006760
		public void Setup(int step)
		{
			if (step < 0)
			{
				this.Disabled = true;
				this.AutoDump = false;
				this.DumpInterval100 = 0;
				return;
			}
			this.Disabled = false;
			this.AutoDump = (step != 0);
			this.DumpInterval100 = step;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00008598 File Offset: 0x00006798
		public bool UpdateDump()
		{
			if (this.Disabled)
			{
				return false;
			}
			bool result = false;
			if (this.AutoDump && SharedUtils.frameId % (this.DumpInterval100 * 100) == 0)
			{
				result = true;
			}
			if (this.DumpingTimer >= 0f)
			{
				this.DumpingTimer -= Time.unscaledDeltaTime;
			}
			if (!this.AutoDump && this.WaitToDump && this.DumpingTimer < 0f)
			{
				this.WaitToDump = false;
				this.DumpingTimer = 1f;
				result = true;
			}
			return result;
		}

		// Token: 0x04000092 RID: 146
		public bool UseDefault;

		// Token: 0x04000093 RID: 147
		public bool Disabled;

		// Token: 0x04000094 RID: 148
		public bool AutoDump = true;

		// Token: 0x04000095 RID: 149
		public int DumpInterval100 = 10;

		// Token: 0x04000096 RID: 150
		public bool WaitToDump;

		// Token: 0x04000097 RID: 151
		public float DumpingTimer = 1f;

		// Token: 0x04000098 RID: 152
		public string Display = "Dump";
	}
}
