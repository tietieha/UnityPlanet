using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;

namespace UWALocal
{
	// Token: 0x0200000F RID: 15
	internal class LogTrackManager : BaseTrackerManager
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00005B64 File Offset: 0x00003D64
		public LogTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005B7C File Offset: 0x00003D7C
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			LogTrackManager._logFilter.Clear();
			this._outputStackTrace = false;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005B90 File Offset: 0x00003D90
		public override void StartTrack()
		{
			base.StartTrack();
			if (base.Enabled && !SharedUtils.Log2File)
			{
				Application.RegisterLogCallback(new Application.LogCallback(this.LogCallback));
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00005BC0 File Offset: 0x00003DC0
		public override void StopTrack()
		{
			base.StopTrack();
			Application.RegisterLogCallback(null);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005BD0 File Offset: 0x00003DD0
		private void LogCallback(string logString, string stackTrace, LogType type)
		{
			if (!base.Enabled && LogTrackManager._logFilter.Contains(type))
			{
				return;
			}
			base.TrackWriter.WriteToBuffer((this._outputStackTrace || type == null || type == 4) ? string.Format("{0},{1},{2},{3}", new object[]
			{
				SharedUtils.frameId,
				type,
				CoreUtils.StringReplace(logString),
				CoreUtils.StringReplace(stackTrace)
			}) : string.Format("{0},{1},{2},", SharedUtils.frameId, type, CoreUtils.StringReplace(logString)));
		}

		// Token: 0x0400004A RID: 74
		private static List<LogType> _logFilter = new List<LogType>
		{
			3,
			2
		};

		// Token: 0x0400004B RID: 75
		private int lastFrame = -1;

		// Token: 0x0400004C RID: 76
		private bool _outputStackTrace;
	}
}
