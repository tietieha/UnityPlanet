using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Encryption;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200008B RID: 139
	[ComVisible(false)]
	public class ZipInputStream : InflaterInputStream
	{
		// Token: 0x0600064C RID: 1612 RVA: 0x00031474 File Offset: 0x0002F674
		public ZipInputStream(Stream baseInputStream) : base(baseInputStream, new Inflater(true))
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000314A4 File Offset: 0x0002F6A4
		public ZipInputStream(Stream baseInputStream, int bufferSize) : base(baseInputStream, new Inflater(true), bufferSize)
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x000314D4 File Offset: 0x0002F6D4
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x000314F4 File Offset: 0x0002F6F4
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x00031500 File Offset: 0x0002F700
		public bool CanDecompressEntry
		{
			get
			{
				return this.entry != null && this.entry.CanDecompress;
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00031538 File Offset: 0x0002F738
		public ZipEntry GetNextEntry()
		{
			bool flag = this.crc == null;
			if (flag)
			{
				throw new InvalidOperationException("Closed.");
			}
			bool flag2 = this.entry != null;
			if (flag2)
			{
				this.CloseEntry();
			}
			int num = this.inputBuffer.ReadLeInt();
			bool flag3 = num == 33639248 || num == 101010256 || num == 84233040 || num == 117853008 || num == 101075792;
			ZipEntry result;
			if (flag3)
			{
				this.Close();
				result = null;
			}
			else
			{
				bool flag4 = num == 808471376 || num == 134695760;
				if (flag4)
				{
					num = this.inputBuffer.ReadLeInt();
				}
				bool flag5 = num != 67324752;
				if (flag5)
				{
					throw new ZipException("Wrong Local header signature: 0x" + string.Format("{0:X}", num));
				}
				short versionRequiredToExtract = (short)this.inputBuffer.ReadLeShort();
				this.flags = this.inputBuffer.ReadLeShort();
				this.method = this.inputBuffer.ReadLeShort();
				uint num2 = (uint)this.inputBuffer.ReadLeInt();
				int num3 = this.inputBuffer.ReadLeInt();
				this.csize = (long)this.inputBuffer.ReadLeInt();
				this.size = (long)this.inputBuffer.ReadLeInt();
				int num4 = this.inputBuffer.ReadLeShort();
				int num5 = this.inputBuffer.ReadLeShort();
				bool flag6 = (this.flags & 1) == 1;
				byte[] array = new byte[num4];
				this.inputBuffer.ReadRawBuffer(array);
				string name = ZipConstants.ConvertToStringExt(this.flags, array);
				this.entry = new ZipEntry(name, (int)versionRequiredToExtract);
				this.entry.Flags = this.flags;
				this.entry.CompressionMethod = (CompressionMethod)this.method;
				bool flag7 = (this.flags & 8) == 0;
				if (flag7)
				{
					this.entry.Crc = ((long)num3 & (long)((ulong)-1));
					this.entry.Size = (this.size & (long)((ulong)-1));
					this.entry.CompressedSize = (this.csize & (long)((ulong)-1));
					this.entry.CryptoCheckValue = (byte)(num3 >> 24 & 255);
				}
				else
				{
					bool flag8 = num3 != 0;
					if (flag8)
					{
						this.entry.Crc = ((long)num3 & (long)((ulong)-1));
					}
					bool flag9 = this.size != 0L;
					if (flag9)
					{
						this.entry.Size = (this.size & (long)((ulong)-1));
					}
					bool flag10 = this.csize != 0L;
					if (flag10)
					{
						this.entry.CompressedSize = (this.csize & (long)((ulong)-1));
					}
					this.entry.CryptoCheckValue = (byte)(num2 >> 8 & 255U);
				}
				this.entry.DosTime = (long)((ulong)num2);
				bool flag11 = num5 > 0;
				if (flag11)
				{
					byte[] array2 = new byte[num5];
					this.inputBuffer.ReadRawBuffer(array2);
					this.entry.ExtraData = array2;
				}
				this.entry.ProcessExtraData(true);
				bool flag12 = this.entry.CompressedSize >= 0L;
				if (flag12)
				{
					this.csize = this.entry.CompressedSize;
				}
				bool flag13 = this.entry.Size >= 0L;
				if (flag13)
				{
					this.size = this.entry.Size;
				}
				bool flag14 = this.method == 0 && ((!flag6 && this.csize != this.size) || (flag6 && this.csize - 12L != this.size));
				if (flag14)
				{
					throw new ZipException("Stored, but compressed != uncompressed");
				}
				bool flag15 = this.entry.IsCompressionMethodSupported();
				if (flag15)
				{
					this.internalReader = new ZipInputStream.ReadDataHandler(this.InitialRead);
				}
				else
				{
					this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotSupported);
				}
				result = this.entry;
			}
			return result;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00031990 File Offset: 0x0002FB90
		private void ReadDataDescriptor()
		{
			bool flag = this.inputBuffer.ReadLeInt() != 134695760;
			if (flag)
			{
				throw new ZipException("Data descriptor signature not found");
			}
			this.entry.Crc = ((long)this.inputBuffer.ReadLeInt() & (long)((ulong)-1));
			bool localHeaderRequiresZip = this.entry.LocalHeaderRequiresZip64;
			if (localHeaderRequiresZip)
			{
				this.csize = this.inputBuffer.ReadLeLong();
				this.size = this.inputBuffer.ReadLeLong();
			}
			else
			{
				this.csize = (long)this.inputBuffer.ReadLeInt();
				this.size = (long)this.inputBuffer.ReadLeInt();
			}
			this.entry.CompressedSize = this.csize;
			this.entry.Size = this.size;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00031A6C File Offset: 0x0002FC6C
		private void CompleteCloseEntry(bool testCrc)
		{
			base.StopDecrypting();
			bool flag = (this.flags & 8) != 0;
			if (flag)
			{
				this.ReadDataDescriptor();
			}
			this.size = 0L;
			bool flag2 = testCrc && (this.crc.Value & (long)((ulong)-1)) != this.entry.Crc && this.entry.Crc != -1L;
			if (flag2)
			{
				throw new ZipException("CRC mismatch");
			}
			this.crc.Reset();
			bool flag3 = this.method == 8;
			if (flag3)
			{
				this.inf.Reset();
			}
			this.entry = null;
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00031B28 File Offset: 0x0002FD28
		public void CloseEntry()
		{
			bool flag = this.crc == null;
			if (flag)
			{
				throw new InvalidOperationException("Closed");
			}
			bool flag2 = this.entry == null;
			if (!flag2)
			{
				bool flag3 = this.method == 8;
				if (flag3)
				{
					bool flag4 = (this.flags & 8) != 0;
					if (flag4)
					{
						byte[] array = new byte[4096];
						while (this.Read(array, 0, array.Length) > 0)
						{
						}
						return;
					}
					this.csize -= this.inf.TotalIn;
					this.inputBuffer.Available += this.inf.RemainingInput;
				}
				bool flag5 = (long)this.inputBuffer.Available > this.csize && this.csize >= 0L;
				if (flag5)
				{
					this.inputBuffer.Available = (int)((long)this.inputBuffer.Available - this.csize);
				}
				else
				{
					this.csize -= (long)this.inputBuffer.Available;
					this.inputBuffer.Available = 0;
					while (this.csize != 0L)
					{
						long num = base.Skip(this.csize);
						bool flag6 = num <= 0L;
						if (flag6)
						{
							throw new ZipException("Zip archive ends early.");
						}
						this.csize -= num;
					}
				}
				this.CompleteCloseEntry(false);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000655 RID: 1621 RVA: 0x00031CD4 File Offset: 0x0002FED4
		public override int Available
		{
			get
			{
				return (this.entry != null) ? 1 : 0;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x00031D00 File Offset: 0x0002FF00
		public override long Length
		{
			get
			{
				bool flag = this.entry != null;
				if (!flag)
				{
					throw new InvalidOperationException("No current entry");
				}
				bool flag2 = this.entry.Size >= 0L;
				if (flag2)
				{
					return this.entry.Size;
				}
				throw new ZipException("Length not available for the current entry");
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00031D68 File Offset: 0x0002FF68
		public override int ReadByte()
		{
			byte[] array = new byte[1];
			bool flag = this.Read(array, 0, 1) <= 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)(array[0] & byte.MaxValue);
			}
			return result;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00031DB0 File Offset: 0x0002FFB0
		private int ReadingNotAvailable(byte[] destination, int offset, int count)
		{
			throw new InvalidOperationException("Unable to read from this stream");
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00031DC0 File Offset: 0x0002FFC0
		private int ReadingNotSupported(byte[] destination, int offset, int count)
		{
			throw new ZipException("The compression method for this entry is not supported");
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00031DD0 File Offset: 0x0002FFD0
		private int InitialRead(byte[] destination, int offset, int count)
		{
			bool flag = !this.CanDecompressEntry;
			if (flag)
			{
				throw new ZipException("Library cannot extract this entry. Version required is (" + this.entry.Version.ToString() + ")");
			}
			bool isCrypted = this.entry.IsCrypted;
			if (isCrypted)
			{
				bool flag2 = this.password == null;
				if (flag2)
				{
					throw new ZipException("No password set.");
				}
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				byte[] rgbKey = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(this.password));
				this.inputBuffer.CryptoTransform = pkzipClassicManaged.CreateDecryptor(rgbKey, null);
				byte[] array = new byte[12];
				this.inputBuffer.ReadClearTextBuffer(array, 0, 12);
				bool flag3 = array[11] != this.entry.CryptoCheckValue;
				if (flag3)
				{
					throw new ZipException("Invalid password");
				}
				bool flag4 = this.csize >= 12L;
				if (flag4)
				{
					this.csize -= 12L;
				}
				else
				{
					bool flag5 = (this.entry.Flags & 8) == 0;
					if (flag5)
					{
						throw new ZipException(string.Format("Entry compressed size {0} too small for encryption", this.csize));
					}
				}
			}
			else
			{
				this.inputBuffer.CryptoTransform = null;
			}
			bool flag6 = this.csize > 0L || (this.flags & 8) != 0;
			int result;
			if (flag6)
			{
				bool flag7 = this.method == 8 && this.inputBuffer.Available > 0;
				if (flag7)
				{
					this.inputBuffer.SetInflaterInput(this.inf);
				}
				this.internalReader = new ZipInputStream.ReadDataHandler(this.BodyRead);
				result = this.BodyRead(destination, offset, count);
			}
			else
			{
				this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
				result = 0;
			}
			return result;
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00031FDC File Offset: 0x000301DC
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be negative");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			bool flag4 = buffer.Length - offset < count;
			if (flag4)
			{
				throw new ArgumentException("Invalid offset/count combination");
			}
			return this.internalReader(buffer, offset, count);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x00032070 File Offset: 0x00030270
		private int BodyRead(byte[] buffer, int offset, int count)
		{
			bool flag = this.crc == null;
			if (flag)
			{
				throw new InvalidOperationException("Closed");
			}
			bool flag2 = this.entry == null || count <= 0;
			int result;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				bool flag3 = offset + count > buffer.Length;
				if (flag3)
				{
					throw new ArgumentException("Offset + count exceeds buffer size");
				}
				bool flag4 = false;
				int num = this.method;
				int num2 = num;
				if (num2 != 0)
				{
					if (num2 == 8)
					{
						count = base.Read(buffer, offset, count);
						bool flag5 = count <= 0;
						if (flag5)
						{
							bool flag6 = !this.inf.IsFinished;
							if (flag6)
							{
								throw new ZipException("Inflater not finished!");
							}
							this.inputBuffer.Available = this.inf.RemainingInput;
							bool flag7 = (this.flags & 8) == 0 && ((this.inf.TotalIn != this.csize && this.csize != (long)((ulong)-1) && this.csize != -1L) || this.inf.TotalOut != this.size);
							if (flag7)
							{
								throw new ZipException(string.Concat(new string[]
								{
									"Size mismatch: ",
									this.csize.ToString(),
									";",
									this.size.ToString(),
									" <-> ",
									this.inf.TotalIn.ToString(),
									";",
									this.inf.TotalOut.ToString()
								}));
							}
							this.inf.Reset();
							flag4 = true;
						}
					}
				}
				else
				{
					bool flag8 = (long)count > this.csize && this.csize >= 0L;
					if (flag8)
					{
						count = (int)this.csize;
					}
					bool flag9 = count > 0;
					if (flag9)
					{
						count = this.inputBuffer.ReadClearTextBuffer(buffer, offset, count);
						bool flag10 = count > 0;
						if (flag10)
						{
							this.csize -= (long)count;
							this.size -= (long)count;
						}
					}
					bool flag11 = this.csize == 0L;
					if (flag11)
					{
						flag4 = true;
					}
					else
					{
						bool flag12 = count < 0;
						if (flag12)
						{
							throw new ZipException("EOF in stored block");
						}
					}
				}
				bool flag13 = count > 0;
				if (flag13)
				{
					this.crc.Update(buffer, offset, count);
				}
				bool flag14 = flag4;
				if (flag14)
				{
					this.CompleteCloseEntry(true);
				}
				result = count;
			}
			return result;
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x00032350 File Offset: 0x00030550
		public override void Close()
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
			this.crc = null;
			this.entry = null;
			base.Close();
		}

		// Token: 0x0400039A RID: 922
		private ZipInputStream.ReadDataHandler internalReader;

		// Token: 0x0400039B RID: 923
		private Crc32 crc = new Crc32();

		// Token: 0x0400039C RID: 924
		private ZipEntry entry;

		// Token: 0x0400039D RID: 925
		private long size;

		// Token: 0x0400039E RID: 926
		private int method;

		// Token: 0x0400039F RID: 927
		private int flags;

		// Token: 0x040003A0 RID: 928
		private string password;

		// Token: 0x02000156 RID: 342
		// (Invoke) Token: 0x06000AFD RID: 2813
		private delegate int ReadDataHandler(byte[] b, int offset, int length);
	}
}
