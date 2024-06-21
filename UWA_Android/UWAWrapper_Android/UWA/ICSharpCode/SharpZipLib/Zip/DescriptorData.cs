using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000079 RID: 121
	[ComVisible(false)]
	public class DescriptorData
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000544 RID: 1348 RVA: 0x00023B80 File Offset: 0x00021D80
		// (set) Token: 0x06000545 RID: 1349 RVA: 0x00023BA0 File Offset: 0x00021DA0
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

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x00023BAC File Offset: 0x00021DAC
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x00023BCC File Offset: 0x00021DCC
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

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x00023BD8 File Offset: 0x00021DD8
		// (set) Token: 0x06000549 RID: 1353 RVA: 0x00023BF8 File Offset: 0x00021DF8
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

		// Token: 0x04000320 RID: 800
		private long size;

		// Token: 0x04000321 RID: 801
		private long compressedSize;

		// Token: 0x04000322 RID: 802
		private long crc;
	}
}
