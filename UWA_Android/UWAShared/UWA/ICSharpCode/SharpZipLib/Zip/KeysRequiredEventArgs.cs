using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000079 RID: 121
	[ComVisible(false)]
	public class KeysRequiredEventArgs : EventArgs
	{
		// Token: 0x06000586 RID: 1414 RVA: 0x0002BEE0 File Offset: 0x0002A0E0
		public KeysRequiredEventArgs(string name)
		{
			this.fileName = name;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0002BEF4 File Offset: 0x0002A0F4
		public KeysRequiredEventArgs(string name, byte[] keyValue)
		{
			this.fileName = name;
			this.key = keyValue;
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x0002BF0C File Offset: 0x0002A10C
		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x0002BF2C File Offset: 0x0002A12C
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x0002BF4C File Offset: 0x0002A14C
		public byte[] Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		// Token: 0x0400035F RID: 863
		private string fileName;

		// Token: 0x04000360 RID: 864
		private byte[] key;
	}
}
