using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UWALib.Editor.NetWork;
using UWALocal;

// Token: 0x0200000E RID: 14
internal class NativeUploadTool : global::UploadTool
{
	// Token: 0x0600006E RID: 110
	[DllImport("uwa")]
	private static extern void SetTest(bool test);

	// Token: 0x0600006F RID: 111
	[DllImport("uwa")]
	private static extern int GotProjectBalance(int projectId);

	// Token: 0x06000070 RID: 112
	[DllImport("uwa")]
	private static extern bool _Login(string username, string password, string auth, bool rememberMe);

	// Token: 0x06000071 RID: 113
	[DllImport("uwa")]
	private static extern void GetIdAndToken(string userid, string token);

	// Token: 0x06000072 RID: 114
	[DllImport("uwa")]
	private static extern IntPtr GetLoginErrorMessage();

	// Token: 0x06000073 RID: 115
	[DllImport("uwa")]
	private static extern IntPtr GetGotRecordId();

	// Token: 0x06000074 RID: 116
	[DllImport("uwa")]
	private static extern int GetLoginErrorCode();

	// Token: 0x06000075 RID: 117
	[DllImport("uwa")]
	private static extern bool GetCreateProjectCheck();

	// Token: 0x06000076 RID: 118
	[DllImport("uwa")]
	private static extern int CreateProject(int gameSubType, int gameType, string projectName, string pkgName);

	// Token: 0x06000077 RID: 119
	[DllImport("uwa")]
	private static extern IntPtr GotProjectAll(int engine);

	// Token: 0x06000078 RID: 120
	[DllImport("uwa")]
	private static extern void CheckDataInfo(string path);

	// Token: 0x06000079 RID: 121
	[DllImport("uwa")]
	private static extern bool GotCompareBalances();

	// Token: 0x0600007A RID: 122
	[DllImport("uwa")]
	private static extern bool GotUploadInfo();

	// Token: 0x0600007B RID: 123
	[DllImport("uwa")]
	private static extern void GotRemarkFinish();

	// Token: 0x0600007C RID: 124
	[DllImport("uwa")]
	private static extern float GetProgress();

	// Token: 0x0600007D RID: 125
	[DllImport("uwa")]
	private static extern void PackageAndUpload_join();

	// Token: 0x0600007E RID: 126
	[DllImport("uwa")]
	private static extern bool GotCheck();

	// Token: 0x0600007F RID: 127
	[DllImport("uwa")]
	private static extern void PackageAndUpload_thread();

	// Token: 0x06000080 RID: 128
	[DllImport("uwa")]
	private static extern void One_click_submission_name_join();

	// Token: 0x06000081 RID: 129 RVA: 0x00004188 File Offset: 0x00002388
	public override void LogSetup()
	{
	}

