using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200002D RID: 45
	public class CheckWindow : UWindow
	{
		// Token: 0x060001A4 RID: 420 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		public CheckWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000C0E0 File Offset: 0x0000A2E0
		public override void WindowInitialize()
		{
			this.Check_icon = new ULabel(TextureLoader.Instance.Get("Exit"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(35f, 35f, 50f, 50f), 1f);
			this.CheckLabel = new ULabel("退出测试", StyleController.Instance.GetStyle("Empty", "label"), new Rect(100f, 30f, 200f, 60f), 1f);
			this.CheckClose = new UButton(new Action(this.CloseCheck), TextureLoader.Instance.Get("Close"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 35f - 50f, 30f, 50f, 50f), 1f);
			this.CheckDeny = new UButton(new Action(this.CloseCheck), "取消", StyleController.Instance.GetStyle("CheckDenyBtn", "label"), new Rect(this.oWindowRect[this.State].width - 200f - 35f - 200f - 35f, this.oWindowRect[this.State].height - 35f - 80f, 200f, 80f), 1f);
			this.CheckConfirm = new UButton(new Action(this.ExitPanel), "退出", StyleController.Instance.GetStyle("CheckConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].width - 200f - 35f, this.oWindowRect[this.State].height - 35f - 80f, 200f, 80f), 1f);
			this.CheckText = new ULabel("将退出测试模式，重启游戏后可重新初始化，是否退出?", StyleController.Instance.GetStyle("InformationText", "label"), new Rect(100f, 150f, this.oWindowRect[this.State].width - 100f - 100f, this.oWindowRect[this.State].height - 150f - 150f), 1f);
			this.controlList.Add(this.Check_icon);
			this.controlList.Add(this.CheckLabel);
			this.controlList.Add(this.CheckClose);
			this.controlList.Add(this.CheckDeny);
			this.controlList.Add(this.CheckConfirm);
			this.controlList.Add(this.CheckText);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000C410 File Offset: 0x0000A610
		public override void WindowView(int windowID)
		{
			this.Check_icon.View();
			this.CheckLabel.View();
			this.CheckClose.View();
			this.CheckText.View();
			this.CheckDeny.View();
			this.CheckConfirm.View();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000C46C File Offset: 0x0000A66C
		private void CloseCheck()
		{
			UWAPanel.Inst.checkOn = false;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000C47C File Offset: 0x0000A67C
		private void ExitPanel()
		{
			SdkCtrlData.Instance.TryExit = true;
		}

		// Token: 0x04000124 RID: 292
		private ULabel Check_icon;

		// Token: 0x04000125 RID: 293
		private ULabel CheckLabel;

		// Token: 0x04000126 RID: 294
		private UButton CheckClose;

		// Token: 0x04000127 RID: 295
		private UButton CheckDeny;

		// Token: 0x04000128 RID: 296
		private UButton CheckConfirm;

		// Token: 0x04000129 RID: 297
		private ULabel CheckText;
	}
}
