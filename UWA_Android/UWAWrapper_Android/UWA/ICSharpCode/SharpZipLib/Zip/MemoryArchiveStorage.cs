using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000078 RID: 120
	[ComVisible(false)]
	public class MemoryArchiveStorage : BaseArchiveStorage
	{
		// Token: 0x0600053C RID: 1340 RVA: 0x000239D4 File Offset: 0x00021BD4
		public MemoryArchiveStorage() : base(FileUpdateMode.Direct)
		{
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x000239E0 File Offset: 0x00021BE0
		public MemoryArchiveStorage(FileUpdateMode updateMode) : base(updateMode)
		{
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x000239EC File Offset: 0x00021BEC
		public MemoryStream FinalStream
		{
			get
			{
				return this.finalStream_;
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00023A0C File Offset: 0x00021C0C
		public override Stream GetTemporaryOutput()
		{
			this.temporaryStream_ = new MemoryStream();
			return this.temporaryStream_;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00023A38 File Offset: 0x00021C38
		public override Stream ConvertTemporaryToFinal()
		{
			bool flag = this.temporaryStream_ == null;
			if (flag)
			{
				throw new ZipException("No temporary stream has been created");
			}
			this.finalStream_ = new MemoryStream(this.temporaryStream_.ToArray());
			return this.finalStream_;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x00023A8C File Offset: 0x00021C8C
		public override Stream MakeTemporaryCopy(Stream stream)
		{
			this.temporaryStream_ = new MemoryStream();
			stream.Position = 0L;
			UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(stream, this.temporaryStream_, new byte[4096]);
			return this.temporaryStream_;
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00023AD8 File Offset: 0x00021CD8
		public override Stream OpenForDirectUpdate(Stream stream)
		{
			bool flag = stream == null || !stream.CanWrite;
			Stream stream2;
			if (flag)
			{
				stream2 = new MemoryStream();
				bool flag2 = stream != null;
				if (flag2)
				{
					stream.Position = 0L;
					UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(stream, stream2, new byte[4096]);
					stream.Close();
				}
			}
			else
			{
				stream2 = stream;
			}
			return stream2;
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00023B50 File Offset: 0x00021D50
		public override void Dispose()
		{
			bool flag = this.temporaryStream_ != null;
			if (flag)
			{
				this.temporaryStream_.Close();
			}
		}

		// Token: 0x0400031E RID: 798
		private MemoryStream temporaryStream_;

		// Token: 0x0400031F RID: 799
		private MemoryStream finalStream_;
	}
}
