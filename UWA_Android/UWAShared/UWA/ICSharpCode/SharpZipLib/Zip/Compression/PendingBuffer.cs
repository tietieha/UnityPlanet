using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000097 RID: 151
	[ComVisible(false)]
	public class PendingBuffer
	{
		// Token: 0x060006D2 RID: 1746 RVA: 0x00037538 File Offset: 0x00035738
		public PendingBuffer() : this(4096)
		{
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00037548 File Offset: 0x00035748
		public PendingBuffer(int bufferSize)
		{
			this.buffer_ = new byte[bufferSize];
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00037560 File Offset: 0x00035760
		public void Reset()
		{
			this.start = (this.end = (this.bitCount = 0));
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0003758C File Offset: 0x0003578C
		public void WriteByte(int value)
		{
			byte[] array = this.buffer_;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x000375B8 File Offset: 0x000357B8
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

		// Token: 0x060006D7 RID: 1751 RVA: 0x00037600 File Offset: 0x00035800
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

		// Token: 0x060006D8 RID: 1752 RVA: 0x00037684 File Offset: 0x00035884
		public void WriteBlock(byte[] block, int offset, int length)
		{
			Array.Copy(block, offset, this.buffer_, this.end, length);
			this.end += length;
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x000376AC File Offset: 0x000358AC
		public int BitCount
		{
			get
			{
				return this.bitCount;
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x000376CC File Offset: 0x000358CC
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

		// Token: 0x060006DB RID: 1755 RVA: 0x00037750 File Offset: 0x00035950
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

		// Token: 0x060006DC RID: 1756 RVA: 0x000377FC File Offset: 0x000359FC
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

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00037844 File Offset: 0x00035A44
		public bool IsFlushed
		{
			get
			{
				return this.end == 0;
			}
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x00037868 File Offset: 0x00035A68
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

		// Token: 0x060006DF RID: 1759 RVA: 0x00037948 File Offset: 0x00035B48
		public byte[] ToByteArray()
		{
			byte[] array = new byte[this.end - this.start];
			Array.Copy(this.buffer_, this.start, array, 0, array.Length);
			this.start = 0;
			this.end = 0;
			return array;
		}

		// Token: 0x0400044B RID: 1099
		private byte[] buffer_;

		// Token: 0x0400044C RID: 1100
		private int start;

		// Token: 0x0400044D RID: 1101
		private int end;

		// Token: 0x0400044E RID: 1102
		private uint bits;

		// Token: 0x0400044F RID: 1103
		private int bitCount;
	}
}
