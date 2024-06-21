using System;
using System.Collections.Generic;

// Token: 0x02000009 RID: 9
public static class SdkEventSystem
{
	// Token: 0x06000028 RID: 40 RVA: 0x00002E2C File Offset: 0x0000102C
	public static void AddEventListener(SdkEventSystem.SdkEventType type, SdkEventSystem.SdkEventListener listener)
	{
		bool flag = !SdkEventSystem.eventList.ContainsKey(type);
		if (flag)
		{
			SdkEventSystem.eventList.Add(type, new List<SdkEventSystem.SdkEventListener>());
		}
		SdkEventSystem.eventList[type].Add(listener);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002E78 File Offset: 0x00001078
	public static void RemoveEventListener(SdkEventSystem.SdkEventType type, SdkEventSystem.SdkEventListener listener)
	{
		bool flag = !SdkEventSystem.eventList.ContainsKey(type);
		if (!flag)
		{
			SdkEventSystem.eventList[type].Remove(listener);
		}
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002EB8 File Offset: 0x000010B8
	public static void DispatchEvent(SdkEventSystem.SdkEventType type, SdkEventSystem.SdkEvent e)
	{
		bool flag = !SdkEventSystem.eventList.ContainsKey(type);
		if (!flag)
		{
			foreach (SdkEventSystem.SdkEventListener sdkEventListener in SdkEventSystem.eventList[type])
			{
				bool flag2 = sdkEventListener != null;
				if (flag2)
				{
					sdkEventListener(e);
				}
			}
		}
	}

	// Token: 0x0400001E RID: 30
	private static Dictionary<SdkEventSystem.SdkEventType, List<SdkEventSystem.SdkEventListener>> eventList = new Dictionary<SdkEventSystem.SdkEventType, List<SdkEventSystem.SdkEventListener>>();

	// Token: 0x020000D5 RID: 213
	public enum SdkEventType
	{
		// Token: 0x040005D1 RID: 1489
		UISTATE_CHANGED,
		// Token: 0x040005D2 RID: 1490
		SCREEN_CHANGED
	}

	// Token: 0x020000D6 RID: 214
	public class SdkEvent
	{
	}

	// Token: 0x020000D7 RID: 215
	// (Invoke) Token: 0x06000970 RID: 2416
	public delegate void SdkEventListener(SdkEventSystem.SdkEvent e);
}
