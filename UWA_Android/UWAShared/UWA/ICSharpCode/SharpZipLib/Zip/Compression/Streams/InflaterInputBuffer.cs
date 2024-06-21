using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x02000099 RID: 153
	[ComVisible(false)]
	public class InflaterInputBuffer
	{
		// Token: 0x060006FE RID: 1790 RVA: 0x00038000 File Offset: 0x00036200
		public InflaterInputBuffer(Stream stream) : this(stream, 4096)
		{
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00038010 File Offset: 0x00036210
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

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000700 RID: 1792 RVA: 0x00038060 File Offset: 0x00036260
		public int RawLength
		{
			get
			{
				return this.rawLength;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x00038080 File Offset: 0x00036280
		public byte[] RawData
		{
			get
			{
				return this.rawData;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x000380A0 File Offset: 0x000362A0
		public int ClearTextLength
		{
			get
			{
				return this.clearTextLength;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x000380C0 File Offset: 0x000362C0
		public byte[] ClearText
		{
			get
			{
				return this.clearText;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x000380E0 File Offset: 0x000362E0
		// (set) Token: 0x06000705 RID: 1797 RVA: 0x00038100 File Offset: 0x00036300
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

		// Token: 0x06000706 RID: 1798 RVA: 0x0003810C File Offset: 0x0003630C
		public void SetInflaterInput(Inflater inflater)
		{
			bool flag = this.available > 0;
			if (flag)
			{
				inflater.SetInput(this.clearText, this.clearTextLength - this.available, this.available);
				this.available = 0;
			}
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00038158 File Offset: 0x00036358
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

		// Token: 0x06000708 RID: 1800 RVA: 0x00038220 File Offset: 0x00036420
		public int ReadRawBuffer(byte[] buffer)
		{
			return this.ReadRawBuffer(buffer, 0, buffer.Length);
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00038244 File Offset: 0x00036444
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

		// Token: 0x0600070A RID: 1802 RVA: 0x00038310 File Offset: 0x00036510
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

		// Token: 0x0600070B RID: 1803 RVA: 0x000383DC File Offset: 0x000365DC
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

		// Token: 0x0600070C RID: 1804 RVA: 0x00038454 File Offset: 0x00036654
		public int ReadLeShort()
		{
			return this.ReadLeByte() | this.ReadLeByte() << 8;
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0003847C File Offset: 0x0003667C
		public int ReadLeInt()
		{
			return this.ReadLeShort() | this.ReadLeShort() << 16;
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x000384A8 File Offset: 0x000366A8
		public long ReadLeLong()
		{
			return (long)((ulong)this.ReadLeInt() | (ulong)((ulong)((long)this.ReadLeInt()) << 32));
		}

		// Token: 0x17000141 RID: 321
		// (set) Token: 0x0600070F RID: 1807 RVA: 0x000384D4 File Offset: 0x000366D4
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

		// Token: 0x04000459 RID: 1113
		private int rawLength;

		// Token: 0x0400045A RID: 1114
		private byte[] rawData;

		// Token: 0x0400045B RID: 1115
		private int clearTextLength;

		// Token: 0x0400045C RID: 1116
		private byte[] clearText;

		// Token: 0x0400045D RID: 1117
		private byte[] internalClearText;

		// Token: 0x0400045E RID: 1118
		private int available;

		// Token: 0x0400045F RID: 1119
		private ICryptoTransform cryptoTransform;

		// Token: 0x04000460 RID: 1120
		private Stream inputStream;
	}
}
