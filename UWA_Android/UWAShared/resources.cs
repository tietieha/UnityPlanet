using System;
using UWAShared;

// Token: 0x02000006 RID: 6
[Serializable]
public class resources
{
	// Token: 0x0600001C RID: 28 RVA: 0x00002BC4 File Offset: 0x00000DC4
	internal void ToJson(ref JSONNode json)
	{
		json["resources.unity_loading"] = new JSONData(this.unity_loading);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002BE0 File Offset: 0x00000DE0
	internal void FromJson(ref JSONNode json)
	{
		bool flag = json["resources.unity_loading"] != null;
		if (flag)
		{
			this.unity_loading = json["resources.unity_loading"].AsBool;
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002C24 File Offset: 0x00000E24
	public void SetDefault()
	{
		this.unity_loading = false;
	}

	// Token: 0x04000018 RID: 24
	public bool unity_loading;
}
