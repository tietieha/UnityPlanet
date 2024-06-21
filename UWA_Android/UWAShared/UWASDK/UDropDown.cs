using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200001C RID: 28
	public class UDropDown : UControl
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00006590 File Offset: 0x00004790
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00006598 File Offset: 0x00004798
		public bool ShowOption { get; protected set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000ED RID: 237 RVA: 0x000065A4 File Offset: 0x000047A4
		public int Selected
		{
			get
			{
				return this.ToolBar.Selected;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000EE RID: 238 RVA: 0x000065C8 File Offset: 0x000047C8
		// (set) Token: 0x060000EF RID: 239 RVA: 0x000065D0 File Offset: 0x000047D0
		public override float Rate
		{
			get
			{
				return base.Rate;
			}
			set
			{
				base.Rate = value;
				this.SelectBar.Rate = value;
				this.Placeholder.Rate = value;
				this.OpenCloseBtn.Rate = value;
				this.ContentBG.Rate = value;
				this.ToolBar.Rate = value;
				this.ScrollView.Rate = value;
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006638 File Offset: 0x00004838
		public UDropDown(string s_style, Rect s_position, string bg_style, Rect bg_position, string op_style, Texture2D sa_tex = null, Texture2D open_tex = null, string p_style = "", string placeholder = "", int interval = 0, ScreenTouch st = null, float rate = 1f)
		{
			base.Init(s_style, s_position, rate);
			this.SelectBar = new UButton(new Action(this.OpenCloseOption), "", s_style, s_position, 1f);
			this.Placeholder = new UButton(new Action(this.OpenCloseOption), placeholder, p_style, new Rect(s_position.x + 30f, s_position.y, s_position.width, s_position.height), 1f);
			this.OpenCloseBtn = new UButton(new Action(this.OpenCloseOption), open_tex, StyleController.Instance.GetStyle("Empty", "label"), new Rect(s_position.x + s_position.width - s_position.height - 20f, s_position.y, s_position.height, s_position.height), 1f);
			this.ContentBG = new ULabel("", bg_style, bg_position, 1f);
			this.ToolBar = new UDropDownToolBar(new Action<int>(this.ChangeSelected), null, sa_tex, new string[0], op_style, new Rect(bg_position.x, bg_position.y, bg_position.width - (float)interval / this.Rate, 0f), UToolBar.State.Vertical, "ProjNameActive", st, 1f, 5f);
			this.ScrollView = new UScrollView(bg_position, new Rect(bg_position.x, bg_position.y, bg_position.width - (float)interval / this.Rate, (float)(this.ToolBar.Options.Length * 90)), st, (float)interval, 1f, "");
			this.ShowOption = false;
			this.Interval = interval;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006810 File Offset: 0x00004A10
		public UDropDown(Action<int> action, string s_style, Rect s_position, string bg_style, Rect bg_position, string op_style, Texture2D sa_tex = null, Texture2D open_tex = null, string p_style = "", string placeholder = "", int interval = 0, ScreenTouch st = null, float rate = 1f)
		{
			base.Init(s_style, s_position, rate);
			this.SelectChange = new UDropDown.Trigger(action.Invoke);
			this.SelectBar = new UButton(new Action(this.OpenCloseOption), "", s_style, s_position, 1f);
			this.Placeholder = new UButton(new Action(this.OpenCloseOption), placeholder, p_style, new Rect(s_position.x + 30f, s_position.y, s_position.width, s_position.height), 1f);
			this.OpenCloseBtn = new UButton(new Action(this.OpenCloseOption), open_tex, StyleController.Instance.GetStyle("Empty", "label"), new Rect(s_position.x + s_position.width - s_position.height - 20f, s_position.y, s_position.height, s_position.height), 1f);
			this.ContentBG = new ULabel("", bg_style, bg_position, 1f);
			this.ToolBar = new UDropDownToolBar(new Action<int>(this.ChangeSelected), null, sa_tex, new string[0], op_style, new Rect(bg_position.x, bg_position.y, bg_position.width - (float)interval / this.Rate, 0f), UToolBar.State.Vertical, "ProjNameActive", st, 1f, 5f);
			this.ScrollView = new UScrollView(bg_position, new Rect(bg_position.x, bg_position.y, bg_position.width - (float)interval / this.Rate, (float)(this.ToolBar.Options.Length * 90)), st, (float)interval, 1f, "");
			this.ShowOption = false;
			this.Interval = interval;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000069FC File Offset: 0x00004BFC
		private void OpenCloseOption()
		{
			this.ShowOption = !this.ShowOption;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006A10 File Offset: 0x00004C10
		private void ChangeSelected(int i)
		{
			this.ShowOption = !this.ShowOption;
			UDropDown.Trigger selectChange = this.SelectChange;
			if (selectChange != null)
			{
				selectChange(i);
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006A3C File Offset: 0x00004C3C
		public override void View()
		{
			bool flag = this.ToolBar.Selected == -1;
			if (flag)
			{
				this.SelectBar.View();
				this.Placeholder.View();
			}
			else
			{
				this.SelectBar.View(this.ToolBar.Options[this.Selected], false);
			}
			this.OpenCloseBtn.View();
			bool showOption = this.ShowOption;
			if (showOption)
			{
				this.ContentBG.View();
				this.ScrollView.View();
				this.ToolBar.View();
				GUI.EndScrollView();
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00006AEC File Offset: 0x00004CEC
		public void SetPosition(Rect s_position, Rect bg_position)
		{
			this.SelectBar.SetPosition(s_position);
			this.Placeholder.SetPosition(new Rect(s_position.x + 30f, s_position.y, s_position.width, s_position.height));
			this.OpenCloseBtn.SetPosition(new Rect(s_position.x + s_position.width - s_position.height - 20f, s_position.y, s_position.height, s_position.height));
			this.ContentBG.SetPosition(bg_position);
			this.ToolBar.SetPosition(new Rect(bg_position.x, bg_position.y, bg_position.width - (float)this.Interval / this.Rate, (float)(this.ToolBar.Options.Length * 90)));
			this.ScrollView.SetPosition(bg_position, new Rect(bg_position.x, bg_position.y, bg_position.width - (float)this.Interval / this.Rate, (float)(this.ToolBar.Options.Length * 90)));
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006C1C File Offset: 0x00004E1C
		public void SetOptions(string[] ss)
		{
			this.ToolBar.SetPosition(new Rect(this.ToolBar.o_Position.x, this.ToolBar.o_Position.y, this.ToolBar.o_Position.width, (float)(90 * ss.Length)));
			this.ToolBar.SetOptions(ss);
			this.ScrollView.SetPosition(this.ScrollView.o_Position, new Rect(this.ScrollView.o_ScrollPosition.x, this.ScrollView.o_ScrollPosition.y, this.ScrollView.o_ScrollPosition.width, (float)(this.ToolBar.Options.Length * 90)));
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006CEC File Offset: 0x00004EEC
		public void ResetSelection()
		{
			this.ToolBar.SetSelection(-1);
			this.ShowOption = false;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00006D04 File Offset: 0x00004F04
		public void SetWindowPos(float x, float y)
		{
			this.ScrollView.SetWindowPos(x, y);
		}

		// Token: 0x040000B2 RID: 178
		private UButton SelectBar;

		// Token: 0x040000B3 RID: 179
		private UButton Placeholder;

		// Token: 0x040000B4 RID: 180
		private UButton OpenCloseBtn;

		// Token: 0x040000B5 RID: 181
		private ULabel ContentBG;

		// Token: 0x040000B6 RID: 182
		private UDropDownToolBar ToolBar;

		// Token: 0x040000B7 RID: 183
		private UScrollView ScrollView;

		// Token: 0x040000B8 RID: 184
		private int Interval;

		// Token: 0x040000B9 RID: 185
		protected UDropDown.Trigger SelectChange = null;

		// Token: 0x020000EF RID: 239
		// (Invoke) Token: 0x060009A3 RID: 2467
		protected delegate void Trigger(int val);
	}
}
