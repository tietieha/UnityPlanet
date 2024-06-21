using System;
using System.Collections.Generic;
using System.Reflection;
using UWA;
using UWASDK;
using UWAShared;

namespace UWALocal
{
	// Token: 0x02000007 RID: 7
	internal static class Utils
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003184 File Offset: 0x00001384
		public static string PluginVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000319C File Offset: 0x0000139C
		public static bool SupportType(eSdkType type)
		{
			HashSet<eSdkType> supportedType = Utils.GetSupportedType();
			return supportedType.Contains(type);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000031BC File Offset: 0x000013BC
		public static HashSet<eSdkType> GetSupportedType()
		{
			if (Utils._supportedSdkTypes == null)
			{
				Utils._supportedSdkTypes = new HashSet<eSdkType>();
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				foreach (KeyValuePair<eSdkType, string> keyValuePair in WrapperTool.StarterTypeMap)
				{
					if (keyValuePair.Value != null && executingAssembly.GetType(keyValuePair.Value) != null)
					{
						Utils._supportedSdkTypes.Add(keyValuePair.Key);
					}
				}
				if (!SharedUtils.Il2Cpp() && !Utils._supportedSdkTypes.Contains(eSdkType.PA))
				{
					Utils._supportedSdkTypes.Add(eSdkType.PA);
				}
			}
			return Utils._supportedSdkTypes;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00003284 File Offset: 0x00001484
		public static string UWALocalCoreUrl
		{
			get
			{
				return "http://" + WrapperTool.UWAServerIp + ":8081/UWALocalCore@Android.dll";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000329C File Offset: 0x0000149C
		public static string UWACoreUrl
		{
			get
			{
				return "http://" + WrapperTool.UWAServerIp + ":8081/UWACore@Android.dll";
			}
		}

		// Token: 0x04000011 RID: 17
		private static HashSet<eSdkType> _supportedSdkTypes = null;

		// Token: 0x04000012 RID: 18
		public static string OssAndroidCoreUrl = "http://uwa-public.oss-cn-beijing.aliyuncs.com/online-pa%2FAndroid%2FUWACore.dll";
	}
}
