using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000068 RID: 104
	[ComVisible(false)]
	public interface IEntryFactory
	{
		// Token: 0x060004D2 RID: 1234
		ZipEntry MakeFileEntry(string fileName);

		// Token: 0x060004D3 RID: 1235
		ZipEntry MakeFileEntry(string fileName, bool useFileSystem);

		// Token: 0x060004D4 RID: 1236
		ZipEntry MakeDirectoryEntry(string directoryName);

		// Token: 0x060004D5 RID: 1237
		ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem);

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060004D6 RID: 1238
		// (set) Token: 0x060004D7 RID: 1239
		INameTransform NameTransform { get; set; }
	}
}
