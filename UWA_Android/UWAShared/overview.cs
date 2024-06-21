using System;
using UWAShared;

// Token: 0x02000004 RID: 4
[Serializable]
public class overview
{
	// Token: 0x06000013 RID: 19 RVA: 0x00002660 File Offset: 0x00000860
	internal void ToJson(ref JSONNode json)
	{
		json["overview.mode"] = new JSONData((int)this.mode);
		json["overview.time_line"] = new JSONData(this.time_line);
		json["overview.stack_detail"] = new JSONData(this.stack_detail);
		json["overview.unity_api"] = new JSONData(this.unity_api);
		json["overview.lua"] = new JSONData(this.lua);
		json["overview.lua_dump_step"] = new JSONData(this.lua_dump_step);
		json["overview.resources"] = new JSONData(this.resources);
		json["overview.unity_loading"] = new JSONData(this.unity_loading);
		json["overview.engine_cpu_stack"] = new JSONData(this.engine_cpu_stack);
		json["overview.lua_cpu_stack"] = new JSONData(this.lua_cpu_stack);
		json["overview.lua_mem_stack"] = new JSONData(this.lua_mem_stack);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x0000277C File Offset: 0x0000097C
	internal void FromJson(ref JSONNode json)
	{
		bool flag = json["overview.time_line"] != null;
		if (flag)
		{
			this.time_line = json["overview.time_line"].AsBool;
		}
		bool flag2 = json["overview.stack_detail"] != null;
		if (flag2)
		{
			this.stack_detail = json["overview.stack_detail"].AsInt;
		}
		bool flag3 = json["overview.unity_api"] != null;
		if (flag3)
		{
			this.unity_api = json["overview.unity_api"].AsBool;
		}
		bool flag4 = json["overview.lua"] != null;
		if (flag4)
		{
			this.lua = json["overview.lua"].AsBool;
		}
		bool flag5 = json["overview.lua_dump_step"] != null;
		if (flag5)
		{
			this.lua_dump_step = json["overview.lua_dump_step"].AsInt;
		}
		bool flag6 = json["overview.resources"] != null;
		if (flag6)
		{
			this.resources = json["overview.resources"].AsInt;
		}
		bool flag7 = json["overview.unity_loading"] != null;
		if (flag7)
		{
			this.unity_loading = json["overview.unity_loading"].AsBool;
		}
		bool flag8 = json["overview.engine_cpu_stack"] != null;
		if (flag8)
		{
			this.engine_cpu_stack = json["overview.engine_cpu_stack"].AsBool;
		}
		bool flag9 = json["overview.lua_cpu_stack"] != null;
		if (flag9)
		{
			this.lua_cpu_stack = json["overview.lua_cpu_stack"].AsBool;
		}
		bool flag10 = json["overview.lua_mem_stack"] != null;
		if (flag10)
		{
			this.lua_mem_stack = json["overview.lua_mem_stack"].AsBool;
		}
		bool flag11 = json["overview.mode"] != null;
		if (flag11)
		{
			this.mode = (overview.ConfigMode)json["overview.mode"].AsInt;
			this.SetMode(this.mode);
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000029BC File Offset: 0x00000BBC
	public void SetDefault()
	{
		this.mode = overview.ConfigMode.Custom;
		this.engine_cpu_stack = true;
		this.time_line = true;
		this.stack_detail = 0;
		this.unity_api = false;
		this.lua = false;
		this.lua_dump_step = 10;
		this.lua_cpu_stack = false;
		this.lua_mem_stack = false;
		this.resources = 1;
		this.unity_loading = false;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002A1C File Offset: 0x00000C1C
	public void SetMode(overview.ConfigMode cMode)
	{
		this.mode = cMode;
		switch (this.mode)
		{
		case overview.ConfigMode.Minimal:
			this.time_line = false;
			this.stack_detail = 0;
			this.unity_api = false;
			this.lua = false;
			this.lua_dump_step = -1;
			this.resources = 0;
			this.unity_loading = false;
			this.engine_cpu_stack = false;
			this.lua_cpu_stack = false;
			this.lua_mem_stack = false;
			break;
		case overview.ConfigMode.CPU:
			this.time_line = true;
			this.stack_detail = 1;
			this.unity_api = true;
			this.lua_dump_step = -1;
			this.unity_loading = true;
			this.engine_cpu_stack = true;
			this.lua_cpu_stack = true;
			this.resources = 0;
			this.lua = false;
			this.lua_mem_stack = false;
			break;
		case overview.ConfigMode.Memory:
			this.lua = true;
			this.lua_dump_step = 10;
			this.resources = 2;
			this.lua_mem_stack = true;
			this.engine_cpu_stack = false;
			this.stack_detail = 0;
			this.lua_cpu_stack = false;
			this.unity_loading = false;
			this.unity_api = false;
			this.time_line = false;
			break;
		}
	}

	// Token: 0x0400000C RID: 12
	public overview.ConfigMode mode;

	// Token: 0x0400000D RID: 13
	public bool time_line;

	// Token: 0x0400000E RID: 14
	public int stack_detail;

	// Token: 0x0400000F RID: 15
	public bool unity_api;

	// Token: 0x04000010 RID: 16
	public bool lua;

	// Token: 0x04000011 RID: 17
	public int lua_dump_step;

	// Token: 0x04000012 RID: 18
	public int resources;

	// Token: 0x04000013 RID: 19
	public bool unity_loading;

	// Token: 0x04000014 RID: 20
	public bool engine_cpu_stack;

	// Token: 0x04000015 RID: 21
	public bool lua_cpu_stack;

	// Token: 0x04000016 RID: 22
	public bool lua_mem_stack;

	// Token: 0x020000D4 RID: 212
	public enum ConfigMode
	{
		// Token: 0x040005CC RID: 1484
		Custom,
		// Token: 0x040005CD RID: 1485
		Minimal,
		// Token: 0x040005CE RID: 1486
		CPU,
		// Token: 0x040005CF RID: 1487
		Memory
	}
}
