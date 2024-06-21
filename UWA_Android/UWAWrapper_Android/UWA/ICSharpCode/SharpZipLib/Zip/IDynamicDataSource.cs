using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000072 RID: 114
	[ComVisible(false)]
	public interface IDynamicDataSource
	{
		// Token: 0x06000522 RID: 1314
		Stream GetSource(ZipEntry entry, string name);
	}
}
