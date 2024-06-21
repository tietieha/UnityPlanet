using System;
using System.Collections.Generic;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x02000041 RID: 65
	internal class TimeTrackManager : BaseTrackerManager
	{
		// Token: 0x060002D6 RID: 726 RVA: 0x00013B18 File Offset: 0x00011D18
		public TimeTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00013B30 File Offset: 0x00011D30
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00013B34 File Offset: 0x00011D34
		public void ResetStartTime(long time)
		{
			this._timeToReset = time;
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x00013B40 File Offset: 0x00011D40
		public static long Duration
		{
			get
			{
				return (long)(DateTime.Now - TimeTrackManager._startTime).TotalMilliseconds;
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00013B74 File Offset: 0x00011D74
		public override void StartTrack()
		{
			this.TryReset();
			base.StartTrack();
			bool flag = !base.Enabled;
			if (!flag)
			{
				CoreUtils.WriteLastTime(TimeTrackManager.Duration);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00013BB4 File Offset: 0x00011DB4
		protected override void DoUpdateAtEnd()
		{
			bool flag = SharedUtils.frameId % 60 == 0;
			if (flag)
			{
				CoreUtils.WriteLastTime(TimeTrackManager.Duration);
			}
			this.TryReset();
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1}", SharedUtils.frameId, (long)(DateTime.Now - TimeTrackManager._startTime).TotalMilliseconds));
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060002DC RID: 732 RVA: 0x00013C2C File Offset: 0x00011E2C
		// (set) Token: 0x060002DD RID: 733 RVA: 0x00013C4C File Offset: 0x00011E4C
		protected override string LogFile
		{
			get
			{
				return base.LogFile;
			}
			set
			{
				base.LogFile = value;
				bool flag = value.Equals("");
				if (flag)
				{
					CoreUtils.WriteLastTime(TimeTrackManager.Duration);
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00013C88 File Offset: 0x00011E88
		private void TryReset()
		{
			bool flag = this._timeToReset != -1L;
			if (flag)
			{
				TimeTrackManager._startTime = DateTime.Now - TimeSpan.FromMilliseconds((double)this._timeToReset);
				this._timeToReset = -1L;
			}
		}

		// Token: 0x0400019C RID: 412
		private string _lastTimeFile;

		// Token: 0x0400019D RID: 413
		private long _timeToReset = -1L;

		// Token: 0x0400019E RID: 414
		private static DateTime _startTime;
	}
}
