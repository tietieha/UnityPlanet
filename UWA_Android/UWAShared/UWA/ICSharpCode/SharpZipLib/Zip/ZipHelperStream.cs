using System;
using System.IO;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200008A RID: 138
	internal class ZipHelperStream : Stream
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x00030A30 File Offset: 0x0002EC30
		public ZipHelperStream(string name)
		{
			this.stream_ = new FileStream(name, FileMode.Open, FileAccess.ReadWrite);
			this.isOwner_ = true;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00030A50 File Offset: 0x0002EC50
		public ZipHelperStream(Stream stream)
		{
			this.stream_ = stream;
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x00030A64 File Offset: 0x0002EC64
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x00030A84 File Offset: 0x0002EC84
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

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x00030A90 File Offset: 0x0002EC90
		public override bool CanRead
		{
			get
			{
				return this.stream_.CanRead;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x00030AB4 File Offset: 0x0002ECB4
		public override bool CanSeek
		{
			get
			{
				return this.stream_.CanSeek;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x00030AD8 File Offset: 0x0002ECD8
		public override bool CanTimeout
		{
			get
			{
				return this.stream_.CanTimeout;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00030AFC File Offset: 0x0002ECFC
		public override long Length
		{
			get
			{
				return this.stream_.Length;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x00030B20 File Offset: 0x0002ED20
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x00030B44 File Offset: 0x0002ED44
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

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x00030B54 File Offset: 0x0002ED54
		public override bool CanWrite
		{
			get
			{
				return this.stream_.CanWrite;
			}
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00030B78 File Offset: 0x0002ED78
		public override void Flush()
		{
			this.stream_.Flush();
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00030B88 File Offset: 0x0002ED88
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream_.Seek(offset, origin);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00030BB0 File Offset: 0x0002EDB0
		public override void SetLength(long value)
		{
			this.stream_.SetLength(value);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00030BC0 File Offset: 0x0002EDC0
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream_.Read(buffer, offset, count);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00030BE8 File Offset: 0x0002EDE8
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream_.Write(buffer, offset, count);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00030BFC File Offset: 0x0002EDFC
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

		// Token: 0x0600063D RID: 1597 RVA: 0x00030C48 File Offset: 0x0002EE48
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

		// Token: 0x0600063E RID: 1598 RVA: 0x00030F38 File Offset: 0x0002F138
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

		// Token: 0x0600063F RID: 1599 RVA: 0x00030FB8 File Offset: 0x0002F1B8
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

		// Token: 0x06000640 RID: 1600 RVA: 0x00031054 File Offset: 0x0002F254
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

		// Token: 0x06000641 RID: 1601 RVA: 0x000311B4 File Offset: 0x0002F3B4
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

		// Token: 0x06000642 RID: 1602 RVA: 0x00031210 File Offset: 0x0002F410
		public int ReadLEInt()
		{
			return this.ReadLEShort() | this.ReadLEShort() << 16;
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0003123C File Offset: 0x0002F43C
		public long ReadLELong()
		{
			return (long)((ulong)this.ReadLEInt() | (ulong)((ulong)((long)this.ReadLEInt()) << 32));
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00031268 File Offset: 0x0002F468
		public void WriteLEShort(int value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00031298 File Offset: 0x0002F498
		public void WriteLEUshort(ushort value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8));
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000312C0 File Offset: 0x0002F4C0
		public void WriteLEInt(int value)
		{
			this.WriteLEShort(value);
			this.WriteLEShort(value >> 16);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000312D8 File Offset: 0x0002F4D8
		public void WriteLEUint(uint value)
		{
			this.WriteLEUshort((ushort)(value & 65535U));
			this.WriteLEUshort((ushort)(value >> 16));
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000312F8 File Offset: 0x0002F4F8
		public void WriteLELong(long value)
		{
			this.WriteLEInt((int)value);
			this.WriteLEInt((int)(value >> 32));
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00031310 File Offset: 0x0002F510
		public void WriteLEUlong(ulong value)
		{
			this.WriteLEUint((uint)(value & (ulong)-1));
			this.WriteLEUint((uint)(value >> 32));
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x0003132C File Offset: 0x0002F52C
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

		// Token: 0x0600064B RID: 1611 RVA: 0x000313E8 File Offset: 0x0002F5E8
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

		// Token: 0x04000398 RID: 920
		private bool isOwner_;

		// Token: 0x04000399 RID: 921
		private Stream stream_;
	}
}
