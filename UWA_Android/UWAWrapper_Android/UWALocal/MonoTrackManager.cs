using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;
using UWA.Android;

namespace UWALocal
{
	// Token: 0x0200001F RID: 31
	internal class MonoTrackManager : BaseTrackerManager, IUWAAPI
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x0000B738 File Offset: 0x00009938
		public MonoTrackManager(UwaProfiler.Mode mode, string extension) : base(extension, 200)
		{
			this._trackMode = mode;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000B750 File Offset: 0x00009950
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000B754 File Offset: 0x00009954
		public override void StartTrack()
		{
			if (base.Enabled)
			{
				UWAEngine.Set(this);
				UwaProfiler.Setup(SharedUtils.FinalDataPath, this._trackMode);
				UwaProfiler.Start();
			}
			base.StartTrack();
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000B784 File Offset: 0x00009984
		public override void StopTrack()
		{
			base.StopTrack();
			if (base.Enabled)
			{
				UwaProfiler.Stop();
				UWAEngine.Set(null);
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000B7A4 File Offset: 0x000099A4
		protected override void DoUpdateAtEnd()
		{
			UwaProfiler.UpdateAtEnd();
			if (MonoTrackManager.MonoDumpHelper.UpdateDump())
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("Mono Dump");
				}
				BaseTrackerManager.Dump(1);
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000B7D4 File Offset: 0x000099D4
		public void Register(Type classType, string fieldName, float updateInterval)
		{
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000B7D8 File Offset: 0x000099D8
		public void Register(object classObj, string instanceName, string fieldName, float updateInterval)
		{
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000B7DC File Offset: 0x000099DC
		public void Tag(string tag)
		{
			UwaLocalStarter.ChangeTag(tag);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000B7E4 File Offset: 0x000099E4
		public void PushSample(string sampleName)
		{
			if ((this._trackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEnginePushSample(sampleName);
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000B7FC File Offset: 0x000099FC
		public void PopSample()
		{
			if ((this._trackMode & UwaProfiler.Mode.Overview) != (UwaProfiler.Mode)0)
			{
				UwaProfiler.UWAEnginePopSample();
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000B810 File Offset: 0x00009A10
		public void AddMarker(string valueName)
		{
			if (!string.IsNullOrEmpty(valueName))
			{
				UwaProfiler.AddMarker(valueName);
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000B824 File Offset: 0x00009A24
		public void LogValue(string valueName, float value)
		{
			if (!string.IsNullOrEmpty(valueName))
			{
				UwaProfiler.LogValue(valueName, value);
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000B838 File Offset: 0x00009A38
		public void LogValue(string valueName, int value)
		{
			if (!string.IsNullOrEmpty(valueName))
			{
				UwaProfiler.LogValue(valueName, value);
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000B84C File Offset: 0x00009A4C
		public void LogValue(string valueName, bool value)
		{
			if (!string.IsNullOrEmpty(valueName))
			{
				UwaProfiler.LogValue(valueName, value);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000B860 File Offset: 0x00009A60
		public void LogValue(string valueName, Vector3 value)
		{
			if (!string.IsNullOrEmpty(valueName))
			{
				UwaProfiler.LogValue(valueName, value);
			}
		}

		// Token: 0x040000D2 RID: 210
		private readonly UwaProfiler.Mode _trackMode;

		// Token: 0x040000D3 RID: 211
		public static bool MultiThreadEnabled = false;

		// Token: 0x040000D4 RID: 212
		public static DumpHelper MonoDumpHelper = new DumpHelper();
	}
}
