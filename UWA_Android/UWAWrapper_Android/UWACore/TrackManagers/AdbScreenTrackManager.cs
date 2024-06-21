using System;
using System.IO;
using UnityEngine;
using UWA;
using UWACore.Util.AndroidScreenShot;

namespace UWACore.TrackManagers
{
	// Token: 0x02000040 RID: 64
	internal class AdbScreenTrackManager : ScreenTrackManager
	{
		// Token: 0x060002D0 RID: 720 RVA: 0x0001394C File Offset: 0x00011B4C
		public override bool IsReady(out string error)
		{
			error = null;
			bool flag = !AdbScreenShot.ScreencapConnect(ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Video);
			bool result;
			if (flag)
			{
				error = "ADB Screenshot not work.";
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00013990 File Offset: 0x00011B90
		public override void StartTrack()
		{
			this.Mark();
			AdbScreenShot.ScreencapConnect(ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Video);
			bool flag = base.Enabled && ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Video;
			if (flag)
			{
				AdbScreenShot.ScreenrecordCMDToSend(true);
			}
			bool flag2 = base.Enabled && ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Image;
			if (flag2)
			{
				AdbScreenShot.ScreencapCMDToSend(true);
			}
			base.StartTrack();
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00013A0C File Offset: 0x00011C0C
		public override void StopTrack()
		{
			base.StopTrack();
			bool flag = base.Enabled && ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Video;
			if (flag)
			{
				AdbScreenShot.ScreenrecordCMDToSend(false);
			}
			bool flag2 = base.Enabled && ScreenTrackManager.TrackMode == ScreenTrackManager.ETrackMode.Image;
			if (flag2)
			{
				AdbScreenShot.ScreencapCMDToSend(false);
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00013A74 File Offset: 0x00011C74
		protected override void DoUpdateAtEnd()
		{
			this.currMarkWait += Time.unscaledDeltaTime;
			bool flag = this.currMarkWait > 1f;
			if (flag)
			{
				this.Mark();
				this.currMarkWait -= 1f;
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00013AC8 File Offset: 0x00011CC8
		private void Mark()
		{
			File.WriteAllText(SharedUtils.FinalDataPath + "/ss", this.markValue.ToString());
			this.markValue++;
		}

		// Token: 0x0400019A RID: 410
		private float currMarkWait = 0f;

		// Token: 0x0400019B RID: 411
		private int markValue = 0;
	}
}
