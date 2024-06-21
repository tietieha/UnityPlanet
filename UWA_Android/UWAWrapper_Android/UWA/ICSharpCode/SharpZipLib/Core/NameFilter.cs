using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B2 RID: 178
	[ComVisible(false)]
	public class NameFilter : IScanFilter
	{
		// Token: 0x060007CD RID: 1997 RVA: 0x00032D6C File Offset: 0x00030F6C
		public NameFilter(string filter)
		{
			this.filter_ = filter;
			this.inclusions_ = new ArrayList();
			this.exclusions_ = new ArrayList();
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x00032D94 File Offset: 0x00030F94
		public static bool IsValidExpression(string expression)
		{
			bool result = true;
			try
			{
				Regex regex = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x00032DD8 File Offset: 0x00030FD8
		public static bool IsValidFilterExpression(string toTest)
		{
			bool flag = toTest == null;
			if (flag)
			{
				throw new ArgumentNullException("toTest");
			}
			bool result = true;
			try
			{
				string[] array = NameFilter.SplitQuoted(toTest);
				for (int i = 0; i < array.Length; i++)
				{
					bool flag2 = array[i] != null && array[i].Length > 0;
					if (flag2)
					{
						bool flag3 = array[i][0] == '+';
						string pattern;
						if (flag3)
						{
							pattern = array[i].Substring(1, array[i].Length - 1);
						}
						else
						{
							bool flag4 = array[i][0] == '-';
							if (flag4)
							{
								pattern = array[i].Substring(1, array[i].Length - 1);
							}
							else
							{
								pattern = array[i];
							}
						}
						Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
					}
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x00032F0C File Offset: 0x0003110C
		public static string[] SplitQuoted(string original)
		{
			char c = '\\';
			char[] array = new char[]
			{
				';'
			};
			ArrayList arrayList = new ArrayList();
			bool flag = original != null && original.Length > 0;
			if (flag)
			{
				int i = -1;
				StringBuilder stringBuilder = new StringBuilder();
				while (i < original.Length)
				{
					i++;
					bool flag2 = i >= original.Length;
					if (flag2)
					{
						arrayList.Add(stringBuilder.ToString());
					}
					else
					{
						bool flag3 = original[i] == c;
						if (flag3)
						{
							i++;
							bool flag4 = i >= original.Length;
							if (flag4)
							{
								throw new ArgumentException("Missing terminating escape character", "original");
							}
							bool flag5 = Array.IndexOf<char>(array, original[i]) < 0;
							if (flag5)
							{
								stringBuilder.Append(c);
							}
							stringBuilder.Append(original[i]);
						}
						else
						{
							bool flag6 = Array.IndexOf<char>(array, original[i]) >= 0;
							if (flag6)
							{
								arrayList.Add(stringBuilder.ToString());
								stringBuilder.Length = 0;
							}
							else
							{
								stringBuilder.Append(original[i]);
							}
						}
					}
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00033090 File Offset: 0x00031290
		public override string ToString()
		{
			return this.filter_;
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x000330B0 File Offset: 0x000312B0
		public bool IsIncluded(string name)
		{
			bool result = false;
			bool flag = this.inclusions_.Count == 0;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (object obj in this.inclusions_)
				{
					Regex regex = (Regex)obj;
					bool flag2 = regex.IsMatch(name);
					if (flag2)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x00033158 File Offset: 0x00031358
		public bool IsExcluded(string name)
		{
			bool result = false;
			foreach (object obj in this.exclusions_)
			{
				Regex regex = (Regex)obj;
				bool flag = regex.IsMatch(name);
				if (flag)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x000331E0 File Offset: 0x000313E0
		public bool IsMatch(string name)
		{
			return this.IsIncluded(name) && !this.IsExcluded(name);
		}

		// Token: 0x040004BB RID: 1211
		private string filter_;

		// Token: 0x040004BC RID: 1212
		private ArrayList inclusions_;

		// Token: 0x040004BD RID: 1213
		private ArrayList exclusions_;
	}
}
