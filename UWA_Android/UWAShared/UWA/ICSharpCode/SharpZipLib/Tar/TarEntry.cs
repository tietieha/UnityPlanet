using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A1 RID: 161
	[ComVisible(false)]
	public class TarEntry : ICloneable
	{
		// Token: 0x06000785 RID: 1925 RVA: 0x0003ABE8 File Offset: 0x00038DE8
		private TarEntry()
		{
			this.header = new TarHeader();
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0003AC00 File Offset: 0x00038E00
		public TarEntry(byte[] headerBuffer)
		{
			this.header = new TarHeader();
			this.header.ParseBuffer(headerBuffer);
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0003AC24 File Offset: 0x00038E24
		public TarEntry(TarHeader header)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			this.header = (TarHeader)header.Clone();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0003AC68 File Offset: 0x00038E68
		public object Clone()
		{
			return new TarEntry
			{
				file = this.file,
				header = (TarHeader)this.header.Clone(),
				Name = this.Name
			};
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0003ACB8 File Offset: 0x00038EB8
		public static TarEntry CreateTarEntry(string name)
		{
			TarEntry tarEntry = new TarEntry();
			TarEntry.NameTarHeader(tarEntry.header, name);
			return tarEntry;
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0003ACE8 File Offset: 0x00038EE8
		public static TarEntry CreateEntryFromFile(string fileName)
		{
			TarEntry tarEntry = new TarEntry();
			tarEntry.GetFileTarHeader(tarEntry.header, fileName);
			return tarEntry;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0003AD18 File Offset: 0x00038F18
		public override bool Equals(object obj)
		{
			TarEntry tarEntry = obj as TarEntry;
			bool flag = tarEntry != null;
			return flag && this.Name.Equals(tarEntry.Name);
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0003AD5C File Offset: 0x00038F5C
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0003AD80 File Offset: 0x00038F80
		public bool IsDescendent(TarEntry toTest)
		{
			bool flag = toTest == null;
			if (flag)
			{
				throw new ArgumentNullException("toTest");
			}
			return toTest.Name.StartsWith(this.Name);
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600078E RID: 1934 RVA: 0x0003ADC4 File Offset: 0x00038FC4
		public TarHeader TarHeader
		{
			get
			{
				return this.header;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x0003ADE4 File Offset: 0x00038FE4
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x0003AE08 File Offset: 0x00039008
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

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x0003AE18 File Offset: 0x00039018
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x0003AE3C File Offset: 0x0003903C
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

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x0003AE4C File Offset: 0x0003904C
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x0003AE70 File Offset: 0x00039070
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

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x0003AE80 File Offset: 0x00039080
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x0003AEA4 File Offset: 0x000390A4
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

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x0003AEB4 File Offset: 0x000390B4
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x0003AED8 File Offset: 0x000390D8
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

		// Token: 0x06000799 RID: 1945 RVA: 0x0003AEE8 File Offset: 0x000390E8
		public void SetIds(int userId, int groupId)
		{
			this.UserId = userId;
			this.GroupId = groupId;
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0003AEFC File Offset: 0x000390FC
		public void SetNames(string userName, string groupName)
		{
			this.UserName = userName;
			this.GroupName = groupName;
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x0003AF10 File Offset: 0x00039110
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x0003AF34 File Offset: 0x00039134
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

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x0003AF44 File Offset: 0x00039144
		public string File
		{
			get
			{
				return this.file;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x0003AF64 File Offset: 0x00039164
		// (set) Token: 0x0600079F RID: 1951 RVA: 0x0003AF88 File Offset: 0x00039188
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

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060007A0 RID: 1952 RVA: 0x0003AF98 File Offset: 0x00039198
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

		// Token: 0x060007A1 RID: 1953 RVA: 0x0003B020 File Offset: 0x00039220
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

		// Token: 0x060007A2 RID: 1954 RVA: 0x0003B1C8 File Offset: 0x000393C8
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

		// Token: 0x060007A3 RID: 1955 RVA: 0x0003B25C File Offset: 0x0003945C
		public void WriteEntryHeader(byte[] outBuffer)
		{
			this.header.WriteHeader(outBuffer);
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0003B26C File Offset: 0x0003946C
		public static void AdjustEntryName(byte[] buffer, string newName)
		{
			TarHeader.GetNameBytes(newName, buffer, 0, 100);
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0003B27C File Offset: 0x0003947C
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

		// Token: 0x04000489 RID: 1161
		private string file;

		// Token: 0x0400048A RID: 1162
		private TarHeader header;
	}
}
