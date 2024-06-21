using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B3 RID: 179
	[ComVisible(false)]
	public class PathFilter : IScanFilter
	{
		// Token: 0x060007D5 RID: 2005 RVA: 0x00033218 File Offset: 0x00031418
		public PathFilter(string filter)
		{
			this.nameFilter_ = new NameFilter(filter);
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00033230 File Offset: 0x00031430
		public virtual bool IsMatch(string name)
		{
			bool result = false;
			bool flag = name != null;
			if (flag)
			{
				string name2 = (name.Length > 0) ? Path.GetFullPath(name) : "";
				result = this.nameFilter_.IsMatch(name2);
			}
			return result;
		}

		// Token: 0x040004BE RID: 1214
		private NameFilter nameFilter_;
	}
}
