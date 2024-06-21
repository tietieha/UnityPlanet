using System;
using System.Collections.Generic;
using System.Reflection;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000055 RID: 85
	internal struct JSONValue
	{
		// Token: 0x0600038B RID: 907 RVA: 0x00020AD8 File Offset: 0x0001ECD8
		public void Deserialize(object obj)
		{
			Type type = obj.GetType();
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				bool flag = this.ContainsKey(fields[i].Name);
				if (flag)
				{
					JSONValue jsonvalue = this[fields[i].Name];
					bool flag2 = jsonvalue.IsString();
					if (flag2)
					{
						fields[i].SetValue(obj, jsonvalue.AsString(false));
					}
					else
					{
						bool flag3 = jsonvalue.IsBool();
						if (flag3)
						{
							fields[i].SetValue(obj, jsonvalue.AsBool(false));
						}
						else
						{
							bool flag4 = jsonvalue.IsFloat();
							if (flag4)
							{
								fields[i].SetValue(obj, (int)jsonvalue.AsFloat(false));
							}
							else
							{
								bool flag5 = jsonvalue.IsList();
								if (flag5)
								{
									fields[i].SetValue(obj, jsonvalue.AsList(false));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00020C00 File Offset: 0x0001EE00
		public JSONValue(object o)
		{
			this.data = o;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00020C0C File Offset: 0x0001EE0C
		public bool IsString()
		{
			return this.data is string;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00020C34 File Offset: 0x0001EE34
		public bool IsFloat()
		{
			return this.data is float;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00020C5C File Offset: 0x0001EE5C
		public bool IsList()
		{
			return this.data is List<JSONValue>;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00020C84 File Offset: 0x0001EE84
		public bool IsStringList()
		{
			return this.data is List<string>;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00020CAC File Offset: 0x0001EEAC
		public bool IsDict()
		{
			return this.data is Dictionary<string, JSONValue>;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00020CD4 File Offset: 0x0001EED4
		public bool IsBool()
		{
			return this.data is bool;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00020CFC File Offset: 0x0001EEFC
		public bool IsNull()
		{
			return this.data == null;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00020D20 File Offset: 0x0001EF20
		public string AsString(bool nothrow = false)
		{
			bool flag = this.data is string;
			string result;
			if (flag)
			{
				result = (string)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read non-string json value as string");
				}
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00020D7C File Offset: 0x0001EF7C
		public float AsFloat(bool nothrow = false)
		{
			bool flag = this.data is float;
			float result;
			if (flag)
			{
				result = (float)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read non-float json value as float");
				}
				result = 0f;
			}
			return result;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00020DD8 File Offset: 0x0001EFD8
		public bool AsBool(bool nothrow = false)
		{
			bool flag = this.data is bool;
			bool result;
			if (flag)
			{
				result = (bool)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read non-bool json value as bool");
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00020E30 File Offset: 0x0001F030
		public List<JSONValue> AsList(bool nothrow = false)
		{
			bool flag = this.data is List<JSONValue>;
			List<JSONValue> result;
			if (flag)
			{
				result = (List<JSONValue>)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read " + this.data.GetType().Name + " json value as list");
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00020EA4 File Offset: 0x0001F0A4
		public List<string> AsStringList(bool nothrow = false)
		{
			bool flag = this.data is List<string>;
			List<string> result;
			if (flag)
			{
				result = (List<string>)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read " + this.data.GetType().Name + " json value as string list");
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00020F18 File Offset: 0x0001F118
		public Dictionary<string, JSONValue> AsDict(bool nothrow = false)
		{
			bool flag = this.data is Dictionary<string, JSONValue>;
			Dictionary<string, JSONValue> result;
			if (flag)
			{
				result = (Dictionary<string, JSONValue>)this.data;
			}
			else
			{
				bool flag2 = !nothrow;
				if (flag2)
				{
					throw new JSONTypeException("Tried to read non-dictionary json value as dictionary");
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00020F70 File Offset: 0x0001F170
		public static JSONValue NewString(string val)
		{
			return new JSONValue(val);
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00020F90 File Offset: 0x0001F190
		public static JSONValue NewFloat(float val)
		{
			return new JSONValue(val);
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00020FB4 File Offset: 0x0001F1B4
		public static JSONValue NewDict()
		{
			return new JSONValue(new Dictionary<string, JSONValue>());
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00020FD8 File Offset: 0x0001F1D8
		public static JSONValue NewList()
		{
			return new JSONValue(new List<JSONValue>());
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00020FFC File Offset: 0x0001F1FC
		public static JSONValue NewBool(bool val)
		{
			return new JSONValue(val);
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00021020 File Offset: 0x0001F220
		public static JSONValue NewNull()
		{
			return new JSONValue(null);
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00021040 File Offset: 0x0001F240
		public JSONValue InitList()
		{
			this.data = new List<JSONValue>();
			return this;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0002106C File Offset: 0x0001F26C
		public JSONValue InitDict()
		{
			this.data = new Dictionary<string, JSONValue>();
			return this;
		}

		// Token: 0x17000082 RID: 130
		public JSONValue this[string index]
		{
			get
			{
				Dictionary<string, JSONValue> dictionary = this.AsDict(false);
				return dictionary[index];
			}
			set
			{
				bool flag = this.data == null;
				if (flag)
				{
					this.data = new Dictionary<string, JSONValue>();
				}
				Dictionary<string, JSONValue> dictionary = this.AsDict(false);
				dictionary[index] = value;
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00021100 File Offset: 0x0001F300
		public bool ContainsKey(string index)
		{
			return this.IsDict() && this.AsDict(false).ContainsKey(index);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00021138 File Offset: 0x0001F338
		public JSONValue Get(string key, out bool found)
		{
			found = false;
			bool flag = !this.IsDict();
			JSONValue result;
			if (flag)
			{
				result = new JSONValue(null);
			}
			else
			{
				JSONValue jsonvalue = this;
				foreach (string index in key.Split(new char[]
				{
					'.'
				}))
				{
					bool flag2 = !jsonvalue.ContainsKey(index);
					if (flag2)
					{
						return new JSONValue(null);
					}
					jsonvalue = jsonvalue[index];
				}
				found = true;
				result = jsonvalue;
			}
			return result;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x000211DC File Offset: 0x0001F3DC
		public JSONValue Get(string key)
		{
			bool flag;
			return this.Get(key, out flag);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00021200 File Offset: 0x0001F400
		public bool Copy(string key, ref string dest)
		{
			return this.Copy(key, ref dest, true);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00021224 File Offset: 0x0001F424
		public bool Copy(string key, ref string dest, bool allowCopyNull)
		{
			bool flag;
			JSONValue jsonvalue = this.Get(key, out flag);
			bool flag2 = flag && (!jsonvalue.IsNull() || allowCopyNull);
			if (flag2)
			{
				dest = ((!jsonvalue.IsNull()) ? jsonvalue.AsString(false) : null);
			}
			return flag;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00021288 File Offset: 0x0001F488
		public bool Copy(string key, ref bool dest)
		{
			bool flag;
			JSONValue jsonvalue = this.Get(key, out flag);
			bool flag2 = flag && !jsonvalue.IsNull();
			if (flag2)
			{
				dest = jsonvalue.AsBool(false);
			}
			return flag;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x000212D8 File Offset: 0x0001F4D8
		public bool Copy(string key, ref int dest)
		{
			bool flag;
			JSONValue jsonvalue = this.Get(key, out flag);
			bool flag2 = flag && !jsonvalue.IsNull();
			if (flag2)
			{
				dest = (int)jsonvalue.AsFloat(false);
			}
			return flag;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00021328 File Offset: 0x0001F528
		public void Set(string key, string value)
		{
			this.Set(key, value, true);
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00021338 File Offset: 0x0001F538
		public void Set(string key, string value, bool allowNull)
		{
			bool flag = value != null;
			if (flag)
			{
				this[key] = JSONValue.NewString(value);
			}
			else
			{
				bool flag2 = !allowNull;
				if (!flag2)
				{
					this[key] = JSONValue.NewNull();
				}
			}
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00021388 File Offset: 0x0001F588
		public void Set(string key, float value)
		{
			this[key] = JSONValue.NewFloat(value);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0002139C File Offset: 0x0001F59C
		public void Set(string key, bool value)
		{
			this[key] = JSONValue.NewBool(value);
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000213B0 File Offset: 0x0001F5B0
		public void Add(string value)
		{
			List<JSONValue> list = this.AsList(false);
			bool flag = value == null;
			if (flag)
			{
				list.Add(JSONValue.NewNull());
			}
			else
			{
				list.Add(JSONValue.NewString(value));
			}
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000213F4 File Offset: 0x0001F5F4
		public void Add(float value)
		{
			List<JSONValue> list = this.AsList(false);
			list.Add(JSONValue.NewFloat(value));
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0002141C File Offset: 0x0001F61C
		public void Add(bool value)
		{
			List<JSONValue> list = this.AsList(false);
			list.Add(JSONValue.NewBool(value));
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00021444 File Offset: 0x0001F644
		public override string ToString()
		{
			return this.ToString(null, string.Empty);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0002146C File Offset: 0x0001F66C
		public string ToString(string curIndent, string indent)
		{
			bool flag = curIndent != null;
			bool flag2 = this.IsString();
			string result;
			if (flag2)
			{
				result = "\"" + JSONValue.EncodeString(this.AsString(false)) + "\"";
			}
			else
			{
				bool flag3 = this.IsFloat();
				if (flag3)
				{
					result = this.AsFloat(false).ToString();
				}
				else
				{
					bool flag4 = this.IsList();
					if (flag4)
					{
						string str = "[";
						string str2 = string.Empty;
						foreach (JSONValue jsonvalue in this.AsList(false))
						{
							str = str + str2 + jsonvalue.ToString();
							str2 = ",";
						}
						result = str + "]";
					}
					else
					{
						bool flag5 = this.IsStringList();
						if (flag5)
						{
							string text = "[";
							string text2 = string.Empty;
							foreach (string text3 in this.AsStringList(false))
							{
								text = string.Concat(new string[]
								{
									text,
									text2,
									"\"",
									text3,
									"\""
								});
								text2 = ",";
							}
							result = text + "]";
						}
						else
						{
							bool flag6 = this.IsDict();
							if (flag6)
							{
								string text4 = "{" + ((!flag) ? string.Empty : "\n");
								string text5 = string.Empty;
								foreach (KeyValuePair<string, JSONValue> keyValuePair in this.AsDict(false))
								{
									string text6 = text4;
									text4 = string.Concat(new object[]
									{
										text6,
										text5,
										curIndent,
										indent,
										'"',
										JSONValue.EncodeString(keyValuePair.Key),
										"\":",
										keyValuePair.Value.ToString(curIndent + indent, indent)
									});
									text5 = "," + ((!flag) ? string.Empty : "\n");
								}
								result = text4 + ((!flag) ? string.Empty : ("\n" + curIndent)) + "}";
							}
							else
							{
								bool flag7 = this.IsBool();
								if (flag7)
								{
									result = ((!this.AsBool(false)) ? "false" : "true");
								}
								else
								{
									bool flag8 = this.IsNull();
									if (!flag8)
									{
										throw new JSONTypeException("Cannot serialize json value of unknown type");
									}
									result = "null";
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000217A0 File Offset: 0x0001F9A0
		private static string EncodeString(string str)
		{
			str = str.Replace("\\", "\\\\");
			str = str.Replace("\"", "\\\"");
			str = str.Replace("/", "\\/");
			str = str.Replace("\b", "\\b");
			str = str.Replace("\f", "\\f");
			str = str.Replace("\n", "\\n");
			str = str.Replace("\r", "\\r");
			str = str.Replace("\t", "\\t");
			return str;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0002184C File Offset: 0x0001FA4C
		public static implicit operator JSONValue(string s)
		{
			return new JSONValue(s);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0002186C File Offset: 0x0001FA6C
		public static implicit operator string(JSONValue s)
		{
			return s.AsString(false);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00021890 File Offset: 0x0001FA90
		public static implicit operator JSONValue(float s)
		{
			return new JSONValue(s);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000218B4 File Offset: 0x0001FAB4
		public static implicit operator float(JSONValue s)
		{
			return s.AsFloat(false);
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000218D8 File Offset: 0x0001FAD8
		public static implicit operator JSONValue(bool s)
		{
			return new JSONValue(s);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000218FC File Offset: 0x0001FAFC
		public static implicit operator bool(JSONValue s)
		{
			return s.AsBool(false);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00021920 File Offset: 0x0001FB20
		public static implicit operator JSONValue(int s)
		{
			return new JSONValue((float)s);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00021948 File Offset: 0x0001FB48
		public static implicit operator int(JSONValue s)
		{
			return (int)s.AsFloat(false);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0002196C File Offset: 0x0001FB6C
		public static implicit operator JSONValue(List<JSONValue> s)
		{
			return new JSONValue(s);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0002198C File Offset: 0x0001FB8C
		public static implicit operator List<JSONValue>(JSONValue s)
		{
			return s.AsList(false);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000219B0 File Offset: 0x0001FBB0
		public static implicit operator Dictionary<string, JSONValue>(JSONValue s)
		{
			return s.AsDict(false);
		}

		// Token: 0x04000273 RID: 627
		private object data;
	}
}
