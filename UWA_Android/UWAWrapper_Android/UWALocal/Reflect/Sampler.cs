using System;
using System.Collections.Generic;
using System.Reflection;

namespace UWALocal.Reflect
{
	// Token: 0x02000027 RID: 39
	internal class Sampler
	{
		// Token: 0x060001ED RID: 493 RVA: 0x0000C680 File Offset: 0x0000A880
		public static int GetNames(List<string> names)
		{
			return (int)Sampler._samplerGetNames.Invoke(null, new object[]
			{
				names
			});
		}

		// Token: 0x040000EC RID: 236
		public static MethodInfo _samplerGetNames;

		// Token: 0x040000ED RID: 237
		public static Type _samplerType;
	}
}
