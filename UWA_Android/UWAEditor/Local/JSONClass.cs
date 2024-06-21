using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UWAEditor.Local
{
	// Token: 0x0200000A RID: 10
	public class JSONClass : JSONNode, IEnumerable
	{
		// Token: 0x17000019 RID: 25
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

		// Token: 0x1700001A RID: 26
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000434C File Offset: 0x0000254C
		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000436C File Offset: 0x0000256C
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

		// Token: 0x06000079 RID: 121 RVA: 0x000043DC File Offset: 0x000025DC
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

		// Token: 0x0600007A RID: 122 RVA: 0x00004420 File Offset: 0x00002620
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

		// Token: 0x0600007B RID: 123 RVA: 0x0000447C File Offset: 0x0000267C
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007C RID: 124 RVA: 0x000044E8 File Offset: 0x000026E8
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

		// Token: 0x0600007D RID: 125 RVA: 0x00004507 File Offset: 0x00002707
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

		// Token: 0x0600007E RID: 126 RVA: 0x00004518 File Offset: 0x00002718
		public bool Contains(string key)
		{
			return this.m_Dict.ContainsKey(key);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004538 File Offset: 0x00002738
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

		// Token: 0x06000080 RID: 128 RVA: 0x000045FC File Offset: 0x000027FC
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

		// Token: 0x06000081 RID: 129 RVA: 0x000046E4 File Offset: 0x000028E4
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

		// Token: 0x06000082 RID: 130 RVA: 0x000047B0 File Offset: 0x000029B0
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

		// Token: 0x0400001E RID: 30
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
	}
}
