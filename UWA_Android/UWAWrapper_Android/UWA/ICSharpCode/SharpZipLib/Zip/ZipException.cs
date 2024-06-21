using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000063 RID: 99
	[ComVisible(false)]
	[Serializable]
	public class ZipException : SharpZipBaseException
	{
		// Token: 0x06000464 RID: 1124 RVA: 0x0001DE74 File Offset: 0x0001C074
		protected ZipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001DE80 File Offset: 0x0001C080
		public ZipException()
		{
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001DE8C File Offset: 0x0001C08C
		public ZipException(string message) : base(message)
		{
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001DE98 File Offset: 0x0001C098
		public ZipException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
