using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using UWACore.TrackManagers;

namespace UWA
{
	// Token: 0x02000053 RID: 83
	internal static class UWAState
	{
		// Token: 0x060003AA RID: 938 RVA: 0x000190D4 File Offset: 0x000172D4
		public static void StaticInit()
		{
			UWAState.VrMode = SharedUtils.isVRMode();
			bool flag = !File.Exists(SharedUtils.FinalDataPath + "/done") && !File.Exists(SharedUtils.FinalDataPath + "/last");
			if (flag)
			{
				UWAState.NewTest = true;
			}
			else
			{
				UWAState.HasLastData = true;
			}
			bool flag2 = File.Exists(SharedUtils.FinalDataPath + "/zip");
			if (flag2)
			{
				UWAState.HasLastPack = true;
			}
			bool vrMode = UWAState.VrMode;
			if (vrMode)
			{
				UWAState.OpenServer();
			}
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00019178 File Offset: 0x00017378
		public static void UpdateSceneName()
		{
			string loadedLevelName = Application.loadedLevelName;
			bool flag = loadedLevelName == null;
			if (!flag)
			{
				bool flag2 = loadedLevelName == UWAState.lastSceneName;
				if (!flag2)
				{
					UWAState.lastSceneName = loadedLevelName;
					bool flag3 = UWAState.onLevelChanged != null;
					if (flag3)
					{
						UWAState.onLevelChanged();
					}
				}
			}
		}

		// Token: 0x060003AC RID: 940 RVA: 0x000191D8 File Offset: 0x000173D8
		public static void CheckTestError()
		{
			UWAGUI.ErrorInfo = "";
			UWAState.HasError = false;
			string str = null;
			bool flag = !BaseTrackerManager.IsAllReady(out str);
			if (flag)
			{
				UWAState.HasError = true;
				UWAGUI.ErrorInfo = UWAGUI.ErrorInfo + str + "\n";
			}
			bool flag2 = !Directory.Exists(SharedUtils.FinalDataPath);
			if (flag2)
			{
				bool flag3 = SharedUtils.VerifyWritePermision();
				UWAState.HasError = true;
				UWAGUI.ErrorInfo = string.Concat(new string[]
				{
					UWAGUI.ErrorInfo,
					"Write Access Internal (",
					flag3.ToString(),
					").\n",
					SharedUtils.FinalDataPath,
					"\n"
				});
			}
			try
			{
				bool flag4 = !SharedUtils.Dev;
				if (flag4)
				{
					UWAState.HasError = true;
					UWAGUI.ErrorInfo += "Profiler is not supported.\n";
				}
				Profiler.enabled = true;
				bool flag5 = !Profiler.enabled;
				if (flag5)
				{
					UWAState.HasError = true;
					UWAGUI.ErrorInfo += "Profiler is not enabled.\n";
				}
			}
			catch (Exception ex)
			{
				UWAState.HasError = true;
				UWAGUI.ErrorInfo += "Profiler is not supported. E.\n";
			}
			long runtimeRomSpace = AndroidHardwareTrackManager.GetRuntimeRomSpace();
			bool flag6 = runtimeRomSpace != -1L && runtimeRomSpace < 1000000000L;
			if (flag6)
			{
				UWAState.HasError = true;
				UWAGUI.ErrorInfo += string.Format("Storage is too low to start : {0}.\n", CoreUtils.FormatSize(runtimeRomSpace));
			}
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00019378 File Offset: 0x00017578
		public static void OpenServer()
		{
			IPAddress selfIp = CoreUtils.GetSelfIp();
			try
			{
				UWAState.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				UWAState.serverSocket.Bind(new IPEndPoint(selfIp, 8899));
				UWAState.serverSocket.Listen(10);
				UWAState.serverSocket.SendTimeout = 3000;
				CoreUtils.UWASendLogToServer(string.Format("State Server start successful | {0}", UWAState.serverSocket.LocalEndPoint));
				Thread thread = new Thread(new ThreadStart(UWAState.ListenClientConnect));
				thread.Start();
			}
			catch (Exception ex)
			{
				string str = "State Server start Exception:";
				Exception ex2 = ex;
				CoreUtils.UWASendLogToServer(str + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00019444 File Offset: 0x00017644
		public static void ListenClientConnect()
		{
			for (;;)
			{
				try
				{
					Socket parameter = UWAState.serverSocket.Accept();
					Thread thread = new Thread(new ParameterizedThreadStart(UWAState.ReceiveMassage));
					thread.Start(parameter);
				}
				catch (Exception ex)
				{
					string str = "ListenClientConnect Exception: ";
					Exception ex2 = ex;
					CoreUtils.UWASendLogToServer(str + ((ex2 != null) ? ex2.ToString() : null) + " | " + ex.StackTrace);
				}
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000194D0 File Offset: 0x000176D0
		private static string OnReceive(string cmdStr)
		{
			string[] array = cmdStr.Split(new char[]
			{
				'$'
			});
			CoreUtils.UWASendLogToServer("OnClientCmd : " + array[0]);
			UWAState.ClientCmd clientCmd = (UWAState.ClientCmd)Enum.Parse(typeof(UWAState.ClientCmd), array[0]);
			UWAState.ClientCmd clientCmd2 = clientCmd;
			UWAState.ClientCmd clientCmd3 = clientCmd2;
			if (clientCmd3 != UWAState.ClientCmd.normal)
			{
				if (clientCmd3 != UWAState.ClientCmd.mono_heap)
				{
					switch (clientCmd3)
					{
					case UWAState.ClientCmd.Connect:
					{
						bool flag = UWAState._serverState == UWAState.GuiServerState.WaitToConnect;
						if (flag)
						{
							UWAState._serverState = UWAState.GuiServerState.Connected;
						}
						break;
					}
					case UWAState.ClientCmd.RequestState:
					{
						bool flag2 = UWAState._serverState != UWAState.GuiServerState.Recording;
						if (flag2)
						{
							bool flag3 = UWAState.UploadType == UWAState.eUploadType.Unset;
							if (flag3)
							{
								UWAState._serverState = UWAState.GuiServerState.UploadType;
							}
							else
							{
								bool flag4 = UWAState.ParseMode == UWAState.eParseMode.Unknown;
								if (flag4)
								{
									UWAState._serverState = UWAState.GuiServerState.ParseMode;
								}
								else
								{
									bool flag5 = !UWAState.NewTest && !UWAState.ContinousTest;
									if (flag5)
									{
										UWAState._serverState = UWAState.GuiServerState.ContinueOrNew;
									}
									else
									{
										UWAState._serverState = UWAState.GuiServerState.WaitToStart;
									}
								}
							}
						}
						break;
					}
					case UWAState.ClientCmd.Continue:
					{
						bool flag6 = UWAState._serverState != UWAState.GuiServerState.Recording;
						if (flag6)
						{
							UWAState.ContinousTest = true;
							UWAState.NewTest = false;
							UWAState._serverState = UWAState.GuiServerState.WaitToStart;
						}
						break;
					}
					case UWAState.ClientCmd.New:
					{
						bool flag7 = UWAState._serverState != UWAState.GuiServerState.Recording;
						if (flag7)
						{
							UWAState.NewTest = true;
							UWAState.ContinousTest = false;
							UWAState._serverState = UWAState.GuiServerState.WaitToStart;
						}
						break;
					}
					case UWAState.ClientCmd.Start:
					{
						bool flag8 = UWAState._serverState != UWAState.GuiServerState.Recording && (UWAState.NewTest || UWAState.ContinousTest);
						if (flag8)
						{
							UWAState._serverState = UWAState.GuiServerState.Recording;
							UWAState.StartFromGUIServer = true;
						}
						break;
					}
					case UWAState.ClientCmd.Stop:
					{
						bool flag9 = UWAState._serverState == UWAState.GuiServerState.Recording;
						if (flag9)
						{
							UWAState._serverState = UWAState.GuiServerState.ToQuit;
							UWAState.StopFromGUIServer = true;
						}
						break;
					}
					}
				}
				else
				{
					bool flag10 = UWAState._serverState != UWAState.GuiServerState.Recording;
					if (flag10)
					{
						UWAState.ParseMode = UWAState.eParseMode.mono_heap;
						UWAState.NewTest = true;
						UWAState.GpuInNormal = false;
						UWAState._serverState = UWAState.GuiServerState.ContinueOrNew;
					}
				}
			}
			else
			{
				bool flag11 = UWAState._serverState != UWAState.GuiServerState.Recording;
				if (flag11)
				{
					UWAState.ParseMode = UWAState.eParseMode.normal;
					UWAState._serverState = UWAState.GuiServerState.ContinueOrNew;
				}
			}
			return UWAState._serverState.ToString();
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00019744 File Offset: 0x00017944
		private static void ReceiveMassage(object clientSocket)
		{
			Socket socket = (Socket)clientSocket;
			bool connected = socket.Connected;
			if (connected)
			{
				byte[] array = new byte[1024];
				int num = socket.Receive(array, array.Length, SocketFlags.None);
				bool flag = num > 0;
				if (flag)
				{
					string @string = Encoding.Unicode.GetString(array);
					bool flag2 = @string.Length > 0;
					if (flag2)
					{
						string str = UWAState.OnReceive(@string);
						bool flag3 = @string.Contains("uwagui-return");
						if (flag3)
						{
							socket.Send(Encoding.Unicode.GetBytes(str + "$"));
						}
					}
					socket.Close();
				}
				else
				{
					socket.Close();
				}
			}
			else
			{
				socket.Close();
			}
			bool isAlive = Thread.CurrentThread.IsAlive;
			if (isAlive)
			{
				Thread.CurrentThread.Abort();
			}
		}

		// Token: 0x0400021A RID: 538
		public static UWAState.eParseMode ParseMode = UWAState.eParseMode.Unknown;

		// Token: 0x0400021B RID: 539
		public static bool HasError = false;

		// Token: 0x0400021C RID: 540
		public static bool VrMode = false;

		// Token: 0x0400021D RID: 541
		public static UWAState.eScreenshotMethod ScreenMethod = UWAState.eScreenshotMethod.Java;

		// Token: 0x0400021E RID: 542
		public static bool GpuInNormal = false;

		// Token: 0x0400021F RID: 543
		public static string NewGpuDesc = "";

		// Token: 0x04000220 RID: 544
		public static string NewGpuDescTemp = "";

		// Token: 0x04000221 RID: 545
		public static bool TextureMeshUsageOnly = false;

		// Token: 0x04000222 RID: 546
		public static bool GUIShow = true;

		// Token: 0x04000223 RID: 547
		public static bool ContinousTest = false;

		// Token: 0x04000224 RID: 548
		public static bool NewTest = false;

		// Token: 0x04000225 RID: 549
		public static bool HasLastData = false;

		// Token: 0x04000226 RID: 550
		public static bool HasLastPack = false;

		// Token: 0x04000227 RID: 551
		public static bool? SendLastPack = null;

		// Token: 0x04000228 RID: 552
		public static bool ScreenLost = false;

		// Token: 0x04000229 RID: 553
		public static bool DataLost = false;

		// Token: 0x0400022A RID: 554
		public static bool ScreencapLost = false;

		// Token: 0x0400022B RID: 555
		public static UWAState.eUploadType UploadType = UWAState.eUploadType.Unset;

		// Token: 0x0400022C RID: 556
		public static UWAState.eRecordingState RecordingState = UWAState.eRecordingState.Unset;

		// Token: 0x0400022D RID: 557
		public static bool Paused = false;

		// Token: 0x0400022E RID: 558
		public static bool TestInited = false;

		// Token: 0x0400022F RID: 559
		public static double TestDuration = 0.0;

		// Token: 0x04000230 RID: 560
		public static string Note = "";

		// Token: 0x04000231 RID: 561
		public static string lastSceneName = null;

		// Token: 0x04000232 RID: 562
		public static UWAState.OnLevelChange onLevelChanged;

		// Token: 0x04000233 RID: 563
		private static UWAState.GuiServerState _serverState = UWAState.GuiServerState.WaitToConnect;

		// Token: 0x04000234 RID: 564
		private static Socket serverSocket;

		// Token: 0x04000235 RID: 565
		public static bool StartFromGUIServer = false;

		// Token: 0x04000236 RID: 566
		public static bool StopFromGUIServer = false;

		// Token: 0x02000108 RID: 264
		public enum eParseMode
		{
			// Token: 0x04000699 RID: 1689
			Unknown,
			// Token: 0x0400069A RID: 1690
			normal,
			// Token: 0x0400069B RID: 1691
			mono_heap,
			// Token: 0x0400069C RID: 1692
			beginner,
			// Token: 0x0400069D RID: 1693
			load,
			// Token: 0x0400069E RID: 1694
			ui,
			// Token: 0x0400069F RID: 1695
			hang,
			// Token: 0x040006A0 RID: 1696
			lua,
			// Token: 0x040006A1 RID: 1697
			gpu,
			// Token: 0x040006A2 RID: 1698
			ui_dc,
			// Token: 0x040006A3 RID: 1699
			opengl,
			// Token: 0x040006A4 RID: 1700
			view
		}

		// Token: 0x02000109 RID: 265
		public enum eScreenshotMethod
		{
			// Token: 0x040006A6 RID: 1702
			Disabled,
			// Token: 0x040006A7 RID: 1703
			Java,
			// Token: 0x040006A8 RID: 1704
			Adb
		}

		// Token: 0x0200010A RID: 266
		public enum eUploadType
		{
			// Token: 0x040006AA RID: 1706
			Online,
			// Token: 0x040006AB RID: 1707
			Offline,
			// Token: 0x040006AC RID: 1708
			Test,
			// Token: 0x040006AD RID: 1709
			Unset
		}

		// Token: 0x0200010B RID: 267
		public enum eRecordingState
		{
			// Token: 0x040006AF RID: 1711
			PrepareForRecord,
			// Token: 0x040006B0 RID: 1712
			Recording,
			// Token: 0x040006B1 RID: 1713
			PrepareForStop,
			// Token: 0x040006B2 RID: 1714
			Stopped,
			// Token: 0x040006B3 RID: 1715
			Unset
		}

		// Token: 0x0200010C RID: 268
		// (Invoke) Token: 0x0600095B RID: 2395
		public delegate void OnLevelChange();

		// Token: 0x0200010D RID: 269
		private enum GuiServerState
		{
			// Token: 0x040006B5 RID: 1717
			WaitToConnect,
			// Token: 0x040006B6 RID: 1718
			Connected,
			// Token: 0x040006B7 RID: 1719
			UploadType,
			// Token: 0x040006B8 RID: 1720
			ParseMode,
			// Token: 0x040006B9 RID: 1721
			ContinueOrNew,
			// Token: 0x040006BA RID: 1722
			WaitToStart,
			// Token: 0x040006BB RID: 1723
			Recording,
			// Token: 0x040006BC RID: 1724
			ToQuit
		}

		// Token: 0x0200010E RID: 270
		private enum ClientCmd
		{
			// Token: 0x040006BE RID: 1726
			Unknown,
			// Token: 0x040006BF RID: 1727
			normal,
			// Token: 0x040006C0 RID: 1728
			mono_heap,
			// Token: 0x040006C1 RID: 1729
			Connect = 101,
			// Token: 0x040006C2 RID: 1730
			RequestState,
			// Token: 0x040006C3 RID: 1731
			Continue,
			// Token: 0x040006C4 RID: 1732
			New,
			// Token: 0x040006C5 RID: 1733
			Start,
			// Token: 0x040006C6 RID: 1734
			Stop
		}
	}
}
