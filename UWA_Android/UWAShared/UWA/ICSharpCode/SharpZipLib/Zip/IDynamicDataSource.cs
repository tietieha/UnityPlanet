using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000081 RID: 129
	[ComVisible(false)]
	public interface IDynamicDataSource
	{
		// Token: 0x060005FE RID: 1534
		Stream GetSource(ZipEntry entry, string name);
	}
}
