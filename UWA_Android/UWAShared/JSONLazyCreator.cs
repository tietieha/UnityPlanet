using System;

namespace UWAShared
{
	// Token: 0x02000047 RID: 71
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x06000308 RID: 776 RVA: 0x0001B37C File Offset: 0x0001957C
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001B3A4 File Offset: 0x000195A4
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001B3CC File Offset: 0x000195CC
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

		// Token: 0x17000075 RID: 117
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

		// Token: 0x17000076 RID: 118
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

		// Token: 0x0600030F RID: 783 RVA: 0x0001B4B4 File Offset: 0x000196B4
		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray
			{
				aItem
			});
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0001B4DC File Offset: 0x000196DC
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

		// Token: 0x06000311 RID: 785 RVA: 0x0001B508 File Offset: 0x00019708
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			bool flag = b == null;
			return flag || a == b;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001B538 File Offset: 0x00019738
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001B55C File Offset: 0x0001975C
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return flag || this == obj;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001B58C File Offset: 0x0001978C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001B5AC File Offset: 0x000197AC
		public override string ToString()
		{
			return "";
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001B5CC File Offset: 0x000197CC
		public override string ToString(string aPrefix)
		{
			return "";
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001B5EC File Offset: 0x000197EC
		public override string ToJSON(int prefix)
		{
			return "";
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000318 RID: 792 RVA: 0x0001B60C File Offset: 0x0001980C
		// (set) Token: 0x06000319 RID: 793 RVA: 0x0001B638 File Offset: 0x00019838
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

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0001B65C File Offset: 0x0001985C
		// (set) Token: 0x0600031B RID: 795 RVA: 0x0001B690 File Offset: 0x00019890
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

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0001B6B4 File Offset: 0x000198B4
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0001B6F0 File Offset: 0x000198F0
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0001B714 File Offset: 0x00019914
		// (set) Token: 0x0600031F RID: 799 RVA: 0x0001B740 File Offset: 0x00019940
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

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0001B764 File Offset: 0x00019964
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0001B78C File Offset: 0x0001998C
		public override JSONClass AsObject
		{
			get
			{
				JSONClass jsonclass = new JSONClass();
				this.Set(jsonclass);
				return jsonclass;
			}
		}

		// Token: 0x04000229 RID: 553
		private JSONNode m_Node = null;

		// Token: 0x0400022A RID: 554
		private string m_Key = null;
	}
}
