using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000059 RID: 89
	[ComVisible(false)]
	public interface IEntryFactory
	{
		// Token: 0x060003F6 RID: 1014
		ZipEntry MakeFileEntry(string fileName);

		// Token: 0x060003F7 RID: 1015
		ZipEntry MakeFileEntry(string fileName, bool useFileSystem);

		// Token: 0x060003F8 RID: 1016
		ZipEntry MakeDirectoryEntry(string directoryName);

		// Token: 0x060003F9 RID: 1017
		ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem);

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060003FA RID: 1018
		// (set) Token: 0x060003FB RID: 1019
		UWA.ICSharpCode.SharpZipLib.Core.INameTransform NameTransform { get; set; }
	}
}
