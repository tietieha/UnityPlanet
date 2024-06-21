using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.Core
{
	// Token: 0x02000060 RID: 96
	[ComVisible(false)]
	public class JSONArray : JSONNode, IEnumerable
	{
		// Token: 0x170000A2 RID: 162
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

		// Token: 0x170000A3 RID: 163
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

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00026D28 File Offset: 0x00024F28
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00026D4C File Offset: 0x00024F4C
		public override void Add(string aKey, JSONNode aItem)
		{
			this.m_List.Add(aItem);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00026D5C File Offset: 0x00024F5C
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

		// Token: 0x0600046A RID: 1130 RVA: 0x00026DBC File Offset: 0x00024FBC
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00026DE4 File Offset: 0x00024FE4
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

		// Token: 0x0600046C RID: 1132 RVA: 0x00026E08 File Offset: 0x00025008
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

		// Token: 0x0600046D RID: 1133 RVA: 0x00026E18 File Offset: 0x00025018
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

		// Token: 0x0600046E RID: 1134 RVA: 0x00026EB4 File Offset: 0x000250B4
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

		// Token: 0x0600046F RID: 1135 RVA: 0x00026F74 File Offset: 0x00025174
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

		// Token: 0x06000470 RID: 1136 RVA: 0x00027038 File Offset: 0x00025238
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		// Token: 0x040002B3 RID: 691
		private List<JSONNode> m_List = new List<JSONNode>();
	}
}
