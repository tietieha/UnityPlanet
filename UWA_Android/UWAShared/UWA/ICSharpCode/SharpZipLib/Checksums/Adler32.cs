using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Checksums
{
	// Token: 0x020000C7 RID: 199
	[ComVisible(false)]
	public sealed class Adler32 : IChecksum
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x000409B8 File Offset: 0x0003EBB8
		public long Value
		{
			get
			{
				return (long)((ulong)this.checksum);
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x000409D8 File Offset: 0x0003EBD8
		public Adler32()
		{
			this.Reset();
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x000409EC File Offset: 0x0003EBEC
		public void Reset()
		{
			this.checksum = 1U;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x000409F8 File Offset: 0x0003EBF8
		public void Update(int value)
		{
			uint num = this.checksum & 65535U;
			uint num2 = this.checksum >> 16;
			num = (num + (uint)(value & 255)) % 65521U;
			num2 = (num + num2) % 65521U;
			this.checksum = (num2 << 16) + num;
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00040A48 File Offset: 0x0003EC48
		public void Update(byte[] buffer)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			this.Update(buffer, 0, buffer.Length);
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x00040A80 File Offset: 0x0003EC80
		public void Update(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "cannot be negative");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "cannot be negative");
			}
			bool flag4 = offset >= buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("offset", "not a valid index into buffer");
			}
			bool flag5 = offset + count > buffer.Length;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("count", "exceeds buffer size");
			}
			uint num = this.checksum & 65535U;
			uint num2 = this.checksum >> 16;
			while (count > 0)
			{
				int num3 = 3800;
				bool flag6 = num3 > count;
				if (flag6)
				{
					num3 = count;
				}
				count -= num3;
				while (--num3 >= 0)
				{
					num += (uint)(buffer[offset++] & byte.MaxValue);
					num2 += num;
				}
				num %= 65521U;
				num2 %= 65521U;
			}
			this.checksum = (num2 << 16 | num);
		}

		// Token: 0x04000538 RID: 1336
		private const uint BASE = 65521U;

		// Token: 0x04000539 RID: 1337
		private uint checksum;
	}
}
