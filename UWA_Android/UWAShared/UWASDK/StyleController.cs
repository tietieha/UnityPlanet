using System;
using System.Collections.Generic;

namespace UWASDK
{
	// Token: 0x02000027 RID: 39
	internal class StyleController
	{
		// Token: 0x06000165 RID: 357 RVA: 0x00009488 File Offset: 0x00007688
		private StyleController()
		{
			this.styles = new Dictionary<string, string>();
			this.whiteList = new List<string>
			{
				"box",
				"button",
				"toggle",
				"label",
				"textfield",
				"textarea",
				"window",
				"horizontalslider",
				"horizontalsliderthumb",
				"verticalslider",
				"verticalsliderthumb",
				"horizontalscrollbar",
				"horizontalscrollbarthumb",
				"horizontalscrollbarleftbutton",
				"horizontalscrollbarrightbutton",
				"verticalscrollbar",
				"verticalscrollbarthumb",
				"verticalscrollbarupbutton",
				"verticalscrollbardownbutton",
				"scrollview"
			};
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000095B0 File Offset: 0x000077B0
		public static StyleController Instance
		{
			get
			{
				bool flag = StyleController._instance == null;
				if (flag)
				{
					StyleController._instance = new StyleController();
				}
				return StyleController._instance;
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000095EC File Offset: 0x000077EC
		public void SetEmpty(bool e = true)
		{
			this.empty = e;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000095F8 File Offset: 0x000077F8
		public string GetStyle(string s, string fail_replace = "label")
		{
			bool flag = this.whiteList.Contains(s);
			string result;
			if (flag)
			{
				result = s;
			}
			else
			{
				bool flag2 = this.empty;
				if (flag2)
				{
					string text;
					bool flag3 = this.styles.TryGetValue(s, out text);
					if (flag3)
					{
						result = text;
					}
					else
					{
						result = fail_replace;
					}
				}
				else
				{
					result = s;
				}
			}
			return result;
		}

		// Token: 0x040000E5 RID: 229
		private Dictionary<string, string> styles;

		// Token: 0x040000E6 RID: 230
		private List<string> whiteList;

		// Token: 0x040000E7 RID: 231
		private static StyleController _instance;

		// Token: 0x040000E8 RID: 232
		private bool empty = false;
	}
}
