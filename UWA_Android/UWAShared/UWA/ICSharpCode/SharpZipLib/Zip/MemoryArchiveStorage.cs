using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000087 RID: 135
	[ComVisible(false)]
	public class MemoryArchiveStorage : BaseArchiveStorage
	{
		// Token: 0x06000618 RID: 1560 RVA: 0x0003078C File Offset: 0x0002E98C
		public MemoryArchiveStorage() : base(FileUpdateMode.Direct)
		{
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00030798 File Offset: 0x0002E998
		public MemoryArchiveStorage(FileUpdateMode updateMode) : base(updateMode)
		{
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x000307A4 File Offset: 0x0002E9A4
		public MemoryStream FinalStream
		{
			get
			{
				return this.finalStream_;
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x000307C4 File Offset: 0x0002E9C4
		public override Stream GetTemporaryOutput()
		{
			this.temporaryStream_ = new MemoryStream();
			return this.temporaryStream_;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x000307F0 File Offset: 0x0002E9F0
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

		// Token: 0x0600061D RID: 1565 RVA: 0x00030844 File Offset: 0x0002EA44
		public override Stream MakeTemporaryCopy(Stream stream)
		{
			this.temporaryStream_ = new MemoryStream();
			stream.Position = 0L;
			StreamUtils.Copy(stream, this.temporaryStream_, new byte[4096]);
			return this.temporaryStream_;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00030890 File Offset: 0x0002EA90
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
					StreamUtils.Copy(stream, stream2, new byte[4096]);
					stream.Close();
				}
			}
			else
			{
				stream2 = stream;
			}
			return stream2;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00030908 File Offset: 0x0002EB08
		public override void Dispose()
		{
			bool flag = this.temporaryStream_ != null;
			if (flag)
			{
				this.temporaryStream_.Close();
			}
		}

		// Token: 0x04000391 RID: 913
		private MemoryStream temporaryStream_;

		// Token: 0x04000392 RID: 914
		private MemoryStream finalStream_;
	}
}
