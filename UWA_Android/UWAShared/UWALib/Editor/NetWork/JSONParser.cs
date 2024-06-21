using System;
using System.Collections.Generic;
using System.Globalization;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000053 RID: 83
	internal class JSONParser
	{
		// Token: 0x0600037D RID: 893 RVA: 0x0001FF04 File Offset: 0x0001E104
		public JSONParser(string jsondata)
		{
			this.json = jsondata + "    ";
			this.line = 1;
			this.linechar = 1;
			this.len = this.json.Length;
			this.idx = 0;
			this.pctParsed = 0;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0001FF5C File Offset: 0x0001E15C
		public static JSONValue SimpleParse(string jsondata)
		{
			JSONParser jsonparser = new JSONParser(jsondata);
			try
			{
				return jsonparser.Parse();
			}
			catch (JSONParseException ex)
			{
				Console.WriteLine(ex.Message);
			}
			return new JSONValue(null);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0001FFB0 File Offset: 0x0001E1B0
		public JSONValue Parse()
		{
			this.cur = this.json[this.idx];
			return this.ParseValue();
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0001FFE8 File Offset: 0x0001E1E8
		private char Next()
		{
			bool flag = this.cur == '\n';
			if (flag)
			{
				this.line++;
				this.linechar = 0;
			}
			this.idx++;
			bool flag2 = this.idx >= this.len;
			if (flag2)
			{
				throw new JSONParseException("End of json while parsing at " + this.PosMsg());
			}
			this.linechar++;
			int num = (int)((float)this.idx * 100f / (float)this.len);
			bool flag3 = num != this.pctParsed;
			if (flag3)
			{
				this.pctParsed = num;
			}
			this.cur = this.json[this.idx];
			return this.cur;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000200C8 File Offset: 0x0001E2C8
		private void SkipWs()
		{
			while (" \n\t\r".IndexOf(this.cur) != -1)
			{
				this.Next();
			}
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00020104 File Offset: 0x0001E304
		private string PosMsg()
		{
			return "line " + this.line.ToString() + ", column " + this.linechar.ToString();
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00020144 File Offset: 0x0001E344
		private JSONValue ParseValue()
		{
			this.SkipWs();
			char c = this.cur;
			switch (c)
			{
			case '-':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return this.ParseNumber();
			}
			bool flag = c == '"';
			JSONValue result;
			if (flag)
			{
				result = this.ParseString();
			}
			else
			{
				bool flag2 = c == '[';
				if (flag2)
				{
					result = this.ParseArray();
				}
				else
				{
					bool flag3 = c == 'f' || c == 'n' || c == 't';
					if (flag3)
					{
						result = this.ParseConstant();
					}
					else
					{
						bool flag4 = c != '{';
						if (flag4)
						{
							throw new JSONParseException("Cannot parse json value starting with '" + this.json.Substring(this.idx, 5) + "' at " + this.PosMsg());
						}
						result = this.ParseDict();
					}
				}
			}
			return result;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0002026C File Offset: 0x0001E46C
		private JSONValue ParseArray()
		{
			this.Next();
			this.SkipWs();
			List<JSONValue> list = new List<JSONValue>();
			while (this.cur != ']')
			{
				list.Add(this.ParseValue());
				this.SkipWs();
				bool flag = this.cur == ',';
				if (flag)
				{
					this.Next();
					this.SkipWs();
				}
			}
			this.Next();
			return new JSONValue(list);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x000202F4 File Offset: 0x0001E4F4
		private JSONValue ParseDict()
		{
			this.Next();
			this.SkipWs();
			Dictionary<string, JSONValue> dictionary = new Dictionary<string, JSONValue>();
			while (this.cur != '}')
			{
				JSONValue jsonvalue = this.ParseValue();
				bool flag = !jsonvalue.IsString();
				if (flag)
				{
					throw new JSONParseException("Key not string type at " + this.PosMsg());
				}
				this.SkipWs();
				bool flag2 = this.cur != ':';
				if (flag2)
				{
					throw new JSONParseException("Missing dict entry delimiter ':' at " + this.PosMsg());
				}
				this.Next();
				dictionary.Add(jsonvalue.AsString(false), this.ParseValue());
				this.SkipWs();
				bool flag3 = this.cur == ',';
				if (flag3)
				{
					this.Next();
					this.SkipWs();
				}
			}
			this.Next();
			return new JSONValue(dictionary);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x000203F4 File Offset: 0x0001E5F4
		private JSONValue ParseString()
		{
			string text = string.Empty;
			this.Next();
			while (this.idx < this.len)
			{
				int num = this.json.IndexOfAny(JSONParser.endcodes, this.idx);
				bool flag = num < 0;
				if (flag)
				{
					throw new JSONParseException("missing '\"' to end string at " + this.PosMsg());
				}
				text += this.json.Substring(this.idx, num - this.idx);
				bool flag2 = this.json[num] == '"';
				if (flag2)
				{
					this.cur = this.json[num];
					this.idx = num;
					break;
				}
				num++;
				bool flag3 = num >= this.len;
				if (flag3)
				{
					throw new JSONParseException("End of json while parsing while parsing string at " + this.PosMsg());
				}
				char c = this.json[num];
				char c2 = c;
				char c3 = c2;
				char c4 = c3;
				if (c4 != 'n')
				{
					switch (c4)
					{
					case 'r':
						text += "\r";
						goto IL_313;
					case 't':
						text += "\t";
						goto IL_313;
					case 'u':
					{
						string text2 = string.Empty;
						bool flag4 = num + 4 >= this.len;
						if (flag4)
						{
							throw new JSONParseException("End of json while parsing while parsing unicode char near " + this.PosMsg());
						}
						text2 += this.json[num + 1].ToString();
						text2 += this.json[num + 2].ToString();
						text2 += this.json[num + 3].ToString();
						text2 += this.json[num + 4].ToString();
						try
						{
							int num2 = int.Parse(text2, NumberStyles.AllowHexSpecifier);
							text += ((char)num2).ToString();
						}
						catch (FormatException)
						{
							throw new JSONParseException("Invalid unicode escape char near " + this.PosMsg());
						}
						num += 4;
						goto IL_313;
					}
					}
					bool flag5 = c2 != '"';
					if (flag5)
					{
						bool flag6 = c2 != '/';
						if (flag6)
						{
							bool flag7 = c2 != '\\';
							if (flag7)
							{
								bool flag8 = c2 == 'b';
								if (flag8)
								{
									text += "\b";
									goto IL_313;
								}
								bool flag9 = c2 != 'f';
								if (flag9)
								{
									throw new JSONParseException(string.Concat(new object[]
									{
										"Invalid escape char '",
										c,
										"' near ",
										this.PosMsg()
									}));
								}
								text += "\f";
								goto IL_313;
							}
						}
					}
					text += c.ToString();
				}
				else
				{
					text += "\n";
				}
				IL_313:
				this.idx = num + 1;
			}
			bool flag10 = this.idx >= this.len;
			if (flag10)
			{
				throw new JSONParseException("End of json while parsing while parsing string near " + this.PosMsg());
			}
			this.cur = this.json[this.idx];
			this.Next();
			return new JSONValue(text);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000207A4 File Offset: 0x0001E9A4
		private JSONValue ParseNumber()
		{
			string text = string.Empty;
			bool flag = this.cur == '-';
			if (flag)
			{
				text = "-";
				this.Next();
			}
			while (this.cur >= '0' && this.cur <= '9')
			{
				text += this.cur.ToString();
				this.Next();
			}
			bool flag2 = this.cur == '.';
			if (flag2)
			{
				this.Next();
				text += ".";
				while (this.cur >= '0' && this.cur <= '9')
				{
					text += this.cur.ToString();
					this.Next();
				}
			}
			bool flag3 = this.cur == 'e' || this.cur == 'E';
			if (flag3)
			{
				text += "e";
				this.Next();
				bool flag4 = this.cur != '-' && this.cur != '+';
				if (flag4)
				{
					text += this.cur.ToString();
					this.Next();
				}
				while (this.cur >= '0')
				{
					bool flag5 = this.cur > '9';
					if (flag5)
					{
						break;
					}
					text += this.cur.ToString();
					this.Next();
				}
			}
			JSONValue result;
			try
			{
				float num = Convert.ToSingle(text);
				result = new JSONValue(num);
			}
			catch (Exception)
			{
				throw new JSONParseException("Cannot convert string to float : '" + text + "' at " + this.PosMsg());
			}
			return result;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x000209AC File Offset: 0x0001EBAC
		private JSONValue ParseConstant()
		{
			string a = string.Concat(new object[]
			{
				string.Empty,
				this.cur,
				this.Next(),
				this.Next(),
				this.Next()
			});
			this.Next();
			bool flag = a == "true";
			if (!flag)
			{
				bool flag2 = a == "fals";
				if (flag2)
				{
					bool flag3 = this.cur == 'e';
					if (flag3)
					{
						this.Next();
						return new JSONValue(false);
					}
				}
				else
				{
					bool flag4 = a == "null";
					if (flag4)
					{
						return new JSONValue(null);
					}
				}
				throw new JSONParseException("Invalid token at " + this.PosMsg());
			}
			return new JSONValue(true);
		}

		// Token: 0x0400026B RID: 619
		private string json;

		// Token: 0x0400026C RID: 620
		private int line;

		// Token: 0x0400026D RID: 621
		private int linechar;

		// Token: 0x0400026E RID: 622
		private int len;

		// Token: 0x0400026F RID: 623
		private int idx;

		// Token: 0x04000270 RID: 624
		private int pctParsed;

		// Token: 0x04000271 RID: 625
		private char cur;

		// Token: 0x04000272 RID: 626
		private static char[] endcodes = new char[]
		{
			'\\',
			'"'
		};
	}
}
