using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x02000042 RID: 66
	internal abstract class BaseTrackerManager
	{
		// Token: 0x060002DF RID: 735
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Dump(int mode);

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00013CD4 File Offset: 0x00011ED4
		public string Name
		{
			get
			{
				bool flag = this._name == null;
				if (flag)
				{
					this._name = base.GetType().Name;
				}
				return this._name;
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00013D18 File Offset: 0x00011F18
		public static void PrepareAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.Prepare();
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00013D5C File Offset: 0x00011F5C
		public static void StartTrackAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.StartTrack();
			}
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00013DA0 File Offset: 0x00011FA0
		public static void StopTrackAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.StopTrack();
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00013DE4 File Offset: 0x00011FE4
		public static void ClearAll()
		{
			BaseTrackerManager.AllTrackManager.Clear();
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00013DF4 File Offset: 0x00011FF4
		public static void CheckTimeAndSwitchLogFileAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.CheckTimeAndSwitchLogFile();
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00013E38 File Offset: 0x00012038
		public static void UpdateAtEndAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.UpdateAtEnd();
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00013E7C File Offset: 0x0001207C
		public static void LateUpdateAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.LateUpdate();
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00013EC0 File Offset: 0x000120C0
		public static void SwitchLogFileAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.SwitchLogFile(false);
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00013F04 File Offset: 0x00012104
		public static bool IsAllReady(out string errors)
		{
			errors = null;
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				string value = null;
				bool flag2 = !baseTrackerManager.IsReady(out value);
				if (flag2)
				{
					stringBuilder.AppendLine(value);
					flag = false;
				}
			}
			bool flag3 = !flag;
			if (flag3)
			{
				errors = stringBuilder.ToString();
			}
			return flag;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00013F90 File Offset: 0x00012190
		protected BaseTrackerManager(string extension, int bufferSize = 200)
		{
			this._ext = extension;
			this._trackTimeInterval = 2.1474836E+09f;
			this._trackFrameInterval = 2.1474836E+09f;
			this._trackTimeSum = 0f;
			this._trackFrameSum = 0f;
			this.Inited = false;
			this.Enabled = false;
			this.Tracking = false;
			this.TrackWriter = (this._ext.Equals("") ? null : new TrackWriter<string>(bufferSize));
			BaseTrackerManager.AllTrackManager.Add(this);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00014030 File Offset: 0x00012230
		public void Init(Dictionary<string, string> config)
		{
			bool inited = this.Inited;
			if (!inited)
			{
				bool flag = config.ContainsKey("Enable");
				if (flag)
				{
					this.Enabled = config["Enable"].Equals("true", StringComparison.OrdinalIgnoreCase);
				}
				bool enabled = this.Enabled;
				if (enabled)
				{
					bool flag2 = config.ContainsKey("TimeInterval");
					if (flag2)
					{
						this._trackTimeInterval = float.Parse(config["TimeInterval"]);
						this._trackFrameInterval = this._trackTimeInterval * 60f;
					}
					this.InitWithConfig(config);
				}
				this.Inited = true;
			}
		}

		// Token: 0x060002EC RID: 748
		protected abstract void InitWithConfig(Dictionary<string, string> config);

		// Token: 0x060002ED RID: 749 RVA: 0x000140E4 File Offset: 0x000122E4
		protected virtual void Prepare()
		{
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000140E8 File Offset: 0x000122E8
		public virtual void StartTrack()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool tracking = this.Tracking;
				if (!tracking)
				{
					this.Tracking = true;
					bool flag2 = string.IsNullOrEmpty(this.LogFile);
					if (flag2)
					{
						this.LogFile = this.GetTargetLogName();
					}
				}
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0001414C File Offset: 0x0001234C
		public virtual void StopTrack()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool flag2 = !this.Tracking;
				if (!flag2)
				{
					this.SwitchLogFile(true);
					this.Tracking = false;
					this.Inited = false;
				}
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x000141A0 File Offset: 0x000123A0
		protected virtual string GetTargetLogName()
		{
			return CoreUtils.GetFrameIdWithExtFilePath(this._ext);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x000141C4 File Offset: 0x000123C4
		public void UpdateAtEnd()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool flag2 = !this.Tracking;
				if (!flag2)
				{
					this.DoUpdateAtEnd();
				}
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00014208 File Offset: 0x00012408
		protected virtual void DoUpdateAtEnd()
		{
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0001420C File Offset: 0x0001240C
		public void LateUpdate()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool flag2 = !this.Tracking;
				if (!flag2)
				{
					this.DoLateUpdate();
				}
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00014250 File Offset: 0x00012450
		protected virtual void DoLateUpdate()
		{
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00014254 File Offset: 0x00012454
		public void CheckTimeAndSwitchLogFile()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool flag2 = !this.Tracking;
				if (!flag2)
				{
					this._trackTimeSum += Time.unscaledDeltaTime;
					this._trackFrameSum += 1f;
					bool useFrameInterval = this.UseFrameInterval;
					if (useFrameInterval)
					{
						bool flag3 = this._trackFrameSum > this._trackFrameInterval;
						if (flag3)
						{
							this.SwitchLogFile(false);
							this._trackFrameSum = 0f;
						}
					}
					else
					{
						bool flag4 = this._trackTimeSum > this._trackTimeInterval;
						if (flag4)
						{
							this.SwitchLogFile(false);
							this._trackTimeSum = 0f;
						}
					}
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00014320 File Offset: 0x00012520
		public virtual void SwitchLogFile(bool end = false)
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				bool flag2 = !this.Tracking;
				if (!flag2)
				{
					bool flag3 = string.IsNullOrEmpty(this.LogFile);
					if (!flag3)
					{
						if (end)
						{
							this.LogFile = "";
						}
					}
				}
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00014388 File Offset: 0x00012588
		public virtual int GetLogFileCount()
		{
			string[] files = Directory.GetFiles(SharedUtils.FinalDataPath, "*" + this._ext);
			return files.Length;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x000143C0 File Offset: 0x000125C0
		public virtual long GetLogFileSize()
		{
			string[] files = Directory.GetFiles(SharedUtils.FinalDataPath, "*" + this._ext);
			long num = 0L;
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = new FileInfo(files[i]);
				num += fileInfo.Length;
			}
			return num;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0001442C File Offset: 0x0001262C
		public virtual bool IsReady(out string error)
		{
			error = null;
			return true;
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002FA RID: 762 RVA: 0x0001444C File Offset: 0x0001264C
		// (set) Token: 0x060002FB RID: 763 RVA: 0x0001449C File Offset: 0x0001269C
		protected virtual string LogFile
		{
			get
			{
				return (this.Enabled && this.Tracking && this.TrackWriter != null) ? this.TrackWriter.LogPath : "";
			}
			set
			{
				bool flag = this.Enabled && this.Tracking && this.TrackWriter != null;
				if (flag)
				{
					this.TrackWriter.LogPath = value;
				}
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060002FC RID: 764 RVA: 0x000144EC File Offset: 0x000126EC
		// (set) Token: 0x060002FD RID: 765 RVA: 0x000144F4 File Offset: 0x000126F4
		public bool Enabled { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002FE RID: 766 RVA: 0x00014500 File Offset: 0x00012700
		// (set) Token: 0x060002FF RID: 767 RVA: 0x00014508 File Offset: 0x00012708
		public bool Tracking { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00014514 File Offset: 0x00012714
		// (set) Token: 0x06000301 RID: 769 RVA: 0x0001451C File Offset: 0x0001271C
		protected bool UseFrameInterval { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00014528 File Offset: 0x00012728
		// (set) Token: 0x06000303 RID: 771 RVA: 0x00014530 File Offset: 0x00012730
		private protected TrackWriter<string> TrackWriter { protected get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0001453C File Offset: 0x0001273C
		// (set) Token: 0x06000305 RID: 773 RVA: 0x00014544 File Offset: 0x00012744
		private protected bool Inited { protected get; private set; }

		// Token: 0x0400019F RID: 415
		private string _name = null;

		// Token: 0x040001A0 RID: 416
		private static readonly List<BaseTrackerManager> AllTrackManager = new List<BaseTrackerManager>();

		// Token: 0x040001A6 RID: 422
		private string _ext;

		// Token: 0x040001A7 RID: 423
		protected float _trackTimeInterval;

		// Token: 0x040001A8 RID: 424
		private float _trackTimeSum;

		// Token: 0x040001A9 RID: 425
		protected float _trackFrameInterval;

		// Token: 0x040001AA RID: 426
		private float _trackFrameSum;
	}
}
