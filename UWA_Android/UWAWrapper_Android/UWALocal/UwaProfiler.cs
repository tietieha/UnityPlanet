using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UWA;
using UWASDK;

namespace UWALocal
{
	// Token: 0x0200001C RID: 28
	internal static class UwaProfiler
	{
		// Token: 0x06000172 RID: 370
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern int NativeVersion();

		// Token: 0x06000173 RID: 371
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DevBuild(bool dev);

		// Token: 0x06000174 RID: 372
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern int StartProfilerOn(int profilingMode, string configJson);

		// Token: 0x06000175 RID: 373
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern void ProfilerDataRootPath(string s);

		// Token: 0x06000176 RID: 374
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetFrameAtEnd(int s);

		// Token: 0x06000177 RID: 375
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void AddFilter(string s, int t);

		// Token: 0x06000178 RID: 376
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Res_setmap_path(string path);

		// Token: 0x06000179 RID: 377
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SetRenderingSyncEventId(int n);

		// Token: 0x0600017A RID: 378
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SetEventId(int event1, int event2, int event3, int event4, int event5, int event6, int event7);

		// Token: 0x0600017B RID: 379
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineInternalEnterProfCpuProfiler(string s);

		// Token: 0x0600017C RID: 380
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineInternalLeaveProfCpuProfiler();

		// Token: 0x0600017D RID: 381
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEnginePushSample(string s);

		// Token: 0x0600017E RID: 382
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEnginePopSample();

		// Token: 0x0600017F RID: 383
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineLogMarker(string n);

		// Token: 0x06000180 RID: 384
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineLogInt(string n, int v);

		// Token: 0x06000181 RID: 385
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineLogFloat(string n, float v);

		// Token: 0x06000182 RID: 386
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineLogVector(string n, float x, float y, float z);

		// Token: 0x06000183 RID: 387
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UWAEngineLogBool(string n, bool v);

		// Token: 0x06000184 RID: 388
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void StopProfiling();

		// Token: 0x06000185 RID: 389
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void MarkStartTime(string date);

		// Token: 0x06000186 RID: 390
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void AddOverhead(long timeMs);

		// Token: 0x06000187 RID: 391
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool IsFeatureSupport(string s);

		// Token: 0x06000188 RID: 392
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr GetFeatureInfo(string s);

		// Token: 0x06000189 RID: 393
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void OutputMonoMemory(uint used, uint unused);

		// Token: 0x0600018A RID: 394
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SyncAndDumpDataMeta(ref UwaProfiler.DataMeta meta);

		// Token: 0x0600018B RID: 395
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool PreLoad(string s, bool i);

		// Token: 0x0600018C RID: 396
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetNativeLibPath(string s);

