using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000CD RID: 205
	[ComVisible(false)]
	[Serializable]
	public class BZip2Exception : SharpZipBaseException
	{
		// Token: 0x060008EC RID: 2284 RVA: 0x000410A0 File Offset: 0x0003F2A0
		protected BZip2Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x000410AC File Offset: 0x0003F2AC
		public BZip2Exception()
		{
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x000410B8 File Offset: 0x0003F2B8
		public BZip2Exception(string message) : base(message)
		{
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x000410C4 File Offset: 0x0003F2C4
		public BZip2Exception(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
