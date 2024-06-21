using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x02000038 RID: 56
	internal class AssetTrackManager : BaseTrackerManager
	{
		// Token: 0x06000280 RID: 640 RVA: 0x00010738 File Offset: 0x0000E938
		public AssetTrackManager(string extension) : base(extension, 200)
		{
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000107F4 File Offset: 0x0000E9F4
		public void MakeNextGetAllAssets()
		{
			this._alltypes = true;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00010800 File Offset: 0x0000EA00
		public void CheckVisible(bool check)
		{
			this._checkVisible = check;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0001080C File Offset: 0x0000EA0C
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			AssetTrackManager._tasks.Clear();
			bool checkVisible = this._checkVisible;
			if (checkVisible)
			{
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Texture2D), 2f));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Mesh), 2f));
			}
			else
			{
				bool flag = config.Count != 0;
				if (flag)
				{
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Texture2D), float.Parse(config["Texture2D"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Mesh), float.Parse(config["Mesh"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AnimationClip), float.Parse(config["AnimationClip"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AudioClip), float.Parse(config["AudioClip"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Font), float.Parse(config["Font"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Material), float.Parse(config["Material"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(MeshCollider), float.Parse(config["MeshCollider"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(ParticleSystem), float.Parse(config["ParticleSystem"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(RenderTexture), float.Parse(config["RenderTexture"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Shader), float.Parse(config["Shader"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(SkinnedMeshRenderer), float.Parse(config["SkinnedMeshRenderer"])));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Animator), float.Parse(config["Animator"])));
				}
				else
				{
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AnimationClip), 5f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AudioClip), 20f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Font), -1f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Material), 3f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Mesh), 10f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(MeshCollider), 10f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(ParticleSystem), 3f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(RenderTexture), 10f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Shader), 20f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Texture2D), 10f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(SkinnedMeshRenderer), 3f));
					AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Animator), 10f));
				}
			}
			PropertyInfo property = typeof(QualitySettings).GetProperty("skinWeights");
			bool flag2 = property == null;
			if (flag2)
			{
				property = typeof(QualitySettings).GetProperty("blendWeights");
			}
			bool flag3 = property != null;
			if (flag3)
			{
				AssetTrackManager.QualitySettings_blendWeights = (int)property.GetValue(null, null);
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00010C58 File Offset: 0x0000EE58
		protected override void DoUpdateAtEnd()
		{
			try
			{
				for (int i = 0; i < AssetTrackManager._tasks.Count; i++)
				{
					AssetTrackManager.AssetTrackTask task = AssetTrackManager._tasks[i];
					this.WriteInfoOfAssetType(task, this._alltypes);
				}
			}
			catch (IOException ex)
			{
				SharedUtils.Log("AssetTrackManager writeLine failed ! \n" + ex.StackTrace);
			}
			bool alltypes = this._alltypes;
			if (alltypes)
			{
				base.TrackWriter.BufferToFile();
			}
			this._alltypes = false;
			this.switchTimer += Time.unscaledDeltaTime;
			bool flag = this.switchTimer > this.swithInterval;
			if (flag)
			{
				this.SwitchCache();
				this.switchTimer = 0f;
			}
			bool checkVisible = this._checkVisible;
			if (checkVisible)
			{
				GL.IssuePluginEvent(2020001);
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00010D50 File Offset: 0x0000EF50
		private void SwitchCache()
		{
			Dictionary<int, string> lastAddedInfoCache = this._lastAddedInfoCache;
			this._lastAddedInfoCache = this._currAddedInfoCache;
			this._currAddedInfoCache = lastAddedInfoCache;
			this._currAddedInfoCache.Clear();
			Dictionary<int, string> lastNameCache = this._lastNameCache;
			this._lastNameCache = this._currNameCache;
			this._currNameCache = lastNameCache;
			this._currNameCache.Clear();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00010DB0 File Offset: 0x0000EFB0
		private void WriteInfoOfAssetType(AssetTrackManager.AssetTrackTask task, bool allTypes = false)
		{
			bool flag = !allTypes && task.trackInternal + 1f < float.Epsilon;
			if (!flag)
			{
				task.trackCurrent += Time.unscaledDeltaTime;
				bool flag2 = !allTypes && task.trackCurrent < task.trackInternal;
				if (!flag2)
				{
					Object[] array = Resources.FindObjectsOfTypeAll(task.type);
					string name = task.type.Name;
					bool flag3 = array.Length == 0;
					if (flag3)
					{
						base.TrackWriter.WriteToBuffer(string.Format("{0},{1},0", SharedUtils.frameId, name));
					}
					bool flag4 = !this._NoCacheType.Contains(task.type);
					for (int i = 0; i < array.Length; i++)
					{
						string text = null;
						int instanceID = array[i].GetInstanceID();
						bool flag5 = name == "RenderTexture" && !((RenderTexture)array[i]).IsCreated();
						if (!flag5)
						{
							bool flag6 = flag4;
							if (flag6)
							{
								bool flag7 = !this._currAddedInfoCache.TryGetValue(instanceID, out text);
								if (flag7)
								{
									bool flag8 = !this._lastAddedInfoCache.TryGetValue(instanceID, out text);
									if (flag8)
									{
										text = this.GetAddedInfo4Asset(task.type, array[i]);
									}
									this._currAddedInfoCache.Add(instanceID, text);
								}
							}
							else
							{
								text = this.GetAddedInfo4Asset(task.type, array[i]);
							}
							string text2 = null;
							bool flag9 = !this._currNameCache.TryGetValue(instanceID, out text2);
							if (flag9)
							{
								bool flag10 = !this._lastNameCache.TryGetValue(instanceID, out text2);
								if (flag10)
								{
									text2 = array[i].name;
									bool flag11 = task.type == typeof(ParticleSystem);
									if (flag11)
									{
										try
										{
											int num = 2;
											Transform parent = ((ParticleSystem)array[i]).transform.parent;
											while (num > 0 && parent != null)
											{
												text2 = parent.name + "/" + text2;
												parent = parent.parent;
												num--;
											}
										}
										catch (Exception)
										{
										}
									}
									text2 = CoreUtils.StringReplace(text2);
								}
								this._currNameCache.Add(instanceID, text2);
							}
							long runtimeMemorySizeLong = Profiler.GetRuntimeMemorySizeLong(array[i]);
							base.TrackWriter.WriteToBuffer(string.Format("{0},{1},{2},{3},{4},{5}{6}", new object[]
							{
								SharedUtils.frameId,
								name,
								text2,
								runtimeMemorySizeLong,
								array[i].GetInstanceID(),
								array[i].hideFlags,
								text
							}));
						}
					}
					task.trackCurrent = 0f;
				}
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00011100 File Offset: 0x0000F300
		protected string GetAddedInfo4Asset(Type t, Object obj)
		{
			string result = "";
			bool flag = t == typeof(Texture2D);
			if (flag)
			{
				Texture2D texture2D = obj as Texture2D;
				int mipmapCount = texture2D.mipmapCount;
				result = string.Format(",{0},{1},{2},{3}", new object[]
				{
					texture2D.height,
					texture2D.width,
					texture2D.format,
					mipmapCount
				});
			}
			bool flag2 = t == typeof(RenderTexture);
			if (flag2)
			{
				RenderTexture renderTexture = obj as RenderTexture;
				result = string.Format(",{0},{1},{2},{3}", new object[]
				{
					renderTexture.height,
					renderTexture.width,
					renderTexture.format,
					renderTexture.antiAliasing
				});
			}
			bool flag3 = t == typeof(Mesh);
			if (flag3)
			{
				Mesh mesh = obj as Mesh;
				bool flag4 = mesh.isReadable && mesh.subMeshCount != 0 && mesh.GetTopology(0) == 0;
				if (flag4)
				{
					bool flag5 = mesh.vertexCount != 0;
					if (flag5)
					{
						uint num = MeshPropTool.TotalIndexCount(mesh);
						bool flag6 = MeshPropTool.HasProp(mesh, MeshPropTool.MeshProp.Normal);
						bool flag7 = MeshPropTool.HasProp(mesh, MeshPropTool.MeshProp.Color);
						bool flag8 = MeshPropTool.HasProp(mesh, MeshPropTool.MeshProp.Tangent);
						bool flag9 = MeshPropTool.HasProp(mesh, MeshPropTool.MeshProp.BoneWeight);
						int num2 = (int)((num != 0U) ? num : ((uint)(mesh.triangles.Length / 3)));
						int num3 = flag6 ? mesh.vertexCount : 0;
						int num4 = flag7 ? mesh.vertexCount : 0;
						int num5 = flag8 ? mesh.vertexCount : 0;
						int num6 = flag9 ? mesh.vertexCount : 0;
						result = string.Format(",{0},{1},{2},{3},{4},{5}", new object[]
						{
							mesh.vertexCount,
							num2,
							num3,
							num4,
							num5,
							num6
						});
					}
					else
					{
						result = ",0,0,0,0,0,0";
					}
				}
				else
				{
					result = string.Format(",{0},-1,-1,-1,-1,-1", mesh.vertexCount);
				}
			}
			bool flag10 = t == typeof(Shader);
			if (flag10)
			{
				Shader shader = obj as Shader;
				result = string.Format(",{0}", shader.isSupported);
			}
			bool flag11 = t == typeof(AnimationClip);
			if (flag11)
			{
				AnimationClip animationClip = obj as AnimationClip;
				result = string.Format(",{0:#.00},{1}", animationClip.length, animationClip.frameRate);
			}
			bool flag12 = t == typeof(AudioClip);
			if (flag12)
			{
				AudioClip audioClip = obj as AudioClip;
				result = string.Format(",{0},{1},{2}", audioClip.length, audioClip.samples, audioClip.loadType);
			}
			bool flag13 = t == typeof(ParticleSystem);
			if (flag13)
			{
				ParticleSystem particleSystem = obj as ParticleSystem;
				result = string.Format(",{0},{1}", particleSystem.particleCount, particleSystem.isPlaying);
			}
			bool flag14 = t == typeof(SkinnedMeshRenderer);
			if (flag14)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = obj as SkinnedMeshRenderer;
				result = string.Format(",{0},{1}", skinnedMeshRenderer.bones.Length, (skinnedMeshRenderer.quality == null) ? AssetTrackManager.QualitySettings_blendWeights : skinnedMeshRenderer.quality);
			}
			bool flag15 = t == typeof(Font);
			if (flag15)
			{
				Font font = obj as Font;
				result = string.Format(",{0},{1}", font.fontSize, font.dynamic);
			}
			bool flag16 = t == typeof(Animator);
			if (flag16)
			{
				Animator animator = obj as Animator;
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					bool checkVisible = this._checkVisible;
					if (checkVisible)
					{
						bool enabled = animator.enabled;
						if (enabled)
						{
							for (int i = 0; i < animator.layerCount; i++)
							{
								AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(i);
								bool flag17 = currentAnimatorClipInfo != null;
								if (flag17)
								{
									for (int j = 0; j < currentAnimatorClipInfo.Length; j++)
									{
										AnimationClip clip = currentAnimatorClipInfo[j].clip;
										bool flag18 = clip != null;
										if (flag18)
										{
											stringBuilder.AppendFormat("{0}|", CoreUtils.StringReplace(clip.name));
										}
									}
								}
							}
						}
					}
					result = string.Format(",{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
					{
						(animator.runtimeAnimatorController == null) ? "None" : animator.runtimeAnimatorController.name,
						stringBuilder,
						animator.applyRootMotion,
						animator.cullingMode,
						animator.hasTransformHierarchy,
						animator.isOptimizable,
						animator.layerCount,
						animator.updateMode,
						animator.isHuman
					});
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		// Token: 0x04000168 RID: 360
		private float switchTimer = 0f;

		// Token: 0x04000169 RID: 361
		private float swithInterval = 20f;

		// Token: 0x0400016A RID: 362
		private bool _checkVisible = false;

		// Token: 0x0400016B RID: 363
		private Dictionary<int, string> _lastAddedInfoCache = new Dictionary<int, string>();

		// Token: 0x0400016C RID: 364
		private Dictionary<int, string> _currAddedInfoCache = new Dictionary<int, string>();

		// Token: 0x0400016D RID: 365
		private Dictionary<int, string> _currNameCache = new Dictionary<int, string>();

		// Token: 0x0400016E RID: 366
		private Dictionary<int, string> _lastNameCache = new Dictionary<int, string>();

		// Token: 0x0400016F RID: 367
		private HashSet<Type> _NoCacheType = new HashSet<Type>
		{
			typeof(ParticleSystem),
			typeof(AudioClip),
			typeof(Animator)
		};

		// Token: 0x04000170 RID: 368
		private List<Vector3> tempList = new List<Vector3>();

		// Token: 0x04000171 RID: 369
		private static int QualitySettings_blendWeights = 0;

		// Token: 0x04000172 RID: 370
		private static List<AssetTrackManager.AssetTrackTask> _tasks = new List<AssetTrackManager.AssetTrackTask>();

		// Token: 0x04000173 RID: 371
		private bool _alltypes = false;

		// Token: 0x020000F7 RID: 247
		private class AssetTrackTask
		{
			// Token: 0x06000930 RID: 2352 RVA: 0x0003CA0C File Offset: 0x0003AC0C
			public AssetTrackTask(Type t, float f)
			{
				this.type = t;
				this.trackInternal = f;
				this.trackCurrent = 0f;
			}

			// Token: 0x04000630 RID: 1584
			public Type type;

			// Token: 0x04000631 RID: 1585
			public float trackInternal;

			// Token: 0x04000632 RID: 1586
			public float trackCurrent;
		}
	}
}
