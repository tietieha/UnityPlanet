using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000BC RID: 188
	[ComVisible(false)]
	public static class BZip2
	{
		// Token: 0x0600080C RID: 2060 RVA: 0x00034180 File Offset: 0x00032380
		public static void Decompress(Stream inStream, Stream outStream, bool isStreamOwner)
		{
			bool flag = inStream == null || outStream == null;
			if (flag)
			{
				throw new Exception("Null Stream");
			}
			try
			{
				using (BZip2InputStream bzip2InputStream = new BZip2InputStream(inStream))
				{
					bzip2InputStream.IsStreamOwner = isStreamOwner;
					UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(bzip2InputStream, outStream, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					outStream.Close();
				}
			}
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x00034220 File Offset: 0x00032420
		public static void Compress(Stream inStream, Stream outStream, bool isStreamOwner, int level)
		{
			bool flag = inStream == null || outStream == null;
			if (flag)
			{
				throw new Exception("Null Stream");
			}
			try
			{
				using (BZip2OutputStream bzip2OutputStream = new BZip2OutputStream(outStream, level))
				{
					bzip2OutputStream.IsStreamOwner = isStreamOwner;
					UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(inStream, bzip2OutputStream, new byte[4096]);
				}
			}
			finally
			{
				if (isStreamOwner)
				{
					inStream.Close();
				}
			}
		}
	}
}
