using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C2 RID: 194
	[ComVisible(false)]
	public class PathFilter : IScanFilter
	{
		// Token: 0x060008B1 RID: 2225 RVA: 0x0003FFD0 File Offset: 0x0003E1D0
		public PathFilter(string filter)
		{
			this.nameFilter_ = new NameFilter(filter);
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0003FFE8 File Offset: 0x0003E1E8
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

		// Token: 0x04000531 RID: 1329
		private NameFilter nameFilter_;
	}
}
