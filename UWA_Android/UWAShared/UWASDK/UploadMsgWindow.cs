using System;
using UnityEngine;
using UWALocal;

namespace UWASDK
{
	// Token: 0x0200003D RID: 61
	public class UploadMsgWindow : UWindow
	{
		// Token: 0x06000242 RID: 578 RVA: 0x000177CC File Offset: 0x000159CC
		public UploadMsgWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x06000243 RID: 579 RVA: 0x000177F0 File Offset: 0x000159F0
		public override void WindowInitialize()
		{
			this.UploadClose = new UButton(new Action(this.CloseUploadMsg), TextureLoader.Instance.Get("Close"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 35f, 35f, 50f, 50f), 1f);
			this.Title = new ULabel("UWA GOT Data Upload", StyleController.Instance.GetStyle("UploadTitle", "label"), new Rect(0f, 0f, this.oWindowRect[this.State].width, 120f), 1f);
			this.Message = new ULabel("", StyleController.Instance.GetStyle("UploadMsgText", "label"), new Rect(150f, 120f, this.oWindowRect[this.State].width - 300f, 300f), 1f);
			this.Cancel = new UButton(new Action(this.ToSecond), "取消", StyleController.Instance.GetStyle("UMCancelBtn", "label"), new Rect(this.oWindowRect[this.State].width - 40f - 230f - 30f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f), 1f);
			this.Confirm = new UButton(new Action(this.ToUpload), "确定", StyleController.Instance.GetStyle("UMConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].width - 40f - 200f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f), 1f);
			this.CAS = new UButton(new Action(this.ToUpload), "取消并上传", StyleController.Instance.GetStyle("UMCancelBtn", "label"), new Rect(this.oWindowRect[this.State].width - 40f - 230f - 30f - 230f, this.oWindowRect[this.State].height - 35f - 80f, 230f, 90f), 1f);
			this.Delete = new UButton(new Action(this.DeleteData), "删除", StyleController.Instance.GetStyle("UMDeleteBtn", "label"), new Rect(this.oWindowRect[this.State].width - 40f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f), 1f);
			this.controlList.Add(this.UploadClose);
			this.controlList.Add(this.Title);
			this.controlList.Add(this.Message);
			this.controlList.Add(this.Cancel);
			this.controlList.Add(this.Confirm);
			this.controlList.Add(this.CAS);
			this.controlList.Add(this.Delete);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00017BFC File Offset: 0x00015DFC
		public override void WindowView(int windowID)
		{
			this.UploadClose.View();
			this.Title.View();
			DataUploader.UploadState s = DataUploader.s;
			DataUploader.UploadState uploadState = s;
			if (uploadState != DataUploader.UploadState.ShowOld)
			{
				if (uploadState == DataUploader.UploadState.DeleteOld)
				{
					this.Message.View("测试数据将被删除，是否继续", true);
					this.CAS.View();
					this.Delete.View();
				}
			}
			else
			{
				this.Message.View(DataUploader.ShowOldTip, false);
				this.Cancel.View();
				this.Confirm.View();
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00017CA4 File Offset: 0x00015EA4
		private void ToSecond()
		{
			DataUploader.s = DataUploader.UploadState.DeleteOld;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00017CB0 File Offset: 0x00015EB0
		private void CloseUploadMsg()
		{
			DataUploader.s = DataUploader.UploadState.Idle;
			bool flag = SdkUIMgr.Get();
			if (flag)
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.MODE);
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00017CE8 File Offset: 0x00015EE8
		private void ToUpload()
		{
			DataUploader.UploadSetup(DataUploader.OldData[0]);
			DataUploader.s = DataUploader.UploadState.Preparing;
			UWAPanel.Inst.uploadWindow.TryGetUserName();
			UWAPanel.Inst.uploadWindow.TryGetUserPassword();
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00017D24 File Offset: 0x00015F24
		private void DeleteData()
		{
			DataUploader.DeleteAllOldData();
			bool flag = SdkUIMgr.Get();
			if (flag)
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.MODE);
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00017D5C File Offset: 0x00015F5C
		public override void WindowHorizontal()
		{
			this.Adapt();
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00017D68 File Offset: 0x00015F68
		public override void WindowVertical()
		{
			this.Adapt();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00017D74 File Offset: 0x00015F74
		private void Adapt()
		{
			this.UploadClose.SetPosition(new Rect(this.oWindowRect[this.State].width - 50f - 65f, 35f, 50f, 50f));
			this.Title.SetPosition(new Rect(0f, 0f, this.oWindowRect[this.State].width, 120f));
			this.Message.SetPosition(new Rect(150f, 120f, this.oWindowRect[this.State].width - 300f, 300f));
			this.Cancel.SetPosition(new Rect(this.oWindowRect[this.State].width - 40f - 230f - 30f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f));
			this.Confirm.SetPosition(new Rect(this.oWindowRect[this.State].width - 40f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f));
			this.CAS.SetPosition(new Rect(this.oWindowRect[this.State].width - 40f - 230f - 30f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f));
			this.Delete.SetPosition(new Rect(this.oWindowRect[this.State].width - 40f - 230f, this.oWindowRect[this.State].height - 35f - 90f, 230f, 90f));
		}

		// Token: 0x040001FB RID: 507
		private ULabel Title;

		// Token: 0x040001FC RID: 508
		private ULabel Message;

		// Token: 0x040001FD RID: 509
		private UButton Cancel;

		// Token: 0x040001FE RID: 510
		private UButton Confirm;

		// Token: 0x040001FF RID: 511
		private UButton CAS;

		// Token: 0x04000200 RID: 512
		private UButton Delete;

		// Token: 0x04000201 RID: 513
		private UButton UploadClose;

		// Token: 0x020000FC RID: 252
		private enum ActionState
		{
			// Token: 0x04000663 RID: 1635
			First,
			// Token: 0x04000664 RID: 1636
			Second
		}
	}
}
