using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000072 RID: 114
	[ComVisible(false)]
	[Serializable]
	public class ZipException : SharpZipBaseException
	{
		// Token: 0x06000540 RID: 1344 RVA: 0x0002AC2C File Offset: 0x00028E2C
		protected ZipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0002AC38 File Offset: 0x00028E38
		public ZipException()
		{
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0002AC44 File Offset: 0x00028E44
		public ZipException(string message) : base(message)
		{
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0002AC50 File Offset: 0x00028E50
		public ZipException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
