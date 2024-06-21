using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UWA;
using UWASDK;
using UWAShared;

// Token: 0x0200000A RID: 10
public class SdkUIMgr : MonoBehaviour
{
	// Token: 0x0600002C RID: 44 RVA: 0x00002F50 File Offset: 0x00001150
	public void SetFuncPtr(SdkUIMgr.FuncPtr func)
	{
		bool flag = this.funcptr == func;
		if (!flag)
		{
			this.funcptr = func;
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600002D RID: 45 RVA: 0x00002F84 File Offset: 0x00001184
	// (set) Token: 0x0600002E RID: 46 RVA: 0x00002FA4 File Offset: 0x000011A4
	public SdkUIMgr.UIState UiState
	{
		get
		{
			return this._uiState;
		}
		private set
		{
			bool flag = SharedUtils.ShowLog && this._uiState != value;
			if (flag)
			{
				SharedUtils.Log("UIState : " + this._uiState.ToString() + " -> " + value.ToString());
			}
			this._uiState = value;
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00003018 File Offset: 0x00001218
	public static SdkUIMgr Get()
	{
		return SdkUIMgr.instance;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00003038 File Offset: 0x00001238
	private void Awake()
	{
		bool flag = SdkUIMgr.instance != null;
		if (flag)
		{
			Object.Destroy(this);
		}
		else
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform);
			this.UIRoot = gameObject.transform;
			SdkUIMgr.instance = this;
			SdkStartConfig.Load();
			SdkStartConfig.Store();
			this.Init();
			this.ChangeState(SdkUIMgr.UIState.MODE);
			this.setMode = SdkUIMgr.SetMode.OVERVIEW;
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000030B8 File Offset: 0x000012B8
	private void Update()
	{
		bool flag = (float)Screen.width != this.lastScWidget;
		if (flag)
		{
			SdkEventSystem.DispatchEvent(SdkEventSystem.SdkEventType.SCREEN_CHANGED, null);
			this.lastScWidget = (float)Screen.width;
		}
		SdkUIMgr.UpdateMainThreadActions();
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00003100 File Offset: 0x00001300
	public static void UpdateMainThreadActions()
	{
		bool flag = SdkUIMgr.MainThreadActions.Count == 0;
		if (!flag)
		{
			object obj = SdkUIMgr.actionLockObj;
			lock (obj)
			{
				bool flag2 = SdkUIMgr.MainThreadActions.Count > 0;
				if (flag2)
				{
					for (int i = 0; i < SdkUIMgr.MainThreadActions.Count; i++)
					{
						bool flag3 = SdkUIMgr.MainThreadActions[i] != null;
						if (flag3)
						{
							SdkUIMgr.MainThreadActions[i]();
						}
					}
					SdkUIMgr.MainThreadActions.Clear();
				}
			}
		}
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000031BC File Offset: 0x000013BC
	private void OnDestroy()
	{
		bool flag = SdkUIMgr.instance == this;
		if (flag)
		{
			SdkUIMgr.instance = null;
		}
		bool flag2 = this.Panel != null;
		if (flag2)
		{
			Object.Destroy(this.Panel);
			this.Panel = null;
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00003210 File Offset: 0x00001410
	private void Init()
	{
		this.lastScWidget = (float)Screen.width;
		AppSdkInfo.ScriptBackend = (SharedUtils.Il2Cpp() ? "IL2CPP" : "MONO");
		AppSdkInfo.GraphicsApi = SystemInfo.graphicsDeviceType.ToString();
		AppSdkInfo.UnityVersion = Application.unityVersion;
		AppSdkInfo.SdkVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		this.Panel = base.gameObject.AddComponent<UWAPanel>();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00003298 File Offset: 0x00001498
	public void ShowDialog(string info)
	{
		bool flag = this.Panel != null;
		if (flag)
		{
			this.Panel.CreateInformation(info);
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000032CC File Offset: 0x000014CC
	public void ShowMsg(string info)
	{
		bool flag = this.Panel != null;
		if (flag)
		{
			this.Panel.CreateMessage(info);
		}
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00003300 File Offset: 0x00001500
	public void ClearDialog()
	{
		bool flag = this.Panel != null;
		if (flag)
		{
			this.Panel.ClearInformation();
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00003334 File Offset: 0x00001534
	public void ClearMsg()
	{
		bool flag = this.Panel != null;
		if (flag)
		{
			this.Panel.ClearMessage();
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00003368 File Offset: 0x00001568
	public void DoDump(eDumpType t)
	{
		SdkCtrlData.Instance.TryDump |= t;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00003380 File Offset: 0x00001580
	public static void RemoveFeature(string feature)
	{
		SdkStartConfig.Instance.RemoveFeat(feature);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003390 File Offset: 0x00001590
	public void SaveAndStart()
	{
		SdkCtrlData.Instance.TryStart = true;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000033A0 File Offset: 0x000015A0
	public void DirectRelaunch()
	{
		string path = Application.persistentDataPath + "/direct";
		File.WriteAllText(path, SdkCtrlData.Instance.GotMode.ToString());
		Application.Quit();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x000033E4 File Offset: 0x000015E4
	public void ChangeState(SdkUIMgr.UIState state)
	{
		this.UiState = state;
		SdkEventSystem.DispatchEvent(SdkEventSystem.SdkEventType.UISTATE_CHANGED, null);
	}

	// Token: 0x0600003E RID: 62 RVA: 0x000033F8 File Offset: 0x000015F8
	public void ChangeSetMode(SdkUIMgr.SetMode state)
	{
		this.setMode = state;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003404 File Offset: 0x00001604
	public SdkUIMgr.SetMode GetSetMode()
	{
		return this.setMode;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003424 File Offset: 0x00001624
	public void ChangeSdkMode(eSdkMode mode)
	{
		SdkCtrlData.Instance.SdkMode = mode;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00003434 File Offset: 0x00001634
	public void ChangeMode(eGotMode mode)
	{
		SdkCtrlData.Instance.GotMode = mode;
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003444 File Offset: 0x00001644
	public void TryStart()
	{
		SdkCtrlData.Instance.TryStart = true;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003454 File Offset: 0x00001654
	public void TryStop()
	{
		SdkCtrlData.Instance.TryStop = true;
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000044 RID: 68 RVA: 0x00003464 File Offset: 0x00001664
	// (set) Token: 0x06000045 RID: 69 RVA: 0x00003484 File Offset: 0x00001684
	public SdkUIMgr.UINotWork NotWork
	{
		get
		{
			return this._notWork;
		}
		set
		{
			bool flag = this._notWork == value;
			if (!flag)
			{
				this._notWork = value;
				bool flag2 = this.UIRoot && this.UIRoot.gameObject.activeInHierarchy != this.UIEnabled;
				if (flag2)
				{
					this.UIRoot.gameObject.SetActive(this.UIEnabled);
				}
			}
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000046 RID: 70 RVA: 0x00003504 File Offset: 0x00001704
	// (set) Token: 0x06000047 RID: 71 RVA: 0x00003528 File Offset: 0x00001728
	public bool Interactable
	{
		get
		{
			return UWAPanel.Inst.Interactable;
		}
		set
		{
			UWAPanel.Inst.Interactable = value;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000048 RID: 72 RVA: 0x00003538 File Offset: 0x00001738
	// (set) Token: 0x06000049 RID: 73 RVA: 0x00003558 File Offset: 0x00001758
	public bool Show
	{
		get
		{
			return this._show;
		}
		set
		{
			bool flag = this._show == value;
			if (!flag)
			{
				this._show = value;
				bool flag2 = this.UIRoot && this.UIRoot.gameObject.activeInHierarchy != this.UIEnabled;
				if (flag2)
				{
					this.UIRoot.gameObject.SetActive(this.UIEnabled);
				}
			}
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600004A RID: 74 RVA: 0x000035D8 File Offset: 0x000017D8
	// (set) Token: 0x0600004B RID: 75 RVA: 0x00003608 File Offset: 0x00001808
	public bool UIEnabled
	{
		get
		{
			return this._notWork == SdkUIMgr.UINotWork.None && this._show;
		}
		private set
		{
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600004C RID: 76 RVA: 0x0000360C File Offset: 0x0000180C
	// (set) Token: 0x0600004D RID: 77 RVA: 0x00003614 File Offset: 0x00001814
	public int DurationS { get; set; }

	// Token: 0x0600004E RID: 78 RVA: 0x00003620 File Offset: 0x00001820
	public bool ShowFeature(string feat)
	{
		return !SdkStartConfig.Instance.IsFeatRemoved(feat);
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003648 File Offset: 0x00001848
	public void PocoConnected(bool on)
	{
		UWAPanel.Inst.PocoOn = on;
	}

	// Token: 0x0400001F RID: 31
	public List<KeyValuePair<SdkUIMgr.InfoState, string>> infoStates = new List<KeyValuePair<SdkUIMgr.InfoState, string>>();

	// Token: 0x04000020 RID: 32
	public List<KeyValuePair<SdkUIMgr.InfoState, string>> BufInfoStates = new List<KeyValuePair<SdkUIMgr.InfoState, string>>();

	// Token: 0x04000021 RID: 33
	public Dictionary<SdkUIMgr.InfoState, string> infoMap = new Dictionary<SdkUIMgr.InfoState, string>
	{
		{
			SdkUIMgr.InfoState.GPUAndSoc,
			"GpuAndSocNotPrompting"
		},
		{
			SdkUIMgr.InfoState.Stack,
			"StackNotPrompting"
		}
	};

	// Token: 0x04000022 RID: 34
	public List<SdkUIMgr.InfoState> NeedBack = new List<SdkUIMgr.InfoState>
	{
		SdkUIMgr.InfoState.Stack,
		SdkUIMgr.InfoState.GPUAndSoc
	};

	// Token: 0x04000023 RID: 35
	public bool bStack = false;

	// Token: 0x04000024 RID: 36
	public overview.ConfigMode configMode = overview.ConfigMode.Custom;

	// Token: 0x04000025 RID: 37
	public SdkUIMgr.FuncPtr funcptr;

	// Token: 0x04000026 RID: 38
	public Vector2 OldPos = new Vector2(float.MinValue, float.MinValue);

	// Token: 0x04000027 RID: 39
	public Vector2 ExitInitPos = new Vector2(float.MinValue, float.MinValue);

	// Token: 0x04000028 RID: 40
	public bool IsFold = false;

	// Token: 0x04000029 RID: 41
	private SdkUIMgr.UIState _uiState = SdkUIMgr.UIState.MODE;

	// Token: 0x0400002A RID: 42
	private SdkUIMgr.SetMode setMode;

	// Token: 0x0400002B RID: 43
	private float lastScWidget = 0f;

	// Token: 0x0400002C RID: 44
	private static SdkUIMgr instance = null;

	// Token: 0x0400002D RID: 45
	public bool isBack = false;

	// Token: 0x0400002E RID: 46
	public WindowState CurState = WindowState.Horizontal;

	// Token: 0x0400002F RID: 47
	public int CurSetModeNum = -1;

	// Token: 0x04000030 RID: 48
	public bool isAnim = false;

	// Token: 0x04000031 RID: 49
	private Transform UIRoot;

	// Token: 0x04000032 RID: 50
	private UWAPanel Panel;

	// Token: 0x04000033 RID: 51
	public static object actionLockObj = new object();

	// Token: 0x04000034 RID: 52
	public static List<Action> MainThreadActions = new List<Action>();

	// Token: 0x04000035 RID: 53
	private SdkUIMgr.UINotWork _notWork = SdkUIMgr.UINotWork.None;

	// Token: 0x04000036 RID: 54
	private bool _show = true;

	// Token: 0x020000D8 RID: 216
	public enum UIState
	{
		// Token: 0x040005D4 RID: 1492
		MODE,
		// Token: 0x040005D5 RID: 1493
		SELECT,
		// Token: 0x040005D6 RID: 1494
		SET,
		// Token: 0x040005D7 RID: 1495
		INFO,
		// Token: 0x040005D8 RID: 1496
		SERVICE,
		// Token: 0x040005D9 RID: 1497
		STOPPED
	}

	// Token: 0x020000D9 RID: 217
	public enum SetMode
	{
		// Token: 0x040005DB RID: 1499
		OVERVIEW,
		// Token: 0x040005DC RID: 1500
		MONO,
		// Token: 0x040005DD RID: 1501
		RESOURCES,
		// Token: 0x040005DE RID: 1502
		LUA,
		// Token: 0x040005DF RID: 1503
		GPU
	}

	// Token: 0x020000DA RID: 218
	public enum InfoState
	{
		// Token: 0x040005E1 RID: 1505
		GPUAndSoc,
		// Token: 0x040005E2 RID: 1506
		Stack,
		// Token: 0x040005E3 RID: 1507
		None
	}

	// Token: 0x020000DB RID: 219
	// (Invoke) Token: 0x06000974 RID: 2420
	public delegate void FuncPtr();

	// Token: 0x020000DC RID: 220
	[Flags]
	public enum UINotWork
	{
		// Token: 0x040005E5 RID: 1509
		None = 0,
		// Token: 0x040005E6 RID: 1510
		GPM = 1,
		// Token: 0x040005E7 RID: 1511
		HasError = 2,
		// Token: 0x040005E8 RID: 1512
		OnlineMode = 4
	}
}
