using System;
using UnityEngine.Scripting;

namespace UWAShared
{
	// Token: 0x02000049 RID: 73
	[Preserve]
	public class UwaSdkSettings
	{
		// Token: 0x06000323 RID: 803 RVA: 0x0001B7D4 File Offset: 0x000199D4
		public UwaSdkSettings(string content)
		{
			this.Default();
			try
			{
				JSONClass jsonclass = (JSONClass)JSON.Parse(content);
				bool flag = jsonclass.Contains("Auto_Launch");
				if (flag)
				{
					this.AutoLaunch = jsonclass["Auto_Launch"].AsBool;
				}
				bool flag2 = jsonclass.Contains("Enable_UWAGOT");
				if (flag2)
				{
					this.EnableUWAGOT = jsonclass["Enable_UWAGOT"].AsBool;
				}
				bool flag3 = jsonclass.Contains("Enable_UWAGPM");
				if (flag3)
				{
					this.EnableUWAGPM = jsonclass["Enable_UWAGPM"].AsBool;
				}
				bool flag4 = jsonclass.Contains("Enable_UWAOL");
				if (flag4)
				{
					this.EnableUWAOL = jsonclass["Enable_UWAOL"].AsBool;
				}
				bool flag5 = jsonclass.Contains("Enable_UWAPoco");
				if (flag5)
				{
					this.EnableUWAPoco = jsonclass["Enable_UWAPoco"].AsBool;
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0001B90C File Offset: 0x00019B0C
		public UwaSdkSettings()
		{
			this.Default();
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0001B940 File Offset: 0x00019B40
		private void Default()
		{
			this.AutoLaunch = true;
			this.EnableUWAGOT = true;
			this.EnableUWAGPM = true;
			this.EnableUWAOL = false;
			this.EnableUWAPoco = false;
		}

		// Token: 0x0400022B RID: 555
		public bool AutoLaunch = true;

		// Token: 0x0400022C RID: 556
		public bool EnableUWAGOT = true;

		// Token: 0x0400022D RID: 557
		public bool EnableUWAGPM = true;

		// Token: 0x0400022E RID: 558
		public bool EnableUWAOL = false;

		// Token: 0x0400022F RID: 559
		public bool EnableUWAPoco = false;
	}
}
