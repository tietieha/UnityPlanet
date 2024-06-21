using System;
using UnityEngine;

// Token: 0x02000004 RID: 4
internal interface IUWAAPI
{
	// Token: 0x06000004 RID: 4
	void Tag(string tag);

	// Token: 0x06000005 RID: 5
	void PushSample(string sampleName);

	// Token: 0x06000006 RID: 6
	void PopSample();

	// Token: 0x06000007 RID: 7
	void LogValue(string valueName, float value);

	// Token: 0x06000008 RID: 8
	void LogValue(string valueName, int value);

	// Token: 0x06000009 RID: 9
	void LogValue(string valueName, bool value);

	// Token: 0x0600000A RID: 10
	void LogValue(string valueName, Vector3 value);

	// Token: 0x0600000B RID: 11
	void AddMarker(string valueName);

	// Token: 0x0600000C RID: 12
	void Register(Type classType, string fieldName, float updateInterval);

	// Token: 0x0600000D RID: 13
	void Register(object classObj, string instanceName, string fieldName, float updateInterval);
}
