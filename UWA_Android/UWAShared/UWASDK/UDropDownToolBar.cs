using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000025 RID: 37
	public class UDropDownToolBar : UToolBar
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00008D78 File Offset: 0x00006F78
		public UDropDownToolBar(Texture2D icon, Texture2D iconOn, string[] options, string style, Rect position, UToolBar.State s = UToolBar.State.Horizontal, string on = "ToggleBg", float rate = 1f, float interval = 5f) : base(icon, iconOn, options, style, position, s, on, rate, interval)
		{
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00008DA8 File Offset: 0x00006FA8
		public UDropDownToolBar(Action<int> func, Texture2D icon, Texture2D iconOn, string[] options, string style, Rect position, UToolBar.State s = UToolBar.State.Horizontal, string on = "ToggleBg", ScreenTouch st = null, float rate = 1f, float interval = 5f) : base(func, icon, iconOn, options, style, position, s, on, rate, interval)
		{
			this.OnChange = new UToolBar.Trigger(func.Invoke);
			base.Options = options;
			this.state = s;
			this.Interval = interval;
			this.Icon = icon;
			this.IconOn = iconOn;
			base.Init(style, position, rate);
			this.InitOption();
			this.selected = -1;
			this.onStyle = StyleController.Instance.GetStyle(on, "label");
			this._st = st;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00008E4C File Offset: 0x0000704C
		private void InitOption()
		{
			int num = base.Options.Length;
			this.tgs = new UToggle<int>[num];
			this.RefreshTGs();
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00008E7C File Offset: 0x0000707C
		private void RefreshTGs()
		{
			int num = base.Options.Length;
			this.tgs = new UToggle<int>[num];
			UToolBar.State state = this.state;
			UToolBar.State state2 = state;
			if (state2 != UToolBar.State.Horizontal)
			{
				if (state2 == UToolBar.State.Vertical)
				{
					int num2 = (int)base.o_Position.width;
					int num3 = (int)(base.o_Position.height / (float)num) - (int)this.Interval;
					for (int i = 0; i < this.tgs.Length; i++)
					{
						Rect position;
						position..ctor(base.o_Position.x, base.o_Position.y + (float)i * ((float)num3 + this.Interval), (float)num2, (float)num3);
						this.tgs[i] = new UToggle<int>(this.Icon, this.IconOn, new Action<bool, int>(this.ChangeSelected), i, base.Options[i], this.Style, position, this.onStyle, 1f);
						this.tgs[i].Rate = this.Rate;
					}
				}
			}
			else
			{
				int num2 = (int)(base.o_Position.width / (float)num) - (int)this.Interval;
				int num3 = (int)base.o_Position.height;
				for (int j = 0; j < this.tgs.Length; j++)
				{
					Rect position2;
					position2..ctor(base.o_Position.x + (float)j * ((float)num2 + this.Interval), base.o_Position.y, (float)num2, (float)num3);
					this.tgs[j] = new UToggle<int>(this.Icon, this.IconOn, new Action<bool, int>(this.ChangeSelected), j, base.Options[j], this.Style, position2, this.onStyle, 1f);
					this.tgs[j].Rate = this.Rate;
				}
			}
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000090B0 File Offset: 0x000072B0
		private void ChangeSelected(bool value, int i)
		{
			bool flag = value & i != base.Selected;
			if (flag)
			{
				bool flag2 = base.Selected >= 0;
				if (flag2)
				{
					this.tgs[base.Selected].IsSelected = false;
				}
				base.Selected = i;
			}
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00009110 File Offset: 0x00007310
		public void SetSelection(int i)
		{
			base.Selected = i;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000911C File Offset: 0x0000731C
		public override void SetPosition(Rect rect)
		{
			base.o_Position = rect;
			this.c_Position = new Rect(base.o_Position.x * this.Rate, base.o_Position.y * this.Rate, base.o_Position.width * this.Rate, base.o_Position.height * this.Rate);
			this.RefreshTGs();
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000919C File Offset: 0x0000739C
		public void SetOptions(string[] ss)
		{
			base.Options = ss;
			this.RefreshTGs();
		}

		// Token: 0x06000158 RID: 344 RVA: 0x000091B0 File Offset: 0x000073B0
		public override void View()
		{
			bool flag = base.Selected >= 0;
			if (flag)
			{
				this.tgs[base.Selected].IsSelected = true;
			}
			bool flag2 = this._st == null;
			if (flag2)
			{
				for (int i = 0; i < this.tgs.Length; i++)
				{
					this.tgs[i].View();
				}
			}
			else
			{
				for (int j = 0; j < this.tgs.Length; j++)
				{
					this.tgs[j].View(this._st.YMoveDistance == 0f);
				}
			}
		}

		// Token: 0x040000DF RID: 223
		private ScreenTouch _st = null;
	}
}
