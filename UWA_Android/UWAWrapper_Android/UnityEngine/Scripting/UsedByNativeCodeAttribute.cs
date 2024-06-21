using System;

namespace UnityEngine.Scripting
{
	// Token: 0x0200000C RID: 12
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, Inherited = false)]
	internal class UsedByNativeCodeAttribute : Attribute
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00005498 File Offset: 0x00003698
		public UsedByNativeCodeAttribute()
		{
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000054A0 File Offset: 0x000036A0
		public UsedByNativeCodeAttribute(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000054B0 File Offset: 0x000036B0
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000054B8 File Offset: 0x000036B8
		public string Name { get; set; }
	}
}
