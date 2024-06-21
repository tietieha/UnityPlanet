using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C0 RID: 192
	[ComVisible(false)]
	public interface IScanFilter
	{
		// Token: 0x060008A8 RID: 2216
		bool IsMatch(string name);
	}
}
