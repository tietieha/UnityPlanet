using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200008B RID: 139
	[ComVisible(false)]
	public class InflaterInputStream : Stream
	{
		// Token: 0x06000634 RID: 1588 RVA: 0x0002B804 File Offset: 0x00029A04
		public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 4096)
		{
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0002B81C File Offset: 0x00029A1C
		public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 4096)
		{
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0002B830 File Offset: 0x00029A30
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

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x0002B8B4 File Offset: 0x00029AB4
		// (set) Token: 0x06000638 RID: 1592 RVA: 0x0002B8D4 File Offset: 0x00029AD4
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

		// Token: 0x06000639 RID: 1593 RVA: 0x0002B8E0 File Offset: 0x00029AE0
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

		// Token: 0x0600063A RID: 1594 RVA: 0x0002B9B8 File Offset: 0x00029BB8
		protected void StopDecrypting()
		{
			this.inputBuffer.CryptoTransform = null;
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x0002B9C8 File Offset: 0x00029BC8
		public virtual int Available
		{
			get
			{
				return this.inf.IsFinished ? 0 : 1;
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0002B9F8 File Offset: 0x00029BF8
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

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x0002BA68 File Offset: 0x00029C68
		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0002BA8C File Offset: 0x00029C8C
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x0002BAA8 File Offset: 0x00029CA8
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0002BAC4 File Offset: 0x00029CC4
		public override long Length
		{
			get
			{
				return (long)this.inputBuffer.RawLength;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x0002BAEC File Offset: 0x00029CEC
		// (set) Token: 0x06000642 RID: 1602 RVA: 0x0002BB10 File Offset: 0x00029D10
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

		// Token: 0x06000643 RID: 1603 RVA: 0x0002BB20 File Offset: 0x00029D20
		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0002BB30 File Offset: 0x00029D30
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0002BB40 File Offset: 0x00029D40
		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x0002BB50 File Offset: 0x00029D50
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0002BB60 File Offset: 0x00029D60
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0002BB70 File Offset: 0x00029D70
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0002BB80 File Offset: 0x00029D80
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

		// Token: 0x0600064A RID: 1610 RVA: 0x0002BBC8 File Offset: 0x00029DC8
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

		// Token: 0x040003EE RID: 1006
		protected Inflater inf;

		// Token: 0x040003EF RID: 1007
		protected InflaterInputBuffer inputBuffer;

		// Token: 0x040003F0 RID: 1008
		private Stream baseInputStream;

		// Token: 0x040003F1 RID: 1009
		protected long csize;

		// Token: 0x040003F2 RID: 1010
		private bool isClosed;

		// Token: 0x040003F3 RID: 1011
		private bool isStreamOwner = true;
	}
}
