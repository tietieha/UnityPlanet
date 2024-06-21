using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000A7 RID: 167
	[ComVisible(false)]
	public class DirectoryEventArgs : ScanEventArgs
	{
		// Token: 0x060007A0 RID: 1952 RVA: 0x000328BC File Offset: 0x00030ABC
		public DirectoryEventArgs(string name, bool hasMatchingFiles) : base(name)
		{
			this.hasMatchingFiles_ = hasMatchingFiles;
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060007A1 RID: 1953 RVA: 0x000328D0 File Offset: 0x00030AD0
		public bool HasMatchingFiles
		{
			get
			{
				return this.hasMatchingFiles_;
			}
		}

		// Token: 0x040004AF RID: 1199
		private bool hasMatchingFiles_;
	}
}
