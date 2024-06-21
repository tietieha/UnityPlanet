using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200001D RID: 29
	public class ULabel : UControl
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00006D18 File Offset: 0x00004F18
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00006D20 File Offset: 0x00004F20
		public string Text { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00006D2C File Offset: 0x00004F2C
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00006D34 File Offset: 0x00004F34
		public Texture2D Texture { get; private set; }

		// Token: 0x060000FD RID: 253 RVA: 0x00006D40 File Offset: 0x00004F40
		public ULabel(string text, string style, Rect position, float rate = 1f)
		{
			this.Text = text;
			this.isText = true;
			base.Init(style, position, rate);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006D74 File Offset: 0x00004F74
		public ULabel(string text, string style, float rate = 1f)
		{
			this.Text = text;
			this.isText = true;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006DAC File Offset: 0x00004FAC
		public ULabel(Texture2D texture, string style, Rect position, float rate = 1f)
		{
			this.Texture = texture;
			this.isText = false;
			base.Init(style, position, rate);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006DE0 File Offset: 0x00004FE0
		public ULabel(Texture2D texture, string style, float rate = 1f)
		{
			this.Texture = texture;
			this.isText = false;
			base.Init(style, Rect.zero, rate);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006E18 File Offset: 0x00005018
		public override void View()
		{
			bool flag = this.isText;
			if (flag)
			{
				GUI.Label(this.c_Position, this.Text, this.Style);
			}
			else
			{
				GUI.Label(this.c_Position, this.Texture, this.Style);
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006E7C File Offset: 0x0000507C
		public void View(string text, bool update = true)
		{
			bool flag = this.isText && update;
			if (flag)
			{
				this.Text = text;
				GUI.Label(this.c_Position, this.Text, this.Style);
			}
			else
			{
				bool flag2 = this.isText && !update;
				if (flag2)
				{
					GUI.Label(this.c_Position, text, this.Style);
				}
				else
				{
					GUI.Label(this.c_Position, "TEXTURE LABEL", this.Style);
				}
			}
		}

		// Token: 0x040000BD RID: 189
		private bool isText;
	}
}
