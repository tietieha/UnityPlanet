using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200005F RID: 95
	[ComVisible(false)]
	public sealed class ZipConstants
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0001C42C File Offset: 0x0001A62C
		// (set) Token: 0x0600040A RID: 1034 RVA: 0x0001C44C File Offset: 0x0001A64C
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

		// Token: 0x0600040B RID: 1035 RVA: 0x0001C458 File Offset: 0x0001A658
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

		// Token: 0x0600040C RID: 1036 RVA: 0x0001C498 File Offset: 0x0001A698
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

		// Token: 0x0600040D RID: 1037 RVA: 0x0001C4D8 File Offset: 0x0001A6D8
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

		// Token: 0x0600040E RID: 1038 RVA: 0x0001C538 File Offset: 0x0001A738
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

		// Token: 0x0600040F RID: 1039 RVA: 0x0001C59C File Offset: 0x0001A79C
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

		// Token: 0x06000410 RID: 1040 RVA: 0x0001C5D8 File Offset: 0x0001A7D8
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

		// Token: 0x06000411 RID: 1041 RVA: 0x0001C638 File Offset: 0x0001A838
		private ZipConstants()
		{
		}

		// Token: 0x0400028B RID: 651
		public const int VersionMadeBy = 51;

		// Token: 0x0400028C RID: 652
		[Obsolete("Use VersionMadeBy instead")]
		public const int VERSION_MADE_BY = 51;

		// Token: 0x0400028D RID: 653
		public const int VersionStrongEncryption = 50;

		// Token: 0x0400028E RID: 654
		[Obsolete("Use VersionStrongEncryption instead")]
		public const int VERSION_STRONG_ENCRYPTION = 50;

		// Token: 0x0400028F RID: 655
		public const int VERSION_AES = 51;

		// Token: 0x04000290 RID: 656
		public const int VersionZip64 = 45;

		// Token: 0x04000291 RID: 657
		public const int LocalHeaderBaseSize = 30;

		// Token: 0x04000292 RID: 658
		[Obsolete("Use LocalHeaderBaseSize instead")]
		public const int LOCHDR = 30;

		// Token: 0x04000293 RID: 659
		public const int Zip64DataDescriptorSize = 24;

		// Token: 0x04000294 RID: 660
		public const int DataDescriptorSize = 16;

		// Token: 0x04000295 RID: 661
		[Obsolete("Use DataDescriptorSize instead")]
		public const int EXTHDR = 16;

		// Token: 0x04000296 RID: 662
		public const int CentralHeaderBaseSize = 46;

		// Token: 0x04000297 RID: 663
		[Obsolete("Use CentralHeaderBaseSize instead")]
		public const int CENHDR = 46;

		// Token: 0x04000298 RID: 664
		public const int EndOfCentralRecordBaseSize = 22;

		// Token: 0x04000299 RID: 665
		[Obsolete("Use EndOfCentralRecordBaseSize instead")]
		public const int ENDHDR = 22;

		// Token: 0x0400029A RID: 666
		public const int CryptoHeaderSize = 12;

		// Token: 0x0400029B RID: 667
		[Obsolete("Use CryptoHeaderSize instead")]
		public const int CRYPTO_HEADER_SIZE = 12;

		// Token: 0x0400029C RID: 668
		public const int LocalHeaderSignature = 67324752;

		// Token: 0x0400029D RID: 669
		[Obsolete("Use LocalHeaderSignature instead")]
		public const int LOCSIG = 67324752;

		// Token: 0x0400029E RID: 670
		public const int SpanningSignature = 134695760;

		// Token: 0x0400029F RID: 671
		[Obsolete("Use SpanningSignature instead")]
		public const int SPANNINGSIG = 134695760;

		// Token: 0x040002A0 RID: 672
		public const int SpanningTempSignature = 808471376;

		// Token: 0x040002A1 RID: 673
		[Obsolete("Use SpanningTempSignature instead")]
		public const int SPANTEMPSIG = 808471376;

		// Token: 0x040002A2 RID: 674
		public const int DataDescriptorSignature = 134695760;

		// Token: 0x040002A3 RID: 675
		[Obsolete("Use DataDescriptorSignature instead")]
		public const int EXTSIG = 134695760;

		// Token: 0x040002A4 RID: 676
		[Obsolete("Use CentralHeaderSignature instead")]
		public const int CENSIG = 33639248;

		// Token: 0x040002A5 RID: 677
		public const int CentralHeaderSignature = 33639248;

		// Token: 0x040002A6 RID: 678
		public const int Zip64CentralFileHeaderSignature = 101075792;

		// Token: 0x040002A7 RID: 679
		[Obsolete("Use Zip64CentralFileHeaderSignature instead")]
		public const int CENSIG64 = 101075792;

		// Token: 0x040002A8 RID: 680
		public const int Zip64CentralDirLocatorSignature = 117853008;

		// Token: 0x040002A9 RID: 681
		public const int ArchiveExtraDataSignature = 117853008;

		// Token: 0x040002AA RID: 682
		public const int CentralHeaderDigitalSignature = 84233040;

		// Token: 0x040002AB RID: 683
		[Obsolete("Use CentralHeaderDigitalSignaure instead")]
		public const int CENDIGITALSIG = 84233040;

		// Token: 0x040002AC RID: 684
		public const int EndOfCentralDirectorySignature = 101010256;

		// Token: 0x040002AD RID: 685
		[Obsolete("Use EndOfCentralDirectorySignature instead")]
		public const int ENDSIG = 101010256;

		// Token: 0x040002AE RID: 686
		private static int defaultCodePage = Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage;
	}
}
