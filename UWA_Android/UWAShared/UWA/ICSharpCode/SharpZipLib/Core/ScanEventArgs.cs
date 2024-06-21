using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B4 RID: 180
	[ComVisible(false)]
	public class ScanEventArgs : EventArgs
	{
		// Token: 0x06000871 RID: 2161 RVA: 0x0003F508 File Offset: 0x0003D708
		public ScanEventArgs(string name)
		{
			this.name_ = name;
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000872 RID: 2162 RVA: 0x0003F520 File Offset: 0x0003D720
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x0003F540 File Offset: 0x0003D740
		// (set) Token: 0x06000874 RID: 2164 RVA: 0x0003F560 File Offset: 0x0003D760
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

		// Token: 0x0400051C RID: 1308
		private string name_;

		// Token: 0x0400051D RID: 1309
		private bool continueRunning_ = true;
	}
}
