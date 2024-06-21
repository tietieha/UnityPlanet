using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000029 RID: 41
	public class UControl
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00009A2C File Offset: 0x00007C2C
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00009A34 File Offset: 0x00007C34
		public virtual string Style { get; protected set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00009A40 File Offset: 0x00007C40
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00009A48 File Offset: 0x00007C48
		public Rect o_Position { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00009A54 File Offset: 0x00007C54
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00009A74 File Offset: 0x00007C74
		public virtual float Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				this._rate = value;
				this.c_Position = new Rect(this.o_Position.x * value, this.o_Position.y * value, this.o_Position.width * value, this.o_Position.height * value);
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009AD8 File Offset: 0x00007CD8
		protected void Init(string style, Rect position, float rate = 1f)
		{
			this.Style = style;
			this.o_Position = new Rect(position);
			this._rate = rate;
			this.c_Position = new Rect(this.o_Position.x * rate, this.o_Position.y * rate, this.o_Position.width * rate, this.o_Position.height * rate);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009B54 File Offset: 0x00007D54
		public void SetStyle(string style)
		{
			this.Style = style;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00009B60 File Offset: 0x00007D60
		public virtual void SetPosition(Rect rect)
		{
			this.o_Position = rect;
			this.c_Position = new Rect(this.o_Position.x * this.Rate, this.o_Position.y * this.Rate, this.o_Position.width * this.Rate, this.o_Position.height * this.Rate);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00009BDC File Offset: 0x00007DDC
		public virtual void View()
		{
		}

		// Token: 0x040000ED RID: 237
		protected Rect c_Position;

		// Token: 0x040000EE RID: 238
		private float _rate;
	}
}
