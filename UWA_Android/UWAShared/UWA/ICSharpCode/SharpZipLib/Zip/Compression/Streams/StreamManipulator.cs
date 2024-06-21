using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200009C RID: 156
	[ComVisible(false)]
	public class StreamManipulator
	{
		// Token: 0x06000732 RID: 1842 RVA: 0x00038EC0 File Offset: 0x000370C0
		public int PeekBits(int bitCount)
		{
			bool flag = this.bitsInBuffer_ < bitCount;
			if (flag)
			{
				bool flag2 = this.windowStart_ == this.windowEnd_;
				if (flag2)
				{
					return -1;
				}
				uint num = this.buffer_;
				byte[] array = this.window_;
				int num2 = this.windowStart_;
				this.windowStart_ = num2 + 1;
				uint num3 = array[num2] & 255U;
				byte[] array2 = this.window_;
				num2 = this.windowStart_;
				this.windowStart_ = num2 + 1;
				this.buffer_ = (num | (num3 | (array2[num2] & 255U) << 8) << this.bitsInBuffer_);
				this.bitsInBuffer_ += 16;
			}
			return (int)((ulong)this.buffer_ & (ulong)((long)((1 << bitCount) - 1)));
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00038F80 File Offset: 0x00037180
		public void DropBits(int bitCount)
		{
			this.buffer_ >>= bitCount;
			this.bitsInBuffer_ -= bitCount;
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00038FA4 File Offset: 0x000371A4
		public int GetBits(int bitCount)
		{
			int num = this.PeekBits(bitCount);
			bool flag = num >= 0;
			if (flag)
			{
				this.DropBits(bitCount);
			}
			return num;
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x00038FE0 File Offset: 0x000371E0
		public int AvailableBits
		{
			get
			{
				return this.bitsInBuffer_;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000736 RID: 1846 RVA: 0x00039000 File Offset: 0x00037200
		public int AvailableBytes
		{
			get
			{
				return this.windowEnd_ - this.windowStart_ + (this.bitsInBuffer_ >> 3);
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00039030 File Offset: 0x00037230
		public void SkipToByteBoundary()
		{
			this.buffer_ >>= (this.bitsInBuffer_ & 7);
			this.bitsInBuffer_ &= -8;
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000738 RID: 1848 RVA: 0x0003905C File Offset: 0x0003725C
		public bool IsNeedingInput
		{
			get
			{
				return this.windowStart_ == this.windowEnd_;
			}
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00039084 File Offset: 0x00037284
		public int CopyBytes(byte[] output, int offset, int length)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			bool flag2 = (this.bitsInBuffer_ & 7) != 0;
			if (flag2)
			{
				throw new InvalidOperationException("Bit buffer is not byte aligned!");
			}
			int num = 0;
			while (this.bitsInBuffer_ > 0 && length > 0)
			{
				output[offset++] = (byte)this.buffer_;
				this.buffer_ >>= 8;
				this.bitsInBuffer_ -= 8;
				length--;
				num++;
			}
			bool flag3 = length == 0;
			int result;
			if (flag3)
			{
				result = num;
			}
			else
			{
				int num2 = this.windowEnd_ - this.windowStart_;
				bool flag4 = length > num2;
				if (flag4)
				{
					length = num2;
				}
				Array.Copy(this.window_, this.windowStart_, output, offset, length);
				this.windowStart_ += length;
				bool flag5 = (this.windowStart_ - this.windowEnd_ & 1) != 0;
				if (flag5)
				{
					byte[] array = this.window_;
					int num3 = this.windowStart_;
					this.windowStart_ = num3 + 1;
					this.buffer_ = (array[num3] & 255U);
					this.bitsInBuffer_ = 8;
				}
				result = num + length;
			}
			return result;
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x000391D8 File Offset: 0x000373D8
		public void Reset()
		{
			this.buffer_ = 0U;
			this.windowStart_ = (this.windowEnd_ = (this.bitsInBuffer_ = 0));
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0003920C File Offset: 0x0003740C
		public void SetInput(byte[] buffer, int offset, int count)
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
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			bool flag4 = this.windowStart_ < this.windowEnd_;
			if (flag4)
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			int num = offset + count;
			bool flag5 = offset > num || num > buffer.Length;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag6 = (count & 1) != 0;
			if (flag6)
			{
				this.buffer_ |= (uint)((uint)(buffer[offset++] & byte.MaxValue) << this.bitsInBuffer_);
				this.bitsInBuffer_ += 8;
			}
			this.window_ = buffer;
			this.windowStart_ = offset;
			this.windowEnd_ = num;
		}

		// Token: 0x0400046C RID: 1132
		private byte[] window_;

		// Token: 0x0400046D RID: 1133
		private int windowStart_;

		// Token: 0x0400046E RID: 1134
		private int windowEnd_;

		// Token: 0x0400046F RID: 1135
		private uint buffer_;

		// Token: 0x04000470 RID: 1136
		private int bitsInBuffer_;
	}
}
