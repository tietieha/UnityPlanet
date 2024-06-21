using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;
using UWA;
using UWASDK;

namespace UWAShared
{
	// Token: 0x0200004A RID: 74
	[Preserve]
	public static class WrapperTool
	{
		// Token: 0x06000326 RID: 806 RVA: 0x0001B968 File Offset: 0x00019B68
		public static string GetOssCoreUrl(string srcUrl)
		{
			bool flag = srcUrl == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !srcUrl.Contains("http://" + WrapperTool.UWAServerIp + ":8081/UWACore@");
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = srcUrl.Contains(".204");
					if (flag3)
					{
						result = "https://uwa-public.oss-cn-beijing.aliyuncs.com/uwa-pa%2F204%2FAndroid%2FUWACore.dll";
					}
					else
					{
						result = "https://uwa-public.oss-cn-beijing.aliyuncs.com/uwa-pa%2F202%2FAndroid%2FUWACore.dll";
					}
				}
			}
			return result;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0001B9E0 File Offset: 0x00019BE0
		public static bool CheckIpValid()
		{
			string[] array = WrapperTool.UWAServerIp.Split(new char[]
			{
				'.'
			});
			return array.Length == 4;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001BA18 File Offset: 0x00019C18
		public static bool CheckPersistFile(string file)
		{
			try
			{
				return File.Exists(Application.persistentDataPath + "/" + file);
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0001BA64 File Offset: 0x00019C64
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0001BB10 File Offset: 0x00019D10
		public static bool OnlineMode
		{
			get
			{
				bool flag = WrapperTool._onlineMode == null;
				if (flag)
				{
					WrapperTool._onlineMode = new bool?(false);
					bool flag2 = SystemInfo.deviceName.StartsWith("Uwa's");
					if (flag2)
					{
						WrapperTool._onlineMode = new bool?(true);
					}
					else
					{
						try
						{
							string path = SharedUtils.ConfigDataFolder + "/online";
							WrapperTool._onlineMode = new bool?(File.Exists(path));
						}
						catch (Exception)
						{
						}
					}
				}
				return WrapperTool._onlineMode.Value;
			}
			set
			{
				WrapperTool._onlineMode = new bool?(value);
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001BB20 File Offset: 0x00019D20
		public static eSdkType GetSdkByTestType(eSdkMode type)
		{
			eSdkType result = eSdkType.No;
			switch (type)
			{
			case eSdkMode.GOT:
			case eSdkMode.GPM:
			case eSdkMode.GOT_D:
				result = eSdkType.GOT;
				break;
			case eSdkMode.PA:
				result = eSdkType.PA;
				break;
			}
			return result;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0001BB74 File Offset: 0x00019D74
		public static WrapperTool.eDirectMode CheckGotDirectMode()
		{
			WrapperTool.eDirectMode result = WrapperTool.eDirectMode.UnSet;
			string path = Application.persistentDataPath + "/direct";
			bool flag = File.Exists(path);
			if (flag)
			{
				try
				{
					string value = File.ReadAllText(path);
					result = (WrapperTool.eDirectMode)Enum.Parse(typeof(WrapperTool.eDirectMode), value, true);
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0001BBEC File Offset: 0x00019DEC
		public static void DeleteDirectFile()
		{
			try
			{
				string path = Application.persistentDataPath + "/direct";
				bool flag = File.Exists(path);
				if (flag)
				{
					File.Delete(path);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001BC40 File Offset: 0x00019E40
		public static eSdkType CheckDirectSdkType()
		{
			string path = Application.persistentDataPath + "/direct";
			bool flag = File.Exists(path);
			eSdkType result;
			if (flag)
			{
				try
				{
					string value = File.ReadAllText(path);
					bool flag2 = string.IsNullOrEmpty(value);
					if (flag2)
					{
						return eSdkType.PA;
					}
				}
				catch (Exception)
				{
				}
				result = eSdkType.GOT;
			}
			else
			{
				result = eSdkType.No;
			}
			return result;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001BCBC File Offset: 0x00019EBC
		public static bool TryGetSavedIP(out string savedIP)
		{
			savedIP = "0.0.0.0";
			string path = SharedUtils.ConfigDataFolder + "/UWAServer.config";
			bool flag = File.Exists(path);
			if (flag)
			{
				try
				{
					string text;
					using (StreamReader streamReader = new StreamReader(path))
					{
						text = streamReader.ReadLine();
					}
					bool flag2 = !string.IsNullOrEmpty(text) && text.Split(new char[]
					{
						'.'
					}).Length == 4;
					if (flag2)
					{
						savedIP = text;
						return true;
					}
				}
				catch (Exception)
				{
				}
			}
			return false;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001BD84 File Offset: 0x00019F84
		public static string GetCorePath(eSdkType sdk)
		{
			bool flag = sdk == eSdkType.GOT;
			string result;
			if (flag)
			{
				result = SharedUtils.ConfigDataFolder + "/uwalocalcore";
			}
			else
			{
				bool flag2 = sdk == eSdkType.PA;
				if (flag2)
				{
					result = SharedUtils.ConfigDataFolder + "/uwacore";
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001BDE0 File Offset: 0x00019FE0
		public static bool SetSavedIP(string ip)
		{
			string path = SharedUtils.ConfigDataFolder + "/UWAServer.config";
			try
			{
				StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.WriteLine(ip.ToString());
				streamWriter.Close();
			}
			catch (Exception)
			{
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("Set Saved IP failed !");
				}
				return false;
			}
			return true;
		}

		// Token: 0x04000230 RID: 560
		public static string UWAServerIp = "0.0.0.0";

		// Token: 0x04000231 RID: 561
		public static string UWAServerPort = "8885";

		// Token: 0x04000232 RID: 562
		public static string LuaDll = null;

		// Token: 0x04000233 RID: 563
		public static AndroidJavaObject Activity = null;

		// Token: 0x04000234 RID: 564
		public static WrapperTool.eDirectMode DirectMode = WrapperTool.eDirectMode.UnSet;

		// Token: 0x04000235 RID: 565
		private const string ConfigFileName = "/UWAServer.config";

		// Token: 0x04000236 RID: 566
		private const string OnlineFile = "/online";

		// Token: 0x04000237 RID: 567
		private const string DirectFile = "/direct";

		// Token: 0x04000238 RID: 568
		public static bool GUIServerMode = false;

		// Token: 0x04000239 RID: 569
		private static bool? _onlineMode = null;

		// Token: 0x0400023A RID: 570
		public static Dictionary<eSdkType, string> StarterTypeMap = new Dictionary<eSdkType, string>
		{
			{
				eSdkType.PA,
				"UWAStarter"
			},
			{
				eSdkType.GOT,
				"UwaLocalStarter"
			},
			{
				eSdkType.No,
				null
			}
		};

		// Token: 0x02000107 RID: 263
		public enum eDirectMode
		{
			// Token: 0x0400068F RID: 1679
			Overview,
			// Token: 0x04000690 RID: 1680
			Mono,
			// Token: 0x04000691 RID: 1681
			Resources,
			// Token: 0x04000692 RID: 1682
			Lua,
			// Token: 0x04000693 RID: 1683
			Gpu,
			// Token: 0x04000694 RID: 1684
			UnSet
		}
	}
}
