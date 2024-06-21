using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B0 RID: 176
	[ComVisible(false)]
	public interface INameTransform
	{
		// Token: 0x060007CA RID: 1994
		string TransformFile(string name);

		// Token: 0x060007CB RID: 1995
		string TransformDirectory(string name);
	}
}
