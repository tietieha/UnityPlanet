using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000071 RID: 113
	[ComVisible(false)]
	public class ZipEntryFactory : IEntryFactory
	{
		// Token: 0x0600052D RID: 1325 RVA: 0x0002A6D4 File Offset: 0x000288D4
		public ZipEntryFactory()
		{
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0002A6FC File Offset: 0x000288FC
		public ZipEntryFactory(ZipEntryFactory.TimeSetting timeSetting)
		{
			this.timeSetting_ = timeSetting;
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0002A72C File Offset: 0x0002892C
		public ZipEntryFactory(DateTime time)
		{
			this.timeSetting_ = ZipEntryFactory.TimeSetting.Fixed;
			this.FixedDateTime = time;
			this.nameTransform_ = new ZipNameTransform();
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0002A764 File Offset: 0x00028964
		// (set) Token: 0x06000531 RID: 1329 RVA: 0x0002A784 File Offset: 0x00028984
		public INameTransform NameTransform
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

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0002A7BC File Offset: 0x000289BC
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x0002A7DC File Offset: 0x000289DC
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

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x0002A7E8 File Offset: 0x000289E8
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x0002A808 File Offset: 0x00028A08
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

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0002A848 File Offset: 0x00028A48
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0002A868 File Offset: 0x00028A68
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

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0002A874 File Offset: 0x00028A74
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x0002A894 File Offset: 0x00028A94
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

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0002A8A0 File Offset: 0x00028AA0
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0002A8C0 File Offset: 0x00028AC0
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

		// Token: 0x0600053C RID: 1340 RVA: 0x0002A8CC File Offset: 0x00028ACC
		public ZipEntry MakeFileEntry(string fileName)
		{
			return this.MakeFileEntry(fileName, true);
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0002A8F0 File Offset: 0x00028AF0
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

		// Token: 0x0600053E RID: 1342 RVA: 0x0002AA88 File Offset: 0x00028C88
		public ZipEntry MakeDirectoryEntry(string directoryName)
		{
			return this.MakeDirectoryEntry(directoryName, true);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0002AAAC File Offset: 0x00028CAC
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

		// Token: 0x0400034B RID: 843
		private INameTransform nameTransform_;

		// Token: 0x0400034C RID: 844
		private DateTime fixedDateTime_ = DateTime.Now;

		// Token: 0x0400034D RID: 845
		private ZipEntryFactory.TimeSetting timeSetting_;

		// Token: 0x0400034E RID: 846
		private bool isUnicodeText_;

		// Token: 0x0400034F RID: 847
		private int getAttributes_ = -1;

		// Token: 0x04000350 RID: 848
		private int setAttributes_;

		// Token: 0x0200014B RID: 331
		public enum TimeSetting
		{
			// Token: 0x040007A0 RID: 1952
			LastWriteTime,
			// Token: 0x040007A1 RID: 1953
			LastWriteTimeUtc,
			// Token: 0x040007A2 RID: 1954
			CreateTime,
			// Token: 0x040007A3 RID: 1955
			CreateTimeUtc,
			// Token: 0x040007A4 RID: 1956
			LastAccessTime,
			// Token: 0x040007A5 RID: 1957
			LastAccessTimeUtc,
			// Token: 0x040007A6 RID: 1958
			Fixed
		}
	}
}
