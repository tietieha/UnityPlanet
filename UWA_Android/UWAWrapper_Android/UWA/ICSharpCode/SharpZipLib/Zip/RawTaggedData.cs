using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000065 RID: 101
	[ComVisible(false)]
	public class RawTaggedData : ITaggedData
	{
		// Token: 0x0600046B RID: 1131 RVA: 0x0001DEA4 File Offset: 0x0001C0A4
		public RawTaggedData(short tag)
		{
			this._tag = tag;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x0001DEB8 File Offset: 0x0001C0B8
		// (set) Token: 0x0600046D RID: 1133 RVA: 0x0001DED8 File Offset: 0x0001C0D8
		public short TagID
		{
			get
			{
				return this._tag;
			}
			set
			{
				this._tag = value;
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001DEE4 File Offset: 0x0001C0E4
		public void SetData(byte[] data, int offset, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			this._data = new byte[count];
			Array.Copy(data, offset, this._data, 0, count);
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001DF2C File Offset: 0x0001C12C
		public byte[] GetData()
		{
			return this._data;
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x0001DF4C File Offset: 0x0001C14C
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x0001DF6C File Offset: 0x0001C16C
		public byte[] Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x040002DE RID: 734
		private short _tag;

		// Token: 0x040002DF RID: 735
		private byte[] _data;
	}
}
