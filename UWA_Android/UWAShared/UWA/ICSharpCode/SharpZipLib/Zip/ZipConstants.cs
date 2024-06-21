using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006E RID: 110
	[ComVisible(false)]
	public sealed class ZipConstants
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x000291E4 File Offset: 0x000273E4
		// (set) Token: 0x060004E6 RID: 1254 RVA: 0x00029204 File Offset: 0x00027404
		public static int DefaultCodePage
		{
			get
			{
				return ZipConstants.defaultCodePage;
			}
			set
			{
				ZipConstants.defaultCodePage = value;
			}
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00029210 File Offset: 0x00027410
		public static string ConvertToString(byte[] data, int count)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.ASCII.GetString(data, 0, count);
			}
			return result;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00029250 File Offset: 0x00027450
		public static string ConvertToString(byte[] data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.ASCII.GetString(data, 0, data.Length);
			}
			return result;
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00029290 File Offset: 0x00027490
		public static string ConvertToStringExt(int flags, byte[] data, int count)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = (flags & 2048) != 0;
				if (flag2)
				{
					result = Encoding.UTF8.GetString(data, 0, count);
				}
				else
				{
					result = ZipConstants.ConvertToString(data, count);
				}
			}
			return result;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000292F0 File Offset: 0x000274F0
		public static string ConvertToStringExt(int flags, byte[] data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = (flags & 2048) != 0;
				if (flag2)
				{
					result = Encoding.UTF8.GetString(data, 0, data.Length);
				}
				else
				{
					result = ZipConstants.ConvertToString(data, data.Length);
				}
			}
			return result;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00029354 File Offset: 0x00027554
		public static byte[] ConvertToArray(string str)
		{
			bool flag = str == null;
			byte[] result;
			if (flag)
			{
				result = new byte[0];
			}
			else
			{
				result = Encoding.ASCII.GetBytes(str);
			}
			return result;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00029390 File Offset: 0x00027590
		public static byte[] ConvertToArray(int flags, string str)
		{
			bool flag = str == null;
			byte[] result;
			if (flag)
			{
				result = new byte[0];
			}
			else
			{
				bool flag2 = (flags & 2048) != 0;
				if (flag2)
				{
					result = Encoding.UTF8.GetBytes(str);
				}
				else
				{
					result = Encoding.ASCII.GetBytes(str);
				}
			}
			return result;
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x000293F0 File Offset: 0x000275F0
		private ZipConstants()
		{
		}

		// Token: 0x040002FE RID: 766
		public const int VersionMadeBy = 51;

		// Token: 0x040002FF RID: 767
		[Obsolete("Use VersionMadeBy instead")]
		public const int VERSION_MADE_BY = 51;

		// Token: 0x04000300 RID: 768
		public const int VersionStrongEncryption = 50;

		// Token: 0x04000301 RID: 769
		[Obsolete("Use VersionStrongEncryption instead")]
		public const int VERSION_STRONG_ENCRYPTION = 50;

		// Token: 0x04000302 RID: 770
		public const int VERSION_AES = 51;

		// Token: 0x04000303 RID: 771
		public const int VersionZip64 = 45;

		// Token: 0x04000304 RID: 772
		public const int LocalHeaderBaseSize = 30;

		// Token: 0x04000305 RID: 773
		[Obsolete("Use LocalHeaderBaseSize instead")]
		public const int LOCHDR = 30;

		// Token: 0x04000306 RID: 774
		public const int Zip64DataDescriptorSize = 24;

		// Token: 0x04000307 RID: 775
		public const int DataDescriptorSize = 16;

		// Token: 0x04000308 RID: 776
		[Obsolete("Use DataDescriptorSize instead")]
		public const int EXTHDR = 16;

		// Token: 0x04000309 RID: 777
		public const int CentralHeaderBaseSize = 46;

		// Token: 0x0400030A RID: 778
		[Obsolete("Use CentralHeaderBaseSize instead")]
		public const int CENHDR = 46;

		// Token: 0x0400030B RID: 779
		public const int EndOfCentralRecordBaseSize = 22;

		// Token: 0x0400030C RID: 780
		[Obsolete("Use EndOfCentralRecordBaseSize instead")]
		public const int ENDHDR = 22;

		// Token: 0x0400030D RID: 781
		public const int CryptoHeaderSize = 12;

		// Token: 0x0400030E RID: 782
		[Obsolete("Use CryptoHeaderSize instead")]
		public const int CRYPTO_HEADER_SIZE = 12;

		// Token: 0x0400030F RID: 783
		public const int LocalHeaderSignature = 67324752;

		// Token: 0x04000310 RID: 784
		[Obsolete("Use LocalHeaderSignature instead")]
		public const int LOCSIG = 67324752;

		// Token: 0x04000311 RID: 785
		public const int SpanningSignature = 134695760;

		// Token: 0x04000312 RID: 786
		[Obsolete("Use SpanningSignature instead")]
		public const int SPANNINGSIG = 134695760;

		// Token: 0x04000313 RID: 787
		public const int SpanningTempSignature = 808471376;

		// Token: 0x04000314 RID: 788
		[Obsolete("Use SpanningTempSignature instead")]
		public const int SPANTEMPSIG = 808471376;

		// Token: 0x04000315 RID: 789
		public const int DataDescriptorSignature = 134695760;

		// Token: 0x04000316 RID: 790
		[Obsolete("Use DataDescriptorSignature instead")]
		public const int EXTSIG = 134695760;

		// Token: 0x04000317 RID: 791
		[Obsolete("Use CentralHeaderSignature instead")]
		public const int CENSIG = 33639248;

		// Token: 0x04000318 RID: 792
		public const int CentralHeaderSignature = 33639248;

		// Token: 0x04000319 RID: 793
		public const int Zip64CentralFileHeaderSignature = 101075792;

		// Token: 0x0400031A RID: 794
		[Obsolete("Use Zip64CentralFileHeaderSignature instead")]
		public const int CENSIG64 = 101075792;

		// Token: 0x0400031B RID: 795
		public const int Zip64CentralDirLocatorSignature = 117853008;

		// Token: 0x0400031C RID: 796
		public const int ArchiveExtraDataSignature = 117853008;

		// Token: 0x0400031D RID: 797
		public const int CentralHeaderDigitalSignature = 84233040;

		// Token: 0x0400031E RID: 798
		[Obsolete("Use CentralHeaderDigitalSignaure instead")]
		public const int CENDIGITALSIG = 84233040;

		// Token: 0x0400031F RID: 799
		public const int EndOfCentralDirectorySignature = 101010256;

		// Token: 0x04000320 RID: 800
		[Obsolete("Use EndOfCentralDirectorySignature instead")]
		public const int ENDSIG = 101010256;

		// Token: 0x04000321 RID: 801
		private static int defaultCodePage = Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage;
	}
}
