using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UWAShared
{
	// Token: 0x02000043 RID: 67
	public abstract class JSONNode
	{
		// Token: 0x060002A7 RID: 679 RVA: 0x00019718 File Offset: 0x00017918
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x1700005F RID: 95
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000060 RID: 96
		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0001975C File Offset: 0x0001795C
		// (set) Token: 0x060002AD RID: 685 RVA: 0x0001977C File Offset: 0x0001797C
		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00019780 File Offset: 0x00017980
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0001979C File Offset: 0x0001799C
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x000197AC File Offset: 0x000179AC
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x000197C8 File Offset: 0x000179C8
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000197E4 File Offset: 0x000179E4
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x00019800 File Offset: 0x00017A00
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x00019824 File Offset: 0x00017A24
		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode C in this.Children)
				{
					foreach (JSONNode D in C.DeepChildren)
					{
						yield return D;
						D = null;
					}
					IEnumerator<JSONNode> enumerator2 = null;
					C = null;
				}
				IEnumerator<JSONNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00019848 File Offset: 0x00017A48
		public override string ToString()
		{
			return "JSONNode";
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00019868 File Offset: 0x00017A68
		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		// Token: 0x060002B7 RID: 695
		public abstract string ToJSON(int prefix);

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00019888 File Offset: 0x00017A88
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x00019890 File Offset: 0x00017A90
		public virtual JSONBinaryTag Tag { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002BA RID: 698 RVA: 0x0001989C File Offset: 0x00017A9C
		// (set) Token: 0x060002BB RID: 699 RVA: 0x000198D4 File Offset: 0x00017AD4
		public virtual int AsInt
		{
			get
			{
				int num = 0;
				bool flag = int.TryParse(this.Value, out num);
				int result;
				if (flag)
				{
					result = num;
				}
				else
				{
					result = 0;
				}
				return result;
			}
			set
			{
				this.Value = value.ToString();
				this.Tag = JSONBinaryTag.IntValue;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002BC RID: 700 RVA: 0x000198F0 File Offset: 0x00017AF0
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00019930 File Offset: 0x00017B30
		public virtual float AsFloat
		{
			get
			{
				float num = 0f;
				bool flag = float.TryParse(this.Value, out num);
				float result;
				if (flag)
				{
					result = num;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
			set
			{
				this.Value = value.ToString();
				this.Tag = JSONBinaryTag.FloatValue;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0001994C File Offset: 0x00017B4C
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00019994 File Offset: 0x00017B94
		public virtual double AsDouble
		{
			get
			{
				double num = 0.0;
				bool flag = double.TryParse(this.Value, out num);
				double result;
				if (flag)
				{
					result = num;
				}
				else
				{
					result = 0.0;
				}
				return result;
			}
			set
			{
				this.Value = value.ToString();
				this.Tag = JSONBinaryTag.DoubleValue;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x000199B0 File Offset: 0x00017BB0
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x000199F4 File Offset: 0x00017BF4
		public virtual bool AsBool
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(this.Value, out flag);
				bool result;
				if (flag2)
				{
					result = flag;
				}
				else
				{
					result = !string.IsNullOrEmpty(this.Value);
				}
				return result;
			}
			set
			{
				this.Value = (value ? "true" : "false");
				this.Tag = JSONBinaryTag.BoolValue;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x00019A1C File Offset: 0x00017C1C
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00019A3C File Offset: 0x00017C3C
		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00019A5C File Offset: 0x00017C5C
		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00019A7C File Offset: 0x00017C7C
		public static implicit operator string(JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00019AB0 File Offset: 0x00017CB0
		public static bool operator ==(JSONNode a, object b)
		{
			bool flag = b == null && a is JSONLazyCreator;
			return flag || a == b;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00019AF0 File Offset: 0x00017CF0
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00019B14 File Offset: 0x00017D14
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00019B34 File Offset: 0x00017D34
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00019B54 File Offset: 0x00017D54
		internal static string Escape(string aText)
		{
			string text = "";
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText[i];
				char c2 = c;
				char c3 = c2;
				switch (c3)
				{
				case '\b':
					text += "\\b";
					break;
				case '\t':
					text += "\\t";
					break;
				case '\n':
					text += "\\n";
					break;
				case '\v':
					goto IL_DA;
				case '\f':
					text += "\\f";
					break;
				case '\r':
					text += "\\r";
					break;
				default:
					if (c3 != '"')
					{
						if (c3 != '\\')
						{
							goto IL_DA;
						}
						text += "\\\\";
					}
					else
					{
						text += "\\\"";
					}
					break;
				}
				IL_ED:
				i++;
				continue;
				IL_DA:
				text += c.ToString();
				goto IL_ED;
			}
			return text;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00019C70 File Offset: 0x00017E70
		private static JSONData Numberize(string token)
		{
			bool aData = false;
			int aData2 = 0;
			double aData3 = 0.0;
			bool flag = int.TryParse(token, out aData2);
			JSONData result;
			if (flag)
			{
				result = new JSONData(aData2);
			}
			else
			{
				bool flag2 = double.TryParse(token, out aData3);
				if (flag2)
				{
					result = new JSONData(aData3);
				}
				else
				{
					bool flag3 = bool.TryParse(token, out aData);
					if (!flag3)
					{
						throw new NotImplementedException(token);
					}
					result = new JSONData(aData);
				}
			}
			return result;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00019CF4 File Offset: 0x00017EF4
		private static void AddElement(JSONNode ctx, string token, string tokenName, bool tokenIsString)
		{
			if (tokenIsString)
			{
				bool flag = ctx is JSONArray;
				if (flag)
				{
					ctx.Add(token);
				}
				else
				{
					ctx.Add(tokenName, token);
				}
			}
			else
			{
				JSONData aItem = JSONNode.Numberize(token);
				bool flag2 = ctx is JSONArray;
				if (flag2)
				{
					ctx.Add(aItem);
				}
				else
				{
					ctx.Add(tokenName, aItem);
				}
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00019D74 File Offset: 0x00017F74
		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			string text = "";
			string text2 = "";
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				char c2 = c;
				if (c2 <= ',')
				{
					if (c2 <= ' ')
					{
						switch (c2)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							goto IL_4F7;
						case '\v':
						case '\f':
							goto IL_4DB;
						default:
							if (c2 != ' ')
							{
								goto IL_4DB;
							}
							break;
						}
						bool flag3 = flag;
						if (flag3)
						{
							text += aJSON[i].ToString();
						}
					}
					else if (c2 != '"')
					{
						if (c2 != ',')
						{
							goto IL_4DB;
						}
						bool flag4 = flag;
						if (flag4)
						{
							text += aJSON[i].ToString();
						}
						else
						{
							bool flag5 = text != "";
							if (flag5)
							{
								JSONNode.AddElement(jsonnode, text, text2, flag2);
							}
							text2 = "";
							text = "";
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = (flag || flag2);
					}
				}
				else
				{
					if (c2 <= ']')
					{
						if (c2 != ':')
						{
							switch (c2)
							{
							case '[':
							{
								bool flag6 = flag;
								if (flag6)
								{
									text += aJSON[i].ToString();
									goto IL_4F7;
								}
								stack.Push(new JSONArray());
								bool flag7 = jsonnode != null;
								if (flag7)
								{
									text2 = text2.Trim();
									bool flag8 = jsonnode is JSONArray;
									if (flag8)
									{
										jsonnode.Add(stack.Peek());
									}
									else
									{
										bool flag9 = text2 != "";
										if (flag9)
										{
											jsonnode.Add(text2, stack.Peek());
										}
									}
								}
								text2 = "";
								text = "";
								jsonnode = stack.Peek();
								goto IL_4F7;
							}
							case '\\':
							{
								i++;
								bool flag10 = flag;
								if (flag10)
								{
									char c3 = aJSON[i];
									char c4 = c3;
									char c5 = c4;
									if (c5 <= 'f')
									{
										if (c5 != 'b')
										{
											if (c5 != 'f')
											{
												goto IL_4C2;
											}
											text += "\f";
										}
										else
										{
											text += "\b";
										}
									}
									else if (c5 != 'n')
									{
										switch (c5)
										{
										case 'r':
											text += "\r";
											break;
										case 's':
											goto IL_4C2;
										case 't':
											text += "\t";
											break;
										case 'u':
										{
											string s = aJSON.Substring(i + 1, 4);
											text += ((char)int.Parse(s, NumberStyles.AllowHexSpecifier)).ToString();
											i += 4;
											break;
										}
										default:
											goto IL_4C2;
										}
									}
									else
									{
										text += "\n";
									}
									goto IL_4D6;
									IL_4C2:
									text += c3.ToString();
								}
								IL_4D6:
								goto IL_4F7;
							}
							case ']':
								break;
							default:
								goto IL_4DB;
							}
						}
						else
						{
							bool flag11 = flag;
							if (flag11)
							{
								text += aJSON[i].ToString();
								goto IL_4F7;
							}
							text2 = text;
							text = "";
							flag2 = false;
							goto IL_4F7;
						}
					}
					else if (c2 != '{')
					{
						if (c2 != '}')
						{
							goto IL_4DB;
						}
					}
					else
					{
						bool flag12 = flag;
						if (flag12)
						{
							text += aJSON[i].ToString();
							goto IL_4F7;
						}
						stack.Push(new JSONClass());
						bool flag13 = jsonnode != null;
						if (flag13)
						{
							text2 = text2.Trim();
							bool flag14 = jsonnode is JSONArray;
							if (flag14)
							{
								jsonnode.Add(stack.Peek());
							}
							else
							{
								bool flag15 = text2 != "";
								if (flag15)
								{
									jsonnode.Add(text2, stack.Peek());
								}
							}
						}
						text2 = "";
						text = "";
						jsonnode = stack.Peek();
						goto IL_4F7;
					}
					bool flag16 = flag;
					if (flag16)
					{
						text += aJSON[i].ToString();
					}
					else
					{
						bool flag17 = stack.Count == 0;
						if (flag17)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						bool flag18 = text != "";
						if (flag18)
						{
							text2 = text2.Trim();
							JSONNode.AddElement(jsonnode, text, text2, flag2);
							flag2 = false;
						}
						text2 = "";
						text = "";
						bool flag19 = stack.Count > 0;
						if (flag19)
						{
							jsonnode = stack.Peek();
						}
					}
				}
				IL_4F7:
				i++;
				continue;
				IL_4DB:
				text += aJSON[i].ToString();
				goto IL_4F7;
			}
			bool flag20 = flag;
			if (flag20)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0001A2B4 File Offset: 0x000184B4
		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0001A2B8 File Offset: 0x000184B8
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001A2DC File Offset: 0x000184DC
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001A2EC File Offset: 0x000184EC
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001A2FC File Offset: 0x000184FC
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0001A30C File Offset: 0x0001850C
		public void SaveToFile(string aFileName)
		{
			throw new Exception("Can't use File IO stuff in webplayer");
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0001A31C File Offset: 0x0001851C
		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001A374 File Offset: 0x00018574
		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jsonbinaryTag = (JSONBinaryTag)aReader.ReadByte();
			JSONNode result;
			switch (jsonbinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				int num = aReader.ReadInt32();
				JSONArray jsonarray = new JSONArray();
				for (int i = 0; i < num; i++)
				{
					jsonarray.Add(JSONNode.Deserialize(aReader));
				}
				result = jsonarray;
				break;
			}
			case JSONBinaryTag.Class:
			{
				int num2 = aReader.ReadInt32();
				JSONClass jsonclass = new JSONClass();
				for (int j = 0; j < num2; j++)
				{
					string aKey = aReader.ReadString();
					JSONNode aItem = JSONNode.Deserialize(aReader);
					jsonclass.Add(aKey, aItem);
				}
				result = jsonclass;
				break;
			}
			case JSONBinaryTag.Value:
				result = new JSONData(aReader.ReadString());
				break;
			case JSONBinaryTag.IntValue:
				result = new JSONData(aReader.ReadInt32());
				break;
			case JSONBinaryTag.DoubleValue:
				result = new JSONData(aReader.ReadDouble());
				break;
			case JSONBinaryTag.BoolValue:
				result = new JSONData(aReader.ReadBoolean());
				break;
			case JSONBinaryTag.FloatValue:
				result = new JSONData(aReader.ReadSingle());
				break;
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jsonbinaryTag.ToString());
			}
			return result;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001A4CC File Offset: 0x000186CC
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001A4DC File Offset: 0x000186DC
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001A4EC File Offset: 0x000186EC
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0001A4FC File Offset: 0x000186FC
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0001A540 File Offset: 0x00018740
		public static JSONNode LoadFromFile(string aFileName)
		{
			throw new Exception("Can't use File IO stuff in webplayer");
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001A550 File Offset: 0x00018750
		public static JSONNode LoadFromBase64(string aBase64)
		{
			byte[] buffer = Convert.FromBase64String(aBase64);
			return JSONNode.LoadFromStream(new MemoryStream(buffer)
			{
				Position = 0L
			});
		}
	}
}
