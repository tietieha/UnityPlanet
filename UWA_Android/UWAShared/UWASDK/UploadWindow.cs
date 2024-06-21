using System;
using System.Collections.Generic;
using System.IO;
using HotFix;
using UnityEngine;
using UWA.Core;
using UWA.SDK;
using UWALocal;

namespace UWASDK
{
	// Token: 0x0200003C RID: 60
	public class UploadWindow : UWindow
	{
		// Token: 0x06000216 RID: 534 RVA: 0x00014258 File Offset: 0x00012458
		public UploadWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000217 RID: 535 RVA: 0x00014298 File Offset: 0x00012498
		// (set) Token: 0x06000218 RID: 536 RVA: 0x000142B8 File Offset: 0x000124B8
		public bool IPon
		{
			get
			{
				return this.ip_on;
			}
			set
			{
				this.ip_on = value;
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x000142C4 File Offset: 0x000124C4
		public override void WindowInitialize()
		{
			this.Title = new ULabel("UWA GOT Data Upload", StyleController.Instance.GetStyle("UploadTitle", "label"), new Rect(0f, 0f, this.oWindowRect[this.State].width, 120f), 1f);
			this.Mode = new UToolBar(TextureLoader.Instance.Get("Checkround-1"), TextureLoader.Instance.Get("Checkround-2"), new string[]
			{
				"Online",
				"GOT"
			}, StyleController.Instance.GetStyle("UploadMode", "label"), new Rect(50f, 120f, 600f, 48f), UToolBar.State.Horizontal, StyleController.Instance.GetStyle("ToggleBg", "label"), 1f, 5f);
			this.OnlineIcon = new ULabel(TextureLoader.Instance.Get("Tab"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(55f, 130f, 15f, 48f), 1f);
			this.OnlineText = new ULabel("Online", StyleController.Instance.GetStyle("UploadMode", "label"), new Rect(100f, 130f, 200f, 48f), 1f);
			this.controlList.Add(this.Title);
			this.controlList.Add(this.Mode);
			this.controlList.Add(this.OnlineIcon);
			this.controlList.Add(this.OnlineText);
			this.OnlineInitialize();
			this.GOTInitialize();
			this.TryGetUserName();
			this.TryGetUserPassword();
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000144AC File Offset: 0x000126AC
		public override void WindowView(int windowID)
		{
			this.Title.View();
			bool flag = this.createProjShow;
			if (flag)
			{
				this.CreateProjView();
			}
			else
			{
				bool ipon = this.IPon;
				if (ipon)
				{
					bool flag2 = DataUploader.s == DataUploader.UploadState.Preparing;
					if (flag2)
					{
						this.Mode.View();
					}
				}
				else
				{
					this.OnlineIcon.View();
					this.OnlineText.View();
				}
				bool flag3 = DataUploader.s == DataUploader.UploadState.TimeConfirm || DataUploader.s == DataUploader.UploadState.Uploading || DataUploader.s == DataUploader.UploadState.Done;
				if (flag3)
				{
					this.OnlineView();
				}
				else
				{
					int selected = this.Mode.Selected;
					int num = selected;
					if (num != 0)
					{
						if (num == 1)
						{
							GotOnlineState.Show = false;
							GotEditorState.Show = true;
							this.GOTView();
						}
					}
					else
					{
						GotOnlineState.Show = true;
						GotEditorState.Show = false;
						this.OnlineView();
					}
				}
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x000145C8 File Offset: 0x000127C8
		private void OnlineInitialize()
		{
			this.OnlineOffInitialize();
			this.OnlineOnInitialize();
			this.CreateProjInitialize();
			this.PostscriptInitialize();
			this.UploadingInitialize();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00014600 File Offset: 0x00012800
		private void OnlineView()
		{
			bool flag = DataUploader.s == DataUploader.UploadState.Preparing;
			if (flag)
			{
				GotOnlineState.eState state = GotOnlineState.State;
				GotOnlineState.eState eState = state;
				if (eState != GotOnlineState.eState.Connected)
				{
					if (eState == GotOnlineState.eState.Login)
					{
						this.OnlineOnView();
					}
				}
				else
				{
					this.OnlineOffView();
				}
			}
			else
			{
				bool flag2 = DataUploader.s == DataUploader.UploadState.TimeConfirm;
				if (flag2)
				{
					this.PostscriptView();
				}
				else
				{
					bool flag3 = DataUploader.s == DataUploader.UploadState.Uploading || DataUploader.s == DataUploader.UploadState.Done;
					if (flag3)
					{
						this.UploadingtView();
					}
				}
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x000146AC File Offset: 0x000128AC
		private void OnlineOffInitialize()
		{
			this.UploadClose = new UButton(new Action(this.CloseUpload), TextureLoader.Instance.Get("Close"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 100f, 35f, 50f, 50f), 1f);
			this.UserNameText = new ULabel("账号", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 200f, 300f, 80f), 1f);
			this.UserName = new UTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), "请输入", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.PasswordText = new ULabel("密码", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 410f, 300f, 80f), 1f);
			this.Password = new PasswordTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(50f, 505f, this.oWindowRect[this.State].width - 100f, 80f), "请输入", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.RememberMe = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "记住密码", StyleController.Instance.GetStyle("SavePassword", "label"), new Rect(this.oWindowRect[this.State].width - 300f, 410f, 200f, 50f), 1f);
			this.OnlineLoginBtn = new UButton(new Action(this.OnlineLogin), "登 录", StyleController.Instance.GetStyle("OnlineLoginBtn", "label"), new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.HintIcon = new ULabel(TextureLoader.Instance.Get("Hint"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 370f, 244f, 30f, 30f), 1f);
			this.HintText = new ULabel("将测试数据上传至UWA网站", StyleController.Instance.GetStyle("UploadHint", "label"), new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.LoginHint = new ULabel("", StyleController.Instance.GetStyle("LoginHint", "label"), new Rect(50f, 720f, 800f, 60f), 1f);
			this.controlList.Add(this.UploadClose);
			this.controlList.Add(this.UserNameText);
			this.controlList.Add(this.UserName);
			this.controlList.Add(this.PasswordText);
			this.controlList.Add(this.Password);
			this.controlList.Add(this.RememberMe);
			this.controlList.Add(this.OnlineLoginBtn);
			this.controlList.Add(this.HintIcon);
			this.controlList.Add(this.HintText);
			this.controlList.Add(this.LoginHint);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00014B3C File Offset: 0x00012D3C
		private void OnlineOffView()
		{
			this.UploadClose.View();
			this.UserNameText.View();
			this.UserName.View();
			this.RememberMe.View();
			this.PasswordText.View();
			this.Password.View();
			this.HintIcon.View();
			this.HintText.View();
			this.LoginHint.View(GotOnlineState.Info, false);
			bool enabled = GUI.enabled;
			GUI.enabled &= (!string.IsNullOrEmpty(this.UserName.Text) & !string.IsNullOrEmpty(this.Password.Text));
			this.OnlineLoginBtn.View();
			GUI.enabled = enabled;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00014C0C File Offset: 0x00012E0C
		private void OnlineLogin()
		{
			GotOnlineState.bSavePassword = this.RememberMe.IsSelected;
			GotOnlineState.Account = this.UserName.Text;
			GotOnlineState.Password = this.Password.Text;
			UploadTool.Get().LogSetup();
			UploadTool.Get().LoginWithCredentials(GotOnlineState.Account, GotOnlineState.Password, GotOnlineState.AuthCode, false, new Action<bool, int, string>(DataUploader.LoginCallback));
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00014C80 File Offset: 0x00012E80
		public void TryGetUserName()
		{
			GotOnlineState.TryGetAccount();
			this.UserName.Text = GotOnlineState.Account;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00014C9C File Offset: 0x00012E9C
		public void TryGetUserPassword()
		{
			bool flag = GotOnlineState.TryGetPassword();
			this.Password.Text = GotOnlineState.Password;
			bool flag2 = flag;
			if (flag2)
			{
				this.RememberMe.IsSelected = true;
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00014CDC File Offset: 0x00012EDC
		public void CloseUpload()
		{
			DataUploader.s = DataUploader.UploadState.Close;
			bool flag = SdkUIMgr.Get();
			if (flag)
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.MODE);
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00014D14 File Offset: 0x00012F14
		private void OnlineOnInitialize()
		{
			this.SwitchIcon = new UButton(new Action(this.Switch), TextureLoader.Instance.Get("Switch"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(47f, 42f, 45f, 45f), 1f);
			this.SwitchBtn = new UButton(new Action(this.Switch), "切换账号", StyleController.Instance.GetStyle("SwitchText", "label"), new Rect(77f, 40f, 600f, 50f), 1f);
			this.UploadProjText = new ULabel("Upload To", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 200f, 300f, 80f), 1f);
			this.ProjSelect = new UDropDown(new Action<int>(this.SelectChange), "UploadInput", new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), StyleController.Instance.GetStyle("ProjBG", "label"), new Rect(50f, 390f, this.oWindowRect[this.State].width - 100f, 350f), StyleController.Instance.GetStyle("ProjName", "label"), TextureLoader.Instance.Get("Proj-Active"), TextureLoader.Instance.Get("ShowItem"), StyleController.Instance.GetStyle("PlaceholderText", "label"), "请选择项目", 20, UWAPanel.st, 1f);
			this.NoProjHint = new ULabel("暂无项目", StyleController.Instance.GetStyle("NoProjHint", "label"), new Rect(50f, 390f, this.oWindowRect[this.State].width - 100f, 350f), 1f);
			this.RefreshIcon = new UButton(new Action(this.RefreshProj), TextureLoader.Instance.Get("Refresh"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(47f, 410f, 45f, 45f), 1f);
			this.RefreshBtn = new UButton(new Action(this.RefreshProj), "刷新项目列表", StyleController.Instance.GetStyle("SwitchText", "label"), new Rect(80f, 410f, 600f, 50f), 1f);
			this.CreateProjBtn = new UButton(new Action(this.ToCreateProj), "新建项目", StyleController.Instance.GetStyle("ToCreateProjBtn", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 250f, 410f, 250f, 80f), 1f);
			this.UploadProjBtn = new UButton(new Action(this.SubmitData), "提 交 数 据", StyleController.Instance.GetStyle("OnlineLoginBtn", "label"), new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.BalanceHint = new ULabel("", StyleController.Instance.GetStyle("LoginHint", "label"), new Rect(50f, 720f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.controlList.Add(this.SwitchIcon);
			this.controlList.Add(this.SwitchBtn);
			this.controlList.Add(this.UploadProjText);
			this.controlList.Add(this.ProjSelect);
			this.controlList.Add(this.NoProjHint);
			this.controlList.Add(this.RefreshIcon);
			this.controlList.Add(this.RefreshBtn);
			this.controlList.Add(this.CreateProjBtn);
			this.controlList.Add(this.UploadProjBtn);
			this.controlList.Add(this.BalanceHint);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x000151E4 File Offset: 0x000133E4
		private void OnlineOnView()
		{
			this.UploadProjText.View();
			bool flag = GotOnlineState.bGetBalance && !GotOnlineState.bBalanceEnough && this.isSelectProj && SdkCtrlData.Instance.SdkMode != eSdkMode.GPM;
			if (flag)
			{
				this.BalanceHint.View("项目余额不足", false);
				this.BalanceHint.SetPosition(new Rect(50f, 720f, this.oWindowRect[this.State].width - 100f, 80f));
			}
			this.ProjSelect.SetWindowPos(this.WindowRect.x, this.WindowRect.y);
			bool flag2 = GotOnlineState.ProjectNameList != null;
			if (flag2)
			{
				UploadWindow.ProjectList = GotOnlineState.ProjectNameList.GetRange(1, GotOnlineState.ProjectNameList.Count - 1).ToArray();
				this.ProjSelect.SetOptions(UploadWindow.ProjectList);
			}
			this.ProjSelect.View();
			bool flag3 = this.ProjSelect.ShowOption && (UploadWindow.ProjectList == null || UploadWindow.ProjectList.Length == 0);
			if (flag3)
			{
				this.NoProjHint.View();
			}
			bool flag4 = !this.ProjSelect.ShowOption;
			if (flag4)
			{
				this.RefreshIcon.View();
				this.RefreshBtn.View();
				bool enabled = GUI.enabled;
				GUI.enabled &= GotOnlineState.CanCreateProj;
				this.CreateProjBtn.View();
				GUI.enabled = enabled;
				bool flag5 = this.ProjSelect.Selected != -1 && DataUploader.itemAction.ProjectId == -1;
				if (flag5)
				{
					this.SelectChange(this.ProjSelect.Selected);
				}
				GUI.enabled &= (this.ProjSelect.Selected != -1 & ((GotOnlineState.bBalanceEnough & GotOnlineState.bGetBalance) | SdkCtrlData.Instance.SdkMode == eSdkMode.GPM));
				this.UploadProjBtn.View();
				GUI.enabled = enabled;
			}
			this.HintIcon.View();
			this.HintText.View();
			this.SwitchIcon.View();
			this.SwitchBtn.View();
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00015460 File Offset: 0x00013660
		private void Switch()
		{
			this.ProjSelect.ResetSelection();
			UploadWindow.ProjectList = null;
			GotOnlineState.bBalanceEnough = false;
			GotOnlineState.bGetBalance = false;
			this.isSelectProj = false;
			GotOnlineState.State = GotOnlineState.eState.Connected;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00015490 File Offset: 0x00013690
		private void SelectChange(int i)
		{
			GotOnlineState.bBalanceEnough = false;
			GotOnlineState.bGetBalance = false;
			DataUploader.s = DataUploader.UploadState.Preparing;
			DataUploader.ChangeProjectInd(i + 1);
			DataUploader.CheckProDataInfo(DataUploader.itemAction.dataPath);
			this.isSelectProj = true;
			bool flag = GotOnlineState.ProjectInd != -1;
			if (flag)
			{
				GotOnlineState.Send = true;
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000154F0 File Offset: 0x000136F0
		private void RefreshProj()
		{
			this.isSelectProj = false;
			UploadTool.Get().UpdateGotProject(1, delegate(List<string> x)
			{
				DataUploader.UpdateProjectList(x);
				UploadWindow.ProjectList = GotOnlineState.ProjectNameList.GetRange(1, GotOnlineState.ProjectNameList.Count - 1).ToArray();
				this.ProjSelect.SetOptions(UploadWindow.ProjectList);
			});
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00015514 File Offset: 0x00013714
		private void ToCreateProj()
		{
			this.createProjShow = true;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00015520 File Offset: 0x00013720
		private void SubmitData()
		{
			bool flag = GotOnlineState.State == GotOnlineState.eState.Login;
			if (flag)
			{
				this.SelectChange(this.ProjSelect.Selected);
				GotOnlineState.BalanceNotEnough = null;
			}
			DataUploader.s = DataUploader.UploadState.TimeConfirm;
			GotOnlineState.bGetBalance = false;
			GotOnlineState.bBalanceEnough = false;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00015570 File Offset: 0x00013770
		private void CreateProjInitialize()
		{
			this.ReturnBtn = new UButton(new Action(this.ReturnOnline), TextureLoader.Instance.Get("Return"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(40f, 50f, 60f, 60f), 1f);
			this.ProjNameLabel = new ULabel("项目名称", StyleController.Instance.GetStyle("InputTitleRight", "label"), new Rect(0f, 170f, 220f, 80f), 1f);
			this.ProjName = new UTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(280f, 170f, this.oWindowRect[this.State].width - 280f - 50f, 80f), "请输入", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.ProjNameHint = new ULabel("", "LoginHint", new Rect(290f, 250f, 350f, 40f), 1f);
			this.PackageNameLabel = new ULabel("包名", StyleController.Instance.GetStyle("InputTitleRight", "label"), new Rect(0f, 290f, 220f, 80f), 1f);
			this.PackageName = new UTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(280f, 290f, this.oWindowRect[this.State].width - 280f - 50f, 80f), "请输入", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.ProjTypeNameLabel = new ULabel("项目类型", StyleController.Instance.GetStyle("InputTitleRight", "label"), new Rect(0f, 410f, 220f, 80f), 1f);
			this.ProjGameType = new UDropDown(StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(280f, 410f, this.oWindowRect[this.State].width - 280f - 50f, 80f), StyleController.Instance.GetStyle("ProjBG", "label"), new Rect(280f, 500f, this.oWindowRect[this.State].width - 280f - 50f, 240f), StyleController.Instance.GetStyle("ProjName", "label"), TextureLoader.Instance.Get("Proj-Active"), TextureLoader.Instance.Get("ShowItem"), StyleController.Instance.GetStyle("PlaceholderText", "label"), "请选择类型", 0, UWAPanel.st, 1f);
			this.ProjGameType.SetOptions(GameTypeDisplay.GameTypeNames);
			this.ProjGameSubType = new UDropDown(StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(280f, 520f, this.oWindowRect[this.State].width - 280f - 50f, 80f), StyleController.Instance.GetStyle("ProjBG", "label"), new Rect(280f, 605f, this.oWindowRect[this.State].width - 280f - 50f, 190f), StyleController.Instance.GetStyle("ProjName", "label"), TextureLoader.Instance.Get("Proj-Active"), TextureLoader.Instance.Get("ShowItem"), StyleController.Instance.GetStyle("PlaceholderText", "label"), "请选择类型", 20, UWAPanel.st, 1f);
			this.ProjGameSubType.SetOptions(GameSubTypeDisplay.GameSubTypeNames);
			this.CreateProj = new UButton(new Action(this.CreateNewProj), "新 建 项 目", StyleController.Instance.GetStyle("OnlineLoginBtn", "label"), new Rect(280f, 640f, this.oWindowRect[this.State].width - 280f - 50f, 80f), 1f);
			this.controlList.Add(this.ReturnBtn);
			this.controlList.Add(this.ProjNameLabel);
			this.controlList.Add(this.ProjName);
			this.controlList.Add(this.ProjNameHint);
			this.controlList.Add(this.PackageNameLabel);
			this.controlList.Add(this.PackageName);
			this.controlList.Add(this.ProjTypeNameLabel);
			this.controlList.Add(this.ProjGameType);
			this.controlList.Add(this.ProjGameSubType);
			this.controlList.Add(this.CreateProj);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00015B04 File Offset: 0x00013D04
		private void CreateProjView()
		{
			this.ReturnBtn.View();
			this.ProjNameLabel.View();
			this.ProjName.View();
			this.PackageNameLabel.View();
			this.PackageName.View();
			bool flag = this.ProjName.Text.Length > 40;
			if (flag)
			{
				this.ProjNameHint.View("最多只能输入40个字符", false);
			}
			this.ProjTypeNameLabel.View();
			this.ProjGameType.SetWindowPos(this.WindowRect.x, this.WindowRect.y);
			this.ProjGameSubType.SetWindowPos(this.WindowRect.x, this.WindowRect.y);
			this.ProjGameType.View();
			bool flag2 = !this.ProjGameType.ShowOption;
			if (flag2)
			{
				this.ProjGameSubType.View();
				bool flag3 = !this.ProjGameSubType.ShowOption;
				if (flag3)
				{
					bool flag4 = !UploadWindow.IsNullOrWhiteSpace(this.ProjName.Text) && !UploadWindow.IsNullOrWhiteSpace(this.PackageName.Text);
					flag4 = (flag4 && this.ProjName.Text.Length <= 40);
					bool flag5 = this.ProjGameType.Selected != -1 && this.ProjGameSubType.Selected != -1;
					bool enabled = GUI.enabled;
					GUI.enabled &= (flag4 && flag5);
					this.CreateProj.View();
					GUI.enabled = enabled;
				}
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00015CC0 File Offset: 0x00013EC0
		public static bool IsNullOrWhiteSpace(string value)
		{
			bool flag = value == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < value.Length; i++)
				{
					bool flag2 = !char.IsWhiteSpace(value[i]);
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00015D28 File Offset: 0x00013F28
		private void ReturnOnline()
		{
			this.createProjShow = false;
			GotOnlineState.State = GotOnlineState.eState.Login;
			this.ProjName.Text = "";
			this.PackageName.Text = "";
			this.ProjGameType.ResetSelection();
			this.ProjGameSubType.ResetSelection();
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00015D84 File Offset: 0x00013F84
		private void CreateNewProj()
		{
			UploadTool.Get().CreateProject((int)GameSubTypeDisplay.GameSubTypes[this.ProjGameSubType.Selected], (int)GameTypeDisplay.GameTypes[this.ProjGameType.Selected], this.ProjName.Text, this.PackageName.Text, delegate(int x)
			{
				bool flag = x != -1;
				if (flag)
				{
					UploadTool.Get().UpdateGotProject(1, delegate(List<string> y)
					{
						DataUploader.UpdateProjectList(y);
						UploadWindow.ProjectList = GotOnlineState.ProjectNameList.GetRange(1, GotOnlineState.ProjectNameList.Count - 1).ToArray();
						this.ProjSelect.SetOptions(UploadWindow.ProjectList);
						this.SelectChange(this.ProjSelect.Selected);
						this.ReturnOnline();
					});
				}
				else
				{
					this.RefreshProj();
					this.ReturnOnline();
				}
			});
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00015DE8 File Offset: 0x00013FE8
		private void PostscriptInitialize()
		{
			this.PostscriptText = new ULabel("备注", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 200f, 300f, 80f), 1f);
			this.Postscript = new UTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), "请输入备注", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.PSHintText = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "上传屏幕截图至 GOT Online", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(50f, 400f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.PSHintText.IsSelected = true;
			this.PSReturnBtn = new UButton(new Action(this.ToSelectProj), "返回", StyleController.Instance.GetStyle("UMCancelBtn", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 200f - 30f - 200f, this.oWindowRect[this.State].height - 50f - 80f, 200f, 80f), 1f);
			this.PSConfirmBtn = new UButton(new Action(this.ConfirmPS), "确定", StyleController.Instance.GetStyle("UMConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 200f, this.oWindowRect[this.State].height - 50f - 80f, 200f, 80f), 1f);
			this.controlList.Add(this.PostscriptText);
			this.controlList.Add(this.Postscript);
			this.controlList.Add(this.PSHintText);
			this.controlList.Add(this.PSReturnBtn);
			this.controlList.Add(this.PSConfirmBtn);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x000160BC File Offset: 0x000142BC
		private void PostscriptView()
		{
			this.PostscriptText.View();
			this.Postscript.View();
			this.PSHintText.View();
			this.PSReturnBtn.View();
			this.PSConfirmBtn.View();
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0001610C File Offset: 0x0001430C
		private void UploadScreenshot(bool upload)
		{
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00016110 File Offset: 0x00014310
		private void ToSelectProj()
		{
			DataUploader.s = DataUploader.UploadState.Preparing;
			this.Postscript.Text = "";
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0001612C File Offset: 0x0001432C
		private void ConfirmPS()
		{
			GotOnlineState.UserNote = this.Postscript.Text;
			bool flag = !string.IsNullOrEmpty(GotOnlineState.UserNote);
			if (flag)
			{
				File.WriteAllText(DataUploader.itemAction.dataPath + "/note", GotOnlineState.UserNote);
			}
			DataUploader.uploadScreen = this.PSHintText.IsSelected;
			DataUploader.UploadStart(delegate
			{
				DataUploader.s = DataUploader.UploadState.Done;
			});
			this.Postscript.Text = "";
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000161CC File Offset: 0x000143CC
		private void UploadingInitialize()
		{
			this.UploadingText = new ULabel("上传进度", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 200f, 300f, 80f), 1f);
			this.ErrorText = new ULabel("", StyleController.Instance.GetStyle("LoginHint", "label"), new Rect(60f, 400f, 800f, 80f), 1f);
			this.UploadingPercentText = new ULabel("", StyleController.Instance.GetStyle("UploadingHint", "label"), new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.uploadPercent = 0f;
			this.ProgressBG = new ULabel(TextureLoader.Instance.Get("ProgressBG"), StyleController.Instance.GetStyle("Progress", "label"), new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.ProgressL = new ULabel(TextureLoader.Instance.Get("Progress-In-Left"), StyleController.Instance.GetStyle("Progress", "label"), new Rect(50f, 295f, 50f, 80f), 1f);
			this.ProgressM = new ULabel("", StyleController.Instance.GetStyle("ProgressIn", "label"), new Rect(55f, 295f, 50f, 80f), 1f);
			this.ProgressR = new ULabel(TextureLoader.Instance.Get("Progress-In-Right"), StyleController.Instance.GetStyle("Progress", "label"), new Rect(200f, 295f, 50f, 80f), 1f);
			this.UploadingConfirmBtn = new UButton(new Action(this.UploadFinish), "确定", StyleController.Instance.GetStyle("UMConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].width - 50f - 320f, this.oWindowRect[this.State].height - 50f - 80f, 320f, 80f), 1f);
			this.controlList.Add(this.UploadingText);
			this.controlList.Add(this.ErrorText);
			this.controlList.Add(this.UploadingPercentText);
			this.controlList.Add(this.ProgressBG);
			this.controlList.Add(this.ProgressL);
			this.controlList.Add(this.ProgressM);
			this.controlList.Add(this.ProgressR);
			this.controlList.Add(this.UploadingConfirmBtn);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00016528 File Offset: 0x00014728
		private void UploadingtView()
		{
			this.UploadingText.View();
			bool flag = !string.IsNullOrEmpty(DataUploader.errorInfo);
			if (flag)
			{
				this.ErrorText.View(DataUploader.errorInfo, true);
			}
			bool flag2 = DataUploader.uploading != null;
			if (flag2)
			{
				this.SetUploadPercentage(DataUploader.uploading.CurrentPercent * 100f);
			}
			this.UploadingPercentText.View(this.uploadPercent.ToString() + "%", true);
			this.ProgressAdapt();
			this.ProgressBG.View();
			bool flag3 = this.uploadPercent > 0f;
			if (flag3)
			{
				this.ProgressL.View();
				this.ProgressM.View();
				this.ProgressR.View();
			}
			bool enabled = GUI.enabled;
			GUI.enabled &= (DataUploader.s == DataUploader.UploadState.Done);
			this.UploadingConfirmBtn.View();
			GUI.enabled = enabled;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00016630 File Offset: 0x00014830
		public void SetUploadPercentage(float p)
		{
			this.uploadPercent = (float)((int)p);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0001663C File Offset: 0x0001483C
		private void ProgressAdapt()
		{
			float num = Math.Min(this.ProgressBG.o_Position.width / 2020f, this.ProgressBG.o_Position.height / 100f);
			float num2 = Math.Min(num, 1f) * 2020f;
			bool flag = num >= 1f;
			if (flag)
			{
				this.ProgressL.SetPosition(new Rect(this.ProgressL.o_Position.x, this.ProgressL.o_Position.y, this.ProgressL.o_Position.width, 100f));
				this.ProgressM.SetPosition(new Rect(this.ProgressM.o_Position.x, this.ProgressM.o_Position.y, num2 * this.uploadPercent / 100f - 30f, 100f));
				this.ProgressR.SetPosition(new Rect(50f + num2 * this.uploadPercent / 100f - 25f, this.ProgressR.o_Position.y, this.ProgressR.o_Position.width, 100f));
			}
			else
			{
				this.ProgressL.SetPosition(new Rect(this.ProgressL.o_Position.x, this.ProgressL.o_Position.y, this.ProgressL.o_Position.width, 100f * num));
				this.ProgressM.SetPosition(new Rect(this.ProgressM.o_Position.x, this.ProgressM.o_Position.y, num2 * this.uploadPercent / 100f - 30f * num, 100f * num));
				this.ProgressR.SetPosition(new Rect(50f + num2 * this.uploadPercent / 100f - 25f * num, this.ProgressR.o_Position.y, this.ProgressR.o_Position.width, 100f * num));
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x000168AC File Offset: 0x00014AAC
		private void UploadFinish()
		{
			DataUploader.CheckOldData();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x000168B8 File Offset: 0x00014AB8
		private void GOTInitialize()
		{
			this.IPText = new ULabel("IP", StyleController.Instance.GetStyle("InputTitle", "label"), new Rect(60f, 200f, 300f, 80f), 1f);
			this.IP = new UTextField("", StyleController.Instance.GetStyle("UploadInput", "label"), new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), "请输入", StyleController.Instance.GetStyle("PlaceholderText", "label"), 1f);
			this.GOTHintIcon = new ULabel(TextureLoader.Instance.Get("Hint"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(this.oWindowRect[this.State].width - 710f, 244f, 30f, 30f), 1f);
			this.GOTHintText = new ULabel("将测试数据上传至本地服务器，在UWA GOT面板中查看数据", StyleController.Instance.GetStyle("UploadHint", "label"), new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.GOTStateHint = new ULabel("", StyleController.Instance.GetStyle("LoginHint", "label"), new Rect(50f, 390f, 500f, 200f), 1f);
			this.IPCheckBtn = new UButton(new Action(this.IPCheck), "确 定", StyleController.Instance.GetStyle("OnlineLoginBtn", "label"), new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.IPConfirmBtn = new UButton(new Action(this.SubmitData), "提 交 数 据", StyleController.Instance.GetStyle("OnlineLoginBtn", "label"), new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f), 1f);
			this.controlList.Add(this.IPText);
			this.controlList.Add(this.IP);
			this.controlList.Add(this.GOTHintIcon);
			this.controlList.Add(this.GOTHintText);
			this.controlList.Add(this.GOTStateHint);
			this.controlList.Add(this.IPCheckBtn);
			this.controlList.Add(this.IPConfirmBtn);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00016BD4 File Offset: 0x00014DD4
		private void GOTView()
		{
			this.IPText.View();
			this.IP.View();
			GotEditorState.IP = this.IP.Text;
			this.GOTHintIcon.View();
			this.GOTHintText.View();
			bool flag = !GotEditorState.Connected;
			if (flag)
			{
				this.GOTStateHint.View(GotEditorState.Info, false);
				bool enabled = GUI.enabled;
				GUI.enabled &= (!string.IsNullOrEmpty(this.IP.Text) && !GotEditorState.Connecting);
				this.IPCheckBtn.View();
				GUI.enabled = enabled;
			}
			else
			{
				this.IPConfirmBtn.View();
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00016CA4 File Offset: 0x00014EA4
		private void IPCheck()
		{
			GotEditorState.Connecting = true;
			UWAConfig.IP = GotEditorState.IP;
			UWAConfig.PORT = 8099;
			Client.DoSocketTestAsync(delegate(bool x)
			{
				object actionLockObj = SdkUIMgr.actionLockObj;
				lock (actionLockObj)
				{
					SdkUIMgr.MainThreadActions.Add(delegate
					{
						GotEditorState.Connecting = false;
						GotEditorState.Connected = x;
						bool flag = !x;
						if (flag)
						{
							GotEditorState.Info = Localization.Instance.Get("Local Server not connected");
						}
						else
						{
							GotEditorState.Info = null;
							GotEditorState.SaveIp();
							GotEditorState.Send = true;
						}
					});
				}
			});
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00016CF8 File Offset: 0x00014EF8
		public override void WindowHorizontal()
		{
			this.bHorizontal = true;
			this.Adapt();
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00016D0C File Offset: 0x00014F0C
		public override void WindowVertical()
		{
			this.bHorizontal = false;
			this.Adapt();
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00016D20 File Offset: 0x00014F20
		private void Adapt()
		{
			this.Title.SetPosition(new Rect(0f, 0f, this.oWindowRect[this.State].width, 120f));
			this.UserName.SetPosition(new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f));
			this.Password.SetPosition(new Rect(50f, 505f, this.oWindowRect[this.State].width - 100f, 80f));
			this.Postscript.SetPosition(new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f));
			this.PSHintText.SetPosition(new Rect(50f, 400f, this.oWindowRect[this.State].width - 100f, 80f));
			this.PSReturnBtn.SetPosition(new Rect(this.oWindowRect[this.State].width - 50f - 200f - 30f - 200f, this.oWindowRect[this.State].height - 50f - 80f, 200f, 80f));
			this.PSConfirmBtn.SetPosition(new Rect(this.oWindowRect[this.State].width - 50f - 200f, this.oWindowRect[this.State].height - 50f - 80f, 200f, 80f));
			this.UploadingPercentText.SetPosition(new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f));
			this.ProgressBG.SetPosition(new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f));
			bool flag = this.bHorizontal;
			if (flag)
			{
				this.ProgressL.SetPosition(new Rect(50f, 295f, 50f, 80f));
				this.ProgressM.SetPosition(new Rect(55f, 295f, 50f, 80f));
				this.ProgressR.SetPosition(new Rect(200f, 295f, 50f, 80f));
			}
			else
			{
				this.ProgressL.SetPosition(new Rect(50f, 300f, 50f, 80f));
				this.ProgressM.SetPosition(new Rect(55f, 300f, 50f, 80f));
				this.ProgressR.SetPosition(new Rect(200f, 300f, 50f, 80f));
			}
			this.UploadingConfirmBtn.SetPosition(new Rect(this.oWindowRect[this.State].width - 50f - 320f, this.oWindowRect[this.State].height - 50f - 80f, 320f, 80f));
			this.IP.SetPosition(new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f));
			this.IPCheckBtn.SetPosition(new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f));
			this.IPConfirmBtn.SetPosition(new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f));
			this.UploadClose.SetPosition(new Rect(this.oWindowRect[this.State].width - 100f, 35f, 50f, 50f));
			this.OnlineLoginBtn.SetPosition(new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f));
			this.RememberMe.SetPosition(new Rect(this.oWindowRect[this.State].width - 220f, 440f, 220f, 40f));
			this.HintIcon.SetPosition(new Rect(this.oWindowRect[this.State].width - 370f, 244f, 30f, 30f));
			this.HintText.SetPosition(new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f));
			this.ProjSelect.SetPosition(new Rect(50f, 295f, this.oWindowRect[this.State].width - 100f, 80f), new Rect(50f, 390f, this.oWindowRect[this.State].width - 100f, 350f));
			this.NoProjHint.SetPosition(new Rect(50f, 390f, this.oWindowRect[this.State].width - 100f, 350f));
			this.CreateProjBtn.SetPosition(new Rect(this.oWindowRect[this.State].width - 50f - 250f, 410f, 250f, 80f));
			this.UploadProjBtn.SetPosition(new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f));
			this.GOTHintIcon.SetPosition(new Rect(this.oWindowRect[this.State].width - 710f, 244f, 30f, 30f));
			this.GOTHintText.SetPosition(new Rect(50f, 220f, this.oWindowRect[this.State].width - 100f, 80f));
			this.ProjName.SetPosition(new Rect(280f, 170f, this.oWindowRect[this.State].width - 280f - 50f, 80f));
			this.PackageName.SetPosition(new Rect(280f, 290f, this.oWindowRect[this.State].width - 280f - 50f, 80f));
			this.ProjGameType.SetPosition(new Rect(280f, 410f, this.oWindowRect[this.State].width - 280f - 50f, 80f), new Rect(280f, 500f, this.oWindowRect[this.State].width - 280f - 50f, 240f));
			this.ProjGameSubType.SetPosition(new Rect(280f, 520f, this.oWindowRect[this.State].width - 280f - 50f, 80f), new Rect(280f, 605f, this.oWindowRect[this.State].width - 280f - 50f, 190f));
			this.CreateProj.SetPosition(new Rect(280f, 640f, this.oWindowRect[this.State].width - 280f - 50f, 80f));
			this.IPCheckBtn.SetPosition(new Rect(50f, 640f, this.oWindowRect[this.State].width - 100f, 80f));
		}

		// Token: 0x040001BF RID: 447
		private bool bHorizontal = true;

		// Token: 0x040001C0 RID: 448
		private bool ip_on = true;

		// Token: 0x040001C1 RID: 449
		private ULabel Title;

		// Token: 0x040001C2 RID: 450
		private UToolBar Mode;

		// Token: 0x040001C3 RID: 451
		private ULabel OnlineIcon;

		// Token: 0x040001C4 RID: 452
		private ULabel OnlineText;

		// Token: 0x040001C5 RID: 453
		private static string[] ProjectList;

		// Token: 0x040001C6 RID: 454
		private bool createProjShow = false;

		// Token: 0x040001C7 RID: 455
		private UButton UploadClose;

		// Token: 0x040001C8 RID: 456
		private ULabel UserNameText;

		// Token: 0x040001C9 RID: 457
		private UTextField UserName;

		// Token: 0x040001CA RID: 458
		private ULabel PasswordText;

		// Token: 0x040001CB RID: 459
		private PasswordTextField Password;

		// Token: 0x040001CC RID: 460
		private UToggle RememberMe;

		// Token: 0x040001CD RID: 461
		private UButton OnlineLoginBtn;

		// Token: 0x040001CE RID: 462
		private ULabel HintIcon;

		// Token: 0x040001CF RID: 463
		private ULabel HintText;

		// Token: 0x040001D0 RID: 464
		private ULabel LoginHint;

		// Token: 0x040001D1 RID: 465
		private UButton SwitchIcon;

		// Token: 0x040001D2 RID: 466
		private UButton SwitchBtn;

		// Token: 0x040001D3 RID: 467
		private ULabel UploadProjText;

		// Token: 0x040001D4 RID: 468
		private UDropDown ProjSelect;

		// Token: 0x040001D5 RID: 469
		private ULabel NoProjHint;

		// Token: 0x040001D6 RID: 470
		private UButton RefreshIcon;

		// Token: 0x040001D7 RID: 471
		private UButton RefreshBtn;

		// Token: 0x040001D8 RID: 472
		private UButton CreateProjBtn;

		// Token: 0x040001D9 RID: 473
		private UButton UploadProjBtn;

		// Token: 0x040001DA RID: 474
		private ULabel BalanceHint;

		// Token: 0x040001DB RID: 475
		private bool isSelectProj = false;

		// Token: 0x040001DC RID: 476
		private UButton ReturnBtn;

		// Token: 0x040001DD RID: 477
		private ULabel ProjNameLabel;

		// Token: 0x040001DE RID: 478
		private ULabel PackageNameLabel;

		// Token: 0x040001DF RID: 479
		private ULabel ProjTypeNameLabel;

		// Token: 0x040001E0 RID: 480
		private UTextField ProjName;

		// Token: 0x040001E1 RID: 481
		private UTextField PackageName;

		// Token: 0x040001E2 RID: 482
		private ULabel ProjNameHint;

		// Token: 0x040001E3 RID: 483
		private UDropDown ProjGameType;

		// Token: 0x040001E4 RID: 484
		private UDropDown ProjGameSubType;

		// Token: 0x040001E5 RID: 485
		private UButton CreateProj;

		// Token: 0x040001E6 RID: 486
		private ULabel PostscriptText;

		// Token: 0x040001E7 RID: 487
		private UTextField Postscript;

		// Token: 0x040001E8 RID: 488
		private UToggle PSHintText;

		// Token: 0x040001E9 RID: 489
		private UButton PSReturnBtn;

		// Token: 0x040001EA RID: 490
		private UButton PSConfirmBtn;

		// Token: 0x040001EB RID: 491
		private ULabel UploadingText;

		// Token: 0x040001EC RID: 492
		private ULabel ErrorText;

		// Token: 0x040001ED RID: 493
		private ULabel UploadingPercentText;

		// Token: 0x040001EE RID: 494
		private float uploadPercent;

		// Token: 0x040001EF RID: 495
		private ULabel ProgressBG;

		// Token: 0x040001F0 RID: 496
		private ULabel ProgressL;

		// Token: 0x040001F1 RID: 497
		private ULabel ProgressM;

		// Token: 0x040001F2 RID: 498
		private ULabel ProgressR;

		// Token: 0x040001F3 RID: 499
		private UButton UploadingConfirmBtn;

		// Token: 0x040001F4 RID: 500
		private ULabel IPText;

		// Token: 0x040001F5 RID: 501
		private UTextField IP;

		// Token: 0x040001F6 RID: 502
		private ULabel GOTHintIcon;

		// Token: 0x040001F7 RID: 503
		private ULabel GOTHintText;

		// Token: 0x040001F8 RID: 504
		private ULabel GOTStateHint;

		// Token: 0x040001F9 RID: 505
		private UButton IPCheckBtn;

		// Token: 0x040001FA RID: 506
		private UButton IPConfirmBtn;
	}
}
