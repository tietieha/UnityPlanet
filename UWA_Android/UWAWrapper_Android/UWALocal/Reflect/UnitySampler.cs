using System;
using System.Reflection;
using UWA;

namespace UWALocal.Reflect
{
	// Token: 0x02000026 RID: 38
	internal class UnitySampler
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000C508 File Offset: 0x0000A708
		public static void StaticInit()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.FullName.Contains("UnityEngine"))
				{
					Sampler._samplerType = assembly.GetType("UnityEngine.Profiling.Sampler");
					Recorder._recorderType = assembly.GetType("UnityEngine.Profiling.Recorder");
					if (Sampler._samplerType != null && Recorder._recorderType != null)
					{
						break;
					}
				}
			}
			if (Sampler._samplerType == null || Recorder._recorderType == null)
			{
				if (SharedUtils.ShowLog)
				{
					SharedUtils.Log("_samplerType or _recorderType not found.");
				}
				return;
			}
			Sampler._samplerGetNames = Sampler._samplerType.GetMethod("GetNames", BindingFlags.Static | BindingFlags.Public);
			Recorder._recorderInvalid = Recorder._recorderType.GetProperty("isValid", BindingFlags.Instance | BindingFlags.Public);
			Recorder._recorderEnabled = Recorder._recorderType.GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
			Recorder._recordersampleBlockCount = Recorder._recorderType.GetProperty("sampleBlockCount", BindingFlags.Instance | BindingFlags.Public);
			Recorder._recorderelapsedNanoseconds = Recorder._recorderType.GetProperty("elapsedNanoseconds", BindingFlags.Instance | BindingFlags.Public);
			Recorder._recorderGet = Recorder._recorderType.GetMethod("Get", BindingFlags.Static | BindingFlags.Public);
			Recorder._recorderCollectFromAllThreads = Recorder._recorderType.GetMethod("CollectFromAllThreads", BindingFlags.Instance | BindingFlags.Public);
			Recorder._recorderFilterToCurrentThread = Recorder._recorderType.GetMethod("FilterToCurrentThread", BindingFlags.Instance | BindingFlags.Public);
			if (Recorder._recorderFilterToCurrentThread != null)
			{
				Recorder.allThread = false;
			}
			UnitySampler._enabled = true;
		}

		// Token: 0x040000EB RID: 235
		public static bool _enabled;
	}
}
