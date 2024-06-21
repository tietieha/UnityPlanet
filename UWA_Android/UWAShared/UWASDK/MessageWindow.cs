using System;
using System.Collections.Generic;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000031 RID: 49
	public class MessageWindow : UWindow
	{
		// Token: 0x060001BA RID: 442 RVA: 0x0000D790 File Offset: 0x0000B990
		public MessageWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000D7C0 File Offset: 0x0000B9C0
		public void CreateMessage(string ms)
		{
			UWAPanel.Inst.messageOn = true;
			this.msgs.Add(ms);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000D7DC File Offset: 0x0000B9DC
		public override void WindowInitialize()
		{
			this.MessageText = new ULabel("", StyleController.Instance.GetStyle("MessageText", "label"), new Rect(100f, 20f, this.oWindowRect[this.State].width - 100f - 200f, 80f), 1f);
			this.NoMore = new UButton(new Action(this.CloseMessage), "不再提示", StyleController.Instance.GetStyle("NoMoreBtn", "label"), new Rect(this.oWindowRect[this.State].width - 200f + 30f, 20f, 150f, 80f), 1f);
			this.controlList.Add(this.MessageText);
			this.controlList.Add(this.NoMore);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000D8E4 File Offset: 0x0000BAE4
		public override void WindowView(int windowID)
		{
			bool flag = this.msgs.Count != 0;
			if (flag)
			{
				this.MessageText.View(this.msgs[0], true);
			}
			this.NoMore.View();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000D934 File Offset: 0x0000BB34
		public override void WindowVertical()
		{
			this.MessageText.SetPosition(new Rect(30f, 20f, this.oWindowRect[WindowState.Vertical].width - 30f - 200f, 200f));
			this.NoMore.SetPosition(new Rect(this.oWindowRect[WindowState.Vertical].width - 200f + 30f, 80f, 150f, 80f));
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000D9C8 File Offset: 0x0000BBC8
		public override void WindowHorizontal()
		{
			this.MessageText.SetPosition(new Rect(100f, 20f, this.oWindowRect[WindowState.Horizontal].width - 100f - 200f, 80f));
			this.NoMore.SetPosition(new Rect(this.oWindowRect[WindowState.Horizontal].width - 200f + 30f, 20f, 150f, 80f));
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000DA5C File Offset: 0x0000BC5C
		private void CloseMessage()
		{
			bool flag = this.msgs.Count > 0;
			if (flag)
			{
				this.msgs.RemoveAt(0);
			}
			bool flag2 = this.msgs.Count == 0;
			if (flag2)
			{
				UWAPanel.Inst.messageOn = false;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000DAB0 File Offset: 0x0000BCB0
		public void ClearMessage()
		{
			this.msgs.Clear();
			UWAPanel.Inst.messageOn = false;
		}

		// Token: 0x04000143 RID: 323
		private List<string> msgs = new List<string>();

		// Token: 0x04000144 RID: 324
		private ULabel MessageText;

		// Token: 0x04000145 RID: 325
		private UButton NoMore;
	}
}
