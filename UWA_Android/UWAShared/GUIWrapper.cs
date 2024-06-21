using System;
using UnityEngine;
using UWA;

namespace UWAShared
{
	// Token: 0x02000041 RID: 65
	public class GUIWrapper : MonoBehaviour
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x00019624 File Offset: 0x00017824
		public static GUIWrapper Get()
		{
			return GUIWrapper._inst;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00019644 File Offset: 0x00017844
		public void Awake()
		{
			bool flag = GUIWrapper._inst != null;
			if (flag)
			{
				Object.Destroy(this);
			}
			else
			{
				Object.DontDestroyOnLoad(base.gameObject);
				GUIWrapper._inst = this;
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00019688 File Offset: 0x00017888
		public void OnGUI()
		{
			GUI.Window(17857, SharedUtils.GroupRect, new GUI.WindowFunction(this.DoVersionWindow), "UWA SDK");
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x000196AC File Offset: 0x000178AC
		public void DoVersionWindow(int id)
		{
			TextAnchor alignment = GUI.skin.label.alignment;
			GUI.skin.label.alignment = 4;
			GUILayout.Label(SharedUtils.Version, SharedUtils.GetSuitableOption(0.8f, 0.8f));
			GUI.skin.label.alignment = alignment;
		}

		// Token: 0x0400021B RID: 539
		public static bool ControlByPoco;

		// Token: 0x0400021C RID: 540
		private static GUIWrapper _inst;
	}
}
