using System;
using System.IO;
using UnityEngine;
using UWACore.UITestTool;

namespace UWA
{
	// Token: 0x02000055 RID: 85
	internal static class UWAGUI
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00019EF8 File Offset: 0x000180F8
		public static int GroupWidth
		{
			get
			{
				return (int)((float)Screen.width * 0.38200003f);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x00019F20 File Offset: 0x00018120
		public static int GroupHeight
		{
			get
			{
				return (int)((float)Screen.height * 0.38200003f * 0.5f);
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00019F4C File Offset: 0x0001814C
		public static Rect GroupRect
		{
			get
			{
				return new Rect(0f, 0f, (float)UWAGUI.GroupWidth, (float)UWAGUI.GroupHeight);
			}
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00019F80 File Offset: 0x00018180
		public static void StaticInit()
		{
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00019F84 File Offset: 0x00018184
		public static void SetFontSize(int size)
		{
			GUI.skin.button.fontSize = size;
			GUI.skin.label.fontSize = size;
			GUI.skin.textField.fontSize = size;
			GUI.skin.window.fontSize = size;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00019FDC File Offset: 0x000181DC
		public static GUILayoutOption[] GetSuitableOption(float x, float y)
		{
			return new GUILayoutOption[]
			{
				GUILayout.Width((float)((int)((float)UWAGUI.GroupWidth * x))),
				GUILayout.Height((float)((int)((float)UWAGUI.GroupHeight * y))),
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			};
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0001A034 File Offset: 0x00018234
		public static void InputCheck()
		{
			bool hasError = UWAState.HasError;
			if (hasError)
			{
				UWAGUI._guiState = UWAGUI.GUIState.Error;
				UWAGUI._errorWaitTime += Time.unscaledDeltaTime;
				bool flag = UWAGUI._errorWaitTime > 5f;
				if (flag)
				{
					UWAGUI._errorWaitTime = 0f;
					UWAState.CheckTestError();
				}
			}
			else
			{
				UWAGUI.NonVRTouchCheck();
				bool vrMode = UWAState.VrMode;
				if (vrMode)
				{
					UWAGUI.CheckGUIServer();
				}
			}
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0001A0AC File Offset: 0x000182AC
		public static void CheckGUIServer()
		{
			bool startFromGUIServer = UWAState.StartFromGUIServer;
			if (startFromGUIServer)
			{
				UWAState.StartFromGUIServer = false;
				UWAStarter.Get().StartCoroutine(UWAStarter.Get().SwitchRecording());
				UWAState.GUIShow = false;
			}
			bool stopFromGUIServer = UWAState.StopFromGUIServer;
			if (stopFromGUIServer)
			{
				UWAState.StopFromGUIServer = false;
				UWAStarter.Get().StartCoroutine(UWAStarter.Get().SwitchRecording());
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0001A118 File Offset: 0x00018318
		public static void NonVRGUI()
		{
			bool vrMode = UWAState.VrMode;
			if (!vrMode)
			{
				bool flag = !UWAState.GUIShow;
				if (!flag)
				{
					GUILayout.Window(10685, UWAGUI.GroupRect, new GUI.WindowFunction(UWAGUI.DoToolWindow), "UWA Tool", new GUILayoutOption[0]);
					bool flag2 = !UWAGUI.HideForPoco && UWAState.ParseMode == UWAState.eParseMode.Unknown && ASCTest.Enabled && ASCTest.Instance != null && !ASCTest.Instance.Started;
					if (flag2)
					{
						GUILayout.Window(10293, new Rect(UWAGUI.GroupRect.x, (float)Screen.height - UWAGUI.GroupRect.height, UWAGUI.GroupRect.width, UWAGUI.GroupRect.height), new GUI.WindowFunction(UWAGUI.DoRemoteWindow), "Remote Tool", new GUILayoutOption[0]);
					}
				}
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0001A224 File Offset: 0x00018424
		public static void NonVRTouchCheck()
		{
			bool vrMode = UWAState.VrMode;
			if (!vrMode)
			{
				bool hasError = UWAState.HasError;
				if (!hasError)
				{
					bool guishow = UWAState.GUIShow;
					if (guishow)
					{
						UWAGUI.TouchTime = 0f;
					}
					else
					{
						bool multiTouchEnabled = Input.multiTouchEnabled;
						if (multiTouchEnabled)
						{
							UWAGUI.TouchTime = ((Input.touchCount > 2) ? (UWAGUI.TouchTime + Time.unscaledDeltaTime) : 0f);
							UWAState.GUIShow = (UWAGUI.TouchTime > UWAGUI.HowLongToShow);
						}
						else
						{
							UWAGUI.TouchTime = ((Input.touchCount > 0) ? (UWAGUI.TouchTime + Time.unscaledDeltaTime) : 0f);
							UWAState.GUIShow = (UWAGUI.TouchTime > UWAGUI.HowLongToShow);
						}
					}
				}
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001A2F8 File Offset: 0x000184F8
		private static void DoLastPackWindow(int windowID)
		{
			UWAGUI.SetFontSize(20);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag = GUILayout.Button("Send", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			if (flag)
			{
				UWAState.SendLastPack = new bool?(true);
			}
			bool flag2 = GUILayout.Button("Skip", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			if (flag2)
			{
				UWAState.SendLastPack = new bool?(false);
				bool hasLastPack = UWAState.HasLastPack;
				if (hasLastPack)
				{
					File.Delete(SharedUtils.FinalDataPath + "/zip");
				}
				UWAState.HasLastPack = false;
				UWAState.HasLastData = false;
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001A3AC File Offset: 0x000185AC
		private static void DoToolWindow(int windowID)
		{
			UWAGUI.SetFontSize(20);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Plugins : " + UWACoreConfig.PLUGIN_VERSION, UWAGUI.GetSuitableOption(0.5f, 0.3f));
			GUILayout.Label("Unity : " + Application.unityVersion, UWAGUI.GetSuitableOption(0.5f, 0.3f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Color color = GUI.color;
			bool flag = string.IsNullOrEmpty(UWACoreConfig.REPORT_IP);
			if (flag)
			{
				GUI.color = Color.red;
			}
			GUILayout.Label("Report Id : " + UWACoreConfig.REPORT_IP, UWAGUI.GetSuitableOption(0.7f, 0.3f));
			GUI.color = color;
			GUILayout.Label(SystemInfo.graphicsMultiThreaded ? "MTR" : "Not-MTR", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			GUILayout.EndHorizontal();
			bool hideForPoco = UWAGUI.HideForPoco;
			if (!hideForPoco)
			{
				bool flag2 = UWAState.UploadType == UWAState.eUploadType.Unset;
				if (flag2)
				{
					bool enabled = GUI.enabled;
					bool flag3 = string.IsNullOrEmpty(UWACoreConfig.GUID);
					if (flag3)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label("Report Id: ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						UWACoreConfig.REPORT_IP = GUILayout.TextField(UWACoreConfig.REPORT_IP, UWAGUI.GetSuitableOption(0.6f, 0.3f));
						GUILayout.EndHorizontal();
						bool flag4 = !UWACoreConfig.REPORT_IP.Contains("-");
						if (flag4)
						{
							GUI.enabled = false;
						}
					}
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					bool flag5 = GUILayout.Button("Online", UWAGUI.GetSuitableOption(0.3f, 0.3f));
					if (flag5)
					{
						UWAState.UploadType = UWAState.eUploadType.Online;
					}
					GUI.enabled = enabled;
					bool flag6 = GUILayout.Button("Offline", UWAGUI.GetSuitableOption(0.3f, 0.3f));
					if (flag6)
					{
						UWAState.UploadType = UWAState.eUploadType.Offline;
					}
					bool flag7 = GUILayout.Button("Test", UWAGUI.GetSuitableOption(0.2f, 0.3f));
					if (flag7)
					{
						UWAState.UploadType = UWAState.eUploadType.Test;
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					bool flag8 = UWAState.ParseMode == UWAState.eParseMode.Unknown;
					if (flag8)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						bool flag9 = GUILayout.Button(" 常规 ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						if (flag9)
						{
							UWAState.ParseMode = UWAState.eParseMode.normal;
						}
						bool flag10 = GUILayout.Button(" 新手 ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						if (flag10)
						{
							UWAState.ParseMode = UWAState.eParseMode.beginner;
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						bool flag11 = GUILayout.Button(" 加载 ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						if (flag11)
						{
							UWAState.ParseMode = UWAState.eParseMode.load;
						}
						bool flag12 = GUILayout.Button(" UI切换 ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						if (flag12)
						{
							UWAState.ParseMode = UWAState.eParseMode.ui;
							UWAState.GpuInNormal = false;
							UWAStarter.Get().gameObject.AddComponent<UISwitcher>();
						}
						bool flag13 = GUILayout.Button(" 挂机 ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
						if (flag13)
						{
							UWAState.ParseMode = UWAState.eParseMode.hang;
						}
						GUILayout.EndHorizontal();
					}
					else
					{
						bool flag14 = !UWAState.ContinousTest && !UWAState.NewTest;
						if (flag14)
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							bool flag15 = GUILayout.Button("Continue", UWAGUI.GetSuitableOption(0.3f, 0.3f));
							if (flag15)
							{
								UWAState.ContinousTest = true;
							}
							bool flag16 = GUILayout.Button("New", UWAGUI.GetSuitableOption(0.3f, 0.3f));
							if (flag16)
							{
								UWAState.NewTest = true;
							}
							GUILayout.EndHorizontal();
						}
						else
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							color = GUI.color;
							GUI.color = ((UWAState.ScreenMethod == UWAState.eScreenshotMethod.Disabled) ? Color.red : Color.green);
							string text = (UWAState.ScreenMethod == UWAState.eScreenshotMethod.Disabled) ? "Screen Disabled" : ((UWAState.ScreenMethod == UWAState.eScreenshotMethod.Java) ? "Screen Java" : "Screen Adb");
							bool flag17 = GUILayout.Button(text, UWAGUI.GetSuitableOption(0.2f, 0.3f));
							if (flag17)
							{
								UWAState.ScreenMethod = (UWAState.ScreenMethod + 1) % (UWAState.eScreenshotMethod)3;
							}
							GUI.color = color;
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							bool enabled2 = GUI.enabled;
							bool flag18 = UWAState.RecordingState != UWAState.eRecordingState.Recording && UWAState.RecordingState != UWAState.eRecordingState.Unset;
							if (flag18)
							{
								GUI.enabled = false;
							}
							GUI.backgroundColor = ((UWAState.RecordingState == UWAState.eRecordingState.Recording) ? Color.red : Color.green);
							bool flag19 = GUILayout.Button((UWAState.RecordingState == UWAState.eRecordingState.Recording) ? "Stop" : "Start", UWAGUI.GetSuitableOption(0.3f, 0.3f));
							if (flag19)
							{
								UWAStarter.Get().StartCoroutine(UWAStarter.Get().SwitchRecording());
							}
							GUI.enabled = enabled2;
							GUI.backgroundColor = Color.black;
							bool flag20 = GUILayout.Button("Hide ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
							if (flag20)
							{
								UWAState.GUIShow = false;
							}
							GUILayout.EndHorizontal();
						}
					}
				}
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0001A93C File Offset: 0x00018B3C
		private static void DoRemoteWindow(int windowId)
		{
			GUILayout.Space(15f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Remote IP: ", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			UWACoreConfig.Remote_IP = GUILayout.TextField(UWACoreConfig.Remote_IP, UWAGUI.GetSuitableOption(0.6f, 0.3f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag = GUILayout.Button("Start Server", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			if (flag)
			{
				ASCTest.Instance.StartServer(false);
			}
			bool flag2 = GUILayout.Button("Start Client", UWAGUI.GetSuitableOption(0.3f, 0.3f));
			if (flag2)
			{
				ASCTest.Instance.StartServer(true);
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0001AA14 File Offset: 0x00018C14
		public static void ShowRecordingInfo()
		{
			GUILayout.Window(25768, new Rect(0f, 0f, UWAGUI.GroupRect.width, (float)(UWAGUI.GroupHeight / 2)), new GUI.WindowFunction(UWAGUI.DoTimeWindow), "UWA Recording", new GUILayoutOption[0]);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001AA6C File Offset: 0x00018C6C
		public static void ShowLastPackInfo()
		{
			GUILayout.Window(25769, new Rect(0f, 0f, UWAGUI.GroupRect.width, (float)(UWAGUI.GroupHeight / 2)), new GUI.WindowFunction(UWAGUI.DoLastPackWindow), "UWA Last Pack", new GUILayoutOption[0]);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0001AAC4 File Offset: 0x00018CC4
		public static void ShowProgressInfo(string dataString, float percent)
		{
			string text = dataString + " [" + percent.ToString("P") + "]";
			UWAGUI.SetFontSize(40);
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			GUI.Button(new Rect(0f, (float)(Screen.height * 2 / 5), (float)Screen.width, (float)(Screen.height / 5)), text);
			GUI.color = Color.green;
			GUI.Button(new Rect(0f, (float)(Screen.height * 2 / 5), (float)Screen.width * percent, (float)(Screen.height / 5)), "");
			UWAGUI.SetFontSize(20);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001AB7C File Offset: 0x00018D7C
		public static bool ShowLostInfo(string dataString, bool canContinue = true)
		{
			UWAGUI.SetFontSize(40);
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			bool flag = GUI.Button(new Rect(2f, 0f, (float)Screen.width, (float)Screen.height), dataString);
			if (flag)
			{
				if (canContinue)
				{
					UWAGUI.press++;
				}
			}
			GUI.color = (canContinue ? Color.yellow : Color.red);
			bool flag2 = GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), dataString);
			if (flag2)
			{
				if (canContinue)
				{
					UWAGUI.press++;
				}
			}
			GUI.color = Color.black;
			UWAGUI.SetFontSize(20);
			bool flag3 = canContinue && UWAGUI.press > 5;
			bool result;
			if (flag3)
			{
				UWAGUI.press = 0;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001AC90 File Offset: 0x00018E90
		public static void ShowStaticBatchInfo()
		{
			bool flag = !UWAGUI.showStaticInfo;
			if (!flag)
			{
				string text = "New GPU 测试中检测到了 Static Batching 的使用。";
				UWAGUI.SetFontSize(40);
				GUI.backgroundColor = Color.black;
				GUI.color = Color.white;
				GUI.Button(new Rect(2f, 0f, (float)Screen.width, (float)Screen.height), text);
				GUI.color = Color.yellow;
				GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
				GUI.color = Color.black;
				UWAGUI.SetFontSize(20);
				bool multiTouchEnabled = Input.multiTouchEnabled;
				if (multiTouchEnabled)
				{
					UWAGUI.TouchTime2 = ((Input.touchCount > 2) ? (UWAGUI.TouchTime2 + Time.unscaledDeltaTime) : 0f);
				}
				else
				{
					UWAGUI.TouchTime2 = ((Input.touchCount > 0) ? (UWAGUI.TouchTime2 + Time.unscaledDeltaTime) : 0f);
				}
				bool flag2 = UWAGUI.TouchTime2 > 4f;
				if (flag2)
				{
					UWAGUI.showStaticInfo = false;
				}
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001ADB8 File Offset: 0x00018FB8
		public static bool ShowProjreviewInfo()
		{
			string text = "当前SDK仅适用于《深度优化》，\n正常测试请从其他服务器进行下载SDK。\n三指长按以取消该提示。";
			UWAGUI.SetFontSize(40);
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			GUI.Button(new Rect(2f, 0f, (float)Screen.width, (float)Screen.height), text);
			GUI.color = Color.yellow;
			GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
			GUI.color = Color.black;
			UWAGUI.SetFontSize(20);
			bool multiTouchEnabled = Input.multiTouchEnabled;
			if (multiTouchEnabled)
			{
				UWAGUI.TouchTime2 = ((Input.touchCount > 2) ? (UWAGUI.TouchTime2 + Time.unscaledDeltaTime) : 0f);
			}
			else
			{
				UWAGUI.TouchTime2 = ((Input.touchCount > 0) ? (UWAGUI.TouchTime2 + Time.unscaledDeltaTime) : 0f);
			}
			return UWAGUI.TouchTime2 > 4f;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0001AEC4 File Offset: 0x000190C4
		public static void ShowSpaceLowInfo()
		{
			string text = "设备磁盘空间已经不足 50 MB，\n请强制退出，清理空间后可以 Continue。";
			UWAGUI.SetFontSize(40);
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			GUI.Button(new Rect(2f, 0f, (float)Screen.width, (float)Screen.height), text);
			GUI.color = Color.yellow;
			GUI.Button(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), text);
			GUI.color = Color.black;
			UWAGUI.SetFontSize(20);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001AF5C File Offset: 0x0001915C
		public static void ShowErrorInfo(string error)
		{
			UWAGUI.SetFontSize(40);
			GUI.color = Color.red;
			string[] array = error.Split(new string[]
			{
				"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				GUILayout.Button(text, new GUILayoutOption[0]);
			}
			GUI.color = Color.black;
			UWAGUI.SetFontSize(20);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001AFD8 File Offset: 0x000191D8
		private static void DoTimeWindow(int windowID)
		{
			UWAGUI.SetFontSize(40);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Button(string.Format("{0:F} m", UWAState.TestDuration), UWAGUI.GetSuitableOption(0.3f, 0.5f));
			GUILayout.Button(string.Format("{0} K f", SharedUtils.frameId / 1000), UWAGUI.GetSuitableOption(0.3f, 0.5f));
			GUILayout.EndHorizontal();
			UWAGUI.SetFontSize(20);
		}

		// Token: 0x04000239 RID: 569
		public static float HowLongToShow = 5f;

		// Token: 0x0400023A RID: 570
		public static float TouchTime = 0f;

		// Token: 0x0400023B RID: 571
		private static float _pressTime = 0f;

		// Token: 0x0400023C RID: 572
		private static float _errorWaitTime = 0f;

		// Token: 0x0400023D RID: 573
		public static string ErrorInfo = null;

		// Token: 0x0400023E RID: 574
		private static bool _VrMode = false;

		// Token: 0x0400023F RID: 575
		private static bool _urpAnlalyze = false;

		// Token: 0x04000240 RID: 576
		private static UWAGUI.GUIState _guiState = UWAGUI.GUIState.Error;

		// Token: 0x04000241 RID: 577
		public static bool HideForPoco = false;

		// Token: 0x04000242 RID: 578
		private static int press = 0;

		// Token: 0x04000243 RID: 579
		private static bool showStaticInfo = true;

		// Token: 0x04000244 RID: 580
		private static float TouchTime2 = 0f;

		// Token: 0x02000111 RID: 273
		private enum GUIState
		{
			// Token: 0x040006D0 RID: 1744
			Error,
			// Token: 0x040006D1 RID: 1745
			ContinousOrNew,
			// Token: 0x040006D2 RID: 1746
			Continous,
			// Token: 0x040006D3 RID: 1747
			New,
			// Token: 0x040006D4 RID: 1748
			StartOrStop,
			// Token: 0x040006D5 RID: 1749
			Start,
			// Token: 0x040006D6 RID: 1750
			Stop
		}
	}
}
