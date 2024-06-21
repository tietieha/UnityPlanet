using System;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000068 RID: 104
	internal interface ITaggedDataFactory
	{
		// Token: 0x0600048A RID: 1162
		ITaggedData Create(short tag, byte[] data, int offset, int count);
	}
}
