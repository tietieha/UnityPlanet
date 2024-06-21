using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000032 RID: 50
	public class ModeWindow : UWindow
	{
		// Token: 0x060001C2 RID: 450 RVA: 0x0000DACC File Offset: 0x0000BCCC
		public ModeWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000DAF0 File Offset: 0x0000BCF0
		public override void WindowInitialize()
		{
			this.GOTbtn = new UButton(new Action(this.OnClickGOT), "GOT", StyleController.Instance.GetStyle("ModeBtn", "label"), new Rect(40f, 40f, 170f, 70f), 1f);
			this.GPMbtn = new UButton(new Action(this.OnClickGPM), "GPM", StyleController.Instance.GetStyle("ModeBtn", "label"), new Rect(290f, 40f, 170f, 70f), 1f);
			this.controlList.Add(this.GOTbtn);
			this.controlList.Add(this.GPMbtn);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
		public override void WindowView(int windowID)
		{
			bool flag = this.WindowRect.y < 10f;
			if (flag)
			{
				this.WindowRect.y = 10f;
			}
			bool flag2 = this.WindowRect.y > (float)Screen.height - this.WindowRect.height - 10f;
			if (flag2)
			{
				this.WindowRect.y = (float)Screen.height - this.WindowRect.height - 10f;
			}
			bool flag3 = SdkUIMgr.Get().ShowFeature("got");
			if (flag3)
			{
				this.GOTbtn.View();
			}
			bool flag4 = SdkUIMgr.Get().ShowFeature("gpm");
			if (flag4)
			{
				this.GPMbtn.View();
			}
			GUI.DragWindow(new Rect(0f, 0f, 100000f, 10000f));
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000DCB8 File Offset: 0x0000BEB8
		public override void WindowHorizontal()
		{
			bool flag = SdkUIMgr.Get().ShowFeature("got") && SdkUIMgr.Get().ShowFeature("gpm");
			if (flag)
			{
				this.GOTbtn.SetPosition(new Rect(40f, 40f, 170f, 70f));
				this.GPMbtn.SetPosition(new Rect(290f, 40f, 170f, 70f));
			}
			else
			{
				this.GOTbtn.SetPosition(new Rect(40f, 40f, 420f, 70f));
				this.GPMbtn.SetPosition(new Rect(40f, 40f, 420f, 70f));
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000DD98 File Offset: 0x0000BF98
		public override void WindowVertical()
		{
			bool flag = SdkUIMgr.Get().ShowFeature("got") && SdkUIMgr.Get().ShowFeature("gpm");
			if (flag)
			{
				this.GOTbtn.SetPosition(new Rect(40f, 40f, 170f, 120f));
				this.GPMbtn.SetPosition(new Rect(40f, 240f, 170f, 120f));
			}
			else
			{
				this.GOTbtn.SetPosition(new Rect(40f, 40f, 170f, 320f));
				this.GPMbtn.SetPosition(new Rect(40f, 40f, 170f, 320f));
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000DE78 File Offset: 0x0000C078
		private void OnClickGOT()
		{
			SdkUIMgr.Get().ChangeSdkMode(eSdkMode.GOT);
			SdkCtrlData.Instance.TryGOT = true;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000DE94 File Offset: 0x0000C094
		private void OnClickGPM()
		{
			SdkUIMgr.Get().ChangeSdkMode(eSdkMode.GPM);
			SdkCtrlData.Instance.TryGOT = true;
		}

		// Token: 0x04000146 RID: 326
		private UButton GOTbtn;

		// Token: 0x04000147 RID: 327
		private UButton GPMbtn;
	}
}
