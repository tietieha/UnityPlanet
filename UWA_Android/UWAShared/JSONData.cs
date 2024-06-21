using System;
using System.IO;

namespace UWAShared
{
	// Token: 0x02000046 RID: 70
	internal class JSONData : JSONNode
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0001B0BC File Offset: 0x000192BC
		// (set) Token: 0x060002FE RID: 766 RVA: 0x0001B0DC File Offset: 0x000192DC
		public override string Value
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
				this.Tag = JSONBinaryTag.Value;
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001B0F0 File Offset: 0x000192F0
		public JSONData(string aData)
		{
			this.m_Data = aData;
			this.Tag = JSONBinaryTag.Value;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0001B10C File Offset: 0x0001930C
		public JSONData(float aData)
		{
			this.AsFloat = aData;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0001B120 File Offset: 0x00019320
		public JSONData(double aData)
		{
			this.AsDouble = aData;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0001B134 File Offset: 0x00019334
		public JSONData(bool aData)
		{
			this.AsBool = aData;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001B148 File Offset: 0x00019348
		public JSONData(int aData)
		{
			this.AsInt = aData;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0001B15C File Offset: 0x0001935C
		public override string ToString()
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001B190 File Offset: 0x00019390
		public override string ToString(string aPrefix)
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001B1C4 File Offset: 0x000193C4
		public override string ToJSON(int prefix)
		{
			JSONBinaryTag tag = this.Tag;
			JSONBinaryTag jsonbinaryTag = tag;
			string result;
			if (jsonbinaryTag != JSONBinaryTag.Value)
			{
				if (jsonbinaryTag - JSONBinaryTag.IntValue > 3)
				{
					throw new NotSupportedException("This shouldn't be here: " + this.Tag.ToString());
				}
				result = this.m_Data;
			}
			else
			{
				result = string.Format("\"{0}\"", JSONNode.Escape(this.m_Data));
			}
			return result;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001B248 File Offset: 0x00019448
		public override void Serialize(BinaryWriter aWriter)
		{
			JSONData jsondata = new JSONData("");
			jsondata.AsInt = this.AsInt;
			bool flag = jsondata.m_Data == this.m_Data;
			if (flag)
			{
				aWriter.Write(4);
				aWriter.Write(this.AsInt);
			}
			else
			{
				jsondata.AsFloat = this.AsFloat;
				bool flag2 = jsondata.m_Data == this.m_Data;
				if (flag2)
				{
					aWriter.Write(7);
					aWriter.Write(this.AsFloat);
				}
				else
				{
					jsondata.AsDouble = this.AsDouble;
					bool flag3 = jsondata.m_Data == this.m_Data;
					if (flag3)
					{
						aWriter.Write(5);
						aWriter.Write(this.AsDouble);
					}
					else
					{
						jsondata.AsBool = this.AsBool;
						bool flag4 = jsondata.m_Data == this.m_Data;
						if (flag4)
						{
							aWriter.Write(6);
							aWriter.Write(this.AsBool);
						}
						else
						{
							aWriter.Write(3);
							aWriter.Write(this.m_Data);
						}
					}
				}
			}
		}

		// Token: 0x04000228 RID: 552
		private string m_Data;
	}
}
