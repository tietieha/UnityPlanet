using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x02000039 RID: 57
	internal abstract class HardwareTrackManager : BaseTrackerManager
	{
		// Token: 0x06000289 RID: 649 RVA: 0x000116F4 File Offset: 0x0000F8F4
		protected HardwareTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600028A RID: 650 RVA: 0x00011714 File Offset: 0x0000F914
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0001171C File Offset: 0x0000F91C
		public static SystyemInfoBuilder.AppInfo AppInfo { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600028C RID: 652 RVA: 0x00011724 File Offset: 0x0000F924
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0001172C File Offset: 0x0000F92C
		public static SystyemInfoBuilder.HardwareInfo HardwareInfo { get; set; }

		// Token: 0x0600028E RID: 654 RVA: 0x00011734 File Offset: 0x0000F934
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			this._runtimeInfo = new HardwareTrackManager.RuntimeInfo
			{
				PssInfo = Vector4.zero,
				Battery = -1f,
				NetworkOut = -1L,
				NetworkIn = -1L,
				Temperature = -1f,
				CpuTemp = new List<float>()
			};
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00011790 File Offset: 0x0000F990
		protected override void DoUpdateAtEnd()
		{
			this.CheckTimeAndDoTask();
			this.CheckInfoToWrite();
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000117A4 File Offset: 0x0000F9A4
		private void CheckTimeAndDoTask()
		{
			for (int i = 0; i < HardwareTrackManager.Tasks.Count; i++)
			{
				HardwareTrackManager.HardwareTrackTask hardwareTrackTask = HardwareTrackManager.Tasks[i];
				hardwareTrackTask.trackCurrent += Time.unscaledDeltaTime;
				bool flag = hardwareTrackTask.trackCurrent > hardwareTrackTask.trackInternal;
				if (flag)
				{
					hardwareTrackTask.trackCurrent = 0f;
					hardwareTrackTask.takenFrame = SharedUtils.frameId;
					bool useRunnable = hardwareTrackTask.useRunnable;
					if (useRunnable)
					{
						this.AsyncCall(hardwareTrackTask.worker);
					}
					else
					{
						hardwareTrackTask.worker();
					}
				}
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00011854 File Offset: 0x0000FA54
		protected virtual void AsyncCall(HardwareTrackManager.WorkCall call)
		{
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00011858 File Offset: 0x0000FA58
		private void CheckInfoToWrite()
		{
			for (int i = 0; i < HardwareTrackManager.Tasks.Count; i++)
			{
				HardwareTrackManager.HardwareTrackTask hardwareTrackTask = HardwareTrackManager.Tasks[i];
				switch (hardwareTrackTask.type)
				{
				case HardwareTrackManager.HardwareTrackTask.TaskType.Network:
				{
					bool flag = hardwareTrackTask.takenFrame != -1;
					if (flag)
					{
						base.TrackWriter.WriteToBuffer(string.Format("{0},n,{1},{2}", hardwareTrackTask.takenFrame, this._runtimeInfo.NetworkIn, this._runtimeInfo.NetworkOut));
						hardwareTrackTask.takenFrame = -1;
					}
					break;
				}
				case HardwareTrackManager.HardwareTrackTask.TaskType.Pss:
				{
					bool flag2 = hardwareTrackTask.takenFrame != -1 && this._runtimeInfo.PssInfo != Vector4.zero;
					if (flag2)
					{
						base.TrackWriter.WriteToBuffer(string.Format("{0},p,{1},{2},{3},{4}", new object[]
						{
							hardwareTrackTask.takenFrame,
							this._runtimeInfo.PssInfo.x,
							this._runtimeInfo.PssInfo.y,
							this._runtimeInfo.PssInfo.z,
							this._runtimeInfo.PssInfo.w
						}));
						hardwareTrackTask.takenFrame = -1;
					}
					break;
				}
				case HardwareTrackManager.HardwareTrackTask.TaskType.Battery:
				{
					bool flag3 = hardwareTrackTask.takenFrame != -1;
					if (flag3)
					{
						base.TrackWriter.WriteToBuffer(string.Format("{0},b,{1:F4}", hardwareTrackTask.takenFrame, this._runtimeInfo.Battery));
						hardwareTrackTask.takenFrame = -1;
					}
					break;
				}
				case HardwareTrackManager.HardwareTrackTask.TaskType.Temperature:
				{
					bool flag4 = hardwareTrackTask.takenFrame != -1;
					if (flag4)
					{
						base.TrackWriter.WriteToBuffer(string.Format("{0},t,{1:F}", hardwareTrackTask.takenFrame, this._runtimeInfo.Temperature));
						hardwareTrackTask.takenFrame = -1;
					}
					break;
				}
				case HardwareTrackManager.HardwareTrackTask.TaskType.CpuLoad:
				{
					bool flag5 = hardwareTrackTask.takenFrame != -1;
					if (flag5)
					{
						StringBuilder stringBuilder = new StringBuilder(string.Format("{0},f,{1}", hardwareTrackTask.takenFrame, this._runtimeInfo.CpuFreqs.Length));
						for (int j = 0; j < this._runtimeInfo.CpuFreqs.Length; j++)
						{
							stringBuilder.Append(",");
							stringBuilder.Append(this._runtimeInfo.CpuFreqs[j]);
						}
						base.TrackWriter.WriteToBuffer(stringBuilder.ToString());
						StringBuilder stringBuilder2 = new StringBuilder(string.Format("{0},u,{1}", hardwareTrackTask.takenFrame, this._runtimeInfo.CpuLoads.Length));
						for (int k = 0; k < this._runtimeInfo.CpuLoads.Length; k++)
						{
							stringBuilder2.Append(",");
							stringBuilder2.Append(this._runtimeInfo.CpuLoads[k]);
						}
						base.TrackWriter.WriteToBuffer(stringBuilder2.ToString());
						hardwareTrackTask.takenFrame = -1;
					}
					break;
				}
				}
			}
		}

		// Token: 0x04000174 RID: 372
		protected long _networkInLast = -1L;

		// Token: 0x04000175 RID: 373
		protected long _networkOutLast = -1L;

		// Token: 0x04000176 RID: 374
		protected HardwareTrackManager.RuntimeInfo _runtimeInfo;

		// Token: 0x04000179 RID: 377
		protected static List<HardwareTrackManager.HardwareTrackTask> Tasks = new List<HardwareTrackManager.HardwareTrackTask>();

		// Token: 0x020000F8 RID: 248
		// (Invoke) Token: 0x06000932 RID: 2354
		public delegate void WorkCall();

		// Token: 0x020000F9 RID: 249
		protected class HardwareTrackTask
		{
			// Token: 0x06000935 RID: 2357 RVA: 0x0003CA30 File Offset: 0x0003AC30
			public HardwareTrackTask(HardwareTrackManager.HardwareTrackTask.TaskType t, float f, HardwareTrackManager.WorkCall callback, bool useRunnable)
			{
				this.type = t;
				this.trackInternal = f;
				this.worker = callback;
				this.trackCurrent = 0f;
				this.takenFrame = -1;
				this.useRunnable = useRunnable;
			}

			// Token: 0x04000633 RID: 1587
			public HardwareTrackManager.HardwareTrackTask.TaskType type;

			// Token: 0x04000634 RID: 1588
			public HardwareTrackManager.WorkCall worker;

			// Token: 0x04000635 RID: 1589
			public int takenFrame;

			// Token: 0x04000636 RID: 1590
			public float trackInternal;

			// Token: 0x04000637 RID: 1591
			public float trackCurrent;

			// Token: 0x04000638 RID: 1592
			public bool useRunnable;

			// Token: 0x0200013B RID: 315
			public enum TaskType
			{
				// Token: 0x04000744 RID: 1860
				Network,
				// Token: 0x04000745 RID: 1861
				Pss,
				// Token: 0x04000746 RID: 1862
				Battery,
				// Token: 0x04000747 RID: 1863
				Temperature,
				// Token: 0x04000748 RID: 1864
				CpuLoad,
				// Token: 0x04000749 RID: 1865
				GpuLoad
			}
		}

		// Token: 0x020000FA RID: 250
		public class RuntimeInfo
		{
			// Token: 0x04000639 RID: 1593
			public long NetworkIn;

			// Token: 0x0400063A RID: 1594
			public long NetworkOut;

			// Token: 0x0400063B RID: 1595
			public Vector4 PssInfo;

			// Token: 0x0400063C RID: 1596
			public float Battery;

			// Token: 0x0400063D RID: 1597
			public float Temperature;

			// Token: 0x0400063E RID: 1598
			public List<float> CpuTemp;

			// Token: 0x0400063F RID: 1599
			public float GpuLoad;

			// Token: 0x04000640 RID: 1600
			public float[] CpuLoads;

			// Token: 0x04000641 RID: 1601
			public int[] CpuFreqs;

			// Token: 0x04000642 RID: 1602
			public float[] LastCpuTicks;
		}
	}
}
