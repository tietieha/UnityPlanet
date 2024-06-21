using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B1 RID: 177
	[ComVisible(false)]
	public interface IScanFilter
	{
		// Token: 0x060007CC RID: 1996
		bool IsMatch(string name);
	}
}
