using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UWA.Core;
using UWALib.Editor.NetWork;

namespace UWA.SDK
{
	// Token: 0x020000D2 RID: 210
	[ComVisible(false)]
	public class CoreUtils
	{
		// Token: 0x06000965 RID: 2405 RVA: 0x000464F0 File Offset: 0x000446F0
		public static void RefreshOnlineConfig()
		{
			try
			{
				CoreUtils.web = new WebClient();
				CoreUtils.web.DownloadStringCompleted += CoreUtils.OnConfig;
				string text = "https://uwa-public.oss-cn-beijing.aliyuncs.com/uwa-got/version/got_online_config.json";
				bool debug = UwaWebsiteClient.Debug;
				if (debug)
				{
					text = "https://uwa-public.oss-cn-beijing.aliyuncs.com/uwa-got/version/got_online_config-test.json";
				}
				UwaWebsiteClient.Log(text);
				CoreUtils.web.DownloadStringAsync(new Uri(text));
			}
			catch (Exception ex)
			{
				string str = "RefreshOnlineConfig Exception : ";
				Exception ex2 = ex;
				UwaWebsiteClient.Log(str + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00046594 File Offset: 0x00044794
		private static void OnConfig(object sender, DownloadStringCompletedEventArgs e)
		{
			UwaWebsiteClient.Log("OnConfig");
			bool flag = e.Result != null;
			if (flag)
			{
				try
				{
					string result = e.Result;
					UwaWebsiteClient.Log(result);
					CoreUtils.jconfig = JSONParser.SimpleParse(result);
					bool flag2 = CoreUtils.jconfig.ContainsKey("freeStart");
					if (flag2)
					{
						CoreUtils.freeStart = DateTime.Parse(CoreUtils.jconfig["freeStart"].AsString(false));
					}
					bool flag3 = CoreUtils.jconfig.ContainsKey("freeEnd");
					if (flag3)
					{
						CoreUtils.freeEnd = DateTime.Parse(CoreUtils.jconfig["freeEnd"].AsString(false));
					}
					UwaWebsiteClient.Log("freeStart : " + CoreUtils.freeStart.ToString());
					UwaWebsiteClient.Log("freeEnd : " + CoreUtils.freeEnd.ToString());
				}
				catch (Exception ex)
				{
					string str = "Onconfig Exception : ";
					Exception ex2 = ex;
					UwaWebsiteClient.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x000466C8 File Offset: 0x000448C8
		public static bool CheckActivityTime()
		{
			bool flag = UwaWebsiteClient.WebSite > Localization.eWebSite.CN;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = CoreUtils._lastAsk != null && (DateTime.Now - CoreUtils._lastAsk.Value).TotalMinutes < 2.0;
				if (flag2)
				{
					result = CoreUtils._lastResult;
				}
				else
				{
					CoreUtils._lastAsk = new DateTime?(DateTime.Now);
					DateTime? beijingTime = UWALib.Editor.NetWork.UploadTool.GetBeijingTime();
					UwaWebsiteClient.Log("GetBeijingTime : " + beijingTime.ToString());
					CoreUtils._lastResult = (beijingTime != null && beijingTime > CoreUtils.freeStart && beijingTime < CoreUtils.freeEnd);
					result = CoreUtils._lastResult;
				}
			}
			return result;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x000467F8 File Offset: 0x000449F8
		public static string GetCurrentTime()
		{
			return DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00046840 File Offset: 0x00044A40
		public static byte[] GetMD5ForFile(string filePath)
		{
			bool flag = !File.Exists(filePath);
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					using (FileStream fileStream = File.OpenRead(filePath))
					{
						MD5 md = MD5.Create();
						return md.ComputeHash(fileStream);
					}
				}
				catch (Exception ex)
				{
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x000468C0 File Offset: 0x00044AC0
		public static void WriteFlagIntoBuffer(ref byte[] buffer, short flag)
		{
			Array.Clear(buffer, 0, buffer.Length);
			byte[] bytes = BitConverter.GetBytes(flag);
			Array.Copy(bytes, 0, buffer, 0, bytes.Length);
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x000468F4 File Offset: 0x00044AF4
		public static string GetFlagFromBytesBuffer(byte[] buffer, ref int startIndex)
		{
			int num = BitConverter.ToInt32(buffer, startIndex);
			startIndex += 4;
			string @string = Encoding.Unicode.GetString(buffer, startIndex, num);
			startIndex += num;
			return @string;
		}

		// Token: 0x040005B0 RID: 1456
		private static bool _lastResult = false;

		// Token: 0x040005B1 RID: 1457
		private static DateTime? _lastAsk = null;

		// Token: 0x040005B2 RID: 1458
		private static DateTime freeStart = DateTime.MaxValue;

		// Token: 0x040005B3 RID: 1459
		private static DateTime freeEnd = DateTime.MinValue;

		// Token: 0x040005B4 RID: 1460
		private static Dictionary<string, JSONValue> jconfig = new Dictionary<string, JSONValue>();

		// Token: 0x040005B5 RID: 1461
		private static WebClient web;
	}
}
