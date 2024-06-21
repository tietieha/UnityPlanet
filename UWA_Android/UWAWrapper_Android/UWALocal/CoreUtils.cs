using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UWA;

namespace UWALocal
{
	// Token: 0x02000017 RID: 23
	internal class CoreUtils
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00007438 File Offset: 0x00005638
		public static void DoSomeThingOnGUI()
		{
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000743C File Offset: 0x0000563C
		public static bool IsURP()
		{
			if (CoreUtils._isURP != null)
			{
				return CoreUtils._isURP.Value;
			}
			CoreUtils._isURP = new bool?(false);
			Type type = typeof(GameObject).Assembly.GetType("UnityEngine.Rendering.RenderPipelineManager");
			if (type != null)
			{
				PropertyInfo property = type.GetProperty("currentPipeline");
				if (property != null && property.GetValue(null, null) != null)
				{
					CoreUtils._isURP = new bool?(true);
				}
			}
			return CoreUtils._isURP.Value;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000074C8 File Offset: 0x000056C8
		private static Type FindType(Assembly a, string fullName)
		{
			Type[] types = a.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].FullName == fullName)
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00007514 File Offset: 0x00005714
		public static Type GetWrapperType(string Typename)
		{
			Assembly assembly = typeof(IUWAAPI).Assembly;
			Type type = CoreUtils.FindType(assembly, Typename);
			if (type != null)
			{
				return type;
			}
			type = CoreUtils.FindType(assembly, "UWA." + Typename);
			if (type != null)
			{
				return type;
			}
			type = CoreUtils.FindType(assembly, "UWA.IOS." + Typename);
			if (type != null)
			{
				return type;
			}
			type = CoreUtils.FindType(assembly, "UWA.Android." + Typename);
			if (type != null)
			{
				return type;
			}
			type = CoreUtils.FindType(assembly, "UWA.Windows." + Typename);
			if (type != null)
			{
				return type;
			}
			return null;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000075B0 File Offset: 0x000057B0
		public static bool NoProfilerCallback()
		{
			return Application.unityVersion.StartsWith("5.") || Application.unityVersion.StartsWith("2017.") || Application.unityVersion.StartsWith("2018.1");
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000075EC File Offset: 0x000057EC
		public static bool NoIl2cppStackTraceCallback()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("il2cpp_stacktrace");
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00007608 File Offset: 0x00005808
		public static bool NoMonoObjectLiveCheck()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("mono_object_alive_check");
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007624 File Offset: 0x00005824
		public static bool NoGPUCounterDeviceSupport()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("gpu_counter_device_support");
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00007640 File Offset: 0x00005840
		public static bool NoGPUCounterRuntimeSupport()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("gpu_counter_runtime_support");
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000765C File Offset: 0x0000585C
		public static bool NoScreenshotSupport()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("android_screenshot");
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007678 File Offset: 0x00005878
		public static string GetFeatureInfo(string f)
		{
			if (UwaLocalState.uwaNotFound)
			{
				return "";
			}
			IntPtr featureInfo = UwaProfiler.GetFeatureInfo(f);
			string result = "";
			if (featureInfo != IntPtr.Zero)
			{
				result = Marshal.PtrToStringAnsi(featureInfo);
			}
			return result;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000076C0 File Offset: 0x000058C0
		public static bool NoSocInfoSupport()
		{
			return UwaLocalState.uwaNotFound || !UwaProfiler.IsFeatureSupport("soc_info_support");
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600011C RID: 284 RVA: 0x000076DC File Offset: 0x000058DC
		// (set) Token: 0x0600011D RID: 285 RVA: 0x000076E4 File Offset: 0x000058E4
		public static int LocalFileIndex { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600011E RID: 286 RVA: 0x000076EC File Offset: 0x000058EC
		// (set) Token: 0x0600011F RID: 287 RVA: 0x000076F4 File Offset: 0x000058F4
		public static string CurrentLevelName { get; set; }

		// Token: 0x06000120 RID: 288 RVA: 0x000076FC File Offset: 0x000058FC
		public static bool IsAndroid4()
		{
			return SystemInfo.operatingSystem.Replace(" ", "").Contains("Android4") || SystemInfo.operatingSystem.Replace(" ", "").Contains("AndroidOS4");
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007750 File Offset: 0x00005950
		public static string GetCurrentTime()
		{
			return DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007790 File Offset: 0x00005990
		public static string GetDataFileName()
		{
			string result = string.Format("{0}_{1}_{2}", SharedUtils.frameId, CoreUtils.CurrentLevelName, CoreUtils.LocalFileIndex);
			CoreUtils.LocalFileIndex++;
			return result;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000077D4 File Offset: 0x000059D4
		public static string GetLogFileFullPath(string fileName)
		{
			return SharedUtils.FinalDataPath + "/" + fileName;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000077E8 File Offset: 0x000059E8
		public static string GetFrameIdWithExtFilePath(string extension)
		{
			return string.Format("{0}/{1}{2}", SharedUtils.FinalDataPath, SharedUtils.frameId, extension);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007804 File Offset: 0x00005A04
		public static string GetfixedWithExtFilePath(string extension)
		{
			return string.Format("{0}/{1}{2}", SharedUtils.FinalDataPath, 0, extension);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000781C File Offset: 0x00005A1C
		public static string GetLicense(HashSet<string> nameSpaces)
		{
			if (nameSpaces == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CODEv1.0.1;");
			foreach (string value in nameSpaces)
			{
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.Append(value).Append(';');
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000078A4 File Offset: 0x00005AA4
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
						if (!string.IsNullOrEmpty(ns))
						{
							string[] array2 = ns.Split(new char[]
							{
								'.'
							});
							if (array2.Length >= 2)
							{
								ns = array2[0] + "." + array2[1];
							}
							if (!string.IsNullOrEmpty(ns) && !CoreUtils.classFilter.Any((string x) => ns.StartsWith(x, StringComparison.Ordinal)) && !hashSet.Contains(ns))
							{
								hashSet.Add(ns);
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

		// Token: 0x06000128 RID: 296 RVA: 0x00007A1C File Offset: 0x00005C1C
		public static void MakeOnlineDevice()
		{
			string path = SharedUtils.ConfigDataFolder + "/online";
			if (!File.Exists(path))
			{
				File.WriteAllText(path, "");
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00007A54 File Offset: 0x00005C54
		public static string GetRuntimeConfigPath()
		{
			return SharedUtils.ConfigDataFolder + "/uwaconfig";
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00007A68 File Offset: 0x00005C68
		public static string GetRuntimeBundlePath(string version)
		{
			return SharedUtils.ConfigDataFolder + "/uwabundle_" + version;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007A7C File Offset: 0x00005C7C
		public static string GetConfigPath(string fileName)
		{
			return SharedUtils.ConfigDataFolder + "/" + fileName;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00007A90 File Offset: 0x00005C90
		public static string AppName
		{
			get
			{
				if (string.IsNullOrEmpty(CoreUtils._appName))
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00007AFC File Offset: 0x00005CFC
		public static string SocName
		{
			get
			{
				if (CoreUtils._socName == null)
				{
					CoreUtils._socName = "";
					string path = "/proc/cpuinfo";
					try
					{
						if (!File.Exists(path))
						{
							return CoreUtils._socName;
						}
						string[] array = File.ReadAllLines(path);
						if (array.Length == 0)
						{
							return CoreUtils._socName;
						}
						string text = array[array.Length - 1];
						if (string.IsNullOrEmpty(text))
						{
							return CoreUtils._socName;
						}
						if (text.Contains("Hardware"))
						{
							string[] array2 = text.Split(new char[]
							{
								':'
							});
							if (array2.Length == 2)
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00007BE0 File Offset: 0x00005DE0
		public static int ProcessorCount
		{
			get
			{
				if (CoreUtils._processorCount == -2)
				{
					CoreUtils._processorCount = -1;
					PropertyInfo property = typeof(SystemInfo).GetProperty("processorCount");
					if (property != null)
					{
						CoreUtils._processorCount = (int)property.GetValue(null, null);
						return CoreUtils._processorCount;
					}
					string path = "/sys/devices/system/cpu/possible";
					if (!File.Exists(path))
					{
						return CoreUtils._processorCount;
					}
					string text = File.ReadAllText(path);
					if (string.IsNullOrEmpty(text))
					{
						return CoreUtils._processorCount;
					}
					string[] array = text.Split(new char[]
					{
						'-'
					});
					int num = 0;
					if (array.Length < 2 || !int.TryParse(array[1], out num))
					{
						return CoreUtils._processorCount;
					}
					CoreUtils._processorCount = num + 1;
				}
				return CoreUtils._processorCount;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00007CAC File Offset: 0x00005EAC
		public static int ProcessorFrequency
		{
			get
			{
				if (CoreUtils._processorFrequency == -2)
				{
					CoreUtils._processorFrequency = -1;
					PropertyInfo property = typeof(SystemInfo).GetProperty("processorFrequency");
					if (property != null)
					{
						CoreUtils._processorFrequency = (int)property.GetValue(null, null) * 1000;
						return CoreUtils._processorFrequency;
					}
					for (int i = 0; i < CoreUtils.ProcessorCount; i++)
					{
						if (!CoreUtils._cpuNotAccess)
						{
							string path = "/sys/devices/system/cpu/cpu" + i.ToString() + "/cpufreq/cpuinfo_max_freq";
							try
							{
								if (File.Exists(path))
								{
									string s = File.ReadAllText(path);
									int val;
									if (int.TryParse(s, out val))
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00007D98 File Offset: 0x00005F98
		public static string PkgName
		{
			get
			{
				if (string.IsNullOrEmpty(CoreUtils._pkgName))
				{
					CoreUtils._pkgName = SharedUtils.CurrentActivity.Call<string>("getPackageName", new object[0]);
				}
				return CoreUtils._pkgName;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00007DC8 File Offset: 0x00005FC8
		public static string BundleVersionName
		{
			get
			{
				if (string.IsNullOrEmpty(CoreUtils._bundleVersionName))
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00007E38 File Offset: 0x00006038
		public static int BundleVersionCode
		{
			get
			{
				if (CoreUtils._bundleVersionCode == -1)
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

		// Token: 0x06000133 RID: 307 RVA: 0x00007EA4 File Offset: 0x000060A4
		public static IPAddress GetSelfIp()
		{
			IPAddress result = null;
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			foreach (IPAddress ipaddress in hostAddresses)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					result = ipaddress;
				}
			}
			return result;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00007EF0 File Offset: 0x000060F0
		public static string FindScriptBackendDll()
		{
			if (!SharedUtils.Il2Cpp())
			{
				string nativeDllPath = CoreUtils.GetNativeDllPath("libmonobdwgc-2.0");
				if (File.Exists(nativeDllPath))
				{
					return nativeDllPath;
				}
				nativeDllPath = CoreUtils.GetNativeDllPath("libmono");
				if (File.Exists(nativeDllPath))
				{
					return nativeDllPath;
				}
			}
			else
			{
				string nativeDllPath2 = CoreUtils.GetNativeDllPath("libil2cpp");
				if (File.Exists(nativeDllPath2))
				{
					return nativeDllPath2;
				}
			}
			return "";
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007F58 File Offset: 0x00006158
		public static bool WordsIScn(string words)
		{
			return false;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007F5C File Offset: 0x0000615C
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
			if (File.Exists(text))
			{
				return text;
			}
			text = CoreUtils.GetNativeLibraryDir() + "/" + soName + ".so";
			if (File.Exists(text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00007FD0 File Offset: 0x000061D0
		public static string GetNativeLibraryDir()
		{
			if (CoreUtils._nativeLibraryDir != null)
			{
				return CoreUtils._nativeLibraryDir;
			}
			object[] array = new object[]
			{
				SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0])
			};
			CoreUtils._nativeLibraryDir = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationInfo", new object[0]).Get<string>("nativeLibraryDir");
			return CoreUtils._nativeLibraryDir;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000803C File Offset: 0x0000623C
		public static string FormatSize(long size)
		{
			string text = null;
			float num;
			if (size >= 1024L)
			{
				text = "KB";
				num = (float)(size / 1024L);
				if (num >= 1024f)
				{
					text = "MB";
					num /= 1024f;
				}
				if (num >= 1024f)
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
			if (text != null)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000080D4 File Offset: 0x000062D4
		public static string StringReplace(string orc)
		{
			return orc.Replace("\n", "^").Replace("\r", "^").Replace(",", "’");
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008104 File Offset: 0x00006304
		public static void CopyStream(Stream input, Stream output)
		{
			byte[] array = new byte[8192];
			int count;
			while ((count = input.Read(array, 0, array.Length)) > 0)
			{
				output.Write(array, 0, count);
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008140 File Offset: 0x00006340
		public static byte[] GetMD5ForFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
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
			return null;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000081A8 File Offset: 0x000063A8
		public static void WriteFlagIntoBuffer(ref byte[] buffer, short flag)
		{
			Array.Clear(buffer, 0, buffer.Length);
			byte[] bytes = BitConverter.GetBytes(flag);
			Array.Copy(bytes, 0, buffer, 0, bytes.Length);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000081DC File Offset: 0x000063DC
		public static string GetFlagFromBytesBuffer(byte[] buffer, ref int startIndex)
		{
			int num = BitConverter.ToInt32(buffer, startIndex);
			startIndex += 4;
			string @string = Encoding.Unicode.GetString(buffer, startIndex, num);
			startIndex += num;
			return @string;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008214 File Offset: 0x00006414
		public static void WriteEndingFlag(bool end = false)
		{
			string text = SharedUtils.FinalDataPath + "/done";
			string text2 = SharedUtils.FinalDataPath + "/last";
			string path = end ? text : text2;
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.WriteLine(SharedUtils.frameId.ToString());
			streamWriter.Close();
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00008294 File Offset: 0x00006494
		public static void WriteLastTime(long duration)
		{
			string path = SharedUtils.FinalDataPath + "/lasttime";
			StreamWriter streamWriter = new StreamWriter(File.Create(path));
			streamWriter.Write(duration.ToString() + "/1000");
			streamWriter.Close();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000082E0 File Offset: 0x000064E0
		public static void WriteStringToFile(string content, string path)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.Write(content);
			streamWriter.Close();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00008308 File Offset: 0x00006508
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

		// Token: 0x06000142 RID: 322 RVA: 0x00008360 File Offset: 0x00006560
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

		// Token: 0x06000143 RID: 323 RVA: 0x000083B4 File Offset: 0x000065B4
		public static void WriteLocalEndingFlag()
		{
			if (CoreUtils._doneFile == null)
			{
				CoreUtils._doneFile = SharedUtils.FinalDataPath + "/done";
			}
			StreamWriter streamWriter = new StreamWriter(CoreUtils._doneFile, false);
			streamWriter.WriteLine(SharedUtils.frameId.ToString());
			streamWriter.WriteLine(((int)(Time.realtimeSinceStartup - CoreUtils.StartTime)).ToString());
			streamWriter.Close();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00008420 File Offset: 0x00006620
		public static string Cn2En(string cnStr)
		{
			string text = Uri.EscapeUriString(cnStr).Replace("%", "");
			if (text.Length >= 30)
			{
				text = text.Substring(0, 30);
			}
			return text;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00008460 File Offset: 0x00006660
		[Conditional("PROFILE_SDK")]
		public static void ProfileSdkBegin(string name)
		{
			UwaProfiler.UWAEngineInternalEnterProfCpuProfiler(name);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00008468 File Offset: 0x00006668
		[Conditional("PROFILE_SDK")]
		public static void ProfileSdkEnd()
		{
			UwaProfiler.UWAEngineInternalLeaveProfCpuProfiler();
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00008470 File Offset: 0x00006670
		public static void Reset()
		{
			CoreUtils.StartTime = 0f;
			CoreUtils._doneFile = null;
		}

		// Token: 0x04000080 RID: 128
		public const string DllName = "uwa";

		// Token: 0x04000081 RID: 129
		private static bool? _isURP = null;

		// Token: 0x04000084 RID: 132
		private static List<string> classFilter = new List<string>
		{
			"System.",
			"UnityEngine.",
			"Microsoft.",
			"Mono.",
			"Unity.",
			"UWA"
		};

		// Token: 0x04000085 RID: 133
		private static string _appName;

		// Token: 0x04000086 RID: 134
		private static string _socName = null;

		// Token: 0x04000087 RID: 135
		private static int _processorCount = -2;

		// Token: 0x04000088 RID: 136
		private static bool _cpuNotAccess = false;

		// Token: 0x04000089 RID: 137
		private static int _processorFrequency = -2;

		// Token: 0x0400008A RID: 138
		private static string _pkgName;

		// Token: 0x0400008B RID: 139
		private static string _bundleVersionName;

		// Token: 0x0400008C RID: 140
		private static int _bundleVersionCode = -1;

		// Token: 0x0400008D RID: 141
		private static string[] preCheckMono = new string[]
		{
			"/Mono/mono.dll",
			"/Mono/EmbedRuntime/mono.dll",
			"/MonoBleedingEdge/EmbedRuntime/mono-2.0-bdwgc.dll"
		};

		// Token: 0x0400008E RID: 142
		private static string[] preCheckIL2CPP = new string[]
		{
			"/GameAssembly.dll"
		};

		// Token: 0x0400008F RID: 143
		private static string _nativeLibraryDir = null;

		// Token: 0x04000090 RID: 144
		private static string _doneFile = null;

		// Token: 0x04000091 RID: 145
		public static float StartTime = 0f;
	}
}
