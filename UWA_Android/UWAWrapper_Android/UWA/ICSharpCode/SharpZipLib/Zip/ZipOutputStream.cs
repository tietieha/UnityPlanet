using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007E RID: 126
	[ComVisible(false)]
	public class ZipOutputStream : UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream
	{
		// Token: 0x0600058C RID: 1420 RVA: 0x000259F8 File Offset: 0x00023BF8
		public ZipOutputStream(Stream baseOutputStream) : base(baseOutputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Deflater(-1, true))
		{
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00025A60 File Offset: 0x00023C60
		public ZipOutputStream(Stream baseOutputStream, int bufferSize) : base(baseOutputStream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Deflater(-1, true), bufferSize)
		{
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x00025ACC File Offset: 0x00023CCC
		public bool IsFinished
		{
			get
			{
				return this.entries == null;
			}
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00025AF0 File Offset: 0x00023CF0
		public void SetComment(string comment)
		{
			byte[] array = ZipConstants.ConvertToArray(comment);
			bool flag = array.Length > 65535;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("comment");
			}
			this.zipComment = array;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00025B30 File Offset: 0x00023D30
		public void SetLevel(int level)
		{
			this.deflater_.SetLevel(level);
			this.defaultCompressionLevel = level;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00025B48 File Offset: 0x00023D48
		public int GetLevel()
		{
			return this.deflater_.GetLevel();
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x00025B6C File Offset: 0x00023D6C
		// (set) Token: 0x06000593 RID: 1427 RVA: 0x00025B8C File Offset: 0x00023D8C
		public UseZip64 UseZip64
		{
			get
			{
				return this.useZip64_;
			}
			set
			{
				this.useZip64_ = value;
			}
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x00025B98 File Offset: 0x00023D98
		private void WriteLeShort(int value)
		{
			this.baseOutputStream_.WriteByte((byte)(value & 255));
			this.baseOutputStream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00025BC8 File Offset: 0x00023DC8
		private void WriteLeInt(int value)
		{
			this.WriteLeShort(value);
			this.WriteLeShort(value >> 16);
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00025BE0 File Offset: 0x00023DE0
		private void WriteLeLong(long value)
		{
			this.WriteLeInt((int)value);
			this.WriteLeInt((int)(value >> 32));
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00025BFC File Offset: 0x00023DFC
		public void PutNextEntry(ZipEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			bool flag2 = this.entries == null;
			if (flag2)
			{
				throw new InvalidOperationException("ZipOutputStream was finished");
			}
			bool flag3 = this.curEntry != null;
			if (flag3)
			{
				this.CloseEntry();
			}
			bool flag4 = this.entries.Count == int.MaxValue;
			if (flag4)
			{
				throw new ZipException("Too many entries for Zip file");
			}
			CompressionMethod compressionMethod = entry.CompressionMethod;
			int level = this.defaultCompressionLevel;
			entry.Flags &= 2048;
			this.patchEntryHeader = false;
			bool flag5 = entry.Size == 0L;
			bool flag6;
			if (flag5)
			{
				entry.CompressedSize = entry.Size;
				entry.Crc = 0L;
				compressionMethod = CompressionMethod.Stored;
				flag6 = true;
			}
			else
			{
				flag6 = (entry.Size >= 0L && entry.HasCrc);
				bool flag7 = compressionMethod == CompressionMethod.Stored;
				if (flag7)
				{
					bool flag8 = !flag6;
					if (flag8)
					{
						bool flag9 = !base.CanPatchEntries;
						if (flag9)
						{
							compressionMethod = CompressionMethod.Deflated;
							level = 0;
						}
					}
					else
					{
						entry.CompressedSize = entry.Size;
						flag6 = entry.HasCrc;
					}
				}
			}
			bool flag10 = !flag6;
			if (flag10)
			{
				bool flag11 = !base.CanPatchEntries;
				if (flag11)
				{
					entry.Flags |= 8;
				}
				else
				{
					this.patchEntryHeader = true;
				}
			}
			bool flag12 = base.Password != null;
			if (flag12)
			{
				entry.IsCrypted = true;
				bool flag13 = entry.Crc < 0L;
				if (flag13)
				{
					entry.Flags |= 8;
				}
			}
			entry.Offset = this.offset;
			entry.CompressionMethod = compressionMethod;
			this.curMethod = compressionMethod;
			this.sizePatchPos = -1L;
			bool flag14 = this.useZip64_ == UseZip64.On || (entry.Size < 0L && this.useZip64_ == UseZip64.Dynamic);
			if (flag14)
			{
				entry.ForceZip64();
			}
			this.WriteLeInt(67324752);
			this.WriteLeShort(entry.Version);
			this.WriteLeShort(entry.Flags);
			this.WriteLeShort((int)((byte)entry.CompressionMethodForHeader));
			this.WriteLeInt((int)entry.DosTime);
			bool flag15 = flag6;
			if (flag15)
			{
				this.WriteLeInt((int)entry.Crc);
				bool localHeaderRequiresZip = entry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip)
				{
					this.WriteLeInt(-1);
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt(entry.IsCrypted ? ((int)entry.CompressedSize + 12) : ((int)entry.CompressedSize));
					this.WriteLeInt((int)entry.Size);
				}
			}
			else
			{
				bool flag16 = this.patchEntryHeader;
				if (flag16)
				{
					this.crcPatchPos = this.baseOutputStream_.Position;
				}
				this.WriteLeInt(0);
				bool flag17 = this.patchEntryHeader;
				if (flag17)
				{
					this.sizePatchPos = this.baseOutputStream_.Position;
				}
				bool flag18 = entry.LocalHeaderRequiresZip64 || this.patchEntryHeader;
				if (flag18)
				{
					this.WriteLeInt(-1);
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt(0);
					this.WriteLeInt(0);
				}
			}
			byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
			bool flag19 = array.Length > 65535;
			if (flag19)
			{
				throw new ZipException("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			bool localHeaderRequiresZip2 = entry.LocalHeaderRequiresZip64;
			if (localHeaderRequiresZip2)
			{
				zipExtraData.StartNewEntry();
				bool flag20 = flag6;
				if (flag20)
				{
					zipExtraData.AddLeLong(entry.Size);
					zipExtraData.AddLeLong(entry.CompressedSize);
				}
				else
				{
					zipExtraData.AddLeLong(-1L);
					zipExtraData.AddLeLong(-1L);
				}
				zipExtraData.AddNewEntry(1);
				bool flag21 = !zipExtraData.Find(1);
				if (flag21)
				{
					throw new ZipException("Internal error cant find extra data");
				}
				bool flag22 = this.patchEntryHeader;
				if (flag22)
				{
					this.sizePatchPos = (long)zipExtraData.CurrentReadIndex;
				}
			}
			else
			{
				zipExtraData.Delete(1);
			}
			bool flag23 = entry.AESKeySize > 0;
			if (flag23)
			{
				ZipOutputStream.AddExtraDataAES(entry, zipExtraData);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			this.WriteLeShort(array.Length);
			this.WriteLeShort(entryData.Length);
			bool flag24 = array.Length != 0;
			if (flag24)
			{
				this.baseOutputStream_.Write(array, 0, array.Length);
			}
			bool flag25 = entry.LocalHeaderRequiresZip64 && this.patchEntryHeader;
			if (flag25)
			{
				this.sizePatchPos += this.baseOutputStream_.Position;
			}
			bool flag26 = entryData.Length != 0;
			if (flag26)
			{
				this.baseOutputStream_.Write(entryData, 0, entryData.Length);
			}
			this.offset += (long)(30 + array.Length + entryData.Length);
			bool flag27 = entry.AESKeySize > 0;
			if (flag27)
			{
				this.offset += (long)entry.AESOverheadSize;
			}
			this.curEntry = entry;
			this.crc.Reset();
			bool flag28 = compressionMethod == CompressionMethod.Deflated;
			if (flag28)
			{
				this.deflater_.Reset();
				this.deflater_.SetLevel(level);
			}
			this.size = 0L;
			bool isCrypted = entry.IsCrypted;
			if (isCrypted)
			{
				bool flag29 = entry.AESKeySize > 0;
				if (flag29)
				{
					this.WriteAESHeader(entry);
				}
				else
				{
					bool flag30 = entry.Crc < 0L;
					if (flag30)
					{
						this.WriteEncryptionHeader(entry.DosTime << 16);
					}
					else
					{
						this.WriteEncryptionHeader(entry.Crc);
					}
				}
			}
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00026238 File Offset: 0x00024438
		public void CloseEntry()
		{
			bool flag = this.curEntry == null;
			if (flag)
			{
				throw new InvalidOperationException("No open entry");
			}
			long totalOut = this.size;
			bool flag2 = this.curMethod == CompressionMethod.Deflated;
			if (flag2)
			{
				bool flag3 = this.size >= 0L;
				if (flag3)
				{
					base.Finish();
					totalOut = this.deflater_.TotalOut;
				}
				else
				{
					this.deflater_.Reset();
				}
			}
			bool flag4 = this.curEntry.AESKeySize > 0;
			if (flag4)
			{
				this.baseOutputStream_.Write(this.AESAuthCode, 0, 10);
			}
			bool flag5 = this.curEntry.Size < 0L;
			if (flag5)
			{
				this.curEntry.Size = this.size;
			}
			else
			{
				bool flag6 = this.curEntry.Size != this.size;
				if (flag6)
				{
					throw new ZipException("size was " + this.size.ToString() + ", but I expected " + this.curEntry.Size.ToString());
				}
			}
			bool flag7 = this.curEntry.CompressedSize < 0L;
			if (flag7)
			{
				this.curEntry.CompressedSize = totalOut;
			}
			else
			{
				bool flag8 = this.curEntry.CompressedSize != totalOut;
				if (flag8)
				{
					throw new ZipException("compressed size was " + totalOut.ToString() + ", but I expected " + this.curEntry.CompressedSize.ToString());
				}
			}
			bool flag9 = this.curEntry.Crc < 0L;
			if (flag9)
			{
				this.curEntry.Crc = this.crc.Value;
			}
			else
			{
				bool flag10 = this.curEntry.Crc != this.crc.Value;
				if (flag10)
				{
					throw new ZipException("crc was " + this.crc.Value.ToString() + ", but I expected " + this.curEntry.Crc.ToString());
				}
			}
			this.offset += totalOut;
			bool isCrypted = this.curEntry.IsCrypted;
			if (isCrypted)
			{
				bool flag11 = this.curEntry.AESKeySize > 0;
				if (flag11)
				{
					this.curEntry.CompressedSize += (long)this.curEntry.AESOverheadSize;
				}
				else
				{
					this.curEntry.CompressedSize += 12L;
				}
			}
			bool flag12 = this.patchEntryHeader;
			if (flag12)
			{
				this.patchEntryHeader = false;
				long position = this.baseOutputStream_.Position;
				this.baseOutputStream_.Seek(this.crcPatchPos, SeekOrigin.Begin);
				this.WriteLeInt((int)this.curEntry.Crc);
				bool localHeaderRequiresZip = this.curEntry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip)
				{
					bool flag13 = this.sizePatchPos == -1L;
					if (flag13)
					{
						throw new ZipException("Entry requires zip64 but this has been turned off");
					}
					this.baseOutputStream_.Seek(this.sizePatchPos, SeekOrigin.Begin);
					this.WriteLeLong(this.curEntry.Size);
					this.WriteLeLong(this.curEntry.CompressedSize);
				}
				else
				{
					this.WriteLeInt((int)this.curEntry.CompressedSize);
					this.WriteLeInt((int)this.curEntry.Size);
				}
				this.baseOutputStream_.Seek(position, SeekOrigin.Begin);
			}
			bool flag14 = (this.curEntry.Flags & 8) != 0;
			if (flag14)
			{
				this.WriteLeInt(134695760);
				this.WriteLeInt((int)this.curEntry.Crc);
				bool localHeaderRequiresZip2 = this.curEntry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip2)
				{
					this.WriteLeLong(this.curEntry.CompressedSize);
					this.WriteLeLong(this.curEntry.Size);
					this.offset += 24L;
				}
				else
				{
					this.WriteLeInt((int)this.curEntry.CompressedSize);
					this.WriteLeInt((int)this.curEntry.Size);
					this.offset += 16L;
				}
			}
			this.entries.Add(this.curEntry);
			this.curEntry = null;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000266C4 File Offset: 0x000248C4
		private void WriteEncryptionHeader(long crcValue)
		{
			this.offset += 12L;
			base.InitializePassword(base.Password);
			byte[] array = new byte[12];
			Random random = new Random();
			random.NextBytes(array);
			array[11] = (byte)(crcValue >> 24);
			base.EncryptBlock(array, 0, array.Length);
			this.baseOutputStream_.Write(array, 0, array.Length);
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00026730 File Offset: 0x00024930
		private static void AddExtraDataAES(ZipEntry entry, ZipExtraData extraData)
		{
			extraData.StartNewEntry();
			extraData.AddLeShort(2);
			extraData.AddLeShort(17729);
			extraData.AddData(entry.AESEncryptionStrength);
			extraData.AddLeShort((int)entry.CompressionMethod);
			extraData.AddNewEntry(39169);
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x00026784 File Offset: 0x00024984
		private void WriteAESHeader(ZipEntry entry)
		{
			byte[] array;
			byte[] array2;
			base.InitializeAESPassword(entry, base.Password, out array, out array2);
			this.baseOutputStream_.Write(array, 0, array.Length);
			this.baseOutputStream_.Write(array2, 0, array2.Length);
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x000267CC File Offset: 0x000249CC
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool flag = this.curEntry == null;
			if (flag)
			{
				throw new InvalidOperationException("No open entry.");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag3 = offset < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be negative");
			}
			bool flag4 = count < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			bool flag5 = buffer.Length - offset < count;
			if (flag5)
			{
				throw new ArgumentException("Invalid offset/count combination");
			}
			this.crc.Update(buffer, offset, count);
			this.size += (long)count;
			CompressionMethod compressionMethod = this.curMethod;
			CompressionMethod compressionMethod2 = compressionMethod;
			if (compressionMethod2 != CompressionMethod.Stored)
			{
				if (compressionMethod2 == CompressionMethod.Deflated)
				{
					base.Write(buffer, offset, count);
				}
			}
			else
			{
				bool flag6 = base.Password != null;
				if (flag6)
				{
					this.CopyAndEncrypt(buffer, offset, count);
				}
				else
				{
					this.baseOutputStream_.Write(buffer, offset, count);
				}
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x000268F4 File Offset: 0x00024AF4
		private void CopyAndEncrypt(byte[] buffer, int offset, int count)
		{
			byte[] array = new byte[4096];
			while (count > 0)
			{
				int num = (count < 4096) ? count : 4096;
				Array.Copy(buffer, offset, array, 0, num);
				base.EncryptBlock(array, 0, num);
				this.baseOutputStream_.Write(array, 0, num);
				count -= num;
				offset += num;
			}
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00026968 File Offset: 0x00024B68
		public override void Finish()
		{
			bool flag = this.entries == null;
			if (!flag)
			{
				bool flag2 = this.curEntry != null;
				if (flag2)
				{
					this.CloseEntry();
				}
				long noOfEntries = (long)this.entries.Count;
				long num = 0L;
				foreach (object obj in this.entries)
				{
					ZipEntry zipEntry = (ZipEntry)obj;
					this.WriteLeInt(33639248);
					this.WriteLeShort(51);
					this.WriteLeShort(zipEntry.Version);
					this.WriteLeShort(zipEntry.Flags);
					this.WriteLeShort((int)((short)zipEntry.CompressionMethodForHeader));
					this.WriteLeInt((int)zipEntry.DosTime);
					this.WriteLeInt((int)zipEntry.Crc);
					bool flag3 = zipEntry.IsZip64Forced() || zipEntry.CompressedSize >= (long)((ulong)-1);
					if (flag3)
					{
						this.WriteLeInt(-1);
					}
					else
					{
						this.WriteLeInt((int)zipEntry.CompressedSize);
					}
					bool flag4 = zipEntry.IsZip64Forced() || zipEntry.Size >= (long)((ulong)-1);
					if (flag4)
					{
						this.WriteLeInt(-1);
					}
					else
					{
						this.WriteLeInt((int)zipEntry.Size);
					}
					byte[] array = ZipConstants.ConvertToArray(zipEntry.Flags, zipEntry.Name);
					bool flag5 = array.Length > 65535;
					if (flag5)
					{
						throw new ZipException("Name too long.");
					}
					ZipExtraData zipExtraData = new ZipExtraData(zipEntry.ExtraData);
					bool centralHeaderRequiresZip = zipEntry.CentralHeaderRequiresZip64;
					if (centralHeaderRequiresZip)
					{
						zipExtraData.StartNewEntry();
						bool flag6 = zipEntry.IsZip64Forced() || zipEntry.Size >= (long)((ulong)-1);
						if (flag6)
						{
							zipExtraData.AddLeLong(zipEntry.Size);
						}
						bool flag7 = zipEntry.IsZip64Forced() || zipEntry.CompressedSize >= (long)((ulong)-1);
						if (flag7)
						{
							zipExtraData.AddLeLong(zipEntry.CompressedSize);
						}
						bool flag8 = zipEntry.Offset >= (long)((ulong)-1);
						if (flag8)
						{
							zipExtraData.AddLeLong(zipEntry.Offset);
						}
						zipExtraData.AddNewEntry(1);
					}
					else
					{
						zipExtraData.Delete(1);
					}
					bool flag9 = zipEntry.AESKeySize > 0;
					if (flag9)
					{
						ZipOutputStream.AddExtraDataAES(zipEntry, zipExtraData);
					}
					byte[] entryData = zipExtraData.GetEntryData();
					byte[] array2 = (zipEntry.Comment != null) ? ZipConstants.ConvertToArray(zipEntry.Flags, zipEntry.Comment) : new byte[0];
					bool flag10 = array2.Length > 65535;
					if (flag10)
					{
						throw new ZipException("Comment too long.");
					}
					this.WriteLeShort(array.Length);
					this.WriteLeShort(entryData.Length);
					this.WriteLeShort(array2.Length);
					this.WriteLeShort(0);
					this.WriteLeShort(0);
					bool flag11 = zipEntry.ExternalFileAttributes != -1;
					if (flag11)
					{
						this.WriteLeInt(zipEntry.ExternalFileAttributes);
					}
					else
					{
						bool isDirectory = zipEntry.IsDirectory;
						if (isDirectory)
						{
							this.WriteLeInt(16);
						}
						else
						{
							this.WriteLeInt(0);
						}
					}
					bool flag12 = zipEntry.Offset >= (long)((ulong)-1);
					if (flag12)
					{
						this.WriteLeInt(-1);
					}
					else
					{
						this.WriteLeInt((int)zipEntry.Offset);
					}
					bool flag13 = array.Length != 0;
					if (flag13)
					{
						this.baseOutputStream_.Write(array, 0, array.Length);
					}
					bool flag14 = entryData.Length != 0;
					if (flag14)
					{
						this.baseOutputStream_.Write(entryData, 0, entryData.Length);
					}
					bool flag15 = array2.Length != 0;
					if (flag15)
					{
						this.baseOutputStream_.Write(array2, 0, array2.Length);
					}
					num += (long)(46 + array.Length + entryData.Length + array2.Length);
				}
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseOutputStream_))
				{
					zipHelperStream.WriteEndOfCentralDirectory(noOfEntries, num, this.offset, this.zipComment);
				}
				this.entries = null;
			}
		}

		// Token: 0x04000331 RID: 817
		private ArrayList entries = new ArrayList();

		// Token: 0x04000332 RID: 818
		private UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();

		// Token: 0x04000333 RID: 819
		private ZipEntry curEntry;

		// Token: 0x04000334 RID: 820
		private int defaultCompressionLevel = -1;

		// Token: 0x04000335 RID: 821
		private CompressionMethod curMethod = CompressionMethod.Deflated;

		// Token: 0x04000336 RID: 822
		private long size;

		// Token: 0x04000337 RID: 823
		private long offset;

		// Token: 0x04000338 RID: 824
		private byte[] zipComment = new byte[0];

		// Token: 0x04000339 RID: 825
		private bool patchEntryHeader;

		// Token: 0x0400033A RID: 826
		private long crcPatchPos = -1L;

		// Token: 0x0400033B RID: 827
		private long sizePatchPos = -1L;

		// Token: 0x0400033C RID: 828
		private UseZip64 useZip64_ = UseZip64.Dynamic;
	}
}
