using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000024 RID: 36
	public class UToolBar : UControl
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00008770 File Offset: 0x00006970
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00008778 File Offset: 0x00006978
		public override string Style
		{
			get
			{
				return base.Style;
			}
			protected set
			{
				base.Style = value;
				bool flag = this.tgs == null;
				if (!flag)
				{
					for (int i = 0; i < this.tgs.Length; i++)
					{
						this.tgs[i].SetStyle(value);
					}
				}
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000087D8 File Offset: 0x000069D8
		// (set) Token: 0x06000141 RID: 321 RVA: 0x000087E0 File Offset: 0x000069E0
		public string[] Options { get; protected set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000142 RID: 322 RVA: 0x000087EC File Offset: 0x000069EC
		// (set) Token: 0x06000143 RID: 323 RVA: 0x0000880C File Offset: 0x00006A0C
		public int Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				bool flag = value != this.selected && this.OnChange != null;
				if (flag)
				{
					this.OnChange(value);
				}
				this.selected = value;
				for (int i = 0; i < this.tgs.Length; i++)
				{
					this.tgs[i].IsSelected = (i == this.selected);
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00008890 File Offset: 0x00006A90
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00008898 File Offset: 0x00006A98
		public override float Rate
		{
			get
			{
				return base.Rate;
			}
			set
			{
				base.Rate = value;
				for (int i = 0; i < this.tgs.Length; i++)
				{
					this.tgs[i].Rate = value;
				}
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000088E4 File Offset: 0x00006AE4
		public UToolBar(Texture2D icon, Texture2D iconOn, string[] options, string style, Rect position, UToolBar.State s = UToolBar.State.Horizontal, string on = "ToggleBg", float rate = 1f, float interval = 5f)
		{
			this.Options = options;
			this.state = s;
			this.Interval = interval;
			this.Icon = icon;
			this.IconOn = iconOn;
			base.Init(style, position, rate);
			this.InitOption();
			bool flag = this.tgs != null;
			if (flag)
			{
				this.selected = 0;
				this.tgs[0].IsSelected = true;
			}
			this.onStyle = StyleController.Instance.GetStyle(on, "label");
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00008990 File Offset: 0x00006B90
		public UToolBar(Action<int> func, Texture2D icon, Texture2D iconOn, string[] options, string style, Rect position, UToolBar.State s = UToolBar.State.Horizontal, string on = "ToggleBg", float rate = 1f, float interval = 5f)
		{
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000089B0 File Offset: 0x00006BB0
		private void InitOption()
		{
			int num = this.Options.Length;
			this.tgs = new UToggle<int>[num];
			this.RefreshTGs();
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000089E0 File Offset: 0x00006BE0
		private void RefreshTGs()
		{
			int num = this.Options.Length;
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
						this.tgs[i] = new UToggle<int>(this.Icon, this.IconOn, new Action<bool, int>(this.ChangeSelected), i, this.Options[i], this.Style, position, "ToggleBg", 1f);
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
					this.tgs[j] = new UToggle<int>(this.Icon, this.IconOn, new Action<bool, int>(this.ChangeSelected), j, this.Options[j], this.Style, position2, "ToggleBg", 1f);
					this.tgs[j].Rate = this.Rate;
				}
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00008C08 File Offset: 0x00006E08
		private void ChangeSelected(bool value, int i)
		{
			bool flag = value & i != this.Selected;
			if (flag)
			{
				bool flag2 = this.Selected >= 0;
				if (flag2)
				{
					this.tgs[this.Selected].IsSelected = false;
				}
				this.Selected = i;
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00008C68 File Offset: 0x00006E68
		public void SetState(UToolBar.State s)
		{
			this.state = s;
			this.RefreshTGs();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00008C7C File Offset: 0x00006E7C
		public void SetInterval(float interval)
		{
			this.Interval = interval;
			this.RefreshTGs();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00008C90 File Offset: 0x00006E90
		public override void SetPosition(Rect rect)
		{
			base.SetPosition(rect);
			this.RefreshTGs();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00008CA4 File Offset: 0x00006EA4
		public void SetText(int i, string text)
		{
			bool flag = i < 0 || i >= this.Options.Length;
			if (!flag)
			{
				this.Options[i] = text;
				this.RefreshTGs();
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00008CF4 File Offset: 0x00006EF4
		public override void View()
		{
			bool flag = this.Selected >= 0 && this.Selected < this.tgs.Length;
			if (flag)
			{
				this.tgs[this.Selected].IsSelected = true;
			}
			for (int i = 0; i < this.tgs.Length; i++)
			{
				this.tgs[i].View();
			}
		}

		// Token: 0x040000D6 RID: 214
		protected string onStyle;

		// Token: 0x040000D8 RID: 216
		protected UToolBar.Trigger OnChange = null;

		// Token: 0x040000D9 RID: 217
		protected UToggle<int>[] tgs = null;

		// Token: 0x040000DA RID: 218
		protected float Interval;

		// Token: 0x040000DB RID: 219
		protected Texture2D Icon;

		// Token: 0x040000DC RID: 220
		protected Texture2D IconOn;

		// Token: 0x040000DD RID: 221
		protected int selected;

		// Token: 0x040000DE RID: 222
		protected UToolBar.State state = UToolBar.State.Horizontal;

		// Token: 0x020000F3 RID: 243
		// (Invoke) Token: 0x060009B3 RID: 2483
		protected delegate void Trigger(int val);

		// Token: 0x020000F4 RID: 244
		public enum State
		{
			// Token: 0x04000641 RID: 1601
			Horizontal,
			// Token: 0x04000642 RID: 1602
			Vertical
		}
	}
}
