using System;
using UnityEngine;
using UWA;
using UWAShared;

namespace UWASDK
{
	// Token: 0x0200002E RID: 46
	public class DetailWindow : UWindow
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000C48C File Offset: 0x0000A68C
		public DetailWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000C4B8 File Offset: 0x0000A6B8
		public override void WindowInitialize()
		{
			this.Detail_icon = new ULabel(TextureLoader.Instance.Get("Detail"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(35f, 35f, 50f, 50f), 1f);
			this.DetailLabel = new ULabel("相关信息", StyleController.Instance.GetStyle("Empty", "label"), new Rect(100f, 30f, 200f, 60f), 1f);
			this.DetailClose = new UButton(new Action(this.CloseDetail), TextureLoader.Instance.Get("Close"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 35f, 35f, 50f, 50f), 1f);
			this.Confirm = new UButton(new Action(this.CloseDetail), "确认", StyleController.Instance.GetStyle("ConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].height - 200f - 35f, this.oWindowRect[this.State].width - 35f - 80f, 200f, 80f), 1f);
			this.PackageItem = new ULabel("打包方式：", StyleController.Instance.GetStyle("Detailitem", "label"), new Rect(50f, 150f, 300f, 60f), 1f);
			this.PackageValue = new ULabel("", StyleController.Instance.GetStyle("DetailValue", "label"), new Rect(350f, 150f, 600f, 60f), 1f);
			this.GAPIItem = new ULabel("图形 API：", StyleController.Instance.GetStyle("Detailitem", "label"), new Rect(50f, 250f, 300f, 60f), 1f);
			this.GAPIValue = new ULabel("", StyleController.Instance.GetStyle("DetailValue", "label"), new Rect(350f, 250f, 600f, 60f), 1f);
			this.UnityVersionItem = new ULabel("Unity版本：", StyleController.Instance.GetStyle("Detailitem", "label"), new Rect(50f, 350f, 300f, 60f), 1f);
			this.UnityVersionValue = new ULabel("", StyleController.Instance.GetStyle("DetailValue", "label"), new Rect(350f, 350f, 600f, 60f), 1f);
			this.SDKVersionItem = new UButton(new Action(this.GotoPA), "UWA SDK版本：", StyleController.Instance.GetStyle("Detailitem", "label"), new Rect(50f, 450f, 300f, 60f), 1f);
			this.SDKVersionValue = new ULabel("", StyleController.Instance.GetStyle("DetailValue", "label"), new Rect(350f, 450f, 600f, 60f), 1f);
			this.PackageInfoItem = new ULabel("发布版本：", StyleController.Instance.GetStyle("Detailitem", "label"), new Rect(50f, 550f, 300f, 60f), 1f);
			this.PackageInfo = new ULabel("", StyleController.Instance.GetStyle("DetailValue", "label"), new Rect(350f, 550f, 600f, 60f), 1f);
			this.controlList.Add(this.Detail_icon);
			this.controlList.Add(this.DetailLabel);
			this.controlList.Add(this.DetailClose);
			this.controlList.Add(this.PackageItem);
			this.controlList.Add(this.PackageValue);
			this.controlList.Add(this.GAPIItem);
			this.controlList.Add(this.GAPIValue);
			this.controlList.Add(this.UnityVersionItem);
			this.controlList.Add(this.UnityVersionValue);
			this.controlList.Add(this.SDKVersionItem);
			this.controlList.Add(this.SDKVersionValue);
			this.controlList.Add(this.PackageInfoItem);
			this.controlList.Add(this.PackageInfo);
			this.controlList.Add(this.Confirm);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000C9FC File Offset: 0x0000ABFC
		public override void WindowView(int windowID)
		{
			this.Detail_icon.View();
			this.DetailLabel.View();
			this.DetailClose.View();
			this.PackageItem.View();
			this.PackageValue.View(AppSdkInfo.ScriptBackend, true);
			this.GAPIItem.View();
			this.GAPIValue.View(AppSdkInfo.GraphicsApi, true);
			this.UnityVersionItem.View();
			this.UnityVersionValue.View(AppSdkInfo.UnityVersion, true);
			this.SDKVersionItem.View();
			this.SDKVersionValue.View(AppSdkInfo.SdkVersion, true);
			this.PackageInfoItem.View();
			this.PackageInfo.View(SharedUtils.Dev ? "Development包" : "Release包", true);
			this.Confirm.View();
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000CAE8 File Offset: 0x0000ACE8
		private void GotoPA()
		{
			this.btnNum++;
			bool flag = this.btnNum == 5;
			if (flag)
			{
				SdkCtrlData.Instance.TryPA = true;
				this.btnNum = 0;
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000CB2C File Offset: 0x0000AD2C
		private void CloseDetail()
		{
			UWAPanel.Inst.detailOn = false;
		}

		// Token: 0x0400012A RID: 298
		private ULabel Detail_icon;

		// Token: 0x0400012B RID: 299
		private ULabel DetailLabel;

		// Token: 0x0400012C RID: 300
		private UButton DetailClose;

		// Token: 0x0400012D RID: 301
		private UButton Confirm;

		// Token: 0x0400012E RID: 302
		private ULabel PackageItem;

		// Token: 0x0400012F RID: 303
		private ULabel PackageValue;

		// Token: 0x04000130 RID: 304
		private ULabel GAPIItem;

		// Token: 0x04000131 RID: 305
		private ULabel GAPIValue;

		// Token: 0x04000132 RID: 306
		private ULabel UnityVersionItem;

		// Token: 0x04000133 RID: 307
		private ULabel UnityVersionValue;

		// Token: 0x04000134 RID: 308
		private UButton SDKVersionItem;

		// Token: 0x04000135 RID: 309
		private ULabel SDKVersionValue;

		// Token: 0x04000136 RID: 310
		private ULabel PackageInfoItem;

		// Token: 0x04000137 RID: 311
		private ULabel PackageInfo;

		// Token: 0x04000138 RID: 312
		private int btnNum = 0;
	}
}
