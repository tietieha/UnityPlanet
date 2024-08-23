// // **********************************************************
// // *		                .-"""-.							*
// // *		               / .===. \			            *
// // *		               \/ 6 6 \/			            *
// // *		     ______ooo__\__=__/_____________			*
// // *		    / @author     Leon			   /			*
// // *		   / @Modified   2024-07-06       /			    *
// // *		  /_____________________ooo______/			    *
// // *		  			    |_ | _|			                *
// // *		  			    /-'Y'-\			                *
// // *		  			   (__/ \__)			            *
// // **********************************************************
//
// using System;
//
// namespace AssetProcessor
// {
// 	public class AssetImportProcessorUtils
// 	{
// 		public static BaseProcessorSettings GetMatchedAssetProcessorConfig(string assetPath)
// 		{
// 			return GetMatchedAssetProcessorConfig(AssetProcessorConfig.DefaultFile, assetPath);
// 		}
//
// 		public static BaseProcessorSettings GetMatchedAssetProcessorConfig(
// 			AssetProcessorConfig config,
// 			string assetPath)
// 		{
// 			AssetProcessorConfigNode node = config.DefaultConfig;
// 			
// 			// 先匹配 global
// 			
// 			
// 			// 再匹配 目录
// 			var path = assetPath;
// 			int index = -1;
// 			while ((index = path.LastIndexOf("/", StringComparison.Ordinal)) > -1)
// 			{
// 				path = path.Substring(0, index);
// 				if (config.ConfigNodeDic.TryGetValue(path, out node))
// 				{
// 					
// 				}
// 			}
// 			
// 			
// 			
// 			return node;
// 		}
// 	}
// }