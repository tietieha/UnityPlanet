using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000023 RID: 35
	public class UToggle<T> : UControl
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00008184 File Offset: 0x00006384
		// (set) Token: 0x06000131 RID: 305 RVA: 0x0000818C File Offset: 0x0000638C
		public Texture2D Icon { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00008198 File Offset: 0x00006398
		// (set) Token: 0x06000133 RID: 307 RVA: 0x000081A0 File Offset: 0x000063A0
		public Texture2D IconOn { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000134 RID: 308 RVA: 0x000081AC File Offset: 0x000063AC
		// (set) Token: 0x06000135 RID: 309 RVA: 0x000081B4 File Offset: 0x000063B4
		public string Text { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000136 RID: 310 RVA: 0x000081C0 File Offset: 0x000063C0
		// (set) Token: 0x06000137 RID: 311 RVA: 0x000081E0 File Offset: 0x000063E0
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
					this.OnChange(value, this.tv);
				}
				this.isSelected = value;
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00008230 File Offset: 0x00006430
		public UToggle(Texture2D icon, Texture2D iconOn, Action<bool, T> act, T t, string text, string style, Rect position, string on = "ToggleBg", float rate = 1f)
		{
			this.OnChange = new UToggle<T>.Trigger(act.Invoke);
			this.Text = text;
			this.tv = t;
			this.Icon = icon;
			this.IconOn = iconOn;
			base.Init(style, position, rate);
			this.OnStyle = StyleController.Instance.GetStyle(on, "label");
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000082A8 File Offset: 0x000064A8
		public UToggle(Action<bool, T> act, T t, string text, string style, string on = "ToggleBg", float rate = 1f)
		{
			this.OnChange = new UToggle<T>.Trigger(act.Invoke);
			this.Text = text;
			this.tv = t;
			base.Init(style, Rect.zero, rate);
			this.OnStyle = StyleController.Instance.GetStyle(on, "label");
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008310 File Offset: 0x00006510
		public UToggle(string text, T t, string style, Rect position, string on = "ToggleBg", float rate = 1f)
		{
			this.Text = text;
			this.tv = t;
			base.Init(style, position, rate);
			this.OnStyle = StyleController.Instance.GetStyle(on, "label");
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008364 File Offset: 0x00006564
		public UToggle(string text, T t, string style, string on = "ToggleBg", float rate = 1f)
		{
			this.Text = text;
			this.tv = t;
			base.Init(style, Rect.zero, rate);
			this.OnStyle = StyleController.Instance.GetStyle(on, "label");
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000083BC File Offset: 0x000065BC
		public override void View()
		{
			bool flag = this.Icon == null;
			if (flag)
			{
				bool flag2 = this.isSelected;
				if (flag2)
				{
					GUI.Label(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.width, this.c_Position.height), this.IconOn, this.OnStyle);
				}
				bool flag3 = GUI.Button(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.width, this.c_Position.height), this.Text, this.Style);
				if (flag3)
				{
					this.IsSelected = !this.IsSelected;
				}
			}
			else
			{
				bool flag4 = GUI.Button(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.height, this.c_Position.height), this.IsSelected ? this.IconOn : this.Icon, StyleController.Instance.GetStyle("Empty", "label")) || GUI.Button(new Rect(this.c_Position.x + this.c_Position.height + 20f, this.c_Position.y, this.c_Position.width - this.c_Position.height - 20f, this.c_Position.height), this.Text, this.Style);
				if (flag4)
				{
					this.IsSelected = !this.IsSelected;
				}
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000859C File Offset: 0x0000679C
		public void View(bool enable_click)
		{
			bool flag = this.Icon == null;
			if (flag)
			{
				bool flag2 = this.isSelected;
				if (flag2)
				{
					GUI.Label(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.width, this.c_Position.height), this.IconOn, this.OnStyle);
				}
				bool flag3 = GUI.Button(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.width, this.c_Position.height), this.Text, this.Style) && enable_click;
				if (flag3)
				{
					this.IsSelected = !this.IsSelected;
				}
			}
			else
			{
				bool flag4 = (GUI.Button(new Rect(this.c_Position.x, this.c_Position.y, this.c_Position.height, this.c_Position.height), this.IsSelected ? this.IconOn : this.Icon, "Empty") || GUI.Button(new Rect(this.c_Position.x + this.c_Position.height + 20f, this.c_Position.y, this.c_Position.width - this.c_Position.height - 20f, this.c_Position.height), this.Text, this.Style)) && enable_click;
				if (flag4)
				{
					this.IsSelected = !this.IsSelected;
				}
			}
		}

		// Token: 0x040000D2 RID: 210
		protected string OnStyle;

		// Token: 0x040000D3 RID: 211
		private T tv;

		// Token: 0x040000D4 RID: 212
		private UToggle<T>.Trigger OnChange = null;

		// Token: 0x040000D5 RID: 213
		private bool isSelected;

		// Token: 0x020000F2 RID: 242
		// (Invoke) Token: 0x060009AF RID: 2479
		private delegate void Trigger(bool val, T t);
	}
}
