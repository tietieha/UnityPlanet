using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UWA
{
	// Token: 0x02000051 RID: 81
	internal class CoreUtils
	{
		// Token: 0x0600036A RID: 874 RVA: 0x0001756C File Offset: 0x0001576C
		public static void DoSomeThingOnGUI()
		{
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00017570 File Offset: 0x00015770
		public static bool IsURP()
		{
			bool flag = CoreUtils._isURP != null;
			bool value;
			if (flag)
			{
				value = CoreUtils._isURP.Value;
			}
			else
			{
				CoreUtils._isURP = new bool?(false);
				Type type = typeof(GameObject).Assembly.GetType("UnityEngine.Rendering.RenderPipelineManager");
				bool flag2 = type != null;
				if (flag2)
				{
					PropertyInfo property = type.GetProperty("currentPipeline");
					bool flag3 = property != null && property.GetValue(null, null) != null;
					if (flag3)
					{
						CoreUtils._isURP = new bool?(true);
					}
				}
				value = CoreUtils._isURP.Value;
			}
			return value;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00017624 File Offset: 0x00015824
		private static Type FindType(Assembly a, string fullName)
		{
			Type[] types = a.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				bool flag = types[i].FullName == fullName;
				if (flag)
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00017688 File Offset: 0x00015888
		public static Type GetWrapperType(string Typename)
		{
			Assembly assembly = typeof(IUWAAPI).Assembly;
			Type type = CoreUtils.FindType(assembly, Typename);
			bool flag = type != null;
			Type result;
			if (flag)
			{
				result = type;
			}
			else
			{
				type = CoreUtils.FindType(assembly, "UWA." + Typename);
				bool flag2 = type != null;
				if (flag2)
				{
					result = type;
				}
				else
				{
					type = CoreUtils.FindType(assembly, "UWA.IOS." + Typename);
					bool flag3 = type != null;
					if (flag3)
					{
						result = type;
					}
					else
					{
						type = CoreUtils.FindType(assembly, "UWA.Android." + Typename);
						bool flag4 = type != null;
						if (flag4)
						{
							result = type;
						}
						else
						{
							type = CoreUtils.FindType(assembly, "UWA.Windows." + Typename);
							bool flag5 = type != null;
							if (flag5)
							{
								result = type;
							}
							else
							{
								result = null;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00017764 File Offset: 0x00015964
		public static bool NoProfilerCallback()
		{
			return Application.unityVersion.StartsWith("5.") || Application.unityVersion.StartsWith("2017.") || Application.unityVersion.StartsWith("2018.1");
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600036F RID: 879 RVA: 0x000177BC File Offset: 0x000159BC
		// (set) Token: 0x06000370 RID: 880 RVA: 0x000177C4 File Offset: 0x000159C4
		public static int LocalFileIndex { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000371 RID: 881 RVA: 0x000177CC File Offset: 0x000159CC
		// (set) Token: 0x06000372 RID: 882 RVA: 0x000177D4 File Offset: 0x000159D4
		public static string CurrentLevelName { get; set; }

		// Token: 0x06000373 RID: 883 RVA: 0x000177DC File Offset: 0x000159DC
		public static bool IsAndroid4()
		{
			return SystemInfo.operatingSystem.Replace(" ", "").Contains("Android4") || SystemInfo.operatingSystem.Replace(" ", "").Contains("AndroidOS4");
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001783C File Offset: 0x00015A3C
		public static string GetCurrentTime()
		{
			return DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00017884 File Offset: 0x00015A84
		public static string GetDataFileName()
		{
			string result = string.Format("{0}_{1}_{2}", SharedUtils.frameId, CoreUtils.CurrentLevelName, CoreUtils.LocalFileIndex);
			CoreUtils.LocalFileIndex++;
			return result;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000178D0 File Offset: 0x00015AD0
		public static string GetLogFileFullPath(string fileName)
		{
			return SharedUtils.FinalDataPath + "/" + fileName;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000178FC File Offset: 0x00015AFC
		public static string GetFrameIdWithExtFilePath(string extension)
		{
			return string.Format("{0}/{1}{2}", SharedUtils.FinalDataPath, SharedUtils.frameId, extension);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00017930 File Offset: 0x00015B30
		public static string GetfixedWithExtFilePath(string extension)
		{
			return string.Format("{0}/{1}{2}", SharedUtils.FinalDataPath, 0, extension);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00017960 File Offset: 0x00015B60
		public static string GetLicense(HashSet<string> nameSpaces)
		{
			bool flag = nameSpaces == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CODEv1.0.1;");
				foreach (string value in nameSpaces)
				{
					bool flag2 = !string.IsNullOrEmpty(value);
					if (flag2)
					{
						stringBuilder.Append(value).Append(';');
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00017A0C File Offset: 0x00015C0C
		public static HashSet<string> ExtractDllClass()
		{
			HashSet<string> hashSet = new HashSet<string>();
			try
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					Type[] types = assembly.GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						Type type = types[j];
						string ns = type.Namespace;
						bool flag = string.IsNullOrEmpty(ns);
						if (!flag)
						{
							string[] array2 = ns.Split(new char[]
							{
								'.'
							});
							bool flag2 = array2.Length >= 2;
							if (flag2)
							{
								ns = array2[0] + "." + array2[1];
							}
							bool flag3 = !string.IsNullOrEmpty(ns) && !CoreUtils.classFilter.Any((string x) => ns.StartsWith(x, StringComparison.Ordinal));
							if (flag3)
							{
								bool flag4 = !hashSet.Contains(ns);
								if (flag4)
								{
									hashSet.Add(ns);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				string str = "Exception reading dll : ";
				Exception ex2 = ex;
				Debug.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				return new HashSet<string>();
			}
			return hashSet;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00017BC0 File Offset: 0x00015DC0
		public static void MakeOnlineDevice()
		{
			string path = SharedUtils.ConfigDataFolder + "/online";
			bool flag = !File.Exists(path);
			if (flag)
			{
				File.WriteAllText(path, "");
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00017C00 File Offset: 0x00015E00
		public static string GetRuntimeConfigPath()
		{
			return SharedUtils.ConfigDataFolder + "/uwaconfig";
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00017C28 File Offset: 0x00015E28
		public static string GetRuntimeBundlePath(string version)
		{
			return SharedUtils.ConfigDataFolder + "/uwabundle_" + version;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00017C54 File Offset: 0x00015E54
		public static string GetConfigPath(string fileName)
		{
			return SharedUtils.ConfigDataFolder + "/" + fileName;
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600037F RID: 895 RVA: 0x00017C80 File Offset: 0x00015E80
		public static string AppName
		{
			get
			{
				bool flag = string.IsNullOrEmpty(CoreUtils._appName);
				if (flag)
				{
					object[] array = new object[]
					{
						SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0])
					};
					CoreUtils._appName = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationInfo", new object[0]).Call<string>("loadLabel", array);
				}
				return CoreUtils._appName;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000380 RID: 896 RVA: 0x00017CF8 File Offset: 0x00015EF8
		public static string SocName
		{
			get
			{
				bool flag = CoreUtils._socName == null;
				if (flag)
				{
					CoreUtils._socName = "";
					string path = "/proc/cpuinfo";
					try
					{
						bool flag2 = !File.Exists(path);
						if (flag2)
						{
							return CoreUtils._socName;
						}
						string[] array = File.ReadAllLines(path);
						bool flag3 = array.Length == 0;
						if (flag3)
						{
							return CoreUtils._socName;
						}
						string text = array[array.Length - 1];
						bool flag4 = string.IsNullOrEmpty(text);
						if (flag4)
						{
							return CoreUtils._socName;
						}
						bool flag5 = text.Contains("Hardware");
						if (flag5)
						{
							string[] array2 = text.Split(new char[]
							{
								':'
							});
							bool flag6 = array2.Length == 2;
							if (flag6)
							{
								CoreUtils._socName = array2[1].Replace(" ", "");
							}
						}
					}
					catch (Exception ex)
					{
					}
				}
				return CoreUtils._socName;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00017E14 File Offset: 0x00016014
		public static int ProcessorCount
		{
			get
			{
				bool flag = CoreUtils._processorCount == -2;
				if (flag)
				{
					CoreUtils._processorCount = -1;
					PropertyInfo property = typeof(SystemInfo).GetProperty("processorCount");
					bool flag2 = property != null;
					if (flag2)
					{
						CoreUtils._processorCount = (int)property.GetValue(null, null);
						return CoreUtils._processorCount;
					}
					string path = "/sys/devices/system/cpu/possible";
					bool flag3 = !File.Exists(path);
					if (flag3)
					{
						return CoreUtils._processorCount;
					}
					string text = File.ReadAllText(path);
					bool flag4 = string.IsNullOrEmpty(text);
					if (flag4)
					{
						return CoreUtils._processorCount;
					}
					string[] array = text.Split(new char[]
					{
						'-'
					});
					int num = 0;
					bool flag5 = array.Length < 2 || !int.TryParse(array[1], out num);
					if (flag5)
					{
						return CoreUtils._processorCount;
					}
					CoreUtils._processorCount = num + 1;
				}
				return CoreUtils._processorCount;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00017F28 File Offset: 0x00016128
		public static int ProcessorFrequency
		{
			get
			{
				bool flag = CoreUtils._processorFrequency == -2;
				if (flag)
				{
					CoreUtils._processorFrequency = -1;
					PropertyInfo property = typeof(SystemInfo).GetProperty("processorFrequency");
					bool flag2 = property != null;
					if (flag2)
					{
						CoreUtils._processorFrequency = (int)property.GetValue(null, null) * 1000;
						return CoreUtils._processorFrequency;
					}
					for (int i = 0; i < CoreUtils.ProcessorCount; i++)
					{
						bool flag3 = !CoreUtils._cpuNotAccess;
						if (flag3)
						{
							string path = "/sys/devices/system/cpu/cpu" + i.ToString() + "/cpufreq/cpuinfo_max_freq";
							try
							{
								bool flag4 = File.Exists(path);
								if (flag4)
								{
									string s = File.ReadAllText(path);
									int val;
									bool flag5 = int.TryParse(s, out val);
									if (flag5)
									{
										CoreUtils._processorFrequency = Math.Max(CoreUtils._processorFrequency, val);
									}
								}
							}
							catch (Exception)
							{
								Debug.Log("cpu_freq can not be accessed.");
								CoreUtils._cpuNotAccess = true;
							}
						}
					}
				}
				return CoreUtils._processorFrequency;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000383 RID: 899 RVA: 0x0001805C File Offset: 0x0001625C
		public static string PkgName
		{
			get
			{
				bool flag = string.IsNullOrEmpty(CoreUtils._pkgName);
				if (flag)
				{
					CoreUtils._pkgName = SharedUtils.CurrentActivity.Call<string>("getPackageName", new object[0]);
				}
				return CoreUtils._pkgName;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000384 RID: 900 RVA: 0x000180A8 File Offset: 0x000162A8
		public static string BundleVersionName
		{
			get
			{
				bool flag = string.IsNullOrEmpty(CoreUtils._bundleVersionName);
				if (flag)
				{
					AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0]);
					AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[]
					{
						CoreUtils.PkgName,
						0
					});
					CoreUtils._bundleVersionName = androidJavaObject2.Get<string>("versionName");
				}
				return CoreUtils._bundleVersionName;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000385 RID: 901 RVA: 0x00018124 File Offset: 0x00016324
		public static int BundleVersionCode
		{
			get
			{
				bool flag = CoreUtils._bundleVersionCode == -1;
				if (flag)
				{
					AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0]);
					AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[]
					{
						CoreUtils.PkgName,
						0
					});
					CoreUtils._bundleVersionCode = androidJavaObject2.Get<int>("versionCode");
				}
				return CoreUtils._bundleVersionCode;
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0001819C File Offset: 0x0001639C
		public static IPAddress GetSelfIp()
		{
			IPAddress result = null;
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			foreach (IPAddress ipaddress in hostAddresses)
			{
				bool flag = ipaddress.AddressFamily == AddressFamily.InterNetwork;
				if (flag)
				{
					result = ipaddress;
				}
			}
			return result;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000181FC File Offset: 0x000163FC
		public static string FindScriptBackendDll()
		{
			bool flag = !SharedUtils.Il2Cpp();
			if (flag)
			{
				string nativeDllPath = CoreUtils.GetNativeDllPath("libmonobdwgc-2.0");
				bool flag2 = File.Exists(nativeDllPath);
				if (flag2)
				{
					return nativeDllPath;
				}
				nativeDllPath = CoreUtils.GetNativeDllPath("libmono");
				bool flag3 = File.Exists(nativeDllPath);
				if (flag3)
				{
					return nativeDllPath;
				}
			}
			else
			{
				string nativeDllPath2 = CoreUtils.GetNativeDllPath("libil2cpp");
				bool flag4 = File.Exists(nativeDllPath2);
				if (flag4)
				{
					return nativeDllPath2;
				}
			}
			return "";
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00018298 File Offset: 0x00016498
		public static void UWASendLogToServer(string log)
		{
			Thread thread = new Thread(new ParameterizedThreadStart(CoreUtils.UWASendLogToServerInThread));
			thread.Start(log);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x000182C4 File Offset: 0x000164C4
		private static void UWASendLogToServerInThread(object info)
		{
			bool flag = CoreUtils.LogEnabled != null && !CoreUtils.LogEnabled.Value;
			if (!flag)
			{
				Socket socket = null;
				IPAddress address = null;
				try
				{
					address = IPAddress.Parse(UWACoreConfig.LOG_IP);
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					CoreUtils.LogEnabled = new bool?(true);
				}
				catch (Exception)
				{
					CoreUtils.LogEnabled = new bool?(false);
				}
				try
				{
					socket.Connect(new IPEndPoint(address, 8083));
				}
				catch
				{
					return;
				}
				try
				{
					Thread.Sleep(500);
					socket.Send(Encoding.ASCII.GetBytes((info as string) + "$"));
				}
				catch
				{
				}
				Thread.Sleep(500);
				bool connected = socket.Connected;
				if (connected)
				{
					socket.Close();
				}
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x000183E4 File Offset: 0x000165E4
		public static bool WordsIScn(string words)
		{
			return false;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00018400 File Offset: 0x00016600
		public static string GetNativeDllPath(string soName)
		{
			string text = string.Concat(new string[]
			{
				"/data/data/",
				CoreUtils.PkgName,
				"/lib/",
				soName,
				".so"
			});
			bool flag = File.Exists(text);
			string result;
			if (flag)
			{
				result = text;
			}
			else
			{
				text = CoreUtils.GetNativeLibraryDir() + "/" + soName + ".so";
				bool flag2 = File.Exists(text);
				if (flag2)
				{
					result = text;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00018488 File Offset: 0x00016688
		public static string GetNativeLibraryDir()
		{
			bool flag = CoreUtils._nativeLibraryDir != null;
			string nativeLibraryDir;
			if (flag)
			{
				nativeLibraryDir = CoreUtils._nativeLibraryDir;
			}
			else
			{
				object[] array = new object[]
				{
					SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0])
				};
				CoreUtils._nativeLibraryDir = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationInfo", new object[0]).Get<string>("nativeLibraryDir");
				nativeLibraryDir = CoreUtils._nativeLibraryDir;
			}
			return nativeLibraryDir;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00018504 File Offset: 0x00016704
		public static void SetLogOrStackTrace(bool enable)
		{
			bool flag = !CoreUtils._tryGetLogField;
			if (flag)
			{
				CoreUtils._tryGetLogField = true;
				CoreUtils._loggerProp = typeof(Debug).GetProperty("unityLogger", BindingFlags.Static | BindingFlags.Public);
				object obj = null;
				bool flag2 = CoreUtils._loggerProp != null;
				if (flag2)
				{
					obj = CoreUtils._loggerProp.GetValue(null, null);
				}
				bool flag3 = obj != null;
				if (flag3)
				{
					CoreUtils._logEnabledProp = obj.GetType().GetProperty("logEnabled", BindingFlags.Instance | BindingFlags.Public);
				}
			}
			bool flag4 = CoreUtils._loggerProp != null && CoreUtils._logEnabledProp != null;
			if (flag4)
			{
				object value = CoreUtils._loggerProp.GetValue(null, null);
				CoreUtils._oldLogEnable = CoreUtils._logEnabledProp.GetValue(value, null);
				CoreUtils._logEnabledProp.SetValue(value, enable, null);
			}
		}

		// Token: 0x0600038E RID: 910 RVA: 0x000185E0 File Offset: 0x000167E0
		public static void RevertStackTrace()
		{
			bool flag = CoreUtils._loggerProp != null && CoreUtils._logEnabledProp != null && CoreUtils._oldLogEnable != null;
			if (flag)
			{
				object value = CoreUtils._loggerProp.GetValue(null, null);
				CoreUtils._logEnabledProp.SetValue(value, CoreUtils._oldLogEnable, null);
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00018640 File Offset: 0x00016840
		public static int EncodeInstanceID(int instanceId)
		{
			bool flag = instanceId >= 0;
			int result;
			if (flag)
			{
				bool flag2 = instanceId >= 8388608;
				if (flag2)
				{
					result = int.MaxValue;
				}
				else
				{
					result = instanceId;
				}
			}
			else
			{
				bool flag3 = instanceId <= -8388608;
				if (flag3)
				{
					result = int.MaxValue;
				}
				else
				{
					result = -instanceId + 8388608;
				}
			}
			return result;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x000186B4 File Offset: 0x000168B4
		public static Color IndexToColor2(int index)
		{
			bool flag = index <= 0;
			Color result;
			if (flag)
			{
				result = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				int num = index % 40 * 6;
				int num2 = index / 40 * 6;
				result = new Color((float)num, (float)num2, 0f, 0f);
			}
			return result;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001871C File Offset: 0x0001691C
		public static Color IndexToColor(int index)
		{
			bool flag = index <= 0;
			Color result;
			if (flag)
			{
				result = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				index *= 6;
				int num = 256;
				int num2 = 65536;
				int num3 = num2 * 256;
				bool flag2 = index < num;
				if (flag2)
				{
					result = new Color((float)index, 0f, 0f, 0f);
				}
				else
				{
					bool flag3 = index < num2;
					if (flag3)
					{
						int num4 = index / num;
						int num5 = index % num;
						result = new Color((float)num5, (float)num4, 0f, 0f);
					}
					else
					{
						bool flag4 = index < num3;
						if (flag4)
						{
							int num6 = index / num2;
							int num7 = index % num2;
							int num8 = num7 / num;
							int num9 = num7 % num;
							result = new Color((float)num9, (float)num8, (float)num6, 0f);
						}
						else
						{
							result = new Color(255f, 255f, 255f, 0f);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00018834 File Offset: 0x00016A34
		public static string FormatSize(long size)
		{
			string text = null;
			bool flag = size >= 1024L;
			float num;
			if (flag)
			{
				text = "KB";
				num = (float)(size / 1024L);
				bool flag2 = num >= 1024f;
				if (flag2)
				{
					text = "MB";
					num /= 1024f;
				}
				bool flag3 = num >= 1024f;
				if (flag3)
				{
					text = "GB";
					num /= 1024f;
				}
			}
			else
			{
				num = (float)size;
			}
			StringBuilder stringBuilder = new StringBuilder(num.ToString("F"));
			bool flag4 = text != null;
			if (flag4)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000393 RID: 915 RVA: 0x000188FC File Offset: 0x00016AFC
		public static string StringReplace(string orc)
		{
			return orc.Replace("\n", "^").Replace("\r", "^").Replace(",", "’");
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00018944 File Offset: 0x00016B44
		public static void CopyStream(Stream input, Stream output)
		{
			byte[] array = new byte[8192];
			int count;
			while ((count = input.Read(array, 0, array.Length)) > 0)
			{
				output.Write(array, 0, count);
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00018988 File Offset: 0x00016B88
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
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00018A08 File Offset: 0x00016C08
		public static void WriteFlagIntoBuffer(ref byte[] buffer, short flag)
		{
			Array.Clear(buffer, 0, buffer.Length);
			byte[] bytes = BitConverter.GetBytes(flag);
			Array.Copy(bytes, 0, buffer, 0, bytes.Length);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00018A3C File Offset: 0x00016C3C
		public static string GetFlagFromBytesBuffer(byte[] buffer, ref int startIndex)
		{
			int num = BitConverter.ToInt32(buffer, startIndex);
			startIndex += 4;
			string @string = Encoding.Unicode.GetString(buffer, startIndex, num);
			startIndex += num;
			return @string;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00018A7C File Offset: 0x00016C7C
		public static void WriteEndingFlag(bool end = false)
		{
			string text = SharedUtils.FinalDataPath + "/done";
			string text2 = SharedUtils.FinalDataPath + "/last";
			string path = end ? text : text2;
			bool flag = File.Exists(text);
			if (flag)
			{
				File.Delete(text);
			}
			bool flag2 = File.Exists(text2);
			if (flag2)
			{
				File.Delete(text2);
			}
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.WriteLine(SharedUtils.frameId.ToString());
			streamWriter.Close();
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00018B08 File Offset: 0x00016D08
		public static void WriteLastTime(long duration)
		{
			string path = SharedUtils.FinalDataPath + "/lasttime";
			StreamWriter streamWriter = new StreamWriter(File.Create(path));
			streamWriter.Write(duration.ToString() + "/1000");
			streamWriter.Close();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00018B58 File Offset: 0x00016D58
		public static void WriteStringToFile(string content, string path)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.Write(content);
			streamWriter.Close();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00018B80 File Offset: 0x00016D80
		public static string[] ReadAllLinesFromFile(string path)
		{
			List<string> list = new List<string>();
			using (StreamReader streamReader = new StreamReader(path))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00018BEC File Offset: 0x00016DEC
		public static void WriteAllLinesToFile(string path, string[] lines)
		{
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				for (int i = 0; i < lines.Length; i++)
				{
					streamWriter.WriteLine(lines[i]);
				}
			}
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00018C4C File Offset: 0x00016E4C
		public static string Cn2En(string cnStr)
		{
			string text = Uri.EscapeUriString(cnStr).Replace("%", "");
			bool flag = text.Length >= 30;
			if (flag)
			{
				text = text.Substring(0, 30);
			}
			return text;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00018C9C File Offset: 0x00016E9C
		[Conditional("PROFILE_SDK")]
		public static void ProfileSdkBegin(string name)
		{
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00018CA0 File Offset: 0x00016EA0
		[Conditional("PROFILE_SDK")]
		public static void ProfileSdkEnd()
		{
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00018CA4 File Offset: 0x00016EA4
		public static void Reset()
		{
		}

		// Token: 0x040001FD RID: 509
		public const string DllName = "uwa";

		// Token: 0x040001FE RID: 510
		private static bool? _isURP = null;

		// Token: 0x04000201 RID: 513
		private static List<string> classFilter = new List<string>
		{
			"System.",
			"UnityEngine.",
			"Microsoft.",
			"Mono.",
			"Unity.",
			"UWA"
		};

		// Token: 0x04000202 RID: 514
		private static string _appName;

		// Token: 0x04000203 RID: 515
		private static string _socName = null;

		// Token: 0x04000204 RID: 516
		private static int _processorCount = -2;

		// Token: 0x04000205 RID: 517
		private static bool _cpuNotAccess = false;

		// Token: 0x04000206 RID: 518
		private static int _processorFrequency = -2;

		// Token: 0x04000207 RID: 519
		private static string _pkgName;

		// Token: 0x04000208 RID: 520
		private static string _bundleVersionName;

		// Token: 0x04000209 RID: 521
		private static int _bundleVersionCode = -1;

		// Token: 0x0400020A RID: 522
		private static string[] preCheckMono = new string[]
		{
			"/Mono/mono.dll",
			"/Mono/EmbedRuntime/mono.dll",
			"/MonoBleedingEdge/EmbedRuntime/mono-2.0-bdwgc.dll"
		};

		// Token: 0x0400020B RID: 523
		private static string[] preCheckIL2CPP = new string[]
		{
			"/GameAssembly.dll"
		};

		// Token: 0x0400020C RID: 524
		private static bool? LogEnabled = null;

		// Token: 0x0400020D RID: 525
		private static string _nativeLibraryDir = null;

		// Token: 0x0400020E RID: 526
		private static object _oldStackEnable = null;

		// Token: 0x0400020F RID: 527
		private static object _oldLogEnable = null;

		// Token: 0x04000210 RID: 528
		private static bool _tryGetStackTraceMethod = false;

		// Token: 0x04000211 RID: 529
		private static bool _tryGetLogField = false;

		// Token: 0x04000212 RID: 530
		private static PropertyInfo _loggerProp = null;

		// Token: 0x04000213 RID: 531
		private static PropertyInfo _logEnabledProp = null;

		// Token: 0x04000214 RID: 532
		private const int signNum = 8388608;
	}
}
