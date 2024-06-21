using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000088 RID: 136
	[ComVisible(false)]
	public class PendingBuffer
	{
		// Token: 0x060005F6 RID: 1526 RVA: 0x0002A780 File Offset: 0x00028980
		public PendingBuffer() : this(4096)
		{
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0002A790 File Offset: 0x00028990
		public PendingBuffer(int bufferSize)
		{
			this.buffer_ = new byte[bufferSize];
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0002A7A8 File Offset: 0x000289A8
		public void Reset()
		{
			this.start = (this.end = (this.bitCount = 0));
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0002A7D4 File Offset: 0x000289D4
		public void WriteByte(int value)
		{
			byte[] array = this.buffer_;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x0002A800 File Offset: 0x00028A00
		public void WriteShort(int value)
		{
			byte[] array = this.buffer_;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
			byte[] array2 = this.buffer_;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)(value >> 8);
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0002A848 File Offset: 0x00028A48
		public void WriteInt(int value)
		{
			byte[] array = this.buffer_;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
			byte[] array2 = this.buffer_;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)(value >> 8);
			byte[] array3 = this.buffer_;
			num = this.end;
			this.end = num + 1;
			array3[num] = (byte)(value >> 16);
			byte[] array4 = this.buffer_;
			num = this.end;
			this.end = num + 1;
			array4[num] = (byte)(value >> 24);
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0002A8CC File Offset: 0x00028ACC
		public void WriteBlock(byte[] block, int offset, int length)
		{
			Array.Copy(block, offset, this.buffer_, this.end, length);
			this.end += length;
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x0002A8F4 File Offset: 0x00028AF4
		public int BitCount
		{
			get
			{
				return this.bitCount;
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0002A914 File Offset: 0x00028B14
		public void AlignToByte()
		{
			bool flag = this.bitCount > 0;
			if (flag)
			{
				byte[] array = this.buffer_;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				bool flag2 = this.bitCount > 8;
				if (flag2)
				{
					byte[] array2 = this.buffer_;
					num = this.end;
					this.end = num + 1;
					array2[num] = (byte)(this.bits >> 8);
				}
			}
			this.bits = 0U;
			this.bitCount = 0;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0002A998 File Offset: 0x00028B98
		public void WriteBits(int b, int count)
		{
			this.bits |= (uint)((uint)b << this.bitCount);
			this.bitCount += count;
			bool flag = this.bitCount >= 16;
			if (flag)
			{
				byte[] array = this.buffer_;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				byte[] array2 = this.buffer_;
				num = this.end;
				this.end = num + 1;
				array2[num] = (byte)(this.bits >> 8);
				this.bits >>= 16;
				this.bitCount -= 16;
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0002AA44 File Offset: 0x00028C44
		public void WriteShortMSB(int s)
		{
			byte[] array = this.buffer_;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)(s >> 8);
			byte[] array2 = this.buffer_;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)s;
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x0002AA8C File Offset: 0x00028C8C
		public bool IsFlushed
		{
			get
			{
				return this.end == 0;
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0002AAB0 File Offset: 0x00028CB0
		public int Flush(byte[] output, int offset, int length)
		{
			bool flag = this.bitCount >= 8;
			if (flag)
			{
				byte[] array = this.buffer_;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				this.bits >>= 8;
				this.bitCount -= 8;
			}
			bool flag2 = length > this.end - this.start;
			if (flag2)
			{
				length = this.end - this.start;
				Array.Copy(this.buffer_, this.start, output, offset, length);
				this.start = 0;
				this.end = 0;
			}
			else
			{
				Array.Copy(this.buffer_, this.start, output, offset, length);
				this.start += length;
			}
			return length;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0002AB90 File Offset: 0x00028D90
		public byte[] ToByteArray()
		{
			byte[] array = new byte[this.end - this.start];
			Array.Copy(this.buffer_, this.start, array, 0, array.Length);
			this.start = 0;
			this.end = 0;
			return array;
		}

		// Token: 0x040003D8 RID: 984
		private byte[] buffer_;

		// Token: 0x040003D9 RID: 985
		private int start;

		// Token: 0x040003DA RID: 986
		private int end;

		// Token: 0x040003DB RID: 987
		private uint bits;

		// Token: 0x040003DC RID: 988
		private int bitCount;
	}
}
