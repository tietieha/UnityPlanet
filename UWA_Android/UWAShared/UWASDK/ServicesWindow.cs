using System;
using System.Collections;
using UnityEngine;
using UWA;

namespace UWASDK
{
	// Token: 0x02000035 RID: 53
	public class ServicesWindow : UWindow
	{
		// Token: 0x060001D9 RID: 473 RVA: 0x0000EBF8 File Offset: 0x0000CDF8
		public ServicesWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f) : base(windowRect_h, windowRect_v, style, name, align, windowState, rate)
		{
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000EC40 File Offset: 0x0000CE40
		public override void WindowInitialize()
		{
			this.TimeBG = new ULabel("", StyleController.Instance.GetStyle("window", "label"), new Rect(0f, 0f, 240f, 180f), 1f);
			this.CurrentMode = new ULabel("", StyleController.Instance.GetStyle("ServiceModeText", "label"), new Rect(0f, 20f, 240f, 40f), 1f);
			this.ServiceLine = new ULabel(TextureLoader.Instance.Get("ServiceLine"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(20f, 80f, 200f, 4f), 1f);
			this.DeltaTime = new ULabel("0\"", StyleController.Instance.GetStyle("DeltaTimeText", "label"), new Rect(0f, 80f, 240f, 100f), 1f);
			this.controlList.Add(this.TimeBG);
			this.controlList.Add(this.CurrentMode);
			this.controlList.Add(this.ServiceLine);
			this.controlList.Add(this.DeltaTime);
			this.FoldInitialize();
			this.UnfoldInitialize();
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000EDBC File Offset: 0x0000CFBC
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
			this.TimeBG.View();
			string text = ServicesWindow.DisplayGotMode[(int)SdkCtrlData.Instance.GotMode];
			bool flag3 = SdkCtrlData.Instance.SdkMode == eSdkMode.GPM;
			if (flag3)
			{
				text = "GPM";
			}
			this.CurrentMode.View(text, true);
			this.DeltaTime.View(SharedUtils.durationStr, true);
			this.ServiceLine.View();
			bool flag4 = this.showBtns;
			if (flag4)
			{
				this.UnfoldView();
			}
			else
			{
				this.FoldView();
			}
			GUI.DragWindow(new Rect(0f, 0f, 100000f, 10000f));
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000EEF8 File Offset: 0x0000D0F8
		private void FoldInitialize()
		{
			this.Right = new UButton(new Action(this.Unfold), TextureLoader.Instance.Get("Right"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(250f, 0f, 60f, 180f), 1f);
			this.controlList.Add(this.Right);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000EF74 File Offset: 0x0000D174
		private void FoldView()
		{
			this.Right.View();
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000EF84 File Offset: 0x0000D184
		private void Unfold()
		{
			SdkUIMgr.Get().IsFold = true;
			bool flag = SdkCtrlData.Instance.GotMode == eGotMode.Overview;
			if (flag)
			{
				this.SetOptions(SdkStartConfig.Instance.overview.lua && SdkStartConfig.Instance.overview.lua_dump_step == 0, SdkCtrlData.Instance.SdkMode != eSdkMode.GPM);
				this.Dump2Text.Text = "Dump(Resources)";
			}
			bool flag2 = SdkCtrlData.Instance.GotMode == eGotMode.Lua;
			if (flag2)
			{
				this.SetOptions(SdkStartConfig.Instance.lua.lua_dump_step == 0, false);
			}
			bool flag3 = SdkCtrlData.Instance.GotMode == eGotMode.Mono;
			if (flag3)
			{
				this.SetOptions(false, SdkStartConfig.Instance.mono.mono_dump_step == 0);
				this.Dump2Text.Text = "Dump(Mono)";
			}
			bool flag4 = SdkCtrlData.Instance.GotMode == eGotMode.Resources;
			if (flag4)
			{
				this.SetOptions(false, false);
			}
			bool flag5 = SdkCtrlData.Instance.GotMode == eGotMode.Gpu;
			if (flag5)
			{
				this.SetOptions(false, true);
				this.Dump2Text.Text = "Dump(Overdraw)";
			}
			this.showBtns = true;
			this.oWindowRect[WindowState.Horizontal] = this.UnfoldWindowRect;
			this.oWindowRect[WindowState.Vertical] = this.UnfoldWindowRect;
			Rect rect = base.ScaleWindowRect(this.oWindowRect[this._windowState], this._rate, this._align);
			this.WindowRect = new Rect(this.WindowRect.x, this.WindowRect.y, rect.width, rect.height);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000F14C File Offset: 0x0000D34C
		private void UnfoldInitialize()
		{
			this.Left = new UButton(new Action(this.Fold), TextureLoader.Instance.Get("Left"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(250f, 0f, 60f, 180f), 1f);
			this.BtnBG = new ULabel("", "window", new Rect(270f, 0f, 685f, 180f), 1f);
			this.DumpLua = new UButton(new Action(this.OnClickedDumpLua), TextureLoader.Instance.Get("Dump-Round"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(380f, 30f, 80f, 80f), 1f);
			this.DumpLuaText = new ULabel("Dump(Lua)", StyleController.Instance.GetStyle("ServiceFunction", "label"), new Rect(320f, 120f, 200f, 40f), 1f);
			this.Dump2 = new UButton(new Action(this.OnClickedDump2), TextureLoader.Instance.Get("Snapshot-Round"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(605f, 30f, 80f, 80f), 1f);
			this.Dump2Text = new ULabel("Dump(Resources)", StyleController.Instance.GetStyle("ServiceFunction", "label"), new Rect(520f, 120f, 250f, 40f), 1f);
			this.ServiceLineH = new ULabel(TextureLoader.Instance.Get("ServiceLine-h"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(800f, 0f, 6f, 180f), 1f);
			this.Stop = new UButton(new Action(this.OnClickedStop), TextureLoader.Instance.Get("Stop-Round"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(845f, 30f, 80f, 80f), 1f);
			this.StopText = new ULabel("Stop", StyleController.Instance.GetStyle("ServiceFunction", "label"), new Rect(785f, 120f, 200f, 40f), 1f);
			this.UnfoldWindowRect = new Rect(60f, 80f, 970f, 180f);
			this.controlList.Add(this.Left);
			this.controlList.Add(this.BtnBG);
			this.controlList.Add(this.DumpLua);
			this.controlList.Add(this.DumpLuaText);
			this.controlList.Add(this.Dump2);
			this.controlList.Add(this.Dump2Text);
			this.controlList.Add(this.ServiceLineH);
			this.controlList.Add(this.Stop);
			this.controlList.Add(this.StopText);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000F4C4 File Offset: 0x0000D6C4
		private void UnfoldView()
		{
			this.BtnBG.View();
			this.Left.View();
			bool flag = this.CheckShowDump() && this.DumpLuaOn;
			if (flag)
			{
				bool enabled = GUI.enabled;
				GUI.enabled &= !this.DumpDisabled;
				this.DumpLua.View();
				this.DumpLuaText.View();
				GUI.enabled = enabled;
			}
			bool flag2 = this.CheckShowDump() && this.Dump2On;
			if (flag2)
			{
				bool enabled2 = GUI.enabled;
				GUI.enabled &= !this.SnapshotDisabled;
				this.Dump2.View();
				this.Dump2Text.View();
				GUI.enabled = enabled2;
			}
			bool flag3 = this.CheckShowDump() && (this.DumpLuaOn || this.Dump2On);
			if (flag3)
			{
				this.ServiceLineH.View();
			}
			this.Stop.View();
			this.StopText.View();
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000F5FC File Offset: 0x0000D7FC
		private bool CheckShowDump()
		{
			return SdkCtrlData.Instance.GotMode != eGotMode.Overview || (SdkCtrlData.Instance.GotMode == eGotMode.Overview && SdkStartConfig.Instance.overview.resources != 0);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000F654 File Offset: 0x0000D854
		private void Fold()
		{
			this.showBtns = false;
			SdkUIMgr.Get().IsFold = false;
			this.oWindowRect[WindowState.Horizontal] = new Rect(60f, 80f, 320f, 180f);
			this.oWindowRect[WindowState.Vertical] = new Rect(60f, 80f, 320f, 180f);
			Rect rect = base.ScaleWindowRect(this.oWindowRect[this._windowState], this._rate, this._align);
			this.WindowRect = new Rect(this.WindowRect.x, this.WindowRect.y, rect.width, rect.height);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000F718 File Offset: 0x0000D918
		public void SetOptions(bool dumpLua = true, bool dump2 = true)
		{
			this.DumpLuaOn = (dumpLua && SharedUtils.Dev);
			this.Dump2On = (dump2 && SharedUtils.Dev);
			bool flag = SharedUtils.Dev && this.CheckShowDump() && dumpLua && dump2;
			if (flag)
			{
				this.UnfoldWindowRect = new Rect(60f, 80f, 970f, 180f);
				this.BtnBG.SetPosition(new Rect(270f, 0f, 685f, 180f));
				this.DumpLua.SetPosition(new Rect(380f, 30f, 80f, 80f));
				this.DumpLuaText.SetPosition(new Rect(320f, 120f, 200f, 40f));
				this.Dump2.SetPosition(new Rect(605f, 30f, 80f, 80f));
				this.Dump2Text.SetPosition(new Rect(520f, 120f, 250f, 40f));
				this.ServiceLineH.SetPosition(new Rect(800f, 0f, 6f, 180f));
				this.Stop.SetPosition(new Rect(845f, 30f, 80f, 80f));
				this.StopText.SetPosition(new Rect(785f, 120f, 200f, 40f));
			}
			bool flag2 = !this.CheckShowDump() || (!dumpLua && !dump2);
			if (flag2)
			{
				this.UnfoldWindowRect = new Rect(60f, 80f, 530f, 180f);
				this.BtnBG.SetPosition(new Rect(270f, 0f, 245f, 180f));
				this.Stop.SetPosition(new Rect(380f, 30f, 80f, 80f));
				this.StopText.SetPosition(new Rect(320f, 120f, 200f, 40f));
			}
			bool flag3 = SharedUtils.Dev && this.CheckShowDump() && dumpLua && !dump2;
			if (flag3)
			{
				this.UnfoldWindowRect = new Rect(60f, 80f, 770f, 180f);
				this.BtnBG.SetPosition(new Rect(270f, 0f, 485f, 180f));
				this.DumpLua.SetPosition(new Rect(380f, 30f, 80f, 80f));
				this.DumpLuaText.SetPosition(new Rect(320f, 120f, 200f, 40f));
				this.ServiceLineH.SetPosition(new Rect(550f, 0f, 6f, 180f));
				this.Stop.SetPosition(new Rect(620f, 30f, 80f, 80f));
				this.StopText.SetPosition(new Rect(560f, 120f, 200f, 40f));
			}
			bool flag4 = SharedUtils.Dev && this.CheckShowDump() && !dumpLua && dump2;
			if (flag4)
			{
				this.UnfoldWindowRect = new Rect(60f, 80f, 780f, 180f);
				this.BtnBG.SetPosition(new Rect(270f, 0f, 485f, 180f));
				this.Dump2.SetPosition(new Rect(405f, 30f, 80f, 80f));
				this.Dump2Text.SetPosition(new Rect(320f, 120f, 250f, 40f));
				this.ServiceLineH.SetPosition(new Rect(600f, 0f, 6f, 180f));
				this.Stop.SetPosition(new Rect(645f, 30f, 80f, 80f));
				this.StopText.SetPosition(new Rect(585f, 120f, 200f, 40f));
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
		private void OnClickedDumpLua()
		{
			bool flag = SdkCtrlData.Instance.GotMode == eGotMode.Overview || SdkCtrlData.Instance.GotMode == eGotMode.Lua;
			if (flag)
			{
				SdkUIMgr.Get().DoDump(eDumpType.Lua);
			}
			SdkUIMgr.Get().StartCoroutine(this.DelayEnableDump());
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000FC1C File Offset: 0x0000DE1C
		private void OnClickedDump2()
		{
			bool flag = SdkCtrlData.Instance.GotMode == eGotMode.Overview;
			if (flag)
			{
				SdkUIMgr.Get().DoDump(eDumpType.Resources);
			}
			bool flag2 = SdkCtrlData.Instance.GotMode == eGotMode.Mono;
			if (flag2)
			{
				SdkUIMgr.Get().DoDump(eDumpType.ManagedHeap);
			}
			bool flag3 = SdkCtrlData.Instance.GotMode == eGotMode.Gpu;
			if (flag3)
			{
				SdkUIMgr.Get().DoDump(eDumpType.Overdraw);
			}
			SdkUIMgr.Get().StartCoroutine(this.DelayEnableDump2());
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000FCA0 File Offset: 0x0000DEA0
		private IEnumerator DelayEnableDump()
		{
			this.DumpDisabled = true;
			yield return new WaitForSecondsRealtime(1f);
			this.DumpDisabled = false;
			yield break;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000FCB0 File Offset: 0x0000DEB0
		private IEnumerator DelayEnableDump2()
		{
			this.SnapshotDisabled = true;
			yield return new WaitForSecondsRealtime(1f);
			this.SnapshotDisabled = false;
			yield break;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000FCC0 File Offset: 0x0000DEC0
		private void OnClickedStop()
		{
			this.showBtns = false;
			SharedUtils.durationStr = "0\"";
			this.DeltaTime.View(SharedUtils.durationStr, true);
			SdkUIMgr.Get().TryStop();
		}

		// Token: 0x04000155 RID: 341
		private ULabel TimeBG;

		// Token: 0x04000156 RID: 342
		private ULabel CurrentMode;

		// Token: 0x04000157 RID: 343
		private ULabel ServiceLine;

		// Token: 0x04000158 RID: 344
		private ULabel DeltaTime;

		// Token: 0x04000159 RID: 345
		private bool showBtns = false;

		// Token: 0x0400015A RID: 346
		private static string[] DisplayGotMode = new string[]
		{
			"Overview",
			"Mono",
			"Resources",
			"Lua",
			"GPU"
		};

		// Token: 0x0400015B RID: 347
		private UButton Right;

		// Token: 0x0400015C RID: 348
		private UButton Left;

		// Token: 0x0400015D RID: 349
		private ULabel BtnBG;

		// Token: 0x0400015E RID: 350
		private bool DumpLuaOn = true;

		// Token: 0x0400015F RID: 351
		private bool DumpDisabled = false;

		// Token: 0x04000160 RID: 352
		private UButton DumpLua;

		// Token: 0x04000161 RID: 353
		private ULabel DumpLuaText;

		// Token: 0x04000162 RID: 354
		private bool Dump2On = true;

		// Token: 0x04000163 RID: 355
		private bool SnapshotDisabled = false;

		// Token: 0x04000164 RID: 356
		private UButton Dump2;

		// Token: 0x04000165 RID: 357
		private ULabel Dump2Text;

		// Token: 0x04000166 RID: 358
		private ULabel ServiceLineH;

		// Token: 0x04000167 RID: 359
		private UButton Stop;

		// Token: 0x04000168 RID: 360
		private ULabel StopText;

		// Token: 0x04000169 RID: 361
		private Rect UnfoldWindowRect;
	}
}
