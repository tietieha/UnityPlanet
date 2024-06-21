using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using UWA;
using UWACore.TrackManagers;
using UWACore.Util;
using UWACore.Util.NetWork;

// Token: 0x02000033 RID: 51
[Preserve]
[ComVisible(false)]
public class UWAStarter : MonoBehaviour
{
	// Token: 0x06000236 RID: 566 RVA: 0x0000E908 File Offset: 0x0000CB08
	public static UWAStarter Get()
	{
		return UWAStarter._instance;
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000E928 File Offset: 0x0000CB28
	private static Type FineTypeGolbal(string typeName)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type type = assemblies[i].GetType(typeName);
			bool flag = type != null;
			if (flag)
			{
				return type;
			}
		}
		return null;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000E98C File Offset: 0x0000CB8C
	private void DoSomeThingStart()
	{
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000E990 File Offset: 0x0000CB90
	private void DoSomeThingOnGUI()
	{
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000E994 File Offset: 0x0000CB94
	private void Awake()
	{
		bool flag = base.transform.parent != null;
		if (flag)
		{
			base.transform.parent = null;
		}
		base.gameObject.name = "UWA_SERVICE";
		bool flag2 = UWAStarter._instance != null || !UWACoreConfig.ToAddStarter;
		if (flag2)
		{
			Object.Destroy(this);
		}
		else
		{
			UWAStarter._instance = this;
			bool flag3 = Application.platform != null && Application.platform != 7;
			if (flag3)
			{
				AndroidHardwareTrackManager.StaticInit();
			}
			UWACoreConfig.KEY = UWACoreConfig.PKG_NAME;
			CoreUtils.CurrentLevelName = Application.loadedLevelName.Replace("/", "-");
			CoreUtils.LocalFileIndex = 0;
			Object.DontDestroyOnLoad(base.gameObject);
			Screen.sleepTimeout = -1;
			UWACoreConfig.Remote_IP = CoreUtils.GetSelfIp().ToString();
			this._timeTrackManager = new TimeTrackManager(".tt");
			this._logTrackManager = new LogTrackManager(".lt");
			this._assetTrackManager = new AssetTrackManager(".at");
			this._apiTrackManager = new ApiTrackManager(".pt");
			this._dataTrackManager = new DataTrackManager();
			this._hardwareTrackManager = new AndroidHardwareTrackManager(".ht");
			ASCTest.StaticInit();
			bool enabled = ASCTest.Enabled;
			if (enabled)
			{
				base.gameObject.AddComponent<ASCTest>();
			}
			string path = Application.streamingAssetsPath + "/uwaTag";
			base.StartCoroutine(this.GetGUID(path));
			BaseTrackerManager.PrepareAll();
			SharedUtils.SetupFinalPath();
			UWAState.StaticInit();
			UProfiler.StaticInit();
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000EB40 File Offset: 0x0000CD40
	private IEnumerator DelaySwitch()
	{
		yield return null;
		yield return null;
		UWAStarter.Get().StartCoroutine(UWAStarter.Get().SwitchRecording());
		yield break;
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000EB50 File Offset: 0x0000CD50
	private IEnumerator GetGUID(string path)
	{
		WWW www = new WWW(path);
		yield return www;
		bool flag = string.IsNullOrEmpty(www.error);
		if (flag)
		{
			UWACoreConfig.GUID = www.text;
		}
		else
		{
			SharedUtils.Log(www.error);
		}
		www = null;
		yield break;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000EB68 File Offset: 0x0000CD68
	private void Start()
	{
		this.DoSomeThingStart();
		bool flag = UWAState.GpuInNormal && CoreUtils.IsURP();
		if (flag)
		{
			UWAState.GpuInNormal = false;
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("GpuInNormal = false for URP");
			}
		}
		UWAState.CheckTestError();
		SharedUtils.Log("SDK Start");
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000EBD0 File Offset: 0x0000CDD0
	private void ScreenShotByCallBack(int frame)
	{
		bool flag = this._screenTrackManager != null;
		if (flag)
		{
			this._screenTrackManager.Shot(CoreUtils.GetLogFileFullPath(frame.ToString() + "_screenshot.jpg"));
		}
		bool showLog = SharedUtils.ShowLog;
		if (showLog)
		{
			SharedUtils.Log("ScreenShotByCallBack");
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000EC2C File Offset: 0x0000CE2C
	private bool CheckLastTime()
	{
		string path = SharedUtils.FinalDataPath + "/lasttime";
		bool flag = !File.Exists(path);
		bool result;
		if (flag)
		{
			SharedUtils.Log("Time tracker is closed for lasttime not found!");
			result = false;
		}
		else
		{
			bool flag2 = true;
			try
			{
				StreamReader streamReader = File.OpenText(path);
				string[] array = streamReader.ReadLine().Split(new char[]
				{
					'/'
				});
				int num = 0;
				long num2 = 0L;
				bool flag3 = long.TryParse(array[0], out num2);
				bool flag4 = int.TryParse(array[1], out num);
				bool flag5 = flag3 && flag4;
				if (flag5)
				{
					this._timeTrackManager.ResetStartTime(num2);
					UWAState.TestDuration = (double)(num2 / (long)num) / 60.0;
				}
				else
				{
					flag2 = false;
					SharedUtils.Log("Time tracker is closed for tryParse failed!");
				}
				streamReader.Close();
			}
			catch (Exception ex)
			{
				string str = "Get last time failed : ";
				Exception ex2 = ex;
				SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				flag2 = false;
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000ED58 File Offset: 0x0000CF58
	private void ClearUnusedData()
	{
		string[] files = Directory.GetFiles(SharedUtils.FinalDataPath);
		bool flag = files.Length != 0;
		if (flag)
		{
			foreach (string path in files)
			{
				string text = Path.GetFileName(path).Split(new char[]
				{
					'_',
					'.'
				})[0];
				int num = 0;
				bool flag2 = text != "" && int.TryParse(text, out num);
				if (flag2)
				{
					bool flag3 = num >= SharedUtils.frameId;
					if (flag3)
					{
						File.Delete(path);
					}
				}
			}
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000EE18 File Offset: 0x0000D018
	private bool PrepareData()
	{
		bool result = false;
		bool newTest = UWAState.NewTest;
		if (newTest)
		{
			bool flag = Directory.Exists(SharedUtils.FinalDataPath);
			if (flag)
			{
				Directory.Delete(SharedUtils.FinalDataPath, true);
			}
			Directory.CreateDirectory(SharedUtils.FinalDataPath);
			result = true;
		}
		else
		{
			string text = SharedUtils.FinalDataPath + "/done";
			string text2 = SharedUtils.FinalDataPath + "/last";
			string path = SharedUtils.FinalDataPath + "/interrupt";
			bool flag2 = File.Exists(text);
			bool flag3 = !flag2 && !File.Exists(text2);
			if (flag3)
			{
				SharedUtils.Log("Both done and last file are not found!");
				return false;
			}
			try
			{
				StreamReader streamReader = File.OpenText(flag2 ? text : text2);
				string s = streamReader.ReadLine();
				int num = 0;
				bool flag4 = int.TryParse(s, out num);
				bool flag5 = flag4;
				if (flag5)
				{
					SharedUtils.frameId = num - 1;
					result = true;
					StreamWriter streamWriter = new StreamWriter(path, true, Encoding.Default, 1024);
					streamWriter.WriteLine(SharedUtils.frameId.ToString());
					streamWriter.Close();
					bool enabled = this._timeTrackManager.Enabled;
					if (enabled)
					{
						this._timeTrackManager.Enabled = this.CheckLastTime();
						bool flag6 = !this._timeTrackManager.Enabled;
						if (flag6)
						{
							CoreUtils.UWASendLogToServer("Time Track " + (this._timeTrackManager.Enabled ? "Enabled" : "Disabled"));
						}
					}
				}
				streamReader.Close();
			}
			catch (Exception ex)
			{
				string str = "Read done or last failed : ";
				Exception ex2 = ex;
				SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				return false;
			}
			this.ClearUnusedData();
		}
		return result;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000F01C File Offset: 0x0000D21C
	private bool TestInit()
	{
		SharedUtils.Log("Start TestInit");
		bool flag = !UWAState.NewTest && !UWAState.ContinousTest;
		bool result;
		if (flag)
		{
			SharedUtils.Log("Call TestInit() before set continueTest/newTest");
			result = false;
		}
		else
		{
			this._timeTrackManager.Init(UWACoreConfig.Config4Manager["TimeTrackManager"]);
			this._timeTrackManager.ResetStartTime(0L);
			UWAState.TestDuration = 0.0;
			ScreenTrackManager.TrackMode = ScreenTrackManager.ETrackMode.Image;
			bool flag2 = this.PrepareData();
			bool flag3 = !flag2;
			if (flag3)
			{
				SharedUtils.Log("Data not prepared!");
				result = false;
			}
			else
			{
				this._screenTrackManager = ScreenTrackManager.Get();
				ScreenTrackManager.DoPrepare();
				bool flag4 = UWAState.ParseMode == UWAState.eParseMode.normal || UWAState.ParseMode == UWAState.eParseMode.beginner || UWAState.ParseMode == UWAState.eParseMode.load || UWAState.ParseMode == UWAState.eParseMode.hang;
				if (flag4)
				{
					this._dataTrackManager.Init(UWACoreConfig.Config4Manager["DataTrackManager"]);
					this._screenTrackManager.Init(UWACoreConfig.Config4Manager["ScreenTrackManager"]);
					this._assetTrackManager.Init(UWACoreConfig.Config4Manager["AssetTrackManager"]);
					this._apiTrackManager.Init(UWACoreConfig.Config4Manager["ApiTrackManager"]);
					this._logTrackManager.Init(UWACoreConfig.Config4Manager["LogTrackManager"]);
					this._hardwareTrackManager.Init(UWACoreConfig.Config4Manager["HardwareTrackManager"]);
				}
				CoreUtils.UWASendLogToServer("Data Track \t: " + (this._dataTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Time Track \t: " + (this._timeTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Screen Track \t: " + (this._screenTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Asset Track \t: " + (this._assetTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Api Track \t: " + (this._apiTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Log Track \t: " + (this._logTrackManager.Enabled ? "Enabled" : "Disabled"));
				CoreUtils.UWASendLogToServer("Hardware Track \t: " + (this._hardwareTrackManager.Enabled ? "Enabled" : "Disabled"));
				this._systemInfoBuilder = new SystyemInfoBuilder(CoreUtils.GetLogFileFullPath("systemInfo.txt"));
				this._systemInfoBuilder.CreateSystemInfoFile();
				bool flag5 = UWACoreConfig.REPORT_IP.Contains("-");
				if (flag5)
				{
					File.WriteAllText(CoreUtils.GetLogFileFullPath("report_id"), UWACoreConfig.REPORT_IP);
				}
				HashSet<string> nameSpaces = CoreUtils.ExtractDllClass();
				File.WriteAllText(CoreUtils.GetLogFileFullPath("license"), CoreUtils.GetLicense(nameSpaces));
				CoreUtils.WriteStringToFile(UWAState.ParseMode.ToString(), CoreUtils.GetLogFileFullPath("mode"));
				UWAState.onLevelChanged = new UWAState.OnLevelChange(this.OnLevelChanged);
				UWAState.TestInited = true;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000F3B0 File Offset: 0x0000D5B0
	private void Update()
	{
		UWAGUI.InputCheck();
		Cursor.lockState = 0;
		bool flag = UWAState.SendLastPack != null && UWAState.SendLastPack.Value;
		if (flag)
		{
			UWACoreConfig.TIME_VERSION = DateTime.Now.ToString("yyyyMMdd-HHmmss");
			base.StartCoroutine(this.TrySendDataAndQuit(UWAState.HasLastPack));
			UWAState.SendLastPack = new bool?(false);
		}
		bool flag2 = UWAState.RecordingState == UWAState.eRecordingState.Recording && UWAState.TestInited;
		if (flag2)
		{
			UWAState.UpdateSceneName();
			SharedUtils.frameId++;
			bool updateLog = this.UpdateLog;
			if (updateLog)
			{
				SharedUtils.Log("");
			}
			this.testLogTime = new DateTime?(this.testLogTime ?? DateTime.Now);
			UWAState.TestDuration += (DateTime.Now - this.testLogTime).Value.TotalMinutes;
			this.testLogTime = new DateTime?(DateTime.Now);
			bool flag3 = SharedUtils.frameId % 1000 == 0;
			if (flag3)
			{
				long runtimeRomSpace = AndroidHardwareTrackManager.GetRuntimeRomSpace();
				bool flag4 = runtimeRomSpace != -1L && runtimeRomSpace < 50000000L;
				if (flag4)
				{
					this.spaceLow = true;
				}
			}
			bool flag5 = this.checkScreen && SharedUtils.frameId % 500 == 0;
			if (flag5)
			{
				bool flag6 = this._screenTrackManager.Enabled && ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Image;
				if (flag6)
				{
					string[] files = Directory.GetFiles(SharedUtils.FinalDataPath, "*.jpg");
					bool flag7 = files.Length == 0 && SharedUtils.frameId > 499;
					if (flag7)
					{
						UWAState.ScreenLost = true;
					}
					bool flag8 = files.Length == this.lastScreenNum && this.lastScreenNum != -1;
					if (flag8)
					{
						UWAState.ScreenLost = true;
					}
					this.lastScreenNum = files.Length;
					bool flag9 = UWAState.GpuInNormal && SharedUtils.frameId <= 1000;
					if (flag9)
					{
						string[] files2 = Directory.GetFiles(SharedUtils.FinalDataPath, "*mipmap.jpg");
						string[] files3 = Directory.GetFiles(SharedUtils.FinalDataPath, "*overdraw.jpg");
						bool flag10 = files.Length == files2.Length + files3.Length && SharedUtils.frameId > 499;
						if (flag10)
						{
							UWAState.ScreenLost = true;
						}
					}
				}
			}
			bool flag11 = SharedUtils.frameId % 500 == 0;
			if (flag11)
			{
				bool enabled = this._dataTrackManager.Enabled;
				if (enabled)
				{
					string[] files4 = Directory.GetFiles(SharedUtils.FinalDataPath, "*.data");
					bool flag12 = files4.Length == 0;
					if (flag12)
					{
						files4 = Directory.GetFiles(SharedUtils.FinalDataPath, "*.raw");
					}
					int num = files4.Length;
					bool flag13 = num == 0 && SharedUtils.frameId > 499;
					if (flag13)
					{
						UWAState.DataLost = true;
					}
					bool flag14 = num == this.lastDataNum && this.lastDataNum != -1;
					if (flag14)
					{
						UWAState.DataLost = true;
					}
					this.lastDataNum = num;
					bool flag15 = SharedUtils.frameId % 1000 == 0;
					if (flag15)
					{
						long num2 = 0L;
						for (int i = 0; i < files4.Length; i++)
						{
							FileInfo fileInfo = new FileInfo(files4[i]);
							num2 += fileInfo.Length;
						}
						float num3 = (float)num2 / 1000000f;
						bool flag16 = Mathf.Abs(num3) < 4f && SharedUtils.frameId > 999;
						if (flag16)
						{
							UWAState.DataLost = true;
						}
						bool flag17 = num3 - this.lastdataSize < 5f && SharedUtils.frameId > 999;
						if (flag17)
						{
							UWAState.DataLost = true;
						}
						this.lastdataSize = num3;
						CoreUtils.UWASendLogToServer(string.Format("{0} frames, {1:F} min, {2} ss, {3:F} M ds.", new object[]
						{
							SharedUtils.frameId,
							UWAState.TestDuration,
							this.lastScreenNum,
							this.lastdataSize
						}));
					}
				}
			}
			bool flag18 = !this.trackerEnabled;
			if (flag18)
			{
				this.EnableTrackers();
				this.trackerEnabled = true;
				SharedUtils.Log("UWA test starts.");
			}
			this.CheckTimeAndSwitchLogFile();
			base.StartCoroutine(this.UWAStarter_EndOfFrame());
		}
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
	private void EnableTrackers()
	{
		BaseTrackerManager.StartTrackAll();
		base.StartCoroutine(this.WaitAndGetAllType());
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000F8D8 File Offset: 0x0000DAD8
	private void CheckTimeAndSwitchLogFile()
	{
		BaseTrackerManager.CheckTimeAndSwitchLogFileAll();
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000F8E4 File Offset: 0x0000DAE4
	private IEnumerator UWAStarter_EndOfFrame()
	{
		yield return this._endOfFrame;
		bool flag = this._lastEndFrame == SharedUtils.frameId;
		if (flag)
		{
			yield break;
		}
		this._lastEndFrame = SharedUtils.frameId;
		Profiler.BeginSample("UWAStarter_EndOfFrame");
		bool switchLevelSync = this.SwitchLevelSync;
		if (switchLevelSync)
		{
			this.SwitchLevelSync = false;
			bool flag2 = UWAState.RecordingState == UWAState.eRecordingState.Recording;
			if (flag2)
			{
				BaseTrackerManager.SwitchLogFileAll();
				base.StopCoroutine(this.WaitAndGetAllType());
				base.StartCoroutine(this.WaitAndGetAllType());
			}
		}
		BaseTrackerManager.UpdateAtEndAll();
		Profiler.EndSample();
		yield break;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000F8F4 File Offset: 0x0000DAF4
	private void OnLevelChanged()
	{
		bool flag = Application.loadedLevelName == "";
		if (!flag)
		{
			Profiler.BeginSample("UWAStarter.OnLevelWasLoaded");
			CoreUtils.CurrentLevelName = Application.loadedLevelName.Replace("/", "-");
			CoreUtils.LocalFileIndex = 0;
			Profiler.EndSample();
			this.SwitchLevelSync = true;
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000F95C File Offset: 0x0000DB5C
	private IEnumerator WaitAndGetAllType()
	{
		yield return new WaitForSeconds(0.5f);
		this._assetTrackManager.MakeNextGetAllAssets();
		yield break;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000F96C File Offset: 0x0000DB6C
	private void LocalInitConfig()
	{
		UWACoreConfig.ParseConfigLocal();
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000F978 File Offset: 0x0000DB78
	private IEnumerator DownloadAndInitConfig()
	{
		WWW www = UWACoreConfig.CheckIpValid() ? new WWW(UWACoreConfig.ConfigUrl) : null;
		bool flag = www != null;
		if (flag)
		{
			yield return www;
		}
		bool flag2 = www != null && string.IsNullOrEmpty(www.error);
		if (flag2)
		{
			byte[] configBytes = www.bytes;
			bool flag3 = configBytes != null;
			if (flag3)
			{
				try
				{
					File.WriteAllBytes(CoreUtils.GetRuntimeConfigPath(), configBytes);
				}
				catch (Exception)
				{
				}
				UWACoreConfig.ParseConfig(configBytes);
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("Config Parsed");
				}
			}
			configBytes = null;
		}
		else
		{
			bool flag4 = File.Exists(CoreUtils.GetRuntimeConfigPath());
			if (flag4)
			{
				byte[] configBytes2 = File.ReadAllBytes(CoreUtils.GetRuntimeConfigPath());
				UWACoreConfig.ParseConfig(configBytes2);
				bool showLog2 = SharedUtils.ShowLog;
				if (showLog2)
				{
					SharedUtils.Log("Config Parsed");
				}
				configBytes2 = null;
			}
		}
		bool flag5 = www != null;
		if (flag5)
		{
			www.Dispose();
		}
		yield break;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000F988 File Offset: 0x0000DB88
	public IEnumerator ShowRecordingInfo(float sec)
	{
		this.showRecordingInfo = true;
		yield return new WaitForSeconds(sec);
		this.showRecordingInfo = false;
		yield break;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000F9A0 File Offset: 0x0000DBA0
	public void OnApplicationQuit()
	{
		bool flag = UWAState.RecordingState == UWAState.eRecordingState.Recording;
		if (flag)
		{
			base.StartCoroutine(this.SwitchRecording());
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
	public IEnumerator TrySendDataAndQuit(bool zipped = false)
	{
		bool flag = UWAState.UploadType == UWAState.eUploadType.Online;
		if (flag)
		{
			File.WriteAllText(SharedUtils.FinalDataPath + "/online", "");
		}
		bool flag2 = UWAState.UploadType == UWAState.eUploadType.Offline;
		if (flag2)
		{
			File.WriteAllText(SharedUtils.FinalDataPath + "/offline", "");
		}
		bool flag3 = UWAState.UploadType == UWAState.eUploadType.Test;
		if (flag3)
		{
			File.WriteAllText(SharedUtils.FinalDataPath + "/admin", "");
			File.WriteAllText(SharedUtils.FinalDataPath + "/test", "");
		}
		yield return base.StartCoroutine(PkgSendUtil.SendFileInZip(SharedUtils.FinalDataPath, string.Concat(new string[]
		{
			UWACoreConfig.KEY,
			"$",
			UWACoreConfig.TIME_VERSION,
			"$",
			Path.GetFileName(SharedUtils.FinalDataPath + ".zip")
		}), zipped, null));
		bool flag4 = PkgSendUtil.State == PkgSendUtil.WorkState.Success;
		if (flag4)
		{
			Application.Quit();
		}
		yield break;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000F9E8 File Offset: 0x0000DBE8
	public IEnumerator SwitchRecording()
	{
		bool flag = UWAState.RecordingState == UWAState.eRecordingState.Recording;
		if (flag)
		{
			UWAState.RecordingState = UWAState.eRecordingState.PrepareForStop;
			BaseTrackerManager.StopTrackAll();
			this.trackerEnabled = false;
			bool flag2 = this.StopCallback != null;
			if (flag2)
			{
				this.StopCallback();
			}
			UWAState.RecordingState = UWAState.eRecordingState.Stopped;
			CoreUtils.UWASendLogToServer("Stop a record...");
			bool useInternalDataFolder = SharedUtils.UseInternalDataFolder;
			if (useInternalDataFolder)
			{
				base.StartCoroutine(this.TrySendDataAndQuit(false));
			}
		}
		bool flag3 = UWAState.RecordingState == UWAState.eRecordingState.Unset;
		if (flag3)
		{
			UWAState.RecordingState = UWAState.eRecordingState.PrepareForRecord;
			this.LocalInitConfig();
			bool flag4 = !this.TestInit();
			if (flag4)
			{
				SharedUtils.Log("TestInit failed, so switch failed!");
				UWAState.ContinousTest = false;
				UWAState.NewTest = false;
				UWAState.RecordingState = UWAState.eRecordingState.Stopped;
				yield break;
			}
			UWAState.CheckTestError();
			while (UWAState.HasError)
			{
				yield return null;
			}
			bool flag5 = !string.IsNullOrEmpty(UWAState.Note);
			if (flag5)
			{
				CoreUtils.WriteStringToFile(UWAState.Note, CoreUtils.GetLogFileFullPath("note"));
			}
			bool flag6 = this.StartCallback != null;
			if (flag6)
			{
				this.StartCallback();
			}
			CoreUtils.UWASendLogToServer(UWAState.NewTest ? "Start a new record..." : "Go on with a previous record...");
			UWAState.RecordingState = UWAState.eRecordingState.Recording;
			UWAState.GUIShow = false;
		}
		yield break;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000F9F8 File Offset: 0x0000DBF8
	private void OnGUI()
	{
		bool vrMode = UWAState.VrMode;
		if (!vrMode)
		{
			GUI.enabled = true;
			this.DoSomeThingOnGUI();
			bool flag = !string.IsNullOrEmpty(UWAGUI.ErrorInfo);
			if (flag)
			{
				UWAGUI.ShowErrorInfo(UWAGUI.ErrorInfo);
			}
			else
			{
				bool hasLastData = UWAState.HasLastData;
				if (hasLastData)
				{
					bool flag2 = UWAState.SendLastPack == null;
					if (flag2)
					{
						UWAGUI.ShowLastPackInfo();
					}
				}
				else
				{
					UWAGUI.NonVRGUI();
				}
				bool flag3 = this.showRecordingInfo;
				if (flag3)
				{
					UWAGUI.ShowRecordingInfo();
				}
				bool flag4 = this.checkScreen && UWAState.ScreenLost;
				if (flag4)
				{
					bool flag5 = UWAGUI.ShowLostInfo("约有 500 帧没有生成 截图 了。\n请强制退出，并在 1~2 分钟后重新开始。", true);
					bool flag6 = flag5;
					if (flag6)
					{
						this.checkScreen = false;
					}
				}
				bool dataLost = UWAState.DataLost;
				if (dataLost)
				{
					UWAGUI.ShowLostInfo("约有 500 帧没有生成 Data 了。\n请强制退出，并在 1~2 分钟后重新开始。", false);
				}
				bool screencapLost = UWAState.ScreencapLost;
				if (screencapLost)
				{
					UWAGUI.ShowLostInfo("截图生成失败，请联系开发人员，告知 Unity 版本。", false);
				}
				bool flag7 = this.spaceLow;
				if (flag7)
				{
					UWAGUI.ShowSpaceLowInfo();
				}
				bool flag8 = this.staticBatchUsed;
				if (flag8)
				{
					UWAGUI.ShowStaticBatchInfo();
				}
				bool flag9 = this.projreview;
				if (flag9)
				{
					this.projreview = !UWAGUI.ShowProjreviewInfo();
				}
				bool flag10 = PkgSendUtil.State == PkgSendUtil.WorkState.Uploading || PkgSendUtil.State == PkgSendUtil.WorkState.Zipping;
				if (flag10)
				{
					UWAGUI.ShowProgressInfo("Compressing & Uploading", PkgSendUtil.Percent);
				}
				bool flag11 = PkgSendUtil.State == PkgSendUtil.WorkState.FailedInZip;
				if (flag11)
				{
					UWAGUI.ShowProgressInfo("压缩失败，请检查磁盘空间。", PkgSendUtil.Percent);
				}
				bool flag12 = PkgSendUtil.State == PkgSendUtil.WorkState.FailedInUpload;
				if (flag12)
				{
					UWAGUI.ShowProgressInfo("上传失败，请检查网络连接。", PkgSendUtil.Percent);
				}
			}
		}
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000FBC8 File Offset: 0x0000DDC8
	private void OnDisable()
	{
		CoreUtils.UWASendLogToServer(string.Format("Starter Disabled at {0}.", SharedUtils.frameId));
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
	private void OnDestroy()
	{
		Screen.sleepTimeout = -2;
		BaseTrackerManager.StopTrackAll();
		this.trackerEnabled = false;
		CoreUtils.UWASendLogToServer(string.Format("Starter Destroyed at {0}.", SharedUtils.frameId));
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000FC1C File Offset: 0x0000DE1C
	public static void SetUWAKey(string key)
	{
		SharedUtils.Log("key set skip");
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000FC2C File Offset: 0x0000DE2C
	public static void SetServerIp(string ip, int port)
	{
		UWACoreConfig.ToAddStarter = true;
		UWACoreConfig.IP = ip;
		UWACoreConfig.LOG_IP = ip;
		UWACoreConfig.PORT = port;
		SharedUtils.Log("ip, port set successfully");
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000FC54 File Offset: 0x0000DE54
	public static void SetPluginsVersion(string version)
	{
		UWACoreConfig.PLUGIN_VERSION = version;
		SharedUtils.Log("plugin version set successfully");
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000FC68 File Offset: 0x0000DE68
	public static void SetGUIServer()
	{
		UWAState.OpenServer();
		SharedUtils.Log("open gui server");
	}

	// Token: 0x0400013B RID: 315
	private static UWAStarter _instance;

	// Token: 0x0400013C RID: 316
	private bool trackerEnabled = false;

	// Token: 0x0400013D RID: 317
	private DateTime? testLogTime = null;

	// Token: 0x0400013E RID: 318
	private bool showRecordingInfo = false;

	// Token: 0x0400013F RID: 319
	private int lastScreenNum = -1;

	// Token: 0x04000140 RID: 320
	private int lastDataNum = -1;

	// Token: 0x04000141 RID: 321
	private float lastdataSize = -10f;

	// Token: 0x04000142 RID: 322
	private bool checkScreen = true;

	// Token: 0x04000143 RID: 323
	private bool spaceLow = false;

	// Token: 0x04000144 RID: 324
	private bool staticBatchUsed = false;

	// Token: 0x04000145 RID: 325
	private bool projreview = false;

	// Token: 0x04000146 RID: 326
	private DataTrackManager _dataTrackManager;

	// Token: 0x04000147 RID: 327
	private TimeTrackManager _timeTrackManager;

	// Token: 0x04000148 RID: 328
	private ScreenTrackManager _screenTrackManager;

	// Token: 0x04000149 RID: 329
	private AssetTrackManager _assetTrackManager;

	// Token: 0x0400014A RID: 330
	private LogTrackManager _logTrackManager;

	// Token: 0x0400014B RID: 331
	private ApiTrackManager _apiTrackManager;

	// Token: 0x0400014C RID: 332
	private AndroidHardwareTrackManager _hardwareTrackManager;

	// Token: 0x0400014D RID: 333
	private SystyemInfoBuilder _systemInfoBuilder;

	// Token: 0x0400014E RID: 334
	public UWAStarter.OnRecordStart StartCallback;

	// Token: 0x0400014F RID: 335
	public UWAStarter.OnRecordStop StopCallback;

	// Token: 0x04000150 RID: 336
	private string GoName = "";

	// Token: 0x04000151 RID: 337
	private List<string> ShowInfos = new List<string>();

	// Token: 0x04000152 RID: 338
	private StreamWriter sw = null;

	// Token: 0x04000153 RID: 339
	private bool UpdateLog = false;

	// Token: 0x04000154 RID: 340
	private readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();

	// Token: 0x04000155 RID: 341
	private int _lastEndFrame = -1;

	// Token: 0x04000156 RID: 342
	private bool SwitchLevelSync = false;

	// Token: 0x04000157 RID: 343
	private bool open = false;

	// Token: 0x04000158 RID: 344
	private float TouchTime = 0f;

	// Token: 0x020000EB RID: 235
	// (Invoke) Token: 0x060008F9 RID: 2297
	public delegate void OnRecordStart();

	// Token: 0x020000EC RID: 236
	// (Invoke) Token: 0x060008FD RID: 2301
	public delegate void OnRecordStop();
}
