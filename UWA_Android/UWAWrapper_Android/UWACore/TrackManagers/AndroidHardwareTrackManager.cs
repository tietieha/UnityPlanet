using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003A RID: 58
	internal class AndroidHardwareTrackManager : HardwareTrackManager
	{
		// Token: 0x06000294 RID: 660
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		protected static extern bool SupportFeature(string feature);

		// Token: 0x06000295 RID: 661
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		protected static extern float GetTiming();

		// Token: 0x06000296 RID: 662
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		protected static extern void SetEventId(int event1, int event2, int event3, int event4, int event5, int event6, int event7);

		// Token: 0x06000297 RID: 663 RVA: 0x00011BE0 File Offset: 0x0000FDE0
		public AndroidHardwareTrackManager(string extension) : base(extension)
		{
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00011C08 File Offset: 0x0000FE08
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			base.InitWithConfig(config);
			int numberOfCores = this.GetNumberOfCores();
			this._runtimeInfo.CpuLoads = ((numberOfCores != -1) ? new float[numberOfCores] : null);
			this._runtimeInfo.CpuFreqs = ((numberOfCores != -1) ? new int[numberOfCores] : null);
			this._runtimeInfo.LastCpuTicks = ((numberOfCores != -1) ? new float[numberOfCores * 8] : null);
			bool flag = config.Count != 0;
			if (flag)
			{
				HardwareTrackManager.Tasks.Clear();
				bool flag2 = config.ContainsKey("Pss");
				if (flag2)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.Pss, float.Parse(config["Pss"]), new HardwareTrackManager.WorkCall(this.UpdatePss), true));
				}
				bool flag3 = config.ContainsKey("Network");
				if (flag3)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.Network, float.Parse(config["Network"]), new HardwareTrackManager.WorkCall(this.UpdateNetwork), false));
				}
				bool flag4 = config.ContainsKey("Battery");
				if (flag4)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.Battery, float.Parse(config["Battery"]), new HardwareTrackManager.WorkCall(this.UpdateBattery), false));
				}
				bool flag5 = config.ContainsKey("Temperature");
				if (flag5)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.Temperature, float.Parse(config["Temperature"]), new HardwareTrackManager.WorkCall(this.UpdateTemperature), false));
				}
				bool flag6 = numberOfCores != -1 && config.ContainsKey("CpuLoad");
				if (flag6)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.CpuLoad, float.Parse(config["CpuLoad"]), new HardwareTrackManager.WorkCall(this.UpdateCpuLoad), false));
				}
				bool flag7 = config.ContainsKey("GpuLoad");
				if (flag7)
				{
					HardwareTrackManager.Tasks.Add(new HardwareTrackManager.HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType.GpuLoad, float.Parse(config["GpuLoad"]), new HardwareTrackManager.WorkCall(this.UpdateGpuLoad), false));
				}
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011E38 File Offset: 0x00010038
		protected override void AsyncCall(HardwareTrackManager.WorkCall call)
		{
			SharedUtils.CurrentActivity.Call("runOnUiThread", new object[]
			{
				new AndroidJavaRunnable(call.Invoke)
			});
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011E60 File Offset: 0x00010060
		public int GetPss()
		{
			object[] array = new object[]
			{
				new int[]
				{
					AndroidHardwareTrackManager.Pid
				}
			};
			AndroidJavaObject[] array2 = AndroidHardwareTrackManager._oActivityManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", array);
			return array2[0].Call<int>("getTotalPss", new object[0]);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00011EBC File Offset: 0x000100BC
		private void UpdatePss()
		{
			bool flag = SharedUtils.GetAndroidAppSDK_INT() >= 29;
			if (flag)
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.Debug$MemoryInfo", new object[0]);
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Debug");
				object[] array = new object[]
				{
					androidJavaObject
				};
				androidJavaClass.CallStatic("getMemoryInfo", array);
				HardwareTrackManager.RuntimeInfo runtimeInfo = this._runtimeInfo;
				Vector4 pssInfo = default(Vector4);
				pssInfo.x = (float)androidJavaObject.Call<int>("getTotalPrivateDirty", new object[0]);
				pssInfo.y = (float)androidJavaObject.Call<int>("getTotalSharedDirty", new object[0]);
				pssInfo.z = (float)androidJavaObject.Call<int>("getTotalPrivateClean", new object[0]);
				pssInfo.w = (float)androidJavaObject.Call<int>("getTotalPss", new object[0]);
				runtimeInfo.PssInfo = pssInfo;
			}
			else
			{
				object[] array2 = new object[]
				{
					new int[]
					{
						AndroidHardwareTrackManager.Pid
					}
				};
				AndroidJavaObject[] array3 = AndroidHardwareTrackManager._oActivityManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", array2);
				HardwareTrackManager.RuntimeInfo runtimeInfo2 = this._runtimeInfo;
				Vector4 pssInfo = default(Vector4);
				pssInfo.x = (float)array3[0].Call<int>("getTotalPrivateDirty", new object[0]);
				pssInfo.y = (float)array3[0].Call<int>("getTotalSharedDirty", new object[0]);
				pssInfo.z = (float)array3[0].Call<int>("getTotalPrivateClean", new object[0]);
				pssInfo.w = (float)array3[0].Call<int>("getTotalPss", new object[0]);
				runtimeInfo2.PssInfo = pssInfo;
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00012058 File Offset: 0x00010258
		private void UpdateNetwork()
		{
			long num = AndroidHardwareTrackManager._clsTrafficStats.CallStatic<long>("getUidRxBytes", new object[]
			{
				AndroidHardwareTrackManager._uid
			});
			long num2 = AndroidHardwareTrackManager._clsTrafficStats.CallStatic<long>("getUidTxBytes", new object[]
			{
				AndroidHardwareTrackManager._uid
			});
			bool flag = num == -1L && num2 == -1L;
			if (flag)
			{
				this._runtimeInfo.NetworkIn = num;
				this._runtimeInfo.NetworkOut = num2;
			}
			else
			{
				bool flag2 = this._networkInLast == -1L && this._networkOutLast == -1L;
				if (flag2)
				{
					this._networkInLast = num;
					this._networkOutLast = num2;
				}
				this._runtimeInfo.NetworkIn = num - this._networkInLast;
				this._runtimeInfo.NetworkOut = num2 - this._networkOutLast;
				this._networkInLast = num;
				this._networkOutLast = num2;
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00012150 File Offset: 0x00010350
		private void UpdateBattery()
		{
			bool flag = AndroidHardwareTrackManager._oApplicationContext == null;
			if (flag)
			{
				AndroidHardwareTrackManager._oApplicationContext = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
			}
			bool flag2 = AndroidHardwareTrackManager._oIntentFilter == null;
			if (flag2)
			{
				AndroidHardwareTrackManager._oIntentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[]
				{
					"android.intent.action.BATTERY_CHANGED"
				});
			}
			AndroidHardwareTrackManager._oBatteryIntent = AndroidHardwareTrackManager._oApplicationContext.Call<AndroidJavaObject>("registerReceiver", new object[]
			{
				null,
				AndroidHardwareTrackManager._oIntentFilter
			});
			int num = AndroidHardwareTrackManager._oBatteryIntent.Call<int>("getIntExtra", new object[]
			{
				"level",
				-1
			});
			int num2 = AndroidHardwareTrackManager._oBatteryIntent.Call<int>("getIntExtra", new object[]
			{
				"scale",
				-1
			});
			this._runtimeInfo.Battery = (float)num * 100f / (float)num2;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00012240 File Offset: 0x00010440
		private int GetNumberOfCores()
		{
			string path = "/sys/devices/system/cpu/possible";
			bool flag = !File.Exists(path);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				string text = File.ReadAllText(path);
				bool flag2 = string.IsNullOrEmpty(text);
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = text == "0";
					if (flag3)
					{
						result = 1;
					}
					else
					{
						string[] array = text.Split(new char[]
						{
							'-'
						});
						int num = 0;
						bool flag4 = !int.TryParse(array[1], out num);
						if (flag4)
						{
							result = -1;
						}
						else
						{
							result = num + 1;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x000122EC File Offset: 0x000104EC
		private void UpdateCpuLoad()
		{
			bool flag = this.cpuNotAccess;
			if (!flag)
			{
				int i = 0;
				while (i < this._runtimeInfo.CpuLoads.Length)
				{
					string path = "/sys/devices/system/cpu/cpu" + i.ToString() + "/cpufreq/scaling_cur_freq";
					try
					{
						bool flag2 = File.Exists(path);
						if (flag2)
						{
							string s = File.ReadAllText(path);
							int num;
							bool flag3 = int.TryParse(s, out num);
							if (flag3)
							{
								this._runtimeInfo.CpuFreqs[i] = num;
								goto IL_B3;
							}
						}
					}
					catch (Exception ex)
					{
						SharedUtils.Log("cpu_freq can not be accessed.");
						this.cpuNotAccess = true;
					}
					goto IL_92;
					IL_B3:
					i++;
					continue;
					IL_92:
					this._runtimeInfo.CpuFreqs[i] = 0;
					this._runtimeInfo.CpuLoads[i] = 0f;
					goto IL_B3;
				}
				bool flag4 = this.cpuNotAccess;
				if (!flag4)
				{
					for (int j = 0; j < this._runtimeInfo.CpuFreqs.Length; j++)
					{
						this._runtimeInfo.CpuLoads[j] = 0f;
					}
					string path2 = "/proc/stat";
					bool flag5 = !File.Exists(path2);
					if (!flag5)
					{
						string[] array = null;
						try
						{
							array = File.ReadAllLines(path2);
						}
						catch (Exception ex2)
						{
							this.cpuNotAccess = true;
							return;
						}
						for (int k = 0; k < array.Length; k++)
						{
							bool flag6 = !array[k].StartsWith("cpu", StringComparison.Ordinal);
							if (!flag6)
							{
								int num2 = -1;
								bool flag7 = !int.TryParse(array[k].Substring(3, 1), out num2);
								if (!flag7)
								{
									bool flag8 = num2 == -1 || num2 >= this._runtimeInfo.CpuLoads.Length;
									if (!flag8)
									{
										float num3 = 0f;
										int num4 = 8;
										int[] array2 = new int[num4];
										string[] array3 = array[k].Split(new char[]
										{
											' '
										});
										array2[1] = int.Parse(array3[1]);
										array2[2] = int.Parse(array3[2]);
										array2[3] = int.Parse(array3[3]);
										array2[4] = int.Parse(array3[4]);
										array2[5] = int.Parse(array3[5]);
										array2[6] = int.Parse(array3[6]);
										array2[7] = int.Parse(array3[7]);
										float[] array4 = new float[num4];
										for (int l = 0; l < num4; l++)
										{
											array4[l] = (float)array2[l] - this._runtimeInfo.LastCpuTicks[num2 * num4 + l];
											num3 += array4[l];
											this._runtimeInfo.LastCpuTicks[num2 * num4 + l] = (float)array2[l];
										}
										this._runtimeInfo.CpuLoads[num2] = array4[1] * 100f / num3;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00012648 File Offset: 0x00010848
		private void UpdateGpuLoad()
		{
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0001264C File Offset: 0x0001084C
		private void UpdateTemperature()
		{
			int num = 0;
			string path = string.Format(this.tempPathFormat, num);
			List<float> list = new List<float>();
			this._runtimeInfo.CpuTemp.Clear();
			while (File.Exists(path))
			{
				this._runtimeInfo.CpuTemp.Add(-1f);
				int index = this._runtimeInfo.CpuTemp.Count - 1;
				try
				{
					string[] array = CoreUtils.ReadAllLinesFromFile(path);
					bool flag = array.Length == 1 && !string.IsNullOrEmpty(array[0]);
					if (flag)
					{
						float num2 = float.Parse(array[0]);
						bool flag2 = num2 < 100f && num2 > 0f;
						if (flag2)
						{
							list.Add(num2);
						}
						bool flag3 = num2 < 1000f && num2 > 100f;
						if (flag3)
						{
							list.Add(num2 / 10f);
						}
						bool flag4 = num2 > 1000f;
						if (flag4)
						{
							list.Add(num2 / 1000f);
						}
						this._runtimeInfo.CpuTemp[index] = num2;
					}
				}
				catch (Exception)
				{
				}
				num++;
				path = string.Format(this.tempPathFormat, num);
			}
			bool flag5 = num == 0;
			if (flag5)
			{
				this._runtimeInfo.CpuTemp.Clear();
				path = string.Format(this.tempPathFormat2, num);
				while (File.Exists(path))
				{
					this._runtimeInfo.CpuTemp.Add(-1f);
					int index2 = this._runtimeInfo.CpuTemp.Count - 1;
					try
					{
						string[] array2 = CoreUtils.ReadAllLinesFromFile(path);
						bool flag6 = array2.Length == 1 && !string.IsNullOrEmpty(array2[0]);
						if (flag6)
						{
							float num3 = float.Parse(array2[0]);
							bool flag7 = num3 < 100f && num3 > 0f;
							if (flag7)
							{
								list.Add(num3);
							}
							bool flag8 = num3 < 1000f && num3 > 100f;
							if (flag8)
							{
								list.Add(num3 / 10f);
							}
							bool flag9 = num3 > 1000f;
							if (flag9)
							{
								list.Add(num3 / 1000f);
							}
							this._runtimeInfo.CpuTemp[index2] = num3;
						}
					}
					catch (Exception)
					{
					}
					num++;
					path = string.Format(this.tempPathFormat, num);
				}
			}
			bool flag10 = num != 0 && list.Count != 0;
			if (flag10)
			{
				float num4 = 0f;
				for (int i = 0; i < list.Count; i++)
				{
					num4 += list[i];
				}
				this._runtimeInfo.Temperature = num4 / (float)list.Count;
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000129B4 File Offset: 0x00010BB4
		public static long GetRuntimeRomSpace()
		{
			AndroidHardwareTrackManager.RefreshRomMemory();
			bool flag = HardwareTrackManager.HardwareInfo.RomMemory != null;
			long result;
			if (flag)
			{
				result = HardwareTrackManager.HardwareInfo.RomMemory[0];
			}
			else
			{
				result = -1L;
			}
			return result;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x000129FC File Offset: 0x00010BFC
		private static void RefreshRomMemory()
		{
			object[] array = new object[]
			{
				new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getDataDirectory", new object[0]).Call<string>("getPath", new object[0])
			};
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.StatFs", array);
			try
			{
				long num = androidJavaObject.Call<long>("getTotalBytes", new object[0]);
				long num2 = androidJavaObject.Call<long>("getAvailableBytes", new object[0]);
				HardwareTrackManager.HardwareInfo.RomMemory = new long[]
				{
					num2,
					num
				};
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00012AA8 File Offset: 0x00010CA8
		public static void StaticInit()
		{
			bool staticInited = AndroidHardwareTrackManager._staticInited;
			if (!staticInited)
			{
				try
				{
					AndroidHardwareTrackManager.Runtime = new AndroidJavaClass("java.lang.Runtime").CallStatic<AndroidJavaObject>("getRuntime", new object[0]);
					AndroidHardwareTrackManager._clsProcess = new AndroidJavaClass("android.os.Process");
					AndroidHardwareTrackManager._clsTrafficStats = new AndroidJavaClass("android.net.TrafficStats");
					string @static = new AndroidJavaClass("android.content.Context").GetStatic<string>("ACTIVITY_SERVICE");
					object[] array = new object[]
					{
						@static
					};
					AndroidHardwareTrackManager._oActivityManager = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getSystemService", array);
					AndroidHardwareTrackManager.Pid = AndroidHardwareTrackManager._clsProcess.CallStatic<int>("myPid", new object[0]);
					AndroidHardwareTrackManager._uid = AndroidHardwareTrackManager._clsProcess.CallStatic<int>("myUid", new object[0]);
					bool showLog = SharedUtils.ShowLog;
					if (showLog)
					{
						SharedUtils.Log("AndroidHardwareTrackManager StaticInit - pid = " + AndroidHardwareTrackManager.Pid.ToString() + " uid = " + AndroidHardwareTrackManager._uid.ToString());
					}
					HardwareTrackManager.AppInfo = new SystyemInfoBuilder.AppInfo();
					HardwareTrackManager.HardwareInfo = new SystyemInfoBuilder.HardwareInfo();
					object[] array2 = new object[]
					{
						SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0])
					};
					HardwareTrackManager.AppInfo.ApplicationName = SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getApplicationInfo", new object[0]).Call<string>("loadLabel", array2);
					HardwareTrackManager.AppInfo.PackageName = SharedUtils.CurrentActivity.Call<string>("getPackageName", new object[0]);
					bool flag = HardwareTrackManager.AppInfo.PackageName.Length >= 50;
					if (flag)
					{
						HardwareTrackManager.AppInfo.PackageName = HardwareTrackManager.AppInfo.PackageName.Substring(0, 50);
					}
					AndroidHardwareTrackManager.RefreshRomMemory();
					UWACoreConfig.APP_NAME = HardwareTrackManager.AppInfo.ApplicationName;
					UWACoreConfig.PKG_NAME = HardwareTrackManager.AppInfo.PackageName;
				}
				catch (Exception ex)
				{
					string str = "Get package / application name failed ! : ";
					Exception ex2 = ex;
					SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				AndroidHardwareTrackManager._staticInited = true;
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00012CDC File Offset: 0x00010EDC
		public static void ApplyRoot()
		{
			AndroidHardwareTrackManager.Runtime.Call<AndroidJavaObject>("exec", new object[]
			{
				"su"
			});
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00012D00 File Offset: 0x00010F00
		public static int[] GetScreen()
		{
			AndroidJavaClass javaClass = DexLoader.Instance.GetJavaClass("com.uwa.uwascreen.ScreenTool");
			return javaClass.CallStatic<int[]>("GetScreen", new object[]
			{
				SharedUtils.CurrentActivity
			});
		}

		// Token: 0x0400017A RID: 378
		private bool cpuNotAccess = false;

		// Token: 0x0400017B RID: 379
		private string tempPathFormat = "/sys/class/thermal/thermal_zone{0}/temp";

		// Token: 0x0400017C RID: 380
		private string tempPathFormat2 = "/sys/devices/virtual/thermal/thermal_zone{0}/temp";

		// Token: 0x0400017D RID: 381
		public static AndroidJavaObject Runtime;

		// Token: 0x0400017E RID: 382
		private static AndroidJavaClass _clsIntentFilter;

		// Token: 0x0400017F RID: 383
		private static AndroidJavaClass _clsProcess;

		// Token: 0x04000180 RID: 384
		private static AndroidJavaClass _clsTrafficStats;

		// Token: 0x04000181 RID: 385
		private static AndroidJavaObject _oBatteryIntent;

		// Token: 0x04000182 RID: 386
		private static AndroidJavaObject _oApplicationContext;

		// Token: 0x04000183 RID: 387
		private static AndroidJavaObject _oIntentFilter;

		// Token: 0x04000184 RID: 388
		private static AndroidJavaObject _oActivityManager;

		// Token: 0x04000185 RID: 389
		private static object[] _param;

		// Token: 0x04000186 RID: 390
		public static int Pid;

		// Token: 0x04000187 RID: 391
		private static int _uid;

		// Token: 0x04000188 RID: 392
		private static bool _staticInited;

		// Token: 0x020000FB RID: 251
		protected enum RenderEvent
		{
			// Token: 0x04000644 RID: 1604
			StartTiming = 115,
			// Token: 0x04000645 RID: 1605
			StopTiming
		}

		// Token: 0x020000FC RID: 252
		private enum LinuxProcStatVariables
		{
			// Token: 0x04000647 RID: 1607
			lpsv_cpuid,
			// Token: 0x04000648 RID: 1608
			lpsv_user,
			// Token: 0x04000649 RID: 1609
			lpsv_nice,
			// Token: 0x0400064A RID: 1610
			lpsv_system,
			// Token: 0x0400064B RID: 1611
			lpsv_idle,
			// Token: 0x0400064C RID: 1612
			lpsv_iowait,
			// Token: 0x0400064D RID: 1613
			lpsv_irq,
			// Token: 0x0400064E RID: 1614
			lpsv_softirq,
			// Token: 0x0400064F RID: 1615
			num_lpsvs
		}
	}
}
