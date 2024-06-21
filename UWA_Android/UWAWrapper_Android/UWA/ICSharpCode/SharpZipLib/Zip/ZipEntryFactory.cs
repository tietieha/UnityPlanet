using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000062 RID: 98
	[ComVisible(false)]
	public class ZipEntryFactory : IEntryFactory
	{
		// Token: 0x06000451 RID: 1105 RVA: 0x0001D91C File Offset: 0x0001BB1C
		public ZipEntryFactory()
		{
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001D944 File Offset: 0x0001BB44
		public ZipEntryFactory(ZipEntryFactory.TimeSetting timeSetting)
		{
			this.timeSetting_ = timeSetting;
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001D974 File Offset: 0x0001BB74
		public ZipEntryFactory(DateTime time)
		{
			this.timeSetting_ = ZipEntryFactory.TimeSetting.Fixed;
			this.FixedDateTime = time;
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x0001D9AC File Offset: 0x0001BBAC
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x0001D9CC File Offset: 0x0001BBCC
		public UWA.ICSharpCode.SharpZipLib.Core.INameTransform NameTransform
		{
			get
			{
				return this.nameTransform_;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.nameTransform_ = new ZipNameTransform();
				}
				else
				{
					this.nameTransform_ = value;
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x0001DA04 File Offset: 0x0001BC04
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x0001DA24 File Offset: 0x0001BC24
		public ZipEntryFactory.TimeSetting Setting
		{
			get
			{
				return this.timeSetting_;
			}
			set
			{
				this.timeSetting_ = value;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x0001DA30 File Offset: 0x0001BC30
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x0001DA50 File Offset: 0x0001BC50
		public DateTime FixedDateTime
		{
			get
			{
				return this.fixedDateTime_;
			}
			set
			{
				bool flag = value.Year < 1970;
				if (flag)
				{
					throw new ArgumentException("Value is too old to be valid", "value");
				}
				this.fixedDateTime_ = value;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0001DA90 File Offset: 0x0001BC90
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x0001DAB0 File Offset: 0x0001BCB0
		public int GetAttributes
		{
			get
			{
				return this.getAttributes_;
			}
			set
			{
				this.getAttributes_ = value;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x0001DABC File Offset: 0x0001BCBC
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x0001DADC File Offset: 0x0001BCDC
		public int SetAttributes
		{
			get
			{
				return this.setAttributes_;
			}
			set
			{
				this.setAttributes_ = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x0001DAE8 File Offset: 0x0001BCE8
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x0001DB08 File Offset: 0x0001BD08
		public bool IsUnicodeText
		{
			get
			{
				return this.isUnicodeText_;
			}
			set
			{
				this.isUnicodeText_ = value;
			}
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001DB14 File Offset: 0x0001BD14
		public ZipEntry MakeFileEntry(string fileName)
		{
			return this.MakeFileEntry(fileName, true);
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0001DB38 File Offset: 0x0001BD38
		public ZipEntry MakeFileEntry(string fileName, bool useFileSystem)
		{
			ZipEntry zipEntry = new ZipEntry(this.nameTransform_.TransformFile(fileName));
			zipEntry.IsUnicodeText = this.isUnicodeText_;
			int num = 0;
			bool flag = this.setAttributes_ != 0;
			FileInfo fileInfo = null;
			if (useFileSystem)
			{
				fileInfo = new FileInfo(fileName);
			}
			bool flag2 = fileInfo != null && fileInfo.Exists;
			if (flag2)
			{
				switch (this.timeSetting_)
				{
				case ZipEntryFactory.TimeSetting.LastWriteTime:
					zipEntry.DateTime = fileInfo.LastWriteTime;
					break;
				case ZipEntryFactory.TimeSetting.LastWriteTimeUtc:
					zipEntry.DateTime = fileInfo.LastWriteTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.CreateTime:
					zipEntry.DateTime = fileInfo.CreationTime;
					break;
				case ZipEntryFactory.TimeSetting.CreateTimeUtc:
					zipEntry.DateTime = fileInfo.CreationTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.LastAccessTime:
					zipEntry.DateTime = fileInfo.LastAccessTime;
					break;
				case ZipEntryFactory.TimeSetting.LastAccessTimeUtc:
					zipEntry.DateTime = fileInfo.LastAccessTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.Fixed:
					zipEntry.DateTime = this.fixedDateTime_;
					break;
				default:
					throw new ZipException("Unhandled time setting in MakeFileEntry");
				}
				zipEntry.Size = fileInfo.Length;
				flag = true;
				num = (int)(fileInfo.Attributes & (FileAttributes)this.getAttributes_);
			}
			else
			{
				bool flag3 = this.timeSetting_ == ZipEntryFactory.TimeSetting.Fixed;
				if (flag3)
				{
					zipEntry.DateTime = this.fixedDateTime_;
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				num |= this.setAttributes_;
				zipEntry.ExternalFileAttributes = num;
			}
			return zipEntry;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0001DCD0 File Offset: 0x0001BED0
		public ZipEntry MakeDirectoryEntry(string directoryName)
		{
			return this.MakeDirectoryEntry(directoryName, true);
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001DCF4 File Offset: 0x0001BEF4
		public ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem)
		{
			ZipEntry zipEntry = new ZipEntry(this.nameTransform_.TransformDirectory(directoryName));
			zipEntry.IsUnicodeText = this.isUnicodeText_;
			zipEntry.Size = 0L;
			int num = 0;
			DirectoryInfo directoryInfo = null;
			if (useFileSystem)
			{
				directoryInfo = new DirectoryInfo(directoryName);
			}
			bool flag = directoryInfo != null && directoryInfo.Exists;
			if (flag)
			{
				switch (this.timeSetting_)
				{
				case ZipEntryFactory.TimeSetting.LastWriteTime:
					zipEntry.DateTime = directoryInfo.LastWriteTime;
					break;
				case ZipEntryFactory.TimeSetting.LastWriteTimeUtc:
					zipEntry.DateTime = directoryInfo.LastWriteTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.CreateTime:
					zipEntry.DateTime = directoryInfo.CreationTime;
					break;
				case ZipEntryFactory.TimeSetting.CreateTimeUtc:
					zipEntry.DateTime = directoryInfo.CreationTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.LastAccessTime:
					zipEntry.DateTime = directoryInfo.LastAccessTime;
					break;
				case ZipEntryFactory.TimeSetting.LastAccessTimeUtc:
					zipEntry.DateTime = directoryInfo.LastAccessTimeUtc;
					break;
				case ZipEntryFactory.TimeSetting.Fixed:
					zipEntry.DateTime = this.fixedDateTime_;
					break;
				default:
					throw new ZipException("Unhandled time setting in MakeDirectoryEntry");
				}
				num = (int)(directoryInfo.Attributes & (FileAttributes)this.getAttributes_);
			}
			else
			{
				bool flag2 = this.timeSetting_ == ZipEntryFactory.TimeSetting.Fixed;
				if (flag2)
				{
					zipEntry.DateTime = this.fixedDateTime_;
				}
			}
			num |= (this.setAttributes_ | 16);
			zipEntry.ExternalFileAttributes = num;
			return zipEntry;
		}

		// Token: 0x040002D8 RID: 728
		private UWA.ICSharpCode.SharpZipLib.Core.INameTransform nameTransform_;

		// Token: 0x040002D9 RID: 729
		private DateTime fixedDateTime_ = DateTime.Now;

		// Token: 0x040002DA RID: 730
		private ZipEntryFactory.TimeSetting timeSetting_;

		// Token: 0x040002DB RID: 731
		private bool isUnicodeText_;

		// Token: 0x040002DC RID: 732
		private int getAttributes_ = -1;

		// Token: 0x040002DD RID: 733
		private int setAttributes_;

		// Token: 0x02000115 RID: 277
		public enum TimeSetting
		{
			// Token: 0x040006E3 RID: 1763
			LastWriteTime,
			// Token: 0x040006E4 RID: 1764
			LastWriteTimeUtc,
			// Token: 0x040006E5 RID: 1765
			CreateTime,
			// Token: 0x040006E6 RID: 1766
			CreateTimeUtc,
			// Token: 0x040006E7 RID: 1767
			LastAccessTime,
			// Token: 0x040006E8 RID: 1768
			LastAccessTimeUtc,
			// Token: 0x040006E9 RID: 1769
			Fixed
		}
	}
}
