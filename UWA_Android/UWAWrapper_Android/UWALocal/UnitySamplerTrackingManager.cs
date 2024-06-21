using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;
using UWA;

namespace UWALocal
{
	// Token: 0x02000024 RID: 36
	internal class UnitySamplerTrackingManager : BaseTrackerManager
	{
		// Token: 0x060001DB RID: 475
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		private static extern void LogSample(int id, long cost, int count);

		// Token: 0x060001DC RID: 476
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void RegisterSample(string name, string pName);

		// Token: 0x060001DD RID: 477 RVA: 0x0000B968 File Offset: 0x00009B68
		protected virtual void Init()
		{
			try
			{
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

		// Token: 0x060001DE RID: 478 RVA: 0x0000BBBC File Offset: 0x00009DBC
		public UnitySamplerTrackingManager(string extension, int bufferSize = 200) : base(extension, bufferSize)
		{
			this.Init();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000BFE8 File Offset: 0x0000A1E8
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			if (!this._work)
			{
				base.Enabled = false;
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000BFFC File Offset: 0x0000A1FC
		protected override void DoUpdateAtEnd()
		{
			if (!this._work)
			{
				return;
			}
			for (int i = 0; i < this._neededSamples.Count; i++)
			{
				BaseRecord rec = this._neededSamples[i].Rec;
				if (rec != null && rec.isValid)
				{
					int sampleBlockCountX = rec.sampleBlockCountX;
					if (sampleBlockCountX != 0)
					{
						UnitySamplerTrackingManager.LogSample(this._neededSamples[i].Uid, rec.elapsedNanosecondsX, sampleBlockCountX);
					}
				}
			}
		}

		// Token: 0x040000D9 RID: 217
		protected bool _work;

		// Token: 0x040000DA RID: 218
		protected readonly List<UnitySamplerTrackingManager.SampleNode> _neededSamples = new List<UnitySamplerTrackingManager.SampleNode>
		{
			new UnitySamplerTrackingManager.SampleNode("Camera.Render", null),
			new UnitySamplerTrackingManager.SampleNode("Drawing", "Camera.Render"),
			new UnitySamplerTrackingManager.SampleNode("Culling", "Camera.Render"),
			new UnitySamplerTrackingManager.SampleNode("Camera.ImageEffects", "Camera.Render"),
			new UnitySamplerTrackingManager.SampleNode("Render.TransparentGeometry", "Drawing"),
			new UnitySamplerTrackingManager.SampleNode("Render.OpaqueGeometry", "Drawing"),
			new UnitySamplerTrackingManager.SampleNode("RenderForwardAlpha.Render", "Render.TransparentGeometry"),
			new UnitySamplerTrackingManager.SampleNode("RenderForwardOpaque.Render", "Render.OpaqueGeometry"),
			new UnitySamplerTrackingManager.SampleNode("Shadows.PrepareShadowmap", "Render.OpaqueGeometry"),
			new UnitySamplerTrackingManager.SampleNode("Canvas.RenderSubBatch", "RenderForwardAlpha.Render"),
			new UnitySamplerTrackingManager.SampleNode("Shadows.RenderShadowMap", "RenderForwardOpaque.Render"),
			new UnitySamplerTrackingManager.SampleNode("PostLateUpdate.UpdateAllRenderers", null),
			new UnitySamplerTrackingManager.SampleNode("Gfx.WaitForPresent", null),
			new UnitySamplerTrackingManager.SampleNode("Gfx.WaitForPresentOnGfxThread", null),
			new UnitySamplerTrackingManager.SampleNode("Graphics.PresentAndSync", null),
			new UnitySamplerTrackingManager.SampleNode("PostLateUpdate.PresentAfterDraw", null),
			new UnitySamplerTrackingManager.SampleNode("PreLateUpdate.EndGraphicsJobsLate", null),
			new UnitySamplerTrackingManager.SampleNode("PreLateUpdate.EndGraphicsJobsAfterScriptUpdate", null),
			new UnitySamplerTrackingManager.SampleNode("Animators.Update", null),
			new UnitySamplerTrackingManager.SampleNode("Animators.ProcessRootMotionJob", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animators.FireAnimationEventsAndBehaviours", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animators.ApplyOnAnimatorMove", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animator.ApplyOnAnimatorMove", "Animators.ApplyOnAnimatorMove"),
			new UnitySamplerTrackingManager.SampleNode("Animators.ProcessAnimationsJob", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animators.WriteJob", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animators.DirtySceneObjects", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("Animators.SortWriteJob", "Animators.Update"),
			new UnitySamplerTrackingManager.SampleNode("AnimatorControllerPlayable.PrepareFrame", null),
			new UnitySamplerTrackingManager.SampleNode("Animation.Update", null),
			new UnitySamplerTrackingManager.SampleNode("MeshSkinning.Update", null),
			new UnitySamplerTrackingManager.SampleNode("ParticleSystem.Update", null),
			new UnitySamplerTrackingManager.SampleNode("ParticleSystem.SubmitVBO", "RenderForwardAlpha.Render"),
			new UnitySamplerTrackingManager.SampleNode("ParticleSystem.Draw", "RenderForwardAlpha.Render"),
			new UnitySamplerTrackingManager.SampleNode("Physics.Processing", null),
			new UnitySamplerTrackingManager.SampleNode("Physics.Simulate", null),
			new UnitySamplerTrackingManager.SampleNode("Physics.ProcessReports", null),
			new UnitySamplerTrackingManager.SampleNode("Physics.FetchResults", null),
			new UnitySamplerTrackingManager.SampleNode("Canvas.BuildBatch", null),
			new UnitySamplerTrackingManager.SampleNode("PostLateUpdate.PlayerEmitCanvasGeometry", null),
			new UnitySamplerTrackingManager.SampleNode("SortingGroupManager.Update", null),
			new UnitySamplerTrackingManager.SampleNode("SortingGroup.SortChildren", "SortingGroupManager.Update"),
			new UnitySamplerTrackingManager.SampleNode("Loading.UpdatePreloading", null),
			new UnitySamplerTrackingManager.SampleNode("Application.Integrate Assets in Background", "Loading.UpdatePreloading"),
			new UnitySamplerTrackingManager.SampleNode("Preload Single Step", "Application.Integrate Assets in Background"),
			new UnitySamplerTrackingManager.SampleNode("GarbageCollectAssetsProfile", "Preload Single Step"),
			new UnitySamplerTrackingManager.SampleNode("Application.LoadLevelAsync Integrate", "Preload Single Step"),
			new UnitySamplerTrackingManager.SampleNode("WaitForTargetFPS", null),
			new UnitySamplerTrackingManager.SampleNode("Instantiate", "UwaLocalSample"),
			new UnitySamplerTrackingManager.SampleNode("Loading.ReadObject", "UwaLocalSample"),
			new UnitySamplerTrackingManager.SampleNode("Animation.RebuildInternalState", "UwaLocalSample"),
			new UnitySamplerTrackingManager.SampleNode("Animator.Initialize", "UwaLocalSample"),
			new UnitySamplerTrackingManager.SampleNode("Shader.Parse", "UwaLocalSample"),
			new UnitySamplerTrackingManager.SampleNode("GC.Collect", "UwaLocalSample")
		};

		// Token: 0x020000DE RID: 222
		public class SampleNode
		{
			// Token: 0x060008E3 RID: 2275 RVA: 0x0003BCD0 File Offset: 0x00039ED0
			public SampleNode(string name, string pName)
			{
				this.Name = name;
				this.ParentName = pName;
			}

			// Token: 0x040005EF RID: 1519
			public string Name;

			// Token: 0x040005F0 RID: 1520
			public string ParentName;

			// Token: 0x040005F1 RID: 1521
			public int Uid;

			// Token: 0x040005F2 RID: 1522
			public BaseRecord Rec;
		}
	}
}
