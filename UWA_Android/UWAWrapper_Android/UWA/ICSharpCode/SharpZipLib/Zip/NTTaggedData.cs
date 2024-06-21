using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000067 RID: 103
	[ComVisible(false)]
	public class NTTaggedData : ITaggedData
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x0001E4A0 File Offset: 0x0001C6A0
		public short TagID
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001E4BC File Offset: 0x0001C6BC
		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.ReadLEInt();
					while (zipHelperStream.Position < zipHelperStream.Length)
					{
						int num = zipHelperStream.ReadLEShort();
						int num2 = zipHelperStream.ReadLEShort();
						bool flag = num == 1;
						if (flag)
						{
							bool flag2 = num2 >= 24;
							if (flag2)
							{
								long fileTime = zipHelperStream.ReadLELong();
								this._lastModificationTime = DateTime.FromFileTime(fileTime);
								long fileTime2 = zipHelperStream.ReadLELong();
								this._lastAccessTime = DateTime.FromFileTime(fileTime2);
								long fileTime3 = zipHelperStream.ReadLELong();
								this._createTime = DateTime.FromFileTime(fileTime3);
							}
							break;
						}
						zipHelperStream.Seek((long)num2, SeekOrigin.Current);
					}
				}
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001E5C4 File Offset: 0x0001C7C4
		public byte[] GetData()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteLEInt(0);
					zipHelperStream.WriteLEShort(1);
					zipHelperStream.WriteLEShort(24);
					zipHelperStream.WriteLELong(this._lastModificationTime.ToFileTime());
					zipHelperStream.WriteLELong(this._lastAccessTime.ToFileTime());
					zipHelperStream.WriteLELong(this._createTime.ToFileTime());
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001E67C File Offset: 0x0001C87C
		public static bool IsValidValue(DateTime value)
		{
			bool result = true;
			try
			{
				value.ToFileTimeUtc();
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x0001E6C0 File Offset: 0x0001C8C0
		// (set) Token: 0x06000484 RID: 1156 RVA: 0x0001E6E0 File Offset: 0x0001C8E0
		public DateTime LastModificationTime
		{
			get
			{
				return this._lastModificationTime;
			}
			set
			{
				bool flag = !NTTaggedData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lastModificationTime = value;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000485 RID: 1157 RVA: 0x0001E718 File Offset: 0x0001C918
		// (set) Token: 0x06000486 RID: 1158 RVA: 0x0001E738 File Offset: 0x0001C938
		public DateTime CreateTime
		{
			get
			{
				return this._createTime;
			}
			set
			{
				bool flag = !NTTaggedData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._createTime = value;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x0001E770 File Offset: 0x0001C970
		// (set) Token: 0x06000488 RID: 1160 RVA: 0x0001E790 File Offset: 0x0001C990
		public DateTime LastAccessTime
		{
			get
			{
				return this._lastAccessTime;
			}
			set
			{
				bool flag = !NTTaggedData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lastAccessTime = value;
			}
		}

		// Token: 0x040002E4 RID: 740
		private DateTime _lastAccessTime = DateTime.FromFileTime(0L);

		// Token: 0x040002E5 RID: 741
		private DateTime _lastModificationTime = DateTime.FromFileTime(0L);

		// Token: 0x040002E6 RID: 742
		private DateTime _createTime = DateTime.FromFileTime(0L);
	}
}
