using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B5 RID: 181
	[ComVisible(false)]
	public class ProgressEventArgs : EventArgs
	{
		// Token: 0x06000875 RID: 2165 RVA: 0x0003F56C File Offset: 0x0003D76C
		public ProgressEventArgs(string name, long processed, long target)
		{
			this.name_ = name;
			this.processed_ = processed;
			this.target_ = target;
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000876 RID: 2166 RVA: 0x0003F594 File Offset: 0x0003D794
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000877 RID: 2167 RVA: 0x0003F5B4 File Offset: 0x0003D7B4
		// (set) Token: 0x06000878 RID: 2168 RVA: 0x0003F5D4 File Offset: 0x0003D7D4
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

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000879 RID: 2169 RVA: 0x0003F5E0 File Offset: 0x0003D7E0
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

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x0003F634 File Offset: 0x0003D834
		public long Processed
		{
			get
			{
				return this.processed_;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600087B RID: 2171 RVA: 0x0003F654 File Offset: 0x0003D854
		public long Target
		{
			get
			{
				return this.target_;
			}
		}

		// Token: 0x0400051E RID: 1310
		private string name_;

		// Token: 0x0400051F RID: 1311
		private long processed_;

		// Token: 0x04000520 RID: 1312
		private long target_;

		// Token: 0x04000521 RID: 1313
		private bool continueRunning_ = true;
	}
}
