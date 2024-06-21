using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000075 RID: 117
	[ComVisible(false)]
	public interface IArchiveStorage
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000527 RID: 1319
		FileUpdateMode UpdateMode { get; }

		// Token: 0x06000528 RID: 1320
		Stream GetTemporaryOutput();

		// Token: 0x06000529 RID: 1321
		Stream ConvertTemporaryToFinal();

		// Token: 0x0600052A RID: 1322
		Stream MakeTemporaryCopy(Stream stream);

		// Token: 0x0600052B RID: 1323
		Stream OpenForDirectUpdate(Stream stream);

		// Token: 0x0600052C RID: 1324
		void Dispose();
	}
}
