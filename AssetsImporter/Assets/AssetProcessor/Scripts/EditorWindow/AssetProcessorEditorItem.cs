// // **********************************************************
// // *		                .-"""-.							*
// // *		               / .===. \			            *
// // *		               \/ 6 6 \/			            *
// // *		     ______ooo__\__=__/_____________			*
// // *		    / @author     Leon			   /			*
// // *		   / @Modified   2024-07-01       /			    *
// // *		  /_____________________ooo______/			    *
// // *		  			    |_ | _|			                *
// // *		  			    /-'Y'-\			                *
// // *		  			   (__/ \__)			            *
// // **********************************************************
//
// using Sirenix.OdinInspector;
//
// namespace AssetProcessor
// {
// 	public class AssetProcessorEditorItem
// 	{
// 		[ShowIf("@this._configIndex > -1")]
// 		[HideLabel]
// 		public AssetProcessorConfigNode ConfigNode;
// 		
//
// 		// =============================================================================================================
// 		private AssetProcessorDataCenter assetProcessorDataCenter;
// 		private readonly string folderPath;
// 		private int _configIndex = -1;
// 		public AssetProcessorEditorItem(string path, AssetProcessorDataCenter assetProcessorDataCenter)
// 		{
// 			this.folderPath = path;
// 			this.assetProcessorDataCenter = assetProcessorDataCenter;
// 		}
//
// 		public void Refresh()
// 		{
// 			// _configIndex = AssetProcessorConfigTree.Instance.AssetProcessorConfigs.FindIndex(item => item.Path.Equals(folderPath));
// 			// if (_configIndex > -1)
// 			// {
// 			// 	ConfigNode = AssetProcessorConfigTree.Instance.AssetProcessorConfigs[_configIndex];
// 			// }
// 			// else
// 			// {
// 			// 	ConfigNode = null;
// 			// }
// 		}
// 		
// 		public void Execute()
// 		{
// 			
// 		}
// 		
// 		public void CreateConfig()
// 		{
// 			// assetProcessorDataCenter.ConfigFile.Add(folderPath);
// 			Refresh();
// 		}
// 		
// 		public bool HasConfig()
// 		{
// 			return false;
// 			// return assetProcessorDataCenter.ConfigFile.HasItem(folderPath);
// 		}
// 	}
// }