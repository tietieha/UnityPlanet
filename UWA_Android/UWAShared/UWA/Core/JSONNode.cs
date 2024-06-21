using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.Core
{
	// Token: 0x0200005F RID: 95
	[ComVisible(false)]
	public abstract class JSONNode
	{
		// Token: 0x0600042D RID: 1069 RVA: 0x00025D50 File Offset: 0x00023F50
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x17000095 RID: 149
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

		// Token: 0x17000096 RID: 150
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

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x00025D94 File Offset: 0x00023F94
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x00025DB4 File Offset: 0x00023FB4
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

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00025DB8 File Offset: 0x00023FB8
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00025DD4 File Offset: 0x00023FD4
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00025DE4 File Offset: 0x00023FE4
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00025E00 File Offset: 0x00024000
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00025E1C File Offset: 0x0002401C
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x00025E38 File Offset: 0x00024038
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00025E5C File Offset: 0x0002405C
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

		// Token: 0x0600043B RID: 1083 RVA: 0x00025E80 File Offset: 0x00024080
		public override string ToString()
		{
			return "JSONNode";
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00025EA0 File Offset: 0x000240A0
		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		// Token: 0x0600043D RID: 1085
		public abstract string ToJSON(int prefix);

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00025EC0 File Offset: 0x000240C0
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x00025EC8 File Offset: 0x000240C8
		public virtual JSONBinaryTag Tag { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00025ED4 File Offset: 0x000240D4
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00025F0C File Offset: 0x0002410C
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

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00025F28 File Offset: 0x00024128
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00025F68 File Offset: 0x00024168
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

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00025F84 File Offset: 0x00024184
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x00025FCC File Offset: 0x000241CC
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

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00025FE8 File Offset: 0x000241E8
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0002602C File Offset: 0x0002422C
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

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00026054 File Offset: 0x00024254
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x00026074 File Offset: 0x00024274
		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00026094 File Offset: 0x00024294
		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x000260B4 File Offset: 0x000242B4
		public static implicit operator string(JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x000260E8 File Offset: 0x000242E8
		public static bool operator ==(JSONNode a, object b)
		{
			bool flag = b == null && a is JSONLazyCreator;
			return flag || a == b;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00026128 File Offset: 0x00024328
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0002614C File Offset: 0x0002434C
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0002616C File Offset: 0x0002436C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0002618C File Offset: 0x0002438C
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

		// Token: 0x06000451 RID: 1105 RVA: 0x000262A8 File Offset: 0x000244A8
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

		// Token: 0x06000452 RID: 1106 RVA: 0x0002632C File Offset: 0x0002452C
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

		// Token: 0x06000453 RID: 1107 RVA: 0x000263AC File Offset: 0x000245AC
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

		// Token: 0x06000454 RID: 1108 RVA: 0x000268EC File Offset: 0x00024AEC
		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x000268F0 File Offset: 0x00024AF0
		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00026914 File Offset: 0x00024B14
		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00026924 File Offset: 0x00024B24
		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00026934 File Offset: 0x00024B34
		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00026944 File Offset: 0x00024B44
		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0002699C File Offset: 0x00024B9C
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

		// Token: 0x0600045B RID: 1115 RVA: 0x000269F4 File Offset: 0x00024BF4
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

		// Token: 0x0600045C RID: 1116 RVA: 0x00026B4C File Offset: 0x00024D4C
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00026B5C File Offset: 0x00024D5C
		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00026B6C File Offset: 0x00024D6C
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00026B7C File Offset: 0x00024D7C
		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00026BC0 File Offset: 0x00024DC0
		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00026C04 File Offset: 0x00024E04
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
