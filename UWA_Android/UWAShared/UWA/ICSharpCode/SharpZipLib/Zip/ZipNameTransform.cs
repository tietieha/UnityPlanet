using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200008C RID: 140
	[ComVisible(false)]
	public class ZipNameTransform : INameTransform
	{
		// Token: 0x0600065E RID: 1630 RVA: 0x0003237C File Offset: 0x0003057C
		public ZipNameTransform()
		{
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00032388 File Offset: 0x00030588
		public ZipNameTransform(string trimPrefix)
		{
			this.TrimPrefix = trimPrefix;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0003239C File Offset: 0x0003059C
		static ZipNameTransform()
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			int num = invalidPathChars.Length + 2;
			ZipNameTransform.InvalidEntryCharsRelaxed = new char[num];
			Array.Copy(invalidPathChars, 0, ZipNameTransform.InvalidEntryCharsRelaxed, 0, invalidPathChars.Length);
			ZipNameTransform.InvalidEntryCharsRelaxed[num - 1] = '*';
			ZipNameTransform.InvalidEntryCharsRelaxed[num - 2] = '?';
			num = invalidPathChars.Length + 4;
			ZipNameTransform.InvalidEntryChars = new char[num];
			Array.Copy(invalidPathChars, 0, ZipNameTransform.InvalidEntryChars, 0, invalidPathChars.Length);
			ZipNameTransform.InvalidEntryChars[num - 1] = ':';
			ZipNameTransform.InvalidEntryChars[num - 2] = '\\';
			ZipNameTransform.InvalidEntryChars[num - 3] = '*';
			ZipNameTransform.InvalidEntryChars[num - 4] = '?';
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0003243C File Offset: 0x0003063C
		public string TransformDirectory(string name)
		{
			name = this.TransformFile(name);
			bool flag = name.Length > 0;
			if (flag)
			{
				bool flag2 = !name.EndsWith("/");
				if (flag2)
				{
					name += "/";
				}
				return name;
			}
			throw new ZipException("Cannot have an empty directory name");
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x000324A8 File Offset: 0x000306A8
		public string TransformFile(string name)
		{
			bool flag = name != null;
			if (flag)
			{
				string text = name.ToLower();
				bool flag2 = this.trimPrefix_ != null && text.IndexOf(this.trimPrefix_) == 0;
				if (flag2)
				{
					name = name.Substring(this.trimPrefix_.Length);
				}
				name = name.Replace("\\", "/");
				name = WindowsPathUtils.DropPathRoot(name);
				while (name.Length > 0 && name[0] == '/')
				{
					name = name.Remove(0, 1);
				}
				while (name.Length > 0 && name[name.Length - 1] == '/')
				{
					name = name.Remove(name.Length - 1, 1);
				}
				for (int i = name.IndexOf("//"); i >= 0; i = name.IndexOf("//"))
				{
					name = name.Remove(i, 1);
				}
				name = ZipNameTransform.MakeValidName(name, '_');
			}
			else
			{
				name = string.Empty;
			}
			return name;
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x000325F4 File Offset: 0x000307F4
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00032614 File Offset: 0x00030814
		public string TrimPrefix
		{
			get
			{
				return this.trimPrefix_;
			}
			set
			{
				this.trimPrefix_ = value;
				bool flag = this.trimPrefix_ != null;
				if (flag)
				{
					this.trimPrefix_ = this.trimPrefix_.ToLower();
				}
			}
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x00032650 File Offset: 0x00030850
		private static string MakeValidName(string name, char replacement)
		{
			int i = name.IndexOfAny(ZipNameTransform.InvalidEntryChars);
			bool flag = i >= 0;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(name);
				while (i >= 0)
				{
					stringBuilder[i] = replacement;
					bool flag2 = i >= name.Length;
					if (flag2)
					{
						i = -1;
					}
					else
					{
						i = name.IndexOfAny(ZipNameTransform.InvalidEntryChars, i + 1);
					}
				}
				name = stringBuilder.ToString();
			}
			bool flag3 = name.Length > 65535;
			if (flag3)
			{
				throw new PathTooLongException();
			}
			return name;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x00032700 File Offset: 0x00030900
		public static bool IsValidName(string name, bool relaxed)
		{
			bool flag = name != null;
			bool flag2 = flag;
			if (flag2)
			{
				if (relaxed)
				{
					flag = (name.IndexOfAny(ZipNameTransform.InvalidEntryCharsRelaxed) < 0);
				}
				else
				{
					flag = (name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0);
				}
			}
			return flag;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0003276C File Offset: 0x0003096C
		public static bool IsValidName(string name)
		{
			return name != null && name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0;
		}

		// Token: 0x040003A1 RID: 929
		private string trimPrefix_;

		// Token: 0x040003A2 RID: 930
		private static readonly char[] InvalidEntryChars;

		// Token: 0x040003A3 RID: 931
		private static readonly char[] InvalidEntryCharsRelaxed;
	}
}
