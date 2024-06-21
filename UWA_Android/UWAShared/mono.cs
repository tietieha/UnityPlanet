using System;
using UWAShared;

// Token: 0x02000005 RID: 5
[Serializable]
public class mono
{
	// Token: 0x06000018 RID: 24 RVA: 0x00002B4C File Offset: 0x00000D4C
	internal void ToJson(ref JSONNode json)
	{
		json["mono.mono_dump_step"] = new JSONData(this.mono_dump_step);
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002B68 File Offset: 0x00000D68
	internal void FromJson(ref JSONNode json)
	{
		bool flag = json["mono.mono_dump_step"] != null;
		if (flag)
		{
			this.mono_dump_step = json["mono.mono_dump_step"].AsInt;
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002BAC File Offset: 0x00000DAC
	public void SetDefault()
	{
		this.mono_dump_step = 10;
	}

	// Token: 0x04000017 RID: 23
	public int mono_dump_step;
}
