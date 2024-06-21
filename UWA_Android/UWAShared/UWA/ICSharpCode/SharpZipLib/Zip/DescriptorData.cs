using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000088 RID: 136
	[ComVisible(false)]
	public class DescriptorData
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x00030938 File Offset: 0x0002EB38
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x00030958 File Offset: 0x0002EB58
		public long CompressedSize
		{
			get
			{
				return this.compressedSize;
			}
			set
			{
				this.compressedSize = value;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00030964 File Offset: 0x0002EB64
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x00030984 File Offset: 0x0002EB84
		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x00030990 File Offset: 0x0002EB90
		// (set) Token: 0x06000625 RID: 1573 RVA: 0x000309B0 File Offset: 0x0002EBB0
		public long Crc
		{
			get
			{
				return this.crc;
			}
			set
			{
				this.crc = (value & (long)((ulong)-1));
			}
		}

		// Token: 0x04000393 RID: 915
		private long size;

		// Token: 0x04000394 RID: 916
		private long compressedSize;

		// Token: 0x04000395 RID: 917
		private long crc;
	}
}
