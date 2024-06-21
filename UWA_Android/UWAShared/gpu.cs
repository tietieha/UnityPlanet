using System;
using UWAShared;

// Token: 0x02000008 RID: 8
public class gpu
{
	// Token: 0x06000024 RID: 36 RVA: 0x00002D64 File Offset: 0x00000F64
	internal void ToJson(ref JSONNode json)
	{
		json["gpu.texture_analysis"] = new JSONData(this.texture_analysis);
		json["gpu.mesh_analysis"] = new JSONData(this.mesh_analysis);
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002D98 File Offset: 0x00000F98
	internal void FromJson(ref JSONNode json)
	{
		bool flag = json["gpu.texture_analysis"] != null;
		if (flag)
		{
			this.texture_analysis = json["gpu.texture_analysis"].AsBool;
		}
		bool flag2 = json["gpu.mesh_analysis"] != null;
		if (flag2)
		{
			this.mesh_analysis = json["gpu.mesh_analysis"].AsBool;
		}
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002E0C File Offset: 0x0000100C
	public void SetDefault()
	{
		this.texture_analysis = false;
		this.mesh_analysis = false;
	}

	// Token: 0x0400001C RID: 28
	public bool texture_analysis;

	// Token: 0x0400001D RID: 29
	public bool mesh_analysis;
}
