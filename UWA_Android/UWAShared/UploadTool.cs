using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWALocal;

// Token: 0x0200000F RID: 15
public abstract class UploadTool
{
	// Token: 0x06000093 RID: 147 RVA: 0x00004620 File Offset: 0x00002820
	public static UploadTool Get()
	{
		bool flag = UploadTool._instance == null;
		if (flag)
		{
			bool flag2 = Application.platform == 11;
			if (flag2)
			{
				UploadTool._instance = new NativeUploadTool();
			}
			else
			{
				UploadTool._instance = new DonetUploadTool();
			}
		}
		return UploadTool._instance;
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000094 RID: 148 RVA: 0x00004678 File Offset: 0x00002878
	// (set) Token: 0x06000095 RID: 149 RVA: 0x00004680 File Offset: 0x00002880
	public bool UploadScreen { get; set; }

	// Token: 0x06000096 RID: 150 RVA: 0x0000468C File Offset: 0x0000288C
	public bool NeedBalance(int dataType)
	{
		bool flag = dataType == 9 || dataType == 8;
		return !flag;
	}

	// Token: 0x06000097 RID: 151
	public abstract void LogSetup();

	// Token: 0x06000098 RID: 152
	public abstract void UploadSetup();

	// Token: 0x06000099 RID: 153
	public abstract void UpdateFrame();

	// Token: 0x0600009A RID: 154
	public abstract void LoginWithCredentials(string user, string pwd, string authCode, bool remeberMe, Action<bool, int, string> callback);

	// Token: 0x0600009B RID: 155
	public abstract void LoginWithUserIdToken(string user, string token, Action<bool, int, string> callback);

	// Token: 0x0600009C RID: 156
	public abstract void UpdateGotProject(int engine, Action<List<string>> callback);

	// Token: 0x0600009D RID: 157
	public abstract int GetProjectIdByDisplayIndex(int projectId);

	// Token: 0x0600009E RID: 158
	public abstract int GetProjectIdByName(string projectName);

	// Token: 0x0600009F RID: 159
	public abstract void CreateProjectCheck(Action<bool> callback);

	// Token: 0x060000A0 RID: 160
	public abstract void CreateProject(int gameSubType, int gameType, string projectName, string pkgName, Action<int> callback);

	// Token: 0x060000A1 RID: 161
	public abstract string GetLastRecordId();

	// Token: 0x060000A2 RID: 162
	public abstract bool HasProjectId(int projectId);

	// Token: 0x060000A3 RID: 163
	public abstract void GotOnlineCheckDataInfo(string dataPath);

	// Token: 0x060000A4 RID: 164
	public abstract bool GotOnlineCompareBalances(int dataType);

	// Token: 0x060000A5 RID: 165
	public abstract void ProjectBalance(string serviceType, int projectId, Action<int, int, string> callback);

	// Token: 0x060000A6 RID: 166
	public abstract IEnumerator DoUpload(DataUploader.UploadingInfo uploading, DataUploader.ItemAction item, float leftWeight);

	// Token: 0x04000053 RID: 83
	private static UploadTool _instance;
}
