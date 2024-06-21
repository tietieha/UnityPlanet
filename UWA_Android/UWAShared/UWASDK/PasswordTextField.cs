using System;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000021 RID: 33
	public class PasswordTextField : UTextField
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00007BB8 File Offset: 0x00005DB8
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00007BD8 File Offset: 0x00005DD8
		public string Password
		{
			get
			{
				return this.password;
			}
			private set
			{
				this.password = value;
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007BE4 File Offset: 0x00005DE4
		public PasswordTextField(Action<string> func, string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
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

		// Token: 0x0600011F RID: 287 RVA: 0x00007C88 File Offset: 0x00005E88
		public PasswordTextField(Action<string> func, string input, string style, string ph = null, string phstyle = "", float rate = 1f)
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

		// Token: 0x06000120 RID: 288 RVA: 0x00007D30 File Offset: 0x00005F30
		public PasswordTextField(string input, string style, Rect position, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, position, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007DC0 File Offset: 0x00005FC0
		public PasswordTextField(string input, string style, string ph = null, string phstyle = "", float rate = 1f)
		{
			this.text = input;
			base.Init(style, Rect.zero, rate);
			bool flag = ph != null;
			if (flag)
			{
				this.Placeholder = new ULabel(ph, phstyle, new Rect(base.o_Position.x + 30f, base.o_Position.y, base.o_Position.width, base.o_Position.height), 1f);
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007E54 File Offset: 0x00006054
		public override void View()
		{
			bool flag = this.Placeholder != null && string.IsNullOrEmpty(this.text);
			if (flag)
			{
				this.Placeholder.View();
			}
			base.Text = GUI.PasswordField(this.c_Position, base.Text, '*', this.Style);
		}

		// Token: 0x040000C9 RID: 201
		private string password;
	}
}
