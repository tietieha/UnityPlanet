// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-07-04       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace AssetProcessor
{
	[Serializable]
	public class AssetProcessorConfigNode
	{
#if ODIN_INSPECTOR
		[HideLabel]
		[Title("目录")]
		[FolderPath(RequireExistingPath = true)]
#endif
		public string Path;
		
#if ODIN_INSPECTOR
		// [HideLabel]
		[Title("目录下的规则列表")]
		[ValueDropdown("AddProcessorConfig", DrawDropdownForListElements = false, DropdownTitle = "添加规则")]
		[ListDrawerSettings(Expanded = true)]
		[SerializeReference, HideReferenceObjectPicker]// [OdinSerialize]
#endif
		public List<BaseProcessorSettings> Configs = new List<BaseProcessorSettings>();

#if ODIN_INSPECTOR
		private IEnumerable AddProcessorConfig()
		{
			return Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>()
				.Select(x => BaseProcessorSettings.Get(x))
				.Select(x => new ValueDropdownItem(x.resourceType.ToString(), x));
		}
#endif
	}
}