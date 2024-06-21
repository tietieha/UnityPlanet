using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using UWA;
using UWACore.RenderTools;
using UWACore.Util;

namespace UWALocal
{
	// Token: 0x0200001E RID: 30
	internal class GpuTrackManager : BaseTrackerManager
	{
		// Token: 0x060001AE RID: 430 RVA: 0x0000B448 File Offset: 0x00009648
		internal GpuTrackManager(string extension) : base(extension, 200)
		{
			GpuTrackManager.GpuResourcesPath = SharedUtils.FinalDataPath + "/gpu_resources";
			GpuTrackManager.GpuOverdrawPath = SharedUtils.FinalDataPath + "/overdraw";
			Directory.CreateDirectory(GpuTrackManager.GpuResourcesPath);
			Directory.CreateDirectory(GpuTrackManager.GpuOverdrawPath);
			this.analysisTrack = new GpuTrackManager.AnalysisTrack(GpuTrackManager.GpuModeFeature);
			this.overdrawTrack = new GpuTrackManager.OverdrawTrack();
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000B4C0 File Offset: 0x000096C0
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			GpuTrackManager.sxor = UwaLocalState.Date;
			if (GpuTrackManager.sxor != null)
			{
				GpuTrackManager.lxor = GpuTrackManager.sxor.Length;
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000B4E8 File Offset: 0x000096E8
		private static int GenerateHashCode(string str)
		{
			int num = 131;
			int num2 = 0;
			char[] array = str.ToCharArray();
			for (int i = array.Length; i > 0; i--)
			{
				num2 = num2 * num + (int)array[array.Length - i];
			}
			return num2 & int.MaxValue;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000B52C File Offset: 0x0000972C
		private static void WriteResourceIdToFile(int id, string str, BinaryWriter bw)
		{
			int num = str.Length;
			bw.Write((byte)(id & 255));
			bw.Write((byte)(id >> 8 & 255));
			bw.Write((byte)(id >> 16 & 255));
			bw.Write((byte)(id >> 24 & 255));
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			num = bytes.Length;
			bw.Write((byte)((short)num & 255));
			bw.Write((byte)((short)num >> 8 & 255));
			for (int i = 0; i < num; i++)
			{
				int index = i % GpuTrackManager.lxor;
				bw.Write(bytes[i] ^ (byte)GpuTrackManager.sxor[index]);
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000B5E4 File Offset: 0x000097E4
		public override void StartTrack()
		{
			base.StartTrack();
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000B5EC File Offset: 0x000097EC
		protected override void DoUpdateAtEnd()
		{
			int frameId = SharedUtils.frameId;
			try
			{
				if (GpuTrackManager.AnalysisTrack.AutoUpdate && frameId % 60 == 0 && this.analysisTrack.NeedUpdate())
				{
					VisibleCollector.UpdateVisibleCache();
					this.analysisTrack.Update();
					this.analysisTrack.Output(frameId);
				}
				this.overdrawTrack.LateUpdate();
				if (GpuTrackManager.GpuDumpHelper.UpdateDump() && this.overdrawTrack.IsVisible() && !GpuTrackManager.OverdrawTrack.IsDumping)
				{
					GpuTrackManager.OverdrawTrack.DumpFrameId = frameId;
					GpuTrackManager.OverdrawTrack.IsDumping = true;
					BaseTrackerManager.Dump(8);
					this.overdrawTrack.Update();
				}
				if (GpuTrackManager.OverdrawTrack.IsDumping && this.overdrawTrack.IsVisible())
				{
					this.overdrawTrack.Output(GpuTrackManager.OverdrawTrack.DumpFrameId);
					GpuTrackManager.OverdrawTrack.IsDumping = false;
				}
			}
			catch (IOException ex)
			{
				Debug.Log("AssetTrackManager writeLine failed ! \n" + ex.StackTrace);
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000B6F0 File Offset: 0x000098F0
		public override void StopTrack()
		{
			base.StopTrack();
			this.analysisTrack.Stop();
			this.overdrawTrack.Stop();
		}

		// Token: 0x040000CA RID: 202
		public static GpuTrackManager.Feature GpuModeFeature = GpuTrackManager.Feature.eNone;

		// Token: 0x040000CB RID: 203
		private static string GpuResourcesPath;

		// Token: 0x040000CC RID: 204
		private static string GpuOverdrawPath;

		// Token: 0x040000CD RID: 205
		public static DumpHelper GpuDumpHelper = new DumpHelper
		{
			AutoDump = false
		};

		// Token: 0x040000CE RID: 206
		private GpuTrackManager.AnalysisTrack analysisTrack;

		// Token: 0x040000CF RID: 207
		private GpuTrackManager.OverdrawTrack overdrawTrack;

		// Token: 0x040000D0 RID: 208
		private static string sxor = null;

		// Token: 0x040000D1 RID: 209
		private static int lxor = 0;

		// Token: 0x020000D8 RID: 216
		public enum Feature
		{
			// Token: 0x040005BE RID: 1470
			eNone,
			// Token: 0x040005BF RID: 1471
			eTextureFeatureTrack,
			// Token: 0x040005C0 RID: 1472
			eMeshFeatureTrack,
			// Token: 0x040005C1 RID: 1473
			eAllFeatureTrack
		}

		// Token: 0x020000D9 RID: 217
		private class AnalysisTrack
		{
			// Token: 0x060008B3 RID: 2227 RVA: 0x000396DC File Offset: 0x000378DC
			public AnalysisTrack(GpuTrackManager.Feature feature)
			{
				this._feature = feature;
				this._RenderCameras = new List<Camera>();
				switch (this._feature)
				{
				case GpuTrackManager.Feature.eTextureFeatureTrack:
					this.featureTracks.Add(new GpuTrackManager.TextureTrack("0.ta"));
					return;
				case GpuTrackManager.Feature.eMeshFeatureTrack:
					this.featureTracks.Add(new GpuTrackManager.MeshTrack("0.ma"));
					return;
				case GpuTrackManager.Feature.eAllFeatureTrack:
					this.featureTracks.Add(new GpuTrackManager.TextureTrack("0.ta"));
					this.featureTracks.Add(new GpuTrackManager.MeshTrack("0.ma"));
					return;
				default:
					return;
				}
			}

			// Token: 0x060008B4 RID: 2228 RVA: 0x00039784 File Offset: 0x00037984
			public void Update()
			{
				foreach (GpuTrackManager.FeatureTrack featureTrack in this.featureTracks)
				{
					try
					{
						featureTrack.Update();
					}
					catch
					{
					}
				}
				Camera[] allCameras = Camera.allCameras;
				foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
				{
					if (renderer.enabled && renderer.isVisible && !(renderer.gameObject == null) && renderer.gameObject.activeInHierarchy)
					{
						this._RenderCameras.Clear();
						foreach (Camera camera in allCameras)
						{
							int cullingMask = camera.cullingMask;
							if ((cullingMask & 1 << renderer.gameObject.layer) != 0 || camera.cullingMask == -1)
							{
								this._RenderCameras.Add(camera);
							}
						}
						foreach (GpuTrackManager.FeatureTrack featureTrack2 in this.featureTracks)
						{
							try
							{
								featureTrack2.UpdateFeature(renderer, this._RenderCameras);
							}
							catch
							{
							}
						}
						this._RenderCameras.Clear();
					}
				}
			}

			// Token: 0x060008B5 RID: 2229 RVA: 0x00039940 File Offset: 0x00037B40
			public void Output(int frame)
			{
				foreach (GpuTrackManager.FeatureTrack featureTrack in this.featureTracks)
				{
					featureTrack.Output(frame);
				}
			}

			// Token: 0x060008B6 RID: 2230 RVA: 0x0003999C File Offset: 0x00037B9C
			public void Stop()
			{
				foreach (GpuTrackManager.FeatureTrack featureTrack in this.featureTracks)
				{
					featureTrack.Stop();
				}
			}

			// Token: 0x060008B7 RID: 2231 RVA: 0x000399F4 File Offset: 0x00037BF4
			public bool NeedUpdate()
			{
				return this.featureTracks.Count > 0;
			}

			// Token: 0x040005C2 RID: 1474
			public static bool AutoUpdate = true;

			// Token: 0x040005C3 RID: 1475
			private GpuTrackManager.Feature _feature;

			// Token: 0x040005C4 RID: 1476
			private List<Camera> _RenderCameras;

			// Token: 0x040005C5 RID: 1477
			private List<GpuTrackManager.FeatureTrack> featureTracks = new List<GpuTrackManager.FeatureTrack>();
		}

		// Token: 0x020000DA RID: 218
		private abstract class FeatureTrack
		{
			// Token: 0x060008B9 RID: 2233 RVA: 0x00039A0C File Offset: 0x00037C0C
			public virtual void Update()
			{
			}

			// Token: 0x060008BA RID: 2234 RVA: 0x00039A10 File Offset: 0x00037C10
			public virtual void UpdateFeature(Renderer obj, List<Camera> cameras)
			{
			}

			// Token: 0x060008BB RID: 2235 RVA: 0x00039A14 File Offset: 0x00037C14
			public virtual void Output(int frame)
			{
			}

			// Token: 0x060008BC RID: 2236 RVA: 0x00039A18 File Offset: 0x00037C18
			public virtual void Stop()
			{
			}
		}

		// Token: 0x020000DB RID: 219
		private class TextureTrack : GpuTrackManager.FeatureTrack
		{
			// Token: 0x060008BE RID: 2238 RVA: 0x00039A24 File Offset: 0x00037C24
			public TextureTrack(string fileName)
			{
				this._TextureId = new List<int>();
				this._TextureIdWriter = new BinaryWriter(File.Create(string.Format("{0}/{1}", GpuTrackManager.GpuResourcesPath, "tex_id_map.txt"), 1024));
				this._TextureIdWriter.Write(66);
				this._TextureIdWriter.Write(1);
				this._TextureInfos = new Dictionary<int, GpuTrackManager.TextureTrack.TextureInfo>();
				this._TextureTrackWriter = new TrackWriter<string>(1000);
				this._TextureTrackWriter.LogPath = string.Format("{0}/{1}", GpuTrackManager.GpuResourcesPath, fileName);
			}

			// Token: 0x060008BF RID: 2239 RVA: 0x00039AC0 File Offset: 0x00037CC0
			public override void Update()
			{
				this._TextureInfos.Clear();
				Texture2D[] array = Resources.FindObjectsOfTypeAll<Texture2D>();
				if (array.Length == 0)
				{
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					Texture2D texture2D = array[i];
					if (!(texture2D.name == "OverdrawTexture"))
					{
						int num = GpuTrackManager.GenerateHashCode(texture2D.name + texture2D.width.ToString() + texture2D.height.ToString());
						bool flag = VisibleCollector.IsVisible(texture2D.GetInstanceID(), texture2D.GetType());
						uint num2 = (uint)Profiler.GetRuntimeMemorySizeLong(array[i]);
						GpuTrackManager.TextureTrack.TextureInfo textureInfo;
						if (this._TextureInfos.TryGetValue(num, out textureInfo))
						{
							textureInfo.used = flag;
							textureInfo.nums += 1U;
							textureInfo.totalMemSize += num2;
						}
						else
						{
							this._TextureInfos.Add(num, new GpuTrackManager.TextureTrack.TextureInfo
							{
								totalMemSize = num2,
								used = flag
							});
						}
						if (!this._TextureId.Contains(num))
						{
							this._TextureId.Add(num);
							GpuTrackManager.WriteResourceIdToFile(num, string.Format("{0},{1},{2},{3}", new object[]
							{
								Uri.EscapeDataString(texture2D.name),
								texture2D.width,
								texture2D.height,
								texture2D.mipmapCount
							}), this._TextureIdWriter);
							this._TextureIdWriter.Flush();
						}
					}
				}
			}

			// Token: 0x060008C0 RID: 2240 RVA: 0x00039C54 File Offset: 0x00037E54
			public override void UpdateFeature(Renderer renderer, List<Camera> cameras)
			{
				if (renderer == null)
				{
					return;
				}
				GameObject gameObject = renderer.gameObject;
				if (gameObject == null)
				{
					return;
				}
				Bounds bounds = renderer.bounds;
				Mesh mesh = null;
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component != null)
				{
					mesh = component.sharedMesh;
				}
				else if (component2 != null)
				{
					mesh = component2.sharedMesh;
				}
				if (mesh == null)
				{
					return;
				}
				float num = 0f;
				SharedUtils.MeshUVDMTools.GetMeshUVDM(mesh, 0, ref num);
				if (Mathf.Abs(num - 1f) < 1E-45f)
				{
					return;
				}
				float largestAreaScale = GpuTrackManager.TextureTrack.GetLargestAreaScale(gameObject.transform.lossyScale);
				if (largestAreaScale < 1E-45f)
				{
					return;
				}
				foreach (Camera camera in cameras)
				{
					Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
					if (GeometryUtility.TestPlanesAABB(array, bounds))
					{
						float num2 = 0.017453292f * camera.fieldOfView * 0.5f;
						float num3 = (float)camera.pixelHeight * 0.5f;
						Vector3 position = camera.transform.position;
						float num4 = Mathf.Pow(num3 / Mathf.Tan(num2), 2f);
						if (camera.aspect > 1f)
						{
							num4 *= camera.aspect;
						}
						float cameraOrthographicScreenToWorld = Mathf.Pow((float)camera.pixelHeight * 0.5f / camera.orthographicSize, 2f);
						Material[] sharedMaterials = renderer.sharedMaterials;
						if (sharedMaterials.Length != 0 && sharedMaterials != null)
						{
							List<int> list = new List<int>();
							foreach (Material material in sharedMaterials)
							{
								if (!(material == null))
								{
									SharedUtils.MatPropTools.GetTexPropIds(material, ref list);
									if (list.Count != 0 && list != null)
									{
										foreach (int num5 in list)
										{
											Texture2D texture2D = material.GetTexture(num5) as Texture2D;
											if (!(texture2D == null))
											{
												Vector2 textureScale = material.GetTextureScale(num5);
												int num6 = (int)((float)texture2D.width * Math.Abs(textureScale.x) * (float)texture2D.height * Math.Abs(textureScale.y));
												if (num6 != 0)
												{
													int num7;
													if (camera.orthographic)
													{
														num7 = this.CalculateMipmapLevelOrthographic(num, (float)num6, cameraOrthographicScreenToWorld);
													}
													else
													{
														num7 = this.CalculateMipmapLevelPerspective(bounds, num * largestAreaScale, (float)num6, position, num4);
													}
													int key = GpuTrackManager.GenerateHashCode(texture2D.name + texture2D.width.ToString() + texture2D.height.ToString());
													GpuTrackManager.TextureTrack.TextureInfo textureInfo;
													if (this._TextureInfos.TryGetValue(key, out textureInfo) && textureInfo.mipLevel > num7)
													{
														textureInfo.mipLevel = num7;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x060008C1 RID: 2241 RVA: 0x00039FC0 File Offset: 0x000381C0
			public override void Output(int frame)
			{
				StringBuilder stringBuilder = new StringBuilder("#" + frame.ToString() + "\r\n");
				foreach (KeyValuePair<int, GpuTrackManager.TextureTrack.TextureInfo> keyValuePair in this._TextureInfos)
				{
					if (!keyValuePair.Value.used || keyValuePair.Value.mipLevel == 100)
					{
						stringBuilder.AppendFormat("{0},{1},{2},{3},{4}\r\n", new object[]
						{
							keyValuePair.Key,
							Convert.ToInt16(keyValuePair.Value.used),
							keyValuePair.Value.nums,
							keyValuePair.Value.totalMemSize,
							'*'
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0},{1},{2},{3},{4}\r\n", new object[]
						{
							keyValuePair.Key,
							Convert.ToInt16(keyValuePair.Value.used),
							keyValuePair.Value.nums,
							keyValuePair.Value.totalMemSize,
							(keyValuePair.Value.mipLevel <= -3) ? -3 : keyValuePair.Value.mipLevel
						});
					}
				}
				this._TextureTrackWriter.WriteToBuffer(stringBuilder.ToString(0, stringBuilder.Length - 1));
				this._TextureTrackWriter.BufferToFile();
				this._TextureInfos.Clear();
			}

			// Token: 0x060008C2 RID: 2242 RVA: 0x0003A194 File Offset: 0x00038394
			public override void Stop()
			{
				this._TextureIdWriter.Close();
				this._TextureTrackWriter.LogPath = "";
				this._TextureId.Clear();
			}

			// Token: 0x060008C3 RID: 2243 RVA: 0x0003A1BC File Offset: 0x000383BC
			public static float GetLargestAreaScale(Vector3 v)
			{
				float num = Math.Abs(v.x);
				float num2 = Math.Abs(v.y);
				float num3 = Math.Abs(v.z);
				if (num > num2)
				{
					if (num2 > num3)
					{
						return num * num2;
					}
					return num * num3;
				}
				else
				{
					if (num < num3)
					{
						return num2 * num3;
					}
					return num * num2;
				}
			}

			// Token: 0x060008C4 RID: 2244 RVA: 0x0003A218 File Offset: 0x00038418
			private int CalculateMipmapLevelOrthographic(float uvDistributionMetric, float texelCount, float cameraOrthographicScreenToWorld)
			{
				float num = texelCount / (uvDistributionMetric * cameraOrthographicScreenToWorld);
				float num2 = 0.5f * Mathf.Log(num, 2f);
				return (int)num2;
			}

			// Token: 0x060008C5 RID: 2245 RVA: 0x0003A244 File Offset: 0x00038444
			private int CalculateMipmapLevelPerspective(Bounds bounds, float uvDistributionMetric, float texelCount, Vector3 camerPosition, float cameraEyeToScreenDS)
			{
				float num = bounds.SqrDistance(camerPosition);
				if ((double)num < 1E-06)
				{
					return 0;
				}
				float num2 = texelCount / (uvDistributionMetric * (cameraEyeToScreenDS / num));
				float num3 = 0.5f * Mathf.Log(num2, 2f);
				return (int)num3;
			}

			// Token: 0x040005C6 RID: 1478
			private List<int> _TextureId;

			// Token: 0x040005C7 RID: 1479
			private BinaryWriter _TextureIdWriter;

			// Token: 0x040005C8 RID: 1480
			private TrackWriter<string> _TextureTrackWriter;

			// Token: 0x040005C9 RID: 1481
			private Dictionary<int, GpuTrackManager.TextureTrack.TextureInfo> _TextureInfos;

			// Token: 0x02000136 RID: 310
			private class TextureInfo
			{
				// Token: 0x04000719 RID: 1817
				public uint nums = 1U;

				// Token: 0x0400071A RID: 1818
				public uint totalMemSize;

				// Token: 0x0400071B RID: 1819
				public bool used;

				// Token: 0x0400071C RID: 1820
				public int mipLevel = 100;
			}
		}

		// Token: 0x020000DC RID: 220
		private class MeshTrack : GpuTrackManager.FeatureTrack
		{
			// Token: 0x060008C6 RID: 2246 RVA: 0x0003A290 File Offset: 0x00038490
			public MeshTrack(string fileName)
			{
				this._MeshId = new List<int>();
				this._MeshIdWriter = new BinaryWriter(File.Create(string.Format("{0}/{1}", GpuTrackManager.GpuResourcesPath, "mesh_id_map.txt"), 1024));
				this._MeshIdWriter.Write(66);
				this._MeshIdWriter.Write(1);
				this._MeshInfos = new Dictionary<int, GpuTrackManager.MeshTrack.MeshInfo>();
				this._MeshTrackWriter = new TrackWriter<string>(1000);
				this._MeshTrackWriter.LogPath = string.Format("{0}/{1}", GpuTrackManager.GpuResourcesPath, fileName);
			}

			// Token: 0x060008C7 RID: 2247 RVA: 0x0003A32C File Offset: 0x0003852C
			public override void Update()
			{
				this._MeshInfos.Clear();
				Object[] array = Resources.FindObjectsOfTypeAll(typeof(Mesh));
				if (array.Length == 0)
				{
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					Mesh mesh = array[i] as Mesh;
					int num = GpuTrackManager.GenerateHashCode(mesh.name + mesh.vertexCount.ToString());
					bool flag = VisibleCollector.IsVisible(mesh.GetInstanceID(), mesh.GetType());
					uint num2 = (uint)Profiler.GetRuntimeMemorySizeLong(array[i]);
					GpuTrackManager.MeshTrack.MeshInfo meshInfo;
					if (this._MeshInfos.TryGetValue(num, out meshInfo))
					{
						meshInfo.used = flag;
						meshInfo.nums += 1U;
						meshInfo.totalMemSize += num2;
					}
					else
					{
						this._MeshInfos.Add(num, new GpuTrackManager.MeshTrack.MeshInfo
						{
							totalMemSize = num2,
							used = flag
						});
					}
					if (!this._MeshId.Contains(num))
					{
						this._MeshId.Add(num);
						GpuTrackManager.WriteResourceIdToFile(num, string.Format("{0},{1}", Uri.EscapeDataString(mesh.name), mesh.vertexCount), this._MeshIdWriter);
						this._MeshIdWriter.Flush();
					}
				}
			}

			// Token: 0x060008C8 RID: 2248 RVA: 0x0003A484 File Offset: 0x00038684
			public override void UpdateFeature(Renderer renderer, List<Camera> cameras)
			{
				if (renderer == null)
				{
					return;
				}
				GameObject gameObject = renderer.gameObject;
				if (gameObject == null)
				{
					return;
				}
				Bounds bounds = renderer.bounds;
				string str = "";
				int num = -1;
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component != null)
				{
					if (component.sharedMesh)
					{
						str = component.sharedMesh.name;
						num = component.sharedMesh.vertexCount;
					}
					else if (component.mesh)
					{
						str = component.mesh.name;
						num = component.mesh.vertexCount;
					}
				}
				else if (component2 != null && component2.sharedMesh)
				{
					str = component2.sharedMesh.name;
					num = component2.sharedMesh.vertexCount;
				}
				if (num == -1)
				{
					return;
				}
				int key = GpuTrackManager.GenerateHashCode(str + num.ToString());
				foreach (Camera camera in cameras)
				{
					if (!(camera == null))
					{
						Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
						if (GeometryUtility.TestPlanesAABB(array, bounds))
						{
							float num2 = 0f;
							try
							{
								GpuTrackManager.MeshTrack.BoundsCalculator boundsCalculator = new GpuTrackManager.MeshTrack.BoundsCalculator(bounds, camera);
								boundsCalculator.Calculate();
								num2 = boundsCalculator.Area;
							}
							catch
							{
							}
							GpuTrackManager.MeshTrack.MeshInfo meshInfo;
							if (this._MeshInfos.TryGetValue(key, out meshInfo) && (float)meshInfo.area < num2)
							{
								meshInfo.area = (int)num2;
							}
						}
					}
				}
			}

			// Token: 0x060008C9 RID: 2249 RVA: 0x0003A660 File Offset: 0x00038860
			public override void Output(int frame)
			{
				StringBuilder stringBuilder = new StringBuilder("#" + frame.ToString() + "\r\n");
				foreach (KeyValuePair<int, GpuTrackManager.MeshTrack.MeshInfo> keyValuePair in this._MeshInfos)
				{
					if (keyValuePair.Value.area == 0)
					{
						stringBuilder.AppendFormat("{0},{1},{2},{3},{4}\r\n", new object[]
						{
							keyValuePair.Key,
							Convert.ToInt16(keyValuePair.Value.used),
							keyValuePair.Value.nums,
							keyValuePair.Value.totalMemSize,
							'*'
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0},{1},{2},{3},{4}\r\n", new object[]
						{
							keyValuePair.Key,
							Convert.ToInt16(keyValuePair.Value.used),
							keyValuePair.Value.nums,
							keyValuePair.Value.totalMemSize,
							keyValuePair.Value.area
						});
					}
				}
				this._MeshTrackWriter.WriteToBuffer(stringBuilder.ToString(0, stringBuilder.Length - 1));
				this._MeshTrackWriter.BufferToFile();
				this._MeshInfos.Clear();
			}

			// Token: 0x060008CA RID: 2250 RVA: 0x0003A808 File Offset: 0x00038A08
			public override void Stop()
			{
				this._MeshIdWriter.Close();
				this._MeshTrackWriter.LogPath = "";
				this._MeshId.Clear();
			}

			// Token: 0x040005CA RID: 1482
			private List<int> _MeshId;

			// Token: 0x040005CB RID: 1483
			private BinaryWriter _MeshIdWriter;

			// Token: 0x040005CC RID: 1484
			private Dictionary<int, GpuTrackManager.MeshTrack.MeshInfo> _MeshInfos;

			// Token: 0x040005CD RID: 1485
			private TrackWriter<string> _MeshTrackWriter;

			// Token: 0x02000137 RID: 311
			private class BoundsCalculator
			{
				// Token: 0x170001B3 RID: 435
				// (get) Token: 0x060009BD RID: 2493 RVA: 0x0003EF94 File Offset: 0x0003D194
				public float Area
				{
					get
					{
						return this._area;
					}
				}

				// Token: 0x170001B4 RID: 436
				// (get) Token: 0x060009BE RID: 2494 RVA: 0x0003EF9C File Offset: 0x0003D19C
				public int VertexInScreenCount
				{
					get
					{
						return this._vertexInScreenCount;
					}
				}

				// Token: 0x060009BF RID: 2495 RVA: 0x0003EFA4 File Offset: 0x0003D1A4
				public BoundsCalculator(Bounds bounds, Camera camera)
				{
					this._scWidth = (float)Screen.width;
					this._scHeight = (float)Screen.height;
					this._bounds = bounds;
					this._targetCamera = camera;
					this._haveInScreen = false;
					this._haveOutScreen = false;
					this._isInScreenWhole = false;
					this._isOutScreenWhole = false;
					this._vertexInScreenCount = 0;
					this._area = 0f;
				}

				// Token: 0x060009C0 RID: 2496 RVA: 0x0003F048 File Offset: 0x0003D248
				public void Calculate()
				{
					this.GetVertexSC();
					this.Bounds();
					this.ClipPolygon();
					this.TriangleArea();
				}

				// Token: 0x060009C1 RID: 2497 RVA: 0x0003F074 File Offset: 0x0003D274
				private void GetVertexSC()
				{
					Vector3[] array = new Vector3[8];
					Vector3[] array2 = new Vector3[]
					{
						this._bounds.min,
						this._bounds.max
					};
					for (int i = 0; i < 8; i++)
					{
						int num = i % 2;
						int num2 = i / 2 % 2;
						int num3 = i / 4 % 2;
						array[i] = new Vector3(array2[num3].x, array2[num2].y, array2[num].z);
					}
					for (int j = 0; j < array.Length; j++)
					{
						Vector3 vector = this._targetCamera.WorldToScreenPoint(array[j]);
						if (vector.z > 0f)
						{
							this._vertexSWList.Add(vector);
							if (this.isInScreen(vector))
							{
								this._vertexInScreenCount++;
							}
						}
					}
				}

				// Token: 0x060009C2 RID: 2498 RVA: 0x0003F174 File Offset: 0x0003D374
				private void Bounds()
				{
					List<Vector3> list = new List<Vector3>();
					List<Vector3> list2 = new List<Vector3>();
					list = this._vertexSWList;
					if (list.Count <= 3)
					{
						return;
					}
					int index = 0;
					for (int i = 1; i < list.Count; i++)
					{
						list2.Add(default(Vector3));
						if (list[i].y < list[index].y || (list[i].y == list[index].y && list[i].x < list[i].x))
						{
							index = i;
						}
					}
					list2.Add(default(Vector3));
					this._p0 = new Vector2(list[index].x, list[index].y);
					list.Sort((Vector3 x, Vector3 y) => this.Cmp(x, y));
					int num = -1;
					list2[++num] = list[0];
					list2[++num] = list[1];
					list2[++num] = list[2];
					for (int j = 3; j < list.Count; j++)
					{
						while (num - 1 >= 0 && (this.Cross(list2[num - 1], list[j], list2[num]) > 0f || (this.Cross(list2[num - 1], list[j], list2[num]) == 0f && this.Dis(list2[num - 1], list[j]) > this.Dis(list2[num - 1], list2[num]))))
						{
							num--;
						}
						list2[++num] = list[j];
					}
					int num2 = this.GetPositionCode(list2[0]);
					int num3 = 0;
					List<Vector2> list3 = new List<Vector2>();
					List<Vector2> list4 = new List<Vector2>();
					List<Vector2> list5 = new List<Vector2>();
					List<Vector2> list6 = new List<Vector2>();
					int num4 = 1;
					for (int k = 0; k <= num; k++)
					{
						int index2 = (k + 1) % (num + 1);
						this._hullVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(list2[k], false, false));
						if (this.isInScreen(list2[k]))
						{
							this._haveInScreen = true;
						}
						else
						{
							this._haveOutScreen = true;
						}
						num3 = this.GetPositionCode(list2[index2]);
						if ((num2 | num3) != 0 && (num2 & num3) == 0)
						{
							List<Vector2> list7 = this.CalculateIntersection(list2[k], list2[index2]);
							foreach (Vector2 vector in list7)
							{
								if (!this._vertexIndexDic.ContainsKey(vector))
								{
									GpuTrackManager.MeshTrack.BoundsCalculator.Vertex item = new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(vector, true, false);
									this._hullVertexList.Add(item);
									if (this.isInScreen(vector))
									{
										this._haveInScreen = true;
									}
									else
									{
										this._haveOutScreen = true;
									}
									this._vertexIndexDic.Add(vector, new GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex(num4, -1));
									if ((double)vector.y == 0.0)
									{
										list3.Add(vector);
									}
									if (vector.x == this._scWidth - 1f)
									{
										list4.Add(vector);
									}
									if (vector.y == this._scHeight - 1f)
									{
										list5.Add(vector);
									}
									if ((double)vector.x == 0.0)
									{
										list6.Add(vector);
									}
									num4++;
								}
							}
						}
						num2 = num3;
						num4++;
					}
					this._isInScreenWhole = (this._haveInScreen & !this._haveOutScreen);
					this._isOutScreenWhole = (!this._haveInScreen & this._haveOutScreen);
					list3.Sort(delegate(Vector2 a, Vector2 b)
					{
						if (a.x <= b.x)
						{
							return -1;
						}
						return 1;
					});
					list4.Sort(delegate(Vector2 a, Vector2 b)
					{
						if (a.y <= b.y)
						{
							return -1;
						}
						return 1;
					});
					list5.Sort(delegate(Vector2 a, Vector2 b)
					{
						if (a.x >= b.x)
						{
							return -1;
						}
						return 1;
					});
					list6.Sort(delegate(Vector2 a, Vector2 b)
					{
						if (a.y >= b.y)
						{
							return -1;
						}
						return 1;
					});
					num4 = 0;
					GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex vertexIndex = new GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex(-1, -1);
					this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(new Vector2(0f, 0f), false, false));
					foreach (Vector2 vector2 in list3)
					{
						num4++;
						this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(vector2, true, false));
						this._vertexIndexDic.TryGetValue(vector2, out vertexIndex);
						vertexIndex.sIndex = num4;
					}
					num4++;
					this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(new Vector2(this._scWidth - 1f, 0f), false, false));
					foreach (Vector2 vector3 in list4)
					{
						num4++;
						this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(vector3, true, false));
						this._vertexIndexDic.TryGetValue(vector3, out vertexIndex);
						vertexIndex.sIndex = num4;
					}
					num4++;
					this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(new Vector2(this._scWidth - 1f, this._scHeight - 1f), false, false));
					foreach (Vector2 vector4 in list5)
					{
						num4++;
						this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(vector4, true, false));
						this._vertexIndexDic.TryGetValue(vector4, out vertexIndex);
						vertexIndex.sIndex = num4;
					}
					num4++;
					this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(new Vector2(0f, this._scHeight - 1f), false, false));
					foreach (Vector2 vector5 in list6)
					{
						num4++;
						this._screenEdgeVertexList.Add(new GpuTrackManager.MeshTrack.BoundsCalculator.Vertex(vector5, true, false));
						this._vertexIndexDic.TryGetValue(vector5, out vertexIndex);
						vertexIndex.sIndex = num4;
					}
				}

				// Token: 0x060009C3 RID: 2499 RVA: 0x0003F954 File Offset: 0x0003DB54
				private int Cmp(Vector3 x, Vector3 y)
				{
					Vector2 b;
					b..ctor(x.x, x.y);
					Vector2 vector;
					vector..ctor(y.x, y.y);
					float num = this.Cross(this._p0, b, vector);
					if (num > 0f || (0f == num && this.Dis(this._p0, b) - this.Dis(this._p0, vector) < 0.0))
					{
						return -1;
					}
					return 1;
				}

				// Token: 0x060009C4 RID: 2500 RVA: 0x0003F9DC File Offset: 0x0003DBDC
				private float Cross(Vector2 _a, Vector2 _b, Vector2 _c)
				{
					return (_b.x - _a.x) * (_c.y - _a.y) - (_c.x - _a.x) * (_b.y - _a.y);
				}

				// Token: 0x060009C5 RID: 2501 RVA: 0x0003FA18 File Offset: 0x0003DC18
				private double Dis(Vector2 a, Vector2 b)
				{
					return (double)Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
				}

				// Token: 0x060009C6 RID: 2502 RVA: 0x0003FA58 File Offset: 0x0003DC58
				private int GetPositionCode(Vector2 a)
				{
					int num = 0;
					if (a.x < 0f)
					{
						num += 8;
					}
					if (a.x > this._scWidth - 1f)
					{
						num += 4;
					}
					if (a.y < 0f)
					{
						num++;
					}
					if (a.y > this._scHeight - 1f)
					{
						num += 2;
					}
					return num;
				}

				// Token: 0x060009C7 RID: 2503 RVA: 0x0003FACC File Offset: 0x0003DCCC
				private List<Vector2> CalculateIntersection(Vector2 a, Vector2 b)
				{
					bool flag = a.x < b.x;
					Vector2 vector = flag ? a : b;
					Vector2 vector2 = flag ? b : a;
					Vector2 vector3 = a - b;
					List<Vector2> list = new List<Vector2>();
					float num = (a.y - b.y) / (a.x - b.x);
					float num2 = (a.x - b.x) / (a.y - b.y);
					list.Add(new Vector2(0f, (0f - a.x) * num + a.y));
					list.Add(new Vector2(this._scWidth - 1f, (this._scWidth - 1f - a.x) * num + a.y));
					list.Add(new Vector2((0f - a.y) * num2 + a.x, 0f));
					list.Add(new Vector2((this._scHeight - 1f - a.y) * num2 + a.x, this._scHeight - 1f));
					list.Add(a);
					list.Add(b);
					list.Sort((Vector2 x, Vector2 y) => this.AscendByX(x, y));
					Vector2 vector4 = this.MaxX(list[0], list[1], list[2]);
					Vector2 vector5 = this.MinX(list[3], list[4], list[5]);
					List<Vector2> list2 = new List<Vector2>();
					if (vector4 != vector && this.isInScreen(vector4))
					{
						list2.Add(vector4);
					}
					if (vector5 != vector2 && this.isInScreen(vector5))
					{
						list2.Add(vector5);
					}
					if (!flag)
					{
						list2.Reverse();
					}
					return list2;
				}

				// Token: 0x060009C8 RID: 2504 RVA: 0x0003FCC8 File Offset: 0x0003DEC8
				private int AscendByX(Vector2 a, Vector2 b)
				{
					if (a.x > b.x)
					{
						return 1;
					}
					return -1;
				}

				// Token: 0x060009C9 RID: 2505 RVA: 0x0003FCE0 File Offset: 0x0003DEE0
				private Vector2 MaxX(Vector2 a, Vector2 b, Vector2 c)
				{
					Vector2 result = a;
					if (b.x > a.x)
					{
						result = b;
					}
					if (c.x > b.x)
					{
						result = c;
					}
					return result;
				}

				// Token: 0x060009CA RID: 2506 RVA: 0x0003FD1C File Offset: 0x0003DF1C
				private Vector2 MinX(Vector2 a, Vector2 b, Vector2 c)
				{
					Vector2 result = a;
					if (b.x < a.x)
					{
						result = b;
					}
					if (c.x < b.x)
					{
						result = c;
					}
					return result;
				}

				// Token: 0x060009CB RID: 2507 RVA: 0x0003FD58 File Offset: 0x0003DF58
				private bool isInScreen(Vector2 a)
				{
					return a.x >= 0f && a.x <= this._scWidth - 1f && a.y >= 0f && a.y <= this._scHeight - 1f;
				}

				// Token: 0x060009CC RID: 2508 RVA: 0x0003FDBC File Offset: 0x0003DFBC
				private bool IsRayIntersectsSegment(Vector2 point, Vector2 head, Vector2 tail)
				{
					if (head.y == tail.y)
					{
						return false;
					}
					if (head.y > point.y && tail.y > point.y)
					{
						return false;
					}
					if (head.y < point.y && tail.y < point.y)
					{
						return false;
					}
					if (head.y == point.y && tail.y > point.y)
					{
						return false;
					}
					if (tail.y == point.y && head.y > point.y)
					{
						return false;
					}
					if (head.x < point.x && tail.x < point.x)
					{
						return false;
					}
					float num = head.x - (head.x - tail.x) * (head.y - point.y) / (head.y - tail.y);
					return num >= point.x;
				}

				// Token: 0x060009CD RID: 2509 RVA: 0x0003FED4 File Offset: 0x0003E0D4
				private void ClipPolygon()
				{
					if (this._isInScreenWhole)
					{
						foreach (GpuTrackManager.MeshTrack.BoundsCalculator.Vertex vertex in this._hullVertexList)
						{
							this._finalVertexList.Add(vertex.position);
						}
						return;
					}
					if (this._isOutScreenWhole)
					{
						bool flag = false;
						bool flag2 = false;
						bool flag3 = false;
						bool flag4 = false;
						for (int i = 0; i < this._hullVertexList.Count; i++)
						{
							int index = (i + 1) % this._hullVertexList.Count;
							Vector2 position = this._hullVertexList[i].position;
							Vector2 position2 = this._hullVertexList[index].position;
							flag |= this.IsRayIntersectsSegment(new Vector2(0f, 0f), position, position2);
							flag2 |= this.IsRayIntersectsSegment(new Vector2(this._scWidth - 1f, 0f), position, position2);
							flag3 |= this.IsRayIntersectsSegment(new Vector2(this._scWidth - 1f, this._scHeight - 1f), position, position2);
							flag4 |= this.IsRayIntersectsSegment(new Vector2(0f, this._scHeight - 1f), position, position2);
						}
						if (flag && flag2 && flag3 && flag4)
						{
							this._finalVertexList.Add(new Vector2(0f, 0f));
							this._finalVertexList.Add(new Vector2(this._scWidth - 1f, 0f));
							this._finalVertexList.Add(new Vector2(this._scWidth - 1f, this._scHeight - 1f));
							this._finalVertexList.Add(new Vector2(0f, this._scHeight - 1f));
							return;
						}
						if (!flag && !flag2 && !flag3 && !flag4)
						{
							return;
						}
					}
					if (this._hullVertexList.Count < 3)
					{
						return;
					}
					int num = 0;
					int num2 = 0;
					bool flag5 = true;
					bool flag6 = false;
					GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex vertexIndex = new GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex(-1, -1);
					while (!flag6 && num < this._hullVertexList.Count && !this._hullVertexList[num].isVisited)
					{
						this._hullVertexList[num].isVisited = true;
						if (this.isInScreen(this._hullVertexList[num].position))
						{
							this._vertexIndexDic.TryGetValue(this._hullVertexList[num].position, out vertexIndex);
							this._hullVertexList[num].isVisited = true;
							if (vertexIndex != null && vertexIndex.sIndex >= 0 && vertexIndex.sIndex < this._screenEdgeVertexList.Count)
							{
								this._screenEdgeVertexList[vertexIndex.sIndex].isVisited = true;
							}
							this._finalVertexList.Add(this._hullVertexList[num].position);
							flag6 = true;
						}
						num++;
					}
					for (;;)
					{
						if (flag5)
						{
							num %= this._hullVertexList.Count;
							if (this._hullVertexList[num].isVisited)
							{
								break;
							}
							this._hullVertexList[num].isVisited = true;
							if (this.isInScreen(this._hullVertexList[num].position))
							{
								this._finalVertexList.Add(this._hullVertexList[num].position);
								if (this._hullVertexList[num].isIntersection)
								{
									this._vertexIndexDic.TryGetValue(this._hullVertexList[num].position, out vertexIndex);
									if (vertexIndex != null && vertexIndex.sIndex >= 0 && vertexIndex.sIndex < this._screenEdgeVertexList.Count)
									{
										this._screenEdgeVertexList[vertexIndex.sIndex].isVisited = true;
									}
									flag5 = false;
									num2 = vertexIndex.sIndex + 1;
								}
							}
							num++;
						}
						else
						{
							num2 %= this._screenEdgeVertexList.Count;
							if (this._screenEdgeVertexList[num2].isVisited)
							{
								break;
							}
							this._screenEdgeVertexList[num2].isVisited = true;
							if (this.isInScreen(this._screenEdgeVertexList[num2].position))
							{
								this._finalVertexList.Add(this._screenEdgeVertexList[num2].position);
								if (this._screenEdgeVertexList[num2].isIntersection)
								{
									this._vertexIndexDic.TryGetValue(this._screenEdgeVertexList[num2].position, out vertexIndex);
									if (vertexIndex != null && vertexIndex.cIndex >= 0 && vertexIndex.cIndex < this._hullVertexList.Count)
									{
										this._hullVertexList[vertexIndex.cIndex].isVisited = true;
									}
									flag5 = true;
									num = vertexIndex.cIndex + 1;
								}
							}
							num2++;
						}
					}
				}

				// Token: 0x060009CE RID: 2510 RVA: 0x00040430 File Offset: 0x0003E630
				private void DebugDrawLine(Vector3 start, Vector3 end, Camera camera, Color color)
				{
					Vector3 vector = camera.ScreenToWorldPoint(new Vector3(start.x, start.y, -camera.transform.position.z));
					Vector3 vector2 = camera.ScreenToWorldPoint(new Vector3(end.x, end.y, -camera.transform.position.z));
					Debug.DrawLine(vector, vector2, color, 0.1f);
				}

				// Token: 0x060009CF RID: 2511 RVA: 0x000404A4 File Offset: 0x0003E6A4
				private float TriangleArea()
				{
					if (this._finalVertexList.Count < 2)
					{
						return -1f;
					}
					float num = 0f;
					Vector2 vector = this._finalVertexList[0];
					Vector2 vector2 = this._finalVertexList[1];
					for (int i = 2; i < this._finalVertexList.Count; i++)
					{
						Vector2 vector3 = this._finalVertexList[i];
						float num2 = vector.x * vector2.y + vector2.x * vector3.y + vector3.x * vector.y - vector.x * vector3.y - vector2.x * vector.y - vector3.x * vector2.y;
						vector2 = vector3;
						num += num2;
					}
					this._area = num / 2f;
					return num / 2f;
				}

				// Token: 0x0400071D RID: 1821
				private float _scWidth;

				// Token: 0x0400071E RID: 1822
				private float _scHeight;

				// Token: 0x0400071F RID: 1823
				private List<Vector3> _vertexSWList = new List<Vector3>();

				// Token: 0x04000720 RID: 1824
				private List<GpuTrackManager.MeshTrack.BoundsCalculator.Vertex> _hullVertexList = new List<GpuTrackManager.MeshTrack.BoundsCalculator.Vertex>();

				// Token: 0x04000721 RID: 1825
				private List<GpuTrackManager.MeshTrack.BoundsCalculator.Vertex> _screenEdgeVertexList = new List<GpuTrackManager.MeshTrack.BoundsCalculator.Vertex>();

				// Token: 0x04000722 RID: 1826
				private Dictionary<Vector2, GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex> _vertexIndexDic = new Dictionary<Vector2, GpuTrackManager.MeshTrack.BoundsCalculator.VertexIndex>();

				// Token: 0x04000723 RID: 1827
				private Bounds _bounds;

				// Token: 0x04000724 RID: 1828
				private Vector2 _p0;

				// Token: 0x04000725 RID: 1829
				private List<Vector2> _finalVertexList = new List<Vector2>();

				// Token: 0x04000726 RID: 1830
				private bool _haveInScreen;

				// Token: 0x04000727 RID: 1831
				private bool _haveOutScreen;

				// Token: 0x04000728 RID: 1832
				private bool _isInScreenWhole;

				// Token: 0x04000729 RID: 1833
				private bool _isOutScreenWhole;

				// Token: 0x0400072A RID: 1834
				private Camera _targetCamera;

				// Token: 0x0400072B RID: 1835
				private float _area;

				// Token: 0x0400072C RID: 1836
				private int _vertexInScreenCount;

				// Token: 0x0200013C RID: 316
				private enum PositionCode
				{
					// Token: 0x0400074B RID: 1867
					Left = 8,
					// Token: 0x0400074C RID: 1868
					Right = 4,
					// Token: 0x0400074D RID: 1869
					Top = 2,
					// Token: 0x0400074E RID: 1870
					Bottom = 1
				}

				// Token: 0x0200013D RID: 317
				private class Vertex
				{
					// Token: 0x060009DE RID: 2526 RVA: 0x00040B20 File Offset: 0x0003ED20
					public Vertex(Vector2 position, bool isIntersection = false, bool isVisited = false)
					{
						this.position = position;
						this.isIntersection = isIntersection;
						this.isVisited = isVisited;
					}

					// Token: 0x0400074F RID: 1871
					public Vector2 position;

					// Token: 0x04000750 RID: 1872
					public bool isIntersection;

					// Token: 0x04000751 RID: 1873
					public bool isVisited;
				}

				// Token: 0x0200013E RID: 318
				private class VertexIndex
				{
					// Token: 0x060009DF RID: 2527 RVA: 0x00040B40 File Offset: 0x0003ED40
					public VertexIndex(int cIndex = -1, int sIndex = -1)
					{
						this.cIndex = cIndex;
						this.sIndex = sIndex;
					}

					// Token: 0x04000752 RID: 1874
					public int cIndex;

					// Token: 0x04000753 RID: 1875
					public int sIndex;
				}
			}

			// Token: 0x02000138 RID: 312
			private class MeshInfo
			{
				// Token: 0x0400072D RID: 1837
				public uint nums = 1U;

				// Token: 0x0400072E RID: 1838
				public uint totalMemSize;

				// Token: 0x0400072F RID: 1839
				public bool used;

				// Token: 0x04000730 RID: 1840
				public int area;
			}
		}

		// Token: 0x020000DD RID: 221
		private class OverdrawTrack
		{
			// Token: 0x060008CB RID: 2251 RVA: 0x0003A830 File Offset: 0x00038A30
			public OverdrawTrack()
			{
				this._CameraIdMapWriter.LogPath = string.Format("{0}/{1}", GpuTrackManager.GpuOverdrawPath, "cam_id_map.txt");
				this._CameraOverdrawDump.LogPath = string.Format("{0}/{1}", GpuTrackManager.GpuOverdrawPath, "0.od");
				this.LoadShaders();
				this.CreateProxyCamera();
				this.CreateCanvasCamera();
				this.RefreshMaterialCache();
			}

			// Token: 0x060008CC RID: 2252 RVA: 0x0003A980 File Offset: 0x00038B80
			private void Canvas_willRenderCanvases()
			{
				if (GpuTrackManager.OverdrawTrack.ReplaceMat)
				{
					Canvas.GetDefaultCanvasMaterial().shader = this._OverdrawTransparentShader;
				}
			}

			// Token: 0x060008CD RID: 2253 RVA: 0x0003A99C File Offset: 0x00038B9C
			public void Update()
			{
				Camera[] allCameras = Camera.allCameras;
				if (allCameras == null || allCameras.Length == 0)
				{
					return;
				}
				if (GpuTrackManager.OverdrawTrack.CurrentUICanvases == null)
				{
					GpuTrackManager.OverdrawTrack.CurrentUICanvases = Object.FindObjectsOfType<Canvas>();
				}
				GpuTrackManager.OverdrawTrack._CameraCanvases.Clear();
				GpuTrackManager.OverdrawTrack._OverlayCanvases.Clear();
				this.CanvasSpaceCameraCheck();
				Array.Sort<Camera>(allCameras, delegate(Camera wp1, Camera wp2)
				{
					if (wp1 == wp2)
					{
						return 0;
					}
					int num = wp1.depth.CompareTo(wp2.depth);
					if (num != 0)
					{
						return num;
					}
					return wp1.clearFlags.CompareTo(wp2.clearFlags);
				});
				if (GpuTrackManager.OverdrawTrack.ReplaceMat)
				{
					this.OnBeforeRenderRT();
				}
				foreach (Camera camera in allCameras)
				{
					GpuTrackManager.OverdrawTrack._CurrentCameraId = GpuTrackManager.GenerateHashCode(camera.name);
					if (this._CameraDumps.ContainsKey(GpuTrackManager.OverdrawTrack._CurrentCameraId))
					{
						Dictionary<int, int> cameraDumps = this._CameraDumps;
						int currentCameraId = GpuTrackManager.OverdrawTrack._CurrentCameraId;
						cameraDumps[currentCameraId]++;
					}
					else
					{
						this._CameraDumps.Add(GpuTrackManager.OverdrawTrack._CurrentCameraId, 1);
					}
					string text = GpuTrackManager.OverdrawTrack._CurrentCameraId.ToString() + "." + this._CameraDumps[GpuTrackManager.OverdrawTrack._CurrentCameraId].ToString();
					int key = GpuTrackManager.GenerateHashCode(text);
					if (!this._RenderTextureWriters.ContainsKey(key))
					{
						this._RenderTextureWriters.Add(key, new GpuTrackManager.OverdrawTrack.RenderTextureWriter(text, 0.5f));
					}
					this._CurrentRenderTextureWrite = this._RenderTextureWriters[key];
					if (!GpuTrackManager.OverdrawTrack.KeepTrackForDebug)
					{
						this._CurrentRenderTextureWrite.Update();
					}
					this.UpdateRT(camera);
				}
				if (GpuTrackManager.OverdrawTrack.RenderCanvasOverlay)
				{
					GpuTrackManager.OverdrawTrack._CurrentCameraId = GpuTrackManager.GenerateHashCode(GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.name);
					if (this._CameraDumps.ContainsKey(GpuTrackManager.OverdrawTrack._CurrentCameraId))
					{
						Dictionary<int, int> cameraDumps = this._CameraDumps;
						int currentCameraId = GpuTrackManager.OverdrawTrack._CurrentCameraId;
						cameraDumps[currentCameraId]++;
					}
					else
					{
						this._CameraDumps.Add(GpuTrackManager.OverdrawTrack._CurrentCameraId, 1);
					}
					string text2 = GpuTrackManager.OverdrawTrack._CurrentCameraId.ToString() + "." + this._CameraDumps[GpuTrackManager.OverdrawTrack._CurrentCameraId].ToString();
					int key2 = GpuTrackManager.GenerateHashCode(text2);
					if (!this._RenderTextureWriters.ContainsKey(key2))
					{
						this._RenderTextureWriters.Add(key2, new GpuTrackManager.OverdrawTrack.RenderTextureWriter(text2, 0.5f));
					}
					this._CurrentRenderTextureWrite = this._RenderTextureWriters[key2];
					if (!GpuTrackManager.OverdrawTrack.KeepTrackForDebug)
					{
						this._CurrentRenderTextureWrite.Update();
					}
					this.UpdateRT(GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera);
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.cullingMask = 0;
					for (int j = 0; j < GpuTrackManager.OverdrawTrack._OverlayCanvases.Count; j++)
					{
						Canvas canvas = GpuTrackManager.OverdrawTrack._OverlayCanvases[j];
						GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.transform.position = new Vector3(canvas.transform.position.x, canvas.transform.position.y, GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.transform.position.z);
						GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.cullingMask |= this.GetCullingMaskForOverlayCanvas(canvas);
						canvas.renderMode = 1;
						canvas.worldCamera = GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera;
					}
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.enabled = true;
					this.DoRenderRTForCamera(GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera);
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.enabled = false;
					for (int k = 0; k < GpuTrackManager.OverdrawTrack._OverlayCanvases.Count; k++)
					{
						Canvas canvas2 = GpuTrackManager.OverdrawTrack._OverlayCanvases[k];
						canvas2.renderMode = 0;
						canvas2.worldCamera = null;
					}
				}
				if (GpuTrackManager.OverdrawTrack.ReplaceMat)
				{
					this.OnAfterRenderRT();
				}
				GpuTrackManager.OverdrawTrack.CurrentUICanvases = null;
				GpuTrackManager.OverdrawTrack._CameraCanvases.Clear();
				GpuTrackManager.OverdrawTrack._OverlayCanvases.Clear();
			}

			// Token: 0x060008CE RID: 2254 RVA: 0x0003AD6C File Offset: 0x00038F6C
			private int GetCullingMaskForOverlayCanvas(Canvas canvas)
			{
				if (!GpuTrackManager.OverdrawTrack.IsRenderCanvas(canvas))
				{
					return 0;
				}
				Canvas[] componentsInChildren = canvas.GetComponentsInChildren<Canvas>();
				int num = 1 << canvas.gameObject.layer;
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled && componentsInChildren[i].gameObject.activeInHierarchy)
					{
						num |= 1 << componentsInChildren[i].gameObject.layer;
					}
				}
				return num;
			}

			// Token: 0x060008CF RID: 2255 RVA: 0x0003ADF4 File Offset: 0x00038FF4
			public void Output(int frameId)
			{
				StringBuilder stringBuilder = new StringBuilder("#" + frameId.ToString() + "\r\n");
				foreach (KeyValuePair<int, int> keyValuePair in this._CameraDumps)
				{
					stringBuilder.AppendFormat("{0},{1}\r\n", keyValuePair.Key, keyValuePair.Value);
				}
				this._CameraOverdrawDump.WriteToBuffer(stringBuilder.ToString(0, stringBuilder.Length - 1));
				this._CameraOverdrawDump.BufferToFile();
				this._CameraDumps.Clear();
			}

			// Token: 0x060008D0 RID: 2256 RVA: 0x0003AEB8 File Offset: 0x000390B8
			public void LateUpdate()
			{
				if (this._RenderTextureWriters.Count <= 0)
				{
					return;
				}
				foreach (KeyValuePair<int, GpuTrackManager.OverdrawTrack.RenderTextureWriter> keyValuePair in this._RenderTextureWriters)
				{
					keyValuePair.Value.LateUpdate();
				}
			}

			// Token: 0x060008D1 RID: 2257 RVA: 0x0003AF28 File Offset: 0x00039128
			public bool IsVisible()
			{
				if (this._RenderTextureWriters.Count <= 0)
				{
					return true;
				}
				bool flag = true;
				foreach (KeyValuePair<int, GpuTrackManager.OverdrawTrack.RenderTextureWriter> keyValuePair in this._RenderTextureWriters)
				{
					flag &= keyValuePair.Value.IsFinished();
				}
				return flag;
			}

			// Token: 0x060008D2 RID: 2258 RVA: 0x0003AFA0 File Offset: 0x000391A0
			public void Stop()
			{
				this._CameraIdMapWriter.LogPath = "";
				this._CameraOverdrawDump.LogPath = "";
			}

			// Token: 0x060008D3 RID: 2259 RVA: 0x0003AFC4 File Offset: 0x000391C4
			private void CreateProxyCamera()
			{
				if (GpuTrackManager.OverdrawTrack.CurrentRenderCamera == null)
				{
					GameObject gameObject = GameObject.Find("UWA-RenderingProxyCam");
					if (gameObject == null)
					{
						gameObject = new GameObject("UWA-RenderingProxyCam");
					}
					Object.DontDestroyOnLoad(gameObject);
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera = gameObject.GetComponent<Camera>();
					if (GpuTrackManager.OverdrawTrack.CurrentRenderCamera == null)
					{
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera = gameObject.AddComponent<Camera>();
					}
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = false;
					gameObject.SetActive(false);
				}
			}

			// Token: 0x060008D4 RID: 2260 RVA: 0x0003B048 File Offset: 0x00039248
			private void CreateCanvasCamera()
			{
				if (GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera == null)
				{
					GameObject gameObject = GameObject.Find("Overlay-UI(UWA)");
					if (gameObject == null)
					{
						gameObject = new GameObject("Overlay-UI(UWA)");
					}
					Object.DontDestroyOnLoad(gameObject);
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera = gameObject.GetComponent<Camera>();
					if (GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera == null)
					{
						GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera = gameObject.AddComponent<Camera>();
					}
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.clearFlags = 3;
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.orthographic = true;
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.orthographicSize = 0.5f * (float)Screen.height;
					GpuTrackManager.OverdrawTrack.CurrentUIRenderCamera.enabled = false;
					gameObject.SetActive(false);
				}
			}

			// Token: 0x060008D5 RID: 2261 RVA: 0x0003B0F8 File Offset: 0x000392F8
			private bool IsBuiltinHiddenMat(string name)
			{
				if (!name.StartsWith("Hidden/"))
				{
					return false;
				}
				for (int i = 0; i < this.BuiltinHiddenMats.Length; i++)
				{
					if (name.Contains(this.BuiltinHiddenMats[i]))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060008D6 RID: 2262 RVA: 0x0003B14C File Offset: 0x0003934C
			private void RefreshMaterialCache()
			{
				if (GpuTrackManager.OverdrawTrack._InternalMaterial == null)
				{
					GpuTrackManager.OverdrawTrack._InternalMaterial = new HashSet<int>();
				}
				if (GpuTrackManager.OverdrawTrack._NormalMaterial == null)
				{
					GpuTrackManager.OverdrawTrack._NormalMaterial = new HashSet<int>();
				}
				GpuTrackManager.OverdrawTrack._InternalMaterial.Clear();
				GpuTrackManager.OverdrawTrack._NormalMaterial.Clear();
				Material[] array = Resources.FindObjectsOfTypeAll<Material>();
				for (int i = 0; i < array.Length; i++)
				{
					if (!(array[i] == null))
					{
						string name = array[i].name;
						if (this.IsBuiltinHiddenMat(name))
						{
							GpuTrackManager.OverdrawTrack._InternalMaterial.Add(array[i].GetInstanceID());
						}
						else
						{
							GpuTrackManager.OverdrawTrack._NormalMaterial.Add(array[i].GetInstanceID());
						}
					}
				}
			}

			// Token: 0x060008D7 RID: 2263 RVA: 0x0003B210 File Offset: 0x00039410
			private void OnBeforeRenderRT()
			{
				Projector[] array = Object.FindObjectsOfType<Projector>();
				Terrain[] array2 = Object.FindObjectsOfType<Terrain>();
				Material[] array3 = Resources.FindObjectsOfTypeAll<Material>();
				this._OrgProjectorList.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].enabled)
					{
						this._OrgProjectorList.Add(array[i]);
					}
				}
				this._OrgTerrainList.Clear();
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].enabled)
					{
						this._OrgTerrainList.Add(array2[j]);
					}
				}
				this._ReplacedOrgMaterialList.Clear();
				if (GpuTrackManager.OverdrawTrack.LogSkipMat)
				{
					SharedUtils.Log("Mat Skip @ " + Time.frameCount.ToString() + "**************************************************");
				}
				int num = 0;
				foreach (Material material in array3)
				{
					if (!(material.shader == null) && !this._FakedSet.Contains(material))
					{
						if (GpuTrackManager.OverdrawTrack.SkipInternalMat && GpuTrackManager.OverdrawTrack._InternalMaterial.Contains(material.GetInstanceID()))
						{
							if (GpuTrackManager.OverdrawTrack.LogSkipMat)
							{
								SharedUtils.Log("Interal Mat Skip:" + material.name);
							}
						}
						else
						{
							if (GpuTrackManager.OverdrawTrack.SkipInternalMat && !GpuTrackManager.OverdrawTrack._NormalMaterial.Contains(material.GetInstanceID()))
							{
								string name = material.name;
								if (this.IsBuiltinHiddenMat(name))
								{
									GpuTrackManager.OverdrawTrack._InternalMaterial.Add(material.GetInstanceID());
									if (GpuTrackManager.OverdrawTrack.LogSkipMat)
									{
										SharedUtils.Log("Internal Mat Skip:" + name);
										goto IL_2F0;
									}
									goto IL_2F0;
								}
								else
								{
									GpuTrackManager.OverdrawTrack._NormalMaterial.Add(material.GetInstanceID());
								}
							}
							string tag = material.GetTag("RenderType", false);
							int num2 = (material.renderQueue > 0) ? material.renderQueue : material.shader.renderQueue;
							if (num >= this._FakedMaterialHolderList.Count)
							{
								Material item = new Material(material)
								{
									name = "UWA-Temp-Mat"
								};
								this._FakedMaterialHolderList.Add(item);
								this._FakedSet.Add(item);
							}
							this._FakedMaterialHolderList[num].shader = material.shader;
							this._FakedMaterialHolderList[num].CopyPropertiesFromMaterial(material);
							num++;
							this._ReplacedOrgMaterialList.Add(material);
							bool? flag = null;
							if (tag.Length > 1)
							{
								if (tag.IndexOf("Opaque") != -1)
								{
									flag = new bool?(true);
								}
								if (tag.IndexOf("Transparent") != -1)
								{
									flag = new bool?(false);
								}
							}
							if (flag == null)
							{
								flag = new bool?(num2 < 2500);
							}
							this.ProcessMaterial(material, flag.Value);
						}
					}
					IL_2F0:;
				}
				for (int l = 0; l < this._OrgProjectorList.Count; l++)
				{
					Projector projector = this._OrgProjectorList[l];
					if (projector != null)
					{
						projector.enabled = false;
					}
				}
				for (int m = 0; m < this._OrgTerrainList.Count; m++)
				{
					Terrain terrain = this._OrgTerrainList[m];
					if (terrain != null)
					{
						terrain.enabled = false;
					}
				}
			}

			// Token: 0x060008D8 RID: 2264 RVA: 0x0003B5AC File Offset: 0x000397AC
			private void DoRenderRTForCamera(Camera camera)
			{
				RenderTexture targetTexture = camera.targetTexture;
				Rect pixelRect = camera.pixelRect;
				Color backgroundColor = camera.backgroundColor;
				CameraClearFlags clearFlags = camera.clearFlags;
				camera.targetTexture = this._CurrentRenderTextureWrite.TargetRT;
				camera.backgroundColor = Color.black;
				camera.clearFlags = 3;
				Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.Canvas_willRenderCanvases);
				camera.Render();
				Canvas.willRenderCanvases -= new Canvas.WillRenderCanvases(this.Canvas_willRenderCanvases);
				camera.backgroundColor = backgroundColor;
				camera.clearFlags = clearFlags;
				camera.targetTexture = targetTexture;
				camera.pixelRect = pixelRect;
			}

			// Token: 0x060008D9 RID: 2265 RVA: 0x0003B640 File Offset: 0x00039840
			private void OnAfterRenderRT()
			{
				for (int i = 0; i < this._ReplacedOrgMaterialList.Count; i++)
				{
					if (!(this._ReplacedOrgMaterialList[i] == null))
					{
						this._ReplacedOrgMaterialList[i].shader = this._FakedMaterialHolderList[i].shader;
						this._ReplacedOrgMaterialList[i].CopyPropertiesFromMaterial(this._FakedMaterialHolderList[i]);
						this._FakedMaterialHolderList[i].shader = null;
					}
				}
				this._ReplacedOrgMaterialList.Clear();
				for (int j = 0; j < this._OrgProjectorList.Count; j++)
				{
					Projector projector = this._OrgProjectorList[j];
					if (projector != null)
					{
						projector.enabled = true;
					}
				}
				this._OrgProjectorList.Clear();
				for (int k = 0; k < this._OrgTerrainList.Count; k++)
				{
					Terrain terrain = this._OrgTerrainList[k];
					if (terrain != null)
					{
						terrain.enabled = true;
					}
				}
				this._OrgTerrainList.Clear();
			}

			// Token: 0x060008DA RID: 2266 RVA: 0x0003B768 File Offset: 0x00039968
			private void UpdateRT(Camera camera)
			{
				this.ClearWithColor(Color.black);
				if (!GpuTrackManager.OverdrawTrack._CameraIdMap.Contains(GpuTrackManager.OverdrawTrack._CurrentCameraId))
				{
					GpuTrackManager.OverdrawTrack._CameraIdMap.Add(GpuTrackManager.OverdrawTrack._CurrentCameraId);
					this._CameraIdMapStr.AppendFormat("{0},{1}\r", GpuTrackManager.OverdrawTrack._CurrentCameraId, Uri.EscapeDataString(camera.name));
					this._CameraIdMapWriter.WriteLineToFile(this._CameraIdMapStr.ToString());
					this._CameraIdMapStr.Length = 0;
				}
				if (camera.enabled && camera.gameObject.activeInHierarchy)
				{
					bool flag = GpuTrackManager.OverdrawTrack._CameraCanvases.ContainsKey(camera);
					if (flag && GpuTrackManager.OverdrawTrack.RenderCanvasBindedCam)
					{
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = true;
						if (camera.transform.parent != null)
						{
							GpuTrackManager.OverdrawTrack.CurrentRenderCamera.transform.parent = camera.transform.parent;
						}
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.CopyFrom(camera);
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.renderingPath = 1;
						foreach (Canvas canvas in GpuTrackManager.OverdrawTrack._CameraCanvases[camera])
						{
							if (canvas != null && canvas.enabled && canvas.gameObject.activeInHierarchy)
							{
								canvas.worldCamera = GpuTrackManager.OverdrawTrack.CurrentRenderCamera;
							}
						}
						this.DoRenderRTForCamera(GpuTrackManager.OverdrawTrack.CurrentRenderCamera);
						foreach (Canvas canvas2 in GpuTrackManager.OverdrawTrack._CameraCanvases[camera])
						{
							if (canvas2 != null)
							{
								canvas2.worldCamera = camera;
							}
						}
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.transform.parent = null;
						Object.DontDestroyOnLoad(GpuTrackManager.OverdrawTrack.CurrentRenderCamera);
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = false;
					}
					if (!flag)
					{
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = true;
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.CopyFrom(camera);
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.renderingPath = 1;
						this.DoRenderRTForCamera(GpuTrackManager.OverdrawTrack.CurrentRenderCamera);
						GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = false;
					}
				}
			}

			// Token: 0x060008DB RID: 2267 RVA: 0x0003B9B8 File Offset: 0x00039BB8
			public static bool IsRenderCanvas(Canvas c)
			{
				return c.isRootCanvas && c.enabled && c.gameObject.activeInHierarchy;
			}

			// Token: 0x060008DC RID: 2268 RVA: 0x0003B9E0 File Offset: 0x00039BE0
			private void CanvasSpaceCameraCheck()
			{
				foreach (Canvas canvas in GpuTrackManager.OverdrawTrack.CurrentUICanvases)
				{
					if (canvas != null && GpuTrackManager.OverdrawTrack.IsRenderCanvas(canvas) && canvas.renderMode == 1 && canvas.worldCamera != null)
					{
						if (!GpuTrackManager.OverdrawTrack._CameraCanvases.ContainsKey(canvas.worldCamera))
						{
							GpuTrackManager.OverdrawTrack._CameraCanvases.Add(canvas.worldCamera, new List<Canvas>());
						}
						GpuTrackManager.OverdrawTrack._CameraCanvases[canvas.worldCamera].Add(canvas);
					}
					if (canvas != null && GpuTrackManager.OverdrawTrack.IsRenderCanvas(canvas) && (canvas.renderMode == null || (canvas.renderMode == 1 && canvas.worldCamera == null)))
					{
						GpuTrackManager.OverdrawTrack._OverlayCanvases.Add(canvas);
					}
				}
			}

			// Token: 0x060008DD RID: 2269 RVA: 0x0003BAD4 File Offset: 0x00039CD4
			private void ProcessMaterial(Material m, bool opaque)
			{
				if (GpuTrackManager.OverdrawTrack.ReplaceOpaqueMat && opaque)
				{
					m.shader = this._OverdrawOpaqueShader;
				}
				if (GpuTrackManager.OverdrawTrack.ReplaceTransparentMat && !opaque)
				{
					m.shader = this._OverdrawTransparentShader;
				}
			}

			// Token: 0x060008DE RID: 2270 RVA: 0x0003BB0C File Offset: 0x00039D0C
			private void ClearWithColor(Color color)
			{
				try
				{
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = true;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.clearFlags = 2;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.backgroundColor = color;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.targetTexture = this._CurrentRenderTextureWrite.TargetRT;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.cullingMask = 0;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.Render();
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.targetTexture = null;
					GpuTrackManager.OverdrawTrack.CurrentRenderCamera.enabled = false;
				}
				catch (Exception ex)
				{
					string str = "ClearWithColor ";
					Exception ex2 = ex;
					SharedUtils.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
				}
			}

			// Token: 0x060008DF RID: 2271 RVA: 0x0003BBB8 File Offset: 0x00039DB8
			private void LoadShaders()
			{
				Shader shader = Shader.Find("UWA/overdraw_opaque");
				if (shader != null)
				{
					this._OverdrawOpaqueShader = shader;
				}
				else
				{
					SharedUtils.Log("_OpaqueShader Shader == null");
				}
				Shader shader2 = Shader.Find("UWA/overdraw_transparent");
				if (shader2 != null)
				{
					this._OverdrawTransparentShader = shader2;
					return;
				}
				SharedUtils.Log("transparent == null");
			}

			// Token: 0x1700017D RID: 381
			// (get) Token: 0x060008E0 RID: 2272 RVA: 0x0003BC20 File Offset: 0x00039E20
			public bool IsShaderReady
			{
				get
				{
					return this._OverdrawOpaqueShader != null && this._OverdrawTransparentShader != null;
				}
			}

			// Token: 0x1700017E RID: 382
			// (get) Token: 0x060008E1 RID: 2273 RVA: 0x0003BC44 File Offset: 0x00039E44
			public Dictionary<int, GpuTrackManager.OverdrawTrack.RenderTextureWriter> RTs
			{
				get
				{
					return this._RenderTextureWriters;
				}
			}

			// Token: 0x040005CE RID: 1486
			public static int DumpFrameId = 0;

			// Token: 0x040005CF RID: 1487
			public static bool IsDumping = false;

			// Token: 0x040005D0 RID: 1488
			public static bool KeepTrackForDebug = false;

			// Token: 0x040005D1 RID: 1489
			public static bool ReplaceMat = true;

			// Token: 0x040005D2 RID: 1490
			public static bool ReplaceOpaqueMat = true;

			// Token: 0x040005D3 RID: 1491
			public static bool ReplaceTransparentMat = true;

			// Token: 0x040005D4 RID: 1492
			public static bool RenderCanvasBindedCam = true;

			// Token: 0x040005D5 RID: 1493
			public static bool RenderCanvasOverlay = true;

			// Token: 0x040005D6 RID: 1494
			public static bool SkipInternalMat = true;

			// Token: 0x040005D7 RID: 1495
			public static bool LogSkipMat = false;

			// Token: 0x040005D8 RID: 1496
			private string[] BuiltinHiddenMats = new string[]
			{
				"Video",
				"Universal",
				"Blit",
				"Copy",
				"Clear",
				"Blur",
				"Depth",
				"Lut",
				"PostProcess",
				"Bloom",
				"Uber",
				"Error"
			};

			// Token: 0x040005D9 RID: 1497
			private Dictionary<int, int> _CameraDumps = new Dictionary<int, int>();

			// Token: 0x040005DA RID: 1498
			private Dictionary<int, GpuTrackManager.OverdrawTrack.RenderTextureWriter> _RenderTextureWriters = new Dictionary<int, GpuTrackManager.OverdrawTrack.RenderTextureWriter>();

			// Token: 0x040005DB RID: 1499
			private GpuTrackManager.OverdrawTrack.RenderTextureWriter _CurrentRenderTextureWrite;

			// Token: 0x040005DC RID: 1500
			private Shader _OverdrawOpaqueShader;

			// Token: 0x040005DD RID: 1501
			private Shader _OverdrawTransparentShader;

			// Token: 0x040005DE RID: 1502
			private readonly List<Material> _ReplacedOrgMaterialList = new List<Material>();

			// Token: 0x040005DF RID: 1503
			private readonly List<Material> _FakedMaterialHolderList = new List<Material>();

			// Token: 0x040005E0 RID: 1504
			private readonly HashSet<Material> _FakedSet = new HashSet<Material>();

			// Token: 0x040005E1 RID: 1505
			private readonly List<Projector> _OrgProjectorList = new List<Projector>();

			// Token: 0x040005E2 RID: 1506
			private readonly List<Terrain> _OrgTerrainList = new List<Terrain>();

			// Token: 0x040005E3 RID: 1507
			private static HashSet<int> _InternalMaterial;

			// Token: 0x040005E4 RID: 1508
			private static HashSet<int> _NormalMaterial;

			// Token: 0x040005E5 RID: 1509
			private static Camera CurrentRenderCamera = null;

			// Token: 0x040005E6 RID: 1510
			private static Camera CurrentUIRenderCamera = null;

			// Token: 0x040005E7 RID: 1511
			private static Canvas[] CurrentUICanvases = null;

			// Token: 0x040005E8 RID: 1512
			private static readonly Dictionary<Camera, List<Canvas>> _CameraCanvases = new Dictionary<Camera, List<Canvas>>();

			// Token: 0x040005E9 RID: 1513
			private static readonly List<Canvas> _OverlayCanvases = new List<Canvas>();

			// Token: 0x040005EA RID: 1514
			private static int _CurrentCameraId = 0;

			// Token: 0x040005EB RID: 1515
			private static List<int> _CameraIdMap = new List<int>();

			// Token: 0x040005EC RID: 1516
			private StringBuilder _CameraIdMapStr = new StringBuilder();

			// Token: 0x040005ED RID: 1517
			private TrackWriter<string> _CameraIdMapWriter = new TrackWriter<string>(100);

			// Token: 0x040005EE RID: 1518
			private TrackWriter<string> _CameraOverdrawDump = new TrackWriter<string>(100);

			// Token: 0x02000139 RID: 313
			internal class RenderTextureWriter
			{
				// Token: 0x060009D3 RID: 2515 RVA: 0x000405B4 File Offset: 0x0003E7B4
				public RenderTextureWriter(string cameraID = "", float scale = 0.5f)
				{
					if (SystemInfo.graphicsDeviceVersion.Contains("Vulkan"))
					{
						this._NeedsFlip = true;
					}
					TextureFormat textureFormat = 3;
					RenderTextureFormat renderTextureFormat = 0;
					this._rtWidth = Screen.width;
					this._rtHeight = Screen.height;
					bool flag = this._rtWidth > this._rtHeight;
					if (flag && this._rtWidth > 5000)
					{
						this._rtWidth = 5000;
						this._rtHeight = 5000 * this._rtHeight / this._rtWidth;
					}
					if (!flag && this._rtHeight > 5000)
					{
						this._rtHeight = 5000;
						this._rtWidth = 5000 * this._rtWidth / this._rtHeight;
					}
					this._rtWidth = (int)((float)this._rtWidth * scale);
					this._rtHeight = (int)((float)this._rtHeight * scale);
					this._sizeBlockX = Mathf.CeilToInt((float)this._rtWidth * 1f / (float)this._numBlockX);
					this._sizeBlockY = Mathf.CeilToInt((float)this._rtHeight * 1f / (float)this._numBlockY);
					this._numBlockX = Mathf.CeilToInt((float)this._rtWidth * 1f / (float)this._sizeBlockX);
					this._numBlockY = Mathf.CeilToInt((float)this._rtHeight * 1f / (float)this._sizeBlockY);
					this._TargetTexture = new Texture2D(this._sizeBlockX, this._sizeBlockY, textureFormat, false, true);
					this._TargetRT = new RenderTexture(this._rtWidth, this._rtHeight, 24, renderTextureFormat, 1);
					this._TargetRT.Create();
					this._TargetTexture.name = "OverdrawTexture";
					this._TargetTexture.hideFlags = 61;
					this._TargetRT.hideFlags = 61;
					this._ColorBuffer = new Color32[this._rtWidth * this._rtHeight];
					this.jPGEncoder = new JPGEncoder(this._ColorBuffer, this._rtWidth, this._rtHeight, 50f);
					this._CameraID = cameraID;
				}

				// Token: 0x060009D4 RID: 2516 RVA: 0x000407F0 File Offset: 0x0003E9F0
				public void Update()
				{
					if (!this._TargetRT.IsCreated())
					{
						this._TargetRT.Create();
						return;
					}
					if (this._EncodeTexFinished)
					{
						this._EncodeTexFinished = false;
					}
				}

				// Token: 0x060009D5 RID: 2517 RVA: 0x00040824 File Offset: 0x0003EA24
				public void LateUpdate()
				{
					if (this._EncodeTexFinished)
					{
						return;
					}
					bool flag = false;
					Rect nextPatch = this.GetNextPatch(out flag);
					RenderTexture.active = this._TargetRT;
					Rect rect = nextPatch;
					if (this._NeedsFlip)
					{
						rect.y = (float)this._rtHeight - rect.y - (float)this._sizeBlockY;
					}
					this._TargetTexture.ReadPixels(rect, 0, 0, false);
					RenderTexture.active = null;
					int num = 0;
					while ((float)num < nextPatch.width)
					{
						int num2 = 0;
						while ((float)num2 < nextPatch.height)
						{
							int num3 = (int)nextPatch.x + num;
							int num4 = (int)nextPatch.y + num2;
							this._ColorBuffer[num4 * this._rtWidth + num3] = this._TargetTexture.GetPixel(num, num2);
							num2++;
						}
						num++;
					}
					if (flag)
					{
						string path = this.GetOutPutLocation();
						this._EncodeThread = new Thread(delegate()
						{
							if (path != null)
							{
								this.jPGEncoder.ResetData(this._ColorBuffer, this._rtWidth, this._rtHeight);
								this.jPGEncoder.doEncoding();
								this.jPGEncoder.WriteTo(path);
							}
						});
						this._EncodeThread.Start();
						this.ClearState();
					}
				}

				// Token: 0x060009D6 RID: 2518 RVA: 0x00040958 File Offset: 0x0003EB58
				public bool IsFinished()
				{
					return this._EncodeTexFinished;
				}

				// Token: 0x060009D7 RID: 2519 RVA: 0x00040960 File Offset: 0x0003EB60
				private string GetOutPutLocation()
				{
					return string.Concat(new string[]
					{
						GpuTrackManager.GpuOverdrawPath,
						"/",
						GpuTrackManager.OverdrawTrack.DumpFrameId.ToString(),
						".",
						this._CameraID.ToString(),
						".jpg"
					});
				}

				// Token: 0x060009D8 RID: 2520 RVA: 0x000409B8 File Offset: 0x0003EBB8
				private void ClearState()
				{
					this._CurrentBlockId = 0;
					this._EncodeTexFinished = true;
				}

				// Token: 0x060009D9 RID: 2521 RVA: 0x000409C8 File Offset: 0x0003EBC8
				private Rect GetNextPatch(out bool last)
				{
					last = false;
					int num = this._sizeBlockX;
					int num2 = this._sizeBlockY;
					int num3 = this._CurrentBlockId % this._numBlockX;
					int num4 = this._CurrentBlockId / this._numBlockX;
					int num5 = num3 * this._sizeBlockX;
					int num6 = num4 * this._sizeBlockY;
					if ((num3 + 1) % this._numBlockX == 0)
					{
						num = this._TargetRT.width - (this._numBlockX - 1) * this._sizeBlockX;
					}
					if ((num4 + 1) % this._numBlockX == 0)
					{
						num2 = this._TargetRT.height - (this._numBlockY - 1) * this._sizeBlockY;
					}
					this._CurrentBlockId++;
					if (this._CurrentBlockId == this._numBlockX * this._numBlockY)
					{
						last = true;
					}
					return new Rect((float)num5, (float)num6, (float)num, (float)num2);
				}

				// Token: 0x170001B5 RID: 437
				// (get) Token: 0x060009DA RID: 2522 RVA: 0x00040AA8 File Offset: 0x0003ECA8
				public RenderTexture TargetRT
				{
					get
					{
						return this._TargetRT;
					}
				}

				// Token: 0x04000731 RID: 1841
				private string _CameraID = "";

				// Token: 0x04000732 RID: 1842
				private int _CurrentBlockId;

				// Token: 0x04000733 RID: 1843
				private bool _EncodeTexFinished = true;

				// Token: 0x04000734 RID: 1844
				private Thread _EncodeThread;

				// Token: 0x04000735 RID: 1845
				private readonly bool _NeedsFlip;

				// Token: 0x04000736 RID: 1846
				private readonly int _numBlockX = 4;

				// Token: 0x04000737 RID: 1847
				private readonly int _numBlockY = 4;

				// Token: 0x04000738 RID: 1848
				private readonly int _sizeBlockX;

				// Token: 0x04000739 RID: 1849
				private readonly int _sizeBlockY;

				// Token: 0x0400073A RID: 1850
				private readonly int _rtWidth;

				// Token: 0x0400073B RID: 1851
				private readonly int _rtHeight;

				// Token: 0x0400073C RID: 1852
				private const int MaxScreenLength = 5000;

				// Token: 0x0400073D RID: 1853
				private JPGEncoder jPGEncoder;

				// Token: 0x0400073E RID: 1854
				private readonly Texture2D _TargetTexture;

				// Token: 0x0400073F RID: 1855
				private readonly RenderTexture _TargetRT;

				// Token: 0x04000740 RID: 1856
				private readonly Color32[] _ColorBuffer;
			}
		}
	}
}
