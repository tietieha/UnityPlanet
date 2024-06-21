using System;

namespace UWAEditor.Local
{
	// Token: 0x0200000C RID: 12
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00004ACE File Offset: 0x00002CCE
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004AF4 File Offset: 0x00002CF4
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004B1C File Offset: 0x00002D1C
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

		// Token: 0x1700001E RID: 30
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

		// Token: 0x1700001F RID: 31
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

		// Token: 0x06000096 RID: 150 RVA: 0x00004BE4 File Offset: 0x00002DE4
		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray
			{
				aItem
			});
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004C08 File Offset: 0x00002E08
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

		// Token: 0x06000098 RID: 152 RVA: 0x00004C30 File Offset: 0x00002E30
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			bool flag = b == null;
			return flag || a == b;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004C54 File Offset: 0x00002E54
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004C70 File Offset: 0x00002E70
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return flag || this == obj;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004C94 File Offset: 0x00002E94
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004CAC File Offset: 0x00002EAC
		public override string ToString()
		{
			return "";
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004CC4 File Offset: 0x00002EC4
		public override string ToString(string aPrefix)
		{
			return "";
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004CDC File Offset: 0x00002EDC
		public override string ToJSON(int prefix)
		{
			return "";
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004CF4 File Offset: 0x00002EF4
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00004D18 File Offset: 0x00002F18
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004D38 File Offset: 0x00002F38
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004D64 File Offset: 0x00002F64
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00004D84 File Offset: 0x00002F84
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00004DB8 File Offset: 0x00002FB8
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004DD8 File Offset: 0x00002FD8
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00004DFC File Offset: 0x00002FFC
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00004E1C File Offset: 0x0000301C
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00004E40 File Offset: 0x00003040
		public override JSONClass AsObject
		{
			get
			{
				JSONClass jsonclass = new JSONClass();
				this.Set(jsonclass);
				return jsonclass;
			}
		}

		// Token: 0x04000020 RID: 32
		private JSONNode m_Node = null;

		// Token: 0x04000021 RID: 33
		private string m_Key = null;
	}
}
