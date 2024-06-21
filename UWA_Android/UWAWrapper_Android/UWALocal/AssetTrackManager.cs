using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UWA;

namespace UWALocal
{
	// Token: 0x0200001D RID: 29
	internal class AssetTrackManager : BaseTrackerManager
	{
		// Token: 0x0600019E RID: 414 RVA: 0x0000A138 File Offset: 0x00008338
		internal void MakeNextGetAllAssets()
		{
			this._waitalltypes = true;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000A144 File Offset: 0x00008344
		internal AssetTrackManager(AssetTrackManager.Mode mode, string extension) : base(extension, 200)
		{
			this.m_mode = mode;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000A2B0 File Offset: 0x000084B0
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			AssetTrackManager._tasks.Clear();
			if (this.m_mode == AssetTrackManager.Mode.simple)
			{
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AnimationClip), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AudioClip), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Font), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Material), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Mesh), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(MeshCollider), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(ParticleSystem), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(RenderTexture), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Shader), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Texture2D), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AssetBundle), 1000, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(TextAsset), 1000, 1));
			}
			else
			{
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AnimationClip), 60, 5));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AudioClip), 60, 10));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Font), 60, 10));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Material), 60, 1));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Mesh), 60, 5));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(MeshCollider), 60, 5));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(ParticleSystem), 60, 3));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(RenderTexture), 60, 5));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Shader), 60, 10));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(Texture2D), 60, 5));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(AssetBundle), 60, 3));
				AssetTrackManager._tasks.Add(new AssetTrackManager.AssetTrackTask(typeof(TextAsset), 60, 5));
			}
			this.m_trackWriterAssets = new BinaryWriter(File.Create(string.Format("{0}/{1}", SharedUtils.FinalDataPath, "asset_map.txt"), 1024));
			this.m_trackWriterAssets.Write(66);
			this.m_trackWriterAssets.Write(1);
			this.sxor = UwaLocalState.Date;
			if (this.sxor != null)
			{
				this.lxor = this.sxor.Length;
			}
			for (int i = 0; i < this.m_TypeNames.Length; i++)
			{
				this.m_dictType2Idx[this.m_TypeNames[i]] = i;
				if (i != 0)
				{
					this.currentMid++;
				}
				this.WriteIdPairToFile(this.currentMid, string.Format("{0},{1},,,,,,,,,,,,,,,,,", this.m_TypeNames[i], this.m_TypeNames[i]), this.m_trackWriterAssets);
			}
			this.m_trackWriterAssets.Flush();
			this.m_listAssetsOfType.Clear();
			for (int j = 0; j < this.m_TypeNames.Length; j++)
			{
				this.m_listAssetsOfType.Add(new AssetTrackManager.FrameTypeCapture(j));
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000A6C0 File Offset: 0x000088C0
		private void WriteIdPairToFile(int id, string str, BinaryWriter bw)
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
				int index = i % this.lxor;
				bw.Write(bytes[i] ^ (byte)this.sxor[index]);
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000A778 File Offset: 0x00008978
		protected override void DoUpdateAtEnd()
		{
			this.m_currentFrame = SharedUtils.frameId;
			try
			{
				if (AssetTrackManager.AssetDumpHelper.UpdateDump())
				{
					BaseTrackerManager.Dump(4);
					this.WriteInfoOfAssetsPassCompleteMode(true);
					if (this.m_currentFrame % 60 == 0 || this.m_currentFrame % 1000 == 0)
					{
						return;
					}
				}
				if (this.m_mode == AssetTrackManager.Mode.simple)
				{
					if (this._waitalltypes || this.m_currentFrame % 1000 == 0)
					{
						this.WriteInfoOfAssetsPassSimpleMode(this._waitalltypes);
						if (this._waitalltypes)
						{
							this._waitalltypes = false;
						}
					}
				}
				else if (this._waitalltypes)
				{
					BaseTrackerManager.Dump(4);
					this.WriteInfoOfAssetsPassCompleteMode(this._waitalltypes);
					if (this._waitalltypes)
					{
						this._waitalltypes = false;
					}
				}
				else if (this.m_currentFrame % 60 == 0)
				{
					this.WriteInfoOfAssetsPassCompleteMode(this._waitalltypes);
				}
			}
			catch (IOException ex)
			{
				Debug.Log("AssetTrackManager writeLine failed ! \n" + ex.StackTrace);
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000A898 File Offset: 0x00008A98
		public override void StartTrack()
		{
			base.StartTrack();
			if (this.m_mode == AssetTrackManager.Mode.simple)
			{
				this._waitalltypes = true;
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000A8B4 File Offset: 0x00008AB4
		public override void StopTrack()
		{
			if (this.m_mode == AssetTrackManager.Mode.simple)
			{
				this.WriteInfoOfAssetsPassSimpleMode(true);
			}
			base.StopTrack();
			this.m_trackWriterAssets.Close();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A8DC File Offset: 0x00008ADC
		private void WriteInfoOfAssetsPassCompleteMode(bool forceall)
		{
			for (int i = 0; i < AssetTrackManager._tasks.Count; i++)
			{
				AssetTrackManager.AssetTrackTask task = AssetTrackManager._tasks[i];
				this.WriteInfoOfAssetTypeCompleteMode(task, forceall);
			}
			if (this.needUpdate)
			{
				this.WriteInfoOfAssetsPass2();
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000A92C File Offset: 0x00008B2C
		private void WriteInfoOfAssetTypeCompleteMode(AssetTrackManager.AssetTrackTask task, bool forceall)
		{
			if (!forceall && this.m_currentFrame % task.frameInternal != 0)
			{
				return;
			}
			Object[] array = Resources.FindObjectsOfTypeAll(task.type);
			int index = this.m_dictType2Idx[task.type.Name];
			AssetTrackManager.FrameTypeCapture frameTypeCapture = this.m_listAssetsOfType[index];
			frameTypeCapture.Reset();
			frameTypeCapture.totalCount = array.Length;
			this.needUpdate = true;
			if (array.Length == 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)Profiler.GetRuntimeMemorySizeLong(array[i]);
				string text = array[i].name;
				int num2 = 0;
				if (task.type != typeof(RenderTexture) || ((RenderTexture)array[i]).IsCreated())
				{
					if (task.type == typeof(ParticleSystem))
					{
						try
						{
							int num3 = 5;
							num2 = Convert.ToInt32(((ParticleSystem)array[i]).isPlaying);
							Transform parent = ((ParticleSystem)array[i]).transform.parent;
							while (num3 > 0 && parent != null)
							{
								text = parent.name + "/" + text;
								parent = parent.parent;
								num3--;
							}
						}
						catch (Exception)
						{
						}
					}
					text = CoreUtils.StringReplace(text);
					int num4 = AssetTrackManager.GetHashCode(text + task.type.Name) + num / 10000;
					if (!this.hashCode2Mid.ContainsKey(num4))
					{
						if (!this.PerfStats.ContainsKey(task.type))
						{
							this.PerfStats.Add(task.type, 0);
						}
						Dictionary<Type, int> perfStats = this.PerfStats;
						Type type = task.type;
						int num5 = perfStats[type];
						perfStats[type] = num5 + 1;
						string addedInfo4Asset = this.GetAddedInfo4Asset(task.type, array[i], text);
						Dictionary<int, int> dictionary = this.hashCode2Mid;
						int key = num4;
						num5 = this.currentMid + 1;
						this.currentMid = num5;
						dictionary[key] = num5;
						this.WriteIdPairToFile(this.currentMid, string.Format("{0},{1},{2},{3},{4}{5}", new object[]
						{
							text,
							task.type.Name,
							num,
							num4,
							array[i].hideFlags,
							addedInfo4Asset
						}), this.m_trackWriterAssets);
					}
					if (!frameTypeCapture.hashCode2AssetInfo.ContainsKey(num4))
					{
						frameTypeCapture.hashCode2AssetInfo.Add(num4, new AssetTrackManager.AssetInfo(num4, 0, 0, 0));
					}
					AssetTrackManager.AssetInfo assetInfo = frameTypeCapture.hashCode2AssetInfo[num4];
					assetInfo.elseinfo += num2;
					assetInfo.dupCount++;
					assetInfo.totalSize += num;
				}
			}
			task.trackCurrent = 0f;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000AC34 File Offset: 0x00008E34
		private static int GetHashCode(string str)
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

		// Token: 0x060001A8 RID: 424 RVA: 0x0000AC78 File Offset: 0x00008E78
		private string GetAddedInfo4Asset(Type t, Object obj, string name)
		{
			string result = "";
			if (t == typeof(Texture2D))
			{
				Texture2D texture2D = obj as Texture2D;
				if (this.isReadableInfoTX2D == null)
				{
					result = string.Format(",{0},{1},{2},{3},-1", new object[]
					{
						texture2D.height,
						texture2D.width,
						texture2D.format,
						texture2D.mipmapCount
					});
				}
				else
				{
					result = string.Format(",{0},{1},{2},{3},{4}", new object[]
					{
						texture2D.height,
						texture2D.width,
						texture2D.format,
						texture2D.mipmapCount,
						this.isReadableInfoTX2D.GetValue(texture2D, null)
					});
				}
			}
			else if (t == typeof(RenderTexture))
			{
				RenderTexture renderTexture = obj as RenderTexture;
				result = string.Format(",{0},{1},{2},{3},{4}", new object[]
				{
					renderTexture.height,
					renderTexture.width,
					renderTexture.format,
					renderTexture.antiAliasing,
					renderTexture.depth
				});
			}
			else if (t == typeof(Mesh))
			{
				Mesh mesh = obj as Mesh;
				if (mesh.isReadable)
				{
					result = ((mesh.vertexCount == 0) ? ",0,0,0,0,0,0,True" : string.Format(",{0},{1},{2},{3},{4},{5},True", new object[]
					{
						mesh.vertexCount,
						mesh.triangles.Length / 3,
						mesh.normals.Length,
						mesh.colors.Length,
						mesh.tangents.Length,
						mesh.boneWeights.Length
					}));
				}
				else
				{
					result = string.Format(",{0},-1,-1,-1,-1,-1,False", mesh.vertexCount);
				}
			}
			else if (t == typeof(Shader))
			{
				Shader shader = obj as Shader;
				result = string.Format(",{0}", shader.isSupported);
			}
			else if (t == typeof(AnimationClip))
			{
				AnimationClip animationClip = obj as AnimationClip;
				result = string.Format(",{0:#.00},{1}", animationClip.length, animationClip.frameRate);
			}
			else if (t == typeof(AudioClip))
			{
				AudioClip audioClip = obj as AudioClip;
				result = string.Format(",{0},{1},{2}", audioClip.length, audioClip.samples, audioClip.loadType);
			}
			else if (t == typeof(ParticleSystem))
			{
				ParticleSystem particleSystem = obj as ParticleSystem;
				result = string.Format(",{0},{1}", particleSystem.particleCount, particleSystem.isPlaying);
			}
			else if (t == typeof(Font))
			{
				Font font = obj as Font;
				result = string.Format(",{0},{1}", font.fontSize, font.dynamic);
			}
			else if (t == typeof(Material))
			{
				Material material = obj as Material;
				result = string.Format(",{0}", material.shader.name);
			}
			return result;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000B008 File Offset: 0x00009208
		private void WriteInfoOfAssetsPass2()
		{
			StringBuilder stringBuilder = new StringBuilder("#" + this.m_currentFrame.ToString() + "\r\n");
			for (int i = 0; i < this.m_listAssetsOfType.Count; i++)
			{
				AssetTrackManager.FrameTypeCapture frameTypeCapture = this.m_listAssetsOfType[i];
				if (frameTypeCapture.totalCount > 0)
				{
					uint num = 0U;
					foreach (KeyValuePair<int, AssetTrackManager.AssetInfo> keyValuePair in frameTypeCapture.hashCode2AssetInfo)
					{
						AssetTrackManager.AssetInfo value = keyValuePair.Value;
						num += (uint)value.totalSize;
					}
					stringBuilder.AppendFormat("1,{0},{1},{2},{3}\r\n", new object[]
					{
						frameTypeCapture.typeIdx,
						0,
						frameTypeCapture.totalCount,
						num
					});
				}
				foreach (KeyValuePair<int, AssetTrackManager.AssetInfo> keyValuePair2 in frameTypeCapture.hashCode2AssetInfo)
				{
					AssetTrackManager.AssetInfo value2 = keyValuePair2.Value;
					if (frameTypeCapture.typeIdx == 7)
					{
						stringBuilder.AppendFormat("2,{0},{1},{2},{3},{4}\r\n", new object[]
						{
							this.hashCode2Mid[value2.hashCode],
							0,
							value2.dupCount,
							value2.totalSize,
							value2.elseinfo
						});
					}
					else
					{
						stringBuilder.AppendFormat("2,{0},{1},{2},{3}\r\n", new object[]
						{
							this.hashCode2Mid[value2.hashCode],
							0,
							value2.dupCount,
							value2.totalSize
						});
					}
				}
			}
			base.TrackWriter.WriteToBuffer(stringBuilder.ToString(0, stringBuilder.Length - 1));
			base.TrackWriter.BufferToFile();
			this.needUpdate = false;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000B248 File Offset: 0x00009448
		private void WriteInfoOfAssetsPassSimpleMode(bool forceall)
		{
			for (int i = 0; i < AssetTrackManager._tasks.Count; i++)
			{
				AssetTrackManager.AssetTrackTask task = AssetTrackManager._tasks[i];
				this.CollectInfoOfAssetType(task, forceall);
			}
			if (this.needUpdate)
			{
				this.WriteInfoOfAssetTypeSimpleMode();
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000B298 File Offset: 0x00009498
		private void CollectInfoOfAssetType(AssetTrackManager.AssetTrackTask task, bool forceall)
		{
			if (!forceall && SharedUtils.frameId % task.frameInternal != 0)
			{
				return;
			}
			Object[] array = Resources.FindObjectsOfTypeAll(task.type);
			long num = 0L;
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (task.type != typeof(RenderTexture) || ((RenderTexture)array[i]).IsCreated())
				{
					num += Profiler.GetRuntimeMemorySizeLong(array[i]);
					num2++;
				}
			}
			task.AssetCount = num2;
			task.AssetMemory = num;
			this.needUpdate = true;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000B338 File Offset: 0x00009538
		private void WriteInfoOfAssetTypeSimpleMode()
		{
			base.TrackWriter.WriteToBuffer("#" + SharedUtils.frameId.ToString());
			for (int i = 0; i < AssetTrackManager._tasks.Count; i++)
			{
				AssetTrackManager.AssetTrackTask assetTrackTask = AssetTrackManager._tasks[i];
				if (assetTrackTask.AssetCount != -1)
				{
					base.TrackWriter.WriteToBuffer(string.Concat(new string[]
					{
						"1,",
						this.m_dictType2Idx[assetTrackTask.type.Name].ToString(),
						",0,",
						assetTrackTask.AssetCount.ToString(),
						",",
						assetTrackTask.AssetMemory.ToString()
					}));
					assetTrackTask.AssetCount = -1;
					assetTrackTask.AssetMemory = -1L;
				}
			}
			base.TrackWriter.BufferToFile();
			this.needUpdate = false;
		}

		// Token: 0x040000B8 RID: 184
		public static DumpHelper AssetDumpHelper = new DumpHelper
		{
			AutoDump = false
		};

		// Token: 0x040000B9 RID: 185
		private AssetTrackManager.Mode m_mode;

		// Token: 0x040000BA RID: 186
		private bool _waitalltypes;

		// Token: 0x040000BB RID: 187
		private bool needUpdate;

		// Token: 0x040000BC RID: 188
		private BinaryWriter m_trackWriterAssets;

		// Token: 0x040000BD RID: 189
		private Dictionary<int, int> hashCode2Mid = new Dictionary<int, int>();

		// Token: 0x040000BE RID: 190
		private Dictionary<string, int> m_dictType2Idx = new Dictionary<string, int>();

		// Token: 0x040000BF RID: 191
		private int currentMid;

		// Token: 0x040000C0 RID: 192
		private const int m_baseFrameIntervalInCompleteMode = 60;

		// Token: 0x040000C1 RID: 193
		private const int m_baseFrameIntervalInSimpleMode = 1000;

		// Token: 0x040000C2 RID: 194
		public Dictionary<Type, int> PerfStats = new Dictionary<Type, int>();

		// Token: 0x040000C3 RID: 195
		private int m_currentFrame;

		// Token: 0x040000C4 RID: 196
		private List<AssetTrackManager.FrameTypeCapture> m_listAssetsOfType = new List<AssetTrackManager.FrameTypeCapture>();

		// Token: 0x040000C5 RID: 197
		private string[] m_TypeNames = new string[]
		{
			"Root",
			typeof(AnimationClip).Name,
			typeof(AudioClip).Name,
			typeof(Font).Name,
			typeof(Material).Name,
			typeof(Mesh).Name,
			typeof(MeshCollider).Name,
			typeof(ParticleSystem).Name,
			typeof(RenderTexture).Name,
			typeof(Shader).Name,
			typeof(Texture2D).Name,
			typeof(AssetBundle).Name,
			typeof(TextAsset).Name
		};

		// Token: 0x040000C6 RID: 198
		private PropertyInfo isReadableInfoTX2D = Assembly.GetAssembly(typeof(Texture2D)).GetType("UnityEngine.Texture2D").GetProperty("isReadable");

		// Token: 0x040000C7 RID: 199
		private static List<AssetTrackManager.AssetTrackTask> _tasks = new List<AssetTrackManager.AssetTrackTask>();

		// Token: 0x040000C8 RID: 200
		private string sxor;

		// Token: 0x040000C9 RID: 201
		private int lxor;

		// Token: 0x020000D4 RID: 212
		public enum Mode
		{
			// Token: 0x040005AD RID: 1453
			simple,
			// Token: 0x040005AE RID: 1454
			complete
		}

		// Token: 0x020000D5 RID: 213
		private class AssetInfo
		{
			// Token: 0x060008AF RID: 2223 RVA: 0x00039650 File Offset: 0x00037850
			public AssetInfo(int h, int fc, int fm, int elseinformation)
			{
				this.hashCode = h;
				this.dupCount = fc;
				this.totalSize = fm;
				this.elseinfo = elseinformation;
			}

			// Token: 0x040005AF RID: 1455
			public int hashCode;

			// Token: 0x040005B0 RID: 1456
			public int dupCount;

			// Token: 0x040005B1 RID: 1457
			public int totalSize;

			// Token: 0x040005B2 RID: 1458
			public int elseinfo;
		}

		// Token: 0x020000D6 RID: 214
		private class FrameTypeCapture
		{
			// Token: 0x060008B0 RID: 2224 RVA: 0x00039678 File Offset: 0x00037878
			public FrameTypeCapture(int t)
			{
				this.typeIdx = t;
			}

			// Token: 0x060008B1 RID: 2225 RVA: 0x00039694 File Offset: 0x00037894
			public void Reset()
			{
				this.totalCount = 0;
				this.hashCode2AssetInfo.Clear();
			}

			// Token: 0x040005B3 RID: 1459
			public int typeIdx;

			// Token: 0x040005B4 RID: 1460
			public int totalCount;

			// Token: 0x040005B5 RID: 1461
			public Dictionary<int, AssetTrackManager.AssetInfo> hashCode2AssetInfo = new Dictionary<int, AssetTrackManager.AssetInfo>();
		}

		// Token: 0x020000D7 RID: 215
		private class AssetTrackTask
		{
			// Token: 0x060008B2 RID: 2226 RVA: 0x000396A8 File Offset: 0x000378A8
			public AssetTrackTask(Type t, int f, int n)
			{
				this.type = t;
				this.frameInternal = f * n;
				this.typeName = t.Name;
			}

			// Token: 0x040005B6 RID: 1462
			public string typeName;

			// Token: 0x040005B7 RID: 1463
			public Type type;

			// Token: 0x040005B8 RID: 1464
			public float trackInternal;

			// Token: 0x040005B9 RID: 1465
			public float trackCurrent;

			// Token: 0x040005BA RID: 1466
			public int frameInternal;

			// Token: 0x040005BB RID: 1467
			public int AssetCount = -1;

			// Token: 0x040005BC RID: 1468
			public long AssetMemory = -1L;
		}
	}
}
