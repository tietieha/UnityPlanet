using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x020000AC RID: 172
	[ComVisible(false)]
	public class GZipOutputStream : DeflaterOutputStream
	{
		// Token: 0x0600083A RID: 2106 RVA: 0x0003E574 File Offset: 0x0003C774
		public GZipOutputStream(Stream baseOutputStream) : this(baseOutputStream, 4096)
		{
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0003E584 File Offset: 0x0003C784
		public GZipOutputStream(Stream baseOutputStream, int size) : base(baseOutputStream, new Deflater(-1, true), size)
		{
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0003E5AC File Offset: 0x0003C7AC
		public void SetLevel(int level)
		{
			bool flag = level < 1;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			this.deflater_.SetLevel(level);
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0003E5E4 File Offset: 0x0003C7E4
		public int GetLevel()
		{
			return this.deflater_.GetLevel();
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0003E608 File Offset: 0x0003C808
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool flag = this.state_ == GZipOutputStream.OutputState.Header;
			if (flag)
			{
				this.WriteHeader();
			}
			bool flag2 = this.state_ != GZipOutputStream.OutputState.Footer;
			if (flag2)
			{
				throw new InvalidOperationException("Write not permitted in current state");
			}
			this.crc.Update(buffer, offset, count);
			base.Write(buffer, offset, count);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0003E66C File Offset: 0x0003C86C
		public override void Close()
		{
			try
			{
				this.Finish();
			}
			finally
			{
				bool flag = this.state_ != GZipOutputStream.OutputState.Closed;
				if (flag)
				{
					this.state_ = GZipOutputStream.OutputState.Closed;
					bool isStreamOwner = base.IsStreamOwner;
					if (isStreamOwner)
					{
						this.baseOutputStream_.Close();
					}
				}
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0003E6D4 File Offset: 0x0003C8D4
		public override void Finish()
		{
			bool flag = this.state_ == GZipOutputStream.OutputState.Header;
			if (flag)
			{
				this.WriteHeader();
			}
			bool flag2 = this.state_ == GZipOutputStream.OutputState.Footer;
			if (flag2)
			{
				this.state_ = GZipOutputStream.OutputState.Finished;
				base.Finish();
				uint num = (uint)(this.deflater_.TotalIn & (long)((ulong)-1));
				uint num2 = (uint)(this.crc.Value & (long)((ulong)-1));
				byte[] array = new byte[]
				{
					(byte)num2,
					(byte)(num2 >> 8),
					(byte)(num2 >> 16),
					(byte)(num2 >> 24),
					(byte)num,
					(byte)(num >> 8),
					(byte)(num >> 16),
					(byte)(num >> 24)
				};
				this.baseOutputStream_.Write(array, 0, array.Length);
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0003E794 File Offset: 0x0003C994
		private void WriteHeader()
		{
			bool flag = this.state_ == GZipOutputStream.OutputState.Header;
			if (flag)
			{
				this.state_ = GZipOutputStream.OutputState.Footer;
				int num = (int)((DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000L);
				byte[] array = new byte[]
				{
					31,
					139,
					8,
					0,
					0,
					0,
					0,
					0,
					0,
					byte.MaxValue
				};
				array[4] = (byte)num;
				array[5] = (byte)(num >> 8);
				array[6] = (byte)(num >> 16);
				array[7] = (byte)(num >> 24);
				byte[] array2 = array;
				this.baseOutputStream_.Write(array2, 0, array2.Length);
			}
		}

		// Token: 0x04000504 RID: 1284
		protected Crc32 crc = new Crc32();

		// Token: 0x04000505 RID: 1285
		private GZipOutputStream.OutputState state_ = GZipOutputStream.OutputState.Header;

		// Token: 0x0200015A RID: 346
		private enum OutputState
		{
			// Token: 0x040007CF RID: 1999
			Header,
			// Token: 0x040007D0 RID: 2000
			Footer,
			// Token: 0x040007D1 RID: 2001
			Finished,
			// Token: 0x040007D2 RID: 2002
			Closed
		}
	}
}
