using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000A8 RID: 168
	[ComVisible(false)]
	public class ScanFailureEventArgs : EventArgs
	{
		// Token: 0x060007A2 RID: 1954 RVA: 0x000328F0 File Offset: 0x00030AF0
		public ScanFailureEventArgs(string name, Exception e)
		{
			this.name_ = name;
			this.exception_ = e;
			this.continueRunning_ = true;
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x00032910 File Offset: 0x00030B10
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00032930 File Offset: 0x00030B30
		public Exception Exception
		{
			get
			{
				return this.exception_;
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00032950 File Offset: 0x00030B50
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x00032970 File Offset: 0x00030B70
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

		// Token: 0x040004B0 RID: 1200
		private string name_;

		// Token: 0x040004B1 RID: 1201
		private Exception exception_;

		// Token: 0x040004B2 RID: 1202
		private bool continueRunning_;
	}
}
