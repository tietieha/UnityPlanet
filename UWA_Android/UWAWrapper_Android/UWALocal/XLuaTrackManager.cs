using System;
using System.Collections.Generic;
using System.Reflection;
using AOT;

namespace UWALocal
{
	// Token: 0x02000015 RID: 21
	internal class XLuaTrackManager : LuaTrackManager
	{
		// Token: 0x060000ED RID: 237 RVA: 0x00006804 File Offset: 0x00004A04
		public XLuaTrackManager(string extension) : base(extension)
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006810 File Offset: 0x00004A10
		protected override void InitType(Assembly assembly)
		{
			XLuaTrackManager.ObjectPoolType = assembly.GetType("XLua.ObjectPool");
			XLuaTrackManager.ObjectTranslatorType = assembly.GetType("XLua.ObjectTranslator");
			XLuaTrackManager.ObjectTranslatorPoolType = assembly.GetType("XLua.ObjectTranslatorPool");
			XLuaTrackManager.pObjectTranslatorPoolInstance = XLuaTrackManager.ObjectTranslatorPoolType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			LuaTrackManager.AddJudgeDestroyedCallback(new LuaTrackManager.JudgeDestroyedFun(XLuaTrackManager.VEquals));
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006878 File Offset: 0x00004A78
		protected override void OnPrepare()
		{
			if (XLuaTrackManager.ObjectTranslatorType != null && XLuaTrackManager.ObjectTranslatorPoolType != null)
			{
				this.translatorField = LuaTrackManager.LuaStateType.GetField("translator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				this.objectsBackMapField = XLuaTrackManager.ObjectTranslatorType.GetField("reverseMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				this.lastTranslatorField = XLuaTrackManager.ObjectTranslatorPoolType.GetField("lastTranslator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				this.luaEnvField = XLuaTrackManager.ObjectTranslatorType.GetField("luaEnv", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000068FC File Offset: 0x00004AFC
		protected override bool CanAccessLuaState()
		{
			return XLuaTrackManager.pObjectTranslatorPoolInstance != null && this.lastTranslatorField != null && this.luaEnvField != null;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006920 File Offset: 0x00004B20
		protected override object GetLuaState()
		{
			if (base.GetOverrideLuaState() != null)
			{
				return base.GetOverrideLuaState();
			}
			if (!this.CanAccessLuaState())
			{
				return null;
			}
			object value = XLuaTrackManager.pObjectTranslatorPoolInstance.GetValue(null, null);
			if (value == null)
			{
				return null;
			}
			object value2 = this.lastTranslatorField.GetValue(value);
			if (value2 == null)
			{
				return null;
			}
			return this.luaEnvField.GetValue(value2);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006988 File Offset: 0x00004B88
		protected override bool CanAccessObjectCache()
		{
			return this.translatorField != null && this.objectsBackMapField != null;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000069A0 File Offset: 0x00004BA0
		protected override string GetDefaultLuaLib()
		{
			return "libxlua.so";
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000069A8 File Offset: 0x00004BA8
		protected override Dictionary<object, int> GetObjMap(object main)
		{
			return (Dictionary<object, int>)this.objectsBackMapField.GetValue(this.translatorField.GetValue(main));
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000069C8 File Offset: 0x00004BC8
		[MonoPInvokeCallback(typeof(LuaTrackManager.JudgeDestroyedFun))]
		protected static bool VEquals(IntPtr L, int index)
		{
			MethodInfo method = XLuaTrackManager.ObjectTranslatorPoolType.GetMethod("Find", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = method.Invoke(XLuaTrackManager.pObjectTranslatorPoolInstance.GetValue(null, null), new object[]
			{
				L
			});
			object value = XLuaTrackManager.ObjectTranslatorType.GetField("objects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj);
			object[] array = new object[2];
			array[0] = index;
			object[] array2 = array;
			XLuaTrackManager.ObjectPoolType.GetMethod("TryGetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(value, array2);
			object obj2 = array2[1];
			return obj2 != null && obj2.Equals(null);
		}

		// Token: 0x04000067 RID: 103
		private static Type ObjectTranslatorPoolType;

		// Token: 0x04000068 RID: 104
		private static Type ObjectTranslatorType;

		// Token: 0x04000069 RID: 105
		private static Type ObjectPoolType;

		// Token: 0x0400006A RID: 106
		private FieldInfo lastTranslatorField;

		// Token: 0x0400006B RID: 107
		private FieldInfo luaEnvField;

		// Token: 0x0400006C RID: 108
		private static PropertyInfo pObjectTranslatorPoolInstance;

		// Token: 0x0400006D RID: 109
		private FieldInfo translatorField;

		// Token: 0x0400006E RID: 110
		private FieldInfo objectsBackMapField;
	}
}
