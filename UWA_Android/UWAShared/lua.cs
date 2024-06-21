using System;
using UWAShared;

// Token: 0x02000007 RID: 7
[Serializable]
public class lua
{
	// Token: 0x06000020 RID: 32 RVA: 0x00002C3C File Offset: 0x00000E3C
	internal void ToJson(ref JSONNode json)
	{
		json["lua.lua_dump_step"] = new JSONData(this.lua_dump_step);
		json["lua.lua_cpu_stack"] = new JSONData(this.lua_cpu_stack);
		json["lua.lua_mem_stack"] = new JSONData(this.lua_mem_stack);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002C98 File Offset: 0x00000E98
	internal void FromJson(ref JSONNode json)
	{
		bool flag = json["lua.lua_dump_step"] != null;
		if (flag)
		{
			this.lua_dump_step = json["lua.lua_dump_step"].AsInt;
		}
		bool flag2 = json["lua.lua_cpu_stack"] != null;
		if (flag2)
		{
			this.lua_cpu_stack = json["lua.lua_cpu_stack"].AsBool;
		}
		bool flag3 = json["lua.lua_mem_stack"] != null;
		if (flag3)
		{
			this.lua_mem_stack = json["lua.lua_mem_stack"].AsBool;
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002D3C File Offset: 0x00000F3C
	public void SetDefault()
	{
		this.lua_dump_step = 10;
		this.lua_cpu_stack = true;
		this.lua_mem_stack = true;
	}

	// Token: 0x04000019 RID: 25
	public int lua_dump_step;

	// Token: 0x0400001A RID: 26
	public bool lua_cpu_stack;

	// Token: 0x0400001B RID: 27
	public bool lua_mem_stack;
}
