using System;
using System.Threading;
using UWA;
using UWACore.TrackManagers;
using UWACore.Util.NetWork;

namespace UWACore.Util.AndroidScreenShot
{
	// Token: 0x0200004D RID: 77
	internal static class AdbScreenShot
	{
		// Token: 0x06000359 RID: 857 RVA: 0x00016E18 File Offset: 0x00015018
		public static bool ScreencapConnect(bool video)
		{
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.LogError("ScreencapConnect");
			}
			string text = video ? "0$0$0$1$" : "0$0$1$0$";
			string text2 = string.Concat(new string[]
			{
				text,
				SharedUtils.FinalDataPath,
				"$",
				AndroidHardwareTrackManager.Pid.ToString(),
				"$"
			});
			bool showLog2 = SharedUtils.ShowLog;
			if (showLog2)
			{
				SharedUtils.LogError(text2);
			}
			string text3 = SimpleSender.SendAndRequestServer(text2);
			bool showLog3 = SharedUtils.ShowLog;
			if (showLog3)
			{
				SharedUtils.LogError(text3);
			}
			bool flag = text3 == null || text3.Contains("not connected") || text3.Contains("offline") || text3.Contains("unauthorized");
			return !flag;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00016F14 File Offset: 0x00015114
		public static void ScreencapCMDToSend(bool start)
		{
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("ScreencapCMDToSend");
			}
			string text = (start ? "5" : "6") + "$" + TimeTrackManager.Duration.ToString() + "$";
			bool showLog2 = SharedUtils.ShowLog;
			if (showLog2)
			{
				SharedUtils.Log(text);
			}
			Thread thread = new Thread(new ParameterizedThreadStart(SimpleSender.SendToServer));
			thread.Start(text);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00016FA0 File Offset: 0x000151A0
		public static void ScreenrecordCMDToSend(bool start)
		{
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("ScreenrecordCMDToSend");
			}
			string text = (start ? "7" : "8") + "$" + TimeTrackManager.Duration.ToString() + "$";
			bool showLog2 = SharedUtils.ShowLog;
			if (showLog2)
			{
				SharedUtils.Log(text);
			}
			Thread thread = new Thread(new ParameterizedThreadStart(SimpleSender.SendToServer));
			thread.Start(text);
		}
	}
}
