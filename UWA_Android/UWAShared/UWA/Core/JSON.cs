using System;
using System.Runtime.InteropServices;

namespace UWA.Core
{
	// Token: 0x02000064 RID: 100
	[ComVisible(false)]
	public static class JSON
	{
		// Token: 0x060004A8 RID: 1192 RVA: 0x00027E6C File Offset: 0x0002606C
		public static JSONNode Parse(string aJSON)
		{
			return JSONNode.Parse(aJSON);
		}
	}
}
