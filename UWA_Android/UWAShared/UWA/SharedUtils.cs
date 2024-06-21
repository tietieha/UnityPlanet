using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UWASDK;
using UWAShared;

namespace UWA
{
	// Token: 0x0200003E RID: 62
	public static class SharedUtils
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600024C RID: 588 RVA: 0x00017FF0 File Offset: 0x000161F0
		public static string Version
		{
			get
			{
				bool flag = SharedUtils._version == null;
				if (flag)
				{
					SharedUtils._version = "UWA SDK v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
				}
				return SharedUtils._version;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600024D RID: 589 RVA: 0x00018044 File Offset: 0x00016244
		public static bool AutoLaunch
		{
			get
			{
				TextAsset textAsset = Resources.Load<TextAsset>("uwasdksettings");
				bool flag = textAsset == null;
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					UwaSdkSettings uwaSdkSettings = new UwaSdkSettings(textAsset.text);
					result = uwaSdkSettings.AutoLaunch;
				}
				return result;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600024E RID: 590 RVA: 0x00018090 File Offset: 0x00016290
		public static AndroidJavaObject CurrentActivity
		{
			get
			{
				bool flag = SharedUtils._currentActivity != null;
				AndroidJavaObject currentActivity;
				if (flag)
				{
					currentActivity = SharedUtils._currentActivity;
				}
				else
				{
					try
					{
						SharedUtils._currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
					}
					catch (Exception)
					{
					}
					currentActivity = SharedUtils._currentActivity;
				}
				return currentActivity;
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00018100 File Offset: 0x00016300
		public static void OverrideAndroidActivity(AndroidJavaObject jObject)
		{
			bool flag = jObject == null;
			if (!flag)
			{
				SharedUtils._currentActivity = jObject;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000250 RID: 592 RVA: 0x00018128 File Offset: 0x00016328
		// (set) Token: 0x06000251 RID: 593 RVA: 0x00018164 File Offset: 0x00016364
		public static object LuaState
		{
			get
			{
				bool isAlive = SharedUtils.luaStateWref.IsAlive;
				object result;
				if (isAlive)
				{
					result = SharedUtils.luaStateWref.Target;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				SharedUtils.luaStateWref.Target = value;
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00018174 File Offset: 0x00016374
		public static void OverrideLuaState(object obj)
		{
			SharedUtils.luaStateWref.Target = obj;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00018184 File Offset: 0x00016384
		public static bool IsSimulator()
		{
			try
			{
				AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getSystemService", new object[]
				{
					"sensor"
				});
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getDefaultSensor", new object[]
				{
					5
				});
				return androidJavaObject2 == null;
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x000181F8 File Offset: 0x000163F8
		public static bool isVRMode()
		{
			bool result = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			try
			{
				foreach (Assembly assembly in assemblies)
				{
					Type type = assembly.GetType("UnityEngine.XR.XRSettings");
					bool flag = type == null;
					if (flag)
					{
						type = assembly.GetType("UnityEngine.VR.VRSettings");
					}
					bool flag2 = type != null;
					if (flag2)
					{
						PropertyInfo property = type.GetProperty("enabled");
						result = bool.Parse(property.GetValue(null, null).ToString());
						break;
					}
				}
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000182C0 File Offset: 0x000164C0
		public static bool Il2Cpp()
		{
			bool flag = SharedUtils._isIL2CPP != null;
			bool value;
			if (flag)
			{
				value = SharedUtils._isIL2CPP.Value;
			}
			else
			{
				bool? isIL2CPP = null;
				bool flag2 = Application.platform == 11;
				if (flag2)
				{
					bool flag3 = false;
					bool flag4 = false;
					StreamReader streamReader = new StreamReader("/proc/" + Process.GetCurrentProcess().Id.ToString() + "/maps");
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						bool flag5 = !flag3 && text.IndexOf("/libmono") != -1;
						if (flag5)
						{
							flag3 = true;
						}
						bool flag6 = !flag4 && text.IndexOf("/libil2cpp.so") != -1;
						if (flag6)
						{
							flag4 = true;
						}
					}
					streamReader.Close();
					bool flag7 = flag3 && !flag4;
					if (flag7)
					{
						isIL2CPP = new bool?(false);
					}
					else
					{
						bool flag8 = !flag3 && flag4;
						if (flag8)
						{
							isIL2CPP = new bool?(true);
						}
					}
				}
				bool flag9 = isIL2CPP == null;
				if (flag9)
				{
					isIL2CPP = new bool?(false);
					try
					{
						byte[] ilasByteArray = typeof(SharedUtils).GetMethod("Il2Cpp", BindingFlags.Static | BindingFlags.Public).GetMethodBody().GetILAsByteArray();
						bool flag10 = ilasByteArray == null || ilasByteArray.Length == 0;
						if (flag10)
						{
							isIL2CPP = new bool?(true);
						}
					}
					catch
					{
						isIL2CPP = new bool?(true);
					}
				}
				SharedUtils._isIL2CPP = isIL2CPP;
				value = SharedUtils._isIL2CPP.Value;
			}
			return value;
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0001849C File Offset: 0x0001669C
		public static bool Dev
		{
			get
			{
				return Profiler.supported || Debug.isDebugBuild;
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000184CC File Offset: 0x000166CC
		public static bool IsAndroid10Featured()
		{
			return (SharedUtils.GetAndroidAppSDK_INT() >= 29 && SharedUtils.GetAndroidDeviceSDK_INT() >= 29) || SharedUtils.GetAndroidDeviceSDK_INT() >= 30;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00018510 File Offset: 0x00016710
		public static bool IsPicoVR()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			string @static = androidJavaClass.GetStatic<string>("MODEL");
			string static2 = androidJavaClass.GetStatic<string>("BRAND");
			return @static.Contains("Pico") || static2.Contains("Pico") || static2.Contains("oculus");
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00018580 File Offset: 0x00016780
		public static int GetAndroidDeviceSDK_INT()
		{
			bool flag = SharedUtils._androidDeviceSdk_INT != 0;
			int androidDeviceSdk_INT;
			if (flag)
			{
				androidDeviceSdk_INT = SharedUtils._androidDeviceSdk_INT;
			}
			else
			{
				try
				{
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION");
					SharedUtils._androidDeviceSdk_INT = androidJavaClass.GetStatic<int>("SDK_INT");
				}
				catch (Exception ex)
				{
				}
				androidDeviceSdk_INT = SharedUtils._androidDeviceSdk_INT;
			}
			return androidDeviceSdk_INT;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x000185F0 File Offset: 0x000167F0
		public static int GetAndroidAppSDK_INT()
		{
			bool flag = SharedUtils._androidAppSdk_INT != 0;
			int androidAppSdk_INT;
			if (flag)
			{
				androidAppSdk_INT = SharedUtils._androidAppSdk_INT;
			}
			else
			{
				try
				{
					AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
					AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getApplicationInfo", new object[0]);
					SharedUtils._androidAppSdk_INT = androidJavaObject2.Get<int>("targetSdkVersion");
				}
				catch (Exception ex)
				{
				}
				androidAppSdk_INT = SharedUtils._androidAppSdk_INT;
			}
			return androidAppSdk_INT;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00018680 File Offset: 0x00016880
		public static string GetAndroidBuildInfo(string info)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			return androidJavaClass.GetStatic<string>(info);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000186AC File Offset: 0x000168AC
		public static string GetCodeCacheDir()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION");
			int @static = androidJavaClass.GetStatic<int>("SDK_INT");
			bool flag = @static >= 21;
			string result;
			if (flag)
			{
				AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getCodeCacheDir", new object[0]);
				string text = androidJavaObject.Call<string>("getAbsolutePath", new object[0]);
				result = text;
			}
			else
			{
				AndroidJavaObject androidJavaObject2 = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationInfo", new object[0]);
				AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.io.File", new object[]
				{
					androidJavaObject2.Get<string>("dataDir"),
					"code_cache"
				});
				bool flag2 = androidJavaObject3.Call<bool>("mkdir", new object[0]);
				if (flag2)
				{
					string text2 = androidJavaObject3.Call<string>("getPath", new object[0]);
					result = text2;
				}
				else
				{
					result = androidJavaObject2.Get<string>("dataDir");
				}
			}
			return result;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x000187A8 File Offset: 0x000169A8
		public static bool VerifyWritePermision()
		{
			bool flag = Application.platform != 11;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				try
				{
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION");
					int @static = androidJavaClass.GetStatic<int>("SDK_INT");
					bool flag2 = @static < 23;
					if (flag2)
					{
						return true;
					}
					int num = 1;
					int num2 = 0;
					string[] array = new string[]
					{
						"android.permission.READ_EXTERNAL_STORAGE",
						"android.permission.WRITE_EXTERNAL_STORAGE"
					};
					int num3 = SharedUtils.CurrentActivity.Call<int>("checkSelfPermission", new object[]
					{
						"android.permission.WRITE_EXTERNAL_STORAGE"
					});
					bool flag3 = num3 != num2;
					if (flag3)
					{
						SharedUtils.CurrentActivity.Call("requestPermissions", new object[]
						{
							array,
							num
						});
					}
					num3 = SharedUtils.CurrentActivity.Call<int>("checkSelfPermission", new object[]
					{
						"android.permission.WRITE_EXTERNAL_STORAGE"
					});
					return num3 == num2;
				}
				catch (Exception)
				{
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x000188C0 File Offset: 0x00016AC0
		public static bool OverrideFinalPath(string dataPath)
		{
			bool flag = !dataPath.StartsWith(Application.persistentDataPath);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SharedUtils.FinalDataPath = dataPath;
				result = true;
			}
			return result;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00018900 File Offset: 0x00016B00
		public static void SetupFinalPath()
		{
			SharedUtils.FinalDataPath = SharedUtils.ProfileDataFolder + "/" + UWACoreConfig.KEY.Replace(" ", "");
			bool flag = !Directory.Exists(SharedUtils.FinalDataPath);
			if (flag)
			{
				Directory.CreateDirectory(SharedUtils.FinalDataPath);
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0001895C File Offset: 0x00016B5C
		public static void ClearFinalPath()
		{
			bool flag = Directory.Exists(SharedUtils.FinalDataPath);
			if (flag)
			{
				Directory.Delete(SharedUtils.FinalDataPath, true);
			}
			Directory.CreateDirectory(SharedUtils.FinalDataPath);
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00018998 File Offset: 0x00016B98
		// (set) Token: 0x06000262 RID: 610 RVA: 0x000189A0 File Offset: 0x00016BA0
		public static string FinalDataPath { get; private set; }

		// Token: 0x06000263 RID: 611 RVA: 0x000189A8 File Offset: 0x00016BA8
		public static string GetGotJsonExternal()
		{
			return Application.persistentDataPath + "/uwa-got.json";
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000189D0 File Offset: 0x00016BD0
		public static string GetGotJsonInternal()
		{
			return SharedUtils.FinalDataPath + "/uwa-got.json";
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000189F8 File Offset: 0x00016BF8
		public static bool HasNote()
		{
			bool flag = File.Exists(SharedUtils.FinalDataPath + "/note");
			return flag && !string.IsNullOrEmpty(File.ReadAllText(SharedUtils.FinalDataPath + "/note"));
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00018A50 File Offset: 0x00016C50
		public static void SetNote(string note)
		{
			File.WriteAllText(SharedUtils.FinalDataPath + "/note", note);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00018A6C File Offset: 0x00016C6C
		public static string GetStorageDirectory()
		{
			string result = "";
			bool flag = Application.platform == 11;
			if (flag)
			{
				bool flag2 = false;
				try
				{
					flag2 = SharedUtils.VerifyWritePermision();
				}
				catch (Exception)
				{
				}
				bool flag3 = !flag2;
				if (flag3)
				{
					SharedUtils.UseInternalDataFolder = true;
					result = Application.persistentDataPath;
				}
				else
				{
					SharedUtils.UseInternalDataFolder = false;
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment");
					AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[]
					{
						0U
					});
					result = androidJavaObject.Call<string>("getCanonicalPath", new object[0]);
				}
			}
			else
			{
				bool flag4 = Application.platform == 2;
				if (flag4)
				{
					SharedUtils.UseInternalDataFolder = false;
					result = "C:";
				}
				else
				{
					bool flag5 = Application.platform == 8;
					if (flag5)
					{
						SharedUtils.UseInternalDataFolder = true;
						result = Application.persistentDataPath;
					}
				}
			}
			return result;
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00018B74 File Offset: 0x00016D74
		// (set) Token: 0x06000269 RID: 617 RVA: 0x00018B7C File Offset: 0x00016D7C
		public static string ConfigDataFolder { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600026A RID: 618 RVA: 0x00018B84 File Offset: 0x00016D84
		// (set) Token: 0x0600026B RID: 619 RVA: 0x00018B8C File Offset: 0x00016D8C
		public static string ProfileDataFolder { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600026C RID: 620 RVA: 0x00018B94 File Offset: 0x00016D94
		// (set) Token: 0x0600026D RID: 621 RVA: 0x00018B9C File Offset: 0x00016D9C
		public static bool UseInternalDataFolder { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600026E RID: 622 RVA: 0x00018BA4 File Offset: 0x00016DA4
		// (set) Token: 0x0600026F RID: 623 RVA: 0x00018BAC File Offset: 0x00016DAC
		public static bool CanAccessDataRoot { get; private set; }

		// Token: 0x06000270 RID: 624 RVA: 0x00018BB4 File Offset: 0x00016DB4
		public static void SetupAllPath()
		{
			SharedUtils._uwadatacenterPath = SharedUtils.GetStorageDirectory() + "/UWA-DataCenter";
			bool flag = SharedUtils.EnsureDataFolders();
			bool flag2 = !flag && !SharedUtils.UseInternalDataFolder;
			if (flag2)
			{
				SharedUtils.UseInternalDataFolder = true;
				SharedUtils._uwadatacenterPath = Application.persistentDataPath + "/UWA-DataCenter";
				flag = SharedUtils.EnsureDataFolders();
			}
			SharedUtils.CanAccessDataRoot = flag;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00018C28 File Offset: 0x00016E28
		private static bool EnsureDataFolders()
		{
			bool flag = false;
			try
			{
				bool flag2 = !Directory.Exists(SharedUtils._uwadatacenterPath);
				if (flag2)
				{
					Directory.CreateDirectory(SharedUtils._uwadatacenterPath);
				}
				SharedUtils.ConfigDataFolder = SharedUtils._uwadatacenterPath + "/ConfigData";
				SharedUtils.ProfileDataFolder = SharedUtils._uwadatacenterPath + "/ProfileData";
				bool flag3 = !Directory.Exists(SharedUtils.ConfigDataFolder);
				if (flag3)
				{
					Directory.CreateDirectory(SharedUtils.ConfigDataFolder);
				}
				bool flag4 = !Directory.Exists(SharedUtils.ProfileDataFolder);
				if (flag4)
				{
					Directory.CreateDirectory(SharedUtils.ProfileDataFolder);
				}
				File.WriteAllText(SharedUtils.ConfigDataFolder + "/try", "");
				File.Delete(SharedUtils.ConfigDataFolder + "/try");
			}
			catch (Exception)
			{
				flag = true;
			}
			return !flag;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00018D24 File Offset: 0x00016F24
		public static List<string> GetOldData(eSdkMode testType)
		{
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("GetOldData : " + SharedUtils.ProfileDataFolder);
			}
			string[] directories = Directory.GetDirectories(SharedUtils.ProfileDataFolder);
			Array.Sort<string>(directories);
			Array.Reverse(directories);
			List<string> list = new List<string>();
			for (int i = 0; i < directories.Length; i++)
			{
				bool flag = SharedUtils.IsDataValid(directories[i], testType);
				if (flag)
				{
					list.Add(directories[i]);
				}
			}
			return list;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00018DC0 File Offset: 0x00016FC0
		private static bool IsDataValid(string path, eSdkMode testType)
		{
			string path2 = path + "/systemInfo";
			return File.Exists(path2);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00018DEC File Offset: 0x00016FEC
		private static void LogCallback(string logString, string stackTrace, LogType type)
		{
			bool flag = SharedUtils._log2File && SharedUtils.logSw != null;
			if (flag)
			{
				SharedUtils.logSw.WriteLine("Unity [" + type.ToString() + "] " + DateTime.Now.ToString("yyyyMMdd HHmmss :"));
				SharedUtils.logSw.WriteLine(logString);
				SharedUtils.logSw.WriteLine(stackTrace);
				SharedUtils.logSw.WriteLine("");
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00018E80 File Offset: 0x00017080
		// (set) Token: 0x06000276 RID: 630 RVA: 0x00018EA0 File Offset: 0x000170A0
		public static bool Log2File
		{
			get
			{
				return SharedUtils._log2File;
			}
			set
			{
				bool flag = SharedUtils._log2File == value;
				if (!flag)
				{
					bool flag2 = value && SharedUtils.logSw == null;
					if (flag2)
					{
						string text = Application.persistentDataPath + "/uwa_log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
						Debug.Log("Local Log:" + text);
						SharedUtils.logSw = new StreamWriter(text, false);
						SharedUtils.logSw.AutoFlush = true;
						Application.logMessageReceived += new Application.LogCallback(SharedUtils.LogCallback);
					}
					bool flag3 = !value && SharedUtils.logSw != null;
					if (flag3)
					{
						SharedUtils.logSw.Close();
						SharedUtils.logSw = null;
						Application.logMessageReceived -= new Application.LogCallback(SharedUtils.LogCallback);
					}
					SharedUtils._log2File = value;
				}
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00018F90 File Offset: 0x00017190
		public static void LogError(string content)
		{
			bool flag = SharedUtils._log2File && SharedUtils.logSw != null;
			if (flag)
			{
				SharedUtils.logSw.WriteLine("UWA [Error] " + DateTime.Now.ToString("yyyyMMdd HHmmss :"));
				SharedUtils.logSw.WriteLine(content);
				SharedUtils.logSw.WriteLine("");
			}
			else
			{
				Debug.LogError(content);
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00019014 File Offset: 0x00017214
		public static void Log(string content)
		{
			bool flag = SharedUtils._log2File && SharedUtils.logSw != null;
			if (flag)
			{
				SharedUtils.logSw.WriteLine("UWA [Log] " + DateTime.Now.ToString("yyyyMMdd HHmmss :"));
				SharedUtils.logSw.WriteLine(content);
				SharedUtils.logSw.WriteLine("");
			}
			else
			{
				Debug.Log(content);
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000279 RID: 633 RVA: 0x00019098 File Offset: 0x00017298
		public static int GroupWidth
		{
			get
			{
				return (int)((float)SharedUtils.ScreenWidth * 0.38200003f);
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600027A RID: 634 RVA: 0x000190C0 File Offset: 0x000172C0
		public static int GroupHeight
		{
			get
			{
				return (int)((float)SharedUtils.ScreenHeight * 0.38200003f * 0.5f);
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600027B RID: 635 RVA: 0x000190EC File Offset: 0x000172EC
		public static Rect GroupRect
		{
			get
			{
				return new Rect((float)(Screen.width - SharedUtils.GroupWidth), 0f, (float)SharedUtils.GroupWidth, (float)SharedUtils.GroupHeight);
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600027C RID: 636 RVA: 0x00019128 File Offset: 0x00017328
		public static int ScreenWidth
		{
			get
			{
				return (Screen.width > Screen.height) ? Screen.width : Screen.height;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600027D RID: 637 RVA: 0x00019160 File Offset: 0x00017360
		public static int ScreenHeight
		{
			get
			{
				return (Screen.width > Screen.height) ? Screen.height : Screen.width;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00019198 File Offset: 0x00017398
		public static int UploadWinWidth
		{
			get
			{
				return (int)((double)SharedUtils.ScreenHeight * 0.9);
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600027F RID: 639 RVA: 0x000191C4 File Offset: 0x000173C4
		public static int UploadWinHeight
		{
			get
			{
				return (int)((double)SharedUtils.ScreenHeight * 0.9);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000280 RID: 640 RVA: 0x000191F0 File Offset: 0x000173F0
		public static int TipsWidth
		{
			get
			{
				return (int)((double)SharedUtils.ScreenWidth * 0.9);
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000281 RID: 641 RVA: 0x0001921C File Offset: 0x0001741C
		public static int TipsHeight
		{
			get
			{
				return (int)((double)SharedUtils.ScreenHeight * 0.1);
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000282 RID: 642 RVA: 0x00019248 File Offset: 0x00017448
		public static float ScreenSizeScale
		{
			get
			{
				bool flag = SharedUtils.ScreenHeight < 750;
				float result;
				if (flag)
				{
					result = 1.5f;
				}
				else
				{
					result = 1f;
				}
				return result;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000283 RID: 643 RVA: 0x00019284 File Offset: 0x00017484
		public static Rect UploadWinRect
		{
			get
			{
				return new Rect((float)(Screen.width / 2 - SharedUtils.UploadWinWidth / 2), (float)(Screen.height / 2 - SharedUtils.UploadWinHeight / 2), (float)SharedUtils.UploadWinWidth, (float)SharedUtils.UploadWinHeight);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000284 RID: 644 RVA: 0x000192D0 File Offset: 0x000174D0
		public static Rect TipsRect
		{
			get
			{
				return new Rect((float)(Screen.width / 2 - SharedUtils.TipsWidth / 2), (float)(Screen.height / 2 - SharedUtils.TipsHeight), (float)SharedUtils.TipsWidth, (float)(SharedUtils.TipsHeight * 2));
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0001931C File Offset: 0x0001751C
		public static bool ResolutionChanged()
		{
			bool flag = SharedUtils.lastScreenWidth == -1;
			if (flag)
			{
				SharedUtils.lastScreenWidth = SharedUtils.ScreenWidth;
			}
			bool result = SharedUtils.lastScreenWidth != SharedUtils.ScreenWidth;
			SharedUtils.lastScreenWidth = SharedUtils.ScreenWidth;
			return result;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0001936C File Offset: 0x0001756C
		public static GUILayoutOption[] GetSuitableOption(float x, float y)
		{
			return SharedUtils.GetLayout((int)((float)SharedUtils.GroupWidth * x), (int)((float)SharedUtils.GroupHeight * y));
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0001939C File Offset: 0x0001759C
		public static GUILayoutOption[] GetSuitableOptionUpload(float x, float y)
		{
			return SharedUtils.GetLayout((int)((float)SharedUtils.UploadWinWidth * x), (int)((float)SharedUtils.UploadWinHeight * y));
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000193CC File Offset: 0x000175CC
		private static GUILayoutOption[] GetLayout(int x, int y)
		{
			bool flag = x < 0;
			if (flag)
			{
				x = -1;
			}
			bool flag2 = y < 0;
			if (flag2)
			{
				y = -1;
			}
			GUILayoutOption[] array = null;
			Dictionary<int, GUILayoutOption[]> dictionary = null;
			bool flag3 = !SharedUtils.layoutCache.TryGetValue(x, out dictionary);
			if (flag3)
			{
				dictionary = new Dictionary<int, GUILayoutOption[]>();
				SharedUtils.layoutCache.Add(x, dictionary);
			}
			bool flag4 = !dictionary.TryGetValue(y, out array);
			if (flag4)
			{
				array = SharedUtils.GetSuitableOptionInternal(x, y);
				dictionary.Add(y, array);
			}
			return array;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00019460 File Offset: 0x00017660
		private static GUILayoutOption[] GetSuitableOptionInternal(int width, int height)
		{
			bool flag = height < 0;
			GUILayoutOption[] result;
			if (flag)
			{
				result = new GUILayoutOption[]
				{
					GUILayout.Width((float)width),
					GUILayout.ExpandWidth(true)
				};
			}
			else
			{
				bool flag2 = width < 0;
				if (flag2)
				{
					result = new GUILayoutOption[]
					{
						GUILayout.Height((float)height),
						GUILayout.ExpandHeight(true)
					};
				}
				else
				{
					result = new GUILayoutOption[]
					{
						GUILayout.Width((float)width),
						GUILayout.Height((float)height),
						GUILayout.ExpandWidth(true),
						GUILayout.ExpandHeight(true)
					};
				}
			}
			return result;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x000194FC File Offset: 0x000176FC
		private static void OnLowMemory()
		{
		}

		// Token: 0x04000202 RID: 514
		public static bool ShowLog = false;

		// Token: 0x04000203 RID: 515
		public static int frameId = -1;

		// Token: 0x04000204 RID: 516
		public static int durationS = -1;

		// Token: 0x04000205 RID: 517
		public static string durationStr = null;

		// Token: 0x04000206 RID: 518
		private static string _version = null;

		// Token: 0x04000207 RID: 519
		private static AndroidJavaObject _currentActivity = null;

		// Token: 0x04000208 RID: 520
		private static WeakReference luaStateWref = new WeakReference(null);

		// Token: 0x04000209 RID: 521
		private static bool? _isIL2CPP = null;

		// Token: 0x0400020A RID: 522
		private static int _androidDeviceSdk_INT = 0;

		// Token: 0x0400020B RID: 523
		private static int _androidAppSdk_INT = 0;

		// Token: 0x0400020D RID: 525
		private static string _uwadatacenterPath;

		// Token: 0x04000212 RID: 530
		private static bool _log2File = false;

		// Token: 0x04000213 RID: 531
		private static StreamWriter logSw = null;

		// Token: 0x04000214 RID: 532
		private static int lastScreenWidth = -1;

		// Token: 0x04000215 RID: 533
		private static Dictionary<int, Dictionary<int, GUILayoutOption[]>> layoutCache = new Dictionary<int, Dictionary<int, GUILayoutOption[]>>();

		// Token: 0x020000FD RID: 253
		public static class MatPropTools
		{
			// Token: 0x060009C9 RID: 2505 RVA: 0x00047920 File Offset: 0x00045B20
			public static bool Support()
			{
				bool flag = !SharedUtils.MatPropTools._tryGetTexProp;
				if (flag)
				{
					SharedUtils.MatPropTools._tryGetTexProp = true;
					SharedUtils.MatPropTools.mGetTexturePropertyNameIDs = typeof(Material).GetMethod("GetTexturePropertyNameIDs", new Type[]
					{
						typeof(List<int>)
					});
				}
				return SharedUtils.MatPropTools.mGetTexturePropertyNameIDs != null;
			}

			// Token: 0x060009CA RID: 2506 RVA: 0x00047984 File Offset: 0x00045B84
			public static bool GetTexPropIds(Material m, ref List<int> ids)
			{
				bool flag = SharedUtils.MatPropTools.Support();
				bool result;
				if (flag)
				{
					SharedUtils.MatPropTools.mGetTexturePropertyNameIDs.Invoke(m, new object[]
					{
						ids
					});
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x04000665 RID: 1637
			private static bool _tryGetTexProp;

			// Token: 0x04000666 RID: 1638
			private static MethodInfo mGetTexturePropertyNameIDs;
		}

		// Token: 0x020000FE RID: 254
		public static class MeshUVDMTools
		{
			// Token: 0x060009CB RID: 2507 RVA: 0x000479CC File Offset: 0x00045BCC
			public static bool Support()
			{
				bool flag = !SharedUtils.MeshUVDMTools._tryGetMeshUVDM;
				if (flag)
				{
					SharedUtils.MeshUVDMTools._tryGetMeshUVDM = true;
					SharedUtils.MeshUVDMTools.mGetUVDistributionMetric = typeof(Mesh).GetMethod("GetUVDistributionMetric", new Type[]
					{
						typeof(int)
					});
				}
				return SharedUtils.MeshUVDMTools.mGetUVDistributionMetric != null;
			}

			// Token: 0x060009CC RID: 2508 RVA: 0x00047A30 File Offset: 0x00045C30
			public static bool GetMeshUVDM(Mesh m, int uvSetIndex, ref float dm)
			{
				bool flag = SharedUtils.MeshUVDMTools.Support();
				bool result;
				if (flag)
				{
					dm = (float)SharedUtils.MeshUVDMTools.mGetUVDistributionMetric.Invoke(m, new object[]
					{
						uvSetIndex
					});
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x04000667 RID: 1639
			private static bool _tryGetMeshUVDM;

			// Token: 0x04000668 RID: 1640
			private static MethodInfo mGetUVDistributionMetric;
		}
	}
}
