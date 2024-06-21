using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000096 RID: 150
	[ComVisible(false)]
	public class TarOutputStream : Stream
	{
		// Token: 0x06000721 RID: 1825 RVA: 0x0002FEFC File Offset: 0x0002E0FC
		public TarOutputStream(Stream outputStream) : this(outputStream, 20)
		{
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x0002FF0C File Offset: 0x0002E10C
		public TarOutputStream(Stream outputStream, int blockFactor)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			this.outputStream = outputStream;
			this.buffer = TarBuffer.CreateOutputTarBuffer(outputStream, blockFactor);
			this.assemblyBuffer = new byte[512];
			this.blockBuffer = new byte[512];
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0002FF70 File Offset: 0x0002E170
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x0002FF94 File Offset: 0x0002E194
		public bool IsStreamOwner
		{
			get
			{
				return this.buffer.IsStreamOwner;
			}
			set
			{
				this.buffer.IsStreamOwner = value;
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
		public override bool CanRead
		{
			get
			{
				return this.outputStream.CanRead;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000726 RID: 1830 RVA: 0x0002FFC8 File Offset: 0x0002E1C8
		public override bool CanSeek
		{
			get
			{
				return this.outputStream.CanSeek;
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0002FFEC File Offset: 0x0002E1EC
		public override bool CanWrite
		{
			get
			{
				return this.outputStream.CanWrite;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x00030010 File Offset: 0x0002E210
		public override long Length
		{
			get
			{
				return this.outputStream.Length;
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x00030034 File Offset: 0x0002E234
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x00030058 File Offset: 0x0002E258
		public override long Position
		{
			get
			{
				return this.outputStream.Position;
			}
			set
			{
				this.outputStream.Position = value;
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x00030068 File Offset: 0x0002E268
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.outputStream.Seek(offset, origin);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00030090 File Offset: 0x0002E290
		public override void SetLength(long value)
		{
			this.outputStream.SetLength(value);
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x000300A0 File Offset: 0x0002E2A0
		public override int ReadByte()
		{
			return this.outputStream.ReadByte();
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x000300C4 File Offset: 0x0002E2C4
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.outputStream.Read(buffer, offset, count);
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x000300EC File Offset: 0x0002E2EC
		public override void Flush()
		{
			this.outputStream.Flush();
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x000300FC File Offset: 0x0002E2FC
		public void Finish()
		{
			bool isEntryOpen = this.IsEntryOpen;
			if (isEntryOpen)
			{
				this.CloseEntry();
			}
			this.WriteEofBlock();
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0003012C File Offset: 0x0002E32C
		public override void Close()
		{
			bool flag = !this.isClosed;
			if (flag)
			{
				this.isClosed = true;
				this.Finish();
				this.buffer.Close();
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x0003016C File Offset: 0x0002E36C
		public int RecordSize
		{
			get
			{
				return this.buffer.RecordSize;
			}
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00030190 File Offset: 0x0002E390
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.buffer.RecordSize;
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x000301B4 File Offset: 0x0002E3B4
		private bool IsEntryOpen
		{
			get
			{
				return this.currBytes < this.currSize;
			}
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x000301DC File Offset: 0x0002E3DC
		public void PutNextEntry(TarEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			bool flag2 = entry.TarHeader.Name.Length >= 100;
			if (flag2)
			{
				TarHeader tarHeader = new TarHeader();
				tarHeader.TypeFlag = 76;
				tarHeader.Name += "././@LongLink";
				tarHeader.UserId = 0;
				tarHeader.GroupId = 0;
				tarHeader.GroupName = "";
				tarHeader.UserName = "";
				tarHeader.LinkName = "";
				tarHeader.Size = (long)entry.TarHeader.Name.Length;
				tarHeader.WriteHeader(this.blockBuffer);
				this.buffer.WriteBlock(this.blockBuffer);
				int i = 0;
				while (i < entry.TarHeader.Name.Length)
				{
					Array.Clear(this.blockBuffer, 0, this.blockBuffer.Length);
					TarHeader.GetAsciiBytes(entry.TarHeader.Name, i, this.blockBuffer, 0, 512);
					i += 512;
					this.buffer.WriteBlock(this.blockBuffer);
				}
			}
			entry.WriteEntryHeader(this.blockBuffer);
			this.buffer.WriteBlock(this.blockBuffer);
			this.currBytes = 0L;
			this.currSize = (entry.IsDirectory ? 0L : entry.Size);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x0003036C File Offset: 0x0002E56C
		public void CloseEntry()
		{
			bool flag = this.assemblyBufferLength > 0;
			if (flag)
			{
				Array.Clear(this.assemblyBuffer, this.assemblyBufferLength, this.assemblyBuffer.Length - this.assemblyBufferLength);
				this.buffer.WriteBlock(this.assemblyBuffer);
				this.currBytes += (long)this.assemblyBufferLength;
				this.assemblyBufferLength = 0;
			}
			bool flag2 = this.currBytes < this.currSize;
			if (flag2)
			{
				string message = string.Format("Entry closed at '{0}' before the '{1}' bytes specified in the header were written", this.currBytes, this.currSize);
				throw new TarException(message);
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0003041C File Offset: 0x0002E61C
		public override void WriteByte(byte value)
		{
			this.Write(new byte[]
			{
				value
			}, 0, 1);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00030434 File Offset: 0x0002E634
		public override void Write(byte[] buffer, int offset, int count)
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
			bool flag3 = buffer.Length - offset < count;
			if (flag3)
			{
				throw new ArgumentException("offset and count combination is invalid");
			}
			bool flag4 = count < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			bool flag5 = this.currBytes + (long)count > this.currSize;
			if (flag5)
			{
				string message = string.Format("request to write '{0}' bytes exceeds size in header of '{1}' bytes", count, this.currSize);
				throw new ArgumentOutOfRangeException("count", message);
			}
			bool flag6 = this.assemblyBufferLength > 0;
			if (flag6)
			{
				bool flag7 = this.assemblyBufferLength + count >= this.blockBuffer.Length;
				if (flag7)
				{
					int num = this.blockBuffer.Length - this.assemblyBufferLength;
					Array.Copy(this.assemblyBuffer, 0, this.blockBuffer, 0, this.assemblyBufferLength);
					Array.Copy(buffer, offset, this.blockBuffer, this.assemblyBufferLength, num);
					this.buffer.WriteBlock(this.blockBuffer);
					this.currBytes += (long)this.blockBuffer.Length;
					offset += num;
					count -= num;
					this.assemblyBufferLength = 0;
				}
				else
				{
					Array.Copy(buffer, offset, this.assemblyBuffer, this.assemblyBufferLength, count);
					offset += count;
					this.assemblyBufferLength += count;
					count -= count;
				}
			}
			while (count > 0)
			{
				bool flag8 = count < this.blockBuffer.Length;
				if (flag8)
				{
					Array.Copy(buffer, offset, this.assemblyBuffer, this.assemblyBufferLength, count);
					this.assemblyBufferLength += count;
					break;
				}
				this.buffer.WriteBlock(buffer, offset);
				int num2 = this.blockBuffer.Length;
				this.currBytes += (long)num2;
				count -= num2;
				offset += num2;
			}
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0003065C File Offset: 0x0002E85C
		private void WriteEofBlock()
		{
			Array.Clear(this.blockBuffer, 0, this.blockBuffer.Length);
			this.buffer.WriteBlock(this.blockBuffer);
		}

		// Token: 0x0400045E RID: 1118
		private long currBytes;

		// Token: 0x0400045F RID: 1119
		private int assemblyBufferLength;

		// Token: 0x04000460 RID: 1120
		private bool isClosed;

		// Token: 0x04000461 RID: 1121
		protected long currSize;

		// Token: 0x04000462 RID: 1122
		protected byte[] blockBuffer;

		// Token: 0x04000463 RID: 1123
		protected byte[] assemblyBuffer;

		// Token: 0x04000464 RID: 1124
		protected TarBuffer buffer;

		// Token: 0x04000465 RID: 1125
		protected Stream outputStream;
	}
}
