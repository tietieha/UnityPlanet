using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000085 RID: 133
	[ComVisible(false)]
	public abstract class BaseArchiveStorage : IArchiveStorage
	{
		// Token: 0x06000609 RID: 1545 RVA: 0x000303C4 File Offset: 0x0002E5C4
		protected BaseArchiveStorage(FileUpdateMode updateMode)
		{
			this.updateMode_ = updateMode;
		}

		// Token: 0x0600060A RID: 1546
		public abstract Stream GetTemporaryOutput();

		// Token: 0x0600060B RID: 1547
		public abstract Stream ConvertTemporaryToFinal();

		// Token: 0x0600060C RID: 1548
		public abstract Stream MakeTemporaryCopy(Stream stream);

		// Token: 0x0600060D RID: 1549
		public abstract Stream OpenForDirectUpdate(Stream stream);

		// Token: 0x0600060E RID: 1550
		public abstract void Dispose();

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x000303D8 File Offset: 0x0002E5D8
		public FileUpdateMode UpdateMode
		{
			get
			{
				return this.updateMode_;
			}
		}

		// Token: 0x0400038D RID: 909
		private FileUpdateMode updateMode_;
	}
}
