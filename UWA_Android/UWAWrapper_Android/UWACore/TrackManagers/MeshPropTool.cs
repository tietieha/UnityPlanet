using System;
using System.Collections.Generic;
using UnityEngine;

namespace UWACore.TrackManagers
{
	// Token: 0x02000037 RID: 55
	internal static class MeshPropTool
	{
		// Token: 0x0600027C RID: 636 RVA: 0x00010604 File Offset: 0x0000E804
		public static void Setup()
		{
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010608 File Offset: 0x0000E808
		public static uint TotalIndexCount(Mesh m)
		{
			int subMeshCount = m.subMeshCount;
			uint num = 0U;
			for (int i = 0; i < subMeshCount; i++)
			{
				num += m.GetIndexCount(i);
			}
			return num;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0001064C File Offset: 0x0000E84C
		public static bool HasProp(Mesh m, MeshPropTool.MeshProp prop)
		{
			bool result;
			switch (prop)
			{
			case MeshPropTool.MeshProp.Normal:
				m.GetNormals(MeshPropTool.tmpV3List);
				result = (MeshPropTool.tmpV3List.Count != 0);
				break;
			case MeshPropTool.MeshProp.Tangent:
				m.GetTangents(MeshPropTool.tmpV4List);
				result = (MeshPropTool.tmpV4List.Count != 0);
				break;
			case MeshPropTool.MeshProp.Color:
				m.GetColors(MeshPropTool.tmpCList);
				result = (MeshPropTool.tmpCList.Count != 0);
				break;
			case MeshPropTool.MeshProp.BoneWeight:
				m.GetBoneWeights(MeshPropTool.tmpBWList);
				result = (MeshPropTool.tmpBWList.Count != 0);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x04000164 RID: 356
		private static List<Vector3> tmpV3List = new List<Vector3>();

		// Token: 0x04000165 RID: 357
		private static List<Vector4> tmpV4List = new List<Vector4>();

		// Token: 0x04000166 RID: 358
		private static List<Color32> tmpCList = new List<Color32>();

		// Token: 0x04000167 RID: 359
		private static List<BoneWeight> tmpBWList = new List<BoneWeight>();

		// Token: 0x020000F6 RID: 246
		public enum MeshProp
		{
			// Token: 0x0400062B RID: 1579
			Normal,
			// Token: 0x0400062C RID: 1580
			Tangent,
			// Token: 0x0400062D RID: 1581
			Color,
			// Token: 0x0400062E RID: 1582
			BoneWeight,
			// Token: 0x0400062F RID: 1583
			Triangle
		}
	}
}
