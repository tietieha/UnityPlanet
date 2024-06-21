using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x02000098 RID: 152
	[ComVisible(false)]
	[Serializable]
	public class LzwException : SharpZipBaseException
	{
		// Token: 0x0600073B RID: 1851 RVA: 0x00030694 File Offset: 0x0002E894
		protected LzwException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x000306A0 File Offset: 0x0002E8A0
		public LzwException()
		{
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x000306AC File Offset: 0x0002E8AC
		public LzwException(string message) : base(message)
		{
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x000306B8 File Offset: 0x0002E8B8
		public LzwException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
