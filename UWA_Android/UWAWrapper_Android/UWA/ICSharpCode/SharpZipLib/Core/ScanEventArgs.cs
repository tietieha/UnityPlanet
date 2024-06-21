using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000A5 RID: 165
	[ComVisible(false)]
	public class ScanEventArgs : EventArgs
	{
		// Token: 0x06000795 RID: 1941 RVA: 0x00032750 File Offset: 0x00030950
		public ScanEventArgs(string name)
		{
			this.name_ = name;
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000796 RID: 1942 RVA: 0x00032768 File Offset: 0x00030968
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x00032788 File Offset: 0x00030988
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x000327A8 File Offset: 0x000309A8
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

		// Token: 0x040004A9 RID: 1193
		private string name_;

		// Token: 0x040004AA RID: 1194
		private bool continueRunning_ = true;
	}
}
