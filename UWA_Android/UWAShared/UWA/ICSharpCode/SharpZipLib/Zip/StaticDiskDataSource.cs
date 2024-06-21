using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000082 RID: 130
	[ComVisible(false)]
	public class StaticDiskDataSource : IStaticDataSource
	{
		// Token: 0x060005FF RID: 1535 RVA: 0x00030348 File Offset: 0x0002E548
		public StaticDiskDataSource(string fileName)
		{
			this.fileName_ = fileName;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0003035C File Offset: 0x0002E55C
		public Stream GetSource()
		{
			return File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		// Token: 0x0400038C RID: 908
		private string fileName_;
	}
}
