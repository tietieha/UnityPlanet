using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000064 RID: 100
	[ComVisible(false)]
	public interface ITaggedData
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000468 RID: 1128
		short TagID { get; }

		// Token: 0x06000469 RID: 1129
		void SetData(byte[] data, int offset, int count);

		// Token: 0x0600046A RID: 1130
		byte[] GetData();
	}
}
