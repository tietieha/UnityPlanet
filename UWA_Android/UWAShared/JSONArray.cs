using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UWAShared
{
	// Token: 0x02000044 RID: 68
	public class JSONArray : JSONNode, IEnumerable
	{
		// Token: 0x1700006C RID: 108
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

		// Token: 0x1700006D RID: 109
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

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0001A674 File Offset: 0x00018874
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0001A698 File Offset: 0x00018898
		public override void Add(string aKey, JSONNode aItem)
		{
			this.m_List.Add(aItem);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0001A6A8 File Offset: 0x000188A8
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

		// Token: 0x060002E4 RID: 740 RVA: 0x0001A708 File Offset: 0x00018908
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0001A730 File Offset: 0x00018930
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

		// Token: 0x060002E6 RID: 742 RVA: 0x0001A754 File Offset: 0x00018954
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

		// Token: 0x060002E7 RID: 743 RVA: 0x0001A764 File Offset: 0x00018964
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

		// Token: 0x060002E8 RID: 744 RVA: 0x0001A800 File Offset: 0x00018A00
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

		// Token: 0x060002E9 RID: 745 RVA: 0x0001A8C0 File Offset: 0x00018AC0
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

		// Token: 0x060002EA RID: 746 RVA: 0x0001A984 File Offset: 0x00018B84
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		// Token: 0x04000226 RID: 550
		private List<JSONNode> m_List = new List<JSONNode>();
	}
}
