using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000BE RID: 190
	[ComVisible(false)]
	[Serializable]
	public class BZip2Exception : SharpZipBaseException
	{
		// Token: 0x06000810 RID: 2064 RVA: 0x000342E8 File Offset: 0x000324E8
		protected BZip2Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x000342F4 File Offset: 0x000324F4
		public BZip2Exception()
		{
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00034300 File Offset: 0x00032500
		public BZip2Exception(string message) : base(message)
		{
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0003430C File Offset: 0x0003250C
		public BZip2Exception(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
