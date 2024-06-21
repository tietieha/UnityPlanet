using System;
using UnityEngine;
using UWA;

namespace UWASDK
{
	// Token: 0x0200001A RID: 26
	public class GUITool
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00005AA4 File Offset: 0x00003CA4
		public static GUIStyle labelGuiStyle
		{
			get
			{
				bool flag = GUITool._labelGuiStyle == null;
				if (flag)
				{
					GUITool._labelGuiStyle = new GUIStyle("label");
					GUITool._labelGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.06f);
					GUITool._labelGuiStyle.stretchHeight = true;
					GUITool._labelGuiStyle.alignment = 4;
					GUITool._labelGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
				}
				return GUITool._labelGuiStyle;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00005B40 File Offset: 0x00003D40
		public static GUIStyle labelSmallGuiStyle
		{
			get
			{
				bool flag = GUITool._labelSmallGuiStyle == null;
				if (flag)
				{
					GUITool._labelSmallGuiStyle = new GUIStyle("label");
					GUITool._labelSmallGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
					GUITool._labelSmallGuiStyle.stretchHeight = true;
					GUITool._labelSmallGuiStyle.alignment = 4;
					GUITool._labelSmallGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
				}
				return GUITool._labelSmallGuiStyle;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00005BDC File Offset: 0x00003DDC
		public static GUIStyle labelSmall2GuiStyle
		{
			get
			{
				bool flag = GUITool._labelSmall2GuiStyle == null;
				if (flag)
				{
					GUITool._labelSmall2GuiStyle = new GUIStyle("label");
					GUITool._labelSmall2GuiStyle.alignment = 3;
					GUITool._labelSmall2GuiStyle.wordWrap = true;
					GUITool._labelSmall2GuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
					GUITool._labelSmall2GuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.12f));
				}
				return GUITool._labelSmall2GuiStyle;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00005C78 File Offset: 0x00003E78
		public static GUIStyle buttonGuiStyle
		{
			get
			{
				bool flag = GUITool._buttonGuiStyle == null;
				if (flag)
				{
					GUITool._buttonGuiStyle = new GUIStyle("button");
					GUITool._buttonGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.06f);
					GUITool._buttonGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
				}
				return GUITool._buttonGuiStyle;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00005CFC File Offset: 0x00003EFC
		public static GUIStyle buttonLabelSmallGuiStyle
		{
			get
			{
				bool flag = GUITool._buttonLabelSmallGuiStyle == null;
				if (flag)
				{
					GUITool._buttonLabelSmallGuiStyle = new GUIStyle("button");
					GUITool._buttonLabelSmallGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.03f);
					GUITool._buttonLabelSmallGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
				}
				return GUITool._buttonLabelSmallGuiStyle;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00005D80 File Offset: 0x00003F80
		public static GUIStyle buttonSmall2GuiStyle
		{
			get
			{
				bool flag = GUITool._buttonSmall2GuiStyle == null;
				if (flag)
				{
					GUITool._buttonSmall2GuiStyle = new GUIStyle("button");
					GUITool._buttonSmall2GuiStyle.wordWrap = true;
					GUITool._buttonSmall2GuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
				}
				return GUITool._buttonSmall2GuiStyle;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00005DF0 File Offset: 0x00003FF0
		public static GUIStyle buttonSmall3GuiStyle
		{
			get
			{
				bool flag = GUITool._buttonSmall3GuiStyle == null;
				if (flag)
				{
					GUITool._buttonSmall3GuiStyle = new GUIStyle("button");
					GUITool._buttonSmall3GuiStyle.wordWrap = true;
					GUITool._buttonSmall3GuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
					GUITool._buttonSmall3GuiStyle.alignment = 4;
					GUITool._buttonSmall3GuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.12f));
				}
				return GUITool._buttonSmall3GuiStyle;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00005E8C File Offset: 0x0000408C
		public static GUIStyle buttonSmallGuiStyle
		{
			get
			{
				bool flag = GUITool._buttonSmallGuiStyle == null;
				if (flag)
				{
					GUITool._buttonSmallGuiStyle = new GUIStyle("button");
					GUITool._buttonSmallGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.03f);
					GUITool._buttonSmallGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
					GUITool._buttonSmallGuiStyle.alignment = 3;
					GUITool._buttonSmallGuiStyle.contentOffset = new Vector2((float)((int)(SharedUtils.UploadWinRect.height * 0.05f)), 0f);
				}
				return GUITool._buttonSmallGuiStyle;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005F44 File Offset: 0x00004144
		public static GUIStyle textFieldTipGuiStyle
		{
			get
			{
				bool flag = GUITool._textFieldTipGuiStyle == null;
				if (flag)
				{
					GUITool._textFieldTipGuiStyle = new GUIStyle("textField");
					Font font = Resources.Load<Font>("GUISkin/UWA");
					GUITool._textFieldTipGuiStyle.font = font;
					GUITool._textFieldTipGuiStyle.fontSize = (int)((float)SharedUtils.TipsHeight * 0.5f);
					GUITool._textFieldTipGuiStyle.fixedHeight = (float)((int)((float)SharedUtils.TipsHeight * 2f));
					GUITool._textFieldTipGuiStyle.alignment = 4;
				}
				return GUITool._textFieldTipGuiStyle;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00005FDC File Offset: 0x000041DC
		public static GUIStyle textFieldGuiStyle
		{
			get
			{
				bool flag = GUITool._textFieldGuiStyle == null;
				if (flag)
				{
					GUITool._textFieldGuiStyle = new GUIStyle("textField");
					GUITool._textFieldGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.06f);
					GUITool._textFieldGuiStyle.fixedHeight = (float)((int)(SharedUtils.UploadWinRect.height * 0.1f));
				}
				return GUITool._textFieldGuiStyle;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00006060 File Offset: 0x00004260
		public static GUIStyle textAreaGuiStyle
		{
			get
			{
				bool flag = GUITool._textAreaGuiStyle == null;
				if (flag)
				{
					GUITool._textAreaGuiStyle = new GUIStyle("textArea");
					GUITool._textAreaGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
				}
				return GUITool._textAreaGuiStyle;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000060C4 File Offset: 0x000042C4
		public static GUIStyle textBoxGuiStyle
		{
			get
			{
				bool flag = GUITool._textBoxGuiStyle == null;
				if (flag)
				{
					GUITool._textBoxGuiStyle = new GUIStyle("box");
					GUITool._textBoxGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
					GUITool._textBoxGuiStyle.wordWrap = true;
					GUITool._textBoxGuiStyle.alignment = 3;
				}
				return GUITool._textBoxGuiStyle;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00006140 File Offset: 0x00004340
		public static GUIStyle windowsGuiStyle
		{
			get
			{
				bool flag = GUITool._windowsGuiStyle == null;
				if (flag)
				{
					GUITool._windowsGuiStyle = new GUIStyle("window");
					GUITool._windowsGuiStyle.fontSize = (int)(SharedUtils.UploadWinRect.height * 0.04f);
					GUITool._windowsGuiStyle.padding.top = (int)(SharedUtils.UploadWinRect.height * 0.06f);
				}
				return GUITool._windowsGuiStyle;
			}
		}

		// Token: 0x040000A0 RID: 160
		public static Color DefaultGuiColor = Color.white;

		// Token: 0x040000A1 RID: 161
		private static GUIStyle _buttonGuiStyle = null;

		// Token: 0x040000A2 RID: 162
		private static GUIStyle _buttonSmallGuiStyle = null;

		// Token: 0x040000A3 RID: 163
		private static GUIStyle _buttonSmall2GuiStyle = null;

		// Token: 0x040000A4 RID: 164
		private static GUIStyle _buttonSmall3GuiStyle = null;

		// Token: 0x040000A5 RID: 165
		private static GUIStyle _buttonLabelSmallGuiStyle = null;

		// Token: 0x040000A6 RID: 166
		private static GUIStyle _labelGuiStyle = null;

		// Token: 0x040000A7 RID: 167
		private static GUIStyle _labelSmallGuiStyle = null;

		// Token: 0x040000A8 RID: 168
		private static GUIStyle _labelSmall2GuiStyle = null;

		// Token: 0x040000A9 RID: 169
		private static GUIStyle _textFieldTipGuiStyle = null;

		// Token: 0x040000AA RID: 170
		private static GUIStyle _textFieldGuiStyle = null;

		// Token: 0x040000AB RID: 171
		private static GUIStyle _textAreaGuiStyle = null;

		// Token: 0x040000AC RID: 172
		private static GUIStyle _textBoxGuiStyle = null;

		// Token: 0x040000AD RID: 173
		private static GUIStyle _windowsGuiStyle = null;
	}
}
