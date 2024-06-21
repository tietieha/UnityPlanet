using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000020 RID: 32
	public class FrameTextField : UTextField
	{
		// Token: 0x06000117 RID: 279 RVA: 0x00007898 File Offset: 0x00005A98
		public FrameTextField(Action<string> func, string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
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

		// Token: 0x06000118 RID: 280 RVA: 0x0000793C File Offset: 0x00005B3C
		public FrameTextField(Action<string> func, string input, string style, string ph = null, string phstyle = "", float rate = 1f)
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

		// Token: 0x06000119 RID: 281 RVA: 0x000079E4 File Offset: 0x00005BE4
		public FrameTextField(string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, position, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007A74 File Offset: 0x00005C74
		public FrameTextField(string input, string style, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, Rect.zero, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007B08 File Offset: 0x00005D08
		protected override void CheckText(string newT)
		{
			bool flag = string.IsNullOrEmpty(newT);
			if (flag)
			{
				base.Text = "";
			}
			else
			{
				try
				{
					int num = int.Parse(newT);
					bool flag2 = num > 20;
					if (flag2)
					{
						base.Text = "20";
					}
					else
					{
						bool flag3 = num <= 0;
						if (flag3)
						{
							base.Text = "10";
						}
						else
						{
							base.Text = num.ToString();
						}
					}
				}
				catch (FormatException)
				{
					base.Text = "10";
				}
			}
		}
	}
}
