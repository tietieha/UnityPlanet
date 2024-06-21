using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000073 RID: 115
	[ComVisible(false)]
	public interface ITaggedData
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000544 RID: 1348
		short TagID { get; }

		// Token: 0x06000545 RID: 1349
		void SetData(byte[] data, int offset, int count);

		// Token: 0x06000546 RID: 1350
		byte[] GetData();
	}
}
