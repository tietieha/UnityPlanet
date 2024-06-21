using System;
using System.IO;

namespace UWAEditor.Local
{
	// Token: 0x0200000B RID: 11
	public class JSONData : JSONNode
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00004850 File Offset: 0x00002A50
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00004868 File Offset: 0x00002A68
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

		// Token: 0x06000086 RID: 134 RVA: 0x0000487A File Offset: 0x00002A7A
		public JSONData(string aData)
		{
			this.m_Data = aData;
			this.Tag = JSONBinaryTag.Value;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004893 File Offset: 0x00002A93
		public JSONData(float aData)
		{
			this.AsFloat = aData;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000048A5 File Offset: 0x00002AA5
		public JSONData(double aData)
		{
			this.AsDouble = aData;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000048B7 File Offset: 0x00002AB7
		public JSONData(bool aData)
		{
			this.AsBool = aData;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000048C9 File Offset: 0x00002AC9
		public JSONData(int aData)
		{
			this.AsInt = aData;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000048DC File Offset: 0x00002ADC
		public override string ToString()
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004908 File Offset: 0x00002B08
		public override string ToString(string aPrefix)
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004934 File Offset: 0x00002B34
		public override string ToJSON(int prefix)
		{
			switch (this.Tag)
			{
			case JSONBinaryTag.Value:
				return string.Format("\"{0}\"", JSONNode.Escape(this.m_Data));
			case JSONBinaryTag.IntValue:
			case JSONBinaryTag.DoubleValue:
			case JSONBinaryTag.FloatValue:
				return this.m_Data;
			}
			throw new NotSupportedException("This shouldn't be here: " + this.Tag.ToString());
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000049B0 File Offset: 0x00002BB0
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

		// Token: 0x0400001F RID: 31
		private string m_Data;
	}
}
