using System;
using System.IO;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007B RID: 123
	internal class ZipHelperStream : Stream
	{
		// Token: 0x06000550 RID: 1360 RVA: 0x00023C78 File Offset: 0x00021E78
		public ZipHelperStream(string name)
		{
			this.stream_ = new FileStream(name, FileMode.Open, FileAccess.ReadWrite);
			this.isOwner_ = true;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00023C98 File Offset: 0x00021E98
		public ZipHelperStream(Stream stream)
		{
			this.stream_ = stream;
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x00023CAC File Offset: 0x00021EAC
		// (set) Token: 0x06000553 RID: 1363 RVA: 0x00023CCC File Offset: 0x00021ECC
		public bool IsStreamOwner
		{
			get
			{
				return this.isOwner_;
			}
			set
			{
				this.isOwner_ = value;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x00023CD8 File Offset: 0x00021ED8
		public override bool CanRead
		{
			get
			{
				return this.stream_.CanRead;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x00023CFC File Offset: 0x00021EFC
		public override bool CanSeek
		{
			get
			{
				return this.stream_.CanSeek;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00023D20 File Offset: 0x00021F20
		public override bool CanTimeout
		{
			get
			{
				return this.stream_.CanTimeout;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x00023D44 File Offset: 0x00021F44
		public override long Length
		{
			get
			{
				return this.stream_.Length;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x00023D68 File Offset: 0x00021F68
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x00023D8C File Offset: 0x00021F8C
		public override long Position
		{
			get
			{
				return this.stream_.Position;
			}
			set
			{
				this.stream_.Position = value;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x00023D9C File Offset: 0x00021F9C
		public override bool CanWrite
		{
			get
			{
				return this.stream_.CanWrite;
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00023DC0 File Offset: 0x00021FC0
		public override void Flush()
		{
			this.stream_.Flush();
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00023DD0 File Offset: 0x00021FD0
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream_.Seek(offset, origin);
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00023DF8 File Offset: 0x00021FF8
		public override void SetLength(long value)
		{
			this.stream_.SetLength(value);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00023E08 File Offset: 0x00022008
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream_.Read(buffer, offset, count);
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00023E30 File Offset: 0x00022030
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream_.Write(buffer, offset, count);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00023E44 File Offset: 0x00022044
		public override void Close()
		{
			Stream stream = this.stream_;
			this.stream_ = null;
			bool flag = this.isOwner_ && stream != null;
			if (flag)
			{
				this.isOwner_ = false;
				stream.Close();
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00023E90 File Offset: 0x00022090
		private void WriteLocalHeader(ZipEntry entry, EntryPatchData patchData)
		{
			CompressionMethod compressionMethod = entry.CompressionMethod;
			bool flag = true;
			bool flag2 = false;
			this.WriteLEInt(67324752);
			this.WriteLEShort(entry.Version);
			this.WriteLEShort(entry.Flags);
			this.WriteLEShort((int)((byte)compressionMethod));
			this.WriteLEInt((int)entry.DosTime);
			bool flag3 = flag;
			if (flag3)
			{
				this.WriteLEInt((int)entry.Crc);
				bool localHeaderRequiresZip = entry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip)
				{
					this.WriteLEInt(-1);
					this.WriteLEInt(-1);
				}
				else
				{
					this.WriteLEInt(entry.IsCrypted ? ((int)entry.CompressedSize + 12) : ((int)entry.CompressedSize));
					this.WriteLEInt((int)entry.Size);
				}
			}
			else
			{
				bool flag4 = patchData != null;
				if (flag4)
				{
					patchData.CrcPatchOffset = this.stream_.Position;
				}
				this.WriteLEInt(0);
				bool flag5 = patchData != null;
				if (flag5)
				{
					patchData.SizePatchOffset = this.stream_.Position;
				}
				bool flag6 = entry.LocalHeaderRequiresZip64 && flag2;
				if (flag6)
				{
					this.WriteLEInt(-1);
					this.WriteLEInt(-1);
				}
				else
				{
					this.WriteLEInt(0);
					this.WriteLEInt(0);
				}
			}
			byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
			bool flag7 = array.Length > 65535;
			if (flag7)
			{
				throw new ZipException("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			bool flag8 = entry.LocalHeaderRequiresZip64 && (flag || flag2);
			if (flag8)
			{
				zipExtraData.StartNewEntry();
				bool flag9 = flag;
				if (flag9)
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
				bool flag10 = !zipExtraData.Find(1);
				if (flag10)
				{
					throw new ZipException("Internal error cant find extra data");
				}
				bool flag11 = patchData != null;
				if (flag11)
				{
					patchData.SizePatchOffset = (long)zipExtraData.CurrentReadIndex;
				}
			}
			else
			{
				zipExtraData.Delete(1);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			this.WriteLEShort(array.Length);
			this.WriteLEShort(entryData.Length);
			bool flag12 = array.Length != 0;
			if (flag12)
			{
				this.stream_.Write(array, 0, array.Length);
			}
			bool flag13 = entry.LocalHeaderRequiresZip64 && flag2;
			if (flag13)
			{
				patchData.SizePatchOffset += this.stream_.Position;
			}
			bool flag14 = entryData.Length != 0;
			if (flag14)
			{
				this.stream_.Write(entryData, 0, entryData.Length);
			}
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00024180 File Offset: 0x00022380
		public long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
		{
			long num = endLocation - (long)minimumBlockSize;
			bool flag = num < 0L;
			long result;
			if (flag)
			{
				result = -1L;
			}
			else
			{
				long num2 = Math.Max(num - (long)maximumVariableData, 0L);
				for (;;)
				{
					bool flag2 = num < num2;
					if (flag2)
					{
						break;
					}
					long num3 = num;
					num = num3 - 1L;
					this.Seek(num3, SeekOrigin.Begin);
					if (this.ReadLEInt() == signature)
					{
						goto Block_3;
					}
				}
				return -1L;
				Block_3:
				result = this.Position;
			}
			return result;
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00024200 File Offset: 0x00022400
		public void WriteZip64EndOfCentralDirectory(long noOfEntries, long sizeEntries, long centralDirOffset)
		{
			long position = this.stream_.Position;
			this.WriteLEInt(101075792);
			this.WriteLELong(44L);
			this.WriteLEShort(51);
			this.WriteLEShort(45);
			this.WriteLEInt(0);
			this.WriteLEInt(0);
			this.WriteLELong(noOfEntries);
			this.WriteLELong(noOfEntries);
			this.WriteLELong(sizeEntries);
			this.WriteLELong(centralDirOffset);
			this.WriteLEInt(117853008);
			this.WriteLEInt(0);
			this.WriteLELong(position);
			this.WriteLEInt(1);
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0002429C File Offset: 0x0002249C
		public void WriteEndOfCentralDirectory(long noOfEntries, long sizeEntries, long startOfCentralDirectory, byte[] comment)
		{
			bool flag = noOfEntries >= 65535L || startOfCentralDirectory >= (long)((ulong)-1) || sizeEntries >= (long)((ulong)-1);
			if (flag)
			{
				this.WriteZip64EndOfCentralDirectory(noOfEntries, sizeEntries, startOfCentralDirectory);
			}
			this.WriteLEInt(101010256);
			this.WriteLEShort(0);
			this.WriteLEShort(0);
			bool flag2 = noOfEntries >= 65535L;
			if (flag2)
			{
				this.WriteLEUshort(ushort.MaxValue);
				this.WriteLEUshort(ushort.MaxValue);
			}
			else
			{
				this.WriteLEShort((int)((short)noOfEntries));
				this.WriteLEShort((int)((short)noOfEntries));
			}
			bool flag3 = sizeEntries >= (long)((ulong)-1);
			if (flag3)
			{
				this.WriteLEUint(uint.MaxValue);
			}
			else
			{
				this.WriteLEInt((int)sizeEntries);
			}
			bool flag4 = startOfCentralDirectory >= (long)((ulong)-1);
			if (flag4)
			{
				this.WriteLEUint(uint.MaxValue);
			}
			else
			{
				this.WriteLEInt((int)startOfCentralDirectory);
			}
			int num = (comment != null) ? comment.Length : 0;
			bool flag5 = num > 65535;
			if (flag5)
			{
				throw new ZipException(string.Format("Comment length({0}) is too long can only be 64K", num));
			}
			this.WriteLEShort(num);
			bool flag6 = num > 0;
			if (flag6)
			{
				this.Write(comment, 0, comment.Length);
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x000243FC File Offset: 0x000225FC
		public int ReadLEShort()
		{
			int num = this.stream_.ReadByte();
			bool flag = num < 0;
			if (flag)
			{
				throw new EndOfStreamException();
			}
			int num2 = this.stream_.ReadByte();
			bool flag2 = num2 < 0;
			if (flag2)
			{
				throw new EndOfStreamException();
			}
			return num | num2 << 8;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00024458 File Offset: 0x00022658
		public int ReadLEInt()
		{
			return this.ReadLEShort() | this.ReadLEShort() << 16;
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00024484 File Offset: 0x00022684
		public long ReadLELong()
		{
			return (long)((ulong)this.ReadLEInt() | (ulong)((ulong)((long)this.ReadLEInt()) << 32));
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x000244B0 File Offset: 0x000226B0
		public void WriteLEShort(int value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x000244E0 File Offset: 0x000226E0
		public void WriteLEUshort(ushort value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8));
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x00024508 File Offset: 0x00022708
		public void WriteLEInt(int value)
		{
			this.WriteLEShort(value);
			this.WriteLEShort(value >> 16);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x00024520 File Offset: 0x00022720
		public void WriteLEUint(uint value)
		{
			this.WriteLEUshort((ushort)(value & 65535U));
			this.WriteLEUshort((ushort)(value >> 16));
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x00024540 File Offset: 0x00022740
		public void WriteLELong(long value)
		{
			this.WriteLEInt((int)value);
			this.WriteLEInt((int)(value >> 32));
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00024558 File Offset: 0x00022758
		public void WriteLEUlong(ulong value)
		{
			this.WriteLEUint((uint)(value & (ulong)-1));
			this.WriteLEUint((uint)(value >> 32));
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00024574 File Offset: 0x00022774
		public int WriteDataDescriptor(ZipEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			int num = 0;
			bool flag2 = (entry.Flags & 8) != 0;
			if (flag2)
			{
				this.WriteLEInt(134695760);
				this.WriteLEInt((int)entry.Crc);
				num += 8;
				bool localHeaderRequiresZip = entry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip)
				{
					this.WriteLELong(entry.CompressedSize);
					this.WriteLELong(entry.Size);
					num += 16;
				}
				else
				{
					this.WriteLEInt((int)entry.CompressedSize);
					this.WriteLEInt((int)entry.Size);
					num += 8;
				}
			}
			return num;
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x00024630 File Offset: 0x00022830
		public void ReadDataDescriptor(bool zip64, DescriptorData data)
		{
			int num = this.ReadLEInt();
			bool flag = num != 134695760;
			if (flag)
			{
				throw new ZipException("Data descriptor signature not found");
			}
			data.Crc = (long)this.ReadLEInt();
			if (zip64)
			{
				data.CompressedSize = this.ReadLELong();
				data.Size = this.ReadLELong();
			}
			else
			{
				data.CompressedSize = (long)this.ReadLEInt();
				data.Size = (long)this.ReadLEInt();
			}
		}

		// Token: 0x04000325 RID: 805
		private bool isOwner_;

		// Token: 0x04000326 RID: 806
		private Stream stream_;
	}
}
