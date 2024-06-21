using System;

namespace UWASDK
{
	// Token: 0x0200003B RID: 59
	public class UProject
	{
		// Token: 0x06000214 RID: 532 RVA: 0x00014208 File Offset: 0x00012408
		public UProject(string pName, string pPackageName, GameType pProjType = GameType.GAME_2D, GameSubType pGameType = GameSubType.ACT)
		{
			this.name = pName;
			this.packageName = pPackageName;
			this.projType = pProjType;
			this.gameType = pGameType;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00014230 File Offset: 0x00012430
		public UProject(string pName, string pPackageName, int pProjType, int pGameType)
		{
			this.name = pName;
			this.packageName = pPackageName;
			this.projType = (GameType)pProjType;
			this.gameType = (GameSubType)pGameType;
		}

		// Token: 0x040001BB RID: 443
		public string name;

		// Token: 0x040001BC RID: 444
		public string packageName;

		// Token: 0x040001BD RID: 445
		public GameType projType;

		// Token: 0x040001BE RID: 446
		public GameSubType gameType;
	}
}
