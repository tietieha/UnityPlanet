using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C1 RID: 193
	[ComVisible(false)]
	public class NameFilter : IScanFilter
	{
		// Token: 0x060008A9 RID: 2217 RVA: 0x0003FB24 File Offset: 0x0003DD24
		public NameFilter(string filter)
		{
			this.filter_ = filter;
			this.inclusions_ = new ArrayList();
			this.exclusions_ = new ArrayList();
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0003FB4C File Offset: 0x0003DD4C
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

		// Token: 0x060008AB RID: 2219 RVA: 0x0003FB90 File Offset: 0x0003DD90
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

		// Token: 0x060008AC RID: 2220 RVA: 0x0003FCC4 File Offset: 0x0003DEC4
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

		// Token: 0x060008AD RID: 2221 RVA: 0x0003FE48 File Offset: 0x0003E048
		public override string ToString()
		{
			return this.filter_;
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0003FE68 File Offset: 0x0003E068
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

		// Token: 0x060008AF RID: 2223 RVA: 0x0003FF10 File Offset: 0x0003E110
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

		// Token: 0x060008B0 RID: 2224 RVA: 0x0003FF98 File Offset: 0x0003E198
		public bool IsMatch(string name)
		{
			return this.IsIncluded(name) && !this.IsExcluded(name);
		}

		// Token: 0x0400052E RID: 1326
		private string filter_;

		// Token: 0x0400052F RID: 1327
		private ArrayList inclusions_;

		// Token: 0x04000530 RID: 1328
		private ArrayList exclusions_;
	}
}
