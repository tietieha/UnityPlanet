using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UWAEditor.Local
{
	// Token: 0x02000008 RID: 8
	public abstract class JSONNode
	{
		// Token: 0x0600002E RID: 46 RVA: 0x000031EA File Offset: 0x000013EA
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x17000008 RID: 8
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

		// Token: 0x17000009 RID: 9
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003218 File Offset: 0x00001418
		// (set) Token: 0x06000034 RID: 52 RVA: 0x000031EA File Offset: 0x000013EA
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003230 File Offset: 0x00001430
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003243 File Offset: 0x00001443
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003254 File Offset: 0x00001454
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003268 File Offset: 0x00001468
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000327C File Offset: 0x0000147C
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00003290 File Offset: 0x00001490
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000032B0 File Offset: 0x000014B0
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

		// Token: 0x0600003C RID: 60 RVA: 0x000032D0 File Offset: 0x000014D0
		public override string ToString()
		{
			return "JSONNode";
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000032E8 File Offset: 0x000014E8
		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		// Token: 0x0600003E RID: 62
		public abstract string ToJSON(int prefix);

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000032FF File Offset: 0x000014FF
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003307 File Offset: 0x00001507
		public virtual JSONBinaryTag Tag { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003310 File Offset: 0x00001510
		// (set) Token: 0x06000042 RID: 66 RVA: 0x0000333A File Offset: 0x0000153A
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003354 File Offset: 0x00001554
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00003386 File Offset: 0x00001586
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000033A0 File Offset: 0x000015A0
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000033DA File Offset: 0x000015DA
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000033F4 File Offset: 0x000015F4
		// (set) Token: 0x06000048 RID: 72 RVA: 0x0000342B File Offset: 0x0000162B
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000344C File Offset: 0x0000164C
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00003464 File Offset: 0x00001664
		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000347C File Offset: 0x0000167C
		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003494 File Offset: 0x00001694
		public static implicit operator string(JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000034B8 File Offset: 0x000016B8
		public static bool operator ==(JSONNode a, object b)
		{
			bool flag = b == null && a is JSONLazyCreator;
			return flag || a == b;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000034E8 File Offset: 0x000016E8
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003504 File Offset: 0x00001704
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000351C File Offset: 0x0000171C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003534 File Offset: 0x00001734
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
					goto IL_B6;
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
							goto IL_B6;
						}
						text += "\\\\";
					}
					else
					{
						text += "\\\"";
					}
					break;
				}
				IL_C6:
				i++;
				continue;
				IL_B6:
				text += c.ToString();
				goto IL_C6;
			}
			return text;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003620 File Offset: 0x00001820
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

		// Token: 0x06000053 RID: 83 RVA: 0x00003690 File Offset: 0x00001890
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

		// Token: 0x06000054 RID: 84 RVA: 0x000036FC File Offset: 0x000018FC
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
							goto IL_473;
						case '\v':
						case '\f':
							goto IL_45A;
						default:
							if (c2 != ' ')
							{
								goto IL_45A;
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
							goto IL_45A;
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
									goto IL_473;
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
								goto IL_473;
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
												goto IL_447;
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
											goto IL_447;
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
											goto IL_447;
										}
									}
									else
									{
										text += "\n";
									}
									goto IL_458;
									IL_447:
									text += c3.ToString();
								}
								IL_458:
								goto IL_473;
							}
							case ']':
								break;
							default:
								goto IL_45A;
							}
						}
						else
						{
							bool flag11 = flag;
							if (flag11)
							{
								text += aJSON[i].ToString();
								goto IL_473;
							}
							text2 = text;
							text = "";
							flag2 = false;
							goto IL_473;
						}
					}
					else if (c2 != '{')
					{
						if (c2 != '}')
						{
							goto IL_45A;
						}
					}
					else
					{
						bool flag12 = flag;
						if (flag12)
						{
							text += aJSON[i].ToString();
							goto IL_473;
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
						goto IL_473;
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
				IL_473:
				i++;
				continue;
				IL_45A:
				text += aJSON[i].ToString();
				goto IL_473;
			}
			bool flag20 = flag;
			if (flag20)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000031EA File Offset: 0x000013EA
		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003BB0 File Offset: 0x00001DB0
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003BCD File Offset: 0x00001DCD
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003BCD File Offset: 0x00001DCD
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003BCD File Offset: 0x00001DCD
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003BDC File Offset: 0x00001DDC
		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003C30 File Offset: 0x00001E30
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

		// Token: 0x0600005C RID: 92 RVA: 0x00003C80 File Offset: 0x00001E80
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

		// Token: 0x0600005D RID: 93 RVA: 0x00003BCD File Offset: 0x00001DCD
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003BCD File Offset: 0x00001DCD
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003BCD File Offset: 0x00001DCD
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003DBC File Offset: 0x00001FBC
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003DF8 File Offset: 0x00001FF8
		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003E34 File Offset: 0x00002034
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
