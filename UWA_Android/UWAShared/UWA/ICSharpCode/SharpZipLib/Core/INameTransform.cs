using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000BF RID: 191
	[ComVisible(false)]
	public interface INameTransform
	{
		// Token: 0x060008A6 RID: 2214
		string TransformFile(string name);

		// Token: 0x060008A7 RID: 2215
		string TransformDirectory(string name);
	}
}
