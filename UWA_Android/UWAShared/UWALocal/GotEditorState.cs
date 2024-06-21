using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UWALocal
{
	// Token: 0x02000011 RID: 17
	[Preserve]
	internal static class GotEditorState
	{
		// Token: 0x060000AE RID: 174 RVA: 0x000048B4 File Offset: 0x00002AB4
		public static void TryGetIp()
		{
			string @string = PlayerPrefs.GetString("uwa_editor_ip", "");
			bool flag = @string.Split(new char[]
			{
				'.'
			}).Length == 4;
			if (flag)
			{
				GotEditorState.IP = @string;
			}
			else
			{
				GotEditorState.IP = "";
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004908 File Offset: 0x00002B08
		public static void SaveIp()
		{
			bool flag = GotEditorState.IP == null;
			if (!flag)
			{
				bool flag2 = GotEditorState.IP.Split(new char[]
				{
					'.'
				}).Length == 4;
				if (flag2)
				{
					PlayerPrefs.SetString("uwa_editor_ip", GotEditorState.IP);
				}
			}
		}

		// Token: 0x04000067 RID: 103
		public static bool Send;

		// Token: 0x04000068 RID: 104
		public static bool Show;

		// Token: 0x04000069 RID: 105
		public static string IP;

		// Token: 0x0400006A RID: 106
		public static bool Connecting;

		// Token: 0x0400006B RID: 107
		public static bool Connected;

		// Token: 0x0400006C RID: 108
		public static string Info;
	}
}
