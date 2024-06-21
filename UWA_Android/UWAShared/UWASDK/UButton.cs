using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200001B RID: 27
	public class UButton : UControl
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00006240 File Offset: 0x00004440
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00006248 File Offset: 0x00004448
		public string Text { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00006254 File Offset: 0x00004454
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x0000625C File Offset: 0x0000445C
		public Texture2D Texture { get; private set; }

		// Token: 0x060000E3 RID: 227 RVA: 0x00006268 File Offset: 0x00004468
		public UButton(Action act, string text, string style, Rect position, float rate = 1f)
		{
			this.OnClick = new UButton.Trigger(act.Invoke);
			this.Text = text;
			this.isText = true;
			base.Init(style, position, rate);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000062B8 File Offset: 0x000044B8
		public UButton(Action act, string text, string style, float rate = 1f)
		{
			this.OnClick = new UButton.Trigger(act.Invoke);
			this.Text = text;
			this.isText = true;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006308 File Offset: 0x00004508
		public UButton(Action act, Texture2D texture, string style, Rect position, float rate = 1f)
		{
			this.OnClick = new UButton.Trigger(act.Invoke);
			this.Texture = texture;
			this.isText = false;
			base.Init(style, position, rate);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006358 File Offset: 0x00004558
		public UButton(Action act, Texture2D texture, string style, float rate = 1f)
		{
			this.OnClick = new UButton.Trigger(act.Invoke);
			this.Texture = texture;
			this.isText = false;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000063A8 File Offset: 0x000045A8
		private new void SetStyle(string style)
		{
			this.Style = style;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000063B4 File Offset: 0x000045B4
		public override void View()
		{
			bool flag = this.isText;
			if (flag)
			{
				bool flag2 = GUI.Button(this.c_Position, this.Text, this.Style);
				if (flag2)
				{
					this.OnClick();
				}
			}
			else
			{
				bool flag3 = GUI.Button(this.c_Position, this.Texture, this.Style);
				if (flag3)
				{
					this.OnClick();
				}
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006440 File Offset: 0x00004640
		public void View(Vector2 _pos)
		{
			this.c_Position = new Rect(_pos.x, _pos.y, this.c_Position.width, this.c_Position.height);
			bool flag = this.isText;
			if (flag)
			{
				bool flag2 = GUI.Button(this.c_Position, this.Text, this.Style);
				if (flag2)
				{
					this.OnClick();
				}
			}
			else
			{
				bool flag3 = GUI.Button(this.c_Position, this.Texture, this.Style);
				if (flag3)
				{
					this.OnClick();
				}
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000064F8 File Offset: 0x000046F8
		public void View(string text, bool update = true)
		{
			bool flag = this.isText;
			if (flag)
			{
				if (update)
				{
					this.Text = text;
				}
				bool flag2 = GUI.Button(this.c_Position, text, this.Style);
				if (flag2)
				{
					this.OnClick();
				}
			}
			else
			{
				bool flag3 = GUI.Button(this.c_Position, this.Texture, this.Style);
				if (flag3)
				{
					this.OnClick();
				}
			}
		}

		// Token: 0x040000B0 RID: 176
		private UButton.Trigger OnClick = null;

		// Token: 0x040000B1 RID: 177
		private bool isText;

		// Token: 0x020000EE RID: 238
		// (Invoke) Token: 0x0600099F RID: 2463
		private delegate void Trigger();
	}
}
