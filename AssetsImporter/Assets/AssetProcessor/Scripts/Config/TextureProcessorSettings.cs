// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-07-01       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif
using UnityEngine;

namespace AssetProcessor
{
	public class TextureProcessorSettings : BaseProcessorSettings
	{
		private const string CONFIG_GROUP = RIGHT_VERTICAL_GROUP + "/Texture";
#if ODIN_INSPECTOR
		[VerticalGroup(RIGHT_VERTICAL_GROUP)]
		[BoxGroup(CONFIG_GROUP)]
#endif
		public bool mipmapEnable;

		#region Func
		public TextureProcessorSettings() : base(ResourceType.Texture)
		{
		}
		#endregion
	}

	// public class TextureProcessorConfigDrawer : OdinValueDrawer<TextureProcessorConfig>
	// {
	// 	protected override void DrawPropertyLayout(GUIContent label)
	// 	{
	// 		base.DrawPropertyLayout(label);
	// 	}
	// }
}