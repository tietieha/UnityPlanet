using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UWAShared
{
	// Token: 0x02000045 RID: 69
	public class JSONClass : JSONNode, IEnumerable
	{
		// Token: 0x17000070 RID: 112
		public override JSONNode this[string aKey]
		{
			get
			{
				bool flag = this.m_Dict.ContainsKey(aKey);
				JSONNode result;
				if (flag)
				{
					result = this.m_Dict[aKey];
				}
				else
				{
					result = new JSONLazyCreator(this, aKey);
				}
				return result;
			}
			set
			{
				bool flag = this.m_Dict.ContainsKey(aKey);
				if (flag)
				{
					this.m_Dict[aKey] = value;
				}
				else
				{
					this.m_Dict.Add(aKey, value);
				}
			}
		}

		// Token: 0x17000071 RID: 113
		public override JSONNode this[int aIndex]
		{
			get
			{
				bool flag = aIndex < 0 || aIndex >= this.m_Dict.Count;
				JSONNode result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.m_Dict.ElementAt(aIndex).Value;
				}
				return result;
			}
			set
			{
				bool flag = aIndex < 0 || aIndex >= this.m_Dict.Count;
				if (!flag)
				{
					string key = this.m_Dict.ElementAt(aIndex).Key;
					this.m_Dict[key] = value;
				}
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x0001AB44 File Offset: 0x00018D44
		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001AB68 File Offset: 0x00018D68
		public override void Add(string aKey, JSONNode aItem)
		{
			bool flag = !string.IsNullOrEmpty(aKey);
			if (flag)
			{
				bool flag2 = this.m_Dict.ContainsKey(aKey);
				if (flag2)
				{
					this.m_Dict[aKey] = aItem;
				}
				else
				{
					this.m_Dict.Add(aKey, aItem);
				}
			}
			else
			{
				this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0001ABE8 File Offset: 0x00018DE8
		public override JSONNode Remove(string aKey)
		{
			bool flag = !this.m_Dict.ContainsKey(aKey);
			JSONNode result;
			if (flag)
			{
				result = null;
			}
			else
			{
				JSONNode jsonnode = this.m_Dict[aKey];
				this.m_Dict.Remove(aKey);
				result = jsonnode;
			}
			return result;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0001AC3C File Offset: 0x00018E3C
		public override JSONNode Remove(int aIndex)
		{
			bool flag = aIndex < 0 || aIndex >= this.m_Dict.Count;
			JSONNode result;
			if (flag)
			{
				result = null;
			}
			else
			{
				KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.ElementAt(aIndex);
				this.m_Dict.Remove(keyValuePair.Key);
				result = keyValuePair.Value;
			}
			return result;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0001ACA8 File Offset: 0x00018EA8
		public override JSONNode Remove(JSONNode aNode)
		{
			JSONNode result;
			try
			{
				KeyValuePair<string, JSONNode> keyValuePair = (from k in this.m_Dict
				where k.Value == aNode
				select k).First<KeyValuePair<string, JSONNode>>();
				this.m_Dict.Remove(keyValuePair.Key);
				result = aNode;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x0001AD1C File Offset: 0x00018F1C
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> N in this.m_Dict)
				{
					yield return N.Value;
					N = default(KeyValuePair<string, JSONNode>);
				}
				Dictionary<string, JSONNode>.Enumerator enumerator = default(Dictionary<string, JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0001AD40 File Offset: 0x00018F40
		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, JSONNode> N in this.m_Dict)
			{
				yield return N;
				N = default(KeyValuePair<string, JSONNode>);
			}
			Dictionary<string, JSONNode>.Enumerator enumerator = default(Dictionary<string, JSONNode>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0001AD50 File Offset: 0x00018F50
		public bool Contains(string key)
		{
			return this.m_Dict.ContainsKey(key);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0001AD78 File Offset: 0x00018F78
		public override string ToString()
		{
			string text = "{";
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				bool flag = text.Length > 2;
				if (flag)
				{
					text += ", ";
				}
				text = string.Concat(new string[]
				{
					text,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\":",
					keyValuePair.Value.ToString()
				});
			}
			text += "}";
			return text;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0001AE48 File Offset: 0x00019048
		public override string ToString(string aPrefix)
		{
			string text = "{ ";
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				bool flag = text.Length > 3;
				if (flag)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text = string.Concat(new string[]
				{
					text,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\" : ",
					keyValuePair.Value.ToString(aPrefix + "   ")
				});
			}
			text = text + "\n" + aPrefix + "}";
			return text;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001AF3C File Offset: 0x0001913C
		public override string ToJSON(int prefix)
		{
			string str = new string(' ', (prefix + 1) * 2);
			string text = "{ ";
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				bool flag = text.Length > 3;
				if (flag)
				{
					text += ", ";
				}
				text = text + "\n" + str;
				text += string.Format("\"{0}\": {1}", keyValuePair.Key, keyValuePair.Value.ToJSON(prefix + 1));
			}
			text = text + "\n" + str + "}";
			return text;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0001B014 File Offset: 0x00019214
		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(2);
			aWriter.Write(this.m_Dict.Count);
			foreach (string text in this.m_Dict.Keys)
			{
				aWriter.Write(text);
				this.m_Dict[text].Serialize(aWriter);
			}
		}

		// Token: 0x04000227 RID: 551
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
	}
}
