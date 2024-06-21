using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A2 RID: 162
	[ComVisible(false)]
	[Serializable]
	public class TarException : SharpZipBaseException
	{
		// Token: 0x060007A6 RID: 1958 RVA: 0x0003B35C File Offset: 0x0003955C
		protected TarException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0003B368 File Offset: 0x00039568
		public TarException()
		{
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0003B374 File Offset: 0x00039574
		public TarException(string message) : base(message)
		{
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0003B380 File Offset: 0x00039580
		public TarException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
