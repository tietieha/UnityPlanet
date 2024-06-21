using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200008C RID: 140
	[ComVisible(false)]
	public class OutputWindow
	{
		// Token: 0x0600064B RID: 1611 RVA: 0x0002BC8C File Offset: 0x00029E8C
		public void Write(int value)
		{
			int num = this.windowFilled;
			this.windowFilled = num + 1;
			bool flag = num == 32768;
			if (flag)
			{
				throw new InvalidOperationException("Window full");
			}
			byte[] array = this.window;
			num = this.windowEnd;
			this.windowEnd = num + 1;
			array[num] = (byte)value;
			this.windowEnd &= 32767;
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x0002BCF8 File Offset: 0x00029EF8
		private void SlowRepeat(int repStart, int length, int distance)
		{
			while (length-- > 0)
			{
				byte[] array = this.window;
				int num = this.windowEnd;
				this.windowEnd = num + 1;
				array[num] = this.window[repStart++];
				this.windowEnd &= 32767;
				repStart &= 32767;
			}
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0002BD60 File Offset: 0x00029F60
		public void Repeat(int length, int distance)
		{
			bool flag = (this.windowFilled += length) > 32768;
			if (flag)
			{
				throw new InvalidOperationException("Window full");
			}
			int num = this.windowEnd - distance & 32767;
			int num2 = 32768 - length;
			bool flag2 = num <= num2 && this.windowEnd < num2;
			if (flag2)
			{
				bool flag3 = length <= distance;
				if (flag3)
				{
					Array.Copy(this.window, num, this.window, this.windowEnd, length);
					this.windowEnd += length;
				}
				else
				{
					while (length-- > 0)
					{
						byte[] array = this.window;
						int num3 = this.windowEnd;
						this.windowEnd = num3 + 1;
						array[num3] = this.window[num++];
					}
				}
			}
			else
			{
				this.SlowRepeat(num, length, distance);
			}
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0002BE64 File Offset: 0x0002A064
		public int CopyStored(StreamManipulator input, int length)
		{
			length = Math.Min(Math.Min(length, 32768 - this.windowFilled), input.AvailableBytes);
			int num = 32768 - this.windowEnd;
			bool flag = length > num;
			int num2;
			if (flag)
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, num);
				bool flag2 = num2 == num;
				if (flag2)
				{
					num2 += input.CopyBytes(this.window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, length);
			}
			this.windowEnd = (this.windowEnd + num2 & 32767);
			this.windowFilled += num2;
			return num2;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0002BF2C File Offset: 0x0002A12C
		public void CopyDict(byte[] dictionary, int offset, int length)
		{
			bool flag = dictionary == null;
			if (flag)
			{
				throw new ArgumentNullException("dictionary");
			}
			bool flag2 = this.windowFilled > 0;
			if (flag2)
			{
				throw new InvalidOperationException();
			}
			bool flag3 = length > 32768;
			if (flag3)
			{
				offset += length - 32768;
				length = 32768;
			}
			Array.Copy(dictionary, offset, this.window, 0, length);
			this.windowEnd = (length & 32767);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0002BFAC File Offset: 0x0002A1AC
		public int GetFreeSpace()
		{
			return 32768 - this.windowFilled;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0002BFD4 File Offset: 0x0002A1D4
		public int GetAvailable()
		{
			return this.windowFilled;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0002BFF4 File Offset: 0x0002A1F4
		public int CopyOutput(byte[] output, int offset, int len)
		{
			int num = this.windowEnd;
			bool flag = len > this.windowFilled;
			if (flag)
			{
				len = this.windowFilled;
			}
			else
			{
				num = (this.windowEnd - this.windowFilled + len & 32767);
			}
			int num2 = len;
			int num3 = len - num;
			bool flag2 = num3 > 0;
			if (flag2)
			{
				Array.Copy(this.window, 32768 - num3, output, offset, num3);
				offset += num3;
				len = num;
			}
			Array.Copy(this.window, num - len, output, offset, len);
			this.windowFilled -= num2;
			bool flag3 = this.windowFilled < 0;
			if (flag3)
			{
				throw new InvalidOperationException();
			}
			return num2;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0002C0BC File Offset: 0x0002A2BC
		public void Reset()
		{
			this.windowFilled = (this.windowEnd = 0);
		}

		// Token: 0x040003F4 RID: 1012
		private const int WindowSize = 32768;

		// Token: 0x040003F5 RID: 1013
		private const int WindowMask = 32767;

		// Token: 0x040003F6 RID: 1014
		private byte[] window = new byte[32768];

		// Token: 0x040003F7 RID: 1015
		private int windowEnd;

		// Token: 0x040003F8 RID: 1016
		private int windowFilled;
	}
}
