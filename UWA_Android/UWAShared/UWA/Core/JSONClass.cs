using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace UWA.Core
{
	// Token: 0x02000061 RID: 97
	[ComVisible(false)]
	public class JSONClass : JSONNode, IEnumerable
	{
		// Token: 0x170000A6 RID: 166
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

		// Token: 0x170000A7 RID: 167
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

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x000271F8 File Offset: 0x000253F8
		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0002721C File Offset: 0x0002541C
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

		// Token: 0x06000478 RID: 1144 RVA: 0x0002729C File Offset: 0x0002549C
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

		// Token: 0x06000479 RID: 1145 RVA: 0x000272F0 File Offset: 0x000254F0
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

		// Token: 0x0600047A RID: 1146 RVA: 0x0002735C File Offset: 0x0002555C
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

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x000273D0 File Offset: 0x000255D0
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

		// Token: 0x0600047C RID: 1148 RVA: 0x000273F4 File Offset: 0x000255F4
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

		// Token: 0x0600047D RID: 1149 RVA: 0x00027404 File Offset: 0x00025604
		public bool Contains(string key)
		{
			return this.m_Dict.ContainsKey(key);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0002742C File Offset: 0x0002562C
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

		// Token: 0x0600047F RID: 1151 RVA: 0x000274FC File Offset: 0x000256FC
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

		// Token: 0x06000480 RID: 1152 RVA: 0x000275F0 File Offset: 0x000257F0
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

		// Token: 0x06000481 RID: 1153 RVA: 0x000276C8 File Offset: 0x000258C8
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

		// Token: 0x040002B4 RID: 692
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
	}
}
