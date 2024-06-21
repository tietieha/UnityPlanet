using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200008D RID: 141
	[ComVisible(false)]
	public class ZipOutputStream : DeflaterOutputStream
	{
		// Token: 0x06000668 RID: 1640 RVA: 0x000327B0 File Offset: 0x000309B0
		public ZipOutputStream(Stream baseOutputStream) : base(baseOutputStream, new Deflater(-1, true))
		{
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00032818 File Offset: 0x00030A18
		public ZipOutputStream(Stream baseOutputStream, int bufferSize) : base(baseOutputStream, new Deflater(-1, true), bufferSize)
		{
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x00032884 File Offset: 0x00030A84
		public bool IsFinished
		{
			get
			{
				return this.entries == null;
			}
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x000328A8 File Offset: 0x00030AA8
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

		// Token: 0x0600066C RID: 1644 RVA: 0x000328E8 File Offset: 0x00030AE8
		public void SetLevel(int level)
		{
			this.deflater_.SetLevel(level);
			this.defaultCompressionLevel = level;
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00032900 File Offset: 0x00030B00
		public int GetLevel()
		{
			return this.deflater_.GetLevel();
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x00032924 File Offset: 0x00030B24
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x00032944 File Offset: 0x00030B44
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

		// Token: 0x06000670 RID: 1648 RVA: 0x00032950 File Offset: 0x00030B50
		private void WriteLeShort(int value)
		{
			this.baseOutputStream_.WriteByte((byte)(value & 255));
			this.baseOutputStream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00032980 File Offset: 0x00030B80
		private void WriteLeInt(int value)
		{
			this.WriteLeShort(value);
			this.WriteLeShort(value >> 16);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00032998 File Offset: 0x00030B98
		private void WriteLeLong(long value)
		{
			this.WriteLeInt((int)value);
			this.WriteLeInt((int)(value >> 32));
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000329B4 File Offset: 0x00030BB4
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

		// Token: 0x06000674 RID: 1652 RVA: 0x00032FF0 File Offset: 0x000311F0
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

		// Token: 0x06000675 RID: 1653 RVA: 0x0003347C File Offset: 0x0003167C
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

		// Token: 0x06000676 RID: 1654 RVA: 0x000334E8 File Offset: 0x000316E8
		private static void AddExtraDataAES(ZipEntry entry, ZipExtraData extraData)
		{
			extraData.StartNewEntry();
			extraData.AddLeShort(2);
			extraData.AddLeShort(17729);
			extraData.AddData(entry.AESEncryptionStrength);
			extraData.AddLeShort((int)entry.CompressionMethod);
			extraData.AddNewEntry(39169);
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0003353C File Offset: 0x0003173C
		private void WriteAESHeader(ZipEntry entry)
		{
			byte[] array;
			byte[] array2;
			base.InitializeAESPassword(entry, base.Password, out array, out array2);
			this.baseOutputStream_.Write(array, 0, array.Length);
			this.baseOutputStream_.Write(array2, 0, array2.Length);
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00033584 File Offset: 0x00031784
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

		// Token: 0x06000679 RID: 1657 RVA: 0x000336AC File Offset: 0x000318AC
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

		// Token: 0x0600067A RID: 1658 RVA: 0x00033720 File Offset: 0x00031920
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

		// Token: 0x040003A4 RID: 932
		private ArrayList entries = new ArrayList();

		// Token: 0x040003A5 RID: 933
		private Crc32 crc = new Crc32();

		// Token: 0x040003A6 RID: 934
		private ZipEntry curEntry;

		// Token: 0x040003A7 RID: 935
		private int defaultCompressionLevel = -1;

		// Token: 0x040003A8 RID: 936
		private CompressionMethod curMethod = CompressionMethod.Deflated;

		// Token: 0x040003A9 RID: 937
		private long size;

		// Token: 0x040003AA RID: 938
		private long offset;

		// Token: 0x040003AB RID: 939
		private byte[] zipComment = new byte[0];

		// Token: 0x040003AC RID: 940
		private bool patchEntryHeader;

		// Token: 0x040003AD RID: 941
		private long crcPatchPos = -1L;

		// Token: 0x040003AE RID: 942
		private long sizePatchPos = -1L;

		// Token: 0x040003AF RID: 943
		private UseZip64 useZip64_ = UseZip64.Dynamic;
	}
}
