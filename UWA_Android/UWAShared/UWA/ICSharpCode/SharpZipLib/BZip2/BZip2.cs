using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000CB RID: 203
	[ComVisible(false)]
	public static class BZip2
	{
		// Token: 0x060008E8 RID: 2280 RVA: 0x00040F38 File Offset: 0x0003F138
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
					StreamUtils.Copy(bzip2InputStream, outStream, new byte[4096]);
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

		// Token: 0x060008E9 RID: 2281 RVA: 0x00040FD8 File Offset: 0x0003F1D8
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
					StreamUtils.Copy(inStream, bzip2OutputStream, new byte[4096]);
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
