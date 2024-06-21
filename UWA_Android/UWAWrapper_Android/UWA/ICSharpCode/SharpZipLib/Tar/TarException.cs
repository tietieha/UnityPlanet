using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000093 RID: 147
	[ComVisible(false)]
	[Serializable]
	public class TarException : SharpZipBaseException
	{
		// Token: 0x060006CA RID: 1738 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
		protected TarException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0002E5B0 File Offset: 0x0002C7B0
		public TarException()
		{
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x0002E5BC File Offset: 0x0002C7BC
		public TarException(string message) : base(message)
		{
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0002E5C8 File Offset: 0x0002C7C8
		public TarException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
