using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200002F RID: 47
	public class ExitWindow : UWindow
	{
		// Token: 0x060001AE RID: 430 RVA: 0x0000CB3C File Offset: 0x0000AD3C
		public ExitWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000CB68 File Offset: 0x0000AD68
		public override void WindowInitialize()
		{
			this.ToDetail = new UButton(new Action(this.DetailOn), TextureLoader.Instance.Get("Detail"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(45f, 45f, 60f, 60f), 1f);
			this.Exit = new UButton(new Action(this.ExitOn), TextureLoader.Instance.Get("Exit"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(145f, 45f, 60f, 60f), 1f);
			this.controlList.Add(this.ToDetail);
			this.controlList.Add(this.Exit);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000CC50 File Offset: 0x0000AE50
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
			this.ToDetail.View();
			this.Exit.View();
			Vector2 windowPos = base.GetWindowPos();
			Rect windowRect = base.GetWindowRect();
			Rect rect;
			rect..ctor(windowPos.x, windowPos.y, windowPos.x + windowRect.width, windowPos.y + windowRect.height);
			bool flag3 = Input.GetMouseButtonDown(0) && Input.mousePosition.x >= rect.x && Input.mousePosition.x <= rect.width && (float)Screen.height - Input.mousePosition.y >= rect.y && (float)Screen.height - Input.mousePosition.y <= rect.height;
			if (flag3)
			{
				this.isDrag = true;
			}
			else
			{
				bool mouseButtonUp = Input.GetMouseButtonUp(0);
				if (mouseButtonUp)
				{
					this.isDrag = false;
				}
			}
			GUI.DragWindow(new Rect(0f, 0f, 100000f, 10000f));
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
		private void DetailOn()
		{
			UWAPanel.Inst.detailOn = true;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000CE08 File Offset: 0x0000B008
		private void ExitOn()
		{
			UWAPanel.Inst.checkOn = true;
		}

		// Token: 0x04000139 RID: 313
		public bool isDrag = false;

		// Token: 0x0400013A RID: 314
		private UButton ToDetail;

		// Token: 0x0400013B RID: 315
		private UButton Exit;
	}
}
