using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000026 RID: 38
	public class ScreenTouch : MonoBehaviour
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00009280 File Offset: 0x00007480
		// (set) Token: 0x0600015A RID: 346 RVA: 0x000092A0 File Offset: 0x000074A0
		public Vector2 StartFingerPos
		{
			get
			{
				return this.startFingerPos;
			}
			private set
			{
				this.startFingerPos = value;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600015B RID: 347 RVA: 0x000092AC File Offset: 0x000074AC
		// (set) Token: 0x0600015C RID: 348 RVA: 0x000092CC File Offset: 0x000074CC
		public Vector2 NowFingerPos
		{
			get
			{
				return this.nowFingerPos;
			}
			private set
			{
				this.nowFingerPos = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600015D RID: 349 RVA: 0x000092D8 File Offset: 0x000074D8
		// (set) Token: 0x0600015E RID: 350 RVA: 0x000092F8 File Offset: 0x000074F8
		public bool IsMoving
		{
			get
			{
				return this.isMoved;
			}
			private set
			{
				this.isMoved = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00009304 File Offset: 0x00007504
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00009324 File Offset: 0x00007524
		public float XMoveDistance
		{
			get
			{
				return this._xMoveDistance;
			}
			private set
			{
				this._xMoveDistance = value;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00009330 File Offset: 0x00007530
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00009350 File Offset: 0x00007550
		public float YMoveDistance
		{
			get
			{
				return this._yMoveDistance;
			}
			private set
			{
				this._yMoveDistance = value;
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000935C File Offset: 0x0000755C
		private void Update()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			if (mouseButtonDown)
			{
				this.startFingerPos.x = Input.mousePosition.x;
				this.startFingerPos.y = (float)Screen.height - Input.mousePosition.y;
				this.isMoved = true;
			}
			bool mouseButtonUp = Input.GetMouseButtonUp(0);
			if (mouseButtonUp)
			{
				this.startFingerPos.x = 0f;
				this.startFingerPos.y = 0f;
				this._xMoveDistance = 0f;
				this._yMoveDistance = 0f;
				this.isMoved = false;
			}
			bool flag = this.isMoved;
			if (flag)
			{
				this.nowFingerPos.x = Input.mousePosition.x;
				this.nowFingerPos.y = (float)Screen.height - Input.mousePosition.y;
				this._xMoveDistance = this.nowFingerPos.x - this.startFingerPos.x;
				this._yMoveDistance = this.nowFingerPos.y - this.startFingerPos.y;
			}
		}

		// Token: 0x040000E0 RID: 224
		private Vector2 startFingerPos;

		// Token: 0x040000E1 RID: 225
		private Vector2 nowFingerPos;

		// Token: 0x040000E2 RID: 226
		private bool isMoved;

		// Token: 0x040000E3 RID: 227
		private float _xMoveDistance;

		// Token: 0x040000E4 RID: 228
		private float _yMoveDistance;
	}
}
