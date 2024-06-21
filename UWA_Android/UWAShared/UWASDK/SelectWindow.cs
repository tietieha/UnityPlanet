using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;

namespace UWASDK
{
	// Token: 0x02000034 RID: 52
	public class SelectWindow : UWindow
	{
		// Token: 0x060001CC RID: 460 RVA: 0x0000DF4C File Offset: 0x0000C14C
		public SelectWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000DFB4 File Offset: 0x0000C1B4
		public override void WindowInitialize()
		{
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.OverivewBtn = new UButton(new Action(this.OnClickedOverivew), "Overview", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(50f, 25f, 250f, 100f), 1f);
				this.MonoBtn = new UButton(new Action(this.OnClickedMono), "Mono", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(300f, 25f, 225f, 100f), 1f);
				this.ResourcesBtn = new UButton(new Action(this.OnClickedResources), "Resources", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(525f, 25f, 300f, 100f), 1f);
				this.LuaBtn = new UButton(new Action(this.OnClickedLua), "Lua", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(825f, 25f, 225f, 100f), 1f);
				this.GpuBtn = new UButton(new Action(this.OnClickedGpu), "GPU", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(1050f, 25f, 250f, 100f), 1f);
			}
			else
			{
				this.OverivewBtn = new UButton(new Action(this.OnClickedOverivew), "Overview", StyleController.Instance.GetStyle("SelectBtn", "label"), new Rect(50f, 25f, 250f, 100f), 1f);
			}
			int num = 0;
			this.selectBtns.Add(this.OverivewBtn);
			this.SetModeCurNum[SdkUIMgr.SetMode.OVERVIEW] = num++;
			bool dev2 = SharedUtils.Dev;
			if (dev2)
			{
				bool flag = SdkUIMgr.Get().ShowFeature("mono");
				if (flag)
				{
					this.selectBtns.Add(this.MonoBtn);
					this.SetModeCurNum[SdkUIMgr.SetMode.MONO] = num++;
				}
				this.selectBtns.Add(this.ResourcesBtn);
				this.SetModeCurNum[SdkUIMgr.SetMode.RESOURCES] = num++;
				bool flag2 = SdkUIMgr.Get().ShowFeature("lua");
				if (flag2)
				{
					this.selectBtns.Add(this.LuaBtn);
					this.SetModeCurNum[SdkUIMgr.SetMode.LUA] = num++;
				}
				bool flag3 = SdkUIMgr.Get().ShowFeature("gpu");
				if (flag3)
				{
					this.selectBtns.Add(this.GpuBtn);
					this.SetModeCurNum[SdkUIMgr.SetMode.GPU] = num++;
				}
				this.select_btnWidth = 1000 / this.selectBtns.Count;
				this.select_btnHeight = 480 / this.selectBtns.Count;
				for (int i = 0; i < this.selectBtns.Count; i++)
				{
					this.selectBtns[i].SetPosition(new Rect((float)(50 + this.select_btnWidth * i), 50f, (float)this.select_btnWidth, 50f));
				}
			}
			else
			{
				this.select_btnWidth = 300 / this.selectBtns.Count;
				this.select_btnHeight = 480 / this.selectBtns.Count;
				UControl ucontrol = this.selectBtns[0];
				float num2 = (float)50;
				int num3 = this.select_btnWidth;
				ucontrol.SetPosition(new Rect(num2 + (float)0, 50f, (float)this.select_btnWidth, 50f));
			}
			bool dev3 = SharedUtils.Dev;
			if (dev3)
			{
				this.st_VerticalLine = new ULabel(TextureLoader.Instance.Get("Interval"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(1050f, 15f, 4f, 120f), 1f);
			}
			else
			{
				this.st_VerticalLine = new ULabel(TextureLoader.Instance.Get("Interval"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(350f, 25f, 4f, 120f), 1f);
			}
			this.st_HorizontalLine = new ULabel(TextureLoader.Instance.Get("SetLine-1"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(0f, 200f, 1000f, 2f), 1f);
			bool dev4 = SharedUtils.Dev;
			if (dev4)
			{
				this.Gear = new UButton(new Action(this.OnClickedGear), TextureLoader.Instance.Get("Gear"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(1080f, 25f, 100f, 100f), 1f);
			}
			else
			{
				this.Gear = new UButton(new Action(this.OnClickedGear), TextureLoader.Instance.Get("Gear"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(380f, 25f, 100f, 100f), 1f);
			}
			this.controlList.Add(this.OverivewBtn);
			bool dev5 = SharedUtils.Dev;
			if (dev5)
			{
				this.controlList.Add(this.MonoBtn);
				this.controlList.Add(this.ResourcesBtn);
				this.controlList.Add(this.LuaBtn);
				this.controlList.Add(this.GpuBtn);
			}
			this.controlList.Add(this.st_VerticalLine);
			this.controlList.Add(this.st_HorizontalLine);
			this.controlList.Add(this.Gear);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000E60C File Offset: 0x0000C80C
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
			for (int i = 0; i < this.selectBtns.Count; i++)
			{
				this.selectBtns[i].View();
			}
			bool flag3 = this.State == WindowState.Horizontal;
			if (flag3)
			{
				this.st_VerticalLine.View();
			}
			else
			{
				bool flag4 = this.State == WindowState.Vertical;
				if (flag4)
				{
					this.st_HorizontalLine.View();
				}
			}
			this.Gear.View();
			GUI.DragWindow(new Rect(0f, 0f, 100000f, 100000f));
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000E740 File Offset: 0x0000C940
		public override void WindowHorizontal()
		{
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				for (int i = 0; i < this.selectBtns.Count; i++)
				{
					this.selectBtns[i].SetPosition(new Rect((float)(50 + this.select_btnWidth * i + 25), 50f, (float)(this.select_btnWidth - 50), 50f));
				}
				this.st_VerticalLine.SetPosition(new Rect(1050f, 15f, 4f, 120f));
				this.Gear.SetPosition(new Rect(1080f, 25f, 100f, 100f));
			}
			else
			{
				this.selectBtns[0].SetPosition(new Rect(25f, 50f, (float)this.select_btnWidth, 50f));
				this.st_VerticalLine.SetPosition(new Rect(350f, 25f, 4f, 120f));
				this.Gear.SetPosition(new Rect(380f, 25f, 100f, 100f));
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000E884 File Offset: 0x0000CA84
		public override void WindowVertical()
		{
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.Gear.SetPosition(new Rect(50f, 20f, 150f, 150f));
				for (int i = 0; i < this.selectBtns.Count; i++)
				{
					this.selectBtns[i].SetPosition(new Rect(25f, (float)(200 + (this.select_btnHeight + 20) * i + 25), 200f, (float)(this.select_btnHeight - 50)));
				}
			}
			else
			{
				this.Gear.SetPosition(new Rect(50f, 20f, 150f, 150f));
				this.selectBtns[0].SetPosition(new Rect(25f, 250f, 200f, 50f));
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000E980 File Offset: 0x0000CB80
		private void OnClickedOverivew()
		{
			bool flag = !this.CheckSet(SdkUIMgr.SetMode.OVERVIEW);
			if (!flag)
			{
				bool flag2 = !SharedUtils.Dev;
				if (flag2)
				{
					UWAPanel.ConfigInst.SetOverviewConfigMode(overview.ConfigMode.Minimal);
					SdkStartConfig.Store();
				}
				SdkUIMgr.Get().ChangeMode(eGotMode.Overview);
				SdkUIMgr.Get().bStack = UWAPanel.ConfigInst.overview.engine_cpu_stack;
				this.CheckGoInfoWindow();
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000E9F8 File Offset: 0x0000CBF8
		private void OnClickedLua()
		{
			bool flag = !this.CheckSet(SdkUIMgr.SetMode.LUA);
			if (!flag)
			{
				SdkUIMgr.Get().ChangeMode(eGotMode.Lua);
				SdkUIMgr.Get().TryStart();
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000EA38 File Offset: 0x0000CC38
		private void OnClickedMono()
		{
			bool flag = !this.CheckSet(SdkUIMgr.SetMode.MONO);
			if (!flag)
			{
				SdkUIMgr.Get().ChangeMode(eGotMode.Mono);
				SdkUIMgr.Get().TryStart();
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000EA78 File Offset: 0x0000CC78
		private void OnClickedResources()
		{
			bool flag = !this.CheckSet(SdkUIMgr.SetMode.RESOURCES);
			if (!flag)
			{
				SdkUIMgr.Get().ChangeMode(eGotMode.Resources);
				SdkUIMgr.Get().TryStart();
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000EAB8 File Offset: 0x0000CCB8
		private void OnClickedGpu()
		{
			bool flag = !this.CheckSet(SdkUIMgr.SetMode.GPU);
			if (!flag)
			{
				SdkUIMgr.Get().ChangeMode(eGotMode.Gpu);
				SdkUIMgr.Get().ChangeSetMode(SdkUIMgr.SetMode.GPU);
				this.CheckGoInfoWindow();
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000EB00 File Offset: 0x0000CD00
		private void OnClickedGear()
		{
			PlayerPrefs.SetInt("First", 1);
			SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SET);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000EB1C File Offset: 0x0000CD1C
		private void CheckGoInfoWindow()
		{
			bool flag = SdkUIMgr.Get().infoStates.Count <= 0;
			if (flag)
			{
				SdkUIMgr.Get().TryStart();
			}
			else
			{
				SdkUIMgr.Get().SetFuncPtr(new SdkUIMgr.FuncPtr(SdkUIMgr.Get().TryStart));
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.INFO);
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000EB84 File Offset: 0x0000CD84
		private bool CheckSet(SdkUIMgr.SetMode mode)
		{
			SdkUIMgr.Get().isBack = false;
			int @int = PlayerPrefs.GetInt("First");
			bool flag = @int != 1;
			bool result;
			if (flag)
			{
				SdkUIMgr.Get().ChangeState(SdkUIMgr.UIState.SET);
				SdkUIMgr.Get().ChangeSetMode(mode);
				SdkUIMgr.Get().CurSetModeNum = this.SetModeCurNum[mode];
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x04000149 RID: 329
		private UButton OverivewBtn;

		// Token: 0x0400014A RID: 330
		private UButton MonoBtn;

		// Token: 0x0400014B RID: 331
		private UButton ResourcesBtn;

		// Token: 0x0400014C RID: 332
		private UButton LuaBtn;

		// Token: 0x0400014D RID: 333
		private UButton GpuBtn;

		// Token: 0x0400014E RID: 334
		private int select_btnWidth;

		// Token: 0x0400014F RID: 335
		private int select_btnHeight;

		// Token: 0x04000150 RID: 336
		private List<UButton> selectBtns = new List<UButton>();

		// Token: 0x04000151 RID: 337
		private ULabel st_VerticalLine;

		// Token: 0x04000152 RID: 338
		private ULabel st_HorizontalLine;

		// Token: 0x04000153 RID: 339
		private UButton Gear;

		// Token: 0x04000154 RID: 340
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
	}
}
