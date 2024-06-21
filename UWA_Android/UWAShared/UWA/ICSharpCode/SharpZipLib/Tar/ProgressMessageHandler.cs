using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x0200009E RID: 158
	// (Invoke) Token: 0x06000741 RID: 1857
	[ComVisible(false)]
	public delegate void ProgressMessageHandler(TarArchive archive, TarEntry entry, string message);
}
