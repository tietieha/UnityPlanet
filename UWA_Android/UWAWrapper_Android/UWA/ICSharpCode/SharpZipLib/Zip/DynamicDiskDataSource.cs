using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000074 RID: 116
	[ComVisible(false)]
	public class DynamicDiskDataSource : IDynamicDataSource
	{
		// Token: 0x06000526 RID: 1318 RVA: 0x000235D8 File Offset: 0x000217D8
		public Stream GetSource(ZipEntry entry, string name)
		{
			Stream result = null;
			bool flag = name != null;
			if (flag)
			{
				result = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			return result;
		}
	}
}
