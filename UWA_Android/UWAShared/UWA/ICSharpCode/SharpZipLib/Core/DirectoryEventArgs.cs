using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B6 RID: 182
	[ComVisible(false)]
	public class DirectoryEventArgs : ScanEventArgs
	{
		// Token: 0x0600087C RID: 2172 RVA: 0x0003F674 File Offset: 0x0003D874
		public DirectoryEventArgs(string name, bool hasMatchingFiles) : base(name)
		{
			this.hasMatchingFiles_ = hasMatchingFiles;
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x0003F688 File Offset: 0x0003D888
		public bool HasMatchingFiles
		{
			get
			{
				return this.hasMatchingFiles_;
			}
		}

		// Token: 0x04000522 RID: 1314
		private bool hasMatchingFiles_;
	}
}
