using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib
{
	// Token: 0x02000056 RID: 86
	[ComVisible(false)]
	[Serializable]
	public class SharpZipBaseException : ApplicationException
	{
		// Token: 0x060003CD RID: 973 RVA: 0x0001B0D4 File Offset: 0x000192D4
		protected SharpZipBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001B0E0 File Offset: 0x000192E0
		public SharpZipBaseException()
		{
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001B0EC File Offset: 0x000192EC
		public SharpZipBaseException(string message) : base(message)
		{
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0001B0F8 File Offset: 0x000192F8
		public SharpZipBaseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
