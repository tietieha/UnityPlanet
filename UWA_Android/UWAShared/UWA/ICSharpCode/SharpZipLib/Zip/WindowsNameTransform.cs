using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000069 RID: 105
	[ComVisible(false)]
	public class WindowsNameTransform : INameTransform
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x00028D50 File Offset: 0x00026F50
		public WindowsNameTransform(string baseDirectory)
		{
			bool flag = baseDirectory == null;
			if (flag)
			{
				throw new ArgumentNullException("baseDirectory", "Directory name is invalid");
			}
			this.BaseDirectory = baseDirectory;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00028D98 File Offset: 0x00026F98
		public WindowsNameTransform()
		{
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x00028DAC File Offset: 0x00026FAC
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x00028DCC File Offset: 0x00026FCC
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

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x00028E04 File Offset: 0x00027004
		// (set) Token: 0x060004DD RID: 1245 RVA: 0x00028E24 File Offset: 0x00027024
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

		// Token: 0x060004DE RID: 1246 RVA: 0x00028E30 File Offset: 0x00027030
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

		// Token: 0x060004DF RID: 1247 RVA: 0x00028EA0 File Offset: 0x000270A0
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

		// Token: 0x060004E0 RID: 1248 RVA: 0x00028F1C File Offset: 0x0002711C
		public static bool IsValidName(string name)
		{
			return name != null && name.Length <= 260 && string.Compare(name, WindowsNameTransform.MakeValidName(name, '_')) == 0;
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x00028F64 File Offset: 0x00027164
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

		// Token: 0x060004E2 RID: 1250 RVA: 0x00028FC0 File Offset: 0x000271C0
		public static string MakeValidName(string name, char replacement)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			name = WindowsPathUtils.DropPathRoot(name.Replace("/", "\\"));
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

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x0002914C File Offset: 0x0002734C
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x0002916C File Offset: 0x0002736C
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

		// Token: 0x040002D0 RID: 720
		private const int MaxPath = 260;

		// Token: 0x040002D1 RID: 721
		private string _baseDirectory;

		// Token: 0x040002D2 RID: 722
		private bool _trimIncomingPaths;

		// Token: 0x040002D3 RID: 723
		private char _replacementChar = '_';

		// Token: 0x040002D4 RID: 724
		private static readonly char[] InvalidEntryChars;
	}
}
