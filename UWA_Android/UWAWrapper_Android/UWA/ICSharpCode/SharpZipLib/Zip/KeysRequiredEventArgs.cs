using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006A RID: 106
	[ComVisible(false)]
	public class KeysRequiredEventArgs : EventArgs
	{
		// Token: 0x060004AA RID: 1194 RVA: 0x0001F128 File Offset: 0x0001D328
		public KeysRequiredEventArgs(string name)
		{
			this.fileName = name;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001F13C File Offset: 0x0001D33C
		public KeysRequiredEventArgs(string name, byte[] keyValue)
		{
			this.fileName = name;
			this.key = keyValue;
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001F154 File Offset: 0x0001D354
		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x0001F174 File Offset: 0x0001D374
		// (set) Token: 0x060004AE RID: 1198 RVA: 0x0001F194 File Offset: 0x0001D394
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

		// Token: 0x040002EC RID: 748
		private string fileName;

		// Token: 0x040002ED RID: 749
		private byte[] key;
	}
}
