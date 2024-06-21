using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWA;

namespace UWACore.RenderTools
{
	// Token: 0x02000030 RID: 48
	internal static class VisibleCollector
	{
		// Token: 0x0600021E RID: 542 RVA: 0x0000DDC4 File Offset: 0x0000BFC4
		public static bool IsVisible(int instId, Type resType)
		{
			if (resType == typeof(Texture2D))
			{
				return VisibleCollector._allTexIds.Contains(instId);
			}
			if (resType == typeof(Mesh))
			{
				return VisibleCollector._allMeshIds.Contains(instId);
			}
			return VisibleCollector._allMeshIds.Contains(instId) || VisibleCollector._allTexIds.Contains(instId);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000DE2C File Offset: 0x0000C02C
		public static void UpdateVisibleCache()
		{
			if (Time.frameCount - VisibleCollector.updateFrame < 30)
			{
				return;
			}
			VisibleCollector.updateFrame = Time.frameCount;
			VisibleCollector._allMeshIds.Clear();
			VisibleCollector._allTexIds.Clear();
			foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>())
			{
				if (!(meshCollider == null) && meshCollider.enabled && !(meshCollider.gameObject == null) && meshCollider.gameObject.activeInHierarchy && meshCollider.sharedMesh != null)
				{
					int instanceID = meshCollider.sharedMesh.GetInstanceID();
					if (instanceID != -1)
					{
						VisibleCollector._allMeshIds.Add(instanceID);
					}
				}
			}
			foreach (Image image in Resources.FindObjectsOfTypeAll<Image>())
			{
				if (!(image == null) && image.enabled && !(image.gameObject == null) && image.gameObject.activeInHierarchy && image.sprite != null)
				{
					if (image.sprite.texture != null)
					{
						int instanceID2 = image.sprite.texture.GetInstanceID();
						if (instanceID2 != -1)
						{
							VisibleCollector._allTexIds.Add(instanceID2);
						}
					}
					Texture2D associatedAlphaSplitTexture = image.sprite.associatedAlphaSplitTexture;
					if (associatedAlphaSplitTexture != null)
					{
						int instanceID2 = associatedAlphaSplitTexture.GetInstanceID();
						if (instanceID2 != -1)
						{
							VisibleCollector._allTexIds.Add(instanceID2);
						}
					}
				}
			}
			foreach (Renderer renderer in Resources.FindObjectsOfTypeAll<Renderer>())
			{
				if (!(renderer == null) && !(renderer.gameObject == null) && renderer.gameObject.activeInHierarchy && renderer.enabled && renderer.isVisible)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					MeshRenderer meshRenderer = renderer as MeshRenderer;
					ParticleSystemRenderer particleSystemRenderer = renderer as ParticleSystemRenderer;
					SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
					int num = -1;
					HashSet<int> hashSet = new HashSet<int>();
					if (skinnedMeshRenderer != null)
					{
						try
						{
							VisibleCollector.TryGetMeshTextureInfo(skinnedMeshRenderer, ref num, ref hashSet);
							goto IL_2C8;
						}
						catch (Exception)
						{
							goto IL_2C8;
						}
						goto IL_25F;
					}
					goto IL_25F;
					IL_2C8:
					if (num != -1)
					{
						VisibleCollector._allMeshIds.Add(num);
					}
					foreach (int item in hashSet)
					{
						VisibleCollector._allTexIds.Add(item);
					}
					goto IL_31D;
					IL_25F:
					if (meshRenderer != null)
					{
						try
						{
							VisibleCollector.TryGetMeshTextureInfo(meshRenderer, ref num, ref hashSet);
							goto IL_2C8;
						}
						catch (Exception)
						{
							goto IL_2C8;
						}
					}
					if (particleSystemRenderer != null)
					{
						try
						{
							VisibleCollector.TryGetMeshTextureInfo(particleSystemRenderer, ref num, ref hashSet);
							goto IL_2C8;
						}
						catch (Exception)
						{
							goto IL_2C8;
						}
					}
					if (spriteRenderer != null)
					{
						try
						{
							VisibleCollector.TryGetMeshTextureInfo(spriteRenderer, ref num, ref hashSet);
						}
						catch (Exception)
						{
						}
						goto IL_2C8;
					}
					goto IL_2C8;
				}
				IL_31D:;
			}
			foreach (LightmapData lightmapData in LightmapSettings.lightmaps)
			{
				if (lightmapData.lightmapColor != null)
				{
					VisibleCollector._allTexIds.Add(lightmapData.lightmapColor.GetInstanceID());
				}
				if (lightmapData.lightmapDir != null)
				{
					VisibleCollector._allTexIds.Add(lightmapData.lightmapDir.GetInstanceID());
				}
				if (lightmapData.shadowMask != null)
				{
					VisibleCollector._allTexIds.Add(lightmapData.shadowMask.GetInstanceID());
				}
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000E250 File Offset: 0x0000C450
		private static void TryGetTextureInfo(Renderer renderer, ref HashSet<int> texInstIds)
		{
			if (renderer == null)
			{
				return;
			}
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			if (spriteRenderer != null && spriteRenderer.sprite != null)
			{
				Texture texture = spriteRenderer.sprite.texture;
				Texture associatedAlphaSplitTexture = spriteRenderer.sprite.associatedAlphaSplitTexture;
				int item = -1;
				if (texture != null && VisibleCollector.GetObjId(texture, out item))
				{
					texInstIds.Add(item);
				}
				if (associatedAlphaSplitTexture != null && VisibleCollector.GetObjId(associatedAlphaSplitTexture, out item))
				{
					texInstIds.Add(item);
				}
				return;
			}
			Material[] sharedMaterials = renderer.sharedMaterials;
			List<int> list = new List<int>();
			if (sharedMaterials != null)
			{
				foreach (Material material in sharedMaterials)
				{
					if (!(material == null))
					{
						SharedUtils.MatPropTools.GetTexPropIds(material, ref list);
						for (int j = 0; j < list.Count; j++)
						{
							Texture texture2 = material.GetTexture(list[j]);
							if (!(texture2 == null))
							{
								int num = -1;
								VisibleCollector.GetObjId(texture2, out num);
								if (num != -1)
								{
									texInstIds.Add(num);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E39C File Offset: 0x0000C59C
		private static void TryGetMeshTextureInfo(SpriteRenderer sr, ref int meshInstId, ref HashSet<int> texInstIds)
		{
			if (sr == null)
			{
				return;
			}
			VisibleCollector.TryGetTextureInfo(sr, ref texInstIds);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
		private static void TryGetMeshTextureInfo(SkinnedMeshRenderer smr, ref int meshInstId, ref HashSet<int> texInstIds)
		{
			if (smr == null)
			{
				return;
			}
			if (smr.sharedMesh != null)
			{
				VisibleCollector.GetObjId(smr.sharedMesh, out meshInstId);
			}
			VisibleCollector.TryGetTextureInfo(smr, ref texInstIds);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E3E8 File Offset: 0x0000C5E8
		private static void TryGetMeshTextureInfo(MeshRenderer mr, ref int meshInstId, ref HashSet<int> texInstIds)
		{
			if (mr == null)
			{
				return;
			}
			MeshFilter component = mr.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh != null)
			{
				VisibleCollector.GetObjId(component.sharedMesh, out meshInstId);
			}
			VisibleCollector.TryGetTextureInfo(mr, ref texInstIds);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E440 File Offset: 0x0000C640
		private static void TryGetMeshTextureInfo(ParticleSystemRenderer psr, ref int meshInstId, ref HashSet<int> texInstIds)
		{
			if (psr == null)
			{
				return;
			}
			if (psr.mesh != null)
			{
				VisibleCollector.GetObjId(psr.mesh, out meshInstId);
			}
			VisibleCollector.TryGetTextureInfo(psr, ref texInstIds);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E474 File Offset: 0x0000C674
		private static bool GetObjId(Object obj, out int instId)
		{
			instId = -1;
			if (obj == null)
			{
				return false;
			}
			instId = obj.GetInstanceID();
			return true;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E490 File Offset: 0x0000C690
		private static string Id2Name(int instId)
		{
			string result = null;
			if (VisibleCollector._nameCache.TryGetValue(instId, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x04000121 RID: 289
		private static readonly HashSet<int> _allMeshIds = new HashSet<int>();

		// Token: 0x04000122 RID: 290
		private static readonly HashSet<int> _allTexIds = new HashSet<int>();

		// Token: 0x04000123 RID: 291
		private static int updateFrame = -1;

		// Token: 0x04000124 RID: 292
		private static readonly Dictionary<int, string> _nameCache = new Dictionary<int, string>();
	}
}
