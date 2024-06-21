using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWA;
using UWAShared;

namespace UWALocal
{
	// Token: 0x02000025 RID: 37
	internal class UwaLocalState
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x0000C080 File Offset: 0x0000A280
		public static MonoBehaviour GetStarterMono()
		{
			return UwaLocalStarter.Get();
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000C088 File Offset: 0x0000A288
		public static void UpdateMainThreadActions()
		{
			if (UwaLocalState.MainThreadActions.Count == 0)
			{
				return;
			}
			object obj = UwaLocalState.actionLockObj;
			lock (obj)
			{
				if (UwaLocalState.MainThreadActions.Count > 0)
				{
					for (int i = 0; i < UwaLocalState.MainThreadActions.Count; i++)
					{
						if (UwaLocalState.MainThreadActions[i] != null)
						{
							UwaLocalState.MainThreadActions[i]();
						}
					}
					UwaLocalState.MainThreadActions.Clear();
				}
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000C120 File Offset: 0x0000A320
		public static bool DoOnGUI()
		{
			bool result = false;
			if (UwaLocalState.canNotAccessDataRoot)
			{
				GUILayout.Button(string.Concat(new string[]
				{
					"  No permissoin to write [",
					SharedUtils.GetAndroidAppSDK_INT().ToString(),
					"][",
					UwaLocalState.writeSdcardPermision.ToString(),
					"].\n",
					SharedUtils.FinalDataPath
				}), new GUILayoutOption[0]);
				result = true;
			}
			if (UwaLocalState.uwaNotFound)
			{
				GUILayout.Button("  libuwa.so is not found.", new GUILayoutOption[0]);
				result = true;
			}
			if (UwaLocalState.nativeVersionMismatch)
			{
				GUILayout.Button("  Legacy libuwa.so(" + UwaLocalState.nativeVersion.ToString() + ").", new GUILayoutOption[0]);
				result = true;
			}
			return result;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000C1E4 File Offset: 0x0000A3E4
		public static void CheckScriptBackend()
		{
			try
			{
				UwaLocalState.monoFailedToPreload = !UwaProfiler.PreLoad();
			}
			catch (Exception)
			{
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UwaProfiler.PreLoad : " + (!UwaLocalState.monoFailedToPreload).ToString());
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000C244 File Offset: 0x0000A444
		public static void CheckNative(int NATIVE_VERSION)
		{
			UwaLocalState.uwaNotFound = (CoreUtils.GetNativeDllPath("libuwa") == null);
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("uwaNotFound: " + UwaLocalState.uwaNotFound.ToString());
			}
			try
			{
				UwaLocalState.nativeVersion = UwaProfiler.NativeVersion();
				UwaLocalState.nativeVersionMismatch = (UwaLocalState.nativeVersion != NATIVE_VERSION);
			}
			catch (Exception)
			{
				UwaLocalState.nativeVersion = -1;
				UwaLocalState.nativeVersionMismatch = true;
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("Native version: " + UwaLocalState.nativeVersion.ToString());
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000C2EC File Offset: 0x0000A4EC
		public static bool CheckProviderConfig(string config)
		{
			try
			{
				if (File.Exists(config))
				{
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("File.Exists : " + config);
					}
					JSONNode jsonnode = JSON.Parse(File.ReadAllText(config));
					string text = jsonnode["dataDirectory"];
					if (!string.IsNullOrEmpty(text))
					{
						SharedUtils.OverrideFinalPath(Application.persistentDataPath + "/" + text);
						if (SharedUtils.ShowLog)
						{
							SharedUtils.Log("Set override datapath : " + text);
						}
						File.Delete(config);
						return true;
					}
					File.Delete(config);
				}
			}
			catch (Exception ex)
			{
				if (SharedUtils.ShowLog)
				{
					string str = "Read gotConfig.json Ex:";
					Exception ex2 = ex;
					SharedUtils.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
			return false;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000C3D4 File Offset: 0x0000A5D4
		public static void UpdateSceneName()
		{
			string loadedLevelName = Application.loadedLevelName;
			if (loadedLevelName == UwaLocalState.lastSceneName)
			{
				return;
			}
			UwaLocalState.lastSceneName = loadedLevelName;
			if (UwaLocalState.onLevelChanged != null)
			{
				UwaLocalState.onLevelChanged();
			}
			string contents;
			if (UwaLocalStarter.Get().TestMode == UwaLocalStarter.eTestMode.GPU)
			{
				contents = SharedUtils.frameId.ToString() + "," + Uri.EscapeDataString(loadedLevelName) + "\n";
			}
			else
			{
				contents = SharedUtils.frameId.ToString() + "," + loadedLevelName + "\n";
			}
			File.AppendAllText(SharedUtils.FinalDataPath + "/scene", contents);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000C480 File Offset: 0x0000A680
		public static void ClearLastSceneName()
		{
			UwaLocalState.lastSceneName = string.Empty;
		}

		// Token: 0x040000DB RID: 219
		public static bool writeSdcardPermision = false;

		// Token: 0x040000DC RID: 220
		public static bool canNotAccessDataRoot = false;

		// Token: 0x040000DD RID: 221
		public static bool uwaNotFound = false;

		// Token: 0x040000DE RID: 222
		public static bool nativeVersionMismatch = false;

		// Token: 0x040000DF RID: 223
		public static int nativeVersion = 0;

		// Token: 0x040000E0 RID: 224
		public static bool monoFailedToPreload = false;

		// Token: 0x040000E1 RID: 225
		public static bool devNotSupported = false;

		// Token: 0x040000E2 RID: 226
		public static DumpHelper currentDumpHelper = null;

		// Token: 0x040000E3 RID: 227
		public static bool pocoStart = false;

		// Token: 0x040000E4 RID: 228
		public static bool pocoStrip = true;

		// Token: 0x040000E5 RID: 229
		public static string Date = null;

		// Token: 0x040000E6 RID: 230
		public static bool WFInited = false;

		// Token: 0x040000E7 RID: 231
		public static object actionLockObj = new object();

		// Token: 0x040000E8 RID: 232
		public static List<Action> MainThreadActions = new List<Action>();

		// Token: 0x040000E9 RID: 233
		public static string lastSceneName = null;

		// Token: 0x040000EA RID: 234
		public static UwaLocalState.OnLevelChange onLevelChanged;

		// Token: 0x020000E0 RID: 224
		// (Invoke) Token: 0x060008E7 RID: 2279
		public delegate void OnLevelChange();
	}
}
