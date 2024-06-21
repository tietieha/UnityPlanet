using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UWA.Core;
using UWACore.Util.NetWork;
using UWALocal;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000051 RID: 81
	[ComVisible(false)]
	public static class SyncDataManager
	{
		// Token: 0x0600035B RID: 859 RVA: 0x0001DA90 File Offset: 0x0001BC90
		public static string GetCoreVersion()
		{
			try
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0001DADC File Offset: 0x0001BCDC
		// (set) Token: 0x0600035D RID: 861 RVA: 0x0001DAFC File Offset: 0x0001BCFC
		public static SyncDataManager.UploadState State
		{
			get
			{
				return SyncDataManager._state;
			}
			set
			{
				UwaWebsiteClient.Log("State : " + SyncDataManager._state.ToString() + " -> " + value.ToString());
				SyncDataManager._state = value;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0001DB38 File Offset: 0x0001BD38
		// (set) Token: 0x0600035F RID: 863 RVA: 0x0001DB58 File Offset: 0x0001BD58
		public static SyncDataManager.FinishType Finish
		{
			get
			{
				return SyncDataManager._finish;
			}
			set
			{
				UwaWebsiteClient.Log("Finish : " + SyncDataManager._finish.ToString() + " -> " + value.ToString());
				SyncDataManager._finish = value;
			}
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0001DB94 File Offset: 0x0001BD94
		public static void Init()
		{
			bool inited = SyncDataManager._inited;
			if (!inited)
			{
				SyncDataManager._inited = true;
				SyncDataManager.UpdateGotProject();
			}
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0001DBC4 File Offset: 0x0001BDC4
		public static void AddTask(string folder, int projectId, string userNote, bool needZip = true)
		{
			UwaWebsiteClient.Log("Add Task:" + folder + "," + projectId.ToString());
			SyncDataManager.Tasks.Enqueue(new SyncDataManager.TaskInfo
			{
				FolderPath = folder,
				ProjectId = projectId,
				UserNote = userNote,
				ProjectName = null,
				NeedZip = needZip
			});
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0001DC28 File Offset: 0x0001BE28
		public static void AddTask(string folder, string projectName, string userNote, bool needZip = true)
		{
			UwaWebsiteClient.Log("Add Task:" + folder + "," + projectName);
			SyncDataManager.Tasks.Enqueue(new SyncDataManager.TaskInfo
			{
				FolderPath = folder,
				ProjectId = -2,
				UserNote = userNote,
				ProjectName = projectName,
				NeedZip = needZip
			});
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0001DC88 File Offset: 0x0001BE88
		public static void AddTask(string folder, int projectId, bool needZip = true)
		{
			UwaWebsiteClient.Log("Add Task:" + folder + "," + projectId.ToString());
			SyncDataManager.Tasks.Enqueue(new SyncDataManager.TaskInfo
			{
				FolderPath = folder,
				ProjectId = projectId,
				ProjectName = null,
				NeedZip = needZip
			});
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
		public static void AddTask(string folder, string projectName, bool needZip = true)
		{
			UwaWebsiteClient.Log("Add Task:" + folder + "," + projectName);
			SyncDataManager.Tasks.Enqueue(new SyncDataManager.TaskInfo
			{
				FolderPath = folder,
				ProjectId = -2,
				ProjectName = projectName,
				NeedZip = needZip
			});
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0001DD3C File Offset: 0x0001BF3C
		public static int TaskCount
		{
			get
			{
				return SyncDataManager.Tasks.Count;
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0001DD60 File Offset: 0x0001BF60
		public static void UpdateScanProject()
		{
			SyncDataManager._lastProjectUpdate = DateTime.Now;
			bool isLoggedIn = UwaWebsiteClient.IsLoggedIn;
			if (isLoggedIn)
			{
				UwaWebsiteClient.GotProjectAll(new UwaWebsiteClient.OnDoneCallback(SyncDataManager.OnGotProject));
			}
			else
			{
				bool flag = !UwaWebsiteClient.IsLoggedIn;
				if (flag)
				{
					SyncDataManager.UpdateProjectName2Id(null);
				}
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0001DDBC File Offset: 0x0001BFBC
		public static void UpdateGotProject()
		{
			SyncDataManager._lastProjectUpdate = DateTime.Now;
			bool isLoggedIn = UwaWebsiteClient.IsLoggedIn;
			if (isLoggedIn)
			{
				UwaWebsiteClient.GotProjectAll(new UwaWebsiteClient.OnDoneCallback(SyncDataManager.OnGotProject));
			}
			else
			{
				bool flag = !UwaWebsiteClient.IsLoggedIn;
				if (flag)
				{
					SyncDataManager.UpdateProjectName2Id(null);
				}
			}
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0001DE18 File Offset: 0x0001C018
		public static void Update()
		{
			bool flag = (DateTime.Now - SyncDataManager._lastUpdate).TotalSeconds < 0.2;
			if (!flag)
			{
				SyncDataManager._lastUpdate = DateTime.Now;
				bool flag2 = SyncDataManager.State == SyncDataManager.UploadState.Idle || SyncDataManager.State == SyncDataManager.UploadState.Finish;
				if (flag2)
				{
					bool toStop = SyncDataManager.ToStop;
					if (toStop)
					{
						SyncDataManager.ToStop = false;
						SyncDataManager.Tasks.Clear();
					}
					bool flag3 = SyncDataManager.Tasks.Count == 0;
					if (flag3)
					{
						SyncDataManager.Progress = 0f;
						bool flag4 = SyncDataManager.State > SyncDataManager.UploadState.Idle;
						if (flag4)
						{
							SyncDataManager.State = SyncDataManager.UploadState.Idle;
						}
						SyncDataManager.OnRepaintWindow = null;
						return;
					}
					SyncDataManager._currentTask = SyncDataManager.Tasks.Dequeue();
					bool flag5 = string.IsNullOrEmpty(UwaWebsiteClient.LogPath);
					if (flag5)
					{
						UwaWebsiteClient.LogPath = SyncDataManager._currentTask.FolderPath + ".sync.log";
					}
					UwaWebsiteClient.Log("Core Version: " + SyncDataManager.GetCoreVersion());
					UwaWebsiteClient.Log("GetTask: " + SyncDataManager._currentTask.FolderPath);
					bool flag6 = SyncDataManager.OnRepaintWindow != null;
					if (flag6)
					{
						SyncDataManager.OnRepaintWindow();
					}
					SyncDataManager.Finish = SyncDataManager.FinishType.Unset;
					SyncDataManager.FinishType finishType = SyncDataManager.FinishType.Failed;
					bool flag7 = !UwaWebsiteClient.Debug && SyncDataManager.CheckSynced(SyncDataManager._currentTask.FolderPath, out finishType);
					if (flag7)
					{
						bool flag8 = finishType == SyncDataManager.FinishType.Success || finishType == SyncDataManager.FinishType.Unhash || finishType == SyncDataManager.FinishType.DataBroken || finishType == SyncDataManager.FinishType.Resubmit;
						if (flag8)
						{
							SyncDataManager.State = SyncDataManager.UploadState.Finish;
							string content = "Already synced " + ((finishType == SyncDataManager.FinishType.Success) ? "successfully." : ("with error : " + finishType.ToString()));
							UwaWebsiteClient.Log(content);
							UwaWebsiteClient.LogPath = null;
							return;
						}
					}
					bool flag9 = File.Exists(SyncDataManager._currentTask.FolderPath + "/projectinfo");
					if (flag9)
					{
						SyncDataManager._currentTask.ProjectType = SyncDataManager.ProjectType.pipeline;
					}
					bool flag10 = File.Exists(SyncDataManager._currentTask.FolderPath + "/result.json");
					if (flag10)
					{
						SyncDataManager._currentTask.ProjectType = SyncDataManager.ProjectType.assetbundle;
					}
					bool flag11 = true;
					bool flag12 = SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.pipeline;
					bool flag13;
					string guid;
					if (flag12)
					{
						SyncDataManager.PPLDataInfo ppldataInfo = null;
						flag13 = !SyncDataManager.GetProjNameTestTimeCompayName(SyncDataManager._currentTask.FolderPath, out ppldataInfo);
						SyncDataManager._currentTask.Duration = 0;
						SyncDataManager._currentTask.Platform = ppldataInfo.platform;
						SyncDataManager._currentTask.ProjectType = SyncDataManager.GetProjectTypeByName(ppldataInfo.testMode, true);
						SyncDataManager._currentTask.Engine = ppldataInfo.engine;
						SyncDataManager._currentTask.License = ppldataInfo.license;
						guid = ppldataInfo.GetGUID();
					}
					else
					{
						bool flag14 = SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.assetbundle;
						if (flag14)
						{
							SyncDataManager.ABDataIfo abdataIfo = null;
							flag13 = !SyncDataManager.GetABInfo(SyncDataManager._currentTask.FolderPath, out abdataIfo);
							SyncDataManager._currentTask.Platform = abdataIfo.platform;
							SyncDataManager._currentTask.ProjectType = SyncDataManager.GetProjectTypeByName(abdataIfo.testMode, true);
							SyncDataManager._currentTask.Engine = abdataIfo.engine;
							SyncDataManager._currentTask.EngineVersion = abdataIfo.engineVersion;
							SyncDataManager._currentTask.License = abdataIfo.license;
							guid = abdataIfo.GetGUID();
						}
						else
						{
							SyncDataManager.SystemInfo systemInfo = null;
							int num = 0;
							int duration = 0;
							flag13 = (!SyncDataManager.GetAppNameTestTimePackageNameKey(SyncDataManager._currentTask.FolderPath, out systemInfo) || !SyncDataManager.GetFrameAndDurationInDoneOrLast(SyncDataManager._currentTask.FolderPath, out num, out duration));
							SyncDataManager._unrealStats = systemInfo.unrealStats;
							bool flag15 = systemInfo.engine == 2;
							if (flag15)
							{
								string fileName = Path.GetFileName(systemInfo.unrealStats);
								string[] files = Directory.GetFiles(SyncDataManager._currentTask.FolderPath, fileName + "*", SearchOption.TopDirectoryOnly);
								bool flag16 = files.Length == 0;
								if (flag16)
								{
									flag11 = false;
								}
							}
							string fileString = SyncDataManager.GetFileString(SyncDataManager._currentTask.FolderPath, "note");
							string fileString2 = SyncDataManager.GetFileString(SyncDataManager._currentTask.FolderPath, "meta");
							bool flag17 = string.IsNullOrEmpty(SyncDataManager._currentTask.UserNote) && fileString != null;
							if (flag17)
							{
								SyncDataManager._currentTask.UserNote = fileString;
							}
							bool flag18 = !string.IsNullOrEmpty(fileString2);
							if (flag18)
							{
								SyncDataManager._currentTask.Meta = fileString2;
							}
							SyncDataManager._currentTask.Duration = duration;
							SyncDataManager._currentTask.Platform = systemInfo.platform;
							SyncDataManager._currentTask.ProjectType = SyncDataManager.GetProjectTypeByName(systemInfo.testMode, systemInfo.dev);
							SyncDataManager._currentTask.Engine = systemInfo.engine;
							SyncDataManager._currentTask.DeviceModel = systemInfo.deviceModel;
							guid = systemInfo.GetGUID();
						}
					}
					string path = SyncDataManager._currentTask.FolderPath + "/license";
					bool flag19 = File.Exists(path);
					if (flag19)
					{
						string text = File.ReadAllText(path);
						SyncDataManager._currentTask.License = text.Replace("\r", "").Replace("\n", "").Replace(" ", "");
					}
					SyncDataManager._currentTask.Guid = guid;
					SyncDataManager._currentTask.DataKey = SyncDataManager.GetOrCreateDataKey(guid);
					bool flag20 = !string.IsNullOrEmpty(SyncDataManager._currentTask.License);
					if (flag20)
					{
						SyncDataManager._currentTask.License = Regex.Replace(SyncDataManager._currentTask.License, "\\s", "");
					}
					bool flag21 = flag13;
					if (flag21)
					{
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
						SyncDataManager.Finish = SyncDataManager.FinishType.DataBroken;
						UwaWebsiteClient.Log("Task DataBroken.");
						SyncDataManager.FinishTask();
						return;
					}
					bool flag22 = !flag11 && SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.overview;
					if (flag22)
					{
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
						SyncDataManager.Finish = SyncDataManager.FinishType.StatNotFound;
						UwaWebsiteClient.Log("Task StatNotFound.");
						SyncDataManager.FinishTask();
						return;
					}
					bool flag23 = SyncDataManager._currentTask.DataKey == null || SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.unknown;
					if (flag23)
					{
						SyncDataManager.Finish = SyncDataManager.FinishType.Unhash;
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
						SyncDataManager.FinishTask();
						return;
					}
					UwaWebsiteClient.Log(string.Concat(new string[]
					{
						"Task DataKey: ",
						SyncDataManager._currentTask.DataKey,
						"; ProjectType: ",
						SyncDataManager._currentTask.ProjectType.ToString(),
						"; ProjectId: ",
						SyncDataManager._currentTask.ProjectId.ToString(),
						"; Platform: ",
						SyncDataManager._currentTask.Platform.ToString(),
						"; Engine: ",
						SyncDataManager._currentTask.Engine.ToString(),
						"; Duration: ",
						SyncDataManager._currentTask.Duration.ToString(),
						"; DeviceModel: ",
						SyncDataManager._currentTask.DeviceModel,
						"; UserNote: ",
						SyncDataManager._currentTask.UserNote,
						"; License: ",
						SyncDataManager._currentTask.License,
						"; Meta: ",
						SyncDataManager._currentTask.Meta
					}));
					SyncDataManager.State = SyncDataManager.UploadState.GettingProjectInfo;
					SyncDataManager.State = SyncDataManager.UploadState.GotgProjectInfo;
					UwaWebsiteClient.Log("GettingProjectInfo.");
				}
				bool flag24 = SyncDataManager.State == SyncDataManager.UploadState.GotgProjectInfo;
				if (flag24)
				{
					bool flag25 = SyncDataManager._currentTask.ProjectId == -2;
					if (flag25)
					{
						UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindByName(SyncDataManager._currentTask.ProjectName);
						bool flag26 = groupProjectInfo != null;
						if (flag26)
						{
							SyncDataManager._currentTask.ProjectId = groupProjectInfo.projectGroupId;
						}
						UwaWebsiteClient.Log("GetProjectIdByAppName " + SyncDataManager._currentTask.ProjectName + " -> " + SyncDataManager._currentTask.ProjectId.ToString());
					}
					bool flag27 = SyncDataManager._currentTask.ProjectId < 0 || UwaWebsiteClient.userProfile.FindById(SyncDataManager._currentTask.ProjectId) == null;
					if (flag27)
					{
						SyncDataManager.Finish = SyncDataManager.FinishType.InvalidProjectId;
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
						return;
					}
					bool flag28 = SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.pipeline;
					if (flag28)
					{
						UwaWebsiteClient.ProjectBalance("LT", SyncDataManager._currentTask.ProjectId, new UwaWebsiteClient.OnDoneCallback(SyncDataManager.OnProjectBalance));
					}
					else
					{
						bool flag29 = SyncDataManager._currentTask.ProjectType != SyncDataManager.ProjectType.gpm_dev && SyncDataManager._currentTask.ProjectType != SyncDataManager.ProjectType.gpm_res;
						if (flag29)
						{
							UwaWebsiteClient.ProjectBalance("GOT_ONLINE", SyncDataManager._currentTask.ProjectId, new UwaWebsiteClient.OnDoneCallback(SyncDataManager.OnProjectBalance));
						}
						else
						{
							UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.GetBalance;
						}
					}
					SyncDataManager.Progress = 0.1f;
					bool flag30 = SyncDataManager.OnRepaintWindow != null;
					if (flag30)
					{
						SyncDataManager.OnRepaintWindow();
					}
					SyncDataManager.State = SyncDataManager.UploadState.Zipping;
				}
				bool flag31 = SyncDataManager.State == SyncDataManager.UploadState.Zipping;
				if (flag31)
				{
					bool needZip = SyncDataManager._currentTask.NeedZip;
					if (needZip)
					{
						SyncDataManager.Progress = SyncDataManager.ZipHelper.Progress * 0.4f + 0.1f;
						bool flag32 = SyncDataManager.OnRepaintWindow != null;
						if (flag32)
						{
							SyncDataManager.OnRepaintWindow();
						}
					}
					bool flag33 = !SyncDataManager._currentTask.NeedZip || (SyncDataManager.ZipHelper.isDone != null && SyncDataManager.ZipHelper.isDone.Value);
					if (flag33)
					{
						bool flag34 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.GetBalance;
						if (flag34)
						{
							UwaWebsiteClient.GroupProjectInfo groupProjectInfo2 = UwaWebsiteClient.userProfile.FindById(SyncDataManager._currentTask.ProjectId);
							groupProjectInfo2.UpdateDefaultBalance();
							bool flag35 = SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.pipeline && groupProjectInfo2 != null && groupProjectInfo2.SelectedBalance == null && groupProjectInfo2.engine == 1;
							if (flag35)
							{
								SyncDataManager.Finish = SyncDataManager.FinishType.BalanceNoEnough;
								SyncDataManager.State = SyncDataManager.UploadState.Finish;
							}
							else
							{
								SyncDataManager.State = SyncDataManager.UploadState.GettingSubmitInfo;
								SyncDataManager.Progress = 0.5f;
								List<string> list = new List<string>();
								bool flag36 = SyncDataManager._currentTask.ProjectType == SyncDataManager.ProjectType.assetbundle;
								if (flag36)
								{
									list.Add(SyncDataManager._currentTask.FolderPath + "/webdata/assetbundles.json");
									bool flag37 = File.Exists(SyncDataManager._currentTask.FolderPath + "/webdata/redund_chains.json");
									if (flag37)
									{
										list.Add(SyncDataManager._currentTask.FolderPath + "/webdata/redund_chains.json");
									}
								}
								else
								{
									list.Add(SyncDataManager._currentTask.FolderPath + ".zip");
								}
								UwaWebsiteClient.GotUploadInfo(new UwaWebsiteClient.OnDoneCallback(SyncDataManager.OnGotSubmitInfo), new UwaWebsiteClient.ServiceDataInfo
								{
									platform = SyncDataManager._currentTask.Platform,
									duration = SyncDataManager._currentTask.Duration,
									deviceModel = SyncDataManager._currentTask.DeviceModel,
									dataHash = SyncDataManager._currentTask.DataKey,
									dataType = (int)SyncDataManager._currentTask.ProjectType,
									engine = SyncDataManager._currentTask.Engine,
									engineVersion = SyncDataManager._currentTask.EngineVersion,
									selectedProjectId = SyncDataManager._currentTask.ProjectId,
									zipPath = list,
									userNote = SyncDataManager._currentTask.UserNote,
									license = SyncDataManager._currentTask.License,
									meta = SyncDataManager._currentTask.Meta
								});
							}
							Using_request.Instance.OSS(SyncDataManager._currentTask.Guid, SyncDataManager._currentTask.Platform);
						}
					}
					else
					{
						bool flag38 = SyncDataManager._currentTask.NeedZip && SyncDataManager.ZipHelper.isDone != null && !SyncDataManager.ZipHelper.isDone.Value;
						if (flag38)
						{
							SyncDataManager.Finish = SyncDataManager.FinishType.Unzip;
							SyncDataManager.State = SyncDataManager.UploadState.Finish;
						}
					}
				}
				bool flag39 = SyncDataManager.State == SyncDataManager.UploadState.GettingSubmitInfo;
				if (flag39)
				{
					bool flag40 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Uploading;
					if (flag40)
					{
						SyncDataManager.Progress = 0.5f + 0.4f * UwaWebsiteClient.gotDataSubmitInfo.lastPercent;
						bool flag41 = SyncDataManager.OnRepaintWindow != null;
						if (flag41)
						{
							SyncDataManager.OnRepaintWindow();
						}
					}
					bool flag42 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Uploaded;
					if (flag42)
					{
						Using_request.Instance.Upload(SyncDataManager._currentTask.Guid, UwaWebsiteClient.gotDataSubmitInfo.fileUrl);
						SyncDataManager.State = SyncDataManager.UploadState.UploadedData;
						File.Delete(SyncDataManager._currentTask.FolderPath + ".zip");
					}
					bool flag43 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
					if (flag43)
					{
						bool flag44 = SyncDataManager.Finish == SyncDataManager.FinishType.Unset;
						if (flag44)
						{
							SyncDataManager.Finish = SyncDataManager.FinishType.Unuploaded;
						}
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
						File.Delete(SyncDataManager._currentTask.FolderPath + ".zip");
					}
				}
				bool flag45 = SyncDataManager.State == SyncDataManager.UploadState.UploadedData;
				if (flag45)
				{
					SyncDataManager.State = SyncDataManager.UploadState.SubmittingData;
					UwaWebsiteClient.GotCheck(new UwaWebsiteClient.OnDoneCallback2(SyncDataManager.OnCheck));
				}
				bool flag46 = SyncDataManager.State == SyncDataManager.UploadState.SubmittingData;
				if (flag46)
				{
					bool flag47 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Submit;
					if (flag47)
					{
						SyncDataManager.State = SyncDataManager.UploadState.SubmitData;
					}
					bool flag48 = UwaWebsiteClient.gotDataSubmitInfo.state == UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
					if (flag48)
					{
						bool flag49 = SyncDataManager.Finish != SyncDataManager.FinishType.Resubmit && SyncDataManager.Finish != SyncDataManager.FinishType.BalanceNoEnough;
						if (flag49)
						{
							SyncDataManager.Finish = SyncDataManager.FinishType.Unsubmit;
						}
						SyncDataManager.State = SyncDataManager.UploadState.Finish;
					}
				}
				bool flag50 = SyncDataManager.State == SyncDataManager.UploadState.SubmitData;
				if (flag50)
				{
					SyncDataManager.Progress = 1f;
					bool flag51 = SyncDataManager.OnRepaintWindow != null;
					if (flag51)
					{
						SyncDataManager.OnRepaintWindow();
					}
					UwaWebsiteClient.gotDataSubmitInfo.lastProjectId = SyncDataManager._currentTask.ProjectId;
					SyncDataManager.State = SyncDataManager.UploadState.Finish;
					SyncDataManager.Finish = SyncDataManager.FinishType.Success;
				}
				bool flag52 = SyncDataManager.State == SyncDataManager.UploadState.Finish;
				if (flag52)
				{
					SyncDataManager.FinishTask();
				}
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0001ECF4 File Offset: 0x0001CEF4
		private static void OnProjectBalance(int errorCode, string errorMessage)
		{
			UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindById(SyncDataManager._currentTask.ProjectId);
			groupProjectInfo.UpdateDefaultBalance();
			bool flag = groupProjectInfo.SelectedBalance != null;
			if (flag)
			{
				SyncDataManager._currentTask.ProjectBalance = groupProjectInfo.SelectedBalance.BalanceFinal;
			}
			UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.GetBalance;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0001ED54 File Offset: 0x0001CF54
		private static void OnCheck(int recordId, int errorCode, string errorMessage)
		{
			bool flag = errorCode == 0 && recordId != 0;
			if (flag)
			{
				UwaWebsiteClient.gotDataSubmitInfo.lastRecordId = recordId;
				Using_request.Instance.Submit(SyncDataManager._currentTask.Guid);
			}
			else
			{
				bool flag2 = errorCode == 20224;
				if (flag2)
				{
					SyncDataManager.Finish = SyncDataManager.FinishType.BalanceNoEnough;
				}
				UwaWebsiteClient.gotDataSubmitInfo.lastRecordId = 0;
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0001EDC8 File Offset: 0x0001CFC8
		private static void FinishTask()
		{
			SyncDataManager.MarkSynced(SyncDataManager.Finish);
			bool flag = SyncDataManager.OnRepaintWindow != null;
			if (flag)
			{
				SyncDataManager.OnRepaintWindow();
			}
			SyncDataManager._currentTask = null;
			UwaWebsiteClient.LogPath = null;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0001EE0C File Offset: 0x0001D00C
		private static void OnGotProject(int errorCode, string msg)
		{
			UwaWebsiteClient.Log(string.Concat(new string[]
			{
				"OnGotProject(",
				errorCode.ToString(),
				" ,",
				msg,
				")"
			}));
			bool flag = msg == null && UwaWebsiteClient.userProfile != null;
			if (flag)
			{
				SyncDataManager.UpdateProjectName2Id(UwaWebsiteClient.userProfile.temp_got_project_array);
				bool flag2 = SyncDataManager.State == SyncDataManager.UploadState.GettingProjectInfo;
				if (flag2)
				{
					SyncDataManager.State = SyncDataManager.UploadState.GotgProjectInfo;
				}
			}
			else
			{
				SyncDataManager.UpdateProjectName2Id(null);
				bool flag3 = SyncDataManager.State == SyncDataManager.UploadState.GettingProjectInfo;
				if (flag3)
				{
					SyncDataManager.State = SyncDataManager.UploadState.Finish;
					SyncDataManager.Finish = SyncDataManager.FinishType.Ungotproject;
				}
			}
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0001EEC8 File Offset: 0x0001D0C8
		private static void MarkSynced(SyncDataManager.FinishType error)
		{
			File.WriteAllText(SyncDataManager._currentTask.FolderPath + "/synced", error.ToString());
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0001EEF4 File Offset: 0x0001D0F4
		public static bool CheckSynced(string folder, out SyncDataManager.FinishType error)
		{
			error = SyncDataManager.FinishType.Failed;
			bool flag = File.Exists(folder + "/synced");
			bool flag2 = flag;
			if (flag2)
			{
				string value = File.ReadAllText(folder + "/synced");
				bool flag3 = string.IsNullOrEmpty(value);
				if (flag3)
				{
					error = SyncDataManager.FinishType.Success;
				}
				else
				{
					try
					{
						error = (SyncDataManager.FinishType)Enum.Parse(typeof(SyncDataManager.FinishType), value);
					}
					catch (Exception ex)
					{
						error = SyncDataManager.FinishType.Failed;
					}
				}
				flag = false;
				bool flag4 = error == SyncDataManager.FinishType.Success || error == SyncDataManager.FinishType.Unhash || error == SyncDataManager.FinishType.DataBroken || error == SyncDataManager.FinishType.Resubmit;
				if (flag4)
				{
					flag = true;
				}
			}
			bool debug = UwaWebsiteClient.Debug;
			return !debug && flag;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001EFDC File Offset: 0x0001D1DC
		public static bool GetABInfo(string dataPath, out SyncDataManager.ABDataIfo pInfo)
		{
			pInfo = new SyncDataManager.ABDataIfo();
			pInfo.testTime = "";
			pInfo.platform = 3;
			pInfo.testMode = "assetbundle";
			try
			{
				JSONValue jsonvalue = JSONParser.SimpleParse(File.ReadAllText(dataPath + "/result.json"));
				pInfo.testMode = "AssetBundle";
				pInfo.platform = jsonvalue["platform"];
				pInfo.testTime = jsonvalue["start_time"];
				bool flag = jsonvalue.ContainsKey("license");
				if (flag)
				{
					pInfo.license = jsonvalue["license"].AsString(false).Replace("\r", "").Replace("\n", "").Replace(" ", "");
				}
				bool flag2 = jsonvalue.ContainsKey("unity_version");
				if (flag2)
				{
					pInfo.engine = 1;
					pInfo.engineVersion = jsonvalue["unity_version"];
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0001F124 File Offset: 0x0001D324
		private static bool GetProjNameTestTimeCompayName(string dataPath, out SyncDataManager.PPLDataInfo pInfo)
		{
			pInfo = new SyncDataManager.PPLDataInfo();
			pInfo.testMode = "pipeline";
			pInfo.companyName = "";
			pInfo.testTime = "";
			pInfo.productName = "";
			pInfo.platform = 3;
			pInfo.license = "";
			StreamReader streamReader = new StreamReader(dataPath + "/projectinfo");
			for (;;)
			{
				string text = streamReader.ReadLine();
				bool flag = text == null;
				if (flag)
				{
					break;
				}
				bool flag2 = text.Equals("\n") || text.Equals("");
				if (!flag2)
				{
					string[] array = text.Split(new char[]
					{
						':'
					});
					bool flag3 = array[0].Equals("CompanyName");
					if (flag3)
					{
						pInfo.companyName = array[1];
					}
					bool flag4 = array[0].Equals("ProductName");
					if (flag4)
					{
						pInfo.productName = array[1];
					}
					bool flag5 = array[0].Equals("BuildTarget");
					if (flag5)
					{
						bool flag6 = array[1].Equals("Android");
						if (flag6)
						{
							pInfo.platform = 1;
						}
						else
						{
							bool flag7 = array[1].Equals("PC") || array[1].Equals("StandaloneWindows64") || array[1].Equals("StandaloneWindows");
							if (flag7)
							{
								pInfo.platform = 3;
							}
							else
							{
								bool flag8 = array[1].Equals("iPhone") || array[1].Equals("iOS") || array[1].Equals("IOS");
								if (flag8)
								{
									pInfo.platform = 2;
								}
								else
								{
									pInfo.platform = 3;
								}
							}
						}
					}
					bool flag9 = array[0].Equals("StartTime");
					if (flag9)
					{
						pInfo.testTime = array[1];
					}
					bool flag10 = array[0].Equals("License");
					if (flag10)
					{
						pInfo.license = array[1].Replace("\r", "").Replace("\n", "").Replace(" ", "");
					}
					bool flag11 = array[0].Equals("UnityVersion");
					if (flag11)
					{
						pInfo.engine = 1;
					}
					bool flag12 = array[0].Equals("UnrealVersion");
					if (flag12)
					{
						pInfo.engine = 2;
					}
				}
			}
			streamReader.Close();
			return !pInfo.productName.Equals("") && !pInfo.testTime.Equals("");
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0001F474 File Offset: 0x0001D674
		public static string GetFileString(string dataPath, string file)
		{
			string path = dataPath + "/" + file;
			bool flag = File.Exists(path);
			if (flag)
			{
				try
				{
					return File.ReadAllText(path);
				}
				catch (Exception ex)
				{
				}
			}
			return null;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0001F4CC File Offset: 0x0001D6CC
		public static bool GetFrameAndDurationInDoneOrLast(string dataPath, out int frame, out int duration)
		{
			bool flag = true;
			frame = 0;
			duration = 0;
			float num = 0f;
			string path = dataPath + "/done";
			bool flag2 = !File.Exists(path);
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				try
				{
					StreamReader streamReader = new StreamReader(path);
					string text = streamReader.ReadLine();
					bool flag3 = string.IsNullOrEmpty(text) || !int.TryParse(text, out frame);
					if (flag3)
					{
						flag = false;
					}
					string text2 = streamReader.ReadLine();
					bool flag4 = string.IsNullOrEmpty(text2) || !float.TryParse(text2, out num);
					if (flag4)
					{
						flag = false;
					}
					streamReader.Close();
				}
				catch (Exception ex)
				{
					flag = false;
				}
				duration = (int)num;
				result = flag;
			}
			return result;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0001F5B8 File Offset: 0x0001D7B8
		public static bool GetAppNameTestTimePackageNameKey(string folder, out SyncDataManager.SystemInfo _info)
		{
			_info = new SyncDataManager.SystemInfo();
			string path = folder + "/systemInfo";
			bool flag = !File.Exists(path);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				StreamReader streamReader = new StreamReader(path);
				for (;;)
				{
					string text = streamReader.ReadLine();
					bool flag2 = text == null;
					if (flag2)
					{
						break;
					}
					bool flag3 = text.Equals("\n") || text.Equals("");
					if (!flag3)
					{
						string[] array = Regex.Split(text, ":", RegexOptions.IgnoreCase);
						bool flag4 = array[0].Equals("PackageName");
						if (flag4)
						{
							_info.pkgName = array[1];
						}
						bool flag5 = array[0].Equals("AppName");
						if (flag5)
						{
							_info.appName = array[1];
						}
						bool flag6 = array[0].Equals("DeviceModel");
						if (flag6)
						{
							_info.deviceModel = array[1];
						}
						bool flag7 = array[0].Equals("StartTime");
						if (flag7)
						{
							_info.testTime = array[1];
						}
						bool flag8 = array[0].Equals("TestMode");
						if (flag8)
						{
							_info.testMode = array[1];
						}
						bool flag9 = array[0].Equals("DeviceID");
						if (flag9)
						{
							_info.deviceId = array[1];
						}
						bool flag10 = array[0].Equals("Platform");
						if (flag10)
						{
							bool flag11 = array[1].Equals("Android");
							if (flag11)
							{
								_info.platform = 1;
							}
							else
							{
								bool flag12 = array[1].Equals("WindowsPlayer");
								if (flag12)
								{
									_info.platform = 3;
								}
								else
								{
									bool flag13 = array[1].Equals("IPhonePlayer");
									if (flag13)
									{
										_info.platform = 2;
									}
									else
									{
										_info.platform = 3;
									}
								}
							}
						}
						bool flag14 = array[0].Equals("UnityVersion");
						if (flag14)
						{
							_info.engine = 1;
							_info.engineVersion = array[1];
						}
						bool flag15 = array[0].Equals("UnrealVersion");
						if (flag15)
						{
							_info.engine = 2;
							_info.engineVersion = array[1];
						}
						bool flag16 = array[0].Equals("StatsDataPath");
						if (flag16)
						{
							_info.unrealStats = text.Substring(array[0].Length + 1);
						}
						bool flag17 = array[0].Equals("DevelopmentBuild");
						if (flag17)
						{
							try
							{
								_info.dev = bool.Parse(array[1]);
							}
							catch (Exception ex)
							{
								_info.dev = false;
							}
						}
					}
				}
				streamReader.Close();
				try
				{
					bool flag18 = !_info.pkgName.Equals("") && !_info.appName.Equals("") && !_info.testTime.Equals("") && !_info.testMode.Equals("") && !_info.deviceId.Equals("");
					if (flag18)
					{
						return true;
					}
				}
				catch (Exception ex2)
				{
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001F9B0 File Offset: 0x0001DBB0
		private static string GetOrCreateDataKey(string guid)
		{
			bool flag = guid == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string path = SyncDataManager._currentTask.FolderPath + "/hash";
				bool flag2 = !UwaWebsiteClient.Debug && File.Exists(path);
				if (flag2)
				{
					result = File.ReadAllText(path);
				}
				else
				{
					try
					{
						string text = guid + new Random().Next(0, 1000).ToString("D3");
						File.WriteAllText(path, text);
						return text;
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0001FA78 File Offset: 0x0001DC78
		public static string Cn2En(string cnStr)
		{
			string text = Uri.EscapeUriString(cnStr).Replace("%", "");
			bool flag = text.Length >= 30;
			if (flag)
			{
				text = text.Substring(0, 30);
			}
			return text;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0001FAC8 File Offset: 0x0001DCC8
		private static void OnGotSubmitInfo(int errorCode, string msg)
		{
			bool flag = errorCode == 1000;
			if (flag)
			{
				SyncDataManager.Finish = SyncDataManager.FinishType.Resubmit;
			}
			else
			{
				bool flag2 = errorCode == 20004 || errorCode == 20224;
				if (flag2)
				{
					SyncDataManager.Finish = SyncDataManager.FinishType.BalanceNoEnough;
				}
				else
				{
					bool flag3 = errorCode == 20306;
					if (flag3)
					{
						SyncDataManager.Finish = SyncDataManager.FinishType.UserNotActive;
					}
					else
					{
						bool flag4 = errorCode == 20303;
						if (flag4)
						{
							SyncDataManager.Finish = SyncDataManager.FinishType.UserNotLogin;
						}
						else
						{
							bool flag5 = errorCode != 0;
							if (flag5)
							{
								SyncDataManager.Finish = SyncDataManager.FinishType.Unsubmit;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0001FB78 File Offset: 0x0001DD78
		private static void UpdateProjectName2Id(Dictionary<int, UwaWebsiteClient.GroupProjectInfo> projectList)
		{
			SyncDataManager.UpdateProjectNameList(projectList);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0001FB84 File Offset: 0x0001DD84
		private static void UpdateProjectNameList(Dictionary<int, UwaWebsiteClient.GroupProjectInfo> projectId2Info)
		{
			Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
			Dictionary<int, List<int>> dictionary2 = new Dictionary<int, List<int>>();
			for (int i = 1; i < 3; i++)
			{
				dictionary.Add(i, new List<string>());
				dictionary[i].Add(Localization.Instance.Get("Select Project"));
				dictionary2.Add(i, new List<int>());
				dictionary2[i].Add(-1);
			}
			bool flag = projectId2Info != null && projectId2Info.Count > 0;
			if (flag)
			{
				foreach (KeyValuePair<int, UwaWebsiteClient.GroupProjectInfo> keyValuePair in projectId2Info)
				{
					dictionary[keyValuePair.Value.engine].Add(keyValuePair.Value.projectGroupId.ToString() + ":" + keyValuePair.Value.projectGroupName);
					dictionary2[keyValuePair.Value.engine].Add(keyValuePair.Value.projectGroupId);
				}
				for (int j = 1; j < 3; j++)
				{
					SyncDataManager.ProjectDisplayList[j].NameList = dictionary[j];
					SyncDataManager.ProjectDisplayList[j].IdList = dictionary2[j].ToArray();
				}
			}
			else
			{
				for (int k = 1; k < 3; k++)
				{
					SyncDataManager.ProjectDisplayList[k].NameList = dictionary[k];
					SyncDataManager.ProjectDisplayList[k].IdList = dictionary2[k].ToArray();
				}
			}
			bool flag2 = SyncDataManager.OnUpdateProjectList != null;
			if (flag2)
			{
				SyncDataManager.OnUpdateProjectList();
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0001FD88 File Offset: 0x0001DF88
		public static int GetProjectIdByDisplayIndex(int ind, int engine = 1)
		{
			bool flag = ind < 0 || engine == -1 || ind >= SyncDataManager.ProjectDisplayList[engine].IdList.Length;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = SyncDataManager.ProjectDisplayList[engine].IdList[ind];
			}
			return result;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0001FDF0 File Offset: 0x0001DFF0
		public static SyncDataManager.ProjectType GetProjectTypeByName(string mode, bool devbuild = true)
		{
			SyncDataManager.ProjectType result;
			try
			{
				bool flag = mode == "GPM";
				if (flag)
				{
					result = (devbuild ? SyncDataManager.ProjectType.gpm_dev : SyncDataManager.ProjectType.gpm_res);
				}
				else
				{
					result = (SyncDataManager.ProjectType)Enum.Parse(typeof(SyncDataManager.ProjectType), mode.ToLower());
				}
			}
			catch (Exception ex)
			{
				result = SyncDataManager.ProjectType.unknown;
			}
			return result;
		}

		// Token: 0x0400025C RID: 604
		public static bool ToStop = false;

		// Token: 0x0400025D RID: 605
		private static SyncDataManager.UploadState _state = SyncDataManager.UploadState.Idle;

		// Token: 0x0400025E RID: 606
		private static SyncDataManager.FinishType _finish = SyncDataManager.FinishType.Unset;

		// Token: 0x0400025F RID: 607
		public static Dictionary<int, SyncDataManager.DisplayList> ProjectDisplayList = new Dictionary<int, SyncDataManager.DisplayList>
		{
			{
				1,
				new SyncDataManager.DisplayList()
			},
			{
				2,
				new SyncDataManager.DisplayList()
			}
		};

		// Token: 0x04000260 RID: 608
		public static ZipFileHelper ZipHelper;

		// Token: 0x04000261 RID: 609
		public static float Progress = 0f;

		// Token: 0x04000262 RID: 610
		[NonSerialized]
		private static bool _inited = false;

		// Token: 0x04000263 RID: 611
		private static readonly Queue<SyncDataManager.TaskInfo> Tasks = new Queue<SyncDataManager.TaskInfo>();

		// Token: 0x04000264 RID: 612
		private static SyncDataManager.TaskInfo _currentTask = null;

		// Token: 0x04000265 RID: 613
		private static DateTime _lastUpdate = DateTime.MinValue;

		// Token: 0x04000266 RID: 614
		private static DateTime _lastProjectUpdate = DateTime.MinValue;

		// Token: 0x04000267 RID: 615
		private static string _unrealStats;

		// Token: 0x04000268 RID: 616
		private static int[] Platforms = new int[]
		{
			1,
			2,
			3
		};

		// Token: 0x04000269 RID: 617
		public static SyncDataManager.OnUpdate OnUpdateProjectList;

		// Token: 0x0400026A RID: 618
		public static SyncDataManager.OnRepaint OnRepaintWindow;

		// Token: 0x0200010D RID: 269
		public enum UploadState
		{
			// Token: 0x0400069D RID: 1693
			Idle,
			// Token: 0x0400069E RID: 1694
			GettingProjectInfo,
			// Token: 0x0400069F RID: 1695
			GotgProjectInfo,
			// Token: 0x040006A0 RID: 1696
			CreatingProjectInfo,
			// Token: 0x040006A1 RID: 1697
			CreateProjectInfo,
			// Token: 0x040006A2 RID: 1698
			Zipping,
			// Token: 0x040006A3 RID: 1699
			GettingSubmitInfo,
			// Token: 0x040006A4 RID: 1700
			UploadedData,
			// Token: 0x040006A5 RID: 1701
			SubmittingData,
			// Token: 0x040006A6 RID: 1702
			SubmitData,
			// Token: 0x040006A7 RID: 1703
			Finish
		}

		// Token: 0x0200010E RID: 270
		public enum FinishType
		{
			// Token: 0x040006A9 RID: 1705
			Unset,
			// Token: 0x040006AA RID: 1706
			Ungotproject,
			// Token: 0x040006AB RID: 1707
			Unzip,
			// Token: 0x040006AC RID: 1708
			Unhash,
			// Token: 0x040006AD RID: 1709
			Unuploaded,
			// Token: 0x040006AE RID: 1710
			StatNotFound,
			// Token: 0x040006AF RID: 1711
			Unsubmit,
			// Token: 0x040006B0 RID: 1712
			Resubmit,
			// Token: 0x040006B1 RID: 1713
			BalanceNoEnough,
			// Token: 0x040006B2 RID: 1714
			DataBroken,
			// Token: 0x040006B3 RID: 1715
			Uncreate,
			// Token: 0x040006B4 RID: 1716
			Success,
			// Token: 0x040006B5 RID: 1717
			Failed,
			// Token: 0x040006B6 RID: 1718
			InvalidProjectId,
			// Token: 0x040006B7 RID: 1719
			UserNotActive,
			// Token: 0x040006B8 RID: 1720
			UserNotLogin
		}

		// Token: 0x0200010F RID: 271
		public class DisplayList
		{
			// Token: 0x040006B9 RID: 1721
			public List<string> NameList = new List<string>();

			// Token: 0x040006BA RID: 1722
			public int[] IdList = new int[0];
		}

		// Token: 0x02000110 RID: 272
		public enum ProjectType
		{
			// Token: 0x040006BC RID: 1724
			mono = 3,
			// Token: 0x040006BD RID: 1725
			overview,
			// Token: 0x040006BE RID: 1726
			assets,
			// Token: 0x040006BF RID: 1727
			resources = 5,
			// Token: 0x040006C0 RID: 1728
			lua,
			// Token: 0x040006C1 RID: 1729
			pipeline,
			// Token: 0x040006C2 RID: 1730
			gpm_dev = 9,
			// Token: 0x040006C3 RID: 1731
			gpm_res = 8,
			// Token: 0x040006C4 RID: 1732
			gpu = 10,
			// Token: 0x040006C5 RID: 1733
			assetbundle = 50,
			// Token: 0x040006C6 RID: 1734
			unknown = 100
		}

		// Token: 0x02000111 RID: 273
		public class TaskInfo
		{
			// Token: 0x040006C7 RID: 1735
			public string FolderPath;

			// Token: 0x040006C8 RID: 1736
			public int ProjectId;

			// Token: 0x040006C9 RID: 1737
			public int ProjectBalance;

			// Token: 0x040006CA RID: 1738
			public string ProjectName;

			// Token: 0x040006CB RID: 1739
			public int Platform;

			// Token: 0x040006CC RID: 1740
			public int Duration = 0;

			// Token: 0x040006CD RID: 1741
			public string Guid;

			// Token: 0x040006CE RID: 1742
			public string DataKey;

			// Token: 0x040006CF RID: 1743
			public SyncDataManager.ProjectType ProjectType;

			// Token: 0x040006D0 RID: 1744
			public string DeviceModel = "";

			// Token: 0x040006D1 RID: 1745
			public string UserNote = "";

			// Token: 0x040006D2 RID: 1746
			public int Engine;

			// Token: 0x040006D3 RID: 1747
			public string EngineVersion = "";

			// Token: 0x040006D4 RID: 1748
			public string License = null;

			// Token: 0x040006D5 RID: 1749
			public string Meta = "";

			// Token: 0x040006D6 RID: 1750
			public bool NeedZip;
		}

		// Token: 0x02000112 RID: 274
		public class ABDataIfo
		{
			// Token: 0x06000A0D RID: 2573 RVA: 0x00048588 File Offset: 0x00046788
			public string GetGUID()
			{
				return Guid.NewGuid().ToString("N").Substring(0, 22);
			}

			// Token: 0x040006D7 RID: 1751
			public string testTime;

			// Token: 0x040006D8 RID: 1752
			public int platform;

			// Token: 0x040006D9 RID: 1753
			public string testMode;

			// Token: 0x040006DA RID: 1754
			public string engineVersion;

			// Token: 0x040006DB RID: 1755
			public string license;

			// Token: 0x040006DC RID: 1756
			public int engine;
		}

		// Token: 0x02000113 RID: 275
		public class PPLDataInfo
		{
			// Token: 0x06000A0F RID: 2575 RVA: 0x000485C8 File Offset: 0x000467C8
			public string GetGUID()
			{
				string text = SyncDataManager.Cn2En(this.companyName);
				while (text.Length < 3)
				{
					text += "x";
				}
				string text2 = SyncDataManager.Cn2En(this.productName);
				while (text2.Length < 4)
				{
					text2 += "x";
				}
				string text3 = this.testTime + text.Substring(text.Length - 3) + text2.Substring(text2.Length - 4) + "7";
				return text3.Replace(" ", "_").Replace("(", "0").Replace(")", "0");
			}

			// Token: 0x040006DD RID: 1757
			public string companyName;

			// Token: 0x040006DE RID: 1758
			public string productName;

			// Token: 0x040006DF RID: 1759
			public string testTime;

			// Token: 0x040006E0 RID: 1760
			public string testMode;

			// Token: 0x040006E1 RID: 1761
			public int platform;

			// Token: 0x040006E2 RID: 1762
			public int engine;

			// Token: 0x040006E3 RID: 1763
			public string license;
		}

		// Token: 0x02000114 RID: 276
		public class SystemInfo
		{
			// Token: 0x06000A11 RID: 2577 RVA: 0x000486A4 File Offset: 0x000468A4
			public string GetGUID()
			{
				string text = SyncDataManager.Cn2En(this.pkgName);
				while (text.Length < 3)
				{
					text += "x";
				}
				string text2 = this.testTime + text.Substring(text.Length - 3) + this.deviceId.Substring(this.deviceId.Length - 4) + ((int)SyncDataManager.GetProjectTypeByName(this.testMode, true)).ToString();
				return text2.Replace(" ", "_");
			}

			// Token: 0x040006E4 RID: 1764
			public string pkgName;

			// Token: 0x040006E5 RID: 1765
			public string appName = "";

			// Token: 0x040006E6 RID: 1766
			public string testTime = "";

			// Token: 0x040006E7 RID: 1767
			public string testMode = "";

			// Token: 0x040006E8 RID: 1768
			public string deviceModel = "";

			// Token: 0x040006E9 RID: 1769
			public string deviceId = "";

			// Token: 0x040006EA RID: 1770
			public int engine = 1;

			// Token: 0x040006EB RID: 1771
			public string engineVersion = "";

			// Token: 0x040006EC RID: 1772
			public int platform = 1;

			// Token: 0x040006ED RID: 1773
			public bool dev = true;

			// Token: 0x040006EE RID: 1774
			public string unrealStats = "";
		}

		// Token: 0x02000115 RID: 277
		// (Invoke) Token: 0x06000A14 RID: 2580
		public delegate void OnUpdate();

		// Token: 0x02000116 RID: 278
		// (Invoke) Token: 0x06000A18 RID: 2584
		public delegate void OnRepaint();
	}
}
