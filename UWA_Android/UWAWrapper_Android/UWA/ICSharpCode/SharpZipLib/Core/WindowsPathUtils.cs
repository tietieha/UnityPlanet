using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B7 RID: 183
	[ComVisible(false)]
	public abstract class WindowsPathUtils
	{
		// Token: 0x060007EF RID: 2031 RVA: 0x00033A50 File Offset: 0x00031C50
		internal WindowsPathUtils()
		{
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00033A5C File Offset: 0x00031C5C
		public static string DropPathRoot(string path)
		{
			string text = path;
			bool flag = path != null && path.Length > 0;
			if (flag)
			{
				bool flag2 = path[0] == '\\' || path[0] == '/';
				if (flag2)
				{
					bool flag3 = path.Length > 1 && (path[1] == '\\' || path[1] == '/');
					if (flag3)
					{
						int num = 2;
						int num2 = 2;
						while (num <= path.Length && ((path[num] != '\\' && path[num] != '/') || --num2 > 0))
						{
							num++;
						}
						num++;
						bool flag4 = num < path.Length;
						if (flag4)
						{
							text = path.Substring(num);
						}
						else
						{
							text = "";
						}
					}
				}
				else
				{
					bool flag5 = path.Length > 1 && path[1] == ':';
					if (flag5)
					{
						int count = 2;
						bool flag6 = path.Length > 2 && (path[2] == '\\' || path[2] == '/');
						if (flag6)
						{
							count = 3;
						}
						text = text.Remove(0, count);
					}
				}
			}
			return text;
		}
	}
}
