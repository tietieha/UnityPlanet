using System;
using UnityEngine;
using UWA;
using UWASDK;

namespace UWAShared
{
	// Token: 0x02000040 RID: 64
	public class UWAEngine
	{
		// Token: 0x0600028C RID: 652 RVA: 0x00019574 File Offset: 0x00017774
		public static void StaticInit()
		{
			bool flag = GUIWrapper.Get() == null;
			if (flag)
			{
				GameObject gameObject = new GameObject("UWA_Launcher");
				gameObject.AddComponent<GUIWrapper>();
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600028D RID: 653 RVA: 0x000195AC File Offset: 0x000177AC
		public static bool AutoLaunch
		{
			get
			{
				return SharedUtils.AutoLaunch;
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000195CC File Offset: 0x000177CC
		public static void Start(UWAEngine.Mode mode)
		{
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000195D0 File Offset: 0x000177D0
		public static void Stop()
		{
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000195D4 File Offset: 0x000177D4
		public static void PushSample(string sampleName)
		{
		}

		// Token: 0x06000291 RID: 657 RVA: 0x000195D8 File Offset: 0x000177D8
		public static void PopSample()
		{
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000195DC File Offset: 0x000177DC
		public static void LogValue(string valueName, float value)
		{
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000195E0 File Offset: 0x000177E0
		public static void LogValue(string valueName, Vector3 value)
		{
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000195E4 File Offset: 0x000177E4
		public static void LogValue(string valueName, int value)
		{
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000195E8 File Offset: 0x000177E8
		public static void LogValue(string valueName, bool value)
		{
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000195EC File Offset: 0x000177EC
		public static void AddMarker(string valueName)
		{
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000195F0 File Offset: 0x000177F0
		public static void SetOverrideLuaLib(string luaLib)
		{
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000195F4 File Offset: 0x000177F4
		public static void SetOverrideLuaState(object luaState)
		{
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000195F8 File Offset: 0x000177F8
		public static void Upload(Action<bool> callback, string user, string password, string projectName, int timeLimitS)
		{
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000195FC File Offset: 0x000177FC
		public static void Upload(Action<bool> callback, string user, string password, int projectId, int timeLimitS)
		{
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00019600 File Offset: 0x00017800
		public static void Tag(string tag)
		{
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00019604 File Offset: 0x00017804
		public static void Note(string note)
		{
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00019608 File Offset: 0x00017808
		public static void SetUIActive(bool active)
		{
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0001960C File Offset: 0x0001780C
		public static void Dump(eDumpType t)
		{
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00019610 File Offset: 0x00017810
		public static void SetOverrideAndroidActivity(AndroidJavaObject activity)
		{
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00019614 File Offset: 0x00017814
		public static void SetConfig(string startConfig)
		{
		}

		// Token: 0x0400021A RID: 538
		public static int FrameId;

		// Token: 0x020000FF RID: 255
		public enum Mode
		{
			// Token: 0x0400066A RID: 1642
			Test
		}
	}
}
