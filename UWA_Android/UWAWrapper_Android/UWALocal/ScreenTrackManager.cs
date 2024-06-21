using System;
using System.Collections.Generic;

namespace UWALocal
{
	// Token: 0x02000020 RID: 32
	internal abstract class ScreenTrackManager : BaseTrackerManager
	{
		// Token: 0x060001C6 RID: 454 RVA: 0x0000B888 File Offset: 0x00009A88
		public static ScreenTrackManager Get()
		{
			if (ScreenTrackManager._screenTrackManager == null)
			{
				ScreenTrackManager._screenTrackManager = new EmptyScreenTrackManager();
			}
			return ScreenTrackManager._screenTrackManager;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		protected ScreenTrackManager() : base(".jpg", 200)
		{
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000B8B8 File Offset: 0x00009AB8
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000B8BC File Offset: 0x00009ABC
		public override void SwitchLogFile(bool end = false)
		{
			base.SwitchLogFile(end);
			if (!end)
			{
				this.LogFile = this.GetTargetLogName();
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000B8D8 File Offset: 0x00009AD8
		public static void Clear_screenTrackManager()
		{
			ScreenTrackManager._screenTrackManager = null;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000B8E0 File Offset: 0x00009AE0
		// (set) Token: 0x060001CC RID: 460 RVA: 0x0000B8E8 File Offset: 0x00009AE8
		protected override string LogFile { get; set; }

		// Token: 0x060001CD RID: 461 RVA: 0x0000B8F4 File Offset: 0x00009AF4
		public void BeginDrawScene()
		{
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000B8F8 File Offset: 0x00009AF8
		public void EndDrawScene()
		{
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000B8FC File Offset: 0x00009AFC
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x0000B904 File Offset: 0x00009B04
		public bool ServiceDeleted { get; set; }

		// Token: 0x060001D1 RID: 465 RVA: 0x0000B910 File Offset: 0x00009B10
		public virtual bool Shot(string path)
		{
			return false;
		}

		// Token: 0x040000D5 RID: 213
		private static ScreenTrackManager _screenTrackManager;
	}
}
