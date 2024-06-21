using System;
using System.Collections.Generic;
using System.Reflection;
using AOT;

namespace UWALocal
{
	// Token: 0x02000014 RID: 20
	internal class SLuaTrackManager : LuaTrackManager
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x0000667C File Offset: 0x0000487C
		public SLuaTrackManager(string extension) : base(extension)
		{
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006690 File Offset: 0x00004890
		protected override void InitType(Assembly assembly)
		{
			SLuaTrackManager.ObjectCacheType = assembly.GetType("SLua.ObjectCache");
			LuaTrackManager.AddJudgeDestroyedCallback(new LuaTrackManager.JudgeDestroyedFun(SLuaTrackManager.VEquals));
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000066B4 File Offset: 0x000048B4
		protected override void OnPrepare()
		{
			base.OnPrepare();
			if (SLuaTrackManager.ObjectCacheType != null)
			{
				this.ObjectCacheGet = SLuaTrackManager.ObjectCacheType.GetMethod("get", BindingFlags.Static | BindingFlags.Public);
				this.objMapField = SLuaTrackManager.ObjectCacheType.GetField("objMap", BindingFlags.Instance | BindingFlags.NonPublic);
				this.cacheField = SLuaTrackManager.ObjectCacheType.GetField("cache", BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000671C File Offset: 0x0000491C
		protected override bool CanAccessObjectCache()
		{
			return this.ObjectCacheGet != null && this.objMapField != null;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006734 File Offset: 0x00004934
		protected override string GetDefaultLuaLib()
		{
			return "libslua.so";
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000673C File Offset: 0x0000493C
		protected override Dictionary<object, int> GetObjMap(object main)
		{
			return (Dictionary<object, int>)this.objMapField.GetValue(this.ObjectCacheGet.Invoke(null, new object[]
			{
				(IntPtr)LuaTrackManager.LField.GetValue(main)
			}));
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006788 File Offset: 0x00004988
		[MonoPInvokeCallback(typeof(LuaTrackManager.JudgeDestroyedFun))]
		protected static bool VEquals(IntPtr L, int index)
		{
			object obj = null;
			MethodInfo method = SLuaTrackManager.ObjectCacheType.GetMethod("get", BindingFlags.Static | BindingFlags.Public);
			FieldInfo field = SLuaTrackManager.ObjectCacheType.GetField("cache", BindingFlags.Instance | BindingFlags.NonPublic);
			Dictionary<int, object> dictionary = (Dictionary<int, object>)field.GetValue(method.Invoke(null, new object[]
			{
				L
			}));
			dictionary.TryGetValue(index, out obj);
			return obj != null && obj.Equals(null);
		}

		// Token: 0x04000062 RID: 98
		private static Type ObjectCacheType;

		// Token: 0x04000063 RID: 99
		private MethodInfo ObjectCacheGet;

		// Token: 0x04000064 RID: 100
		private FieldInfo objMapField;

		// Token: 0x04000065 RID: 101
		private FieldInfo cacheField;

		// Token: 0x04000066 RID: 102
		private List<int> UserdataIndex = new List<int>();
	}
}
