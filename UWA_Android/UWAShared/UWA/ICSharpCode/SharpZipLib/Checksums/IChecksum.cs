using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Checksums
{
	// Token: 0x020000C9 RID: 201
	[ComVisible(false)]
	public interface IChecksum
	{
		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060008DC RID: 2268
		long Value { get; }

		// Token: 0x060008DD RID: 2269
		void Reset();

		// Token: 0x060008DE RID: 2270
		void Update(int value);

		// Token: 0x060008DF RID: 2271
		void Update(byte[] buffer);

		// Token: 0x060008E0 RID: 2272
		void Update(byte[] buffer, int offset, int count);
	}
}
