using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UWA;
using UWAShared;

namespace UWALocal
{
	// Token: 0x02000016 RID: 22
	internal abstract class LuaTrackManager : BaseTrackerManager
	{
		// Token: 0x060000F6 RID: 246
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void AddJudgeDestroyedCallback(LuaTrackManager.JudgeDestroyedFun t);

		// Token: 0x060000F7 RID: 247
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void LuaSettingsFromCSharp(string dllPath, int byteLen, IntPtr l);

		// Token: 0x060000F8 RID: 248
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ChangeLuaMainState(IntPtr luastate);

		// Token: 0x060000F9 RID: 249
		protected abstract bool CanAccessObjectCache();

		// Token: 0x060000FA RID: 250
		protected abstract Dictionary<object, int> GetObjMap(object main);

		// Token: 0x060000FB RID: 251 RVA: 0x00006A6C File Offset: 0x00004C6C
		public LuaTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006A88 File Offset: 0x00004C88
		public static void StaticInit()
		{
			LuaTrackManager.LuaDetected = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				LuaTrackManager.LuaStateType = assemblies[i].GetType("LuaInterface.LuaState");
				if (LuaTrackManager.LuaStateType != null)
				{
					LuaTrackManager.LuaAssembly = assemblies[i];
				}
				else
				{
					LuaTrackManager.LuaStateType = assemblies[i].GetType("SLua.LuaState");
					if (LuaTrackManager.LuaStateType == null)
					{
						LuaTrackManager.LuaStateType = assemblies[i].GetType("SLua.LuaStack");
					}
					if (LuaTrackManager.LuaStateType != null)
					{
						LuaTrackManager.LuaAssembly = assemblies[i];
					}
					else
					{
						LuaTrackManager.LuaStateType = assemblies[i].GetType("XLua.LuaEnv");
						if (LuaTrackManager.LuaStateType != null)
						{
							LuaTrackManager.LuaAssembly = assemblies[i];
						}
					}
				}
				if (LuaTrackManager.LuaStateType != null)
				{
					LuaTrackManager.LuaDetected = true;
					return;
				}
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00006B80 File Offset: 0x00004D80
		protected virtual void InitType(Assembly assembly)
		{
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006B84 File Offset: 0x00004D84
		public static LuaTrackManager Get(string ext)
		{
			if (!LuaTrackManager.LuaDetected || LuaTrackManager.LuaAssembly == null || LuaTrackManager.LuaStateType == null)
			{
				return null;
			}
			LuaTrackManager luaTrackManager = null;
			if (LuaTrackManager.LuaStateType.FullName == "LuaInterface.LuaState")
			{
				luaTrackManager = ((LuaTrackManager.LuaAssembly.GetType("ToLua") != null || LuaTrackManager.LuaAssembly.GetType("LuaInterface.ToLua") != null) ? new ToLuaTrackManager(ext) : new ULuaTrackManager(ext));
				luaTrackManager.InitType(LuaTrackManager.LuaAssembly);
			}
			else if (LuaTrackManager.LuaStateType.FullName == "SLua.LuaState" || LuaTrackManager.LuaStateType.FullName == "SLua.LuaStack")
			{
				luaTrackManager = new SLuaTrackManager(ext);
				luaTrackManager.InitType(LuaTrackManager.LuaAssembly);
			}
			else if (LuaTrackManager.LuaStateType.FullName == "XLua.LuaEnv")
			{
				luaTrackManager = new XLuaTrackManager(ext);
				luaTrackManager.InitType(LuaTrackManager.LuaAssembly);
			}
			if (luaTrackManager == null)
			{
				SharedUtils.Log("Target type is not found from all the dlls.");
				return null;
			}
			if (SharedUtils.ShowLog)
			{
				string str = "LuaTrackType = ";
				Type type = luaTrackManager.GetType();
				SharedUtils.Log(str + ((type != null) ? type.ToString() : null));
			}
			return luaTrackManager;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006CD4 File Offset: 0x00004ED4
		protected virtual void OnPrepare()
		{
			FieldInfo fieldInfo;
			if ((fieldInfo = LuaTrackManager.LuaStateType.GetField("mainState", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) == null)
			{
				fieldInfo = (LuaTrackManager.LuaStateType.GetField("main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ?? LuaTrackManager.LuaStateType.GetField("m_instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			}
			this.mainStateField = fieldInfo;
			if (this.mainStateField == null)
			{
				this.mainStateProperty = (LuaTrackManager.LuaStateType.GetProperty("main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ?? LuaTrackManager.LuaStateType.GetProperty("mainState", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			}
			if (this.mainStateField == null && this.mainStateProperty == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Type type = assemblies[i].GetType("LuaClient");
					if (type != null)
					{
						this.mainStateMethod = type.GetMethod("GetMainState", BindingFlags.Static | BindingFlags.Public);
					}
				}
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006DC4 File Offset: 0x00004FC4
		private string GetLuaDllPath()
		{
			string str = "/data/data/" + CoreUtils.PkgName + "/lib/";
			string text = str + this.GetLuaLib();
			if (File.Exists(text))
			{
				return text;
			}
			text = CoreUtils.GetNativeLibraryDir() + "/" + this.GetLuaLib();
			if (File.Exists(text))
			{
				return text;
			}
			return this.GetLuaLib();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006E34 File Offset: 0x00005034
		private string GetLuaLib()
		{
			string text = this.GetOverrideLuaLib();
			if (string.IsNullOrEmpty(text))
			{
				text = this.GetDefaultLuaLib();
			}
			return text;
		}

		// Token: 0x06000102 RID: 258
		protected abstract string GetDefaultLuaLib();

		// Token: 0x06000103 RID: 259 RVA: 0x00006E60 File Offset: 0x00005060
		protected string GetOverrideLuaLib()
		{
			return WrapperTool.LuaDll;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006E68 File Offset: 0x00005068
		protected object GetOverrideLuaState()
		{
			return SharedUtils.LuaState;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00006E70 File Offset: 0x00005070
		protected virtual bool CanAccessLuaState()
		{
			return this.mainStateField != null || this.mainStateProperty != null || this.mainStateMethod != null;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006E94 File Offset: 0x00005094
		protected bool CanAccessL()
		{
			return LuaTrackManager.LField != null || LuaTrackManager.LProp != null;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006EAC File Offset: 0x000050AC
		protected override void Prepare()
		{
			if (!LuaTrackManager.LuaDetected)
			{
				return;
			}
			this.OnPrepare();
			if (this.CanAccessObjectCache())
			{
				this.ObjectGet = new ObjectGetType(SharedUtils.FinalDataPath + "/lua_mono_map.txt");
			}
			FieldInfo lfield;
			if ((lfield = LuaTrackManager.LuaStateType.GetField("L", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null && (lfield = LuaTrackManager.LuaStateType.GetField("l_", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null)
			{
				lfield = (LuaTrackManager.LuaStateType.GetField("rawL", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? LuaTrackManager.LuaStateType.GetField("state", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
			}
			LuaTrackManager.LField = lfield;
			LuaTrackManager.LProp = LuaTrackManager.LuaStateType.GetProperty("L", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			this.worked = (this.CanAccessLuaState() && this.CanAccessL());
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log(string.Concat(new string[]
				{
					"Worked ",
					this.worked.ToString(),
					"; LField ",
					(LuaTrackManager.LField != null).ToString(),
					"; LProp ",
					(LuaTrackManager.LProp != null).ToString()
				}));
			}
			string luaDllPath = this.GetLuaDllPath();
			this.dllFound = File.Exists(luaDllPath);
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Lua Path : " + luaDllPath);
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Lua Tracking : " + this.worked.ToString());
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000703C File Offset: 0x0000523C
		protected virtual object GetLuaState()
		{
			if (this.GetOverrideLuaState() != null)
			{
				return this.GetOverrideLuaState();
			}
			if (this.mainStateField != null)
			{
				return this.mainStateField.GetValue(null);
			}
			if (this.mainStateProperty != null)
			{
				return this.mainStateProperty.GetValue(null, null);
			}
			if (this.mainStateMethod != null)
			{
				return this.mainStateMethod.Invoke(null, null);
			}
			return null;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000070AC File Offset: 0x000052AC
		public override bool IsReady(out string error)
		{
			error = null;
			if (!LuaTrackManager.LuaDetected)
			{
				error = "lua not detected.\n";
				return false;
			}
			if (!this.dllFound)
			{
				string luaDllPath = this.GetLuaDllPath();
				this.dllFound = File.Exists(luaDllPath);
				error = "lua lib not found: \n" + luaDllPath;
				return false;
			}
			if (!this.CanAccessLuaState())
			{
				error = "can not access lua state.";
				return false;
			}
			if (!this.CanAccessL())
			{
				error = "can not access L ptr.";
				return false;
			}
			if (!this.worked)
			{
				error = "lua not worked.";
				return false;
			}
			object luaState = this.GetLuaState();
			if (LuaTrackManager.LuaStartWithState && luaState == null)
			{
				error = "Waiting Lua State...";
				return false;
			}
			return true;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00007160 File Offset: 0x00005360
		public override void StartTrack()
		{
			if (this.worked && base.Enabled)
			{
				try
				{
					object luaState = this.GetLuaState();
					if (luaState == null)
					{
						if (SharedUtils.ShowLog)
						{
							SharedUtils.Log("main lua state = null");
						}
						return;
					}
					object obj = null;
					if (LuaTrackManager.LField != null)
					{
						obj = LuaTrackManager.LField.GetValue(luaState);
					}
					else if (LuaTrackManager.LProp != null)
					{
						obj = LuaTrackManager.LProp.GetValue(luaState, null);
					}
					if (LuaTrackManager.LuaMainState != (IntPtr)obj)
					{
						LuaTrackManager.LuaMainState = (IntPtr)obj;
					}
					LuaTrackManager.LuaSettingsFromCSharp(this.GetLuaDllPath(), 0, LuaTrackManager.LuaMainState);
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("StartLuaOn " + this.worked.ToString());
					}
				}
				catch (Exception ex)
				{
					string str = "StartLuaOn ";
					Exception ex2 = ex;
					SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				if (!this.worked)
				{
					base.Enabled = false;
				}
			}
			base.StartTrack();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00007288 File Offset: 0x00005488
		protected override void DoUpdateAtEnd()
		{
			if (!LuaTrackManager.LuaDetected)
			{
				return;
			}
			if (!this.worked)
			{
				return;
			}
			object luaState = this.GetLuaState();
			if (luaState == null)
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("main lua state = null");
				}
				return;
			}
			object obj = null;
			if (LuaTrackManager.LField != null)
			{
				obj = LuaTrackManager.LField.GetValue(luaState);
			}
			else if (LuaTrackManager.LProp != null)
			{
				obj = LuaTrackManager.LProp.GetValue(luaState, null);
			}
			if (LuaTrackManager.LuaMainState != (IntPtr)obj || SharedUtils.frameId % 30 == 0)
			{
				LuaTrackManager.LuaMainState = (IntPtr)obj;
				LuaTrackManager.ChangeLuaMainState(LuaTrackManager.LuaMainState);
			}
			if (LuaTrackManager.LuaDumpHelper.UpdateDump())
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("Lua Dump");
				}
				if (this.CanAccessObjectCache())
				{
					Dictionary<object, int> objMap = this.GetObjMap(luaState);
					this.ObjectGet.ObjectGet(objMap, CoreUtils.GetFrameIdWithExtFilePath(""));
				}
				BaseTrackerManager.Dump(2);
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007390 File Offset: 0x00005590
		public override void StopTrack()
		{
			if (this.worked && base.Enabled)
			{
				if (this.ObjectGet != null)
				{
					this.ObjectGet.Stop();
				}
				this.OnStopTrack();
			}
			base.StopTrack();
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000073CC File Offset: 0x000055CC
		protected void OnStopTrack()
		{
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000073D0 File Offset: 0x000055D0
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x0400006F RID: 111
		public static DumpHelper LuaDumpHelper = new DumpHelper
		{
			UseDefault = false,
			AutoDump = true,
			DumpInterval100 = 5
		};

		// Token: 0x04000070 RID: 112
		public static bool LuaInOverview = false;

		// Token: 0x04000071 RID: 113
		public static bool LuaDetected = false;

		// Token: 0x04000072 RID: 114
		public static bool LuaStartWithState = true;

		// Token: 0x04000073 RID: 115
		private bool worked;

		// Token: 0x04000074 RID: 116
		private bool dllFound;

		// Token: 0x04000075 RID: 117
		public static Type LuaStateType = null;

		// Token: 0x04000076 RID: 118
		public static PropertyInfo LProp = null;

		// Token: 0x04000077 RID: 119
		public static FieldInfo LField = null;

		// Token: 0x04000078 RID: 120
		private static Assembly LuaAssembly = null;

		// Token: 0x04000079 RID: 121
		private static IntPtr LuaMainState = IntPtr.Zero;

		// Token: 0x0400007A RID: 122
		protected List<object> luastates = new List<object>();

		// Token: 0x0400007B RID: 123
		private FieldInfo mainStateField;

		// Token: 0x0400007C RID: 124
		private PropertyInfo mainStateProperty;

		// Token: 0x0400007D RID: 125
		private MethodInfo mainStateMethod;

		// Token: 0x0400007E RID: 126
		private ObjectGetType ObjectGet;

		// Token: 0x0400007F RID: 127
		public Dictionary<int, object> CacheDict;

		// Token: 0x020000CF RID: 207
		// (Invoke) Token: 0x060008AA RID: 2218
		public delegate bool JudgeDestroyedFun(IntPtr l, int index);

		// Token: 0x020000D0 RID: 208
		[Flags]
		public enum Mode
		{
			// Token: 0x04000587 RID: 1415
			CpuOnly = 1,
			// Token: 0x04000588 RID: 1416
			CPUandMemory = 2
		}
	}
}
