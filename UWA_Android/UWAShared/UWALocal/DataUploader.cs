using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;
using UWA;
using UWA.Core;
using UWA.SDK;
using UWALib.Editor.NetWork;
using UWASDK;

namespace UWALocal
{
	// Token: 0x02000012 RID: 18
	[Preserve]
	public static class DataUploader
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00004960 File Offset: 0x00002B60
		public static void OKUBalanceCallback(int balance, int error, string errorMsg)
		{
			bool flag = error != 0;
			if (flag)
			{
				DataUploader.errorInfo = Localization.Instance.Get("Cannot Get Balance");
				bool flag2 = DataUploader._okuInfo.CallBack != null;
				if (flag2)
				{
					DataUploader._okuInfo.CallBack(false, DataUploader.errorInfo);
				}
				DataUploader.s = DataUploader.UploadState.Done;
			}
			else
			{
				bool flag3 = DataUploader.s == DataUploader.UploadState.Logining;
				if (flag3)
				{
					DataUploader.UploadStart(delegate
					{
						DataUploader.s = DataUploader.UploadState.Done;
						bool flag4 = DataUploader._okuInfo.CallBack != null;
						if (flag4)
						{
							DataUploader._okuInfo.CallBack(DataUploader.uploading.Success, global::UploadTool.Get().GetLastRecordId());
						}
					});
				}
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004A04 File Offset: 0x00002C04
		public static void OKUUpdateProjectList(List<string> list)
		{
			bool flag = DataUploader.itemAction != null;
			if (flag)
			{
				GotOnlineState.ProjectNameList = list;
				GotOnlineState.ProjectInd = 0;
				bool flag2 = DataUploader._okuInfo.ProjectId != -1;
				if (flag2)
				{
					bool flag3 = global::UploadTool.Get().HasProjectId(DataUploader._okuInfo.ProjectId);
					if (!flag3)
					{
						DataUploader.errorInfo = "ProjectId not found: " + DataUploader._okuInfo.ProjectId.ToString();
						bool flag4 = DataUploader._okuInfo.CallBack != null;
						if (flag4)
						{
							DataUploader._okuInfo.CallBack(false, DataUploader.errorInfo);
						}
						DataUploader.s = DataUploader.UploadState.Done;
						return;
					}
					DataUploader.itemAction.ProjectId = DataUploader._okuInfo.ProjectId;
				}
				else
				{
					bool flag5 = DataUploader._okuInfo.ProjectName != null;
					if (flag5)
					{
						int projectIdByName = global::UploadTool.Get().GetProjectIdByName(DataUploader._okuInfo.ProjectName);
						bool flag6 = projectIdByName != -1;
						if (!flag6)
						{
							DataUploader.errorInfo = "ProjectName not found: " + DataUploader._okuInfo.ProjectName;
							bool flag7 = DataUploader._okuInfo.CallBack != null;
							if (flag7)
							{
								DataUploader._okuInfo.CallBack(false, DataUploader.errorInfo);
							}
							DataUploader.s = DataUploader.UploadState.Done;
							return;
						}
						DataUploader._okuInfo.ProjectId = projectIdByName;
						DataUploader.itemAction.ProjectId = DataUploader._okuInfo.ProjectId;
					}
				}
				bool flag8 = DataUploader.itemAction.ProjectId != -1;
				if (flag8)
				{
					global::UploadTool.Get().ProjectBalance("GOT_ONLINE", DataUploader.itemAction.ProjectId, new Action<int, int, string>(DataUploader.OKUBalanceCallback));
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004BD8 File Offset: 0x00002DD8
		public static void OKULoginCallback(bool login, int errorCode, string errorMessage)
		{
			if (login)
			{
				GotOnlineState.Send = true;
				GotOnlineState.State = GotOnlineState.eState.Login;
				GotOnlineState.SaveAccount();
				GotOnlineState.SavePassword();
				global::UploadTool.Get().UpdateGotProject(1, new Action<List<string>>(DataUploader.OKUUpdateProjectList));
			}
			else
			{
				DataUploader.errorInfo = "Login error : " + errorMessage;
				bool flag = DataUploader._okuInfo.CallBack != null;
				if (flag)
				{
					DataUploader._okuInfo.CallBack(false, DataUploader.errorInfo);
				}
				DataUploader.s = DataUploader.UploadState.Done;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004C70 File Offset: 0x00002E70
		private static void DoOKULogin(DataUploader.OKUInfo info)
		{
			bool flag = info.TimeLimit < DataUploader.itemAction.duration;
			if (flag)
			{
				DataUploader.errorInfo = string.Concat(new string[]
				{
					"Duration [",
					DataUploader.itemAction.duration.ToString(),
					"] has exceeded the limit [",
					info.TimeLimit.ToString(),
					"]"
				});
				bool flag2 = info.CallBack != null;
				if (flag2)
				{
					info.CallBack(false, DataUploader.errorInfo);
				}
				DataUploader.s = DataUploader.UploadState.Done;
			}
			else
			{
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("DoOKULogin");
				}
				GotOnlineState.Account = info.User;
				GotOnlineState.Password = info.Pwd;
				global::UploadTool.Get().LogSetup();
				bool useToken = info.UseToken;
				if (useToken)
				{
					global::UploadTool.Get().LoginWithUserIdToken(info.UserId, info.LoginToken, new Action<bool, int, string>(DataUploader.OKULoginCallback));
				}
				else
				{
					global::UploadTool.Get().LoginWithCredentials(info.User, info.Pwd, "", false, new Action<bool, int, string>(DataUploader.OKULoginCallback));
				}
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004DAC File Offset: 0x00002FAC
		public static IEnumerator DoUpload(DataUploader.UploadingInfo uploading, DataUploader.ItemAction item, Action ac)
		{
			uploading.CurrentPercent = 0f;
			string pkgName = item.pkgName;
			string testTime = item.testTime;
			string device = item.deviceModel.Replace(" ", "");
			string testMode = item.testMode;
			float startTime = Time.realtimeSinceStartup;
			uploading.ZipHelper = new ZipFileHelper(item.dataPath);
			uploading.ZipHelper.skipScreenshot = !DataUploader.uploadScreen;
			try
			{
				File.WriteAllText(item.dataPath + "/upload.json", "{\"screenshot\":" + (DataUploader.uploadScreen ? "true" : "false") + "}");
			}
			catch (Exception)
			{
			}
			bool flag = !uploading.ZipHelper.Do();
			if (flag)
			{
				uploading.GotEditorFinish = DataUploader.eGotEditorFinish.CannotZip;
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.LogError("ZipFileHelper.Do failed");
				}
				bool flag2 = ac != null;
				if (flag2)
				{
					ac();
				}
				yield break;
			}
			bool showLog2 = SharedUtils.ShowLog;
			if (showLog2)
			{
				SharedUtils.Log("Zipping...");
			}
			while (Mathf.Abs(uploading.ZipHelper.Progress - 1f) > 1E-45f)
			{
				uploading.CurrentPercent = 0.5f * uploading.ZipHelper.Progress;
				yield return null;
			}
			float leftWeight = 0.5f;
			bool flag3 = item.SendToLocalServer && item.SendToUwaOnline;
			if (flag3)
			{
				leftWeight = 0.25f;
			}
			yield return new WaitForSeconds(1f);
			string zipPath = item.dataPath + ".zip";
			float duration = Time.realtimeSinceStartup - startTime;
			bool showLog3 = SharedUtils.ShowLog;
			if (showLog3)
			{
				SharedUtils.Log("Zip finished in " + duration.ToString("F") + " sec");
			}
			startTime = Time.realtimeSinceStartup;
			bool sendToLocalServer = item.SendToLocalServer;
			if (sendToLocalServer)
			{
				uploading.LocalClient = LogSendController.AddTaskToSend(zipPath, string.Concat(new string[]
				{
					testMode,
					"/",
					device,
					"-",
					pkgName,
					"-",
					testTime,
					".zip"
				}));
				bool flag4 = uploading.LocalClient == null;
				if (flag4)
				{
					uploading.GotEditorFinish = DataUploader.eGotEditorFinish.CannotAddClient;
					bool showLog4 = SharedUtils.ShowLog;
					if (showLog4)
					{
						SharedUtils.LogError("LogSendController.AddTaskToSend failed");
					}
					bool flag5 = ac != null;
					if (flag5)
					{
						ac();
					}
					yield break;
				}
				bool showLog5 = SharedUtils.ShowLog;
				if (showLog5)
				{
					SharedUtils.Log("Sending to local...");
				}
				while (Mathf.Abs(uploading.LocalClient.Progress - 1f) > 1E-45f)
				{
					uploading.CurrentPercent = 0.5f + leftWeight * uploading.LocalClient.Progress;
					bool failed = uploading.LocalClient.Failed;
					if (failed)
					{
						uploading.GotEditorFinish = DataUploader.eGotEditorFinish.CannotSend;
						bool showLog6 = SharedUtils.ShowLog;
						if (showLog6)
						{
							SharedUtils.Log("failed to send to local ...");
						}
						uploading.Success = false;
						bool flag6 = ac != null;
						if (flag6)
						{
							ac();
						}
						break;
					}
					yield return null;
				}
				uploading.CurrentPercent = 0.5f + leftWeight;
				duration = Time.realtimeSinceStartup - startTime;
				bool showLog7 = SharedUtils.ShowLog;
				if (showLog7)
				{
					SharedUtils.Log("Send to local finished in " + duration.ToString("F") + " sec");
				}
				yield return new WaitForSeconds(2f);
			}
			startTime = Time.realtimeSinceStartup;
			bool sendToUwaOnline = item.SendToUwaOnline;
			if (sendToUwaOnline)
			{
				bool showLog8 = SharedUtils.ShowLog;
				if (showLog8)
				{
					SharedUtils.Log("Sending to online...");
				}
				yield return global::UploadTool.Get().DoUpload(uploading, item, leftWeight);
				bool flag7 = uploading.GotOnlineFinish != SyncDataManager.FinishType.Success;
				if (flag7)
				{
					uploading.Success = false;
					DataUploader.errorInfo = Localization.Instance.Get(uploading.GotOnlineFinish.ToString());
				}
				uploading.CurrentPercent = 1f;
				duration = Time.realtimeSinceStartup - startTime;
				bool showLog9 = SharedUtils.ShowLog;
				if (showLog9)
				{
					SharedUtils.Log("Send to online finished in " + duration.ToString("F") + " sec");
				}
			}
			yield return new WaitForSeconds(2f);
			bool success = uploading.Success;
			if (success)
			{
				try
				{
					File.Delete(zipPath);
					Directory.Delete(item.dataPath, true);
				}
				catch (Exception ex)
				{
				}
			}
			else
			{
				bool sendToUwaOnline2 = item.SendToUwaOnline;
				if (sendToUwaOnline2)
				{
					SyncDataManager.FinishType error = SyncDataManager.FinishType.Unset;
					bool synced = SyncDataManager.CheckSynced(item.dataPath, out error);
				}
			}
			yield return new WaitForSeconds(2f);
			bool flag8 = ac != null;
			if (flag8)
			{
				ac();
			}
			yield break;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00004DCC File Offset: 0x00002FCC
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00004DEC File Offset: 0x00002FEC
		public static DataUploader.UploadState s
		{
			get
			{
				return DataUploader._s;
			}
			set
			{
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("UploadState " + DataUploader._s.ToString() + " > " + value.ToString());
				}
				DataUploader._s = value;
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004E44 File Offset: 0x00003044
		public static void CheckOldData()
		{
			DataUploader.OldData = SharedUtils.GetOldData(SdkCtrlData.Instance.SdkMode);
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("CheckOldData : " + DataUploader.OldData.Count.ToString());
			}
			DataUploader.ShowOldTip = "检测到本地有 " + DataUploader.OldData.Count.ToString() + " 份测试数据未上传，\n是否上传？";
			bool flag = DataUploader.OldData.Count > 0;
			if (flag)
			{
				DataUploader.s = DataUploader.UploadState.ShowOld;
			}
			else
			{
				DataUploader.s = DataUploader.UploadState.Idle;
				bool flag2 = SdkUIMgr.Get();
				if (flag2)
				{
					SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.MODE);
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00004F04 File Offset: 0x00003104
		public static void DeleteAllOldData()
		{
			bool flag = DataUploader.OldData == null || DataUploader.OldData.Count == 0;
			if (!flag)
			{
				for (int i = 0; i < DataUploader.OldData.Count; i++)
				{
					Directory.Delete(DataUploader.OldData[i], true);
				}
				DataUploader.s = DataUploader.UploadState.Idle;
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004F78 File Offset: 0x00003178
		public static void ChangeProjectInd(int ind)
		{
			bool flag = ind < 0 || ind >= GotOnlineState.ProjectNameList.Count;
			if (!flag)
			{
				GotOnlineState.ProjectInd = ind;
				DataUploader.itemAction.ProjectId = global::UploadTool.Get().GetProjectIdByDisplayIndex(GotOnlineState.ProjectInd);
				bool flag2 = DataUploader.itemAction.ProjectId != -1;
				if (flag2)
				{
					global::UploadTool.Get().ProjectBalance("GOT_ONLINE", DataUploader.itemAction.ProjectId, new Action<int, int, string>(DataUploader.BalanceCallback));
				}
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005014 File Offset: 0x00003214
		public static void CheckProDataInfo(string dataPath)
		{
			global::UploadTool.Get().GotOnlineCheckDataInfo(dataPath);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005024 File Offset: 0x00003224
		public static bool CompareProBalances(int dataType)
		{
			return global::UploadTool.Get().GotOnlineCompareBalances(dataType);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005048 File Offset: 0x00003248
		public static void UpdateProjectList(List<string> list)
		{
			bool flag = DataUploader.itemAction != null;
			if (flag)
			{
				GotOnlineState.ProjectNameList = list;
				GotOnlineState.ProjectInd = 0;
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005078 File Offset: 0x00003278
		[Preserve]
		public static void TryOneKeyUploadPoco(Action<bool, string> callback, object[] uploadInfo)
		{
			bool flag = uploadInfo == null || (uploadInfo.Length != 4 && uploadInfo.Length != 5);
			if (flag)
			{
				DataUploader.errorInfo = "Uploadinfo Count.";
				bool flag2 = callback != null;
				if (flag2)
				{
					callback(false, DataUploader.errorInfo);
				}
				DataUploader.s = DataUploader.UploadState.Done;
			}
			else
			{
				bool startOKU = DataUploader.StartOKU;
				if (startOKU)
				{
					DataUploader.errorInfo = "Already Started.";
					bool flag3 = callback != null;
					if (flag3)
					{
						callback(false, DataUploader.errorInfo);
					}
					DataUploader.s = DataUploader.UploadState.Done;
				}
				else
				{
					try
					{
						DataUploader._okuInfo.UseToken = true;
						DataUploader._okuInfo.UserId = (uploadInfo[0] as string);
						DataUploader._okuInfo.LoginToken = (uploadInfo[1] as string);
						DataUploader._okuInfo.ProjectId = -1;
						DataUploader._okuInfo.ProjectName = null;
						string text = "";
						bool flag4 = uploadInfo.Length == 5;
						if (flag4)
						{
							text = (uploadInfo[4] as string);
							bool flag5 = !SharedUtils.HasNote() && !string.IsNullOrEmpty(text);
							if (flag5)
							{
								SharedUtils.SetNote(text);
							}
						}
						bool flag6 = uploadInfo[2].GetType() == typeof(int);
						if (flag6)
						{
							DataUploader._okuInfo.ProjectId = (int)uploadInfo[2];
						}
						else
						{
							bool flag7 = uploadInfo[2].GetType() == typeof(string);
							if (flag7)
							{
								DataUploader._okuInfo.ProjectName = (string)uploadInfo[2];
							}
						}
						DataUploader._okuInfo.TimeLimit = (int)uploadInfo[3];
						DataUploader._okuInfo.CallBack = callback;
						bool showLog = SharedUtils.ShowLog;
						if (showLog)
						{
							SharedUtils.Log(string.Concat(new string[]
							{
								"TryOneKeyUploadPoco StartOKU : ",
								DataUploader._okuInfo.ProjectId.ToString(),
								",",
								DataUploader._okuInfo.ProjectName,
								",",
								DataUploader._okuInfo.TimeLimit.ToString(),
								",",
								text
							}));
						}
						DataUploader.StartOKU = true;
					}
					catch (Exception ex)
					{
						DataUploader.errorInfo = ex.ToString();
						bool flag8 = callback != null;
						if (flag8)
						{
							callback(false, DataUploader.errorInfo);
						}
						DataUploader.s = DataUploader.UploadState.Done;
					}
				}
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005308 File Offset: 0x00003508
		[Preserve]
		public static void TryOneKeyUploadUP(Action<bool, string> callback, object[] uploadInfo)
		{
			bool flag = uploadInfo == null || uploadInfo.Length != 4;
			if (flag)
			{
				DataUploader.errorInfo = "Wrong Uploadinfo Count.";
				bool flag2 = callback != null;
				if (flag2)
				{
					callback(false, DataUploader.errorInfo);
				}
				DataUploader.s = DataUploader.UploadState.Done;
			}
			else
			{
				bool startOKU = DataUploader.StartOKU;
				if (startOKU)
				{
					DataUploader.errorInfo = "Already Started.";
					bool flag3 = callback != null;
					if (flag3)
					{
						callback(false, DataUploader.errorInfo);
					}
					DataUploader.s = DataUploader.UploadState.Done;
				}
				else
				{
					try
					{
						DataUploader._okuInfo.UseToken = false;
						DataUploader._okuInfo.User = (uploadInfo[0] as string);
						DataUploader._okuInfo.Pwd = (uploadInfo[1] as string);
						DataUploader._okuInfo.ProjectId = -1;
						DataUploader._okuInfo.ProjectName = null;
						bool flag4 = uploadInfo[2].GetType() == typeof(int);
						if (flag4)
						{
							DataUploader._okuInfo.ProjectId = (int)uploadInfo[2];
						}
						else
						{
							bool flag5 = uploadInfo[2].GetType() == typeof(string);
							if (flag5)
							{
								DataUploader._okuInfo.ProjectName = (string)uploadInfo[2];
							}
						}
						DataUploader._okuInfo.TimeLimit = (int)uploadInfo[3];
						DataUploader._okuInfo.CallBack = callback;
						bool showLog = SharedUtils.ShowLog;
						if (showLog)
						{
							SharedUtils.Log(string.Concat(new string[]
							{
								"TryOneKeyUploadUP StartOKU : ",
								DataUploader._okuInfo.ProjectId.ToString(),
								",",
								DataUploader._okuInfo.ProjectName,
								",",
								DataUploader._okuInfo.TimeLimit.ToString()
							}));
						}
						DataUploader.StartOKU = true;
					}
					catch (Exception ex)
					{
						DataUploader.errorInfo = ex.ToString();
						bool flag6 = callback != null;
						if (flag6)
						{
							callback(false, DataUploader.errorInfo);
						}
						DataUploader.s = DataUploader.UploadState.Done;
					}
				}
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000552C File Offset: 0x0000372C
		[Preserve]
		public static void Note(string note)
		{
			SharedUtils.SetNote(note);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005538 File Offset: 0x00003738
		[Preserve]
		public static void TryOneKeyUpload(Action<bool> callback, object[] uploadInfo)
		{
			DataUploader.TryOneKeyUploadUP(delegate(bool x, string y)
			{
				bool flag = callback != null;
				if (flag)
				{
					callback(x);
				}
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("TryOneKeyUpload : " + x.ToString() + "," + y);
				}
			}, uploadInfo);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000556C File Offset: 0x0000376C
		public static void DoOneKeyUpload()
		{
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("DoOneKeyUpload");
			}
			DataUploader.s = DataUploader.UploadState.Logining;
			DataUploader.DoOKULogin(DataUploader._okuInfo);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000055A8 File Offset: 0x000037A8
		public static void StaticInit()
		{
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000055AC File Offset: 0x000037AC
		public static void UploadSetup(string dataPath)
		{
			Localization.Instance.SetLocale(Localization.eLocale.zh_CN);
			global::UploadTool.Get().UploadSetup();
			int duration = 0;
			int num = 0;
			SyncDataManager.SystemInfo systemInfo = null;
			SyncDataManager.GetAppNameTestTimePackageNameKey(dataPath, out systemInfo);
			string pkgName = systemInfo.pkgName;
			string testMode = systemInfo.testMode;
			string deviceModel = systemInfo.deviceModel;
			string testTime = systemInfo.testTime;
			int platform = systemInfo.platform;
			bool dev = systemInfo.dev;
			bool flag = !SyncDataManager.GetFrameAndDurationInDoneOrLast(dataPath, out num, out duration);
			if (flag)
			{
				num = -1;
				duration = -1;
			}
			int projectTypeByName = (int)SyncDataManager.GetProjectTypeByName(testMode, dev);
			DataUploader.itemAction = new DataUploader.ItemAction
			{
				dataPath = dataPath,
				pkgName = pkgName,
				testMode = testMode,
				deviceModel = deviceModel,
				testTime = testTime,
				platform = platform,
				dataType = projectTypeByName,
				duration = duration,
				ProjectId = -1
			};
			DataUploader.s = DataUploader.UploadState.Preparing;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000056A4 File Offset: 0x000038A4
		public static void BalanceCallback(int balance, int error, string errorMsg)
		{
			bool flag = error != 0;
			if (flag)
			{
				GotOnlineState.BalanceInfo = string.Format(Localization.Instance.Get("Cannot Get Balance"), new object[0]);
			}
			else
			{
				bool flag2 = DataUploader.itemAction.ProjectId != -1;
				if (flag2)
				{
					int num = balance / 60;
					int num2 = balance % 60;
					GotOnlineState.BalanceInfo = string.Format(Localization.Instance.Get("Project Balance Tip"), num, num2);
					bool flag3 = balance == int.MaxValue;
					if (flag3)
					{
						GotOnlineState.BalanceInfo = null;
					}
				}
				bool flag4 = global::UploadTool.Get().NeedBalance(DataUploader.itemAction.dataType);
				if (flag4)
				{
					GotOnlineState.bBalanceEnough = (balance >= DataUploader.itemAction.duration);
					bool flag5 = balance == 0;
					if (flag5)
					{
						GotOnlineState.bBalanceEnough = false;
					}
					GotOnlineState.bGetBalance = true;
				}
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005794 File Offset: 0x00003994
		public static void LoginCallback(bool login, int errorCode, string errorMessage)
		{
			if (login)
			{
				global::UploadTool.Get().CreateProjectCheck(delegate(bool x)
				{
					GotOnlineState.CanCreateProj = x;
					if (x)
					{
						GotOnlineState.State = GotOnlineState.eState.Login;
						GotOnlineState.SaveAccount();
						bool bSavePassword = GotOnlineState.bSavePassword;
						if (bSavePassword)
						{
							GotOnlineState.SavePassword();
						}
						else
						{
							GotOnlineState.ClearPassword();
						}
						global::UploadTool.Get().UpdateGotProject(1, new Action<List<string>>(DataUploader.UpdateProjectList));
						GotOnlineState.Info = null;
					}
					else
					{
						GotOnlineState.Info = "账号未认证，前往认证获得会员试用权益";
					}
				});
			}
			else
			{
				bool flag = errorCode == 0;
				if (flag)
				{
					GotOnlineState.Info = Localization.Instance.Get("Check net");
				}
				else
				{
					GotOnlineState.Info = Localization.Instance.Get("Login Rsp " + errorCode.ToString());
				}
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005828 File Offset: 0x00003A28
		public static void UploadStart(Action callback)
		{
			DataUploader.s = DataUploader.UploadState.Uploading;
			DataUploader.uploading = new DataUploader.UploadingInfo();
			DataUploader.uploading.Success = true;
			DataUploader.uploading.CurrentPercent = 0f;
			DataUploader.errorInfo = "";
			Screen.sleepTimeout = -1;
			DataUploader.itemAction.SendToLocalServer = GotEditorState.Send;
			DataUploader.itemAction.SendToUwaOnline = GotOnlineState.Send;
			SdkUIMgr.Get().StartCoroutine(DataUploader.DoUpload(DataUploader.uploading, DataUploader.itemAction, callback));
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000058B0 File Offset: 0x00003AB0
		public static void UploadReset()
		{
			GotEditorState.Connected = false;
			GotEditorState.Send = false;
			GotOnlineState.Send = false;
			GotOnlineState.Show = true;
			GotEditorState.Show = false;
			GotOnlineState.State = GotOnlineState.eState.Connected;
			DataUploader.s = DataUploader.UploadState.Preparing;
		}

		// Token: 0x0400006D RID: 109
		public static bool isOneClick = false;

		// Token: 0x0400006E RID: 110
		public static eSdkMode TestType = eSdkMode.None;

		// Token: 0x0400006F RID: 111
		public static bool uploadScreen = true;

		// Token: 0x04000070 RID: 112
		public static bool StartOKU = false;

		// Token: 0x04000071 RID: 113
		private static DataUploader.OKUInfo _okuInfo = new DataUploader.OKUInfo();

		// Token: 0x04000072 RID: 114
		private static DataUploader.UploadState _s = DataUploader.UploadState.Idle;

		// Token: 0x04000073 RID: 115
		public static DataUploader.UploadingInfo uploading;

		// Token: 0x04000074 RID: 116
		public static string errorInfo = null;

		// Token: 0x04000075 RID: 117
		public static DataUploader.ItemAction itemAction = null;

		// Token: 0x04000076 RID: 118
		public static List<string> OldData = null;

		// Token: 0x04000077 RID: 119
		public static string ShowOldTip = "";

		// Token: 0x020000E6 RID: 230
		private class OKUInfo
		{
			// Token: 0x04000601 RID: 1537
			public bool UseToken;

			// Token: 0x04000602 RID: 1538
			public string UserId;

			// Token: 0x04000603 RID: 1539
			public string LoginToken;

			// Token: 0x04000604 RID: 1540
			public string User;

			// Token: 0x04000605 RID: 1541
			public string Pwd;

			// Token: 0x04000606 RID: 1542
			public int ProjectId;

			// Token: 0x04000607 RID: 1543
			public string ProjectName;

			// Token: 0x04000608 RID: 1544
			public int TimeLimit;

			// Token: 0x04000609 RID: 1545
			public Action<bool, string> CallBack;
		}

		// Token: 0x020000E7 RID: 231
		public class ItemAction
		{
			// Token: 0x0400060A RID: 1546
			public string pkgName;

			// Token: 0x0400060B RID: 1547
			public string testTime;

			// Token: 0x0400060C RID: 1548
			public string deviceModel;

			// Token: 0x0400060D RID: 1549
			public string testMode;

			// Token: 0x0400060E RID: 1550
			public string dataPath;

			// Token: 0x0400060F RID: 1551
			public int duration;

			// Token: 0x04000610 RID: 1552
			public bool SendToLocalServer;

			// Token: 0x04000611 RID: 1553
			public bool SendToUwaOnline;

			// Token: 0x04000612 RID: 1554
			public int ProjectId;

			// Token: 0x04000613 RID: 1555
			public int platform;

			// Token: 0x04000614 RID: 1556
			public int dataType;
		}

		// Token: 0x020000E8 RID: 232
		public enum eGotEditorFinish
		{
			// Token: 0x04000616 RID: 1558
			Finish,
			// Token: 0x04000617 RID: 1559
			CannotZip,
			// Token: 0x04000618 RID: 1560
			CannotAddClient,
			// Token: 0x04000619 RID: 1561
			CannotSend
		}

		// Token: 0x020000E9 RID: 233
		public class UploadingInfo
		{
			// Token: 0x0400061A RID: 1562
			public float CurrentPercent;

			// Token: 0x0400061B RID: 1563
			public ZipFileHelper ZipHelper;

			// Token: 0x0400061C RID: 1564
			public Client LocalClient;

			// Token: 0x0400061D RID: 1565
			public bool Success;

			// Token: 0x0400061E RID: 1566
			public DataUploader.eGotEditorFinish GotEditorFinish;

			// Token: 0x0400061F RID: 1567
			public SyncDataManager.FinishType GotOnlineFinish;
		}

		// Token: 0x020000EA RID: 234
		public enum UploadState
		{
			// Token: 0x04000621 RID: 1569
			ShowOld,
			// Token: 0x04000622 RID: 1570
			DeleteOld,
			// Token: 0x04000623 RID: 1571
			Idle,
			// Token: 0x04000624 RID: 1572
			Preparing,
			// Token: 0x04000625 RID: 1573
			Logining,
			// Token: 0x04000626 RID: 1574
			Selecting,
			// Token: 0x04000627 RID: 1575
			TimeConfirm,
			// Token: 0x04000628 RID: 1576
			Uploading,
			// Token: 0x04000629 RID: 1577
			Uploaded,
			// Token: 0x0400062A RID: 1578
			Close,
			// Token: 0x0400062B RID: 1579
			Done
		}
	}
}
