using System;
using UnityEngine;

namespace UWACore.TrackManagers
{
	// Token: 0x02000034 RID: 52
	internal interface IUWAAPIInternal
	{
		// Token: 0x06000257 RID: 599
		void LogValueInternal(string valueName, float value, ApiTrackManager.TrackTag tag);

		// Token: 0x06000258 RID: 600
		void LogValueInternal(string valueName, int value, ApiTrackManager.TrackTag tag);

		// Token: 0x06000259 RID: 601
		void LogValueInternal(string valueName, bool value, ApiTrackManager.TrackTag tag);

		// Token: 0x0600025A RID: 602
		void LogValueInternal(string valueName, Vector3 value, ApiTrackManager.TrackTag tag);
	}
}
