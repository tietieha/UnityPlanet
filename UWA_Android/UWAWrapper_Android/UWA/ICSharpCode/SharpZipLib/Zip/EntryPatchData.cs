using System;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007A RID: 122
	internal class EntryPatchData
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x00023C14 File Offset: 0x00021E14
		// (set) Token: 0x0600054C RID: 1356 RVA: 0x00023C34 File Offset: 0x00021E34
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

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x00023C40 File Offset: 0x00021E40
		// (set) Token: 0x0600054E RID: 1358 RVA: 0x00023C60 File Offset: 0x00021E60
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

		// Token: 0x04000323 RID: 803
		private long sizePatchOffset_;

		// Token: 0x04000324 RID: 804
		private long crcPatchOffset_;
	}
}
