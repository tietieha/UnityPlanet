using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000076 RID: 118
	[ComVisible(false)]
	public abstract class BaseArchiveStorage : IArchiveStorage
	{
		// Token: 0x0600052D RID: 1325 RVA: 0x0002360C File Offset: 0x0002180C
		protected BaseArchiveStorage(FileUpdateMode updateMode)
		{
			this.updateMode_ = updateMode;
		}

		// Token: 0x0600052E RID: 1326
		public abstract Stream GetTemporaryOutput();

		// Token: 0x0600052F RID: 1327
		public abstract Stream ConvertTemporaryToFinal();

		// Token: 0x06000530 RID: 1328
		public abstract Stream MakeTemporaryCopy(Stream stream);

		// Token: 0x06000531 RID: 1329
		public abstract Stream OpenForDirectUpdate(Stream stream);

		// Token: 0x06000532 RID: 1330
		public abstract void Dispose();

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000533 RID: 1331 RVA: 0x00023620 File Offset: 0x00021820
		public FileUpdateMode UpdateMode
		{
			get
			{
				return this.updateMode_;
			}
		}

		// Token: 0x0400031A RID: 794
		private FileUpdateMode updateMode_;
	}
}
