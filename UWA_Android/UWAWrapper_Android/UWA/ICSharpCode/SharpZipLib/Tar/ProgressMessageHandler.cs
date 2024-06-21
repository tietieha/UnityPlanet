using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x0200008F RID: 143
	// (Invoke) Token: 0x06000665 RID: 1637
	[ComVisible(false)]
	public delegate void ProgressMessageHandler(TarArchive archive, TarEntry entry, string message);
}
