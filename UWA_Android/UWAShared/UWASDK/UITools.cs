using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000019 RID: 25
	internal static class UITools
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00005980 File Offset: 0x00003B80
		private static int ScreenWidth
		{
			get
			{
				return (Screen.width > Screen.height) ? Screen.width : Screen.height;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000CC RID: 204 RVA: 0x000059B8 File Offset: 0x00003BB8
		private static int ScreenHeight
		{
			get
			{
				return (Screen.width > Screen.height) ? Screen.height : Screen.width;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000059F0 File Offset: 0x00003BF0
		public static int OnGUIWinLen
		{
			get
			{
				return (int)((double)UITools.ScreenHeight * 0.9);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00005A1C File Offset: 0x00003C1C
		public static float ScreenSizeScale
		{
			get
			{
				bool flag = UITools.ScreenHeight < 750;
				float result;
				if (flag)
				{
					result = 1.5f;
				}
				else
				{
					result = 1f;
				}
				return result;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00005A58 File Offset: 0x00003C58
		public static Rect OnGUIWinRect
		{
			get
			{
				return new Rect((float)(Screen.width / 2 - UITools.OnGUIWinLen / 2), (float)(Screen.height / 2 - UITools.OnGUIWinLen / 2), (float)UITools.OnGUIWinLen, (float)UITools.OnGUIWinLen);
			}
		}
	}
}
