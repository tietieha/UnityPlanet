using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B7 RID: 183
	[ComVisible(false)]
	public class ScanFailureEventArgs : EventArgs
	{
		// Token: 0x0600087E RID: 2174 RVA: 0x0003F6A8 File Offset: 0x0003D8A8
		public ScanFailureEventArgs(string name, Exception e)
		{
			this.name_ = name;
			this.exception_ = e;
			this.continueRunning_ = true;
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x0600087F RID: 2175 RVA: 0x0003F6C8 File Offset: 0x0003D8C8
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x0003F6E8 File Offset: 0x0003D8E8
		public Exception Exception
		{
			get
			{
				return this.exception_;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x0003F708 File Offset: 0x0003D908
		// (set) Token: 0x06000882 RID: 2178 RVA: 0x0003F728 File Offset: 0x0003D928
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

		// Token: 0x04000523 RID: 1315
		private string name_;

		// Token: 0x04000524 RID: 1316
		private Exception exception_;

		// Token: 0x04000525 RID: 1317
		private bool continueRunning_;
	}
}
