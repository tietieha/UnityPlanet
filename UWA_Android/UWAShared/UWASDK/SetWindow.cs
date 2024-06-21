using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;

namespace UWASDK
{
	// Token: 0x02000036 RID: 54
	public class SetWindow : UWindow
	{
		// Token: 0x060001EA RID: 490 RVA: 0x0000FD2C File Offset: 0x0000DF2C
		public SetWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000FE4C File Offset: 0x0000E04C
		public override void WindowInitialize()
		{
			bool flag = !SharedUtils.Dev;
			if (flag)
			{
				SdkUIMgr.Get().configMode = overview.ConfigMode.Minimal;
				this.HaveText = true;
			}
			this.yOffset = (float)(SharedUtils.Dev ? 0 : -130);
			this.ModeOptions.Clear();
			List<string> list = new List<string>();
			int num = 0;
			this.ModeOptions.Add(new SetWindow.ModeOption("Overview", eGotMode.Overview, new Action(this.OverdrawOptionView)));
			this.SetModeCurNum[SdkUIMgr.SetMode.OVERVIEW] = num++;
			list.Add("Overview");
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				bool flag2 = SdkUIMgr.Get().ShowFeature("mono");
				if (flag2)
				{
					this.ModeOptions.Add(new SetWindow.ModeOption("Mono", eGotMode.Mono, new Action(this.MonoOptionView)));
					list.Add("Mono");
					this.SetModeCurNum[SdkUIMgr.SetMode.MONO] = num++;
				}
				this.ModeOptions.Add(new SetWindow.ModeOption("Resources", eGotMode.Resources, new Action(this.ResourcesOptionView)));
				list.Add("Resources");
				this.SetModeCurNum[SdkUIMgr.SetMode.RESOURCES] = num++;
				bool flag3 = SdkUIMgr.Get().ShowFeature("lua");
				if (flag3)
				{
					this.ModeOptions.Add(new SetWindow.ModeOption("Lua", eGotMode.Lua, new Action(this.LuaOptionView)));
					list.Add("Lua");
					this.SetModeCurNum[SdkUIMgr.SetMode.LUA] = num++;
				}
				bool flag4 = SdkUIMgr.Get().ShowFeature("gpu");
				if (flag4)
				{
					this.ModeOptions.Add(new SetWindow.ModeOption("GPU", eGotMode.Gpu, new Action(this.GpuOptionView)));
					list.Add("GPU");
					this.SetModeCurNum[SdkUIMgr.SetMode.GPU] = num++;
				}
			}
			this.SetBtns = new UToolBar(null, TextureLoader.Instance.Get("BtnBG-1"), list.ToArray(), StyleController.Instance.GetStyle("ButToggleGroup", "label"), new Rect(25f, 25f, (float)(256 * list.Count), 120f), UToolBar.State.Horizontal, "ToggleBg", 1f, 5f);
			this.Close = new UButton(new Action(this.OnClickedClose), TextureLoader.Instance.Get("Close"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(1420f, 25f, 50f, 50f), 1f);
			this.DirectBtn = new UButton(new Action(this.OnClickedDMBtn), "Direct Mode (Relaunch)", StyleController.Instance.GetStyle("DmBtn", "label"), new Rect(630f, 680f + this.yOffset, 530f, 96f), 1f);
			this.SaveAndStart = new UButton(new Action(this.OnClickedSaveAndStart), "Save & Start", StyleController.Instance.GetStyle("SaveBtn", "label"), new Rect(1180f, 680f + this.yOffset, 310f, 96f), 1f);
			this.set_VerticalLine = new ULabel(TextureLoader.Instance.Get("SetLine-2"), StyleController.Instance.GetStyle("VerLine", "label"), new Rect(250f, 40f, 2f, 1325f), 1f);
			this.set_HorizontalLine = new ULabel(TextureLoader.Instance.Get("SetLine-1"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(25f, 1360f, 850f, 5f), 1f);
			this.controlList.Add(this.SetBtns);
			this.controlList.Add(this.Close);
			this.controlList.Add(this.DirectBtn);
			this.controlList.Add(this.SaveAndStart);
			this.controlList.Add(this.set_VerticalLine);
			this.controlList.Add(this.set_HorizontalLine);
			this.OverdrawOptionInitialize();
			this.MonoOptionIntialize();
			this.ResourcesOptionInitialize();
			this.LuaOptionIntialize();
			this.GpuOptionInitialize();
			this.LoadAllPageCtrlsState();
		}

		// Token: 0x060001EC RID: 492 RVA: 0x000102E8 File Offset: 0x0000E4E8
		public override void WindowView(int windowID)
		{
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.SetBtns.View();
				this.SetBtns.SetText(this.SetModeCurNum[SdkUIMgr.SetMode.RESOURCES], (this.State == WindowState.Horizontal) ? "Resources" : "Resour\nces");
			}
			this.Close.View();
			bool flag = PlayerPrefs.GetInt("First") != 1;
			if (flag)
			{
				this.SetBtns.Selected = SdkUIMgr.Get().CurSetModeNum;
				PlayerPrefs.SetInt("First", 1);
			}
			bool flag2 = this.SetBtns.Selected >= 0 && this.SetBtns.Selected < this.ModeOptions.Count;
			if (flag2)
			{
				this.ModeOptions[this.SetBtns.Selected].Call();
				SdkUIMgr.Get().ChangeSetMode((SdkUIMgr.SetMode)this.SetBtns.Selected);
			}
			bool flag3 = this.State == WindowState.Vertical;
			if (flag3)
			{
				bool dev2 = SharedUtils.Dev;
				if (dev2)
				{
					this.set_VerticalLine.View();
				}
				this.set_HorizontalLine.View();
			}
			this.DirectBtn.View();
			this.SaveAndStart.View();
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001044C File Offset: 0x0000E64C
		public override void WindowHorizontal()
		{
			this.SetBtns.SetPosition(new Rect(25f, 25f, (float)(256 * this.SetBtns.Options.Length), 110f));
			this.SetBtns.SetState(UToolBar.State.Horizontal);
			this.SetBtns.SetInterval(30f);
			this.Close.SetPosition(new Rect(1400f, 25f, 50f, 50f));
			this.DirectBtn.SetPosition(new Rect(630f, 680f + this.yOffset, 530f, 96f));
			this.SaveAndStart.SetPosition(new Rect(1180f, 680f + this.yOffset, 310f, 96f));
			this.OverdrawOptionHorizontal();
			this.MonoOptionHorizontal();
			this.ResourcesOptionHorizontal();
			this.LuaOptionHorizontal();
			this.GpuOptionHorizontal();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00010550 File Offset: 0x0000E750
		public override void WindowVertical()
		{
			this.SetBtns.SetPosition(new Rect(25f, 25f, 220f, (float)(150 * this.SetBtns.Options.Length)));
			this.SetBtns.SetState(UToolBar.State.Vertical);
			this.SetBtns.SetInterval(30f);
			this.Close.SetPosition(new Rect(800f, 30f, 50f, 50f));
			this.DirectBtn.SetPosition(new Rect(30f, 1380f, 530f, 96f));
			this.SaveAndStart.SetPosition(new Rect(590f, 1380f, 310f, 96f));
			this.OverdrawOptionVertical();
			this.MonoOptionVertical();
			this.ResourcesOptionVertical();
			this.LuaOptionVertical();
			this.GpuOptionVertical();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00010648 File Offset: 0x0000E848
		private void OverdrawOptionInitialize()
		{
			this.Unchangeable &= SharedUtils.Dev;
			Dictionary<int, Rect> value = new Dictionary<int, Rect>
			{
				{
					0,
					new Rect(100f, 230f + this.yOffset, 100f, 10f)
				},
				{
					1,
					new Rect(1075f, 230f + this.yOffset, 100f, 10f)
				},
				{
					2,
					new Rect(425f, 230f + this.yOffset, 100f, 10f)
				},
				{
					3,
					new Rect(750f, 230f + this.yOffset, 100f, 10f)
				}
			};
			this.ButtonLabRect.Add(WindowState.Horizontal, value);
			Dictionary<int, Rect> value2 = new Dictionary<int, Rect>
			{
				{
					0,
					new Rect(325f, 185f, 100f, 10f)
				},
				{
					1,
					new Rect(930f, 185f, 100f, 10f)
				},
				{
					2,
					new Rect(520f, 185f, 100f, 10f)
				},
				{
					3,
					new Rect(725f, 185f, 100f, 10f)
				}
			};
			this.ButtonLabRect.Add(WindowState.Vertical, value2);
			this.Hint.Add(WindowState.Horizontal, this.HorHint);
			this.Hint.Add(WindowState.Vertical, this.VerHint);
			float num = 25f;
			bool flag = this.State == WindowState.Horizontal;
			if (flag)
			{
				this.ov_Customize = new UButton(new Action(this.CustomizeAction), "自定义", StyleController.Instance.GetStyle("OverviewBtn-1", "label"), new Rect(num, 140f + this.yOffset, 256f, 120f), 1f);
				this.ov_ButtonLab = new ULabel(TextureLoader.Instance.Get("BtnTab"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(SharedUtils.Dev ? num : (num + 70f), 220f + this.yOffset, 100f, 30f), 1f);
				this.ov_Buttons.Add(this.ov_Customize);
				num += 325f;
				this.ov_CpuMode = new UButton(new Action(this.CpuModeAction), "CPU模式", StyleController.Instance.GetStyle("OverviewBtn", "label"), new Rect(num, 140f + this.yOffset, 256f, 120f), 1f);
				this.ov_Buttons.Add(this.ov_CpuMode);
				num += 325f;
				this.ov_MemoryMode = new UButton(new Action(this.MemoryModeAction), "内存模式", StyleController.Instance.GetStyle("OverviewBtn", "label"), new Rect(num, 140f + this.yOffset, 256f, 120f), 1f);
				this.ov_Buttons.Add(this.ov_MemoryMode);
				num += 325f;
				this.ov_MinMode = new UButton(new Action(this.MinModeAction), "极简模式", StyleController.Instance.GetStyle(SharedUtils.Dev ? "OverviewBtn" : "OverviewBtn-1", "label"), SharedUtils.Dev ? new Rect(num, 140f + this.yOffset, 256f, 120f) : new Rect(25f, 140f + this.yOffset, 256f, 120f), 1f);
				this.ov_Buttons.Add(this.ov_MinMode);
				num += 325f;
			}
			this.ov_HorizontalLine_1 = new ULabel(TextureLoader.Instance.Get("SetLine-1"), StyleController.Instance.GetStyle("HorLine", "label"), new Rect(10f, 240f + this.yOffset, 24000f, 2f), 1f);
			num = 75f;
			this.ov_HintText = new ULabel("", StyleController.Instance.GetStyle("Left", "label"), new Rect(num, 260f, 1500f, 50f), 1f);
			this.ov_StackLabel = new ULabel("堆栈获取：", StyleController.Instance.GetStyle("EmptyRight", "label"), new Rect(0f, 260f + this.yOffset, 250f, 60f), 1f);
			this.ov_Stack = new UToggle(TextureLoader.Instance.Get("Switch-2"), TextureLoader.Instance.Get("Switch-1"), "", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(260f, 260f + this.yOffset, 100f, 60f), 1f);
			this.ov_LuaLabel = new ULabel("Lua内存：", StyleController.Instance.GetStyle("EmptyRight", "label"), new Rect(50f, 460f + this.yOffset, 200f, 60f), 1f);
			this.ov_LuaTg = new UToggle(TextureLoader.Instance.Get("Switch-2"), TextureLoader.Instance.Get("Switch-1"), "", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(260f, 460f + this.yOffset, 100f, 60f), 1f);
			num += 355f;
			this.ov_DetailedStack = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "详细堆栈", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(num, 260f + this.yOffset, 300f, 60f), 1f);
			this.ov_UNF = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "Unity API", StyleController.Instance.GetStyle("RecToggle", "label"), SdkUIMgr.Get().ShowFeature("lua") ? new Rect(num, 360f + this.yOffset, 300f, 60f) : new Rect(num + 710f, 260f + this.yOffset, 300f, 60f), 1f);
			num += 355f;
			this.ov_LuaStack = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "Lua堆栈", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(num, 260f + this.yOffset, 300f, 60f), 1f);
			num += 355f;
			this.ov_Timeline = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "Timeline", StyleController.Instance.GetStyle("RecToggle", "label"), SdkUIMgr.Get().ShowFeature("lua") ? new Rect(num, 260f + this.yOffset, 300f, 60f) : new Rect(num - 355f, 260f + this.yOffset, 300f, 60f), 1f);
			this.ov_LuaOptions = new UToolBar(TextureLoader.Instance.Get("Checkround-1"), TextureLoader.Instance.Get("Checkround-2"), new string[]
			{
				"Auto",
				"Manual"
			}, StyleController.Instance.GetStyle("EmptyLeft", "label"), new Rect(430f, 460f + this.yOffset, 1400f, 60f), UToolBar.State.Horizontal, "ToggleBg", 1f, 5f);
			this.ov_FILabel = new ULabel("00 Frames", StyleController.Instance.GetStyle("label", "label"), new Rect(810f, 460f + this.yOffset, 300f, 60f), 1f);
			this.ov_FramesInput = new FrameTextField("10", StyleController.Instance.GetStyle("textfield", "label"), new Rect(630f, 460f + this.yOffset, 150f, 60f), null, "", 1f);
			this.ov_ResLabel = new ULabel("Resources：", StyleController.Instance.GetStyle("EmptyRight", "label"), SdkUIMgr.Get().ShowFeature("lua") ? new Rect(0f, 560f + this.yOffset, 250f, 60f) : new Rect(0f, 360f + this.yOffset, 250f, 60f), 1f);
			this.ov_ResourcesTg = new UToggle(TextureLoader.Instance.Get("Switch-2"), TextureLoader.Instance.Get("Switch-1"), "", StyleController.Instance.GetStyle("RecToggle", "label"), SdkUIMgr.Get().ShowFeature("lua") ? new Rect(260f, 560f + this.yOffset, 300f, 60f) : new Rect(260f, 360f + this.yOffset, 300f, 60f), 1f);
			this.ov_RSManagerLable = new ULabel("资源管理：", StyleController.Instance.GetStyle("EmptyRight", "label"), new Rect(50f, SdkUIMgr.Get().ShowFeature("lua") ? (660f + this.yOffset) : (460f + this.yOffset), 200f, 60f), 1f);
			this.ov_RSManagerTg = new UToggle(TextureLoader.Instance.Get("Switch-2"), TextureLoader.Instance.Get("Switch-1"), "", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(260f, SdkUIMgr.Get().ShowFeature("lua") ? (660f + this.yOffset) : (460f + this.yOffset), 300f, 60f), 1f);
			this.ov_HorizontalLine_2 = new ULabel(TextureLoader.Instance.Get("SetLine-1"), StyleController.Instance.GetStyle("HorLine", "label"), new Rect(50f, 650f + this.yOffset, 1400f, 2f), 1f);
			this.ov_Scroll = new UScrollView(this, new Rect(0f, 250f + this.yOffset, 1700f, 400f), new Rect(0f, 250f + this.yOffset, 1600f, 500f), UWAPanel.st, 0f, 1f, "");
			this.ov_VerScroll = new UScrollView(this, new Rect(300f, 50f, 600f, 185f), new Rect(300f, 50f, 1000f, 180f), UWAPanel.st, 0f, 1f, "");
			this.controlList.Add(this.ov_ButtonLab);
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.controlList.Add(this.ov_Customize);
				this.controlList.Add(this.ov_CpuMode);
				this.controlList.Add(this.ov_MemoryMode);
			}
			this.controlList.Add(this.ov_MinMode);
			this.controlList.Add(this.ov_Scroll);
			this.controlList.Add(this.ov_HintText);
			this.controlList.Add(this.ov_VerScroll);
			this.controlList.Add(this.ov_DetailedStack);
			this.controlList.Add(this.ov_UNF);
			this.controlList.Add(this.ov_LuaStack);
			this.controlList.Add(this.ov_Timeline);
			this.controlList.Add(this.ov_StackLabel);
			this.controlList.Add(this.ov_Stack);
			this.controlList.Add(this.ov_HorizontalLine_1);
			this.controlList.Add(this.ov_LuaLabel);
			this.controlList.Add(this.ov_LuaTg);
			this.controlList.Add(this.ov_LuaOptions);
			this.controlList.Add(this.ov_FILabel);
			this.controlList.Add(this.ov_FramesInput);
			this.controlList.Add(this.ov_ResLabel);
			this.controlList.Add(this.ov_ResourcesTg);
			this.controlList.Add(this.ov_RSManagerLable);
			this.controlList.Add(this.ov_RSManagerTg);
			this.controlList.Add(this.ov_HorizontalLine_2);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0001146C File Offset: 0x0000F66C
		private void OverdrawOptionView()
		{
			bool flag = this.State == WindowState.Vertical && SharedUtils.Dev;
			if (flag)
			{
				this.ov_VerScroll.View();
				bool isAnim = SdkUIMgr.Get().isAnim;
				if (isAnim)
				{
					this.ov_VerScroll.ScrollVectorMove((this.animLimite == 0f) ? 10f : -10f, 0f);
				}
				bool flag2 = this.ov_VerScroll.GetScrollVector().x >= 240f || this.ov_VerScroll.GetScrollVector().x <= 0f;
				if (flag2)
				{
					SdkUIMgr.Get().isAnim = false;
				}
			}
			this.ov_ButtonLab.View();
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.ov_Customize.View();
				this.ov_CpuMode.View();
				this.ov_MemoryMode.View();
			}
			this.ov_MinMode.View();
			bool flag3 = this.State == WindowState.Vertical && SharedUtils.Dev;
			if (flag3)
			{
				GUI.EndScrollView();
			}
			this.ov_HorizontalLine_1.View();
			bool flag4 = SdkUIMgr.Get().ShowFeature("lua") && this.State == WindowState.Horizontal;
			if (flag4)
			{
				this.ov_Scroll.View();
			}
			GUI.enabled = this.Unchangeable;
			bool flag5 = SdkUIMgr.Get().configMode > overview.ConfigMode.Custom;
			if (flag5)
			{
				this.ov_HintText.View(this.Hint[this.State][(int)SdkUIMgr.Get().configMode], true);
			}
			this.ov_StackLabel.View();
			this.ov_Stack.View();
			bool enabled = GUI.enabled;
			bool flag6 = !this.ov_Stack.IsSelected;
			if (flag6)
			{
				this.ov_DetailedStack.IsSelected = false;
				this.ov_LuaStack.IsSelected = false;
				this.ov_Timeline.IsSelected = false;
				this.ov_UNF.IsSelected = false;
			}
			bool flag7 = SdkUIMgr.Get().ShowFeature("unity_api");
			if (flag7)
			{
				enabled = GUI.enabled;
				GUI.enabled &= this.ov_Stack.IsSelected;
				this.ov_UNF.View();
				GUI.enabled = enabled;
			}
			bool flag8 = SdkUIMgr.Get().ShowFeature("time_line");
			if (flag8)
			{
				enabled = GUI.enabled;
				GUI.enabled &= this.ov_Stack.IsSelected;
				this.ov_Timeline.View();
				this.ov_DetailedStack.View();
				GUI.enabled = enabled;
			}
			bool flag9 = SdkUIMgr.Get().ShowFeature("unity_loading");
			if (flag9)
			{
				this.ov_RSManagerLable.View();
				this.ov_RSManagerTg.View();
			}
			bool flag10 = SdkUIMgr.Get().ShowFeature("lua");
			if (flag10)
			{
				enabled = GUI.enabled;
				GUI.enabled &= this.ov_Stack.IsSelected;
				this.ov_LuaStack.View();
				GUI.enabled = enabled;
				this.ov_LuaLabel.View();
				this.ov_LuaTg.View();
				enabled = GUI.enabled;
				GUI.enabled &= (this.ov_LuaTg.IsSelected & this.ov_LuaOptions.Selected == 0);
				GUI.enabled = enabled;
				GUI.enabled &= this.ov_LuaTg.IsSelected;
				bool flag11 = !this.ov_LuaTg.IsSelected;
				if (flag11)
				{
					this.LuaSelect = false;
					this.ov_LuaOptions.Selected = -1;
					this.ov_FramesInput.Text = "-1";
				}
				bool flag12 = this.ov_LuaTg.IsSelected && this.LuaSelect != this.ov_LuaTg.IsSelected;
				if (flag12)
				{
					this.LuaSelect = this.ov_LuaTg.IsSelected;
					this.ov_LuaOptions.Selected = 0;
					this.ov_FramesInput.Text = "10";
				}
				bool enabled2 = GUI.enabled;
				GUI.enabled &= (this.ov_LuaOptions.Selected != 1);
				this.ov_FramesInput.View();
				GUI.enabled = enabled2;
				this.ov_LuaOptions.View();
				this.ov_FILabel.View();
				GUI.enabled = enabled;
			}
			this.ov_ResLabel.View();
			this.ov_ResourcesTg.View();
			bool flag13 = SdkUIMgr.Get().ShowFeature("lua") && this.State == WindowState.Horizontal;
			if (flag13)
			{
				GUI.EndScrollView();
			}
			GUI.enabled = true;
			bool flag14 = this.State == WindowState.Horizontal;
			if (flag14)
			{
				this.ov_HorizontalLine_2.View();
			}
			SdkUIMgr.Get().bStack = this.ov_Stack.IsSelected;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x000119B0 File Offset: 0x0000FBB0
		private void OverdrawOptionHorizontal()
		{
			this.ov_ButtonLab.SetPosition(SharedUtils.Dev ? this.ButtonLabRect[this.State][(int)SdkUIMgr.Get().configMode] : new Rect(100f, 225f + this.yOffset, 100f, 10f));
			float num = (float)((SdkUIMgr.Get().ShowFeature("time_line") && SdkUIMgr.Get().ShowFeature("unity_api")) ? 0 : -100);
			int num2 = 25;
			this.ov_Customize.SetPosition(new Rect((float)num2, 140f + this.yOffset, 256f, 120f));
			num2 += 325;
			this.ov_CpuMode.SetPosition(new Rect((float)num2, 140f + this.yOffset, 256f, 120f));
			num2 += 325;
			this.ov_MemoryMode.SetPosition(new Rect((float)num2, 140f + this.yOffset, 256f, 120f));
			num2 += 325;
			this.ov_MinMode.SetPosition(SharedUtils.Dev ? new Rect((float)num2, 140f + this.yOffset, 256f, 120f) : new Rect(25f, 140f + this.yOffset, 256f, 120f));
			this.ov_HorizontalLine_1.SetPosition(new Rect(0f, 240f + this.yOffset, 1600f, 2f));
			this.ov_HintText.SetPosition(new Rect(50f, 260f + this.yOffset, 1500f, 50f));
			num2 = 75;
			float num3 = (float)(this.HaveText ? 65 : 0);
			this.ov_Stack.SetPosition(new Rect(260f, 240f + this.yOffset + num3, 120f, 96f));
			this.ov_StackLabel.SetPosition(new Rect(0f, 260f + this.yOffset + num3, 250f, 60f));
			this.ov_LuaLabel.SetPosition(new Rect(50f, 460f + this.yOffset + num3 + num, 200f, 60f));
			num2 += 355;
			this.ov_DetailedStack.SetPosition(new Rect((float)num2, 260f + this.yOffset + num3, 300f, 60f));
			this.ov_UNF.SetPosition(SdkUIMgr.Get().ShowFeature("lua") ? (SdkUIMgr.Get().ShowFeature("time_line") ? new Rect((float)num2, 360f + this.yOffset + num3, 300f, 60f) : new Rect((float)num2, 360f + this.yOffset + num3 - 100f, 300f, 60f)) : (SdkUIMgr.Get().ShowFeature("time_line") ? new Rect((float)(num2 + 710), 260f + this.yOffset + num3, 300f, 60f) : new Rect((float)num2, 260f + this.yOffset + num3, 300f, 60f)));
			num2 += 355;
			this.ov_LuaStack.SetPosition(new Rect((float)num2, 260f + this.yOffset + num3, 300f, 60f));
			num2 += 355;
			this.ov_Timeline.SetPosition(SdkUIMgr.Get().ShowFeature("lua") ? new Rect((float)num2, 260f + this.yOffset + num3, 300f, 60f) : new Rect((float)(num2 - 355), 260f + this.yOffset + num3, 300f, 60f));
			float num4 = (float)(SdkUIMgr.Get().ShowFeature("lua") ? ((SdkUIMgr.Get().ShowFeature("time_line") && SdkUIMgr.Get().ShowFeature("unity_api")) ? 0 : -100) : -200);
			this.ov_RSManagerLable.SetPosition(new Rect(50f, 654f + this.yOffset + num3 + num4, 200f, 60f));
			this.ov_RSManagerTg.SetPosition(new Rect(260f, 634f + this.yOffset + num3 + num4, 120f, 96f));
			this.ov_LuaTg.SetPosition(new Rect(260f, 440f + this.yOffset + num3 + num, 120f, 96f));
			this.ov_LuaOptions.SetPosition(new Rect(430f, 460f + this.yOffset + num3 + num, 1400f, 60f));
			this.ov_LuaOptions.SetState(UToolBar.State.Horizontal);
			this.ov_LuaOptions.SetInterval(400f);
			this.ov_FILabel.SetPosition(new Rect(810f, 460f + this.yOffset + num3 + num, 300f, 60f));
			this.ov_FramesInput.SetPosition(new Rect(630f, 460f + this.yOffset + num3 + num, 150f, 60f));
			this.ov_ResLabel.SetPosition(new Rect(0f, 560f + this.yOffset + num3 + num4, 250f, 60f));
			this.ov_ResourcesTg.SetPosition(new Rect(260f, 540f + this.yOffset + num3 + num4, 120f, 96f));
			this.ov_HorizontalLine_2.SetPosition(new Rect(0f, 670f + this.yOffset, 1600f, 2f));
			this.ov_Scroll.SetPosition(new Rect(0f, 250f + this.yOffset, 1500f, 420f), new Rect(0f, 250f + this.yOffset, 1450f, 620f));
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00012058 File Offset: 0x00010258
		private void OverdrawOptionVertical()
		{
			float num = (float)(SharedUtils.Dev ? 0 : -250);
			float num2 = (float)(SharedUtils.Dev ? 0 : -70);
			float num3 = (float)(this.HaveText ? 100 : 0);
			this.ov_VerScroll.SetPosition(new Rect(260f, 0f, 600f, 220f), new Rect(280f, 20f, 840f, 190f));
			this.ov_ButtonLab.SetPosition(SharedUtils.Dev ? this.ButtonLabRect[this.State][(int)SdkUIMgr.Get().configMode] : new Rect(110f, 115f, 100f, 10f));
			int num4 = 250;
			this.ov_Customize.SetPosition(new Rect((float)num4, 100f, 256f, 120f));
			num4 += 200;
			this.ov_CpuMode.SetPosition(new Rect((float)num4, 100f, 256f, 120f));
			num4 += 200;
			this.ov_MemoryMode.SetPosition(new Rect((float)num4, 100f, 256f, 120f));
			num4 += 200;
			this.ov_MinMode.SetPosition(SharedUtils.Dev ? new Rect((float)num4, 100f, 256f, 120f) : new Rect(25f, 100f + num2, 256f, 120f));
			this.ov_HorizontalLine_1.SetPosition(new Rect(250f + num, 190f + num2, 1000f, 2f));
			this.ov_HintText.SetPosition(new Rect(270f + num, 210f + num2, 620f, 120f));
			this.ov_StackLabel.SetPosition(new Rect(250f + num, 240f + num2 + num3, 250f, 60f));
			this.ov_Stack.SetPosition(new Rect(500f + num, 220f + num2 + num3, 120f, 96f));
			this.ov_DetailedStack.SetPosition(new Rect(290f + num, 345f + num2 + num3, 250f, 60f));
			this.ov_LuaStack.SetPosition(new Rect(550f + num, 345f + num2 + num3, 300f, 60f));
			this.ov_Timeline.SetPosition(new Rect(290f + num, 430f + num2 + num3, 300f, 60f));
			Vector2 vector = (!SdkUIMgr.Get().ShowFeature("lua") && !SdkUIMgr.Get().ShowFeature("time_line")) ? new Vector2(-260f, -85f) : new Vector2(0f, 0f);
			this.ov_UNF.SetPosition(new Rect(550f + num + vector.x, 430f + num2 + num3 + vector.y, 300f, 60f));
			this.ov_LuaLabel.SetPosition(new Rect(250f + num, 530f + num2 + num3, 230f, 60f));
			this.ov_LuaTg.SetPosition(new Rect(500f + num, 510f + num2 + num3, 120f, 96f));
			this.ov_LuaOptions.SetPosition(new Rect(280f + num, 630f + num2 + num3, 1400f, 170f));
			this.ov_LuaOptions.SetState(UToolBar.State.Vertical);
			this.ov_LuaOptions.SetInterval(20f);
			this.ov_FramesInput.SetPosition(new Rect(470f + num, 630f + num2 + num3, 130f, 60f));
			this.ov_FILabel.SetPosition(new Rect(610f + num, 630f + num2 + num3, 300f, 60f));
			float num5 = (float)(SdkUIMgr.Get().ShowFeature("lua") ? (SdkUIMgr.Get().ShowFeature("time_line") ? 0 : -300) : (SdkUIMgr.Get().ShowFeature("time_line") ? -300 : -400));
			this.ov_ResLabel.SetPosition(new Rect(250f + num, 830f + num2 + num3 + num5, 250f, 60f));
			this.ov_ResourcesTg.SetPosition(new Rect(500f + num, 810f + num2 + num3 + num5, 120f, 96f));
			this.ov_RSManagerLable.SetPosition(new Rect(280f + num, 920f + num2 + num3 + num5, 200f, 60f));
			this.ov_RSManagerTg.SetPosition(new Rect(500f + num, 910f + num2 + num3 + num5, 120f, 96f));
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000125C8 File Offset: 0x000107C8
		private void OverviewButtonAction(UButton target, overview.ConfigMode mode)
		{
			target.SetStyle(StyleController.Instance.GetStyle("OverviewBtn-1", "label"));
			for (int i = 0; i < this.ov_Buttons.Count; i++)
			{
				bool flag = this.ov_Buttons[i] == target;
				if (!flag)
				{
					this.ov_Buttons[i].SetStyle(StyleController.Instance.GetStyle("OverviewBtn", "label"));
				}
			}
			SdkUIMgr.Get().configMode = mode;
			bool flag2 = this.State == WindowState.Horizontal;
			if (flag2)
			{
				this.OverdrawOptionHorizontal();
			}
			else
			{
				this.OverdrawOptionVertical();
			}
			UWAPanel.ConfigInst.SetOverviewConfigMode(mode);
			this.LoadAllPageCtrlsState();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00012698 File Offset: 0x00010898
		private void AnimationScroll(overview.ConfigMode mode)
		{
			bool flag = mode == overview.ConfigMode.Custom || mode == overview.ConfigMode.CPU;
			if (flag)
			{
				this.animLimite = 0f;
			}
			else
			{
				this.animLimite = 240f;
			}
			bool flag2 = this.State == WindowState.Vertical;
			if (flag2)
			{
				SdkUIMgr.Get().isAnim = true;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x000126F8 File Offset: 0x000108F8
		private void CustomizeAction()
		{
			this.HaveText = false;
			this.AnimationScroll(overview.ConfigMode.Custom);
			this.OverviewButtonAction(this.ov_Customize, overview.ConfigMode.Custom);
			this.Unchangeable = true;
			this.UpdateCustomCtrlsState();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00012728 File Offset: 0x00010928
		private void CpuModeAction()
		{
			this.HaveText = true;
			this.AnimationScroll(overview.ConfigMode.CPU);
			this.TempSaveCustomState();
			this.OverviewButtonAction(this.ov_CpuMode, overview.ConfigMode.CPU);
			this.Unchangeable = false;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00012758 File Offset: 0x00010958
		private void MemoryModeAction()
		{
			this.HaveText = true;
			this.AnimationScroll(overview.ConfigMode.Memory);
			this.TempSaveCustomState();
			this.OverviewButtonAction(this.ov_MemoryMode, overview.ConfigMode.Memory);
			this.Unchangeable = false;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00012788 File Offset: 0x00010988
		private void MinModeAction()
		{
			this.HaveText = true;
			this.AnimationScroll(overview.ConfigMode.Minimal);
			this.TempSaveCustomState();
			this.OverviewButtonAction(this.ov_MinMode, overview.ConfigMode.Minimal);
			this.Unchangeable = false;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x000127B8 File Offset: 0x000109B8
		private void MonoOptionIntialize()
		{
			this.MonoOptions = new UToolBar(TextureLoader.Instance.Get("Checkround-1"), TextureLoader.Instance.Get("Checkround-2"), new string[]
			{
				"Auto",
				"Manual"
			}, StyleController.Instance.GetStyle("EmptyLeft", "label"), new Rect(75f, 180f, 1400f, 60f), UToolBar.State.Horizontal, StyleController.Instance.GetStyle("ToggleBg", "label"), 1f, 5f);
			this.mono_FramesInput = new FrameTextField("10", StyleController.Instance.GetStyle("textfield", "label"), new Rect(280f, 180f, 160f, 60f), null, "", 1f);
			this.mono_FILabel = new ULabel("00 Frames", StyleController.Instance.GetStyle("label", "label"), new Rect(450f, 180f, 300f, 60f), 1f);
			this.controlList.Add(this.MonoOptions);
			this.controlList.Add(this.mono_FramesInput);
			this.controlList.Add(this.mono_FILabel);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00012918 File Offset: 0x00010B18
		private void MonoOptionView()
		{
			bool enabled = GUI.enabled;
			GUI.enabled &= (this.MonoOptions.Selected == 0);
			this.mono_FramesInput.View();
			this.mono_FILabel.View();
			GUI.enabled = enabled;
			this.MonoOptions.View();
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00012978 File Offset: 0x00010B78
		private void MonoOptionHorizontal()
		{
			this.MonoOptions.SetPosition(new Rect(75f, 180f, 1400f, 60f));
			this.MonoOptions.SetState(UToolBar.State.Horizontal);
			this.MonoOptions.SetInterval(400f);
			this.mono_FramesInput.SetPosition(new Rect(280f, 180f, 160f, 60f));
			this.mono_FILabel.SetPosition(new Rect(450f, 180f, 300f, 60f));
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00012A18 File Offset: 0x00010C18
		private void MonoOptionVertical()
		{
			this.MonoOptions.SetPosition(new Rect(340f, 100f, 1400f, 400f));
			this.MonoOptions.SetState(UToolBar.State.Vertical);
			this.MonoOptions.SetInterval(150f);
			this.mono_FramesInput.SetPosition(new Rect(360f, 190f, 160f, 60f));
			this.mono_FILabel.SetPosition(new Rect(540f, 190f, 300f, 60f));
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00012AB8 File Offset: 0x00010CB8
		private void ResourcesOptionInitialize()
		{
			this.rs_ResourcesMG = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "资源管理", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect(75f, 180f, 250f, 60f), 1f);
			this.rs_NoFeature = new ULabel("暂无可配置项", StyleController.Instance.GetStyle("GrayCenter", "label"), new Rect(50f, 370f, 1400f, 100f), 1f);
			this.controlList.Add(this.rs_ResourcesMG);
			this.controlList.Add(this.rs_NoFeature);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00012B90 File Offset: 0x00010D90
		private void ResourcesOptionView()
		{
			bool flag = SdkUIMgr.Get().ShowFeature("unity_loading");
			if (flag)
			{
				this.rs_ResourcesMG.View();
			}
			else
			{
				this.rs_NoFeature.View();
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00012BDC File Offset: 0x00010DDC
		private void ResourcesOptionHorizontal()
		{
			this.rs_ResourcesMG.SetPosition(new Rect(75f, 180f, 250f, 60f));
			this.rs_NoFeature.SetPosition(new Rect(50f, 370f, 1400f, 100f));
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00012C38 File Offset: 0x00010E38
		private void ResourcesOptionVertical()
		{
			this.rs_ResourcesMG.SetPosition(new Rect(320f, 80f, 250f, 60f));
			this.rs_NoFeature.SetPosition(new Rect(270f, 500f, 600f, 100f));
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00012C94 File Offset: 0x00010E94
		private void LuaOptionIntialize()
		{
			this.LuaOptions = new UToolBar(TextureLoader.Instance.Get("Checkround-1"), TextureLoader.Instance.Get("Checkround-2"), new string[]
			{
				"Auto",
				"Manual"
			}, StyleController.Instance.GetStyle("EmptyLeft", "label"), new Rect(75f, 180f, 1400f, 60f), UToolBar.State.Horizontal, StyleController.Instance.GetStyle("ToggleBg", "label"), 1f, 5f);
			this.lua_FramesInput = new FrameTextField("10", StyleController.Instance.GetStyle("textfield", "label"), new Rect(280f, 180f, 160f, 60f), null, "", 1f);
			this.lua_FILabel = new ULabel("00 Frames", StyleController.Instance.GetStyle("label", "label"), new Rect(450f, 180f, 300f, 60f), 1f);
			this.controlList.Add(this.LuaOptions);
			this.controlList.Add(this.lua_FramesInput);
			this.controlList.Add(this.lua_FILabel);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00012DF4 File Offset: 0x00010FF4
		private void LuaOptionView()
		{
			bool enabled = GUI.enabled;
			GUI.enabled &= (this.LuaOptions.Selected == 0);
			this.lua_FramesInput.View();
			this.lua_FILabel.View();
			GUI.enabled = enabled;
			this.LuaOptions.View();
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00012E54 File Offset: 0x00011054
		private void LuaOptionHorizontal()
		{
			this.LuaOptions.SetPosition(new Rect(75f, 180f, 1400f, 60f));
			this.LuaOptions.SetState(UToolBar.State.Horizontal);
			this.LuaOptions.SetInterval(400f);
			this.lua_FramesInput.SetPosition(new Rect(280f, 180f, 160f, 60f));
			this.lua_FILabel.SetPosition(new Rect(450f, 180f, 300f, 60f));
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00012EF4 File Offset: 0x000110F4
		private void LuaOptionVertical()
		{
			this.LuaOptions.SetPosition(new Rect(340f, 100f, 1400f, 400f));
			this.LuaOptions.SetState(UToolBar.State.Vertical);
			this.LuaOptions.SetInterval(150f);
			this.lua_FramesInput.SetPosition(new Rect(360f, 190f, 160f, 60f));
			this.lua_FILabel.SetPosition(new Rect(540f, 190f, 300f, 60f));
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00012F94 File Offset: 0x00011194
		private void GpuOptionInitialize()
		{
			int num = 75;
			this.gpu_texture_analysis = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "纹理资源分析", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect((float)num, 180f, 350f, 60f), 1f);
			num += 425;
			this.gpu_mesh_analysis = new UToggle(TextureLoader.Instance.Get("Checkbox-1"), TextureLoader.Instance.Get("Checkbox-2"), "网格资源分析", StyleController.Instance.GetStyle("RecToggle", "label"), new Rect((float)num, 180f, 350f, 60f), 1f);
			this.gpu_NoFeature = new ULabel("暂无可配置项", StyleController.Instance.GetStyle("GrayCenter", "label"), new Rect(50f, 370f, 1400f, 100f), 1f);
			this.controlList.Add(this.gpu_texture_analysis);
			this.controlList.Add(this.gpu_mesh_analysis);
			this.controlList.Add(this.gpu_NoFeature);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000130E4 File Offset: 0x000112E4
		private void GpuOptionView()
		{
			bool flag = SdkUIMgr.Get().ShowFeature("gpu_res");
			if (flag)
			{
				this.gpu_texture_analysis.View();
				this.gpu_mesh_analysis.View();
			}
			else
			{
				this.gpu_NoFeature.View();
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0001313C File Offset: 0x0001133C
		private void GpuOptionHorizontal()
		{
			int num = 75;
			this.gpu_texture_analysis.SetPosition(new Rect((float)num, 160f, 350f, 60f));
			num += 425;
			this.gpu_mesh_analysis.SetPosition(new Rect((float)num, 160f, 350f, 60f));
			this.gpu_NoFeature.SetPosition(new Rect(50f, 370f, 1400f, 100f));
		}

		// Token: 0x06000208 RID: 520 RVA: 0x000131C4 File Offset: 0x000113C4
		private void GpuOptionVertical()
		{
			int num = 0;
			this.gpu_texture_analysis.SetPosition(new Rect(340f, 60f, 350f, 60f));
			num += 120;
			this.gpu_mesh_analysis.SetPosition(new Rect(340f, (float)(60 + num), 350f, 60f));
			this.gpu_NoFeature.SetPosition(new Rect(270f, 500f, 600f, 100f));
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0001324C File Offset: 0x0001144C
		private void OnClickedClose()
		{
			this.SaveAllPageCtrlsState();
			SdkStartConfig.Store();
			SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SELECT);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00013268 File Offset: 0x00011468
		private void LoadAllPageCtrlsState()
		{
			bool flag = !SharedUtils.Dev;
			if (flag)
			{
				UWAPanel.ConfigInst.SetOverviewConfigMode(overview.ConfigMode.Minimal);
			}
			this.ov_Stack.IsSelected = UWAPanel.ConfigInst.overview.engine_cpu_stack;
			this.ov_DetailedStack.IsSelected = (UWAPanel.ConfigInst.overview.stack_detail == 1);
			this.ov_LuaStack.IsSelected = UWAPanel.ConfigInst.overview.lua_cpu_stack;
			this.ov_Timeline.IsSelected = UWAPanel.ConfigInst.overview.time_line;
			this.ov_UNF.IsSelected = UWAPanel.ConfigInst.overview.unity_api;
			this.ov_LuaTg.IsSelected = UWAPanel.ConfigInst.overview.lua;
			this.ov_LuaOptions.Selected = (UWAPanel.ConfigInst.overview.lua ? ((UWAPanel.ConfigInst.overview.lua_dump_step == 0) ? 1 : 0) : -1);
			this.ov_FramesInput.Text = ((UWAPanel.ConfigInst.overview.lua_dump_step == 0) ? "10" : UWAPanel.ConfigInst.overview.lua_dump_step.ToString());
			bool flag2 = UWAPanel.ConfigInst.overview.lua_dump_step == -1 && SdkUIMgr.Get().ShowFeature("lua");
			if (flag2)
			{
				this.ov_FramesInput.Text = "10";
			}
			this.ov_ResourcesTg.IsSelected = (UWAPanel.ConfigInst.overview.resources == 2);
			this.ov_RSManagerTg.IsSelected = UWAPanel.ConfigInst.overview.unity_loading;
			this.MonoOptions.Selected = ((UWAPanel.ConfigInst.mono.mono_dump_step == 0) ? 1 : 0);
			this.mono_FramesInput.Text = ((UWAPanel.ConfigInst.mono.mono_dump_step == 0) ? "10" : UWAPanel.ConfigInst.mono.mono_dump_step.ToString());
			bool flag3 = UWAPanel.ConfigInst.mono.mono_dump_step == -1 && SdkUIMgr.Get().ShowFeature("mono");
			if (flag3)
			{
				this.mono_FramesInput.Text = "10";
			}
			this.rs_ResourcesMG.IsSelected = UWAPanel.ConfigInst.resources.unity_loading;
			this.LuaOptions.Selected = ((UWAPanel.ConfigInst.lua.lua_dump_step == 0) ? 1 : 0);
			this.lua_FramesInput.Text = ((UWAPanel.ConfigInst.lua.lua_dump_step == 0) ? "10" : UWAPanel.ConfigInst.lua.lua_dump_step.ToString());
			bool flag4 = UWAPanel.ConfigInst.lua.lua_dump_step == -1 && SdkUIMgr.Get().ShowFeature("lua");
			if (flag4)
			{
				this.lua_FramesInput.Text = "10";
			}
			this.gpu_texture_analysis.IsSelected = UWAPanel.ConfigInst.gpu.texture_analysis;
			this.gpu_mesh_analysis.IsSelected = UWAPanel.ConfigInst.gpu.mesh_analysis;
			this.LuaSelect = this.ov_LuaTg.IsSelected;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x000135F0 File Offset: 0x000117F0
		private void SaveAllPageCtrlsState()
		{
			bool flag = string.IsNullOrEmpty(this.ov_FramesInput.Text);
			if (flag)
			{
				this.ov_FramesInput.Text = "10";
			}
			bool flag2 = string.IsNullOrEmpty(this.mono_FramesInput.Text);
			if (flag2)
			{
				this.mono_FramesInput.Text = "10";
			}
			bool flag3 = string.IsNullOrEmpty(this.lua_FramesInput.Text);
			if (flag3)
			{
				this.lua_FramesInput.Text = "10";
			}
			UWAPanel.ConfigInst.overview.engine_cpu_stack = this.ov_Stack.IsSelected;
			UWAPanel.ConfigInst.overview.stack_detail = ((!this.ov_DetailedStack.IsSelected) ? 0 : (this.ov_DetailedStack.IsSelected ? 1 : 2));
			UWAPanel.ConfigInst.overview.lua_cpu_stack = this.ov_LuaStack.IsSelected;
			UWAPanel.ConfigInst.overview.time_line = this.ov_Timeline.IsSelected;
			UWAPanel.ConfigInst.overview.unity_api = this.ov_UNF.IsSelected;
			UWAPanel.ConfigInst.overview.lua = this.ov_LuaTg.IsSelected;
			UWAPanel.ConfigInst.overview.lua_mem_stack = (this.ov_Stack.IsSelected ? (this.ov_LuaTg.IsSelected && this.ov_LuaStack.IsSelected) : this.ov_LuaTg.IsSelected);
			UWAPanel.ConfigInst.overview.lua_dump_step = ((this.ov_LuaOptions.Selected == 1) ? 0 : int.Parse(this.ov_FramesInput.Text));
			UWAPanel.ConfigInst.overview.resources = (this.ov_ResourcesTg.IsSelected ? 2 : ((UWAPanel.ConfigInst.overview.mode == overview.ConfigMode.Custom && SharedUtils.Dev) ? 1 : 0));
			UWAPanel.ConfigInst.overview.unity_loading = this.ov_RSManagerTg.IsSelected;
			UWAPanel.ConfigInst.overview.mode = SdkUIMgr.Get().configMode;
			UWAPanel.ConfigInst.mono.mono_dump_step = ((this.MonoOptions.Selected == 1) ? 0 : int.Parse(this.mono_FramesInput.Text));
			UWAPanel.ConfigInst.resources.unity_loading = this.rs_ResourcesMG.IsSelected;
			UWAPanel.ConfigInst.lua.lua_dump_step = ((this.LuaOptions.Selected == 1) ? 0 : int.Parse(this.lua_FramesInput.Text));
			UWAPanel.ConfigInst.gpu.texture_analysis = this.gpu_texture_analysis.IsSelected;
			UWAPanel.ConfigInst.gpu.mesh_analysis = this.gpu_mesh_analysis.IsSelected;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x000138F8 File Offset: 0x00011AF8
		private void TempSaveCustomState()
		{
			bool flag = SdkUIMgr.Get().configMode > overview.ConfigMode.Custom;
			if (!flag)
			{
				UWAPanel.UseTempSave = true;
				bool flag2 = string.IsNullOrEmpty(this.ov_FramesInput.Text);
				if (flag2)
				{
					this.ov_FramesInput.Text = "10";
				}
				bool flag3 = string.IsNullOrEmpty(this.mono_FramesInput.Text);
				if (flag3)
				{
					this.mono_FramesInput.Text = "10";
				}
				bool flag4 = string.IsNullOrEmpty(this.lua_FramesInput.Text);
				if (flag4)
				{
					this.lua_FramesInput.Text = "10";
				}
				UWAPanel.BufConfigInst.overview.engine_cpu_stack = this.ov_Stack.IsSelected;
				UWAPanel.BufConfigInst.overview.stack_detail = ((!this.ov_DetailedStack.IsSelected) ? 0 : (this.ov_DetailedStack.IsSelected ? 1 : 2));
				UWAPanel.BufConfigInst.overview.lua_cpu_stack = this.ov_LuaStack.IsSelected;
				UWAPanel.BufConfigInst.overview.time_line = this.ov_Timeline.IsSelected;
				UWAPanel.BufConfigInst.overview.unity_api = this.ov_UNF.IsSelected;
				UWAPanel.BufConfigInst.overview.lua = this.ov_LuaTg.IsSelected;
				UWAPanel.BufConfigInst.overview.lua_mem_stack = (this.ov_Stack.IsSelected ? (this.ov_LuaTg.IsSelected && this.ov_LuaStack.IsSelected) : this.ov_LuaTg.IsSelected);
				UWAPanel.BufConfigInst.overview.lua_dump_step = ((this.ov_LuaOptions.Selected == 1) ? 0 : int.Parse(this.ov_FramesInput.Text));
				UWAPanel.BufConfigInst.overview.resources = (this.ov_ResourcesTg.IsSelected ? 2 : ((UWAPanel.BufConfigInst.overview.mode == overview.ConfigMode.Custom) ? 1 : 0));
				UWAPanel.BufConfigInst.overview.unity_loading = this.ov_RSManagerTg.IsSelected;
				UWAPanel.ConfigInst.overview.mode = SdkUIMgr.Get().configMode;
				UWAPanel.BufConfigInst.mono.mono_dump_step = ((this.MonoOptions.Selected == 1) ? 0 : int.Parse(this.mono_FramesInput.Text));
				UWAPanel.BufConfigInst.resources.unity_loading = this.rs_ResourcesMG.IsSelected;
				UWAPanel.BufConfigInst.lua.lua_dump_step = ((this.LuaOptions.Selected == 1) ? 0 : int.Parse(this.lua_FramesInput.Text));
				UWAPanel.BufConfigInst.gpu.texture_analysis = this.gpu_texture_analysis.IsSelected;
				UWAPanel.BufConfigInst.gpu.mesh_analysis = this.gpu_mesh_analysis.IsSelected;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00013C14 File Offset: 0x00011E14
		private void UpdateCustomCtrlsState()
		{
			bool flag = !UWAPanel.UseTempSave;
			if (!flag)
			{
				bool flag2 = !SharedUtils.Dev;
				if (flag2)
				{
					UWAPanel.BufConfigInst.SetOverviewConfigMode(overview.ConfigMode.Minimal);
				}
				this.ov_Stack.IsSelected = UWAPanel.BufConfigInst.overview.engine_cpu_stack;
				this.ov_DetailedStack.IsSelected = (UWAPanel.BufConfigInst.overview.stack_detail == 1);
				this.ov_LuaStack.IsSelected = UWAPanel.BufConfigInst.overview.lua_cpu_stack;
				this.ov_Timeline.IsSelected = UWAPanel.BufConfigInst.overview.time_line;
				this.ov_UNF.IsSelected = UWAPanel.BufConfigInst.overview.unity_api;
				this.ov_LuaTg.IsSelected = UWAPanel.BufConfigInst.overview.lua;
				this.ov_LuaOptions.Selected = (UWAPanel.BufConfigInst.overview.lua ? ((UWAPanel.BufConfigInst.overview.lua_dump_step == 0) ? 1 : 0) : -1);
				this.ov_FramesInput.Text = ((UWAPanel.BufConfigInst.overview.lua_dump_step == 0) ? "10" : UWAPanel.BufConfigInst.overview.lua_dump_step.ToString());
				this.ov_ResourcesTg.IsSelected = (UWAPanel.BufConfigInst.overview.resources == 2);
				this.ov_RSManagerTg.IsSelected = UWAPanel.BufConfigInst.overview.unity_loading;
				this.MonoOptions.Selected = ((UWAPanel.BufConfigInst.mono.mono_dump_step == 0) ? 1 : 0);
				this.mono_FramesInput.Text = ((UWAPanel.BufConfigInst.mono.mono_dump_step == 0) ? "10" : UWAPanel.BufConfigInst.mono.mono_dump_step.ToString());
				this.rs_ResourcesMG.IsSelected = UWAPanel.BufConfigInst.resources.unity_loading;
				this.LuaOptions.Selected = ((UWAPanel.BufConfigInst.lua.lua_dump_step == 0) ? 1 : 0);
				this.lua_FramesInput.Text = ((UWAPanel.BufConfigInst.lua.lua_dump_step == 0) ? "10" : UWAPanel.BufConfigInst.lua.lua_dump_step.ToString());
				this.gpu_texture_analysis.IsSelected = UWAPanel.BufConfigInst.gpu.texture_analysis;
				this.gpu_mesh_analysis.IsSelected = UWAPanel.BufConfigInst.gpu.mesh_analysis;
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00013ED8 File Offset: 0x000120D8
		private void OnClickedSaveAndStart()
		{
			this.SaveAllPageCtrlsState();
			SdkStartConfig.Store();
			SdkUIMgr.Get().ChangeMode(this.ModeOptions[this.SetBtns.Selected].Mode);
			SdkUIMgr.Get().isBack = false;
			bool flag = SdkUIMgr.Get().infoStates.Count <= 0 || (this.SetBtns.Selected != this.SetModeCurNum[SdkUIMgr.SetMode.OVERVIEW] && this.SetBtns.Selected != this.SetModeCurNum[SdkUIMgr.SetMode.GPU]);
			if (flag)
			{
				SdkUIMgr.Get().SaveAndStart();
			}
			else
			{
				SdkUIMgr.Get().SetFuncPtr(new SdkUIMgr.FuncPtr(SdkUIMgr.Get().SaveAndStart));
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.INFO);
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00013FC4 File Offset: 0x000121C4
		private void OnClickedDMBtn()
		{
			this.SaveAllPageCtrlsState();
			SdkStartConfig.Store();
			SdkUIMgr.Get().isBack = false;
			SdkUIMgr.Get().ChangeMode(this.ModeOptions[this.SetBtns.Selected].Mode);
			bool flag = SdkUIMgr.Get().infoStates.Count <= 0 || (this.SetBtns.Selected != 0 && this.SetBtns.Selected != 4);
			if (flag)
			{
				SdkUIMgr.Get().DirectRelaunch();
			}
			else
			{
				SdkUIMgr.Get().SetFuncPtr(new SdkUIMgr.FuncPtr(SdkUIMgr.Get().DirectRelaunch));
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.INFO);
			}
		}

		// Token: 0x0400016A RID: 362
		private UToolBar SetBtns;

		// Token: 0x0400016B RID: 363
		private UButton Close;

		// Token: 0x0400016C RID: 364
		private UButton DirectBtn;

		// Token: 0x0400016D RID: 365
		private UButton SaveAndStart;

		// Token: 0x0400016E RID: 366
		private ULabel set_VerticalLine;

		// Token: 0x0400016F RID: 367
		private ULabel set_HorizontalLine;

		// Token: 0x04000170 RID: 368
		private List<SetWindow.ModeOption> ModeOptions = new List<SetWindow.ModeOption>();

		// Token: 0x04000171 RID: 369
		private float yOffset = 0f;

		// Token: 0x04000172 RID: 370
		private Dictionary<WindowState, Dictionary<int, Rect>> ButtonLabRect = new Dictionary<WindowState, Dictionary<int, Rect>>();

		// Token: 0x04000173 RID: 371
		public Dictionary<SdkUIMgr.SetMode, int> SetModeCurNum = new Dictionary<SdkUIMgr.SetMode, int>
		{
			{
				SdkUIMgr.SetMode.OVERVIEW,
				-1
			},
			{
				SdkUIMgr.SetMode.MONO,
				-1
			},
			{
				SdkUIMgr.SetMode.RESOURCES,
				-1
			},
			{
				SdkUIMgr.SetMode.LUA,
				-1
			},
			{
				SdkUIMgr.SetMode.GPU,
				-1
			}
		};

		// Token: 0x04000174 RID: 372
		private UToggle ov_DetailedStack;

		// Token: 0x04000175 RID: 373
		private UToggle ov_Stack;

		// Token: 0x04000176 RID: 374
		private UToggle ov_LuaStack;

		// Token: 0x04000177 RID: 375
		private UToggle ov_Timeline;

		// Token: 0x04000178 RID: 376
		private UToggle ov_UNF;

		// Token: 0x04000179 RID: 377
		private ULabel ov_StackLabel;

		// Token: 0x0400017A RID: 378
		private ULabel ov_LuaLabel;

		// Token: 0x0400017B RID: 379
		private ULabel ov_ResLabel;

		// Token: 0x0400017C RID: 380
		private ULabel ov_RSManagerLable;

		// Token: 0x0400017D RID: 381
		private ULabel ov_HorizontalLine_1;

		// Token: 0x0400017E RID: 382
		private UToggle ov_LuaTg;

		// Token: 0x0400017F RID: 383
		private UToolBar ov_LuaOptions;

		// Token: 0x04000180 RID: 384
		private ULabel ov_FILabel;

		// Token: 0x04000181 RID: 385
		private FrameTextField ov_FramesInput;

		// Token: 0x04000182 RID: 386
		private UToggle ov_ResourcesTg;

		// Token: 0x04000183 RID: 387
		private UToggle ov_RSManagerTg;

		// Token: 0x04000184 RID: 388
		private ULabel ov_HorizontalLine_2;

		// Token: 0x04000185 RID: 389
		public UScrollView ov_Scroll;

		// Token: 0x04000186 RID: 390
		public UScrollView ov_VerScroll;

		// Token: 0x04000187 RID: 391
		private UButton ov_Customize;

		// Token: 0x04000188 RID: 392
		private UButton ov_CpuMode;

		// Token: 0x04000189 RID: 393
		private UButton ov_MemoryMode;

		// Token: 0x0400018A RID: 394
		private UButton ov_MinMode;

		// Token: 0x0400018B RID: 395
		private ULabel ov_ButtonLab;

		// Token: 0x0400018C RID: 396
		private ULabel ov_HintText;

		// Token: 0x0400018D RID: 397
		private List<UControl> ov_Buttons = new List<UControl>();

		// Token: 0x0400018E RID: 398
		private bool Unchangeable = true;

		// Token: 0x0400018F RID: 399
		private bool LuaSelect = false;

		// Token: 0x04000190 RID: 400
		private bool HaveText = false;

		// Token: 0x04000191 RID: 401
		private List<string> HorHint = new List<string>
		{
			"",
			"极简模式不额外占用性能开销，您可准确获取各类指标数值以及报告评分。",
			"CPU模式专注于定位CPU性能瓶颈，您可通过此模式查看更细节的函数堆栈信息。",
			"内存模式专注于定位内存使用问题，您可通过此模式查看更细节的内存分布信息。"
		};

		// Token: 0x04000192 RID: 402
		private List<string> VerHint = new List<string>
		{
			"",
			"极简模式不额外占用性能开销，您可准确获取\n各类指标数值以及报告评分。",
			"CPU模式专注于定位CPU性能瓶颈，您可通过\n此模式查看更细节的函数堆栈信息。",
			"内存模式专注于定位内存使用问题，您可通过\n此模式查看更细节的内存分布信息。"
		};

		// Token: 0x04000193 RID: 403
		private Dictionary<WindowState, List<string>> Hint = new Dictionary<WindowState, List<string>>();

		// Token: 0x04000194 RID: 404
		private float animLimite;

		// Token: 0x04000195 RID: 405
		private UToolBar MonoOptions;

		// Token: 0x04000196 RID: 406
		private FrameTextField mono_FramesInput;

		// Token: 0x04000197 RID: 407
		private ULabel mono_FILabel;

		// Token: 0x04000198 RID: 408
		private UToggle rs_ResourcesMG;

		// Token: 0x04000199 RID: 409
		private ULabel rs_NoFeature;

		// Token: 0x0400019A RID: 410
		private UToolBar LuaOptions;

		// Token: 0x0400019B RID: 411
		private FrameTextField lua_FramesInput;

		// Token: 0x0400019C RID: 412
		private ULabel lua_FILabel;

		// Token: 0x0400019D RID: 413
		private UToggle gpu_texture_analysis;

		// Token: 0x0400019E RID: 414
		private UToggle gpu_mesh_analysis;

		// Token: 0x0400019F RID: 415
		private ULabel gpu_NoFeature;

		// Token: 0x020000F9 RID: 249
		private class ModeOption
		{
			// Token: 0x060009C2 RID: 2498 RVA: 0x00047810 File Offset: 0x00045A10
			public ModeOption(string t, eGotMode m, Action a)
			{
				this.Title = t;
				this.Call = a;
				this.Mode = m;
			}

			// Token: 0x0400065B RID: 1627
			public string Title;

			// Token: 0x0400065C RID: 1628
			public eGotMode Mode;

			// Token: 0x0400065D RID: 1629
			public Action Call;
		}
	}
}
