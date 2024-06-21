using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000083 RID: 131
	[ComVisible(false)]
	public class DynamicDiskDataSource : IDynamicDataSource
	{
		// Token: 0x06000602 RID: 1538 RVA: 0x00030390 File Offset: 0x0002E590
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
