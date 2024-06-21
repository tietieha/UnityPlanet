using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x020000A7 RID: 167
	[ComVisible(false)]
	[Serializable]
	public class LzwException : SharpZipBaseException
	{
		// Token: 0x06000817 RID: 2071 RVA: 0x0003D44C File Offset: 0x0003B64C
		protected LzwException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0003D458 File Offset: 0x0003B658
		public LzwException()
		{
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0003D464 File Offset: 0x0003B664
		public LzwException(string message) : base(message)
		{
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0003D470 File Offset: 0x0003B670
		public LzwException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
