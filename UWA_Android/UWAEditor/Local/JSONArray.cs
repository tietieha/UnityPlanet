using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UWAEditor.Local
{
	// Token: 0x02000009 RID: 9
	public class JSONArray : JSONNode, IEnumerable
	{
		// Token: 0x17000015 RID: 21
		public override JSONNode this[int aIndex]
		{
			get
			{
				bool flag = aIndex < 0 || aIndex >= this.m_List.Count;
				JSONNode result;
				if (flag)
				{
					result = new JSONLazyCreator(this);
				}
				else
				{
					result = this.m_List[aIndex];
				}
				return result;
			}
			set
			{
				bool flag = aIndex < 0 || aIndex >= this.m_List.Count;
				if (flag)
				{
					this.m_List.Add(value);
				}
				else
				{
					this.m_List[aIndex] = value;
				}
			}
		}

		// Token: 0x17000016 RID: 22
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.m_List.Add(value);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003F18 File Offset: 0x00002118
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003F08 File Offset: 0x00002108
		public override void Add(string aKey, JSONNode aItem)
		{
			this.m_List.Add(aItem);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003F38 File Offset: 0x00002138
		public override JSONNode Remove(int aIndex)
		{
			bool flag = aIndex < 0 || aIndex >= this.m_List.Count;
			JSONNode result;
			if (flag)
			{
				result = null;
			}
			else
			{
				JSONNode jsonnode = this.m_List[aIndex];
				this.m_List.RemoveAt(aIndex);
				result = jsonnode;
			}
			return result;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003F88 File Offset: 0x00002188
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00003FA8 File Offset: 0x000021A8
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (JSONNode N in this.m_List)
				{
					yield return N;
					N = null;
				}
				List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003FC7 File Offset: 0x000021C7
		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode N in this.m_List)
			{
				yield return N;
				N = null;
			}
			List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003FD8 File Offset: 0x000021D8
		public override string ToString()
		{
			string text = "[ ";
			foreach (JSONNode jsonnode in this.m_List)
			{
				bool flag = text.Length > 2;
				if (flag)
				{
					text += ", ";
				}
				text += jsonnode.ToString();
			}
			text += " ]";
			return text;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004068 File Offset: 0x00002268
		public override string ToString(string aPrefix)
		{
			string text = "[ ";
			foreach (JSONNode jsonnode in this.m_List)
			{
				bool flag = text.Length > 3;
				if (flag)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text += jsonnode.ToString(aPrefix + "   ");
			}
			text = text + "\n" + aPrefix + "]";
			return text;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000411C File Offset: 0x0000231C
		public override string ToJSON(int prefix)
		{
			string str = new string(' ', (prefix + 1) * 2);
			string text = "[ ";
			foreach (JSONNode jsonnode in this.m_List)
			{
				bool flag = text.Length > 3;
				if (flag)
				{
					text += ", ";
				}
				text = text + "\n" + str;
				text += jsonnode.ToJSON(prefix + 1);
			}
			text = text + "\n" + str + "]";
			return text;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000041D4 File Offset: 0x000023D4
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		// Token: 0x0400001D RID: 29
		private List<JSONNode> m_List = new List<JSONNode>();
	}
}
