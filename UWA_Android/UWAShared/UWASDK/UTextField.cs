using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200001F RID: 31
	public class UTextField : UControl
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00007498 File Offset: 0x00005698
		// (set) Token: 0x0600010D RID: 269 RVA: 0x000074B8 File Offset: 0x000056B8
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				bool flag = value != this.text && this.OnChange != null;
				if (flag)
				{
					this.OnChange(value);
				}
				this.text = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00007508 File Offset: 0x00005708
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00007510 File Offset: 0x00005710
		public override float Rate
		{
			get
			{
				return base.Rate;
			}
			set
			{
				base.Rate = value;
				bool flag = this.Placeholder != null;
				if (flag)
				{
					this.Placeholder.Rate = value;
				}
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000754C File Offset: 0x0000574C
		public UTextField()
		{
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00007564 File Offset: 0x00005764
		public UTextField(Action<string> func, string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.OnChange = new UTextField.Trigger(func.Invoke);
			this.text = input;
			base.Init(style, position, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00007618 File Offset: 0x00005818
		public UTextField(Action<string> func, string input, string style, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.OnChange = new UTextField.Trigger(func.Invoke);
			this.text = input;
			base.Init(style, Rect.zero, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000076CC File Offset: 0x000058CC
		public UTextField(string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, position, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000776C File Offset: 0x0000596C
		public UTextField(string input, string style, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, Rect.zero, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000780C File Offset: 0x00005A0C
		public override void View()
		{
			bool flag = this.Placeholder != null && string.IsNullOrEmpty(this.text);
			if (flag)
			{
				this.Placeholder.View();
			}
			string text = GUI.TextField(this.c_Position, this.Text, this.Style);
			bool flag2 = text == this.Text;
			if (!flag2)
			{
				this.CheckText(text);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000788C File Offset: 0x00005A8C
		protected virtual void CheckText(string newT)
		{
			this.Text = newT;
		}

		// Token: 0x040000C6 RID: 198
		protected UTextField.Trigger OnChange = null;

		// Token: 0x040000C7 RID: 199
		protected string text;

		// Token: 0x040000C8 RID: 200
		protected ULabel Placeholder = null;

		// Token: 0x020000F0 RID: 240
		// (Invoke) Token: 0x060009A7 RID: 2471
		protected delegate void Trigger(string val);
	}
}
