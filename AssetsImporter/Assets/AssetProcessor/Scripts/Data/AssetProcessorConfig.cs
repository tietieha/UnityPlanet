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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetProcessor
{
	[CreateAssetMenu(menuName = "AssetPreprocessor/Asset Processor Config", fileName = "AssetProcessorConfig")]
	public class AssetProcessorConfig : ScriptableObject, ISerializationCallbackReceiver
	{
		#region 数据
		public AssetProcessorConfigNode DefaultConfig;
		public AssetProcessorConfigNode GlobalConfig;
		
		[SerializeField]
		// [ListDrawerSettings(CustomAddFunction = "", CustomRemoveElementFunction = "")]
		public List<AssetProcessorConfigNode> ConfigNodes;
		#endregion
		
		#region 默认数据
		private static AssetProcessorConfig _defaultFile;
		public static AssetProcessorConfig DefaultFile
		{
			get
			{
				if (_defaultFile == null)
				{
					var findPath = AssetDatabase.FindAssets($"t:Script {nameof(AssetProcessorConfig)}");
					var scriptPath = AssetDatabase.GUIDToAssetPath(findPath[0]);
					var configPath = scriptPath.Replace($"{nameof(AssetProcessorConfig)}.cs", $"{nameof(AssetProcessorConfig)}.asset");
					if (File.Exists(configPath))
					{
						_defaultFile = AssetDatabase.LoadAssetAtPath<AssetProcessorConfig>(configPath);
					}
					else
					{
						_defaultFile = ScriptableObject.CreateInstance<AssetProcessorConfig>();
						AssetDatabase.CreateAsset(_defaultFile, configPath);
					}
				}

				return _defaultFile;
			}
		}
		
		private Dictionary<string, AssetProcessorConfigNode> _configNodeDic;
		public Dictionary<string, AssetProcessorConfigNode> ConfigNodeDic
		{
			get
			{
				if (_configNodeDic == null)
				{
					_configNodeDic = ConfigNodes
						.Where(node => !string.IsNullOrEmpty(node.Path))
						.ToDictionary(node => node.Path, node => node);
				}
				return _configNodeDic;
			}
		}
		#endregion

		public void OnBeforeSerialize()
		{
			
		}

		public void OnAfterDeserialize()
		{
			_configNodeDic = null;
		}
	}
}