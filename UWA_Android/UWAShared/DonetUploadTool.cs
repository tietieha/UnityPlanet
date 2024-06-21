using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using UnityEngine;
using UWA;
using UWA.Core;
using UWALib.Editor.NetWork;
using UWALocal;

// Token: 0x0200000C RID: 12
internal class DonetUploadTool : global::UploadTool
{
	// Token: 0x0600005C RID: 92 RVA: 0x00003E38 File Offset: 0x00002038
	public override void UploadSetup()
	{
		ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(UwaWebsiteClient.MyRemoteCertificateValidationCallback);
		Localization.Instance.SetWebSite(Localization.eWebSite.CN);
		UwaWebsiteClient.WebSite = Localization.eWebSite.CN;
		SyncDataManager.Init();
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003E68 File Offset: 0x00002068
	public override void LogSetup()
	{
		bool showLog = SharedUtils.ShowLog;
		if (showLog)
		{
			SharedUtils.Log("UwaWebsiteClient.Debug = " + UwaWebsiteClient.Debug.ToString());
		}
		bool flag = UwaWebsiteClient.Debug || File.Exists(Application.persistentDataPath + "/uwa_log");
		if (flag)
		{
			UwaWebsiteClient.LogPath = Application.persistentDataPath + "/uwa_weblog.txt";
			bool showLog2 = SharedUtils.ShowLog;
			if (showLog2)
			{
				SharedUtils.Log("UwaWebsiteClient.LogPath = " + UwaWebsiteClient.LogPath);
			}
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003F04 File Offset: 0x00002104
	public override void UpdateFrame()
	{
		UwaWebsiteClient.Update();
		SyncDataManager.Update();
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003F14 File Offset: 0x00002114
	public override void LoginWithCredentials(string user, string pwd, string authCode, bool remeberMe, Action<bool, int, string> callback)
	{
		UwaWebsiteClient.LoginWithCredentials(user, pwd, authCode, remeberMe, delegate(int x, string y)
		{
			bool flag = UwaWebsiteClient.LoginAuthImage != null;
			if (flag)
			{
				Object.DestroyImmediate(UwaWebsiteClient.LoginAuthImage);
				UwaWebsiteClient.LoginAuthImage = null;
			}
			UwaWebsiteClient.AuthCode = null;
			Action<bool, int, string> callback2 = callback;
			if (callback2 != null)
			{
				callback2(UwaWebsiteClient.IsLoggedIn, x, y);
			}
			bool needAuthCode = UwaWebsiteClient.NeedAuthCode;
			if (needAuthCode)
			{
				UwaWebsiteClient.MainAuthcodeImage(false, null);
			}
		});
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003F4C File Offset: 0x0000214C
	public override void LoginWithUserIdToken(string user, string token, Action<bool, int, string> callback)
	{
		UwaWebsiteClient.LoginWithUserIdToken(user, token, delegate(int x, string y)
		{
			Action<bool, int, string> callback2 = callback;
			if (callback2 != null)
			{
				callback2(UwaWebsiteClient.IsLoggedIn, x, y);
			}
		});
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00003F80 File Offset: 0x00002180
	public override int GetProjectIdByDisplayIndex(int projectId)
	{
		return SyncDataManager.GetProjectIdByDisplayIndex(projectId, 1);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003FA0 File Offset: 0x000021A0
	public override int GetProjectIdByName(string projectName)
	{
		UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindByName(projectName);
		return (groupProjectInfo != null) ? groupProjectInfo.projectGroupId : -1;
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003FD8 File Offset: 0x000021D8
	public override bool HasProjectId(int projectId)
	{
		UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindById(projectId);
		return groupProjectInfo != null;
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00004004 File Offset: 0x00002204
	public override void GotOnlineCheckDataInfo(string dataPath)
	{
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00004008 File Offset: 0x00002208
	public override bool GotOnlineCompareBalances(int dataType)
	{
		return true;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00004024 File Offset: 0x00002224
	public override string GetLastRecordId()
	{
		return UwaWebsiteClient.gotDataSubmitInfo.lastRecordId.ToString();
	}

	// Token: 0x06000067 RID: 103 RVA: 0x0000404C File Offset: 0x0000224C
	public override void ProjectBalance(string serviceMetaType, int projectId, Action<int, int, string> callback)
	{
		UwaWebsiteClient.ProjectBalance(serviceMetaType, projectId, delegate(int errorCode, string errorMsg)
		{
			UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindById(projectId);
			int arg = (groupProjectInfo.SelectedBalance == null) ? 0 : groupProjectInfo.SelectedBalance.BalanceFinal;
			Action<int, int, string> callback2 = callback;
			if (callback2 != null)
			{
				callback2(arg, errorCode, errorMsg);
			}
		});
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000408C File Offset: 0x0000228C
	public override void UpdateGotProject(int engine, Action<List<string>> callback)
	{
		SyncDataManager.OnUpdateProjectList = delegate()
		{
			Action<List<string>> callback2 = callback;
			if (callback2 != null)
			{
				callback2(SyncDataManager.ProjectDisplayList[1].NameList);
			}
		};
		SyncDataManager.UpdateGotProject();
	}

	// Token: 0x06000069 RID: 105 RVA: 0x000040C4 File Offset: 0x000022C4
	public override IEnumerator DoUpload(DataUploader.UploadingInfo uploading, DataUploader.ItemAction item, float leftWeight)
	{
		bool flag = item.ProjectId != -1;
		if (flag)
		{
			SyncDataManager.AddTask(item.dataPath, item.ProjectId, false);
		}
		while (SyncDataManager.TaskCount > 0 || SyncDataManager.State != SyncDataManager.UploadState.Finish)
		{
			uploading.CurrentPercent = 1f - leftWeight + leftWeight * Math.Max(0f, UwaWebsiteClient.gotDataSubmitInfo.lastPercent);
			yield return null;
		}
		uploading.GotOnlineFinish = SyncDataManager.Finish;
		yield break;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x000040E8 File Offset: 0x000022E8
	public override void CreateProjectCheck(Action<bool> callback)
	{
		UwaWebsiteClient.GetCreateProjectCheck(delegate(bool success, int errorCode, string errorMsg)
		{
			Action<bool> callback2 = callback;
			if (callback2 != null)
			{
				callback2(success);
			}
		});
	}

	// Token: 0x0600006B RID: 107 RVA: 0x0000411C File Offset: 0x0000231C
	public override void CreateProject(int gameSubType, int gameType, string projectName, string pkgName, Action<int> callback)
	{
		UwaWebsiteClient.CreateProject(gameSubType, gameType, projectName, pkgName, delegate(int pId, int errorCode, string errorMsg)
		{
			Action<int> callback2 = callback;
			if (callback2 != null)
			{
				callback2(pId);
			}
		});
	}
}
