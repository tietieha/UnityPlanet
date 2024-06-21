using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x020000AA RID: 170
	[ComVisible(false)]
	[Serializable]
	public class GZipException : SharpZipBaseException
	{
		// Token: 0x06000831 RID: 2097 RVA: 0x0003DE8C File Offset: 0x0003C08C
		protected GZipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0003DE98 File Offset: 0x0003C098
		public GZipException()
		{
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0003DEA4 File Offset: 0x0003C0A4
		public GZipException(string message) : base(message)
		{
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0003DEB0 File Offset: 0x0003C0B0
		public GZipException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
