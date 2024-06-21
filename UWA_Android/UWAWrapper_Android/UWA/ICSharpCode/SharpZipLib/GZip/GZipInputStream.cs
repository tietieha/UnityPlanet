using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x0200009C RID: 156
	[ComVisible(false)]
	public class GZipInputStream : UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream
	{
		// Token: 0x06000759 RID: 1881 RVA: 0x00031104 File Offset: 0x0002F304
		public GZipInputStream(Stream baseInputStream) : this(baseInputStream, 4096)
		{
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00031114 File Offset: 0x0002F314
		public GZipInputStream(Stream baseInputStream, int size) : base(baseInputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Inflater(true), size)
		{
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00031128 File Offset: 0x0002F328
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num;
			for (;;)
			{
				bool flag = !this.readGZIPHeader;
				if (flag)
				{
					bool flag2 = !this.ReadHeader();
					if (flag2)
					{
						break;
					}
				}
				num = base.Read(buffer, offset, count);
				bool flag3 = num > 0;
				if (flag3)
				{
					this.crc.Update(buffer, offset, num);
				}
				bool isFinished = this.inf.IsFinished;
				if (isFinished)
				{
					this.ReadFooter();
				}
				bool flag4 = num > 0;
				if (flag4)
				{
					goto Block_5;
				}
			}
			return 0;
			Block_5:
			return num;
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x000311D0 File Offset: 0x0002F3D0
		private bool ReadHeader()
		{
			this.crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();
			bool flag = this.inputBuffer.Available <= 0;
			if (flag)
			{
				this.inputBuffer.Fill();
				bool flag2 = this.inputBuffer.Available <= 0;
				if (flag2)
				{
					return false;
				}
			}
			UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();
			int num = this.inputBuffer.ReadLeByte();
			bool flag3 = num < 0;
			if (flag3)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num);
			bool flag4 = num != 31;
			if (flag4)
			{
				throw new GZipException("Error GZIP header, first magic byte doesn't match");
			}
			num = this.inputBuffer.ReadLeByte();
			bool flag5 = num < 0;
			if (flag5)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			bool flag6 = num != 139;
			if (flag6)
			{
				throw new GZipException("Error GZIP header,  second magic byte doesn't match");
			}
			crc.Update(num);
			int num2 = this.inputBuffer.ReadLeByte();
			bool flag7 = num2 < 0;
			if (flag7)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			bool flag8 = num2 != 8;
			if (flag8)
			{
				throw new GZipException("Error GZIP header, data not in deflate format");
			}
			crc.Update(num2);
			int num3 = this.inputBuffer.ReadLeByte();
			bool flag9 = num3 < 0;
			if (flag9)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num3);
			bool flag10 = (num3 & 224) != 0;
			if (flag10)
			{
				throw new GZipException("Reserved flag bits in GZIP header != 0");
			}
			for (int i = 0; i < 6; i++)
			{
				int num4 = this.inputBuffer.ReadLeByte();
				bool flag11 = num4 < 0;
				if (flag11)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num4);
			}
			bool flag12 = (num3 & 4) != 0;
			if (flag12)
			{
				for (int j = 0; j < 2; j++)
				{
					int num5 = this.inputBuffer.ReadLeByte();
					bool flag13 = num5 < 0;
					if (flag13)
					{
						throw new EndOfStreamException("EOS reading GZIP header");
					}
					crc.Update(num5);
				}
				bool flag14 = this.inputBuffer.ReadLeByte() < 0 || this.inputBuffer.ReadLeByte() < 0;
				if (flag14)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num6 = this.inputBuffer.ReadLeByte();
				int num7 = this.inputBuffer.ReadLeByte();
				bool flag15 = num6 < 0 || num7 < 0;
				if (flag15)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num6);
				crc.Update(num7);
				int num8 = num6 << 8 | num7;
				for (int k = 0; k < num8; k++)
				{
					int num9 = this.inputBuffer.ReadLeByte();
					bool flag16 = num9 < 0;
					if (flag16)
					{
						throw new EndOfStreamException("EOS reading GZIP header");
					}
					crc.Update(num9);
				}
			}
			bool flag17 = (num3 & 8) != 0;
			if (flag17)
			{
				int num10;
				while ((num10 = this.inputBuffer.ReadLeByte()) > 0)
				{
					crc.Update(num10);
				}
				bool flag18 = num10 < 0;
				if (flag18)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num10);
			}
			bool flag19 = (num3 & 16) != 0;
			if (flag19)
			{
				int num11;
				while ((num11 = this.inputBuffer.ReadLeByte()) > 0)
				{
					crc.Update(num11);
				}
				bool flag20 = num11 < 0;
				if (flag20)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num11);
			}
			bool flag21 = (num3 & 2) != 0;
			if (flag21)
			{
				int num12 = this.inputBuffer.ReadLeByte();
				bool flag22 = num12 < 0;
				if (flag22)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num13 = this.inputBuffer.ReadLeByte();
				bool flag23 = num13 < 0;
				if (flag23)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				num12 = (num12 << 8 | num13);
				bool flag24 = num12 != ((int)crc.Value & 65535);
				if (flag24)
				{
					throw new GZipException("Header CRC value mismatch");
				}
			}
			this.readGZIPHeader = true;
			return true;
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0003165C File Offset: 0x0002F85C
		private void ReadFooter()
		{
			byte[] array = new byte[8];
			long num = this.inf.TotalOut & (long)((ulong)-1);
			this.inputBuffer.Available += this.inf.RemainingInput;
			this.inf.Reset();
			int num2;
			for (int i = 8; i > 0; i -= num2)
			{
				num2 = this.inputBuffer.ReadClearTextBuffer(array, 8 - i, i);
				bool flag = num2 <= 0;
				if (flag)
				{
					throw new EndOfStreamException("EOS reading GZIP footer");
				}
			}
			int num3 = (int)(array[0] & byte.MaxValue) | (int)(array[1] & byte.MaxValue) << 8 | (int)(array[2] & byte.MaxValue) << 16 | (int)array[3] << 24;
			bool flag2 = num3 != (int)this.crc.Value;
			if (flag2)
			{
				throw new GZipException("GZIP crc sum mismatch, theirs \"" + num3.ToString() + "\" and ours \"" + ((int)this.crc.Value).ToString());
			}
			uint num4 = (uint)((int)(array[4] & byte.MaxValue) | (int)(array[5] & byte.MaxValue) << 8 | (int)(array[6] & byte.MaxValue) << 16 | (int)array[7] << 24);
			bool flag3 = num != (long)((ulong)num4);
			if (flag3)
			{
				throw new GZipException("Number of bytes mismatch in footer");
			}
			this.readGZIPHeader = false;
		}

		// Token: 0x0400048F RID: 1167
		protected UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc;

		// Token: 0x04000490 RID: 1168
		private bool readGZIPHeader;
	}
}
