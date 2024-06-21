using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x0200009B RID: 155
	[ComVisible(false)]
	public class OutputWindow
	{
		// Token: 0x06000727 RID: 1831 RVA: 0x00038A44 File Offset: 0x00036C44
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

		// Token: 0x06000728 RID: 1832 RVA: 0x00038AB0 File Offset: 0x00036CB0
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

		// Token: 0x06000729 RID: 1833 RVA: 0x00038B18 File Offset: 0x00036D18
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

		// Token: 0x0600072A RID: 1834 RVA: 0x00038C1C File Offset: 0x00036E1C
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

		// Token: 0x0600072B RID: 1835 RVA: 0x00038CE4 File Offset: 0x00036EE4
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

		// Token: 0x0600072C RID: 1836 RVA: 0x00038D64 File Offset: 0x00036F64
		public int GetFreeSpace()
		{
			return 32768 - this.windowFilled;
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x00038D8C File Offset: 0x00036F8C
		public int GetAvailable()
		{
			return this.windowFilled;
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x00038DAC File Offset: 0x00036FAC
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

		// Token: 0x0600072F RID: 1839 RVA: 0x00038E74 File Offset: 0x00037074
		public void Reset()
		{
			this.windowFilled = (this.windowEnd = 0);
		}

		// Token: 0x04000467 RID: 1127
		private const int WindowSize = 32768;

		// Token: 0x04000468 RID: 1128
		private const int WindowMask = 32767;

		// Token: 0x04000469 RID: 1129
		private byte[] window = new byte[32768];

		// Token: 0x0400046A RID: 1130
		private int windowEnd;

		// Token: 0x0400046B RID: 1131
		private int windowFilled;
	}
}
