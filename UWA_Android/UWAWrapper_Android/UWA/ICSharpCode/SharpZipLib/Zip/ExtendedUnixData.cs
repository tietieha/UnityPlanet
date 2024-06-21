using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000066 RID: 102
	[ComVisible(false)]
	public class ExtendedUnixData : ITaggedData
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x0001DF78 File Offset: 0x0001C178
		public short TagID
		{
			get
			{
				return 21589;
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001DF98 File Offset: 0x0001C198
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

		// Token: 0x06000474 RID: 1140 RVA: 0x0001E12C File Offset: 0x0001C32C
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

		// Token: 0x06000475 RID: 1141 RVA: 0x0001E2B0 File Offset: 0x0001C4B0
		public static bool IsValidValue(DateTime value)
		{
			return value >= new DateTime(1901, 12, 13, 20, 45, 52) || value <= new DateTime(2038, 1, 19, 3, 14, 7);
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x0001E308 File Offset: 0x0001C508
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x0001E328 File Offset: 0x0001C528
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

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x0001E36C File Offset: 0x0001C56C
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x0001E38C File Offset: 0x0001C58C
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

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001E3D0 File Offset: 0x0001C5D0
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x0001E3F0 File Offset: 0x0001C5F0
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

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x0001E434 File Offset: 0x0001C634
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x0001E454 File Offset: 0x0001C654
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

		// Token: 0x040002E0 RID: 736
		private ExtendedUnixData.Flags _flags;

		// Token: 0x040002E1 RID: 737
		private DateTime _modificationTime = new DateTime(1970, 1, 1);

		// Token: 0x040002E2 RID: 738
		private DateTime _lastAccessTime = new DateTime(1970, 1, 1);

		// Token: 0x040002E3 RID: 739
		private DateTime _createTime = new DateTime(1970, 1, 1);

		// Token: 0x02000116 RID: 278
		[Flags]
		public enum Flags : byte
		{
			// Token: 0x040006EB RID: 1771
			ModificationTime = 1,
			// Token: 0x040006EC RID: 1772
			AccessTime = 2,
			// Token: 0x040006ED RID: 1773
			CreateTime = 4
		}
	}
}
