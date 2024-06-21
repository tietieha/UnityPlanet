using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000074 RID: 116
	[ComVisible(false)]
	public class RawTaggedData : ITaggedData
	{
		// Token: 0x06000547 RID: 1351 RVA: 0x0002AC5C File Offset: 0x00028E5C
		public RawTaggedData(short tag)
		{
			this._tag = tag;
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x0002AC70 File Offset: 0x00028E70
		// (set) Token: 0x06000549 RID: 1353 RVA: 0x0002AC90 File Offset: 0x00028E90
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

		// Token: 0x0600054A RID: 1354 RVA: 0x0002AC9C File Offset: 0x00028E9C
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

		// Token: 0x0600054B RID: 1355 RVA: 0x0002ACE4 File Offset: 0x00028EE4
		public byte[] GetData()
		{
			return this._data;
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x0002AD04 File Offset: 0x00028F04
		// (set) Token: 0x0600054D RID: 1357 RVA: 0x0002AD24 File Offset: 0x00028F24
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

		// Token: 0x04000351 RID: 849
		private short _tag;

		// Token: 0x04000352 RID: 850
		private byte[] _data;
	}
}
