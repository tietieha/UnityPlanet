using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000A6 RID: 166
	[ComVisible(false)]
	public class ProgressEventArgs : EventArgs
	{
		// Token: 0x06000799 RID: 1945 RVA: 0x000327B4 File Offset: 0x000309B4
		public ProgressEventArgs(string name, long processed, long target)
		{
			this.name_ = name;
			this.processed_ = processed;
			this.target_ = target;
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600079A RID: 1946 RVA: 0x000327DC File Offset: 0x000309DC
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x000327FC File Offset: 0x000309FC
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x0003281C File Offset: 0x00030A1C
		public bool ContinueRunning
		{
			get
			{
				return this.continueRunning_;
			}
			set
			{
				this.continueRunning_ = value;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x00032828 File Offset: 0x00030A28
		public float PercentComplete
		{
			get
			{
				bool flag = this.target_ <= 0L;
				float result;
				if (flag)
				{
					result = 0f;
				}
				else
				{
					result = (float)this.processed_ / (float)this.target_ * 100f;
				}
				return result;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x0003287C File Offset: 0x00030A7C
		public long Processed
		{
			get
			{
				return this.processed_;
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x0003289C File Offset: 0x00030A9C
		public long Target
		{
			get
			{
				return this.target_;
			}
		}

		// Token: 0x040004AB RID: 1195
		private string name_;

		// Token: 0x040004AC RID: 1196
		private long processed_;

		// Token: 0x040004AD RID: 1197
		private long target_;

		// Token: 0x040004AE RID: 1198
		private bool continueRunning_ = true;
	}
}
