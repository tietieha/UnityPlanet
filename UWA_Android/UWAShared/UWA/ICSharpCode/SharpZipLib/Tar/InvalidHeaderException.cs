using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x0200009D RID: 157
	[ComVisible(false)]
	[Serializable]
	public class InvalidHeaderException : TarException
	{
		// Token: 0x0600073C RID: 1852 RVA: 0x00039318 File Offset: 0x00037518
		protected InvalidHeaderException(SerializationInfo information, StreamingContext context) : base(information, context)
		{
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00039324 File Offset: 0x00037524
		public InvalidHeaderException()
		{
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00039330 File Offset: 0x00037530
		public InvalidHeaderException(string message) : base(message)
		{
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0003933C File Offset: 0x0003753C
		public InvalidHeaderException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
