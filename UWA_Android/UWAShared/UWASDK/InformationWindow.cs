using System;
using System.Collections.Generic;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000030 RID: 48
	public class InformationWindow : UWindow
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x0000CE18 File Offset: 0x0000B018
		public InformationWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000CE48 File Offset: 0x0000B048
		public void CreateInformation(string inf)
		{
			UWAPanel.Inst.informationOn = true;
			this.infomations.Add(inf);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000CE64 File Offset: 0x0000B064
		public override void WindowInitialize()
		{
			SdkUIMgr.Get().isBack = false;
			this.Information_icon = new ULabel(TextureLoader.Instance.Get("Information"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(35f, 35f, 50f, 50f), 1f);
			this.InformationLabel = new ULabel("提示", StyleController.Instance.GetStyle("Empty", "label"), new Rect(100f, 30f, 100f, 60f), 1f);
			this.InformationConfirm = new UButton(new Action(this.CloseInformation), "确认", StyleController.Instance.GetStyle("ConfirmBtn", "label"), new Rect(this.oWindowRect[this.State].width - 200f - 35f, this.oWindowRect[this.State].height - 35f - 80f, 200f, 80f), 1f);
			this.BackSelect = new UButton(new Action(this.BackSelectWindow), "取消", StyleController.Instance.GetStyle("CheckDenyBtn", "label"), new Rect(this.oWindowRect[this.State].width - 450f - 35f, this.oWindowRect[this.State].height - 35f - 80f, 200f, 80f), 1f);
			this.InformationText = new ULabel("", StyleController.Instance.GetStyle("Empty", "label"), new Rect(0f, 150f, this.oWindowRect[this.State].width, this.oWindowRect[this.State].height - 150f - 150f), 1f);
			this.NotPrompting = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "不再提示", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(this.oWindowRect[this.State].width - 300f - 35f, this.oWindowRect[this.State].height - 35f - 180f, 260f, 40f), 1f);
			this.controlList.Add(this.Information_icon);
			this.controlList.Add(this.InformationLabel);
			this.controlList.Add(this.InformationConfirm);
			this.controlList.Add(this.BackSelect);
			this.controlList.Add(this.InformationText);
			this.controlList.Add(this.NotPrompting);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000D1B0 File Offset: 0x0000B3B0
		public override void WindowView(int windowID)
		{
			bool flag = SdkUIMgr.Get().UiState == SdkUIMgr.UIState.INFO && SdkUIMgr.Get().infoStates.Count <= 0;
			if (flag)
			{
				bool flag2 = SdkUIMgr.Get().funcptr != null;
				if (flag2)
				{
					SdkUIMgr.Get().funcptr();
				}
			}
			else
			{
				foreach (object obj in Enum.GetValues(typeof(SdkUIMgr.InfoState)))
				{
					SdkUIMgr.InfoState infoState = (SdkUIMgr.InfoState)obj;
					bool flag3 = SdkUIMgr.Get().infoStates.Count > 0 && SdkUIMgr.Get().infoStates[0].Key == infoState;
					if (flag3)
					{
						PlayerPrefs.SetInt(SdkUIMgr.Get().infoMap[infoState], this.NotPrompting.IsSelected ? 1 : 0);
					}
				}
				bool flag4 = SdkUIMgr.Get().infoStates.Count > 0;
				if (flag4)
				{
					bool flag5 = SdkUIMgr.Get().UiState == SdkUIMgr.UIState.INFO && SdkUIMgr.Get().NeedBack.Contains(SdkUIMgr.Get().infoStates[0].Key);
					if (flag5)
					{
						this.BackSelect.View();
					}
					bool flag6 = SdkUIMgr.Get().infoStates[0].Key == SdkUIMgr.InfoState.Stack && (SdkUIMgr.Get().bStack || SdkUIMgr.Get().GetSetMode() != SdkUIMgr.SetMode.OVERVIEW || (SdkUIMgr.Get().GetSetMode() == SdkUIMgr.SetMode.OVERVIEW && SdkUIMgr.Get().configMode > overview.ConfigMode.Custom));
					if (flag6)
					{
						SdkUIMgr.Get().infoStates.RemoveAt(0);
						bool flag7 = !SdkUIMgr.Get().isBack && SdkUIMgr.Get().infoStates.Count == 0;
						if (flag7)
						{
							bool flag8 = SdkUIMgr.Get().funcptr != null;
							if (flag8)
							{
								SdkUIMgr.Get().funcptr();
							}
						}
					}
					this.Information_icon.View();
					this.InformationLabel.View();
					bool flag9 = this.infomations.Count > 0;
					if (flag9)
					{
						this.InformationText.View(this.infomations[0], true);
					}
					this.InformationConfirm.View();
					bool flag10 = SdkUIMgr.Get().infoStates.Count > 0 && SdkUIMgr.Get().UiState == SdkUIMgr.UIState.INFO;
					if (flag10)
					{
						this.NotPrompting.View();
						this.InformationText.View(SdkUIMgr.Get().infoStates[0].Value, true);
					}
				}
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000D4F4 File Offset: 0x0000B6F4
		private void CloseInformation()
		{
			this.NotPrompting.IsSelected = false;
			bool flag = this.infomations.Count > 0;
			if (flag)
			{
				this.infomations.RemoveAt(0);
			}
			bool flag2 = this.infomations.Count == 0;
			if (flag2)
			{
				UWAPanel.Inst.informationOn = false;
			}
			bool flag3 = SdkUIMgr.Get().UiState == SdkUIMgr.UIState.INFO && SdkUIMgr.Get().infoStates.Count > 0;
			if (flag3)
			{
				bool flag4 = PlayerPrefs.GetInt(SdkUIMgr.Get().infoMap[SdkUIMgr.Get().infoStates[0].Key]) != 1;
				if (flag4)
				{
					SdkUIMgr.Get().BufInfoStates.Add(SdkUIMgr.Get().infoStates[0]);
				}
				SdkUIMgr.Get().infoStates.RemoveAt(0);
			}
			bool flag5 = SdkUIMgr.Get().UiState == SdkUIMgr.UIState.INFO && SdkUIMgr.Get().infoStates.Count == 0;
			if (flag5)
			{
				bool flag6 = SdkUIMgr.Get().funcptr != null;
				if (flag6)
				{
					SdkUIMgr.Get().funcptr();
				}
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000D650 File Offset: 0x0000B850
		private void BackSelectWindow()
		{
			this.NotPrompting.IsSelected = false;
			SdkUIMgr.Get().isBack = true;
			for (int i = 0; i < SdkUIMgr.Get().infoStates.Count; i++)
			{
				bool flag = PlayerPrefs.GetInt(SdkUIMgr.Get().infoMap[SdkUIMgr.Get().infoStates[i].Key]) != 1;
				if (flag)
				{
					KeyValuePair<SdkUIMgr.InfoState, string> item = SdkUIMgr.Get().infoStates[i];
					SdkUIMgr.Get().BufInfoStates.Add(item);
				}
			}
			SdkUIMgr.Get().infoStates.Clear();
			for (int j = 0; j < SdkUIMgr.Get().BufInfoStates.Count; j++)
			{
				SdkUIMgr.Get().infoStates.Add(SdkUIMgr.Get().BufInfoStates[j]);
			}
			SdkUIMgr.Get().BufInfoStates.Clear();
			SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SELECT);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000D774 File Offset: 0x0000B974
		public void ClearInformation()
		{
			this.infomations.Clear();
			UWAPanel.Inst.informationOn = false;
		}

		// Token: 0x0400013C RID: 316
		private ULabel Information_icon;

		// Token: 0x0400013D RID: 317
		private ULabel InformationLabel;

		// Token: 0x0400013E RID: 318
		private UButton InformationConfirm;

		// Token: 0x0400013F RID: 319
		private UButton BackSelect;

		// Token: 0x04000140 RID: 320
		private UToggle NotPrompting;

		// Token: 0x04000141 RID: 321
		private List<string> infomations = new List<string>();

		// Token: 0x04000142 RID: 322
		private ULabel InformationText;
	}
}
