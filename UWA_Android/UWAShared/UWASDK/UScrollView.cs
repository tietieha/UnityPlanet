using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200001E RID: 30
	public class UScrollView : UControl
	{
		// Token: 0x17000027 RID: 39
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00006F24 File Offset: 0x00005124
		public override float Rate
		{
			set
			{
				base.Rate = value;
				this.c_ScrollPosition = new Rect(this.o_ScrollPosition.x * value, this.o_ScrollPosition.y * value, this.o_ScrollPosition.width * value, this.o_ScrollPosition.height * value);
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006F80 File Offset: 0x00005180
		public UScrollView(Rect o, Rect s, ScreenTouch st = null, float interval = 0f, float rate = 1f, string style = "")
		{
			base.Init(style, o, rate);
			this.o_ScrollPosition = s;
			this.c_ScrollPosition = new Rect(this.o_ScrollPosition.x * this.Rate, this.o_ScrollPosition.y * this.Rate, this.o_ScrollPosition.width * this.Rate, this.o_ScrollPosition.height * this.Rate);
			this._st = st;
			this._interval = interval;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007040 File Offset: 0x00005240
		public UScrollView(UWindow parent, Rect o, Rect s, ScreenTouch st = null, float interval = 0f, float rate = 1f, string style = "")
		{
			base.Init(style, o, rate);
			this.o_ScrollPosition = s;
			this.c_ScrollPosition = new Rect(this.o_ScrollPosition.x * this.Rate, this.o_ScrollPosition.y * this.Rate, this.o_ScrollPosition.width * this.Rate, this.o_ScrollPosition.height * this.Rate);
			this._st = st;
			this._interval = interval;
			this.Parent = parent;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007108 File Offset: 0x00005308
		public void SetPosition(Rect o, Rect s)
		{
			base.o_Position = o;
			this.o_ScrollPosition = s;
			this.c_Position = new Rect(base.o_Position.x * this.Rate, base.o_Position.y * this.Rate, base.o_Position.width * this.Rate, base.o_Position.height * this.Rate);
			this.c_ScrollPosition = new Rect(this.o_ScrollPosition.x * this.Rate, this.o_ScrollPosition.y * this.Rate, this.o_ScrollPosition.width * this.Rate, this.o_ScrollPosition.height * this.Rate);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x000071DC File Offset: 0x000053DC
		public override void View()
		{
			this.ScrollVector = GUI.BeginScrollView(this.c_Position, this.ScrollVector, this.c_ScrollPosition, false, false);
			bool flag = this._st != null;
			if (flag)
			{
				bool flag2 = Input.GetMouseButtonDown(0) || SdkUIMgr.Get().isAnim;
				if (flag2)
				{
					this.LastScrollVector.x = this.ScrollVector.x;
					this.LastScrollVector.y = this.ScrollVector.y;
				}
				float num = (this.Parent != null) ? this.Parent.GetWindowPos().y : 0f;
				bool flag3 = this._st.IsMoving && this._st.StartFingerPos.x > this.WindowPos.x + this.c_Position.x && this._st.StartFingerPos.x < this.WindowPos.x + this.c_Position.x + this.c_Position.width - this._interval / this.Rate && this._st.StartFingerPos.y - num > this.WindowPos.y + this.c_Position.y && this._st.StartFingerPos.y - num < this.WindowPos.y + this.c_Position.y + this.c_Position.height;
				if (flag3)
				{
					this.ScrollVectorMove(this._st.XMoveDistance, this._st.YMoveDistance);
				}
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x000073AC File Offset: 0x000055AC
		public Vector2 GetScrollVector()
		{
			return this.ScrollVector;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000073CC File Offset: 0x000055CC
		public void SetScrollVector(Vector2 v)
		{
			this.ScrollVector = v;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000073D8 File Offset: 0x000055D8
		public void ScrollVectorMove(float x, float y)
		{
			bool flag = this.LastScrollVector.x - x < 0f;
			if (flag)
			{
				this.ScrollVector.x = 0f;
			}
			else
			{
				this.ScrollVector.x = this.LastScrollVector.x - x;
			}
			bool flag2 = this.LastScrollVector.y - y < 0f;
			if (flag2)
			{
				this.ScrollVector.y = 0f;
			}
			else
			{
				this.ScrollVector.y = this.LastScrollVector.y - y;
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000747C File Offset: 0x0000567C
		public void SetWindowPos(float x, float y)
		{
			this.WindowPos.x = x;
			this.WindowPos.y = y;
		}

		// Token: 0x040000BE RID: 190
		private Vector2 ScrollVector = Vector2.zero;

		// Token: 0x040000BF RID: 191
		private Vector2 LastScrollVector = Vector2.zero;

		// Token: 0x040000C0 RID: 192
		public Rect o_ScrollPosition;

		// Token: 0x040000C1 RID: 193
		private Rect c_ScrollPosition;

		// Token: 0x040000C2 RID: 194
		private ScreenTouch _st = null;

		// Token: 0x040000C3 RID: 195
		private float _interval;

		// Token: 0x040000C4 RID: 196
		private Vector2 WindowPos = default(Vector2);

		// Token: 0x040000C5 RID: 197
		public UWindow Parent = null;
	}
}
