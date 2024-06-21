using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000080 RID: 128
	[ComVisible(false)]
	public class DeflaterConstants
	{
		// Token: 0x04000352 RID: 850
		public const bool DEBUGGING = false;

		// Token: 0x04000353 RID: 851
		public const int STORED_BLOCK = 0;

		// Token: 0x04000354 RID: 852
		public const int STATIC_TREES = 1;

		// Token: 0x04000355 RID: 853
		public const int DYN_TREES = 2;

		// Token: 0x04000356 RID: 854
		public const int PRESET_DICT = 32;

		// Token: 0x04000357 RID: 855
		public const int DEFAULT_MEM_LEVEL = 8;

		// Token: 0x04000358 RID: 856
		public const int MAX_MATCH = 258;

		// Token: 0x04000359 RID: 857
		public const int MIN_MATCH = 3;

		// Token: 0x0400035A RID: 858
		public const int MAX_WBITS = 15;

		// Token: 0x0400035B RID: 859
		public const int WSIZE = 32768;

		// Token: 0x0400035C RID: 860
		public const int WMASK = 32767;

		// Token: 0x0400035D RID: 861
		public const int HASH_BITS = 15;

		// Token: 0x0400035E RID: 862
		public const int HASH_SIZE = 32768;

		// Token: 0x0400035F RID: 863
		public const int HASH_MASK = 32767;

		// Token: 0x04000360 RID: 864
		public const int HASH_SHIFT = 5;

		// Token: 0x04000361 RID: 865
		public const int MIN_LOOKAHEAD = 262;

		// Token: 0x04000362 RID: 866
		public const int MAX_DIST = 32506;

		// Token: 0x04000363 RID: 867
		public const int PENDING_BUF_SIZE = 65536;

		// Token: 0x04000364 RID: 868
		public static int MAX_BLOCK_SIZE = Math.Min(65535, 65531);

		// Token: 0x04000365 RID: 869
		public const int DEFLATE_STORED = 0;

		// Token: 0x04000366 RID: 870
		public const int DEFLATE_FAST = 1;

		// Token: 0x04000367 RID: 871
		public const int DEFLATE_SLOW = 2;

		// Token: 0x04000368 RID: 872
		public static int[] GOOD_LENGTH = new int[]
		{
			0,
			4,
			4,
			4,
			4,
			8,
			8,
			8,
			32,
			32
		};

		// Token: 0x04000369 RID: 873
		public static int[] MAX_LAZY = new int[]
		{
			0,
			4,
			5,
			6,
			4,
			16,
			16,
			32,
			128,
			258
		};

		// Token: 0x0400036A RID: 874
		public static int[] NICE_LENGTH = new int[]
		{
			0,
			8,
			16,
			32,
			16,
			32,
			128,
			128,
			258,
			258
		};

		// Token: 0x0400036B RID: 875
		public static int[] MAX_CHAIN = new int[]
		{
			0,
			4,
			8,
			32,
			16,
			32,
			128,
			256,
			1024,
			4096
		};

		// Token: 0x0400036C RID: 876
		public static int[] COMPR_FUNC = new int[]
		{
			0,
			1,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			2
		};
	}
}
