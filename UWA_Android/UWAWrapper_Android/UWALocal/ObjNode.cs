using System;
using System.Collections.Generic;

namespace UWALocal
{
	// Token: 0x02000010 RID: 16
	public class ObjNode
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00005C9C File Offset: 0x00003E9C
		public ObjNode(string data, int depth = 0)
		{
			this.Children = new List<ObjNode>();
			this.Depth = depth;
			this.Data = data;
			this.TotalCount = 0;
			this.DestroyedCount = 0;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00005CDC File Offset: 0x00003EDC
		public void Clear()
		{
			this.Children.Clear();
			this.TotalCount = 0;
			this.DestroyedCount = 0;
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00005D08 File Offset: 0x00003F08
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00005D10 File Offset: 0x00003F10
		public string Data { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00005D1C File Offset: 0x00003F1C
		// (set) Token: 0x060000CB RID: 203 RVA: 0x00005D24 File Offset: 0x00003F24
		public int Depth { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00005D30 File Offset: 0x00003F30
		// (set) Token: 0x060000CD RID: 205 RVA: 0x00005D38 File Offset: 0x00003F38
		public int TotalCount { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00005D44 File Offset: 0x00003F44
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00005D4C File Offset: 0x00003F4C
		public int DestroyedCount { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00005D58 File Offset: 0x00003F58
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00005D60 File Offset: 0x00003F60
		public List<ObjNode> Children { get; private set; }
	}
}
