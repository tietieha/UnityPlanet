using System;
using System.Reflection;
using UnityEngine.Profiling;
using UWA;

namespace UWACore.Util
{
	// Token: 0x02000048 RID: 72
	internal static class UProfiler
	{
		// Token: 0x0600032E RID: 814 RVA: 0x00015CB4 File Offset: 0x00013EB4
		public static void StaticInit()
		{
			UProfiler._profilerType = typeof(Profiler);
			UProfiler._getAllocatedMemoryForGraphicsDriverMtd = UProfiler._profilerType.GetMethod("GetAllocatedMemoryForGraphicsDriver", BindingFlags.Static | BindingFlags.Public);
			PropertyInfo property = UProfiler._profilerType.GetProperty("maxNumberOfSamplesPerFrame", BindingFlags.Static | BindingFlags.Public);
			bool flag = property != null;
			if (flag)
			{
				property.SetValue(null, -1, null);
			}
			UProfiler._setAreaEnabledMtd = UProfiler._profilerType.GetMethod("SetAreaEnabled", BindingFlags.Static | BindingFlags.Public);
			UProfiler._SetArea = (UProfiler._setAreaEnabledMtd != null);
			UProfiler.SetProfilerArea(true);
			UProfiler._enabled = true;
			Profiler.enabled = false;
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("_enabled:" + UProfiler._enabled.ToString());
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00015D74 File Offset: 0x00013F74
		public static void SetProfilerArea(bool v)
		{
			bool setArea = UProfiler._SetArea;
			if (setArea)
			{
				UProfiler._setAreaEnabledMtd.Invoke(null, new object[]
				{
					0,
					v
				});
				UProfiler._setAreaEnabledMtd.Invoke(null, new object[]
				{
					4,
					v
				});
				UProfiler._setAreaEnabledMtd.Invoke(null, new object[]
				{
					3,
					v
				});
				UProfiler._setAreaEnabledMtd.Invoke(null, new object[]
				{
					6,
					v
				});
				UProfiler._setAreaEnabledMtd.Invoke(null, new object[]
				{
					2,
					v
				});
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00015E48 File Offset: 0x00014048
		public static void BeginRecord(string path)
		{
			UProfiler.SetProfilerArea(true);
			Profiler.enabled = true;
			Profiler.logFile = path;
			Profiler.enableBinaryLog = true;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00015E68 File Offset: 0x00014068
		public static void EndRecord()
		{
			Profiler.enableBinaryLog = false;
			Profiler.logFile = "";
			Profiler.enabled = false;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00015E84 File Offset: 0x00014084
		public static long GetAllocatedMemoryForGraphicsDriver()
		{
			bool flag = !UProfiler._enabled || UProfiler._getAllocatedMemoryForGraphicsDriverMtd == null;
			long result;
			if (flag)
			{
				result = -1L;
			}
			else
			{
				object obj = UProfiler._getAllocatedMemoryForGraphicsDriverMtd.Invoke(null, null);
				result = (long)obj;
			}
			return result;
		}

		// Token: 0x040001D0 RID: 464
		private static MethodInfo _getAllocatedMemoryForGraphicsDriverMtd;

		// Token: 0x040001D1 RID: 465
		private static MethodInfo _setAreaEnabledMtd;

		// Token: 0x040001D2 RID: 466
		private static Type _profilerType;

		// Token: 0x040001D3 RID: 467
		private static bool _SetArea;

		// Token: 0x040001D4 RID: 468
		private static bool _enabled;

		// Token: 0x020000FF RID: 255
		public enum ProfilerArea
		{
			// Token: 0x04000661 RID: 1633
			CPU,
			// Token: 0x04000662 RID: 1634
			GPU,
			// Token: 0x04000663 RID: 1635
			Rendering,
			// Token: 0x04000664 RID: 1636
			Memory,
			// Token: 0x04000665 RID: 1637
			Audio,
			// Token: 0x04000666 RID: 1638
			Video,
			// Token: 0x04000667 RID: 1639
			Physics,
			// Token: 0x04000668 RID: 1640
			Physics2D,
			// Token: 0x04000669 RID: 1641
			NetworkMessages,
			// Token: 0x0400066A RID: 1642
			NetworkOperations,
			// Token: 0x0400066B RID: 1643
			UI,
			// Token: 0x0400066C RID: 1644
			UIDetails,
			// Token: 0x0400066D RID: 1645
			GlobalIllumination
		}
	}
}
