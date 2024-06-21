using System;
using UnityEngine;

namespace UWACore.TrackManagers
{
	// Token: 0x02000035 RID: 53
	internal class UWAEngineInternal
	{
		// Token: 0x0600025B RID: 603 RVA: 0x0000FD34 File Offset: 0x0000DF34
		public static void LogValueInternal(string valueName, float value, ApiTrackManager.TrackTag tag)
		{
			bool flag = UWAEngineInternal.Instance != null;
			if (flag)
			{
				UWAEngineInternal.Instance.LogValueInternal(valueName, value, tag);
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000FD64 File Offset: 0x0000DF64
		public static void LogValueInternal(string valueName, int value, ApiTrackManager.TrackTag tag)
		{
			bool flag = UWAEngineInternal.Instance != null;
			if (flag)
			{
				UWAEngineInternal.Instance.LogValueInternal(valueName, value, tag);
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000FD94 File Offset: 0x0000DF94
		public static void LogValueInternal(string valueName, Vector3 value, ApiTrackManager.TrackTag tag)
		{
			bool flag = UWAEngineInternal.Instance != null;
			if (flag)
			{
				UWAEngineInternal.Instance.LogValueInternal(valueName, value, tag);
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000FDC4 File Offset: 0x0000DFC4
		public static void LogValueInternal(string valueName, bool value, ApiTrackManager.TrackTag tag)
		{
			bool flag = UWAEngineInternal.Instance != null;
			if (flag)
			{
				UWAEngineInternal.Instance.LogValueInternal(valueName, value, tag);
			}
		}

		// Token: 0x04000159 RID: 345
		public static IUWAAPIInternal Instance = null;

		// Token: 0x0400015A RID: 346
		public static string SamplePrefix = "UWA-";
	}
}
