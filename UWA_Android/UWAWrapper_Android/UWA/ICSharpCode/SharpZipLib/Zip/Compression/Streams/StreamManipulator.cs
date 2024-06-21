using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200008D RID: 141
	[ComVisible(false)]
	public class StreamManipulator
	{
		// Token: 0x06000656 RID: 1622 RVA: 0x0002C108 File Offset: 0x0002A308
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

		// Token: 0x06000657 RID: 1623 RVA: 0x0002C1C8 File Offset: 0x0002A3C8
		public void DropBits(int bitCount)
		{
			this.buffer_ >>= bitCount;
			this.bitsInBuffer_ -= bitCount;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0002C1EC File Offset: 0x0002A3EC
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

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x0002C228 File Offset: 0x0002A428
		public int AvailableBits
		{
			get
			{
				return this.bitsInBuffer_;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x0002C248 File Offset: 0x0002A448
		public int AvailableBytes
		{
			get
			{
				return this.windowEnd_ - this.windowStart_ + (this.bitsInBuffer_ >> 3);
			}
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0002C278 File Offset: 0x0002A478
		public void SkipToByteBoundary()
		{
			this.buffer_ >>= (this.bitsInBuffer_ & 7);
			this.bitsInBuffer_ &= -8;
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x0002C2A4 File Offset: 0x0002A4A4
		public bool IsNeedingInput
		{
			get
			{
				return this.windowStart_ == this.windowEnd_;
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0002C2CC File Offset: 0x0002A4CC
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

		// Token: 0x0600065E RID: 1630 RVA: 0x0002C420 File Offset: 0x0002A620
		public void Reset()
		{
			this.buffer_ = 0U;
			this.windowStart_ = (this.windowEnd_ = (this.bitsInBuffer_ = 0));
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0002C454 File Offset: 0x0002A654
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

		// Token: 0x040003F9 RID: 1017
		private byte[] window_;

		// Token: 0x040003FA RID: 1018
		private int windowStart_;

		// Token: 0x040003FB RID: 1019
		private int windowEnd_;

		// Token: 0x040003FC RID: 1020
		private uint buffer_;

		// Token: 0x040003FD RID: 1021
		private int bitsInBuffer_;
	}
}
