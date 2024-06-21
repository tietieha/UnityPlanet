using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000084 RID: 132
	[ComVisible(false)]
	public interface IArchiveStorage
	{
		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000603 RID: 1539
		FileUpdateMode UpdateMode { get; }

		// Token: 0x06000604 RID: 1540
		Stream GetTemporaryOutput();

		// Token: 0x06000605 RID: 1541
		Stream ConvertTemporaryToFinal();

		// Token: 0x06000606 RID: 1542
		Stream MakeTemporaryCopy(Stream stream);

		// Token: 0x06000607 RID: 1543
		Stream OpenForDirectUpdate(Stream stream);

		// Token: 0x06000608 RID: 1544
		void Dispose();
	}
}
