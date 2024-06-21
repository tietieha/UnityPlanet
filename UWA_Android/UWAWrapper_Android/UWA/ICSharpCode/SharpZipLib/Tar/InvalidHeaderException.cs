using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x0200008E RID: 142
	[ComVisible(false)]
	[Serializable]
	public class InvalidHeaderException : TarException
	{
		// Token: 0x06000660 RID: 1632 RVA: 0x0002C560 File Offset: 0x0002A760
		protected InvalidHeaderException(SerializationInfo information, StreamingContext context) : base(information, context)
		{
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0002C56C File Offset: 0x0002A76C
		public InvalidHeaderException()
		{
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0002C578 File Offset: 0x0002A778
		public InvalidHeaderException(string message) : base(message)
		{
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0002C584 File Offset: 0x0002A784
		public InvalidHeaderException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
