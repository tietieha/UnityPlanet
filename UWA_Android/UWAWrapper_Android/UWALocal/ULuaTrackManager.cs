using System;
using System.Collections;
using System.Reflection;
using UWA;

namespace UWALocal
{
	// Token: 0x02000013 RID: 19
	internal class ULuaTrackManager : ToLuaTrackManager
	{
		// Token: 0x060000DF RID: 223 RVA: 0x00006338 File Offset: 0x00004538
		public ULuaTrackManager(string extension) : base(extension)
		{
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006344 File Offset: 0x00004544
		protected override void InitType(Assembly assembly)
		{
			base.InitType(assembly);
			this.LuaScriptMgrType = assembly.GetType("LuaScriptMgr");
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("LuaScriptMgrType : " + (this.LuaScriptMgrType != null).ToString());
			}
			if (this.LuaScriptMgrType == null)
			{
				this.TranslatorType = assembly.GetType("LuaInterface.ObjectTranslator");
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TranslatorType : " + (this.TranslatorType != null).ToString());
				}
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000063E0 File Offset: 0x000045E0
		protected override void OnPrepare()
		{
			base.OnPrepare();
			if (this.LuaScriptMgrType != null)
			{
				this.InstanceProperty = this.LuaScriptMgrType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				this.luaField = this.LuaScriptMgrType.GetField("lua", BindingFlags.Instance | BindingFlags.Public);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("InstanceProperty : " + (this.InstanceProperty != null).ToString() + "; luaField : " + (this.luaField != null).ToString());
				}
			}
			if (this.TranslatorType != null)
			{
				this.TranslatorListField = this.TranslatorType.GetField("list", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				this.TranslatorInterpreterField = this.TranslatorType.GetField("interpreter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("TranslatorListField : " + (this.TranslatorListField != null).ToString() + "; TranslatorInterpreterField : " + (this.TranslatorInterpreterField != null).ToString());
				}
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000064F0 File Offset: 0x000046F0
		protected override bool CanAccessLuaState()
		{
			if (this.LuaScriptMgrType != null)
			{
				return this.InstanceProperty != null && this.luaField != null;
			}
			return this.TranslatorType != null && this.TranslatorListField != null && this.TranslatorInterpreterField != null;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006548 File Offset: 0x00004748
		protected override object GetLuaState()
		{
			if (base.GetOverrideLuaState() != null)
			{
				return base.GetOverrideLuaState();
			}
			this.luastates.Clear();
			if (this.CanAccessLuaState())
			{
				if (this.LuaScriptMgrType != null)
				{
					object value = this.InstanceProperty.GetValue(null, null);
					if (value == null)
					{
						return null;
					}
					object value2 = this.luaField.GetValue(value);
					this.luastates.Add(value2);
					return value2;
				}
				else if (this.TranslatorType != null)
				{
					IList list = this.TranslatorListField.GetValue(null) as IList;
					if (list == null || list.Count == 0)
					{
						return null;
					}
					object obj = null;
					for (int i = 0; i < list.Count - 1; i++)
					{
						object obj2 = list[i];
						if (obj2 != null)
						{
							object value3 = this.TranslatorInterpreterField.GetValue(obj2);
							if (value3 != null)
							{
								obj = value3;
								this.luastates.Add(obj);
							}
						}
					}
					if (obj != null)
					{
						return obj;
					}
				}
			}
			return null;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006648 File Offset: 0x00004848
		protected override string GetDefaultLuaLib()
		{
			return "libulua.so";
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006650 File Offset: 0x00004850
		protected new static bool VEquals(IntPtr L, int index)
		{
			object obj = null;
			return obj != null && obj.Equals(null);
		}

		// Token: 0x0400005C RID: 92
		private Type LuaScriptMgrType;

		// Token: 0x0400005D RID: 93
		private Type TranslatorType;

		// Token: 0x0400005E RID: 94
		private FieldInfo TranslatorListField;

		// Token: 0x0400005F RID: 95
		private FieldInfo TranslatorInterpreterField;

		// Token: 0x04000060 RID: 96
		private PropertyInfo InstanceProperty;

		// Token: 0x04000061 RID: 97
		private FieldInfo luaField;
	}
}
