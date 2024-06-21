using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200009A RID: 154
	[ComVisible(false)]
	public class InflaterInputStream : Stream
	{
		// Token: 0x06000710 RID: 1808 RVA: 0x000385BC File Offset: 0x000367BC
		public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 4096)
		{
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x000385D4 File Offset: 0x000367D4
		public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 4096)
		{
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x000385E8 File Offset: 0x000367E8
		public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
		{
			bool flag = baseInputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("baseInputStream");
			}
			bool flag2 = inflater == null;
			if (flag2)
			{
				throw new ArgumentNullException("inflater");
			}
			bool flag3 = bufferSize <= 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.baseInputStream = baseInputStream;
			this.inf = inflater;
			this.inputBuffer = new InflaterInputBuffer(baseInputStream, bufferSize);
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x0003866C File Offset: 0x0003686C
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x0003868C File Offset: 0x0003688C
		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner;
			}
			set
			{
				this.isStreamOwner = value;
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00038698 File Offset: 0x00036898
		public long Skip(long count)
		{
			bool flag = count <= 0L;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool canSeek = this.baseInputStream.CanSeek;
			long result;
			if (canSeek)
			{
				this.baseInputStream.Seek(count, SeekOrigin.Current);
				result = count;
			}
			else
			{
				int num = 2048;
				bool flag2 = count < (long)num;
				if (flag2)
				{
					num = (int)count;
				}
				byte[] buffer = new byte[num];
				int num2 = 1;
				long num3 = count;
				while (num3 > 0L && num2 > 0)
				{
					bool flag3 = num3 < (long)num;
					if (flag3)
					{
						num = (int)num3;
					}
					num2 = this.baseInputStream.Read(buffer, 0, num);
					num3 -= (long)num2;
				}
				result = count - num3;
			}
			return result;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00038770 File Offset: 0x00036970
		protected void StopDecrypting()
		{
			this.inputBuffer.CryptoTransform = null;
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x00038780 File Offset: 0x00036980
		public virtual int Available
		{
			get
			{
				return this.inf.IsFinished ? 0 : 1;
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x000387B0 File Offset: 0x000369B0
		protected void Fill()
		{
			bool flag = this.inputBuffer.Available <= 0;
			if (flag)
			{
				this.inputBuffer.Fill();
				bool flag2 = this.inputBuffer.Available <= 0;
				if (flag2)
				{
					throw new SharpZipBaseException("Unexpected EOF");
				}
			}
			this.inputBuffer.SetInflaterInput(this.inf);
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x00038820 File Offset: 0x00036A20
		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600071A RID: 1818 RVA: 0x00038844 File Offset: 0x00036A44
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x00038860 File Offset: 0x00036A60
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0003887C File Offset: 0x00036A7C
		public override long Length
		{
			get
			{
				return (long)this.inputBuffer.RawLength;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x000388A4 File Offset: 0x00036AA4
		// (set) Token: 0x0600071E RID: 1822 RVA: 0x000388C8 File Offset: 0x00036AC8
		public override long Position
		{
			get
			{
				return this.baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x000388D8 File Offset: 0x00036AD8
		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x000388E8 File Offset: 0x00036AE8
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x000388F8 File Offset: 0x00036AF8
		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00038908 File Offset: 0x00036B08
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00038918 File Offset: 0x00036B18
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00038928 File Offset: 0x00036B28
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00038938 File Offset: 0x00036B38
		public override void Close()
		{
			bool flag = !this.isClosed;
			if (flag)
			{
				this.isClosed = true;
				bool flag2 = this.isStreamOwner;
				if (flag2)
				{
					this.baseInputStream.Close();
				}
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00038980 File Offset: 0x00036B80
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isNeedingDictionary = this.inf.IsNeedingDictionary;
			if (isNeedingDictionary)
			{
				throw new SharpZipBaseException("Need a dictionary");
			}
			int num = count;
			for (;;)
			{
				int num2 = this.inf.Inflate(buffer, offset, num);
				offset += num2;
				num -= num2;
				bool flag = num == 0 || this.inf.IsFinished;
				if (flag)
				{
					break;
				}
				bool isNeedingInput = this.inf.IsNeedingInput;
				if (isNeedingInput)
				{
					this.Fill();
				}
				else
				{
					bool flag2 = num2 == 0;
					if (flag2)
					{
						goto Block_5;
					}
				}
			}
			return count - num;
			Block_5:
			throw new ZipException("Dont know what to do");
		}

		// Token: 0x04000461 RID: 1121
		protected Inflater inf;

		// Token: 0x04000462 RID: 1122
		protected InflaterInputBuffer inputBuffer;

		// Token: 0x04000463 RID: 1123
		private Stream baseInputStream;

		// Token: 0x04000464 RID: 1124
		protected long csize;

		// Token: 0x04000465 RID: 1125
		private bool isClosed;

		// Token: 0x04000466 RID: 1126
		private bool isStreamOwner = true;
	}
}
