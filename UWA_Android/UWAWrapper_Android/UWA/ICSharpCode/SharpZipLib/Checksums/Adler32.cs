using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Checksums
{
	// Token: 0x020000B8 RID: 184
	[ComVisible(false)]
	public sealed class Adler32 : IChecksum
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x00033C00 File Offset: 0x00031E00
		public long Value
		{
			get
			{
				return (long)((ulong)this.checksum);
			}
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x00033C20 File Offset: 0x00031E20
		public Adler32()
		{
			this.Reset();
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00033C34 File Offset: 0x00031E34
		public void Reset()
		{
			this.checksum = 1U;
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00033C40 File Offset: 0x00031E40
		public void Update(int value)
		{
			uint num = this.checksum & 65535U;
			uint num2 = this.checksum >> 16;
			num = (num + (uint)(value & 255)) % 65521U;
			num2 = (num + num2) % 65521U;
			this.checksum = (num2 << 16) + num;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x00033C90 File Offset: 0x00031E90
		public void Update(byte[] buffer)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			this.Update(buffer, 0, buffer.Length);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x00033CC8 File Offset: 0x00031EC8
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

		// Token: 0x040004C5 RID: 1221
		private const uint BASE = 65521U;

		// Token: 0x040004C6 RID: 1222
		private uint checksum;
	}
}
