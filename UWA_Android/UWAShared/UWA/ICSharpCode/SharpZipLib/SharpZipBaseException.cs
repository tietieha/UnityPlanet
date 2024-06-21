using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib
{
	// Token: 0x02000065 RID: 101
	[ComVisible(false)]
	[Serializable]
	public class SharpZipBaseException : ApplicationException
	{
		// Token: 0x060004A9 RID: 1193 RVA: 0x00027E8C File Offset: 0x0002608C
		protected SharpZipBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00027E98 File Offset: 0x00026098
		public SharpZipBaseException()
		{
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00027EA4 File Offset: 0x000260A4
		public SharpZipBaseException(string message) : base(message)
		{
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00027EB0 File Offset: 0x000260B0
		public SharpZipBaseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
