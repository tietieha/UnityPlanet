using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A5 RID: 165
	[ComVisible(false)]
	public class TarOutputStream : Stream
	{
		// Token: 0x060007FD RID: 2045 RVA: 0x0003CCB4 File Offset: 0x0003AEB4
		public TarOutputStream(Stream outputStream) : this(outputStream, 20)
		{
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x0003CCC4 File Offset: 0x0003AEC4
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

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x0003CD28 File Offset: 0x0003AF28
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x0003CD4C File Offset: 0x0003AF4C
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

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x0003CD5C File Offset: 0x0003AF5C
		public override bool CanRead
		{
			get
			{
				return this.outputStream.CanRead;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000802 RID: 2050 RVA: 0x0003CD80 File Offset: 0x0003AF80
		public override bool CanSeek
		{
			get
			{
				return this.outputStream.CanSeek;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x0003CDA4 File Offset: 0x0003AFA4
		public override bool CanWrite
		{
			get
			{
				return this.outputStream.CanWrite;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000804 RID: 2052 RVA: 0x0003CDC8 File Offset: 0x0003AFC8
		public override long Length
		{
			get
			{
				return this.outputStream.Length;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x0003CDEC File Offset: 0x0003AFEC
		// (set) Token: 0x06000806 RID: 2054 RVA: 0x0003CE10 File Offset: 0x0003B010
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

		// Token: 0x06000807 RID: 2055 RVA: 0x0003CE20 File Offset: 0x0003B020
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.outputStream.Seek(offset, origin);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0003CE48 File Offset: 0x0003B048
		public override void SetLength(long value)
		{
			this.outputStream.SetLength(value);
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0003CE58 File Offset: 0x0003B058
		public override int ReadByte()
		{
			return this.outputStream.ReadByte();
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0003CE7C File Offset: 0x0003B07C
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.outputStream.Read(buffer, offset, count);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0003CEA4 File Offset: 0x0003B0A4
		public override void Flush()
		{
			this.outputStream.Flush();
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0003CEB4 File Offset: 0x0003B0B4
		public void Finish()
		{
			bool isEntryOpen = this.IsEntryOpen;
			if (isEntryOpen)
			{
				this.CloseEntry();
			}
			this.WriteEofBlock();
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0003CEE4 File Offset: 0x0003B0E4
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

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x0600080E RID: 2062 RVA: 0x0003CF24 File Offset: 0x0003B124
		public int RecordSize
		{
			get
			{
				return this.buffer.RecordSize;
			}
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0003CF48 File Offset: 0x0003B148
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.buffer.RecordSize;
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000810 RID: 2064 RVA: 0x0003CF6C File Offset: 0x0003B16C
		private bool IsEntryOpen
		{
			get
			{
				return this.currBytes < this.currSize;
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0003CF94 File Offset: 0x0003B194
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

		// Token: 0x06000812 RID: 2066 RVA: 0x0003D124 File Offset: 0x0003B324
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

		// Token: 0x06000813 RID: 2067 RVA: 0x0003D1D4 File Offset: 0x0003B3D4
		public override void WriteByte(byte value)
		{
			this.Write(new byte[]
			{
				value
			}, 0, 1);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0003D1EC File Offset: 0x0003B3EC
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

		// Token: 0x06000815 RID: 2069 RVA: 0x0003D414 File Offset: 0x0003B614
		private void WriteEofBlock()
		{
			Array.Clear(this.blockBuffer, 0, this.blockBuffer.Length);
			this.buffer.WriteBlock(this.blockBuffer);
		}

		// Token: 0x040004D1 RID: 1233
		private long currBytes;

		// Token: 0x040004D2 RID: 1234
		private int assemblyBufferLength;

		// Token: 0x040004D3 RID: 1235
		private bool isClosed;

		// Token: 0x040004D4 RID: 1236
		protected long currSize;

		// Token: 0x040004D5 RID: 1237
		protected byte[] blockBuffer;

		// Token: 0x040004D6 RID: 1238
		protected byte[] assemblyBuffer;

		// Token: 0x040004D7 RID: 1239
		protected TarBuffer buffer;

		// Token: 0x040004D8 RID: 1240
		protected Stream outputStream;
	}
}
