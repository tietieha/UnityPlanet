using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007D RID: 125
	[ComVisible(false)]
	public class ZipNameTransform : UWA.ICSharpCode.SharpZipLib.Core.INameTransform
	{
		// Token: 0x06000582 RID: 1410 RVA: 0x000255C4 File Offset: 0x000237C4
		public ZipNameTransform()
		{
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x000255D0 File Offset: 0x000237D0
		public ZipNameTransform(string trimPrefix)
		{
			this.TrimPrefix = trimPrefix;
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x000255E4 File Offset: 0x000237E4
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

		// Token: 0x06000585 RID: 1413 RVA: 0x00025684 File Offset: 0x00023884
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

		// Token: 0x06000586 RID: 1414 RVA: 0x000256F0 File Offset: 0x000238F0
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
				name = UWA.ICSharpCode.SharpZipLib.Core.WindowsPathUtils.DropPathRoot(name);
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

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x0002583C File Offset: 0x00023A3C
		// (set) Token: 0x06000588 RID: 1416 RVA: 0x0002585C File Offset: 0x00023A5C
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

		// Token: 0x06000589 RID: 1417 RVA: 0x00025898 File Offset: 0x00023A98
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

		// Token: 0x0600058A RID: 1418 RVA: 0x00025948 File Offset: 0x00023B48
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

		// Token: 0x0600058B RID: 1419 RVA: 0x000259B4 File Offset: 0x00023BB4
		public static bool IsValidName(string name)
		{
			return name != null && name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0;
		}

		// Token: 0x0400032E RID: 814
		private string trimPrefix_;

		// Token: 0x0400032F RID: 815
		private static readonly char[] InvalidEntryChars;

		// Token: 0x04000330 RID: 816
		private static readonly char[] InvalidEntryCharsRelaxed;
	}
}
