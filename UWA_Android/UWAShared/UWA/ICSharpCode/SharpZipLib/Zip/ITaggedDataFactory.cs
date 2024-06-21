using System;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000077 RID: 119
	internal interface ITaggedDataFactory
	{
		// Token: 0x06000566 RID: 1382
		ITaggedData Create(short tag, byte[] data, int offset, int count);
	}
}
