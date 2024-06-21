using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UWA
{
	// Token: 0x02000054 RID: 84
	internal class SystyemInfoBuilder
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x00019910 File Offset: 0x00017B10
		public SystyemInfoBuilder(string filePath)
		{
			this._filePath = filePath;
			this._infoPairs = new Dictionary<string, string>();
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0001992C File Offset: 0x00017B2C
		public void CreateSystemInfoFile()
		{
			this.SetSystemInfo();
			this.WriteIntoFile();
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00019940 File Offset: 0x00017B40
		private void SetSystemInfo()
		{
			DateTime now = DateTime.Now;
			this._infoPairs.Add("Test Time", now.ToString("yyyy-MM-dd HH:mm:ss"));
			this._infoPairs.Add("Plugin Version", UWACoreConfig.PLUGIN_VERSION);
			UWACoreConfig.TIME_VERSION = now.ToString("yyyyMMdd-HHmmss");
			bool flag = !string.IsNullOrEmpty(UWACoreConfig.REPORT_IP);
			if (flag)
			{
				this._infoPairs.Add("UWA PID", UWACoreConfig.REPORT_IP);
				bool flag2 = !string.IsNullOrEmpty(UWACoreConfig.PROJECT_NAME);
				if (flag2)
				{
					this._infoPairs.Add("UWA PNAME", UWACoreConfig.PROJECT_NAME);
				}
				else
				{
					this._infoPairs.Add("UWA PNAME", "default");
				}
			}
			this._infoPairs.Add("UWA Key", UWACoreConfig.KEY);
			this._infoPairs.Add("VR Mode", SharedUtils.isVRMode() ? "True" : "False");
			this._infoPairs.Add("Mono Mode", (UWAState.ParseMode == UWAState.eParseMode.mono_heap) ? "True" : "False");
			this._infoPairs.Add("Lua Mode", (UWAState.ParseMode == UWAState.eParseMode.lua) ? "True" : "False");
			this._infoPairs.Add("Parse Mode", UWAState.ParseMode.ToString());
			this._infoPairs.Add("URP", CoreUtils.IsURP() ? "True" : "False");
			this._infoPairs.Add("IL2CPP Mode", SharedUtils.Il2Cpp() ? "True" : "False");
			this._infoPairs.Add("Device ID", SystemInfo.deviceUniqueIdentifier);
			this._infoPairs.Add("Device Name", SystemInfo.deviceName);
			this._infoPairs.Add("Device Model", SystemInfo.deviceModel);
			this._infoPairs.Add("Device Type", SystemInfo.deviceType.ToString());
			this._infoPairs.Add("Application Name", UWACoreConfig.APP_NAME);
			this._infoPairs.Add("Package Name", UWACoreConfig.PKG_NAME);
			this._infoPairs.Add("App Target SDK", SharedUtils.GetAndroidAppSDK_INT().ToString());
			this._infoPairs.Add("Device Target SDK", SharedUtils.GetAndroidDeviceSDK_INT().ToString());
			this._infoPairs.Add("Bundle Version", CoreUtils.BundleVersionName);
			this._infoPairs.Add("Bundle Version Code", CoreUtils.BundleVersionCode.ToString());
			this._infoPairs.Add("Graphics Device Name", SystemInfo.graphicsDeviceName);
			this._infoPairs.Add("Graphics Device Version", SystemInfo.graphicsDeviceVersion);
			this._infoPairs.Add("Graphics Memory Size", SystemInfo.graphicsMemorySize.ToString());
			this._infoPairs.Add("Graphics Pixel Fill Rate", SystemInfo.graphicsPixelFillrate.ToString());
			this._infoPairs.Add("Operating System", SystemInfo.operatingSystem);
			this._infoPairs.Add("Processor Count", SystemInfo.processorCount.ToString());
			this._infoPairs.Add("Processor Type", SystemInfo.processorType.ToString());
			bool flag3 = SystemInfo.systemMemorySize < 0;
			if (flag3)
			{
				this._infoPairs.Add("System Memory Size", "0");
			}
			else
			{
				this._infoPairs.Add("System Memory Size", SystemInfo.systemMemorySize.ToString());
			}
			this._infoPairs.Add("Unity Version", Application.unityVersion);
			this._infoPairs.Add("Platform", Application.platform.ToString());
			this._infoPairs.Add("System Language", Application.systemLanguage.ToString());
			this._infoPairs.Add("Screen Resolution", Screen.currentResolution.height.ToString() + "*" + Screen.currentResolution.width.ToString());
			this._infoPairs.Add("Screen Orientation", Screen.orientation.ToString());
			this._infoPairs.Add("MultiThreaded Rendering", SystemInfo.graphicsMultiThreaded.ToString());
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00019E14 File Offset: 0x00018014
		private void WriteIntoFile()
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Create(this._filePath)))
				{
					foreach (KeyValuePair<string, string> keyValuePair in this._infoPairs)
					{
						streamWriter.WriteLine(keyValuePair.Key + ": " + keyValuePair.Value);
					}
				}
			}
			catch (IOException ex)
			{
				Debug.Log("exception occurs when write system info into file: " + this._filePath + "\n" + ex.StackTrace);
			}
		}

		// Token: 0x04000237 RID: 567
		private Dictionary<string, string> _infoPairs;

		// Token: 0x04000238 RID: 568
		private string _filePath;

		// Token: 0x0200010F RID: 271
		public class HardwareInfo
		{
			// Token: 0x040006C7 RID: 1735
			public string CpuName;

			// Token: 0x040006C8 RID: 1736
			public string CpuHardware;

			// Token: 0x040006C9 RID: 1737
			public string CpuFreq;

			// Token: 0x040006CA RID: 1738
			public long[] RamMemory;

			// Token: 0x040006CB RID: 1739
			public long[] RomMemory;

			// Token: 0x040006CC RID: 1740
			public long[] SDCardMemory;
		}

		// Token: 0x02000110 RID: 272
		public class AppInfo
		{
			// Token: 0x040006CD RID: 1741
			public string ApplicationName;

			// Token: 0x040006CE RID: 1742
			public string PackageName;
		}
	}
}
