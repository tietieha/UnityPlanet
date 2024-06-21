using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UWASDK;
using UWAShared;

namespace UWA.Android
{
	// Token: 0x02000005 RID: 5
	[Preserve]
	[ComVisible(false)]
	public class UWAEngine
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002048 File Offset: 0x00000248
		public static void StaticInit()
		{
			if (GUIWrapper.Get() == null)
			{
				GameObject gameObject = new GameObject("UWA_Launcher");
				GUIWrapper guiwrapper = gameObject.AddComponent<GUIWrapper>();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000207C File Offset: 0x0000027C
		public static void SetUIActive(bool active)
		{
			UWAEngine._uiActive = active;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002084 File Offset: 0x00000284
		public static void Start(UWAEngine.Mode mode)
		{
			SdkCtrlData.Instance.SdkMode = eSdkMode.GOT;
			SdkCtrlData.Instance.GotMode = (eGotMode)mode;
			UWAEngine._mode = mode;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000020A4 File Offset: 0x000002A4
		public static void StartGPM()
		{
			SdkCtrlData.Instance.SdkMode = eSdkMode.GPM;
			SdkCtrlData.Instance.GPMTest = true;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000020BC File Offset: 0x000002BC
		public static void SetConfig(string startConfig)
		{
			SdkStartConfig.UpdateConfig(startConfig);
			SdkStartConfig.Store();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000020CC File Offset: 0x000002CC
		public static void Stop()
		{
			UWAEngine._stop = true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000020D4 File Offset: 0x000002D4
		[Conditional("ENABLE_PROFILER")]
		public static void Dump(eDumpType t)
		{
			SdkCtrlData.Instance.TryDump = t;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000020E4 File Offset: 0x000002E4
		internal static void Set(IUWAAPI api)
		{
			if (UWAEngine._instance == api)
			{
				return;
			}
			UWAEngine._instance = api;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000020F8 File Offset: 0x000002F8
		public static void Note(string note)
		{
			if (UWAEngine._noteCall != null)
			{
				UWAEngine._noteCall(note);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002110 File Offset: 0x00000310
		public static void Upload(Action<bool> callback, string user, string pwd, int projectId, int timeLimitSec)
		{
			if (UWAEngine._uploadInstance == null)
			{
				SharedUtils.LogError("Upload can not be started when the testing is not stopped.");
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			UWAEngine._uploadInstance(callback, new object[]
			{
				user,
				pwd,
				projectId,
				timeLimitSec
			});
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002170 File Offset: 0x00000370
		public static void Upload(Action<bool> callback, string user, string pwd, string projectName, int timeLimitSec)
		{
			if (UWAEngine._uploadInstance == null)
			{
				SharedUtils.LogError("Upload can not be started when the testing is not stopped.");
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			UWAEngine._uploadInstance(callback, new object[]
			{
				user,
				pwd,
				projectName,
				timeLimitSec
			});
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000021CC File Offset: 0x000003CC
		public static void Tag(string tag)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.Tag(tag);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000021E4 File Offset: 0x000003E4
		[Conditional("ENABLE_PROFILER")]
		public static void PushSample(string sampleName)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.PushSample(sampleName);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000021FC File Offset: 0x000003FC
		[Conditional("ENABLE_PROFILER")]
		public static void PopSample()
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.PopSample();
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002214 File Offset: 0x00000414
		[Conditional("ENABLE_PROFILER")]
		public static void LogValue(string valueName, float value)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.LogValue(valueName, value);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000222C File Offset: 0x0000042C
		[Conditional("ENABLE_PROFILER")]
		public static void LogValue(string valueName, int value)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.LogValue(valueName, value);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002244 File Offset: 0x00000444
		[Conditional("ENABLE_PROFILER")]
		public static void LogValue(string valueName, Vector3 value)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.LogValue(valueName, value);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000225C File Offset: 0x0000045C
		[Conditional("ENABLE_PROFILER")]
		public static void LogValue(string valueName, bool value)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.LogValue(valueName, value);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002274 File Offset: 0x00000474
		[Conditional("ENABLE_PROFILER")]
		public static void AddMarker(string valueName)
		{
			if (UWAEngine._instance != null)
			{
				UWAEngine._instance.AddMarker(valueName);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000228C File Offset: 0x0000048C
		public static void SetOverrideAndroidActivity(AndroidJavaObject activity)
		{
			if (activity == null)
			{
				return;
			}
			SharedUtils.OverrideAndroidActivity(activity);
			SharedUtils.Log("Set Override AndroidActivity");
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000022A8 File Offset: 0x000004A8
		public static void SetOverrideLuaState(object luaState)
		{
			SharedUtils.OverrideLuaState(luaState);
			SharedUtils.Log("Set Override LuaState");
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000022BC File Offset: 0x000004BC
		public static void SetOverrideLuaLib(string luaLib)
		{
			if (string.IsNullOrEmpty(luaLib))
			{
				return;
			}
			if (!luaLib.StartsWith("lib", StringComparison.Ordinal))
			{
				luaLib = "lib" + luaLib;
			}
			if (!luaLib.EndsWith(".so", StringComparison.Ordinal))
			{
				luaLib += ".so";
			}
			WrapperTool.LuaDll = luaLib;
			SharedUtils.Log("Set Override Lua Lib as : " + luaLib);
		}

		// Token: 0x04000001 RID: 1
		internal static bool _uiActive = true;

		// Token: 0x04000002 RID: 2
		public static int FrameId = 0;

		// Token: 0x04000003 RID: 3
		public static int DurationS = 0;

		// Token: 0x04000004 RID: 4
		private static IUWAAPI _instance = null;

		// Token: 0x04000005 RID: 5
		internal static UWAEngine.Mode _mode = UWAEngine.Mode.Unset;

		// Token: 0x04000006 RID: 6
		internal static bool _stop = false;

		// Token: 0x04000007 RID: 7
		internal static Action<string> _noteCall = null;

		// Token: 0x04000008 RID: 8
		internal static Action<Action<bool>, object[]> _uploadInstance = null;

		// Token: 0x020000C2 RID: 194
		public enum Mode
		{
			// Token: 0x0400054E RID: 1358
			Overview,
			// Token: 0x0400054F RID: 1359
			Mono,
			// Token: 0x04000550 RID: 1360
			Resources,
			// Token: 0x04000551 RID: 1361
			Lua,
			// Token: 0x04000552 RID: 1362
			GPU,
			// Token: 0x04000553 RID: 1363
			Unset
		}
	}
}