		// Token: 0x0600018D RID: 397 RVA: 0x0000967C File Offset: 0x0000787C
		public static bool PreLoad()
		{
			string text = CoreUtils.FindScriptBackendDll();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Script Dll: " + text);
			}
			bool result = UwaProfiler.PreLoad(text, SharedUtils.Il2Cpp());
			string text2 = CoreUtils.GetNativeLibraryDir() ?? "";
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Native Dll: " + text2);
			}
			UwaProfiler.SetNativeLibPath(text2);
			return result;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000096F4 File Offset: 0x000078F4
		private static void SetupDataMeta()
		{
			UwaProfiler.m.PluginVersion = UWACoreConfig.PLUGIN_VERSION;
			UwaProfiler.m.TestMode = ((SdkCtrlData.Instance.SdkMode == eSdkMode.GPM) ? "GPM" : UwaLocalStarter.Get().TestMode.ToString());
			UwaProfiler.m.DeviceModel = SystemInfo.deviceModel;
			UwaProfiler.m.EngineVersion = Application.unityVersion;
			UwaProfiler.m.Engine = 1;
			UwaProfiler.m.Platform = 3;
			if (Application.platform == 11)
			{
				UwaProfiler.m.Platform = 1;
			}
			if (Application.platform == 8)
			{
				UwaProfiler.m.Platform = 2;
			}
			UwaProfiler.m.FrameCount = 0;
			UwaProfiler.m.Duration = 0;
			UwaProfiler.m.OverviewLua = 0;
			UwaProfiler.m.OverviewGpu = 0;
			UwaProfiler.m.OverviewMono = 0;
			UwaProfiler.m.OverviewResources = 0;
			UwaProfiler.m.Texture2DCount = 0;
			UwaProfiler.m.MeshCount = 0;
			UwaProfiler.m.AnimationClipCount = 0;
			UwaProfiler.m.AudioClipCount = 0;
			UwaProfiler.m.FontCount = 0;
			UwaProfiler.m.MaterialCount = 0;
			UwaProfiler.m.MeshColliderCount = 0;
			UwaProfiler.m.ParticleSystemCount = 0;
			UwaProfiler.m.RenderTextureCount = 0;
			UwaProfiler.m.ShaderCount = 0;
			UwaProfiler.m.AssetBundleCount = 0;
			UwaProfiler.m.TextAssetCount = 0;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00009878 File Offset: 0x00007A78
		public static bool Setup(string dataPath, UwaProfiler.Mode mode)
		{
			if (UwaProfiler._worked)
			{
				return true;
			}
			UwaProfiler.SetupDataMeta();
			try
			{
				UwaProfiler._worked = false;
				UwaProfiler._mono_worked = false;
				UwaProfiler.TrackMode = mode;
				int profilingMode = -1;
				if ((mode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
				{
					profilingMode = 0;
				}
				else if ((mode & UwaProfiler.Mode.Memory) != (UwaProfiler.Mode)0)
				{
					profilingMode = 1;
				}
				else if ((mode & UwaProfiler.Mode.Resources) != (UwaProfiler.Mode)0)
				{
					profilingMode = 2;
				}
				else if ((mode & UwaProfiler.Mode.Lua) != (UwaProfiler.Mode)0)
				{
					profilingMode = 3;
				}
				else if ((mode & UwaProfiler.Mode.Gpu) != (UwaProfiler.Mode)0)
				{
					profilingMode = 4;
				}
				try
				{
					UwaProfiler._dataPath = dataPath;
					if (!Directory.Exists(UwaProfiler._dataPath))
					{
						Directory.CreateDirectory(UwaProfiler._dataPath);
					}
					UwaProfiler.ProfilerDataRootPath(UwaProfiler._dataPath);
					UwaProfiler.DevBuild(SharedUtils.Dev);
				}
				catch (Exception)
				{
				}
				string configJson = "";
				string gotJsonInternal = SharedUtils.GetGotJsonInternal();
				if (File.Exists(gotJsonInternal))
				{
					configJson = File.ReadAllText(gotJsonInternal);
				}
				string path = SharedUtils.GetGotJsonExternal().Replace("uwa-got.json", "uwa-got-debug.json");
				if (File.Exists(path))
				{
					configJson = File.ReadAllText(path);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("Use Debug Json.");
					}
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("Start StartProfilerOn..");
				}
				int num = UwaProfiler.StartProfilerOn(profilingMode, configJson);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("End StartProfilerOn..");
				}
				UwaProfiler._mono_worked = (num == 1);
				UwaProfiler._newProfiler = true;
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("New MonoProfiler : " + ((num == 0) ? "Failed" : "Success"));
				}
				UwaProfiler._dataPath = dataPath;
				if (!Directory.Exists(UwaProfiler._dataPath))
				{
					Directory.CreateDirectory(UwaProfiler._dataPath);
				}
				UwaProfiler._worked = true;
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("UwaProfiler works");
				}
			}
			catch (Exception ex)
			{
				string str = "StartProfiler exception : ";
				Exception ex2 = ex;
				SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
			}
			try
			{
				UwaProfiler.AddFilter("", 0);
				UwaProfiler.SupportFilter = true;
			}
			catch (Exception ex3)
			{
				string str2 = "AddFilter is not supported ";
				Exception ex4 = ex3;
				SharedUtils.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
				UwaProfiler.SupportFilter = false;
			}
			if (UwaProfiler.SupportFilter)
			{
				for (int i = 0; i < UwaProfiler.NameSpaceFilter.Length; i++)
				{
					UwaProfiler.AddFilter(UwaProfiler.NameSpaceFilter[i], 1);
				}
				for (int j = 0; j < UwaProfiler.ClassNameFilter.Length; j++)
				{
					UwaProfiler.AddFilter(UwaProfiler.ClassNameFilter[j], 2);
				}
				for (int k = 0; k < UwaProfiler.MethodNameFilter.Length; k++)
				{
					UwaProfiler.AddFilter(UwaProfiler.MethodNameFilter[k], 3);
				}
				for (int l = 0; l < UwaProfiler.MethodNameRemain.Length; l++)
				{
					UwaProfiler.AddFilter(UwaProfiler.MethodNameRemain[l], 4);
				}
				for (int m = 0; m < UwaProfiler.NameSpaceRemain.Length; m++)
				{
					UwaProfiler.AddFilter(UwaProfiler.NameSpaceRemain[m], 5);
				}
			}
			return UwaProfiler._worked;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00009BE0 File Offset: 0x00007DE0
		public static void SetWorked(bool worked)
		{
			UwaProfiler._worked = worked;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00009BE8 File Offset: 0x00007DE8
		public static void Start()
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.EnableRenderingLog();
			}
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Gpu) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.EnableRenderingLog();
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00009C10 File Offset: 0x00007E10
		public static void UpdateAtEnd()
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.RenderingSync();
				UwaProfiler.RenderingStart();
			}
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Gpu) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.RenderingSync();
				UwaProfiler.RenderingStart();
			}
			UwaProfiler.SetFrameAtEnd(SharedUtils.frameId);
			if (SharedUtils.frameId % 1000 == 0)
			{
				UwaProfiler.UpdateDataMeta();
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00009C70 File Offset: 0x00007E70
		public static void Stop()
		{
			UwaProfiler.UpdateDataMeta();
			try
			{
				UwaProfiler.StopProfiling();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00009CA4 File Offset: 0x00007EA4
		private static void UpdateDataMeta()
		{
			UwaProfiler.m.FrameCount = SharedUtils.frameId;
			UwaProfiler.m.Duration = SharedUtils.durationS;
			if (UwaLocalStarter.Get().TestMode == UwaLocalStarter.eTestMode.Overview)
			{
				UwaProfiler.m.OverviewLua = ((SdkStartConfig.Instance.overview.lua_mem_stack || SdkStartConfig.Instance.overview.lua_cpu_stack) ? 1 : 0);
				UwaProfiler.m.OverviewGpu = 1;
				UwaProfiler.m.OverviewMono = 1;
				UwaProfiler.m.OverviewResources = ((SdkStartConfig.Instance.overview.resources > 0) ? 1 : 0);
			}
			if (UwaLocalStarter.Get().AssetTrackMgr != null && UwaLocalStarter.Get().AssetTrackMgr.Enabled)
			{
				Dictionary<Type, int> perfStats = UwaLocalStarter.Get().AssetTrackMgr.PerfStats;
				int num;
				if (perfStats.TryGetValue(typeof(Texture2D), out num))
				{
					UwaProfiler.m.Texture2DCount = num;
				}
				if (perfStats.TryGetValue(typeof(Mesh), out num))
				{
					UwaProfiler.m.MeshCount = num;
				}
				if (perfStats.TryGetValue(typeof(AnimationClip), out num))
				{
					UwaProfiler.m.AnimationClipCount = num;
				}
				if (perfStats.TryGetValue(typeof(AudioClip), out num))
				{
					UwaProfiler.m.AudioClipCount = num;
				}
				if (perfStats.TryGetValue(typeof(Font), out num))
				{
					UwaProfiler.m.FontCount = num;
				}
				if (perfStats.TryGetValue(typeof(Material), out num))
				{
					UwaProfiler.m.MaterialCount = num;
				}
				if (perfStats.TryGetValue(typeof(MeshCollider), out num))
				{
					UwaProfiler.m.MeshColliderCount = num;
				}
				if (perfStats.TryGetValue(typeof(ParticleSystem), out num))
				{
					UwaProfiler.m.ParticleSystemCount = num;
				}
				if (perfStats.TryGetValue(typeof(RenderTexture), out num))
				{
					UwaProfiler.m.RenderTextureCount = num;
				}
				if (perfStats.TryGetValue(typeof(Shader), out num))
				{
					UwaProfiler.m.ShaderCount = num;
				}
				if (perfStats.TryGetValue(typeof(AssetBundle), out num))
				{
					UwaProfiler.m.AssetBundleCount = num;
				}
				if (perfStats.TryGetValue(typeof(TextAsset), out num))
				{
					UwaProfiler.m.TextAssetCount = num;
				}
			}
			UwaProfiler.SyncAndDumpDataMeta(ref UwaProfiler.m);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00009F2C File Offset: 0x0000812C
		public static void EnableRenderingLog()
		{
			try
			{
				UwaProfiler.SetEventId(0, 0, 0, 0, 0, 20201, 20202);
				UwaProfiler._renderingSync = true;
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("SetRenderingSyncEventId");
				}
			}
			catch (Exception)
			{
			}
			UwaProfiler.Res_setmap_path(SharedUtils.FinalDataPath + "/gpu_res_id_map.txt");
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00009F98 File Offset: 0x00008198
		public static void RenderingStart()
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("RenderingStart");
			}
			if (UwaProfiler._renderingSync)
			{
				GL.IssuePluginEvent(20201);
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00009FC4 File Offset: 0x000081C4
		public static void RenderingSync()
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("RenderingSync");
			}
			if (UwaProfiler._renderingSync)
			{
				GL.IssuePluginEvent(20202);
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00009FF0 File Offset: 0x000081F0
		public static void AddMarker(string valueName)
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEngineLogMarker(valueName);
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A004 File Offset: 0x00008204
		public static void LogValue(string valueName, float value)
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEngineLogFloat(valueName, value);
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A01C File Offset: 0x0000821C
		public static void LogValue(string valueName, int value)
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEngineLogInt(valueName, value);
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000A034 File Offset: 0x00008234
		public static void LogValue(string valueName, bool value)
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEngineLogBool(valueName, value);
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000A04C File Offset: 0x0000824C
		public static void LogValue(string valueName, Vector3 value)
		{
			if ((UwaProfiler.TrackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEngineLogVector(valueName, value.x, value.y, value.z);
			}
		}

		// Token: 0x040000A8 RID: 168
		private const string PluginName = "uwa";

		// Token: 0x040000A9 RID: 169
		private static bool _worked = false;

		// Token: 0x040000AA RID: 170
		private static bool _mono_worked = false;

		// Token: 0x040000AB RID: 171
		private static string _dataPath = "";

		// Token: 0x040000AC RID: 172
		public static bool SupportFilter = false;

		// Token: 0x040000AD RID: 173
		private static bool _newProfiler = false;

		// Token: 0x040000AE RID: 174
		private static bool _renderingSync = false;

		// Token: 0x040000AF RID: 175
		public static UwaProfiler.Mode TrackMode;

		// Token: 0x040000B0 RID: 176
		private static UwaProfiler.DataMeta m = default(UwaProfiler.DataMeta);

		// Token: 0x040000B1 RID: 177
		private const int RenderingSyncEventID = 20202;

		// Token: 0x040000B2 RID: 178
		private const int RenderingStartEventID = 20201;

		// Token: 0x040000B3 RID: 179
		private static readonly string[] NameSpaceFilter = new string[]
		{
			"System",
			"ProtoBuf"
		};

		// Token: 0x040000B4 RID: 180
		private static readonly string[] ClassNameFilter = new string[]
		{
			"string",
			"StringBuilder"
		};

		// Token: 0x040000B5 RID: 181
		private static readonly string[] MethodNameFilter = new string[]
		{
			"ToString"
		};

		// Token: 0x040000B6 RID: 182
		private static readonly string[] NameSpaceRemain = new string[]
		{
			"System.Threading",
			"System.Runtime"
		};

		// Token: 0x040000B7 RID: 183
		private static readonly string[] MethodNameRemain = new string[]
		{
			"Invoke",
			"DynamicInvoke"
		};

		// Token: 0x020000D2 RID: 210
		[Flags]
		public enum Mode
		{
			// Token: 0x0400058B RID: 1419
			Overview = 1,
			// Token: 0x0400058C RID: 1420
			Memory = 2,
			// Token: 0x0400058D RID: 1421
			Resources = 4,
			// Token: 0x0400058E RID: 1422
			Lua = 8,
			// Token: 0x0400058F RID: 1423
			Gpu = 16
		}

		// Token: 0x020000D3 RID: 211
		public struct DataMeta
		{
			// Token: 0x04000590 RID: 1424
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string PluginVersion;

			// Token: 0x04000591 RID: 1425
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string TestMode;

			// Token: 0x04000592 RID: 1426
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceModel;

			// Token: 0x04000593 RID: 1427
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string EngineVersion;

			// Token: 0x04000594 RID: 1428
			public int FrameCount;

			// Token: 0x04000595 RID: 1429
			public int Duration;

			// Token: 0x04000596 RID: 1430
			public int Platform;

			// Token: 0x04000597 RID: 1431
			public int Engine;

			// Token: 0x04000598 RID: 1432
			public int OverviewLua;

			// Token: 0x04000599 RID: 1433
			public int OverviewGpu;

			// Token: 0x0400059A RID: 1434
			public int OverviewMono;

			// Token: 0x0400059B RID: 1435
			public int OverviewResources;

			// Token: 0x0400059C RID: 1436
			public int Texture2DCount;

			// Token: 0x0400059D RID: 1437
			public int MeshCount;

			// Token: 0x0400059E RID: 1438
			public int AnimationClipCount;

			// Token: 0x0400059F RID: 1439
			public int AudioClipCount;

			// Token: 0x040005A0 RID: 1440
			public int FontCount;

			// Token: 0x040005A1 RID: 1441
			public int MaterialCount;

			// Token: 0x040005A2 RID: 1442
			public int MeshColliderCount;

			// Token: 0x040005A3 RID: 1443
			public int ParticleSystemCount;

			// Token: 0x040005A4 RID: 1444
			public int RenderTextureCount;

			// Token: 0x040005A5 RID: 1445
			public int ShaderCount;

			// Token: 0x040005A6 RID: 1446
			public int AssetBundleCount;

			// Token: 0x040005A7 RID: 1447
			public int TextAssetCount;

			// Token: 0x040005A8 RID: 1448
			public int SampleIdCount;

			// Token: 0x040005A9 RID: 1449
			public int SampleDepthMax;

			// Token: 0x040005AA RID: 1450
			public int MethodIdCount;

			// Token: 0x040005AB RID: 1451
			public int StackLineCount;
		}
	}
}
