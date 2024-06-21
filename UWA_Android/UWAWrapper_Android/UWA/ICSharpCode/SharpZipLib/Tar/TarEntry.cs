using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000092 RID: 146
	[ComVisible(false)]
	public class TarEntry : ICloneable
	{
		// Token: 0x060006A9 RID: 1705 RVA: 0x0002DE30 File Offset: 0x0002C030
		private TarEntry()
		{
			this.header = new TarHeader();
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0002DE48 File Offset: 0x0002C048
		public TarEntry(byte[] headerBuffer)
		{
			this.header = new TarHeader();
			this.header.ParseBuffer(headerBuffer);
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0002DE6C File Offset: 0x0002C06C
		public TarEntry(TarHeader header)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			this.header = (TarHeader)header.Clone();
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0002DEB0 File Offset: 0x0002C0B0
		public object Clone()
		{
			return new TarEntry
			{
				file = this.file,
				header = (TarHeader)this.header.Clone(),
				Name = this.Name
			};
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0002DF00 File Offset: 0x0002C100
		public static TarEntry CreateTarEntry(string name)
		{
			TarEntry tarEntry = new TarEntry();
			TarEntry.NameTarHeader(tarEntry.header, name);
			return tarEntry;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0002DF30 File Offset: 0x0002C130
		public static TarEntry CreateEntryFromFile(string fileName)
		{
			TarEntry tarEntry = new TarEntry();
			tarEntry.GetFileTarHeader(tarEntry.header, fileName);
			return tarEntry;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0002DF60 File Offset: 0x0002C160
		public override bool Equals(object obj)
		{
			TarEntry tarEntry = obj as TarEntry;
			bool flag = tarEntry != null;
			return flag && this.Name.Equals(tarEntry.Name);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0002DFA4 File Offset: 0x0002C1A4
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0002DFC8 File Offset: 0x0002C1C8
		public bool IsDescendent(TarEntry toTest)
		{
			bool flag = toTest == null;
			if (flag)
			{
				throw new ArgumentNullException("toTest");
			}
			return toTest.Name.StartsWith(this.Name);
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x0002E00C File Offset: 0x0002C20C
		public TarHeader TarHeader
		{
			get
			{
				return this.header;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0002E02C File Offset: 0x0002C22C
		// (set) Token: 0x060006B4 RID: 1716 RVA: 0x0002E050 File Offset: 0x0002C250
		public string Name
		{
			get
			{
				return this.header.Name;
			}
			set
			{
				this.header.Name = value;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0002E060 File Offset: 0x0002C260
		// (set) Token: 0x060006B6 RID: 1718 RVA: 0x0002E084 File Offset: 0x0002C284
		public int UserId
		{
			get
			{
				return this.header.UserId;
			}
			set
			{
				this.header.UserId = value;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0002E094 File Offset: 0x0002C294
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x0002E0B8 File Offset: 0x0002C2B8
		public int GroupId
		{
			get
			{
				return this.header.GroupId;
			}
			set
			{
				this.header.GroupId = value;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0002E0C8 File Offset: 0x0002C2C8
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x0002E0EC File Offset: 0x0002C2EC
		public string UserName
		{
			get
			{
				return this.header.UserName;
			}
			set
			{
				this.header.UserName = value;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0002E0FC File Offset: 0x0002C2FC
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x0002E120 File Offset: 0x0002C320
		public string GroupName
		{
			get
			{
				return this.header.GroupName;
			}
			set
			{
				this.header.GroupName = value;
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0002E130 File Offset: 0x0002C330
		public void SetIds(int userId, int groupId)
		{
			this.UserId = userId;
			this.GroupId = groupId;
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0002E144 File Offset: 0x0002C344
		public void SetNames(string userName, string groupName)
		{
			this.UserName = userName;
			this.GroupName = groupName;
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x0002E158 File Offset: 0x0002C358
		// (set) Token: 0x060006C0 RID: 1728 RVA: 0x0002E17C File Offset: 0x0002C37C
		public DateTime ModTime
		{
			get
			{
				return this.header.ModTime;
			}
			set
			{
				this.header.ModTime = value;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x0002E18C File Offset: 0x0002C38C
		public string File
		{
			get
			{
				return this.file;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0002E1AC File Offset: 0x0002C3AC
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0002E1D0 File Offset: 0x0002C3D0
		public long Size
		{
			get
			{
				return this.header.Size;
			}
			set
			{
				this.header.Size = value;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0002E1E0 File Offset: 0x0002C3E0
		public bool IsDirectory
		{
			get
			{
				bool flag = this.file != null;
				bool result;
				if (flag)
				{
					result = Directory.Exists(this.file);
				}
				else
				{
					bool flag2 = this.header != null;
					if (flag2)
					{
						bool flag3 = this.header.TypeFlag == 53 || this.Name.EndsWith("/");
						if (flag3)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0002E268 File Offset: 0x0002C468
		public void GetFileTarHeader(TarHeader header, string file)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			bool flag2 = file == null;
			if (flag2)
			{
				throw new ArgumentNullException("file");
			}
			this.file = file;
			string text = file;
			bool flag3 = text.IndexOf(Environment.CurrentDirectory) == 0;
			if (flag3)
			{
				text = text.Substring(Environment.CurrentDirectory.Length);
			}
			text = text.Replace(Path.DirectorySeparatorChar, '/');
			while (text.StartsWith("/"))
			{
				text = text.Substring(1);
			}
			header.LinkName = string.Empty;
			header.Name = text;
			bool flag4 = Directory.Exists(file);
			if (flag4)
			{
				header.Mode = 1003;
				header.TypeFlag = 53;
				bool flag5 = header.Name.Length == 0 || header.Name[header.Name.Length - 1] != '/';
				if (flag5)
				{
					header.Name += "/";
				}
				header.Size = 0L;
			}
			else
			{
				header.Mode = 33216;
				header.TypeFlag = 48;
				header.Size = new FileInfo(file.Replace('/', Path.DirectorySeparatorChar)).Length;
			}
			header.ModTime = System.IO.File.GetLastWriteTime(file.Replace('/', Path.DirectorySeparatorChar)).ToUniversalTime();
			header.DevMajor = 0;
			header.DevMinor = 0;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0002E410 File Offset: 0x0002C610
		public TarEntry[] GetDirectoryEntries()
		{
			bool flag = this.file == null || !Directory.Exists(this.file);
			TarEntry[] result;
			if (flag)
			{
				result = new TarEntry[0];
			}
			else
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(this.file);
				TarEntry[] array = new TarEntry[fileSystemEntries.Length];
				for (int i = 0; i < fileSystemEntries.Length; i++)
				{
					array[i] = TarEntry.CreateEntryFromFile(fileSystemEntries[i]);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0002E4A4 File Offset: 0x0002C6A4
		public void WriteEntryHeader(byte[] outBuffer)
		{
			this.header.WriteHeader(outBuffer);
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0002E4B4 File Offset: 0x0002C6B4
		public static void AdjustEntryName(byte[] buffer, string newName)
		{
			TarHeader.GetNameBytes(newName, buffer, 0, 100);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0002E4C4 File Offset: 0x0002C6C4
		public static void NameTarHeader(TarHeader header, string name)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			bool flag2 = name == null;
			if (flag2)
			{
				throw new ArgumentNullException("name");
			}
			bool flag3 = name.EndsWith("/");
			header.Name = name;
			header.Mode = (flag3 ? 1003 : 33216);
			header.UserId = 0;
			header.GroupId = 0;
			header.Size = 0L;
			header.ModTime = DateTime.UtcNow;
			header.TypeFlag = (flag3 ? 53 : 48);
			header.LinkName = string.Empty;
			header.UserName = string.Empty;
			header.GroupName = string.Empty;
			header.DevMajor = 0;
			header.DevMinor = 0;
		}

		// Token: 0x04000416 RID: 1046
		private string file;

		// Token: 0x04000417 RID: 1047
		private TarHeader header;
	}
}
