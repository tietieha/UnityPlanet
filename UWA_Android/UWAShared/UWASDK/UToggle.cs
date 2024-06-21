using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000022 RID: 34
	public class UToggle : UControl
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00007EBC File Offset: 0x000060BC
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00007EC4 File Offset: 0x000060C4
		public string Text { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00007ED0 File Offset: 0x000060D0
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00007ED8 File Offset: 0x000060D8
		public Texture2D Icon { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00007EE4 File Offset: 0x000060E4
		// (set) Token: 0x06000128 RID: 296 RVA: 0x00007EEC File Offset: 0x000060EC
		public Texture2D IconOn { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00007EF8 File Offset: 0x000060F8
		// (set) Token: 0x0600012A RID: 298 RVA: 0x00007F18 File Offset: 0x00006118
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				bool flag = value != this.isSelected && this.OnChange != null;
				if (flag)
				{
					this.OnChange(value);
				}
				this.isSelected = value;
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007F64 File Offset: 0x00006164
		public UToggle(Action<bool> act, string text, string style, Rect position, float rate = 1f)
		{
			this.OnChange = new UToggle.Trigger(act.Invoke);
			this.Text = text;
			base.Init(style, position, rate);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00007FAC File Offset: 0x000061AC
		public UToggle(Action<bool> act, string text, string style, float rate = 1f)
		{
			this.OnChange = new UToggle.Trigger(act.Invoke);
			this.Text = text;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00007FF8 File Offset: 0x000061F8
		public UToggle(Texture2D icon, Texture2D iconOn, string text, string style, Rect position, float rate = 1f)
		{
			this.Text = text;
			this.Icon = icon;
			this.IconOn = iconOn;
			base.Init(style, position, rate);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008040 File Offset: 0x00006240
		public UToggle(string text, string style, float rate = 1f)
		{
			this.Text = text;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00008078 File Offset: 0x00006278
		public override void View()
		{
			bool flag = GUI.Button(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.height, this.c_Position.height), this.IsSelected ? this.IconOn : this.Icon, StyleController.Instance.GetStyle("Empty", "label")) || GUI.Button(new Rect(this.c_Position.x + this.c_Position.height + 20f, this.c_Position.y, this.c_Position.width - this.c_Position.height - 20f, this.c_Position.height), this.Text, this.Style);
			if (flag)
			{
				this.IsSelected = !this.IsSelected;
			}
		}

		// Token: 0x040000CD RID: 205
		private UToggle.Trigger OnChange = null;

		// Token: 0x040000CE RID: 206
		private bool isSelected;

		// Token: 0x020000F1 RID: 241
		// (Invoke) Token: 0x060009AB RID: 2475
		private delegate void Trigger(bool val);
	}
}
