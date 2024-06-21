using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UWA;
using UWA.ICSharpCode.SharpZipLib.Zip;

namespace UWACore.Util.NetWork
{
	// Token: 0x0200004C RID: 76
	[ComVisible(false)]
	public static class PkgSendUtil
	{
		// Token: 0x06000352 RID: 850 RVA: 0x00016C6C File Offset: 0x00014E6C
		public static void Pack(string path, string folder)
		{
			UWA.ICSharpCode.SharpZipLib.Zip.FastZip fastZip = new UWA.ICSharpCode.SharpZipLib.Zip.FastZip();
			fastZip.CreateZip(path, folder, true, null);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00016C90 File Offset: 0x00014E90
		public static Client AddTaskToSend(string filePath, string uploadName)
		{
			bool flag = File.Exists(filePath);
			Client result;
			if (flag)
			{
				Client client = new Client(filePath, uploadName, 2);
				ThreadPool.QueueUserWorkItem(new WaitCallback(client.SendFile), new ManualResetEvent(false));
				result = client;
			}
			else
			{
				SharedUtils.LogError("filePath does not exist " + filePath);
				result = null;
			}
			return result;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00016CF4 File Offset: 0x00014EF4
		public static void Delete(string folder, string file)
		{
			bool flag = !string.IsNullOrEmpty(folder) && Directory.Exists(folder);
			if (flag)
			{
				Directory.Delete(folder, true);
			}
			bool flag2 = !string.IsNullOrEmpty(file) && File.Exists(file);
			if (flag2)
			{
				File.Delete(file);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000355 RID: 853 RVA: 0x00016D54 File Offset: 0x00014F54
		public static PkgSendUtil.WorkState State
		{
			get
			{
				return PkgSendUtil._workState;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000356 RID: 854 RVA: 0x00016D74 File Offset: 0x00014F74
		public static float Percent
		{
			get
			{
				bool flag = PkgSendUtil._workState == PkgSendUtil.WorkState.Zipping;
				float result;
				if (flag)
				{
					result = PkgSendUtil._zipP * 0.5f;
				}
				else
				{
					bool flag2 = PkgSendUtil._workState == PkgSendUtil.WorkState.Uploading;
					if (flag2)
					{
						result = 0.5f + PkgSendUtil._uploadP * 0.5f;
					}
					else
					{
						result = -1f;
					}
				}
				return result;
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00016DD8 File Offset: 0x00014FD8
		public static IEnumerator SendFileInZip(string datapath, string uploadName, bool zipped = false, Action ac = null)
		{
			bool flag = PkgSendUtil._workState > PkgSendUtil.WorkState.Idle;
			if (flag)
			{
				yield break;
			}
			PkgSendUtil._zipP = 0f;
			PkgSendUtil._uploadP = 0f;
			Screen.sleepTimeout = -1;
			float startTime = 0f;
			float duration = 0f;
			bool flag2 = !zipped;
			if (flag2)
			{
				PkgSendUtil._workState = PkgSendUtil.WorkState.Zipping;
				yield return new WaitForSeconds(2f);
				startTime = Time.realtimeSinceStartup;
				ZipFileHelper mainZipHelper = new ZipFileHelper(datapath);
				bool flag3 = !mainZipHelper.Do();
				if (flag3)
				{
					bool showLog = SharedUtils.ShowLog;
					if (showLog)
					{
						SharedUtils.Log("ZipFileHelper.Do failed");
					}
					PkgSendUtil._workState = PkgSendUtil.WorkState.FailedInZip;
					yield break;
				}
				bool showLog2 = SharedUtils.ShowLog;
				if (showLog2)
				{
					SharedUtils.Log("Zipping...");
				}
				while (Mathf.Abs(mainZipHelper.Progress - 1f) > 1E-45f)
				{
					PkgSendUtil._zipP = mainZipHelper.Progress;
					yield return null;
				}
				duration = Time.realtimeSinceStartup - startTime;
				bool showLog3 = SharedUtils.ShowLog;
				if (showLog3)
				{
					SharedUtils.Log("Finished in " + duration.ToString("F") + " sec");
				}
				File.WriteAllText(SharedUtils.FinalDataPath + "/zip", "");
				mainZipHelper = null;
			}
			PkgSendUtil._workState = PkgSendUtil.WorkState.Uploading;
			yield return new WaitForSeconds(1f);
			bool sec = true;
			startTime = Time.realtimeSinceStartup;
			Client mainClient = PkgSendUtil.AddTaskToSend(datapath + ".zip", uploadName);
			bool flag4 = mainClient == null;
			if (flag4)
			{
				bool showLog4 = SharedUtils.ShowLog;
				if (showLog4)
				{
					SharedUtils.Log("AddTaskToSend failed");
				}
				PkgSendUtil._workState = PkgSendUtil.WorkState.FailedInUpload;
				yield break;
			}
			bool showLog5 = SharedUtils.ShowLog;
			if (showLog5)
			{
				SharedUtils.Log("Sending...");
			}
			while (Mathf.Abs(mainClient.Progress - 1f) > 1E-45f)
			{
				PkgSendUtil._uploadP = mainClient.Progress;
				bool failed = mainClient.Failed;
				if (failed)
				{
					sec = false;
					break;
				}
				yield return null;
			}
			bool flag5 = sec;
			if (flag5)
			{
				int tryTime = 1;
				for (;;)
				{
					int num = tryTime;
					tryTime = num + 1;
					if (num >= 5)
					{
						break;
					}
					try
					{
						bool flag6 = File.Exists(datapath + ".zip");
						if (flag6)
						{
							File.Delete(datapath + ".zip");
						}
						bool flag7 = Directory.Exists(datapath);
						if (flag7)
						{
							Directory.Delete(datapath, true);
						}
						break;
					}
					catch (Exception)
					{
						bool showLog6 = SharedUtils.ShowLog;
						if (showLog6)
						{
							SharedUtils.Log("Failed to delete " + datapath + " at " + tryTime.ToString());
						}
					}
					yield return new WaitForSeconds(1f);
				}
				duration = Time.realtimeSinceStartup - startTime;
				bool showLog7 = SharedUtils.ShowLog;
				if (showLog7)
				{
					SharedUtils.Log("Finished in " + duration.ToString("F") + " sec");
				}
				PkgSendUtil._workState = PkgSendUtil.WorkState.Success;
			}
			else
			{
				bool showLog8 = SharedUtils.ShowLog;
				if (showLog8)
				{
					SharedUtils.Log("Failed to send, please check network.");
				}
				PkgSendUtil._workState = PkgSendUtil.WorkState.FailedInUpload;
			}
			yield return new WaitForSeconds(2f);
			Screen.sleepTimeout = -2;
			bool flag8 = ac != null;
			if (flag8)
			{
				ac();
			}
			yield break;
		}

		// Token: 0x040001E7 RID: 487
		private static PkgSendUtil.WorkState _workState = PkgSendUtil.WorkState.Idle;

		// Token: 0x040001E8 RID: 488
		private static float _zipP = -1f;

		// Token: 0x040001E9 RID: 489
		private static float _uploadP = -1f;

		// Token: 0x02000101 RID: 257
		public enum WorkState
		{
			// Token: 0x04000670 RID: 1648
			Idle,
			// Token: 0x04000671 RID: 1649
			Zipping,
			// Token: 0x04000672 RID: 1650
			Uploading,
			// Token: 0x04000673 RID: 1651
			Success,
			// Token: 0x04000674 RID: 1652
			FailedInZip,
			// Token: 0x04000675 RID: 1653
			FailedInUpload
		}
	}
}
