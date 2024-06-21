using System;
using System.Collections.Generic;
using UWA;

namespace UWALocal.Reflect
{
	// Token: 0x0200002A RID: 42
	internal class UnitySamplerReflectTrackingManager : UnitySamplerTrackingManager
	{
		// Token: 0x060001FB RID: 507 RVA: 0x0000C850 File Offset: 0x0000AA50
		public UnitySamplerReflectTrackingManager(string extension, int bufferSize = 200) : base(extension, bufferSize)
		{
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000C85C File Offset: 0x0000AA5C
		protected override void Init()
		{
			try
			{
				UnitySampler.StaticInit();
				if (!UnitySampler._enabled)
				{
					return;
				}
				List<string> sampleNames = new List<string>();
				int names = Sampler.GetNames(sampleNames);
				Recorder recorder = Recorder.Get("Camera.Render");
				if (!recorder.isValid)
				{
					Recorder.v20183 = true;
				}
				this._neededSamples.RemoveAll((UnitySamplerTrackingManager.SampleNode x) => !sampleNames.Contains(x.Name));
				int num = 0;
				for (int i = 0; i < this._neededSamples.Count; i++)
				{
					Recorder recorder2 = Recorder.Get(this._neededSamples[i].Name);
					if (recorder2 != null && recorder2.isValid)
					{
						recorder2.enabledX = true;
						this._neededSamples[i].Uid = num;
						this._neededSamples[i].Rec = new UnityRecord(recorder2);
						string text = this._neededSamples[i].Name;
						string text2 = this._neededSamples[i].ParentName;
						if (text == "Camera.Render")
						{
							text = (Recorder.allThread ? "[All Threads] " : "[Main Thread] ") + text;
						}
						if (text2 == "Camera.Render")
						{
							text2 = (Recorder.allThread ? "[All Threads] " : "[Main Thread] ") + text2;
						}
						UnitySamplerTrackingManager.RegisterSample(text, text2);
						num++;
					}
				}
				if (names > 0 && this._neededSamples.Count > 0)
				{
					this._work = true;
				}
			}
			catch (Exception ex)
			{
				if (SharedUtils.ShowLog)
				{
					string str = "Sampler Failed : ";
					Exception ex2 = ex;
					SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				this._work = false;
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("UnitySample enable : " + this._work.ToString());
			}
			UnitySamplerTrackingManager.RegisterSample("UWALocalStarter:UpdateAtEndAll", null);
			UnitySamplerTrackingManager.RegisterSample("UwaLocalSample", null);
			UnitySamplerTrackingManager.RegisterSample("-", null);
			if (!this._work)
			{
				UnitySamplerTrackingManager.RegisterSample("GC.Collect", null);
			}
		}
	}
}
