using System;
using System.Collections.Generic;
using System.Reflection;
using AOT;
using UWA;

namespace UWALocal
{
	// Token: 0x02000012 RID: 18
	internal class ToLuaTrackManager : LuaTrackManager
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x00006144 File Offset: 0x00004344
		public ToLuaTrackManager(string extension) : base(extension)
		{
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006150 File Offset: 0x00004350
		protected override void InitType(Assembly assembly)
		{
			ToLuaTrackManager.ObjectTranslatorType = assembly.GetType("LuaInterface.ObjectTranslator");
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("ObjectTranslatorType : " + (ToLuaTrackManager.ObjectTranslatorType != null).ToString());
			}
			ToLuaTrackManager.GetFun = ToLuaTrackManager.ObjectTranslatorType.GetMethod("Get", BindingFlags.Static | BindingFlags.Public);
			ToLuaTrackManager.GetObjectFun = ToLuaTrackManager.ObjectTranslatorType.GetMethod("GetObject", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
			{
				typeof(int)
			}, null);
			if (ToLuaTrackManager.GetFun != null && ToLuaTrackManager.GetObjectFun != null && !ToLuaTrackManager.GetObjectFun.IsGenericMethod)
			{
				LuaTrackManager.AddJudgeDestroyedCallback(new LuaTrackManager.JudgeDestroyedFun(ToLuaTrackManager.VEquals));
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006210 File Offset: 0x00004410
		protected override void OnPrepare()
		{
			base.OnPrepare();
			if (ToLuaTrackManager.ObjectTranslatorType != null)
			{
				ToLuaTrackManager.translatorField = LuaTrackManager.LuaStateType.GetField("translator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				ToLuaTrackManager.objectsBackMapField = ToLuaTrackManager.ObjectTranslatorType.GetField("objectsBackMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("translatorField : " + (ToLuaTrackManager.translatorField != null).ToString() + "; objectsBackMapField : " + (ToLuaTrackManager.objectsBackMapField != null).ToString());
				}
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000629C File Offset: 0x0000449C
		protected override bool CanAccessObjectCache()
		{
			return ToLuaTrackManager.objectsBackMapField != null && ToLuaTrackManager.translatorField != null;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000062B4 File Offset: 0x000044B4
		protected override string GetDefaultLuaLib()
		{
			return "libtolua.so";
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000062BC File Offset: 0x000044BC
		protected override Dictionary<object, int> GetObjMap(object main)
		{
			return (Dictionary<object, int>)ToLuaTrackManager.objectsBackMapField.GetValue(ToLuaTrackManager.translatorField.GetValue(main));
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000062D8 File Offset: 0x000044D8
		[MonoPInvokeCallback(typeof(LuaTrackManager.JudgeDestroyedFun))]
		protected static bool VEquals(IntPtr L, int index)
		{
			object obj = ToLuaTrackManager.GetFun.Invoke(null, new object[]
			{
				L
			});
			object obj2 = ToLuaTrackManager.GetObjectFun.Invoke(obj, new object[]
			{
				index
			});
			return obj2 != null && obj2.Equals(null);
		}

		// Token: 0x04000057 RID: 87
		private static Type ObjectTranslatorType;

		// Token: 0x04000058 RID: 88
		private static FieldInfo translatorField;

		// Token: 0x04000059 RID: 89
		private static FieldInfo objectsBackMapField;

		// Token: 0x0400005A RID: 90
		private static MethodInfo GetFun;

		// Token: 0x0400005B RID: 91
		private static MethodInfo GetObjectFun;
	}
}
