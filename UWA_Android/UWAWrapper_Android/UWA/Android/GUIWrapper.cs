using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UWALocal;
using UWASDK;
using UWAShared;

namespace UWA.Android
{
	// Token: 0x02000006 RID: 6
	[ExecuteInEditMode]
	[Preserve]
	[ComVisible(false)]
	public class GUIWrapper : MonoBehaviour
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002368 File Offset: 0x00000568
		public static GUIWrapper Get()
		{
			return GUIWrapper._instance;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002370 File Offset: 0x00000570
		private void Awake()
		{
			try
			{
				this._title = "UWA Online Mode v" + Utils.PluginVersion;
				if (!Application.isEditor)
				{
					this._title += (SharedUtils.Il2Cpp() ? "[IL2CPP]" : "[Mono]");
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("UWA SDKv" + Utils.PluginVersion + " awake.");
					}
				}
			}
			catch
			{
			}
			if (GUIWrapper._inited)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(base.gameObject.GetComponent<GUIWrapper>());
				}
				return;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			GUIWrapper._inited = true;
			GUIWrapper._instance = this;
			if (Application.isEditor)
			{
				this.fontSize = 10;
				return;
			}
			bool flag = WrapperTool.CheckPersistFile("auto");
			SharedUtils.ShowLog |= WrapperTool.CheckPersistFile("uwa_log");
			SharedUtils.Log2File = SharedUtils.ShowLog;
			this.VrMode = SharedUtils.isVRMode();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("VrMode: " + this.VrMode.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Auto: " + flag.ToString());
			}
			bool flag2 = SharedUtils.VerifyWritePermision();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("VerifyWritePermision : " + flag2.ToString());
			}
			SharedUtils.SetupAllPath();
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UseInternalPath : " + SharedUtils.UseInternalDataFolder.ToString());
			}
			bool flag3 = WrapperTool.TryGetSavedIP(out WrapperTool.UWAServerIp);
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("ipGet : " + flag3.ToString());
			}
			if (this.VrMode && flag)
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("auto download.");
				}
				base.StartCoroutine(this.RepeatGetSavedIP());
				SdkCtrlData.Instance.SdkMode = eSdkMode.PA;
				base.StartCoroutine(this.AutoDownload());
			}
			if (this.VrMode && flag)
			{
				return;
			}
			bool onlineMode = WrapperTool.OnlineMode;
			bool flag4 = this.CheckDirectMode(onlineMode);
			TextAsset textAsset = Resources.Load<TextAsset>("uwasdksettings");
			UwaSdkSettings uwaSdkSettings;
			if (textAsset == null)
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("Uwa Settings not found, use default.");
				}
				uwaSdkSettings = new UwaSdkSettings();
			}
			else
			{
				uwaSdkSettings = new UwaSdkSettings(textAsset.text);
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("onlineMode : " + onlineMode.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("dllLoaded : " + flag4.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("EnableGPM : " + uwaSdkSettings.EnableUWAGPM.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("EnableGOT : " + uwaSdkSettings.EnableUWAGOT.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("EnableOL : " + uwaSdkSettings.EnableUWAOL.ToString());
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("EnablePoco : " + uwaSdkSettings.EnableUWAPoco.ToString());
			}
			GUIWrapper.ControlByPoco = (GUIWrapper.ControlByPoco || uwaSdkSettings.EnableUWAPoco);
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("ControlByPoco : " + GUIWrapper.ControlByPoco.ToString());
			}
			if (!flag4 && !onlineMode)
			{
				if (!uwaSdkSettings.EnableUWAGPM)
				{
					SdkUIMgr.RemoveFeature("gpm");
				}
				if (!uwaSdkSettings.EnableUWAGOT)
				{
					SdkUIMgr.RemoveFeature("got");
				}
				if (!uwaSdkSettings.EnableUWAGOT && !uwaSdkSettings.EnableUWAGPM)
				{
					SdkCtrlData.Instance.SdkMode = eSdkMode.None;
				}
				if ((SdkCtrlData.Instance.SdkMode == eSdkMode.GOT || SdkCtrlData.Instance.SdkMode == eSdkMode.GPM || SdkCtrlData.Instance.SdkMode == eSdkMode.None) && Utils.SupportType(eSdkType.GOT) && this.TryLoadDllSync(eSdkType.GOT, false))
				{
					flag4 = true;
					base.StartCoroutine(this.DelayDestroy());
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("dllLoaded : " + flag4.ToString());
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TestMode : " + SdkCtrlData.Instance.SdkMode.ToString());
				}
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002838 File Offset: 0x00000A38
		private bool CheckDirectMode(bool online)
		{
			bool result = false;
			eSdkType eSdkType = WrapperTool.CheckDirectSdkType();
			if (eSdkType == eSdkType.PA)
			{
				SdkCtrlData.Instance.SdkMode = eSdkMode.PA;
				if (this.TryLoadDllSync(eSdkType.PA, online))
				{
					result = true;
					base.StartCoroutine(this.DelayDestroy());
				}
			}
			else if (eSdkType == eSdkType.GOT)
			{
				WrapperTool.DirectMode = WrapperTool.CheckGotDirectMode();
				if (WrapperTool.DirectMode != WrapperTool.eDirectMode.UnSet)
				{
					SdkCtrlData.Instance.SdkMode = eSdkMode.GOT;
					SdkCtrlData.Instance.GotMode = (eGotMode)WrapperTool.DirectMode;
					SdkCtrlData.Instance.SdkMode = eSdkMode.GOT;
					if (this.TryLoadDllSync(eSdkType.GOT, online))
					{
						result = true;
						base.StartCoroutine(this.DelayDestroy());
					}
				}
			}
			WrapperTool.DeleteDirectFile();
			return result;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000028E8 File Offset: 0x00000AE8
		private IEnumerator AutoDownload()
		{
			yield return new WaitForSeconds(5f);
			base.StopCoroutine(this.RepeatGetSavedIP());
			base.StartCoroutine(this.DownLoadUwaDll());
			WrapperTool.SetSavedIP(WrapperTool.UWAServerIp);
			yield break;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000028F8 File Offset: 0x00000AF8
		private void Update()
		{
			Cursor.lockState = 0;
			if (!this.startDownload && this.VrMode && Input.GetKeyUp(293))
			{
				base.StopCoroutine(this.RepeatGetSavedIP());
				base.StartCoroutine(this.DownLoadUwaDll());
				WrapperTool.SetSavedIP(WrapperTool.UWAServerIp);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002958 File Offset: 0x00000B58
		private IEnumerator RepeatGetSavedIP()
		{
			for (;;)
			{
				string text = "";
				if (!WrapperTool.TryGetSavedIP(out text))
				{
					break;
				}
				if (text != WrapperTool.UWAServerIp)
				{
					WrapperTool.UWAServerIp = text;
				}
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002960 File Offset: 0x00000B60
		private void DoOnlineModeSetWindow(int windowId)
		{
			int num = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = this.fontSize;
			GUI.skin.label.fontSize = this.fontSize;
			GUI.skin.textField.fontSize = this.fontSize;
			float x = 0.9f / (float)Utils.GetSupportedType().Count;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (Utils.SupportType(eSdkType.GOT) && GUILayout.Button("GOT", SharedUtils.GetSuitableOption(x, 0.4f)))
			{
				SdkCtrlData.Instance.SdkMode = eSdkMode.GOT_D;
				if (SharedUtils.Il2Cpp())
				{
					SdkCtrlData.Instance.SdkMode = eSdkMode.GOT;
					base.StartCoroutine(this.AttachUwaDirect());
				}
			}
			if (Utils.SupportType(eSdkType.PA) && GUILayout.Button("PA", SharedUtils.GetSuitableOption(x, 0.4f)))
			{
				SdkCtrlData.Instance.SdkMode = eSdkMode.PA;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.skin.button.fontSize = num;
			GUI.skin.label.fontSize = num;
			GUI.skin.textField.fontSize = num;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002AA4 File Offset: 0x00000CA4
		private bool TryLoadDllSync(eSdkType sdkType, bool localOverride)
		{
			bool result = false;
			if (SharedUtils.Il2Cpp())
			{
				result = this.TryLoadMergedDll(sdkType);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TryLoadMergedDll " + result.ToString());
				}
			}
			else if (sdkType == eSdkType.GOT)
			{
				if (localOverride)
				{
					result = this.TryLoadLocalDll(sdkType);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("TryLoadLocalDll " + result.ToString());
					}
				}
				else
				{
					result = this.TryLoadMergedDll(sdkType);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("TryLoadEmbeddedDll " + result.ToString());
					}
				}
			}
			else if (sdkType == eSdkType.PA)
			{
				result = this.TryLoadLocalDll(sdkType);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TryLoadLocalDll " + result.ToString());
				}
			}
			return result;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002B84 File Offset: 0x00000D84
		private bool TryLoadLocalDll(eSdkType sdkType)
		{
			string starter = WrapperTool.StarterTypeMap[sdkType];
			if (File.Exists(WrapperTool.GetCorePath(sdkType)))
			{
				try
				{
					byte[] rawAssembly = File.ReadAllBytes(WrapperTool.GetCorePath(sdkType));
					Assembly uwaCoreAssembly = Assembly.Load(rawAssembly);
					this.InvokeUwaCore(uwaCoreAssembly, starter);
					return true;
				}
				catch (Exception ex)
				{
					if (SharedUtils.ShowLog)
					{
						string str = "TryLoadLocalDll Ex: ";
						Exception ex2 = ex;
						SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002C18 File Offset: 0x00000E18
		private bool TryLoadMergedDll(eSdkType sdkType)
		{
			string text = WrapperTool.StarterTypeMap[sdkType];
			if (text == null)
			{
				return false;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (executingAssembly.GetType(text) != null)
			{
				this.InvokeUwaCore(executingAssembly, text);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TryLoadMergedDll success : " + text);
				}
				return true;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetType(text) != null)
				{
					this.InvokeUwaCore(assembly, text);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("TryLoadMergedDll success : " + text);
					}
					return true;
				}
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("TryLoadMergedDll failed : " + text);
			}
			return false;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002CEC File Offset: 0x00000EEC
		private IEnumerator AttachUwaDirect()
		{
			this.startDownload = true;
			yield return null;
			eSdkType sdkByTestType = WrapperTool.GetSdkByTestType(SdkCtrlData.Instance.SdkMode);
			this.TryLoadMergedDll(sdkByTestType);
			base.StartCoroutine(this.DelayDestroy());
			yield break;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002CFC File Offset: 0x00000EFC
		private void InvokeUwaCore(Assembly uwaCoreAssembly, string starter)
		{
			if (uwaCoreAssembly != null)
			{
				Type type = uwaCoreAssembly.GetType(starter);
				if (type == null)
				{
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("UWAStarterType " + starter + " == null");
					}
					return;
				}
				try
				{
					MethodInfo method = type.GetMethod("SetServerIp");
					if (method != null)
					{
						method.Invoke(null, new object[]
						{
							WrapperTool.UWAServerIp,
							int.Parse(WrapperTool.UWAServerPort)
						});
					}
					MethodInfo method2 = type.GetMethod("SetUWAKey");
					if (method2 != null)
					{
						method2.Invoke(null, new object[]
						{
							""
						});
					}
					MethodInfo method3 = type.GetMethod("SetPluginsVersion");
					if (method3 != null)
					{
						method3.Invoke(null, new object[]
						{
							Utils.PluginVersion
						});
					}
				}
				catch (Exception ex)
				{
					if (SharedUtils.ShowLog)
					{
						string str = "Method Invoke Failed : ";
						Exception ex2 = ex;
						SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				if (base.gameObject.GetComponent(type) == null)
				{
					base.gameObject.AddComponent(type);
					return;
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.LogError("UWAStarter already be attached");
					return;
				}
			}
			else if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UWACore assembly cannot be loaded correctly");
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002E70 File Offset: 0x00001070
		private void DoDownloadWindow(int windowId)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			int num = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = this.fontSize;
			GUI.skin.label.fontSize = this.fontSize;
			GUI.skin.textField.fontSize = this.fontSize;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("UWA Server IP: ", SharedUtils.GetSuitableOption(0.3f, 0.2f));
			WrapperTool.UWAServerIp = GUILayout.TextField(WrapperTool.UWAServerIp, SharedUtils.GetSuitableOption(0.6f, 0.2f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Download", SharedUtils.GetSuitableOption(0.3f, 0.2f)) && !Application.isEditor)
			{
				if (SharedUtils.Il2Cpp())
				{
					base.StartCoroutine(this.AttachUwaDirect());
				}
				else
				{
					base.StartCoroutine(this.DownLoadUwaDll());
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("UWA Server: " + WrapperTool.UWAServerIp);
				}
				if (WrapperTool.CheckIpValid())
				{
					WrapperTool.SetSavedIP(WrapperTool.UWAServerIp);
				}
			}
			if (GUILayout.Button("Local", SharedUtils.GetSuitableOption(0.3f, 0.2f)) && !Application.isEditor)
			{
				if (SharedUtils.Il2Cpp())
				{
					base.StartCoroutine(this.AttachUwaDirect());
				}
				else
				{
					this.startDownload = true;
					this.TryLoadLocalDll(WrapperTool.GetSdkByTestType(SdkCtrlData.Instance.SdkMode));
				}
			}
			if (GUILayout.Button("Cancel", SharedUtils.GetSuitableOption(0.2f, 0.2f)) && !Application.isEditor)
			{
				SdkCtrlData.Instance.SdkMode = eSdkMode.None;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.skin.button.fontSize = num;
			GUI.skin.label.fontSize = num;
			GUI.skin.textField.fontSize = num;
			GUI.enabled = enabled;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003094 File Offset: 0x00001294
		private IEnumerator DelayDestroy()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000309C File Offset: 0x0000129C
		private IEnumerator DownLoadUwaDll()
		{
			this.startDownload = true;
			eSdkType sdk = WrapperTool.GetSdkByTestType(SdkCtrlData.Instance.SdkMode);
			string url = (SdkCtrlData.Instance.SdkMode == eSdkMode.PA) ? Utils.UWACoreUrl : Utils.UWALocalCoreUrl;
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UWACore URL: " + url);
			}
			string starter = WrapperTool.StarterTypeMap[sdk];
			WWW www = WrapperTool.CheckIpValid() ? new WWW(url) : null;
			if (www != null)
			{
				yield return www;
			}
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.PA && (www == null || !string.IsNullOrEmpty(www.error)))
			{
				if (www != null)
				{
					SharedUtils.LogError(www.error);
				}
				string ossCoreUrl = WrapperTool.GetOssCoreUrl(url);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("UWACore OssURL: " + url);
				}
				www = new WWW(ossCoreUrl);
				yield return www;
			}
			if (www != null && string.IsNullOrEmpty(www.error))
			{
				byte[] bytes = www.bytes;
				if (bytes != null)
				{
					try
					{
						File.WriteAllBytes(WrapperTool.GetCorePath(sdk), bytes);
					}
					catch (Exception)
					{
					}
					try
					{
						Assembly uwaCoreAssembly = Assembly.Load(bytes);
						this.InvokeUwaCore(uwaCoreAssembly, starter);
						goto IL_247;
					}
					catch (NotSupportedException)
					{
						this.il2cppBuild = true;
						goto IL_247;
					}
				}
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("Dll bytes downloaded is null");
				}
			}
			else
			{
				if (www != null)
				{
					SharedUtils.LogError("error: " + www.error);
				}
				this.TryLoadLocalDll(sdk);
			}
			IL_247:
			if (this.il2cppBuild)
			{
				this.startDownload = false;
				yield break;
			}
			yield break;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000030AC File Offset: 0x000012AC
		private void OnGUI()
		{
			if (this.startDownload)
			{
				return;
			}
			if (this.VrMode)
			{
				return;
			}
			if (!WrapperTool.OnlineMode)
			{
				return;
			}
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.None)
			{
				GUI.Window(17857, SharedUtils.GroupRect, new GUI.WindowFunction(this.DoOnlineModeSetWindow), "UWA Mode Select");
			}
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.GOT_D)
			{
				GUI.Window(17858, SharedUtils.GroupRect, new GUI.WindowFunction(this.DoDownloadWindow), "UWA Local Mode");
			}
			if (SdkCtrlData.Instance.SdkMode == eSdkMode.PA)
			{
				GUI.Window(17859, SharedUtils.GroupRect, new GUI.WindowFunction(this.DoDownloadWindow), this._title);
			}
		}

		// Token: 0x04000009 RID: 9
		[NonSerialized]
		private static bool _inited;

		// Token: 0x0400000A RID: 10
		[HideInInspector]
		public static bool ControlByPoco;

		// Token: 0x0400000B RID: 11
		private bool startDownload;

		// Token: 0x0400000C RID: 12
		private bool VrMode;

		// Token: 0x0400000D RID: 13
		private int fontSize = 20;

		// Token: 0x0400000E RID: 14
		[NonSerialized]
		private static GUIWrapper _instance;

		// Token: 0x0400000F RID: 15
		private bool il2cppBuild;

		// Token: 0x04000010 RID: 16
		private string _title;
	}
}
