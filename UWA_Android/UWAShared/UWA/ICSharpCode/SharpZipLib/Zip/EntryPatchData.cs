using System;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000089 RID: 137
	internal class EntryPatchData
	{
		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x000309CC File Offset: 0x0002EBCC
		// (set) Token: 0x06000628 RID: 1576 RVA: 0x000309EC File Offset: 0x0002EBEC
		public long SizePatchOffset
		{
			get
			{
				return this.sizePatchOffset_;
			}
			set
			{
				this.sizePatchOffset_ = value;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x000309F8 File Offset: 0x0002EBF8
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x00030A18 File Offset: 0x0002EC18
		public long CrcPatchOffset
		{
			get
			{
				return this.crcPatchOffset_;
			}
			set
			{
				this.crcPatchOffset_ = value;
			}
		}

		// Token: 0x04000396 RID: 918
		private long sizePatchOffset_;

		// Token: 0x04000397 RID: 919
		private long crcPatchOffset_;
	}
}
