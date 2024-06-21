using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000073 RID: 115
	[ComVisible(false)]
	public class StaticDiskDataSource : IStaticDataSource
	{
		// Token: 0x06000523 RID: 1315 RVA: 0x00023590 File Offset: 0x00021790
		public StaticDiskDataSource(string fileName)
		{
			this.fileName_ = fileName;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x000235A4 File Offset: 0x000217A4
		public Stream GetSource()
		{
			return File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		// Token: 0x04000319 RID: 793
		private string fileName_;
	}
}
