using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000075 RID: 117
	[ComVisible(false)]
	public class ExtendedUnixData : ITaggedData
	{
		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0002AD30 File Offset: 0x00028F30
		public short TagID
		{
			get
			{
				return 21589;
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0002AD50 File Offset: 0x00028F50
		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					this._flags = (ExtendedUnixData.Flags)zipHelperStream.ReadByte();
					bool flag = (this._flags & ExtendedUnixData.Flags.ModificationTime) != (ExtendedUnixData.Flags)0 && count >= 5;
					if (flag)
					{
						int seconds = zipHelperStream.ReadLEInt();
						this._modificationTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
					}
					bool flag2 = (this._flags & ExtendedUnixData.Flags.AccessTime) > (ExtendedUnixData.Flags)0;
					if (flag2)
					{
						int seconds2 = zipHelperStream.ReadLEInt();
						this._lastAccessTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds2, 0)).ToLocalTime();
					}
					bool flag3 = (this._flags & ExtendedUnixData.Flags.CreateTime) > (ExtendedUnixData.Flags)0;
					if (flag3)
					{
						int seconds3 = zipHelperStream.ReadLEInt();
						this._createTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds3, 0)).ToLocalTime();
					}
				}
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0002AEE4 File Offset: 0x000290E4
		public byte[] GetData()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteByte((byte)this._flags);
					bool flag = (this._flags & ExtendedUnixData.Flags.ModificationTime) > (ExtendedUnixData.Flags)0;
					if (flag)
					{
						int value = (int)(this._modificationTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value);
					}
					bool flag2 = (this._flags & ExtendedUnixData.Flags.AccessTime) > (ExtendedUnixData.Flags)0;
					if (flag2)
					{
						int value2 = (int)(this._lastAccessTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value2);
					}
					bool flag3 = (this._flags & ExtendedUnixData.Flags.CreateTime) > (ExtendedUnixData.Flags)0;
					if (flag3)
					{
						int value3 = (int)(this._createTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value3);
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0002B068 File Offset: 0x00029268
		public static bool IsValidValue(DateTime value)
		{
			return value >= new DateTime(1901, 12, 13, 20, 45, 52) || value <= new DateTime(2038, 1, 19, 3, 14, 7);
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x0002B0C0 File Offset: 0x000292C0
		// (set) Token: 0x06000553 RID: 1363 RVA: 0x0002B0E0 File Offset: 0x000292E0
		public DateTime ModificationTime
		{
			get
			{
				return this._modificationTime;
			}
			set
			{
				bool flag = !ExtendedUnixData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._flags |= ExtendedUnixData.Flags.ModificationTime;
				this._modificationTime = value;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0002B124 File Offset: 0x00029324
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x0002B144 File Offset: 0x00029344
		public DateTime AccessTime
		{
			get
			{
				return this._lastAccessTime;
			}
			set
			{
				bool flag = !ExtendedUnixData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._flags |= ExtendedUnixData.Flags.AccessTime;
				this._lastAccessTime = value;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x0002B188 File Offset: 0x00029388
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x0002B1A8 File Offset: 0x000293A8
		public DateTime CreateTime
		{
			get
			{
				return this._createTime;
			}
			set
			{
				bool flag = !ExtendedUnixData.IsValidValue(value);
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._flags |= ExtendedUnixData.Flags.CreateTime;
				this._createTime = value;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x0002B1EC File Offset: 0x000293EC
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x0002B20C File Offset: 0x0002940C
		private ExtendedUnixData.Flags Include
		{
			get
			{
				return this._flags;
			}
			set
			{
				this._flags = value;
			}
		}

		// Token: 0x04000353 RID: 851
		private ExtendedUnixData.Flags _flags;

		// Token: 0x04000354 RID: 852
		private DateTime _modificationTime = new DateTime(1970, 1, 1);

		// Token: 0x04000355 RID: 853
		private DateTime _lastAccessTime = new DateTime(1970, 1, 1);

		// Token: 0x04000356 RID: 854
		private DateTime _createTime = new DateTime(1970, 1, 1);

		// Token: 0x0200014C RID: 332
		[Flags]
		public enum Flags : byte
		{
			// Token: 0x040007A8 RID: 1960
			ModificationTime = 1,
			// Token: 0x040007A9 RID: 1961
			AccessTime = 2,
			// Token: 0x040007AA RID: 1962
			CreateTime = 4
		}
	}
}
