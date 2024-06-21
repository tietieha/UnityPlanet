using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200005A RID: 90
	[ComVisible(false)]
	public class WindowsNameTransform : UWA.ICSharpCode.SharpZipLib.Core.INameTransform
	{
		// Token: 0x060003FC RID: 1020 RVA: 0x0001BF98 File Offset: 0x0001A198
		public WindowsNameTransform(string baseDirectory)
		{
			bool flag = baseDirectory == null;
			if (flag)
			{
				throw new ArgumentNullException("baseDirectory", "Directory name is invalid");
			}
			this.BaseDirectory = baseDirectory;
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0001BFE0 File Offset: 0x0001A1E0
		public WindowsNameTransform()
		{
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x0001BFF4 File Offset: 0x0001A1F4
		// (set) Token: 0x060003FF RID: 1023 RVA: 0x0001C014 File Offset: 0x0001A214
		public string BaseDirectory
		{
			get
			{
				return this._baseDirectory;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this._baseDirectory = Path.GetFullPath(value);
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x0001C04C File Offset: 0x0001A24C
		// (set) Token: 0x06000401 RID: 1025 RVA: 0x0001C06C File Offset: 0x0001A26C
		public bool TrimIncomingPaths
		{
			get
			{
				return this._trimIncomingPaths;
			}
			set
			{
				this._trimIncomingPaths = value;
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0001C078 File Offset: 0x0001A278
		public string TransformDirectory(string name)
		{
			name = this.TransformFile(name);
			bool flag = name.Length > 0;
			if (flag)
			{
				while (name.EndsWith("\\"))
				{
					name = name.Remove(name.Length - 1, 1);
				}
				return name;
			}
			throw new ZipException("Cannot have an empty directory name");
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0001C0E8 File Offset: 0x0001A2E8
		public string TransformFile(string name)
		{
			bool flag = name != null;
			if (flag)
			{
				name = WindowsNameTransform.MakeValidName(name, this._replacementChar);
				bool trimIncomingPaths = this._trimIncomingPaths;
				if (trimIncomingPaths)
				{
					name = Path.GetFileName(name);
				}
				bool flag2 = this._baseDirectory != null;
				if (flag2)
				{
					name = Path.Combine(this._baseDirectory, name);
				}
			}
			else
			{
				name = string.Empty;
			}
			return name;
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0001C164 File Offset: 0x0001A364
		public static bool IsValidName(string name)
		{
			return name != null && name.Length <= 260 && string.Compare(name, WindowsNameTransform.MakeValidName(name, '_')) == 0;
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001C1AC File Offset: 0x0001A3AC
		static WindowsNameTransform()
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			int num = invalidPathChars.Length + 3;
			WindowsNameTransform.InvalidEntryChars = new char[num];
			Array.Copy(invalidPathChars, 0, WindowsNameTransform.InvalidEntryChars, 0, invalidPathChars.Length);
			WindowsNameTransform.InvalidEntryChars[num - 1] = '*';
			WindowsNameTransform.InvalidEntryChars[num - 2] = '?';
			WindowsNameTransform.InvalidEntryChars[num - 3] = ':';
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0001C208 File Offset: 0x0001A408
		public static string MakeValidName(string name, char replacement)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			name = UWA.ICSharpCode.SharpZipLib.Core.WindowsPathUtils.DropPathRoot(name.Replace("/", "\\"));
			while (name.Length > 0 && name[0] == '\\')
			{
				name = name.Remove(0, 1);
			}
			while (name.Length > 0 && name[name.Length - 1] == '\\')
			{
				name = name.Remove(name.Length - 1, 1);
			}
			int i;
			for (i = name.IndexOf("\\\\"); i >= 0; i = name.IndexOf("\\\\"))
			{
				name = name.Remove(i, 1);
			}
			i = name.IndexOfAny(WindowsNameTransform.InvalidEntryChars);
			bool flag2 = i >= 0;
			if (flag2)
			{
				StringBuilder stringBuilder = new StringBuilder(name);
				while (i >= 0)
				{
					stringBuilder[i] = replacement;
					bool flag3 = i >= name.Length;
					if (flag3)
					{
						i = -1;
					}
					else
					{
						i = name.IndexOfAny(WindowsNameTransform.InvalidEntryChars, i + 1);
					}
				}
				name = stringBuilder.ToString();
			}
			bool flag4 = name.Length > 260;
			if (flag4)
			{
				throw new PathTooLongException();
			}
			return name;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0001C394 File Offset: 0x0001A594
		// (set) Token: 0x06000408 RID: 1032 RVA: 0x0001C3B4 File Offset: 0x0001A5B4
		public char Replacement
		{
			get
			{
				return this._replacementChar;
			}
			set
			{
				for (int i = 0; i < WindowsNameTransform.InvalidEntryChars.Length; i++)
				{
					bool flag = WindowsNameTransform.InvalidEntryChars[i] == value;
					if (flag)
					{
						throw new ArgumentException("invalid path character");
					}
				}
				bool flag2 = value == '\\' || value == '/';
				if (flag2)
				{
					throw new ArgumentException("invalid replacement character");
				}
				this._replacementChar = value;
			}
		}

		// Token: 0x0400025D RID: 605
		private const int MaxPath = 260;

		// Token: 0x0400025E RID: 606
		private string _baseDirectory;

		// Token: 0x0400025F RID: 607
		private bool _trimIncomingPaths;

		// Token: 0x04000260 RID: 608
		private char _replacementChar = '_';

		// Token: 0x04000261 RID: 609
		private static readonly char[] InvalidEntryChars;
	}
}
