using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000033 RID: 51
	public class PocoTipWindow : UWindow
	{
		// Token: 0x060001C9 RID: 457 RVA: 0x0000DEB0 File Offset: 0x0000C0B0
		public PocoTipWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000DED4 File Offset: 0x0000C0D4
		public override void WindowInitialize()
		{
			this.Tip = new ULabel("GOT & Poco  connected", StyleController.Instance.GetStyle("Empty", "label"), new Rect(25f, 20f, 480f, 60f), 1f);
			this.controlList.Add(this.Tip);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000DF3C File Offset: 0x0000C13C
		public override void WindowView(int windowID)
		{
			this.Tip.View();
		}

		// Token: 0x04000148 RID: 328
		private ULabel Tip;
	}
}
