using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Encryption;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007C RID: 124
	[ComVisible(false)]
	public class ZipInputStream : UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream
	{
		// Token: 0x06000570 RID: 1392 RVA: 0x000246BC File Offset: 0x000228BC
		public ZipInputStream(Stream baseInputStream) : base(baseInputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Inflater(true))
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x000246EC File Offset: 0x000228EC
		public ZipInputStream(Stream baseInputStream, int bufferSize) : base(baseInputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Inflater(true), bufferSize)
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x0002471C File Offset: 0x0002291C
		// (set) Token: 0x06000573 RID: 1395 RVA: 0x0002473C File Offset: 0x0002293C
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

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00024748 File Offset: 0x00022948
		public bool CanDecompressEntry
		{
			get
			{
				return this.entry != null && this.entry.CanDecompress;
			}
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x00024780 File Offset: 0x00022980
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

		// Token: 0x06000576 RID: 1398 RVA: 0x00024BD8 File Offset: 0x00022DD8
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

		// Token: 0x06000577 RID: 1399 RVA: 0x00024CB4 File Offset: 0x00022EB4
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

		// Token: 0x06000578 RID: 1400 RVA: 0x00024D70 File Offset: 0x00022F70
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

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x00024F1C File Offset: 0x0002311C
		public override int Available
		{
			get
			{
				return (this.entry != null) ? 1 : 0;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00024F48 File Offset: 0x00023148
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

		// Token: 0x0600057B RID: 1403 RVA: 0x00024FB0 File Offset: 0x000231B0
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

		// Token: 0x0600057C RID: 1404 RVA: 0x00024FF8 File Offset: 0x000231F8
		private int ReadingNotAvailable(byte[] destination, int offset, int count)
		{
			throw new InvalidOperationException("Unable to read from this stream");
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00025008 File Offset: 0x00023208
		private int ReadingNotSupported(byte[] destination, int offset, int count)
		{
			throw new ZipException("The compression method for this entry is not supported");
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00025018 File Offset: 0x00023218
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
				UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged pkzipClassicManaged = new UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged();
				byte[] rgbKey = UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(this.password));
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

		// Token: 0x0600057F RID: 1407 RVA: 0x00025224 File Offset: 0x00023424
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

		// Token: 0x06000580 RID: 1408 RVA: 0x000252B8 File Offset: 0x000234B8
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

		// Token: 0x06000581 RID: 1409 RVA: 0x00025598 File Offset: 0x00023798
		public override void Close()
		{
			this.internalReader = new ZipInputStream.ReadDataHandler(this.ReadingNotAvailable);
			this.crc = null;
			this.entry = null;
			base.Close();
		}

		// Token: 0x04000327 RID: 807
		private ZipInputStream.ReadDataHandler internalReader;

		// Token: 0x04000328 RID: 808
		private UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();

		// Token: 0x04000329 RID: 809
		private ZipEntry entry;

		// Token: 0x0400032A RID: 810
		private long size;

		// Token: 0x0400032B RID: 811
		private int method;

		// Token: 0x0400032C RID: 812
		private int flags;

		// Token: 0x0400032D RID: 813
		private string password;

		// Token: 0x02000120 RID: 288
		// (Invoke) Token: 0x060009A7 RID: 2471
		private delegate int ReadDataHandler(byte[] b, int offset, int length);
	}
}
