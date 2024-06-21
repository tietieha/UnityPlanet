using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Checksums
{
	// Token: 0x020000BA RID: 186
	[ComVisible(false)]
	public interface IChecksum
	{
		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000800 RID: 2048
		long Value { get; }

		// Token: 0x06000801 RID: 2049
		void Reset();

		// Token: 0x06000802 RID: 2050
		void Update(int value);

		// Token: 0x06000803 RID: 2051
		void Update(byte[] buffer);

		// Token: 0x06000804 RID: 2052
		void Update(byte[] buffer, int offset, int count);
	}
}