	// Token: 0x06000082 RID: 130 RVA: 0x0000418C File Offset: 0x0000238C
	public override void UpdateFrame()
	{
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004190 File Offset: 0x00002390
	public override void LoginWithCredentials(string user, string pwd, string authCode, bool remeberMe, Action<bool, int, string> callback)
	{
		bool flag = NativeUploadTool._Login(user, pwd, authCode, remeberMe);
		int loginErrorCode = NativeUploadTool.GetLoginErrorCode();
		IntPtr loginErrorMessage = NativeUploadTool.GetLoginErrorMessage();
		string text = Marshal.PtrToStringAnsi(loginErrorMessage);
		if (callback != null)
		{
			callback(flag, loginErrorCode, flag ? "" : text);
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000041E8 File Offset: 0x000023E8
	public override void LoginWithUserIdToken(string userid, string token, Action<bool, int, string> callback)
	{
		NativeUploadTool.GetIdAndToken(userid, token);
		if (callback != null)
		{
			callback(true, 0, "");
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x0000420C File Offset: 0x0000240C
	public override void UpdateGotProject(int engine, Action<List<string>> callback)
	{
		IntPtr ptr = NativeUploadTool.GotProjectAll(engine);
		string text = Marshal.PtrToStringAnsi(ptr);
		List<string> list = new List<string>();
		list.Add("选择项目");
		string text2 = "";
		for (int i = 0; i < text.Length; i++)
		{
			bool flag = text[i] == '$';
			if (flag)
			{
				list.Add(text2);
				this.ParseIdAndNameFields(text2);
				text2 = "";
			}
			else
			{
				text2 += text[i].ToString();
			}
		}
		if (callback != null)
		{
			callback(list);
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000042C0 File Offset: 0x000024C0
	public void ParseIdAndNameFields(string projectName)
	{
		bool flag = true;
		string text = "";
		string text2 = "";
		foreach (char c in projectName)
		{
			bool flag2 = c == ':' && flag;
			if (flag2)
			{
				flag = false;
			}
			else
			{
				bool flag3 = flag;
				if (flag3)
				{
					text += c.ToString();
				}
				else
				{
					text2 += c.ToString();
				}
			}
		}
		this.DataInstance.Proid.Add(int.Parse(text));
		this.DataInstance.Proname.Add(text2);
		this.DataInstance.idx++;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00004388 File Offset: 0x00002588
	public override int GetProjectIdByDisplayIndex(int projectInd)
	{
		string text = GotOnlineState.ProjectNameList[projectInd];
		string text2 = "";
		foreach (char c in text)
		{
			bool flag = c == ':';
			if (flag)
			{
				break;
			}
			text2 += c.ToString();
		}
		int num;
		bool flag2 = int.TryParse(text2, out num);
		int result;
		if (flag2)
		{
			result = num;
		}
		else
		{
			result = -1;
		}
		return result;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00004418 File Offset: 0x00002618
	public override int GetProjectIdByName(string projectName)
	{
		int num = -1;
		bool flag = this.DataInstance.Proname.Contains(projectName);
		if (flag)
		{
			num = this.DataInstance.Proname.IndexOf(projectName);
		}
		return (num < this.DataInstance.Proid.Count && num >= 0) ? this.DataInstance.Proid[num] : -1;
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00004494 File Offset: 0x00002694
	public override bool HasProjectId(int projectId)
	{
		return this.DataInstance.Proid.Contains(projectId);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000044C0 File Offset: 0x000026C0
	public override void GotOnlineCheckDataInfo(string dataPath)
	{
		NativeUploadTool.CheckDataInfo(dataPath);
	}

	// Token: 0x0600008B RID: 139 RVA: 0x000044CC File Offset: 0x000026CC
	public override bool GotOnlineCompareBalances(int dataType)
	{
		bool flag = !base.NeedBalance(dataType);
		return flag || NativeUploadTool.GotCompareBalances();
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004504 File Offset: 0x00002704
	public override string GetLastRecordId()
	{
		IntPtr gotRecordId = NativeUploadTool.GetGotRecordId();
		return Marshal.PtrToStringAnsi(gotRecordId);
	}

	// Token: 0x0600008D RID: 141 RVA: 0x0000452C File Offset: 0x0000272C
	public override void ProjectBalance(string serviceMetaType, int projectId, Action<int, int, string> callback)
	{
		int arg = NativeUploadTool.GotProjectBalance(projectId);
		if (callback != null)
		{
			callback(arg, 0, "");
		}
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00004560 File Offset: 0x00002760
	public override void UploadSetup()
	{
		bool debug = UwaWebsiteClient.Debug;
		if (debug)
		{
			NativeUploadTool.SetTest(true);
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00004588 File Offset: 0x00002788
	public override IEnumerator DoUpload(DataUploader.UploadingInfo uploading, DataUploader.ItemAction item, float leftWeight)
	{
		NativeUploadTool.PackageAndUpload_thread();
		NativeUploadTool.CheckDataInfo(item.dataPath);
		bool flag = !NativeUploadTool.GotUploadInfo();
		if (flag)
		{
			uploading.GotOnlineFinish = SyncDataManager.FinishType.Unuploaded;
			yield break;
		}
		NativeUploadTool.GotRemarkFinish();
		for (float progress = NativeUploadTool.GetProgress(); progress < 99f; progress = NativeUploadTool.GetProgress())
		{
			uploading.CurrentPercent = 1f - leftWeight + leftWeight * Math.Max(0f, progress / 100f);
			yield return null;
		}
		NativeUploadTool.PackageAndUpload_join();
		uploading.GotOnlineFinish = SyncDataManager.FinishType.Unsubmit;
		bool flag2 = NativeUploadTool.GotCheck();
		if (flag2)
		{
			uploading.CurrentPercent = 1f;
			uploading.GotOnlineFinish = SyncDataManager.FinishType.Success;
		}
		yield break;
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000045AC File Offset: 0x000027AC
	public override void CreateProjectCheck(Action<bool> callback)
	{
		bool createProjectCheck = NativeUploadTool.GetCreateProjectCheck();
		if (callback != null)
		{
			callback(createProjectCheck);
		}
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000045D8 File Offset: 0x000027D8
	public override void CreateProject(int gameSubType, int gameType, string projectName, string pkgName, Action<int> callback)
	{
		int obj = NativeUploadTool.CreateProject(gameSubType, gameType, projectName, pkgName);
		if (callback != null)
		{
			callback(obj);
		}
	}

	// Token: 0x04000052 RID: 82
	private ProNameData DataInstance = new ProNameData();
}
