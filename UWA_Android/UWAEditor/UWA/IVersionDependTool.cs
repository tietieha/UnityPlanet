using System;
using UnityEditor;

namespace UWA
{
	// Token: 0x02000003 RID: 3
	public interface IVersionDependTool
	{
		// Token: 0x06000006 RID: 6
		void PrepareNativePlugin(PluginImporter pImporter);

		// Token: 0x06000007 RID: 7
		void PrepareManagedStrip();

		// Token: 0x06000008 RID: 8
		void PrepareOtherSetup();
	}
}
