using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace UWALocal
{
	// Token: 0x02000010 RID: 16
	[Preserve]
	internal static class GotOnlineState
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x000046D4 File Offset: 0x000028D4
		public static bool TryGetAccount()
		{
			string @string = PlayerPrefs.GetString("uwa_online_account", "");
			bool flag = !string.IsNullOrEmpty(@string);
			bool result;
			if (flag)
			{
				GotOnlineState.Account = @string;
				result = true;
			}
			else
			{
				GotOnlineState.Account = "";
				result = false;
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004728 File Offset: 0x00002928
		public static void SaveAccount()
		{
			bool flag = GotOnlineState.Account == null;
			if (!flag)
			{
				bool flag2 = !string.IsNullOrEmpty(GotOnlineState.Account);
				if (flag2)
				{
					PlayerPrefs.SetString("uwa_online_account", GotOnlineState.Account);
				}
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004774 File Offset: 0x00002974
		public static bool TryGetPassword()
		{
			string @string = PlayerPrefs.GetString("uwa_online_password", "");
			bool flag = !string.IsNullOrEmpty(@string);
			bool result;
			if (flag)
			{
				GotOnlineState.Password = @string;
				result = true;
			}
			else
			{
				GotOnlineState.Password = "";
				result = false;
			}
			return result;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000047C8 File Offset: 0x000029C8
		public static void SavePassword()
		{
			bool flag = GotOnlineState.Password == null;
			if (!flag)
			{
				bool flag2 = !string.IsNullOrEmpty(GotOnlineState.Password);
				if (flag2)
				{
					PlayerPrefs.SetString("uwa_online_password", GotOnlineState.Password);
				}
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004814 File Offset: 0x00002A14
		public static void ClearPassword()
		{
			PlayerPrefs.SetString("uwa_online_password", "");
		}

		// Token: 0x04000055 RID: 85
		public static bool Send = false;

		// Token: 0x04000056 RID: 86
		public static bool Show = true;

		// Token: 0x04000057 RID: 87
		public static bool bSavePassword = false;

		// Token: 0x04000058 RID: 88
		public static bool CanCreateProj = false;

		// Token: 0x04000059 RID: 89
		public static bool bBalanceEnough = false;

		// Token: 0x0400005A RID: 90
		public static bool bGetBalance = false;

		// Token: 0x0400005B RID: 91
		public static GotOnlineState.eState State = GotOnlineState.eState.Connected;

		// Token: 0x0400005C RID: 92
		public static string Account = null;

		// Token: 0x0400005D RID: 93
		public static string UserNote = "";

		// Token: 0x0400005E RID: 94
		public static string Password = null;

		// Token: 0x0400005F RID: 95
		public static string AuthCode = "";

		// Token: 0x04000060 RID: 96
		public static List<string> ProjectNameList = null;

		// Token: 0x04000061 RID: 97
		public static int ProjectInd = 0;

		// Token: 0x04000062 RID: 98
		public static string BalanceNotEnough = null;

		// Token: 0x04000063 RID: 99
		public static bool Selecting = false;

		// Token: 0x04000064 RID: 100
		public static Vector2 ScrollViewPos = Vector2.zero;

		// Token: 0x04000065 RID: 101
		public static string Info = null;

		// Token: 0x04000066 RID: 102
		public static string BalanceInfo = null;

		// Token: 0x020000E5 RID: 229
		public enum eState
		{
			// Token: 0x040005FE RID: 1534
			Connecting,
			// Token: 0x040005FF RID: 1535
			Connected,
			// Token: 0x04000600 RID: 1536
			Login
		}
	}
}
