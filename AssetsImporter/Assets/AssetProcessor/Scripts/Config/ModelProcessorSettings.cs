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
	public class ModelProcessorSettings : BaseProcessorSettings
	{
		private const string CONFIG_GROUP = RIGHT_VERTICAL_GROUP + "/Model";
#if ODIN_INSPECTOR
		[VerticalGroup(RIGHT_VERTICAL_GROUP)]
		[BoxGroup(CONFIG_GROUP)]
#endif
		public bool import;

		#region Func
		public ModelProcessorSettings() : base(ResourceType.Model)
		{
		}
		#endregion

	}
	
	
	// public class ModelProcessorConfigDrawer : OdinValueDrawer<ModelProcessorConfig> 
	// {
	// 	protected override void DrawPropertyLayout(GUIContent label)
	// 	{
	// 		base.DrawPropertyLayout(label);
	// 	}
	// }
}