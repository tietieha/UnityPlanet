using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.Core
{
	// Token: 0x02000062 RID: 98
	[ComVisible(false)]
	public class JSONData : JSONNode
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x00027770 File Offset: 0x00025970
		// (set) Token: 0x06000484 RID: 1156 RVA: 0x00027790 File Offset: 0x00025990
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

		// Token: 0x06000485 RID: 1157 RVA: 0x000277A4 File Offset: 0x000259A4
		public JSONData(string aData)
		{
			this.m_Data = aData;
			this.Tag = JSONBinaryTag.Value;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000277C0 File Offset: 0x000259C0
		public JSONData(float aData)
		{
			this.AsFloat = aData;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x000277D4 File Offset: 0x000259D4
		public JSONData(double aData)
		{
			this.AsDouble = aData;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x000277E8 File Offset: 0x000259E8
		public JSONData(bool aData)
		{
			this.AsBool = aData;
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x000277FC File Offset: 0x000259FC
		public JSONData(int aData)
		{
			this.AsInt = aData;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00027810 File Offset: 0x00025A10
		public override string ToString()
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00027844 File Offset: 0x00025A44
		public override string ToString(string aPrefix)
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00027878 File Offset: 0x00025A78
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

		// Token: 0x0600048D RID: 1165 RVA: 0x00027900 File Offset: 0x00025B00
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

		// Token: 0x040002B5 RID: 693
		private string m_Data;
	}
}
