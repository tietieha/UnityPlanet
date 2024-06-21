using System;

namespace UWA.Core
{
	// Token: 0x02000063 RID: 99
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x0600048E RID: 1166 RVA: 0x00027A34 File Offset: 0x00025C34
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00027A5C File Offset: 0x00025C5C
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00027A84 File Offset: 0x00025C84
		private void Set(JSONNode aVal)
		{
			bool flag = this.m_Key == null;
			if (flag)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
		}

		// Token: 0x170000AB RID: 171
		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.Set(new JSONArray
				{
					value
				});
			}
		}

		// Token: 0x170000AC RID: 172
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				this.Set(new JSONClass
				{
					{
						aKey,
						value
					}
				});
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x00027B6C File Offset: 0x00025D6C
		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray
			{
				aItem
			});
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00027B94 File Offset: 0x00025D94
		public override void Add(string aKey, JSONNode aItem)
		{
			this.Set(new JSONClass
			{
				{
					aKey,
					aItem
				}
			});
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x00027BC0 File Offset: 0x00025DC0
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			bool flag = b == null;
			return flag || a == b;
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00027BF0 File Offset: 0x00025DF0
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00027C14 File Offset: 0x00025E14
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return flag || this == obj;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00027C44 File Offset: 0x00025E44
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00027C64 File Offset: 0x00025E64
		public override string ToString()
		{
			return "";
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00027C84 File Offset: 0x00025E84
		public override string ToString(string aPrefix)
		{
			return "";
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00027CA4 File Offset: 0x00025EA4
		public override string ToJSON(int prefix)
		{
			return "";
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x00027CC4 File Offset: 0x00025EC4
		// (set) Token: 0x0600049F RID: 1183 RVA: 0x00027CF0 File Offset: 0x00025EF0
		public override int AsInt
		{
			get
			{
				JSONData aVal = new JSONData(0);
				this.Set(aVal);
				return 0;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				this.Set(aVal);
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00027D14 File Offset: 0x00025F14
		// (set) Token: 0x060004A1 RID: 1185 RVA: 0x00027D48 File Offset: 0x00025F48
		public override float AsFloat
		{
			get
			{
				JSONData aVal = new JSONData(0f);
				this.Set(aVal);
				return 0f;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				this.Set(aVal);
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x00027D6C File Offset: 0x00025F6C
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00027DA8 File Offset: 0x00025FA8
		public override double AsDouble
		{
			get
			{
				JSONData aVal = new JSONData(0.0);
				this.Set(aVal);
				return 0.0;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				this.Set(aVal);
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x00027DCC File Offset: 0x00025FCC
		// (set) Token: 0x060004A5 RID: 1189 RVA: 0x00027DF8 File Offset: 0x00025FF8
		public override bool AsBool
		{
			get
			{
				JSONData aVal = new JSONData(false);
				this.Set(aVal);
				return false;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				this.Set(aVal);
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00027E1C File Offset: 0x0002601C
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00027E44 File Offset: 0x00026044
		public override JSONClass AsObject
		{
			get
			{
				JSONClass jsonclass = new JSONClass();
				this.Set(jsonclass);
				return jsonclass;
			}
		}

		// Token: 0x040002B6 RID: 694
		private JSONNode m_Node = null;

		// Token: 0x040002B7 RID: 695
		private string m_Key = null;
	}
}
