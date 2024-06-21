using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UWA;

// Token: 0x02000032 RID: 50
[ComVisible(false)]
public class ASCTest : MonoBehaviour
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000228 RID: 552 RVA: 0x0000E4E0 File Offset: 0x0000C6E0
	// (set) Token: 0x06000229 RID: 553 RVA: 0x0000E4E8 File Offset: 0x0000C6E8
	public static ASCTest Instance { get; set; }

	// Token: 0x0600022A RID: 554 RVA: 0x0000E4F0 File Offset: 0x0000C6F0
	public static void StaticInit()
	{
		ASCTest.Instance = null;
		bool vrMode = UWAState.VrMode;
		if (vrMode)
		{
			ASCTest.Enabled = false;
		}
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000E51C File Offset: 0x0000C71C
	private void Start()
	{
		bool flag = ASCTest.Instance != null;
		if (flag)
		{
			Object.Destroy(this);
		}
		ASCTest.Instance = this;
		base.StartCoroutine(this.LoadRCPlugin(this.m_rcPlugInURL));
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000E564 File Offset: 0x0000C764
	public void StartServer(bool client)
	{
		bool flag = !ASCTest.Enabled || this.Started;
		if (!flag)
		{
			this.m_ip = UWACoreConfig.Remote_IP;
			this.m_port = UWACoreConfig.Remote_PORT;
			bool flag2 = !client;
			if (flag2)
			{
				bool flag3 = this.StartRCPlugin("startEventSender", this.m_port, "");
				CoreUtils.UWASendLogToServer(flag3 ? "Sever started" : "Sever Start failure");
				this.Started = flag3;
				this.Mode = ASCTest.eMode.Sever;
			}
			else
			{
				bool flag4 = this.StartRCPlugin("startEventReceiver", this.m_port, this.m_ip);
				CoreUtils.UWASendLogToServer(flag4 ? "Client started" : "Client Start failure");
				this.Started = flag4;
				this.Mode = ASCTest.eMode.Client;
			}
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000E644 File Offset: 0x0000C844
	public void StopServer()
	{
		bool flag = !ASCTest.Enabled || !this.Started;
		if (!flag)
		{
			bool flag2 = this.Stop();
			this.Started = !flag2;
			CoreUtils.UWASendLogToServer(flag2 ? "Stopped" : "Stop failure");
			this.Mode = ASCTest.eMode.None;
		}
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600022E RID: 558 RVA: 0x0000E6B0 File Offset: 0x0000C8B0
	private string m_rcPlugInURL
	{
		get
		{
			return UWACoreConfig.DexUrl + "/" + this.m_rcPlugInName;
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000E6E0 File Offset: 0x0000C8E0
	private IEnumerator LoadRCPlugin(string url)
	{
		yield return base.StartCoroutine(this.DownloadRCPPlugin(url));
		bool rcPluginIsDown = this.m_rcPluginIsDown;
		if (rcPluginIsDown)
		{
			try
			{
				this.m_rcPluginLoaded = this.LoadRCUManager();
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("LoadRCUManager returns " + this.m_rcPluginLoaded.ToString());
				}
				ASCTest.Enabled = true;
			}
			catch (Exception ex)
			{
				ASCTest.Enabled = false;
				ASCTest.Instance = null;
			}
		}
		yield break;
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
	private bool StartRCPlugin(string eventHandler, int port, string ip = "")
	{
		Screen.sleepTimeout = -1;
		return this.m_jcRCUManager.CallStatic<bool>(eventHandler, new object[]
		{
			SharedUtils.CurrentActivity,
			ip,
			port
		});
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000E740 File Offset: 0x0000C940
	private bool Stop()
	{
		bool flag = this.m_jcRCUManager != null;
		return flag && this.m_jcRCUManager.CallStatic<bool>("stop", new object[0]);
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000E788 File Offset: 0x0000C988
	private IEnumerator DownloadRCPPlugin(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		bool flag = !string.IsNullOrEmpty(www.error);
		if (flag)
		{
			Debug.Log(www.error);
			yield break;
		}
		bool isDone = www.isDone;
		if (isDone)
		{
			byte[] tempbuffer = www.bytes;
			string path = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getFilesDir", new object[0]).Call<string>("getAbsolutePath", new object[0]) + "/" + this.m_rcPlugInName;
			AndroidJavaObject joFile = new AndroidJavaObject("java.io.File", new object[]
			{
				path
			});
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("Write File " + tempbuffer.Length.ToString() + " " + path);
			}
			AndroidJavaObject joFileOutputStream = new AndroidJavaObject("java.io.FileOutputStream", new object[]
			{
				joFile
			});
			joFileOutputStream.Call("write", new object[]
			{
				tempbuffer
			});
			joFileOutputStream.Call("flush", new object[0]);
			joFileOutputStream.Call("close", new object[0]);
			this.m_rcPluginIsDown = true;
			tempbuffer = null;
			path = null;
			joFile = null;
			joFileOutputStream = null;
		}
		yield break;
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000E7A0 File Offset: 0x0000C9A0
	private bool LoadRCUManager()
	{
		string codeCacheDir = SharedUtils.GetCodeCacheDir();
		AndroidJavaObject androidJavaObject = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getFilesDir", new object[0]);
		string str = androidJavaObject.Call<string>("getAbsolutePath", new object[0]);
		string text = str + "/" + this.m_rcPlugInName;
		bool showLog = SharedUtils.ShowLog;
		if (showLog)
		{
			SharedUtils.Log(text);
		}
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("dalvik.system.DexClassLoader", new object[]
		{
			text,
			codeCacheDir,
			null,
			SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getClassLoader", new object[0])
		});
		bool showLog2 = SharedUtils.ShowLog;
		if (showLog2)
		{
			SharedUtils.Log("DexClassLoader Called");
		}
		this.m_jcRCUManager = androidJavaObject2.Call<AndroidJavaClass>("loadClass", new object[]
		{
			"com.uwa4d.rsc.rcunity.RCUManager"
		});
		bool flag = this.m_jcRCUManager != null;
		bool result;
		if (flag)
		{
			bool showLog3 = SharedUtils.ShowLog;
			if (showLog3)
			{
				SharedUtils.Log("m_jcRCUManager != null");
			}
			result = this.m_jcRCUManager.CallStatic<bool>("setLoadState", new object[]
			{
				true
			});
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x04000131 RID: 305
	public static bool Enabled = true;

	// Token: 0x04000133 RID: 307
	public ASCTest.eMode Mode = ASCTest.eMode.None;

	// Token: 0x04000134 RID: 308
	public bool Started = false;

	// Token: 0x04000135 RID: 309
	private string m_ip;

	// Token: 0x04000136 RID: 310
	private int m_port;

	// Token: 0x04000137 RID: 311
	private string m_rcPlugInName = "uwaTools_RC.dex";

	// Token: 0x04000138 RID: 312
	private bool m_rcPluginIsDown = false;

	// Token: 0x04000139 RID: 313
	private bool m_rcPluginLoaded = false;

	// Token: 0x0400013A RID: 314
	private AndroidJavaClass m_jcRCUManager;

	// Token: 0x020000E8 RID: 232
	public enum eMode
	{
		// Token: 0x040005F6 RID: 1526
		None,
		// Token: 0x040005F7 RID: 1527
		Sever,
		// Token: 0x040005F8 RID: 1528
		Client
	}
}
