using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x0200009D RID: 157
	[ComVisible(false)]
	public class GZipOutputStream : UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream
	{
		// Token: 0x0600075E RID: 1886 RVA: 0x000317BC File Offset: 0x0002F9BC
		public GZipOutputStream(Stream baseOutputStream) : this(baseOutputStream, 4096)
		{
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x000317CC File Offset: 0x0002F9CC
		public GZipOutputStream(Stream baseOutputStream, int size) : base(baseOutputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Deflater(-1, true), size)
		{
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x000317F4 File Offset: 0x0002F9F4
		public void SetLevel(int level)
		{
			bool flag = level < 1;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			this.deflater_.SetLevel(level);
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x0003182C File Offset: 0x0002FA2C
		public int GetLevel()
		{
			return this.deflater_.GetLevel();
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00031850 File Offset: 0x0002FA50
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

		// Token: 0x06000763 RID: 1891 RVA: 0x000318B4 File Offset: 0x0002FAB4
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

		// Token: 0x06000764 RID: 1892 RVA: 0x0003191C File Offset: 0x0002FB1C
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

		// Token: 0x06000765 RID: 1893 RVA: 0x000319DC File Offset: 0x0002FBDC
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

		// Token: 0x04000491 RID: 1169
		protected UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();

		// Token: 0x04000492 RID: 1170
		private GZipOutputStream.OutputState state_ = GZipOutputStream.OutputState.Header;

		// Token: 0x02000124 RID: 292
		private enum OutputState
		{
			// Token: 0x04000712 RID: 1810
			Header,
			// Token: 0x04000713 RID: 1811
			Footer,
			// Token: 0x04000714 RID: 1812
			Finished,
			// Token: 0x04000715 RID: 1813
			Closed
		}
	}
}
