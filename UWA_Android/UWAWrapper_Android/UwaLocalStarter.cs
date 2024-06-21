using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using UWA;
using UWA.Android;
using UWALocal;
using UWALocal.Reflect;
using UWASDK;
using UWAShared;

// Token: 0x02000008 RID: 8
internal class UwaLocalStarter : MonoBehaviour
{
	// Token: 0x0600003D RID: 61
	[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
	public static extern void UpdateGlobalFrameId(int frameId);

	// Token: 0x0600003E RID: 62
	[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
	public static extern void UWALowerMemory();

	// Token: 0x0600003F RID: 63 RVA: 0x000032C8 File Offset: 0x000014C8
	public static UwaLocalStarter Get()
	{
		return UwaLocalStarter._instance;
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000040 RID: 64 RVA: 0x000032D0 File Offset: 0x000014D0
	// (set) Token: 0x06000041 RID: 65 RVA: 0x000032D8 File Offset: 0x000014D8
	public UwaLocalStarter.eTestMode TestMode
	{
		get
		{
			return this._testMode;
		}
		private set
		{
			this._testMode = value;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000042 RID: 66 RVA: 0x000032E4 File Offset: 0x000014E4
	public AssetTrackManager AssetTrackMgr
	{
		get
		{
			return this._assetTrackManager;
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x000032EC File Offset: 0x000014EC
	public static void SetUWAKey(string key)
	{
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("SetUWAKey(" + key + ")");
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003310 File Offset: 0x00001510
	public static void SetServerIp(string ip, int port)
	{
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log(string.Concat(new string[]
			{
				"SetServerIp(",
				ip,
				",",
				port.ToString(),
				")"
			}));
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00003364 File Offset: 0x00001564
	public static void SetPluginsVersion(string version)
	{
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("SetPluginsVersion(" + version + ")");
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003388 File Offset: 0x00001588
	private void DoSomethingUpdate()
	{
	}

	// Token: 0x06000047 RID: 71 RVA: 0x0000338C File Offset: 0x0000158C
	private void DoSomeThingOnGUI()
	{
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003390 File Offset: 0x00001590
	private void Awake()
	{
		if (UwaLocalStarter._instance == null)
		{
			UwaLocalStarter._instance = this;
			this.InitWindowFunctions();
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			base.gameObject.name = "UWA_SERVICE";
			for (int i = 0; i <= 5; i++)
			{
				Dictionary<int, string> modeString = this._modeString;
				int key = i;
				UwaLocalStarter.eTestMode eTestMode = (UwaLocalStarter.eTestMode)i;
				modeString.Add(key, eTestMode.ToString());
			}
			SharedUtils.ShowLog |= WrapperTool.CheckPersistFile("uwa_log");
			SharedUtils.Log2File = SharedUtils.ShowLog;
			Application.lowMemory += new Application.LowMemoryCallback(UwaLocalStarter.UWALowerMemory);
			this.pkgUrl = UWALocal.CoreUtils.PkgName;
			if (this.pkgUrl.Length >= 50)
			{
				this.pkgUrl = this.pkgUrl.Substring(0, 50);
			}
			UWACoreConfig.KEY = this.pkgUrl + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			UWACoreConfig.PLUGIN_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			UwaLocalState.CheckNative(249);
			LuaTrackManager.StaticInit();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("LuaTrackManager.StaticInit: " + LuaTrackManager.LuaDetected.ToString());
			}
			UWA.Android.UWAEngine._uploadInstance = new Action<Action<bool>, object[]>(DataUploader.TryOneKeyUpload);
			UWA.Android.UWAEngine._noteCall = new Action<string>(DataUploader.Note);
			if (WrapperTool.DirectMode == WrapperTool.eDirectMode.Overview || UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Overview)
			{
				this._testMode = UwaLocalStarter.eTestMode.Overview;
			}
			if (WrapperTool.DirectMode == WrapperTool.eDirectMode.Mono || UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Mono)
			{
				this._testMode = UwaLocalStarter.eTestMode.Mono;
			}
			if (WrapperTool.DirectMode == WrapperTool.eDirectMode.Resources || UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Resources)
			{
				this._testMode = UwaLocalStarter.eTestMode.Resources;
			}
			if (WrapperTool.DirectMode == WrapperTool.eDirectMode.Lua || UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Lua)
			{
				this._testMode = UwaLocalStarter.eTestMode.Lua;
			}
			if (WrapperTool.DirectMode == WrapperTool.eDirectMode.Gpu || UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.GPU)
			{
				this._testMode = UwaLocalStarter.eTestMode.GPU;
			}
			UwaLocalState.CheckScriptBackend();
			SharedUtils.Log("UWA_SDKv" + UWACoreConfig.PLUGIN_VERSION + " Awake");
			SharedUtils.SetupFinalPath();
			SharedUtils.ClearFinalPath();
			DataUploader.CheckOldData();
			base.StartCoroutine(this.DelayCheckPoco(5));
			base.StartCoroutine(this.DelayProviderConfig(10));
			this.FeatureCheckForUI();
			new GameObject("UWAUIMgr")
			{
				transform = 
				{
					parent = base.gameObject.transform
				}
			}.AddComponent<SdkUIMgr>();
			GUITool.DefaultGuiColor = GUI.backgroundColor;
			if (SdkCtrlData.Instance.SdkMode != eSdkMode.None)
			{
				this.GotoMode();
			}
			this.LoadText();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Awake Done");
			}
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x0000365C File Offset: 0x0000185C
	private void FeatureCheckForUI()
	{
		if (UWALocal.CoreUtils.NoProfilerCallback())
		{
			SdkUIMgr.RemoveFeature("time_line");
		}
		if (SharedUtils.Il2Cpp() && UWALocal.CoreUtils.NoIl2cppStackTraceCallback())
		{
			SdkUIMgr.RemoveFeature("mono");
		}
		if (!SharedUtils.Il2Cpp() && UWALocal.CoreUtils.NoMonoObjectLiveCheck())
		{
			SdkUIMgr.RemoveFeature("mono");
		}
		if (!LuaTrackManager.LuaDetected)
		{
			SdkUIMgr.RemoveFeature("lua");
		}
		if (!SharedUtils.MatPropTools.Support() || !SharedUtils.MeshUVDMTools.Support())
		{
			SdkUIMgr.RemoveFeature("gpu_res");
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000036F0 File Offset: 0x000018F0
	private void GotoOnline()
	{
		SdkCtrlData.Instance.GotMode = eGotMode.None;
		SdkCtrlData.Instance.SdkMode = eSdkMode.None;
		WrapperTool.OnlineMode = true;
		Object.Destroy(this);
		Object.Destroy(SdkUIMgr.Get());
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003720 File Offset: 0x00001920
	private void GotoMode()
	{
		if (SdkCtrlData.Instance.SdkMode == eSdkMode.GPM)
		{
			SdkStartConfig.Store();
			SdkStartConfig.Instance.SetDefault();
		}
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("SdkCtrlData.Instance.SdkMode " + SdkCtrlData.Instance.SdkMode.ToString());
		}
		if (SdkUIMgr.Get())
		{
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.GPM)
			{
				this.TestState = UwaLocalStarter.eTestState.Starting;
				this._testMode = UwaLocalStarter.eTestMode.Overview;
				return;
			}
			SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SELECT);
			SdkUIMgr.Get().OldPos = new Vector2(float.MinValue, float.MinValue);
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x000037D0 File Offset: 0x000019D0
	public IEnumerator DelayRuntimeFeatureCheck(int time)
	{
		int num;
		for (int i = 0; i < time; i = num + 1)
		{
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x000037E0 File Offset: 0x000019E0
	public IEnumerator DelayProviderConfig(int time)
	{
		int num;
		for (int i = 0; i < time; i = num + 1)
		{
			yield return null;
			num = i;
		}
		string config = Application.persistentDataPath + "/gotConfig.json";
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("Start CheckProviderConfig : " + config);
		}
		if (UwaLocalState.CheckProviderConfig(config))
		{
			yield break;
		}
		while (this._testMode == UwaLocalStarter.eTestMode.UnSelect)
		{
			if (Time.frameCount % 30 == 0)
			{
				try
				{
					if (UwaLocalState.CheckProviderConfig(config))
					{
						yield break;
					}
				}
				catch (Exception)
				{
					yield break;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x000037F8 File Offset: 0x000019F8
	private IEnumerator DelayCheckPoco(int time)
	{
		int num;
		for (int i = 0; i < time; i = num + 1)
		{
			yield return null;
			num = i;
		}
		if (UWA.Android.GUIWrapper.ControlByPoco)
		{
			SdkCtrlData.Instance.PocoConnected = true;
		}
		if (!PocoTool.CheckPoco())
		{
			yield break;
		}
		bool hooked = false;
		try
		{
			hooked = PocoTool.AddGOTRPC();
			goto IL_E6;
		}
		catch (Exception)
		{
			yield break;
		}
		IL_AA:
		if (Time.frameCount % 60 == 0)
		{
			try
			{
				hooked = PocoTool.AddGOTRPC();
			}
			catch (Exception)
			{
				yield break;
			}
		}
		yield return null;
		IL_E6:
		if (hooked || this._testMode != UwaLocalStarter.eTestMode.UnSelect)
		{
			yield break;
		}
		goto IL_AA;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003810 File Offset: 0x00001A10
	internal static void StartWithPoco(string mode, string config)
	{
		if (DataUploader.s == DataUploader.UploadState.Preparing)
		{
			DataUploader.s = DataUploader.UploadState.Close;
			UwaLocalStarter.Get().ResetSDK();
		}
		if (UwaLocalStarter.Get().TestState != UwaLocalStarter.eTestState.Waiting)
		{
			return;
		}
		if (DataUploader.s != DataUploader.UploadState.ShowOld && DataUploader.s != DataUploader.UploadState.DeleteOld && DataUploader.s != DataUploader.UploadState.Idle && DataUploader.s != DataUploader.UploadState.Done)
		{
			return;
		}
		DataUploader.s = DataUploader.UploadState.Idle;
		UwaLocalState.pocoStart = true;
		SdkStartConfig.Instance.SetDefault();
		if (config != null)
		{
			UWA.Android.UWAEngine.SetConfig(config);
		}
		SdkStartConfig.Store();
		try
		{
			UWA.Android.UWAEngine.Mode mode2 = (UWA.Android.UWAEngine.Mode)Enum.Parse(typeof(UWA.Android.UWAEngine.Mode), mode, true);
			if (Enum.IsDefined(typeof(UWA.Android.UWAEngine.Mode), mode2) && mode2 != UWA.Android.UWAEngine.Mode.Unset)
			{
				if (!SharedUtils.Dev)
				{
					UWAPanel.ConfigInst.SetOverviewConfigMode(overview.ConfigMode.Minimal);
					SdkStartConfig.Store();
					UWA.Android.UWAEngine.Start(UWA.Android.UWAEngine.Mode.Overview);
				}
				else
				{
					UWA.Android.UWAEngine.Start(mode2);
				}
				return;
			}
		}
		catch (Exception)
		{
		}
		try
		{
			eSdkMode eSdkMode = (eSdkMode)Enum.Parse(typeof(eSdkMode), mode, true);
			if (Enum.IsDefined(typeof(eSdkMode), eSdkMode) && eSdkMode == eSdkMode.GPM)
			{
				UWA.Android.UWAEngine.StartGPM();
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003978 File Offset: 0x00001B78
	internal static void StopWithPoco()
	{
		if (UwaLocalStarter.Get().TestState != UwaLocalStarter.eTestState.Recording)
		{
			return;
		}
		UWA.Android.UWAEngine.Stop();
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000051 RID: 81 RVA: 0x00003990 File Offset: 0x00001B90
	// (set) Token: 0x06000052 RID: 82 RVA: 0x00003998 File Offset: 0x00001B98
	public UwaLocalStarter.eTestState TestState
	{
		get
		{
			return this._testState;
		}
		private set
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("TestState " + this._testState.ToString() + " > " + value.ToString());
			}
			this._testState = value;
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000039EC File Offset: 0x00001BEC
	private void UIUpdate()
	{
		if (SdkCtrlData.Instance.TryPA)
		{
			this.GotoOnline();
			SdkCtrlData.Instance.TryPA = false;
		}
		if (SdkCtrlData.Instance.TryGOT)
		{
			this.GotoMode();
			SdkCtrlData.Instance.TryGOT = false;
		}
		if (SdkCtrlData.Instance.GPMTest)
		{
			this.GotoMode();
			SdkCtrlData.Instance.GPMTest = false;
		}
		if (this.TestState == UwaLocalStarter.eTestState.Waiting && SdkCtrlData.Instance.TryStart)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UIUpdate Start : " + SdkCtrlData.Instance.GotMode.ToString());
			}
			try
			{
				UWA.Android.UWAEngine._mode = (UWA.Android.UWAEngine.Mode)SdkCtrlData.Instance.GotMode;
			}
			catch
			{
			}
			SdkCtrlData.Instance.TryStart = false;
		}
		if (this.TestState == UwaLocalStarter.eTestState.Recording && SdkCtrlData.Instance.TryStop)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UIUpdate Stop : " + SdkCtrlData.Instance.GotMode.ToString());
			}
			UWA.Android.UWAEngine._stop = true;
			SdkCtrlData.Instance.TryStop = false;
		}
		if (this.TestState == UwaLocalStarter.eTestState.Recording && SdkCtrlData.Instance.TryDump != eDumpType.None)
		{
			if ((SdkCtrlData.Instance.TryDump & eDumpType.ManagedHeap) != eDumpType.None)
			{
				MonoTrackManager.MonoDumpHelper.WaitToDump = true;
			}
			if ((SdkCtrlData.Instance.TryDump & eDumpType.Lua) != eDumpType.None)
			{
				LuaTrackManager.LuaDumpHelper.WaitToDump = true;
			}
			if ((SdkCtrlData.Instance.TryDump & eDumpType.Resources) != eDumpType.None)
			{
				AssetTrackManager.AssetDumpHelper.WaitToDump = true;
			}
			if ((SdkCtrlData.Instance.TryDump & eDumpType.Overdraw) != eDumpType.None)
			{
				GpuTrackManager.GpuDumpHelper.WaitToDump = true;
			}
			SdkCtrlData.Instance.TryDump = eDumpType.None;
		}
		if (this.TestState == UwaLocalStarter.eTestState.Waiting && SdkCtrlData.Instance.TryExit)
		{
			Object.Destroy(this);
			Object.Destroy(SdkUIMgr.Get());
			SdkCtrlData.Instance.TryExit = false;
		}
		if (SdkUIMgr.Get())
		{
			SdkUIMgr.Get().Interactable = !SdkCtrlData.Instance.PocoConnected;
			SdkUIMgr.Get().PocoConnected(SdkCtrlData.Instance.PocoConnected);
			SdkUIMgr.Get().Show = UWA.Android.UWAEngine._uiActive;
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003C48 File Offset: 0x00001E48
	private void LoadText()
	{
		SdkUIMgr.Get().infoStates.Clear();
		SdkUIMgr.Get().BufInfoStates.Clear();
		bool flag = UWALocal.CoreUtils.NoGPUCounterDeviceSupport();
		bool flag2 = UWALocal.CoreUtils.NoGPUCounterRuntimeSupport();
		string featureInfo = UWALocal.CoreUtils.GetFeatureInfo("gpu_counter_error");
		bool flag3 = UWALocal.CoreUtils.NoSocInfoSupport();
		string featureInfo2 = UWALocal.CoreUtils.GetFeatureInfo("soc_info_error");
		if (PlayerPrefs.GetInt("GpuAndSocNotPrompting") != 1 && (!string.IsNullOrEmpty(featureInfo) || !string.IsNullOrEmpty(featureInfo2)))
		{
			string str = "\n\n具体原因请查看 SDK 附带文档中“本地测试-注意事项”部分。";
			string text = "";
			if ((flag || flag2) && flag3)
			{
				text = "本次测试将无法获取 GPU 频率相关及性能指标数据。";
			}
			else if ((flag2 || flag2) && !flag3)
			{
				text = "本次测试将无法获取 GPU 性能指标数据。";
			}
			else if (!flag2 && !flag2 && flag3)
			{
				text = "本次测试将无法获取 GPU 频率相关信息。";
			}
			if (text.Length > 0)
			{
				SdkUIMgr.Get().infoStates.Add(new KeyValuePair<SdkUIMgr.InfoState, string>(SdkUIMgr.InfoState.GPUAndSoc, text + str));
			}
		}
		bool flag4 = UWALocal.CoreUtils.NoScreenshotSupport();
		if (flag4)
		{
			SdkUIMgr.Get().ShowDialog("本次测试将无法获取到屏幕截图。\n请检查 SDK 中 aar 文件是否正确集成。\n\n具体请查看文档“本地测试-注意事项”部分。");
		}
		if (PlayerPrefs.GetInt("StackNotPrompting") != 1)
		{
			string text2 = "关闭堆栈获取后，可显著降低SDK额外开销，\n提高硬件数据准确性，但会屏蔽CPU堆栈的获取，\n包括各个引擎模块、逻辑代码的堆栈，卡顿帧堆栈等，\n是否继续测试？";
			if (text2.Length > 0)
			{
				SdkUIMgr.Get().infoStates.Add(new KeyValuePair<SdkUIMgr.InfoState, string>(SdkUIMgr.InfoState.Stack, text2));
			}
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003D9C File Offset: 0x00001F9C
	private void ResetSDK()
	{
		BaseTrackerManager.ClearAll();
		this._monoTrackManager = null;
		this._assetTrackManager = null;
		this._GpuTrackManager = null;
		this._screenTrackManager = null;
		ScreenTrackManager.Clear_screenTrackManager();
		this._logTrackManager = null;
		this._luaTrackManager = null;
		this._unitySamplerTrackingManager = null;
		if (DataUploader.s == DataUploader.UploadState.Close)
		{
			DataUploader.s = DataUploader.UploadState.Idle;
		}
		UWACoreConfig.KEY = this.pkgUrl + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
		SharedUtils.SetupFinalPath();
		UWALocal.CoreUtils.Reset();
		UwaProfiler.SetWorked(false);
		UwaLocalState.ClearLastSceneName();
		SdkStartConfig.Load();
		this._testState = UwaLocalStarter.eTestState.Waiting;
		this._testMode = UwaLocalStarter.eTestMode.UnSelect;
		UWA.Android.UWAEngine._mode = UWA.Android.UWAEngine.Mode.Unset;
		SdkUIMgr.Get().OldPos = new Vector2(float.MinValue, float.MinValue);
		this.LoadText();
		SdkUIMgr.Get().ChangeMode(eGotMode.Overview);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003E7C File Offset: 0x0000207C
	private void Update()
	{
		if (this.testTimeTip > 0f)
		{
			this.testTimeTip -= Time.unscaledDeltaTime;
		}
		UwaLocalState.UpdateMainThreadActions();
		if ((DataUploader.s == DataUploader.UploadState.Done && this.TestState != UwaLocalStarter.eTestState.Waiting) || DataUploader.s == DataUploader.UploadState.Close)
		{
			this.ResetSDK();
		}
		this.UIUpdate();
		if (this.TestState == UwaLocalStarter.eTestState.Recording && UWA.Android.UWAEngine._stop)
		{
			this.TestState = UwaLocalStarter.eTestState.Stopping;
			UWA.Android.UWAEngine._stop = false;
		}
		Cursor.lockState = 0;
		if (this._testMode == UwaLocalStarter.eTestMode.UnSelect && UWA.Android.UWAEngine._mode != UWA.Android.UWAEngine.Mode.Unset)
		{
			if (UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Overview)
			{
				this._testMode = UwaLocalStarter.eTestMode.Overview;
			}
			if (UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Resources)
			{
				this._testMode = UwaLocalStarter.eTestMode.Resources;
			}
			if (UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Mono)
			{
				this._testMode = UwaLocalStarter.eTestMode.Mono;
			}
			if (UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.Lua)
			{
				this._testMode = UwaLocalStarter.eTestMode.Lua;
			}
			if (UWA.Android.UWAEngine._mode == UWA.Android.UWAEngine.Mode.GPU)
			{
				this._testMode = UwaLocalStarter.eTestMode.GPU;
			}
		}
		if (this._testMode != UwaLocalStarter.eTestMode.UnSelect && this.TestState == UwaLocalStarter.eTestState.Waiting)
		{
			this.TestState = UwaLocalStarter.eTestState.Starting;
		}
		if (this.TestState == UwaLocalStarter.eTestState.Starting)
		{
			this._screenTrackManager = ScreenTrackManager.Get();
			switch (this._testMode)
			{
			case UwaLocalStarter.eTestMode.Overview:
				if (SharedUtils.Dev)
				{
					if (SdkStartConfig.Instance.overview.resources == 2)
					{
						this._assetTrackManager = new AssetTrackManager(AssetTrackManager.Mode.complete, ".at");
					}
					else if (SdkStartConfig.Instance.overview.resources == 1)
					{
						this._assetTrackManager = new AssetTrackManager(AssetTrackManager.Mode.simple, ".at");
					}
				}
				this._logTrackManager = new LogTrackManager(".lg");
				this._luaTrackManager = LuaTrackManager.Get("");
				LuaTrackManager.LuaDumpHelper.AutoDump = false;
				if (SdkStartConfig.Instance.overview.lua)
				{
					LuaTrackManager.LuaDumpHelper.Setup(SdkStartConfig.Instance.overview.lua_dump_step);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("LuaInOverview DumpInterval100 : " + LuaTrackManager.LuaDumpHelper.DumpInterval100.ToString());
					}
				}
				if (UWALocal.CoreUtils.NoProfilerCallback() && SharedUtils.Dev && SdkStartConfig.Instance.overview.engine_cpu_stack)
				{
					if (SharedUtils.Il2Cpp())
					{
						this._unitySamplerTrackingManager = new UnitySamplerReflectTrackingManager("", 200);
					}
					else
					{
						this._unitySamplerTrackingManager = new UnitySamplerTrackingManager("", 200);
					}
				}
				if (!UWALocal.CoreUtils.NoProfilerCallback())
				{
					Profiler.enabled = true;
				}
				this._monoTrackManager = new MonoTrackManager(UwaProfiler.Mode.Overview, "");
				break;
			case UwaLocalStarter.eTestMode.Mono:
				this._monoTrackManager = new MonoTrackManager(UwaProfiler.Mode.Memory, "");
				MonoTrackManager.MonoDumpHelper.Setup(SdkStartConfig.Instance.mono.mono_dump_step);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("MonoTrackManager DumpInterval100 : " + MonoTrackManager.MonoDumpHelper.DumpInterval100.ToString());
				}
				break;
			case UwaLocalStarter.eTestMode.Resources:
				this._assetTrackManager = new AssetTrackManager(AssetTrackManager.Mode.complete, ".at");
				this._monoTrackManager = new MonoTrackManager(UwaProfiler.Mode.Resources, "");
				break;
			case UwaLocalStarter.eTestMode.Lua:
				this._luaTrackManager = LuaTrackManager.Get("");
				LuaTrackManager.LuaDumpHelper.Setup(SdkStartConfig.Instance.lua.lua_dump_step);
				this._monoTrackManager = new MonoTrackManager(UwaProfiler.Mode.Lua, "");
				break;
			case UwaLocalStarter.eTestMode.GPU:
				if (SdkStartConfig.Instance.gpu.texture_analysis && SdkStartConfig.Instance.gpu.mesh_analysis)
				{
					GpuTrackManager.GpuModeFeature = GpuTrackManager.Feature.eAllFeatureTrack;
				}
				else if (SdkStartConfig.Instance.gpu.texture_analysis)
				{
					GpuTrackManager.GpuModeFeature = GpuTrackManager.Feature.eTextureFeatureTrack;
				}
				else if (SdkStartConfig.Instance.gpu.mesh_analysis)
				{
					GpuTrackManager.GpuModeFeature = GpuTrackManager.Feature.eMeshFeatureTrack;
				}
				else
				{
					GpuTrackManager.GpuModeFeature = GpuTrackManager.Feature.eNone;
				}
				this._GpuTrackManager = new GpuTrackManager("");
				this._monoTrackManager = new MonoTrackManager(UwaProfiler.Mode.Gpu, "");
				break;
			}
			UwaProfiler.UWAEngineInternalEnterProfCpuProfiler("UWA.Overhead");
			BaseTrackerManager.PrepareAll();
			UwaProfiler.UWAEngineInternalLeaveProfCpuProfiler();
			SdkUIMgr.Get().ClearDialog();
			SdkUIMgr.Get().ClearMsg();
			DataUploader.s = DataUploader.UploadState.Idle;
			this.TestState = UwaLocalStarter.eTestState.Preparing;
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PrepareAll");
			}
		}
		bool flag = this._testMode == UwaLocalStarter.eTestMode.Overview || this._testMode == UwaLocalStarter.eTestMode.Lua;
		if (this.TestState == UwaLocalStarter.eTestState.Preparing && (!flag || BaseTrackerManager.IsAllReady(out this._errorCode)))
		{
			if (SdkUIMgr.Get())
			{
				SdkUIMgr.Get().NotWork &= ~SdkUIMgr.UINotWork.HasError;
			}
			SharedUtils.frameId = 0;
			UWA.Android.UWAEngine.FrameId = 0;
			SharedUtils.durationS = 0;
			UWA.Android.UWAEngine.DurationS = 0;
			if (SdkUIMgr.Get())
			{
				SdkUIMgr.Get().DurationS = 0;
			}
			UWALocal.CoreUtils.StartTime = Time.realtimeSinceStartup;
			this.DumpSystemInfo();
			this.DumpLicense();
			this.DumpConfig();
			this.UpdateSceneName();
			if (this._monoTrackManager != null)
			{
				this._monoTrackManager.Init(UwaLocalStarter.LocalConfig10);
			}
			if (this._GpuTrackManager != null)
			{
				this._GpuTrackManager.Init(UwaLocalStarter.LocalConfig1);
			}
			if (this._assetTrackManager != null)
			{
				this._assetTrackManager.Init(UwaLocalStarter.LocalConfig2);
			}
			if (this._screenTrackManager != null)
			{
				this._screenTrackManager.Init(UwaLocalStarter.LocalConfig2);
			}
			if (this._logTrackManager != null)
			{
				this._logTrackManager.Init(UwaLocalStarter.LocalConfig2);
			}
			if (this._luaTrackManager != null)
			{
				this._luaTrackManager.Init(UwaLocalStarter.LocalConfig10);
			}
			if (this._unitySamplerTrackingManager != null)
			{
				this._unitySamplerTrackingManager.Init(UwaLocalStarter.LocalConfig10);
			}
			this.EnableTrackers();
			UwaLocalStarter.UpdateGlobalFrameId(SharedUtils.frameId);
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("EnableTrackers");
			}
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.GPM)
			{
				UWALocal.CoreUtils.WriteStringToFile("GPM", SharedUtils.FinalDataPath + "/mode");
			}
			else
			{
				UWALocal.CoreUtils.WriteStringToFile(this._testMode.ToString(), SharedUtils.FinalDataPath + "/mode");
			}
			this.TestState = UwaLocalStarter.eTestState.Recording;
			this.testTimeTip = 3f;
			base.StartCoroutine(this.UWAStarter_EndOfFrame());
			UwaLocalState.onLevelChanged = new UwaLocalState.OnLevelChange(this.OnLevelChanged);
			if (SdkUIMgr.Get())
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SERVICE);
			}
		}
		if (this.TestState == UwaLocalStarter.eTestState.Recording)
		{
			SharedUtils.frameId++;
			UWA.Android.UWAEngine.FrameId++;
			if (SharedUtils.frameId == 1 && this._assetTrackManager != null && this._assetTrackManager.Enabled)
			{
				this._assetTrackManager.MakeNextGetAllAssets();
			}
			this.UpdateSceneName();
			UwaLocalStarter.UpdateGlobalFrameId(SharedUtils.frameId);
			this.CheckTimeAndSwitchLogFile();
			this._flagWriteCurrent += Time.unscaledDeltaTime;
			if (this._flagWriteCurrent > this._flagWriteInterval)
			{
				UWALocal.CoreUtils.WriteLocalEndingFlag();
				this._flagWriteCurrent = 0f;
			}
			int num = (int)(Time.realtimeSinceStartup - UWALocal.CoreUtils.StartTime);
			if (SharedUtils.durationStr == null || SharedUtils.durationS != num)
			{
				if (SdkUIMgr.Get())
				{
					SdkUIMgr.Get().DurationS = num;
				}
				SharedUtils.durationS = num;
				UWA.Android.UWAEngine.DurationS = num;
				int num2 = num / 60;
				int num3 = num % 60;
				SharedUtils.durationStr = ((num2 > 0) ? (num2.ToString() + "' ") : "") + num3.ToString() + "\"";
			}
		}
		if (this.TestState == UwaLocalStarter.eTestState.Stopping)
		{
			this.DisableTrackers();
			UWALocal.CoreUtils.WriteLocalEndingFlag();
			this.TestState = UwaLocalStarter.eTestState.Stopped;
			DataUploader.TestType = SdkCtrlData.Instance.SdkMode;
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UploadSetup");
			}
			DataUploader.UploadSetup(SharedUtils.FinalDataPath);
			if (SdkUIMgr.Get())
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.STOPPED);
			}
		}
		if (this.TestState == UwaLocalStarter.eTestState.Stopped && DataUploader.StartOKU)
		{
			DataUploader.StartOKU = false;
			DataUploader.UploadSetup(SharedUtils.FinalDataPath);
			DataUploader.DoOneKeyUpload();
		}
		if (DataUploader.s != DataUploader.UploadState.Idle)
		{
			UploadTool.Get().UpdateFrame();
		}
		this.DoSomethingUpdate();
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000046E0 File Offset: 0x000028E0
	private void EnableTrackers()
	{
		BaseTrackerManager.StartTrackAll();
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000046E8 File Offset: 0x000028E8
	private void DisableTrackers()
	{
		BaseTrackerManager.StopTrackAll();
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000046F0 File Offset: 0x000028F0
	private void CheckTimeAndSwitchLogFile()
	{
		UwaProfiler.UWAEngineInternalEnterProfCpuProfiler("UWA.Overhead");
		BaseTrackerManager.CheckTimeAndSwitchLogFileAll();
		UwaProfiler.UWAEngineInternalLeaveProfCpuProfiler();
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00004708 File Offset: 0x00002908
	private void LateUpdate()
	{
		UwaProfiler.UWAEngineInternalEnterProfCpuProfiler("UWA.Overhead");
		BaseTrackerManager.LateUpdateAll();
		UwaProfiler.UWAEngineInternalLeaveProfCpuProfiler();
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00004720 File Offset: 0x00002920
	private IEnumerator UWAStarter_EndOfFrame()
	{
		yield return this._endOfFrame;
		while (this.TestState == UwaLocalStarter.eTestState.Recording)
		{
			UwaProfiler.UWAEngineInternalEnterProfCpuProfiler("UWA.Overhead");
			try
			{
				BaseTrackerManager.UpdateAtEndAll();
			}
			catch (Exception ex)
			{
				SharedUtils.LogError(ex.ToString());
			}
			UwaProfiler.UWAEngineInternalLeaveProfCpuProfiler();
			yield return this._endOfFrame;
		}
		yield break;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00004730 File Offset: 0x00002930
	private void InitWindowFunctions()
	{
		UwaLocalState.WFInited = true;
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00004738 File Offset: 0x00002938
	private void OnGUI()
	{
		if (!UwaLocalState.WFInited)
		{
			return;
		}
		this.DoSomeThingOnGUI();
		if (this.testTimeTip > 0f && this.TestState == UwaLocalStarter.eTestState.Recording)
		{
			GUI.Label(SharedUtils.TipsRect, "为获得完整的测试数据，\n建议测试时间不少于60秒。", GUITool.textFieldTipGuiStyle);
		}
		if (this._testMode == UwaLocalStarter.eTestMode.UnSelect)
		{
			Color color = GUI.color;
			GUI.color = Color.red;
			int fontSize = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = (int)((float)SharedUtils.GroupHeight * 0.2f);
			bool flag = UwaLocalState.DoOnGUI();
			GUI.color = color;
			GUI.skin.button.fontSize = fontSize;
			if (SdkUIMgr.Get())
			{
				if (flag)
				{
					SdkUIMgr.Get().NotWork |= SdkUIMgr.UINotWork.HasError;
				}
				else
				{
					SdkUIMgr.Get().NotWork &= ~SdkUIMgr.UINotWork.HasError;
				}
			}
		}
		if (this.TestState == UwaLocalStarter.eTestState.Preparing && this._errorCode != null)
		{
			Color color2 = GUI.color;
			GUI.color = Color.red;
			int fontSize2 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = (int)((float)SharedUtils.GroupHeight * 0.2f);
			GUILayout.Button(this._errorCode, new GUILayoutOption[0]);
			GUI.color = color2;
			GUI.skin.button.fontSize = fontSize2;
			if (SdkUIMgr.Get())
			{
				SdkUIMgr.Get().NotWork |= SdkUIMgr.UINotWork.HasError;
			}
		}
		UwaLocalStarter.eTestState testState = this.TestState;
	}

	// Token: 0x0600005E RID: 94 RVA: 0x000048D4 File Offset: 0x00002AD4
	private void OnDestroy()
	{
		if (UwaLocalStarter._instance == this)
		{
			UwaLocalStarter._instance = null;
		}
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("UWALocalStarter OnDestroy");
		}
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00004900 File Offset: 0x00002B00
	private void OnApplicationPause(bool pauseStatus)
	{
		if (SharedUtils.ShowLog)
		{
			SharedUtils.Log("UWALocalStarter OnApplicationPause " + pauseStatus.ToString());
		}
		if (pauseStatus)
		{
			this.isPause = true;
			UWA.Android.UWAEngine.PushSample("ApplicationPause");
			return;
		}
		if (this.isPause)
		{
			UWA.Android.UWAEngine.PopSample();
			this.isPause = false;
		}
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004960 File Offset: 0x00002B60
	private void DoPocoTips(int windowId)
	{
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 4;
		GUILayout.Label("UWA GOT SDK driven by Poco.", SharedUtils.GetSuitableOption(0.8f, 0.8f));
		GUI.skin.label.alignment = alignment;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000049BC File Offset: 0x00002BBC
	public void UpdateSceneName()
	{
		UwaLocalState.UpdateSceneName();
	}

	// Token: 0x06000062 RID: 98 RVA: 0x000049C4 File Offset: 0x00002BC4
	public static void ChangeTag(string tag)
	{
		if (string.IsNullOrEmpty(tag))
		{
			tag = "(null)";
		}
		if (UwaLocalStarter.lastTag == null && SharedUtils.frameId > 1)
		{
			if (UwaLocalState.onLevelChanged != null)
			{
				UwaLocalState.onLevelChanged();
			}
			string contents = "0,default\n";
			File.AppendAllText(SharedUtils.FinalDataPath + "/tags", contents);
		}
		if (tag != UwaLocalStarter.lastTag)
		{
			if (UwaLocalState.onLevelChanged != null)
			{
				UwaLocalState.onLevelChanged();
			}
			string contents2;
			if (UwaLocalStarter._instance.TestMode == UwaLocalStarter.eTestMode.GPU)
			{
				contents2 = SharedUtils.frameId.ToString() + "," + Uri.EscapeDataString(tag) + "\n";
			}
			else
			{
				contents2 = SharedUtils.frameId.ToString() + "," + tag + "\n";
			}
			File.AppendAllText(SharedUtils.FinalDataPath + "/tags", contents2);
			UwaLocalStarter.lastTag = tag;
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00004ABC File Offset: 0x00002CBC
	private void OnLevelChanged()
	{
		if (this._testState == UwaLocalStarter.eTestState.Recording && this._assetTrackManager != null && this._assetTrackManager.Enabled)
		{
			this._assetTrackManager.MakeNextGetAllAssets();
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00004AF0 File Offset: 0x00002CF0
	private void DumpSystemInfo()
	{
		using (StreamWriter streamWriter = new StreamWriter(SharedUtils.FinalDataPath + "/systemInfo"))
		{
			UwaLocalState.Date = DateTime.Now.ToString("yyyyMMddHHmmss");
			try
			{
				UwaProfiler.MarkStartTime(UwaLocalState.Date);
			}
			catch (Exception)
			{
			}
			streamWriter.WriteLine("StartTime:" + UwaLocalState.Date);
			streamWriter.WriteLine("PluginVersion:" + UWACoreConfig.PLUGIN_VERSION);
			streamWriter.WriteLine("ScriptBackend:" + (SharedUtils.Il2Cpp() ? "IL2CPP" : "MONO"));
			streamWriter.WriteLine("PackageName:" + UWALocal.CoreUtils.PkgName);
			streamWriter.WriteLine("AppName:" + UWALocal.CoreUtils.AppName);
			streamWriter.WriteLine("DevelopmentBuild:" + SharedUtils.Dev.ToString());
			streamWriter.WriteLine("VulkanSDK:" + UwaProfiler.IsFeatureSupport("vulkan").ToString());
			streamWriter.WriteLine("PocoMode:" + UwaLocalState.pocoStart.ToString());
			streamWriter.WriteLine("PocoStrip:" + UwaLocalState.pocoStrip.ToString());
			streamWriter.WriteLine("BundleVersion:" + UWALocal.CoreUtils.BundleVersionName);
			streamWriter.WriteLine("BundleVersionCode:" + UWALocal.CoreUtils.BundleVersionCode.ToString());
			streamWriter.WriteLine("TestMode:" + ((SdkCtrlData.Instance.SdkMode == eSdkMode.GPM) ? "GPM" : this._testMode.ToString()));
			streamWriter.WriteLine("UnityVersion:" + Application.unityVersion);
			streamWriter.WriteLine("Platform:" + Application.platform.ToString());
			streamWriter.WriteLine("Simulator:" + SharedUtils.IsSimulator().ToString());
			streamWriter.WriteLine("Hardware:" + UWALocal.CoreUtils.SocName);
			streamWriter.WriteLine("OperatingSystem:" + SystemInfo.operatingSystem);
			streamWriter.WriteLine("DeviceID:" + SystemInfo.deviceUniqueIdentifier);
			streamWriter.WriteLine("DeviceName:" + SystemInfo.deviceName);
			streamWriter.WriteLine("DeviceModel:" + SystemInfo.deviceModel.Replace("-", ""));
			streamWriter.WriteLine("DeviceType:" + SystemInfo.deviceType.ToString());
			streamWriter.WriteLine("GraphicsDeviceName:" + SystemInfo.graphicsDeviceName);
			streamWriter.WriteLine("GraphicsDeviceVersion:" + SystemInfo.graphicsDeviceVersion);
			streamWriter.WriteLine("GraphicsMemorySize:" + SystemInfo.graphicsMemorySize.ToString());
			streamWriter.WriteLine("ProcessorCount:" + UWALocal.CoreUtils.ProcessorCount.ToString());
			streamWriter.WriteLine("ProcessorFrequency:" + UWALocal.CoreUtils.ProcessorFrequency.ToString());
			streamWriter.WriteLine("ProcessorType:" + SystemInfo.processorType);
			streamWriter.WriteLine("SystemMemorySize:" + ((SystemInfo.systemMemorySize < 0) ? 0 : SystemInfo.systemMemorySize).ToString());
			streamWriter.WriteLine("SystemLanguage:" + Application.systemLanguage.ToString());
			streamWriter.WriteLine("ScreenResolution:" + Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString());
			streamWriter.WriteLine("DeviceResolution:" + Display.main.systemWidth.ToString() + "x" + Display.main.systemHeight.ToString());
			streamWriter.WriteLine("ScreenOrientation:" + Screen.orientation.ToString());
			streamWriter.WriteLine("MultiThreadedRendering:" + SystemInfo.graphicsMultiThreaded.ToString());
			streamWriter.WriteLine("SoC:" + UWALocal.CoreUtils.GetFeatureInfo("soc_name"));
			streamWriter.WriteLine("GPUMaxFreq:" + UWALocal.CoreUtils.GetFeatureInfo("gpu_max_freq"));
			streamWriter.Close();
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00004FB0 File Offset: 0x000031B0
	private void DumpConfig()
	{
		if (SdkCtrlData.Instance.SdkMode == eSdkMode.GPM)
		{
			try
			{
				File.WriteAllText(SharedUtils.GetGotJsonInternal(), SdkStartConfig.Json);
				return;
			}
			catch (Exception ex)
			{
				return;
			}
		}
		string gotJsonExternal = SharedUtils.GetGotJsonExternal();
		if (File.Exists(gotJsonExternal))
		{
			try
			{
				File.Copy(gotJsonExternal, SharedUtils.GetGotJsonInternal());
			}
			catch (Exception ex2)
			{
			}
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000502C File Offset: 0x0000322C
	private void DumpLicense()
	{
		HashSet<string> nameSpaces = UWALocal.CoreUtils.ExtractDllClass();
		using (StreamWriter streamWriter = new StreamWriter(SharedUtils.FinalDataPath + "/license"))
		{
			streamWriter.WriteLine(UWALocal.CoreUtils.GetLicense(nameSpaces));
			streamWriter.Close();
		}
	}

	// Token: 0x04000013 RID: 19
	public const int NATIVE_VERSION = 249;

	// Token: 0x04000014 RID: 20
	private static UwaLocalStarter _instance = null;

	// Token: 0x04000015 RID: 21
	private UwaLocalStarter.eTestMode _testMode = UwaLocalStarter.eTestMode.UnSelect;

	// Token: 0x04000016 RID: 22
	private readonly Dictionary<int, string> _modeString = new Dictionary<int, string>();

	// Token: 0x04000017 RID: 23
	private AssetTrackManager _assetTrackManager;

	// Token: 0x04000018 RID: 24
	private GpuTrackManager _GpuTrackManager;

	// Token: 0x04000019 RID: 25
	private ScreenTrackManager _screenTrackManager;

	// Token: 0x0400001A RID: 26
	private LogTrackManager _logTrackManager;

	// Token: 0x0400001B RID: 27
	private LuaTrackManager _luaTrackManager;

	// Token: 0x0400001C RID: 28
	private UnitySamplerTrackingManager _unitySamplerTrackingManager;

	// Token: 0x0400001D RID: 29
	private MonoTrackManager _monoTrackManager;

	// Token: 0x0400001E RID: 30
	private string pkgUrl = "";

	// Token: 0x0400001F RID: 31
	private UwaLocalStarter.eTestState _testState;

	// Token: 0x04000020 RID: 32
	private float testTimeTip;

	// Token: 0x04000021 RID: 33
	private string _errorCode;

	// Token: 0x04000022 RID: 34
	private static readonly Dictionary<string, string> LocalConfig1 = new Dictionary<string, string>
	{
		{
			"Enable",
			"true"
		},
		{
			"TimeInterval",
			"1"
		}
	};

	// Token: 0x04000023 RID: 35
	private static readonly Dictionary<string, string> LocalConfig2 = new Dictionary<string, string>
	{
		{
			"Enable",
			"true"
		},
		{
			"TimeInterval",
			"2"
		}
	};

	// Token: 0x04000024 RID: 36
	private static readonly Dictionary<string, string> LocalConfig10 = new Dictionary<string, string>
	{
		{
			"Enable",
			"true"
		},
		{
			"TimeInterval",
			"10"
		}
	};

	// Token: 0x04000025 RID: 37
	private static readonly Dictionary<string, string> LocalConfigMax = new Dictionary<string, string>
	{
		{
			"Enable",
			"true"
		},
		{
			"TimeInterval",
			"100000"
		}
	};

	// Token: 0x04000026 RID: 38
	private float _flagWriteInterval = 10f;

	// Token: 0x04000027 RID: 39
	private float _flagWriteCurrent = 11f;

	// Token: 0x04000028 RID: 40
	private readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();

	// Token: 0x04000029 RID: 41
	private GUI.WindowFunction DoPocoTipsWF;

	// Token: 0x0400002A RID: 42
	private bool isPause;

	// Token: 0x0400002B RID: 43
	private bool _checkingGraphicsTools;

	// Token: 0x0400002C RID: 44
	private bool _restart2Direct;

	// Token: 0x0400002D RID: 45
	private bool _checkingMono;

	// Token: 0x0400002E RID: 46
	private bool _checkingLua;

	// Token: 0x0400002F RID: 47
	private bool _checkingLuaInOverview;

	// Token: 0x04000030 RID: 48
	private string _intervalStr = "10";

	// Token: 0x04000031 RID: 49
	private static GUILayoutOption[] gpmviewOp = null;

	// Token: 0x04000032 RID: 50
	private static GUILayoutOption[] toolviewOp = null;

	// Token: 0x04000033 RID: 51
	private static GUILayoutOption[] toolviewOp2 = null;

	// Token: 0x04000034 RID: 52
	public static string lastSceneName = null;

	// Token: 0x04000035 RID: 53
	public static UwaLocalStarter.OnLevelChange onLevelChanged;

	// Token: 0x04000036 RID: 54
	private static string lastTag = null;

	// Token: 0x020000C8 RID: 200
	public enum eTestMode
	{
		// Token: 0x04000566 RID: 1382
		Overview,
		// Token: 0x04000567 RID: 1383
		Mono,
		// Token: 0x04000568 RID: 1384
		Resources,
		// Token: 0x04000569 RID: 1385
		Lua,
		// Token: 0x0400056A RID: 1386
		GPU,
		// Token: 0x0400056B RID: 1387
		UnSelect
	}

	// Token: 0x020000C9 RID: 201
	public enum eTestState
	{
		// Token: 0x0400056D RID: 1389
		Waiting,
		// Token: 0x0400056E RID: 1390
		Starting,
		// Token: 0x0400056F RID: 1391
		Preparing,
		// Token: 0x04000570 RID: 1392
		Recording,
		// Token: 0x04000571 RID: 1393
		Stopping,
		// Token: 0x04000572 RID: 1394
		Stopped
	}

	// Token: 0x020000CA RID: 202
	// (Invoke) Token: 0x0600088E RID: 2190
	public delegate void OnLevelChange();
}
