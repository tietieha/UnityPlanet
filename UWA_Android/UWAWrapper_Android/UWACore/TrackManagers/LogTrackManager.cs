using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003B RID: 59
	internal class LogTrackManager : BaseTrackerManager
	{
		// Token: 0x060002A7 RID: 679 RVA: 0x00012D44 File Offset: 0x00010F44
		public LogTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00012D64 File Offset: 0x00010F64
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			LogTrackManager._logFilter.Clear();
			bool flag = config.Count != 0;
			if (flag)
			{
				this._outputStackTrace = bool.Parse(config["outputStackTrace"]);
				bool flag2 = !bool.Parse(config["Error"]);
				if (flag2)
				{
					LogTrackManager._logFilter.Add(0);
				}
				bool flag3 = !bool.Parse(config["Assert"]);
				if (flag3)
				{
					LogTrackManager._logFilter.Add(1);
				}
				bool flag4 = !bool.Parse(config["Warning"]);
				if (flag4)
				{
					LogTrackManager._logFilter.Add(2);
				}
				bool flag5 = !bool.Parse(config["Log"]);
				if (flag5)
				{
					LogTrackManager._logFilter.Add(3);
				}
				bool flag6 = !bool.Parse(config["Exception"]);
				if (flag6)
				{
					LogTrackManager._logFilter.Add(4);
				}
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00012E78 File Offset: 0x00011078
		public override void StartTrack()
		{
			base.StartTrack();
			bool flag = base.Enabled && !SharedUtils.Log2File;
			if (flag)
			{
				Application.RegisterLogCallback(new Application.LogCallback(this.LogCallback));
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00012EC8 File Offset: 0x000110C8
		public override void StopTrack()
		{
			base.StopTrack();
			Application.RegisterLogCallback(null);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00012EDC File Offset: 0x000110DC
		private void LogCallback(string logString, string stackTrace, LogType type)
		{
			bool flag = !base.Enabled && LogTrackManager._logFilter.Contains(type);
			if (!flag)
			{
				base.TrackWriter.WriteToBuffer((this._outputStackTrace || type == null || type == 4) ? string.Format("{0},{1},{2},{3}", new object[]
				{
					SharedUtils.frameId,
					type,
					CoreUtils.StringReplace(logString),
					CoreUtils.StringReplace(stackTrace)
				}) : string.Format("{0},{1},{2},", SharedUtils.frameId, type, CoreUtils.StringReplace(logString)));
			}
		}

		// Token: 0x04000189 RID: 393
		private static List<LogType> _logFilter = new List<LogType>
		{
			3,
			2
		};

		// Token: 0x0400018A RID: 394
		private int lastFrame = -1;

		// Token: 0x0400018B RID: 395
		private bool _outputStackTrace = false;
	}
}
