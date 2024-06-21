using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UWA;
using UWACore.Util;

namespace UWACore.TrackManagers
{
	// Token: 0x02000043 RID: 67
	internal class DataTrackManager : BaseTrackerManager
	{
		// Token: 0x06000307 RID: 775 RVA: 0x0001455C File Offset: 0x0001275C
		public DataTrackManager() : base("", 200)
		{
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00014570 File Offset: 0x00012770
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			base.UseFrameInterval = true;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001457C File Offset: 0x0001277C
		public override void StartTrack()
		{
			base.StartTrack();
			bool enabled = base.Enabled;
			if (enabled)
			{
				Profiler.enabled = true;
				Profiler.enableBinaryLog = true;
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000145B4 File Offset: 0x000127B4
		public override void StopTrack()
		{
			Profiler.enabled = false;
			Profiler.enableBinaryLog = false;
			base.StopTrack();
		}

		// Token: 0x0600030B RID: 779 RVA: 0x000145CC File Offset: 0x000127CC
		protected override void DoUpdateAtEnd()
		{
			bool flag = !Profiler.enabled;
			if (flag)
			{
				Profiler.enabled = true;
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x000145F8 File Offset: 0x000127F8
		public override void SwitchLogFile(bool end = false)
		{
			bool flag = !base.Enabled;
			if (!flag)
			{
				bool flag2 = !base.Tracking;
				if (!flag2)
				{
					bool flag3 = string.IsNullOrEmpty(this.LogFile);
					if (!flag3)
					{
						this.LogFile = "";
						bool flag4 = !end;
						if (flag4)
						{
							this.LogFile = this.GetTargetLogName();
						}
						CoreUtils.WriteEndingFlag(end);
					}
				}
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600030D RID: 781 RVA: 0x00014678 File Offset: 0x00012878
		// (set) Token: 0x0600030E RID: 782 RVA: 0x000146AC File Offset: 0x000128AC
		protected override string LogFile
		{
			get
			{
				return base.Enabled ? Profiler.logFile : "";
			}
			set
			{
				bool flag = !base.Enabled;
				if (!flag)
				{
					bool flag2 = !string.IsNullOrEmpty(Profiler.logFile);
					if (flag2)
					{
						UProfiler.EndRecord();
					}
					bool flag3 = !string.IsNullOrEmpty(value);
					if (flag3)
					{
						UProfiler.BeginRecord(value);
					}
				}
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001470C File Offset: 0x0001290C
		protected override string GetTargetLogName()
		{
			return CoreUtils.GetLogFileFullPath(CoreUtils.GetDataFileName());
		}
	}
}
