using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWAShared;

// Token: 0x02000003 RID: 3
[Serializable]
public class SdkStartConfig
{
	// Token: 0x06000007 RID: 7 RVA: 0x00002128 File Offset: 0x00000328
	public SdkStartConfig()
	{
		this.overview = new overview();
		this.mono = new mono();
		this.resources = new resources();
		this.lua = new lua();
		this.gpu = new gpu();
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002184 File Offset: 0x00000384
	public void SetDefault()
	{
		this.overview.SetDefault();
		this.mono.SetDefault();
		this.resources.SetDefault();
		this.lua.SetDefault();
		this.gpu.SetDefault();
		this.ApplyRemoved();
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000021DC File Offset: 0x000003DC
	public void SetOverviewConfigMode(overview.ConfigMode mode)
	{
		this.overview.SetMode(mode);
		this.ApplyRemoved();
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000021F4 File Offset: 0x000003F4
	public void RemoveFeat(string f)
	{
		this.removedFeats.Add(f);
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002204 File Offset: 0x00000404
	public bool IsFeatRemoved(string f)
	{
		return this.removedFeats.Contains(f);
	}

	// Token: 0x0600000C RID: 12 RVA: 0x0000222C File Offset: 0x0000042C
	public void ApplyRemoved()
	{
		bool flag = this.IsFeatRemoved("lua");
		if (flag)
		{
			this.overview.lua = false;
			this.overview.lua_cpu_stack = false;
			this.overview.lua_mem_stack = false;
			this.overview.lua_dump_step = -1;
			this.lua.lua_cpu_stack = false;
			this.lua.lua_mem_stack = false;
			this.lua.lua_dump_step = -1;
		}
		bool flag2 = this.IsFeatRemoved("mono");
		if (flag2)
		{
			this.mono.mono_dump_step = -1;
		}
		bool flag3 = this.IsFeatRemoved("time_line");
		if (flag3)
		{
			this.overview.time_line = false;
			this.overview.stack_detail = 0;
		}
		bool flag4 = this.IsFeatRemoved("unity_api");
		if (flag4)
		{
			this.overview.unity_api = false;
		}
		bool flag5 = this.IsFeatRemoved("unity_loading");
		if (flag5)
		{
			this.overview.unity_loading = false;
			this.resources.unity_loading = false;
		}
		bool flag6 = this.IsFeatRemoved("gpu_res");
		if (flag6)
		{
			this.gpu.mesh_analysis = false;
			this.gpu.texture_analysis = false;
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002370 File Offset: 0x00000570
	public static void Store()
	{
		StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "/uwa-got.json");
		streamWriter.Write(SdkStartConfig.Json);
		streamWriter.Close();
	}

	// Token: 0x0600000E RID: 14 RVA: 0x000023AC File Offset: 0x000005AC
	public static void Load()
	{
		string path = Application.persistentDataPath + "/uwa-got.json";
		SdkStartConfig.Instance.SetDefault();
		bool flag = !File.Exists(path);
		if (flag)
		{
			File.WriteAllText(path, SdkStartConfig.Json);
		}
		try
		{
			SdkStartConfig.Json = File.ReadAllText(path);
		}
		catch
		{
		}
		SdkStartConfig.Instance.ApplyRemoved();
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000242C File Offset: 0x0000062C
	public static void UpdateConfig(string startConfig)
	{
		try
		{
			JSONNode jsonnode = JSON.Parse(startConfig);
			SdkStartConfig.Instance.overview.FromJson(ref jsonnode);
			SdkStartConfig.Instance.mono.FromJson(ref jsonnode);
			SdkStartConfig.Instance.resources.FromJson(ref jsonnode);
			SdkStartConfig.Instance.lua.FromJson(ref jsonnode);
			SdkStartConfig.Instance.gpu.FromJson(ref jsonnode);
		}
		catch (Exception)
		{
		}
		SdkStartConfig.Instance.ApplyRemoved();
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000010 RID: 16 RVA: 0x000024C8 File Offset: 0x000006C8
	// (set) Token: 0x06000011 RID: 17 RVA: 0x00002560 File Offset: 0x00000760
	public static string Json
	{
		get
		{
			JSONNode jsonnode = new JSONClass();
			jsonnode["version"] = "2.4.9.0";
			SdkStartConfig.Instance.overview.ToJson(ref jsonnode);
			SdkStartConfig.Instance.mono.ToJson(ref jsonnode);
			SdkStartConfig.Instance.resources.ToJson(ref jsonnode);
			SdkStartConfig.Instance.lua.ToJson(ref jsonnode);
			SdkStartConfig.Instance.gpu.ToJson(ref jsonnode);
			return jsonnode.ToJSON(0);
		}
		private set
		{
			try
			{
				JSONNode jsonnode = JSON.Parse(value);
				string a = (jsonnode["version"] != null) ? jsonnode["version"] : "null";
				bool flag = jsonnode == null || a != "2.4.9.0";
				if (!flag)
				{
					SdkStartConfig.Instance.overview.FromJson(ref jsonnode);
					SdkStartConfig.Instance.mono.FromJson(ref jsonnode);
					SdkStartConfig.Instance.resources.FromJson(ref jsonnode);
					SdkStartConfig.Instance.lua.FromJson(ref jsonnode);
					SdkStartConfig.Instance.gpu.FromJson(ref jsonnode);
					SdkStartConfig.Instance.ApplyRemoved();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x04000004 RID: 4
	public const string version = "2.4.9.0";

	// Token: 0x04000005 RID: 5
	public overview overview;

	// Token: 0x04000006 RID: 6
	public mono mono;

	// Token: 0x04000007 RID: 7
	public resources resources;

	// Token: 0x04000008 RID: 8
	public lua lua;

	// Token: 0x04000009 RID: 9
	public gpu gpu;

	// Token: 0x0400000A RID: 10
	private HashSet<string> removedFeats = new HashSet<string>();

	// Token: 0x0400000B RID: 11
	public static readonly SdkStartConfig Instance = new SdkStartConfig();
}
