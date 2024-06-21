using System;
using System.Collections.Generic;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x0200002C RID: 44
	public class UWindow
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000BA28 File Offset: 0x00009C28
		// (set) Token: 0x0600018E RID: 398 RVA: 0x0000BA30 File Offset: 0x00009C30
		public string Name { get; protected set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000BA3C File Offset: 0x00009C3C
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000BA5C File Offset: 0x00009C5C
		public virtual UWindow.YAlign Align
		{
			get
			{
				return this._align;
			}
			set
			{
				bool flag = value == this._align;
				if (!flag)
				{
					this._align = value;
					this.WindowRect = this.ScaleWindowRect(this.oWindowRect[this._windowState], this.Rate, value);
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000BAD0 File Offset: 0x00009CD0
		public virtual WindowState State
		{
			get
			{
				return this._windowState;
			}
			set
			{
				this._windowState = value;
				if (value != WindowState.Horizontal)
				{
					if (value == WindowState.Vertical)
					{
						this.WindowVertical();
					}
				}
				else
				{
					this.WindowHorizontal();
				}
				this.WindowRect = this.ScaleWindowRect(this.oWindowRect[value], this.Rate, this.Align);
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000BB40 File Offset: 0x00009D40
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000BB60 File Offset: 0x00009D60
		public virtual float Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				bool flag = value < 0f;
				if (!flag)
				{
					this._rate = value;
					this.WindowRect = this.ScaleWindowRect(this.oWindowRect[this.State], value, this.Align);
					for (int i = 0; i < this.controlList.Count; i++)
					{
						this.controlList[i].Rate = value;
					}
				}
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000BBE4 File Offset: 0x00009DE4
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000BC04 File Offset: 0x00009E04
		public virtual string Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000BC10 File Offset: 0x00009E10
		public UWindow(Rect windowRect_h, Rect windowRect_v, string style = "window", string name = "UWindow", UWindow.YAlign align = UWindow.YAlign.Top, WindowState windowState = WindowState.Horizontal, float rate = 1f)
		{
			this.oWindowRect[WindowState.Horizontal] = windowRect_h;
			this.oWindowRect[WindowState.Vertical] = windowRect_v;
			this._style = style;
			this.Name = name;
			this._align = align;
			this._windowState = windowState;
			this._rate = rate;
			this.WindowRect = this.ScaleWindowRect(this.oWindowRect[this._windowState], this._rate, this._align);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000BCE4 File Offset: 0x00009EE4
		public void View(int windowID)
		{
			bool flag = this._winFunc == null;
			if (flag)
			{
				this._winFunc = new GUI.WindowFunction(this.WindowViewBase);
			}
			this.WindowRect = GUI.Window(windowID, this.WindowRect, this._winFunc, "", this._style);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000BD40 File Offset: 0x00009F40
		public Vector2 GetWindowPos()
		{
			return new Vector2(this.WindowRect.x, this.WindowRect.y);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000BD74 File Offset: 0x00009F74
		public Rect GetWindowRect()
		{
			Rect rect = this.oWindowRect[this._windowState];
			Rect result;
			switch (this._align)
			{
			case UWindow.YAlign.Top:
				result = new Rect(rect.x * this._rate, rect.y * this._rate, rect.width * this._rate, rect.height * this._rate);
				break;
			case UWindow.YAlign.Bottom:
				result = new Rect(rect.x * this._rate, (float)Screen.height - (1080f - rect.yMax) * this._rate, rect.width * this._rate, rect.height * this._rate);
				break;
			case UWindow.YAlign.Center:
				result = new Rect(((float)Screen.width - rect.width * this._rate) / 2f, ((float)Screen.height - rect.height * this._rate) / 2f, rect.width * this._rate, rect.height * this._rate);
				break;
			default:
				result = new Rect(rect.x * this._rate, rect.y * this._rate, rect.width * this._rate, rect.height * this._rate);
				break;
			}
			return result;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		public void SetWindowPos(Vector2 Pos)
		{
			this.WindowRect = new Rect(Pos.x, Pos.y, this.WindowRect.width, this.WindowRect.height);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000BF2C File Offset: 0x0000A12C
		public virtual void WindowInitialize()
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000BF30 File Offset: 0x0000A130
		public void WindowViewBase(int windowID)
		{
			GUI.enabled = UWAPanel.Inst.Interactable;
			this.WindowView(windowID);
			GUI.enabled = true;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000BF54 File Offset: 0x0000A154
		public virtual void WindowView(int windowID)
		{
			for (int i = 0; i < this.controlList.Count; i++)
			{
				this.controlList[i].View();
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000BF98 File Offset: 0x0000A198
		public virtual void WindowHorizontal()
		{
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000BF9C File Offset: 0x0000A19C
		public virtual void WindowVertical()
		{
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000BFA0 File Offset: 0x0000A1A0
		public virtual void WindowNormal()
		{
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		public virtual void WindowSmall()
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000BFA8 File Offset: 0x0000A1A8
		protected Rect ScaleWindowRect(Rect rect, float rate, UWindow.YAlign align = UWindow.YAlign.Top)
		{
			Rect result;
			switch (align)
			{
			case UWindow.YAlign.Top:
				result = new Rect(rect.x * rate, rect.y * rate, rect.width * rate, rect.height * rate);
				break;
			case UWindow.YAlign.Bottom:
				result = new Rect(rect.x * rate, (float)Screen.height - (1080f - rect.yMax) * rate, rect.width * rate, rect.height * rate);
				break;
			case UWindow.YAlign.Center:
				result = new Rect(((float)Screen.width - rect.width * rate) / 2f, ((float)Screen.height - rect.height * rate) / 2f, rect.width * rate, rect.height * rate);
				break;
			default:
				result = new Rect(rect.x * rate, rect.y * rate, rect.width * rate, rect.height * rate);
				break;
			}
			return result;
		}

		// Token: 0x0400011C RID: 284
		protected Dictionary<WindowState, Rect> oWindowRect = new Dictionary<WindowState, Rect>
		{
			{
				WindowState.Horizontal,
				default(Rect)
			},
			{
				WindowState.Vertical,
				default(Rect)
			}
		};

		// Token: 0x0400011D RID: 285
		protected Rect WindowRect;

		// Token: 0x0400011E RID: 286
		protected UWindow.YAlign _align = UWindow.YAlign.Top;

		// Token: 0x0400011F RID: 287
		protected WindowState _windowState = WindowState.Horizontal;

		// Token: 0x04000120 RID: 288
		protected float _rate;

		// Token: 0x04000121 RID: 289
		protected string _style;

		// Token: 0x04000122 RID: 290
		protected List<UControl> controlList = new List<UControl>();

		// Token: 0x04000123 RID: 291
		private GUI.WindowFunction _winFunc = null;

		// Token: 0x020000F6 RID: 246
		public enum YAlign
		{
			// Token: 0x04000652 RID: 1618
			Top,
			// Token: 0x04000653 RID: 1619
			Bottom,
			// Token: 0x04000654 RID: 1620
			Center
		}
	}
}
