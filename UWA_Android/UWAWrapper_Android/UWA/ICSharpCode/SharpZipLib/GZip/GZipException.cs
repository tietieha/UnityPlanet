using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.GZip
{
	// Token: 0x0200009B RID: 155
	[ComVisible(false)]
	[Serializable]
	public class GZipException : SharpZipBaseException
	{
		// Token: 0x06000755 RID: 1877 RVA: 0x000310D4 File Offset: 0x0002F2D4
		protected GZipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x000310E0 File Offset: 0x0002F2E0
		public GZipException()
		{
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x000310EC File Offset: 0x0002F2EC
		public GZipException(string message) : base(message)
		{
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x000310F8 File Offset: 0x0002F2F8
		public GZipException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
