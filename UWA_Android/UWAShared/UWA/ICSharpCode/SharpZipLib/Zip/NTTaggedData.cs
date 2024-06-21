using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000076 RID: 118
	[ComVisible(false)]
	public class NTTaggedData : ITaggedData
	{
		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0002B258 File Offset: 0x00029458
		public short TagID
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0002B274 File Offset: 0x00029474
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

		// Token: 0x0600055D RID: 1373 RVA: 0x0002B37C File Offset: 0x0002957C
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

		// Token: 0x0600055E RID: 1374 RVA: 0x0002B434 File Offset: 0x00029634
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

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x0002B478 File Offset: 0x00029678
		// (set) Token: 0x06000560 RID: 1376 RVA: 0x0002B498 File Offset: 0x00029698
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

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000561 RID: 1377 RVA: 0x0002B4D0 File Offset: 0x000296D0
		// (set) Token: 0x06000562 RID: 1378 RVA: 0x0002B4F0 File Offset: 0x000296F0
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

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000563 RID: 1379 RVA: 0x0002B528 File Offset: 0x00029728
		// (set) Token: 0x06000564 RID: 1380 RVA: 0x0002B548 File Offset: 0x00029748
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

		// Token: 0x04000357 RID: 855
		private DateTime _lastAccessTime = DateTime.FromFileTime(0L);

		// Token: 0x04000358 RID: 856
		private DateTime _lastModificationTime = DateTime.FromFileTime(0L);

		// Token: 0x04000359 RID: 857
		private DateTime _createTime = DateTime.FromFileTime(0L);
	}
}
