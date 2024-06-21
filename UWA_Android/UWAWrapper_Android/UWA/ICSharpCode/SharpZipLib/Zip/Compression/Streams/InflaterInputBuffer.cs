using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200008A RID: 138
	[ComVisible(false)]
	public class InflaterInputBuffer
	{
		// Token: 0x06000622 RID: 1570 RVA: 0x0002B248 File Offset: 0x00029448
		public InflaterInputBuffer(Stream stream) : this(stream, 4096)
		{
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0002B258 File Offset: 0x00029458
		public InflaterInputBuffer(Stream stream, int bufferSize)
		{
			this.inputStream = stream;
			bool flag = bufferSize < 1024;
			if (flag)
			{
				bufferSize = 1024;
			}
			this.rawData = new byte[bufferSize];
			this.clearText = this.rawData;
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x0002B2A8 File Offset: 0x000294A8
		public int RawLength
		{
			get
			{
				return this.rawLength;
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x0002B2C8 File Offset: 0x000294C8
		public byte[] RawData
		{
			get
			{
				return this.rawData;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x0002B2E8 File Offset: 0x000294E8
		public int ClearTextLength
		{
			get
			{
				return this.clearTextLength;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x0002B308 File Offset: 0x00029508
		public byte[] ClearText
		{
			get
			{
				return this.clearText;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x0002B328 File Offset: 0x00029528
		// (set) Token: 0x06000629 RID: 1577 RVA: 0x0002B348 File Offset: 0x00029548
		public int Available
		{
			get
			{
				return this.available;
			}
			set
			{
				this.available = value;
			}
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0002B354 File Offset: 0x00029554
		public void SetInflaterInput(Inflater inflater)
		{
			bool flag = this.available > 0;
			if (flag)
			{
				inflater.SetInput(this.clearText, this.clearTextLength - this.available, this.available);
				this.available = 0;
			}
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0002B3A0 File Offset: 0x000295A0
		public void Fill()
		{
			this.rawLength = 0;
			int num;
			for (int i = this.rawData.Length; i > 0; i -= num)
			{
				num = this.inputStream.Read(this.rawData, this.rawLength, i);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
				this.rawLength += num;
			}
			bool flag2 = this.cryptoTransform != null;
			if (flag2)
			{
				this.clearTextLength = this.cryptoTransform.TransformBlock(this.rawData, 0, this.rawLength, this.clearText, 0);
			}
			else
			{
				this.clearTextLength = this.rawLength;
			}
			this.available = this.clearTextLength;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x0002B468 File Offset: 0x00029668
		public int ReadRawBuffer(byte[] buffer)
		{
			return this.ReadRawBuffer(buffer, 0, buffer.Length);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0002B48C File Offset: 0x0002968C
		public int ReadRawBuffer(byte[] outBuffer, int offset, int length)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int i = length;
			while (i > 0)
			{
				bool flag2 = this.available <= 0;
				if (flag2)
				{
					this.Fill();
					bool flag3 = this.available <= 0;
					if (flag3)
					{
						return 0;
					}
				}
				int num2 = Math.Min(i, this.available);
				Array.Copy(this.rawData, this.rawLength - this.available, outBuffer, num, num2);
				num += num2;
				i -= num2;
				this.available -= num2;
			}
			return length;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0002B558 File Offset: 0x00029758
		public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int i = length;
			while (i > 0)
			{
				bool flag2 = this.available <= 0;
				if (flag2)
				{
					this.Fill();
					bool flag3 = this.available <= 0;
					if (flag3)
					{
						return 0;
					}
				}
				int num2 = Math.Min(i, this.available);
				Array.Copy(this.clearText, this.clearTextLength - this.available, outBuffer, num, num2);
				num += num2;
				i -= num2;
				this.available -= num2;
			}
			return length;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0002B624 File Offset: 0x00029824
		public int ReadLeByte()
		{
			bool flag = this.available <= 0;
			if (flag)
			{
				this.Fill();
				bool flag2 = this.available <= 0;
				if (flag2)
				{
					throw new ZipException("EOF in header");
				}
			}
			byte result = this.rawData[this.rawLength - this.available];
			this.available--;
			return (int)result;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0002B69C File Offset: 0x0002989C
		public int ReadLeShort()
		{
			return this.ReadLeByte() | this.ReadLeByte() << 8;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0002B6C4 File Offset: 0x000298C4
		public int ReadLeInt()
		{
			return this.ReadLeShort() | this.ReadLeShort() << 16;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0002B6F0 File Offset: 0x000298F0
		public long ReadLeLong()
		{
			return (long)((ulong)this.ReadLeInt() | (ulong)((ulong)((long)this.ReadLeInt()) << 32));
		}

		// Token: 0x170000ED RID: 237
		// (set) Token: 0x06000633 RID: 1587 RVA: 0x0002B71C File Offset: 0x0002991C
		public ICryptoTransform CryptoTransform
		{
			set
			{
				this.cryptoTransform = value;
				bool flag = this.cryptoTransform != null;
				if (flag)
				{
					bool flag2 = this.rawData == this.clearText;
					if (flag2)
					{
						bool flag3 = this.internalClearText == null;
						if (flag3)
						{
							this.internalClearText = new byte[this.rawData.Length];
						}
						this.clearText = this.internalClearText;
					}
					this.clearTextLength = this.rawLength;
					bool flag4 = this.available > 0;
					if (flag4)
					{
						this.cryptoTransform.TransformBlock(this.rawData, this.rawLength - this.available, this.available, this.clearText, this.rawLength - this.available);
					}
				}
				else
				{
					this.clearText = this.rawData;
					this.clearTextLength = this.rawLength;
				}
			}
		}

		// Token: 0x040003E6 RID: 998
		private int rawLength;

		// Token: 0x040003E7 RID: 999
		private byte[] rawData;

		// Token: 0x040003E8 RID: 1000
		private int clearTextLength;

		// Token: 0x040003E9 RID: 1001
		private byte[] clearText;

		// Token: 0x040003EA RID: 1002
		private byte[] internalClearText;

		// Token: 0x040003EB RID: 1003
		private int available;

		// Token: 0x040003EC RID: 1004
		private ICryptoTransform cryptoTransform;

		// Token: 0x040003ED RID: 1005
		private Stream inputStream;
	}
}
