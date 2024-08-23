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

using System;
using System.Collections.Generic;
using System.Linq;
using AssetPreprocessor.Scripts.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace AssetProcessor
{
	public enum ResourceType
	{
		Texture,
		Model,
		Audio
	}

	public enum ProcessorPlatform
	{
		Default,
		Standalone,
		Android,
		Iphone
	}

	[Serializable]
	public class BaseProcessorSettings
	{
		protected const string LEFT_VERTICAL_GROUP = "Split/Left";
		protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
		protected const string BASE_GROUP = LEFT_VERTICAL_GROUP + "/Base";

#if ODIN_INSPECTOR
		[HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
		[VerticalGroup(LEFT_VERTICAL_GROUP)]
		[BoxGroup(BASE_GROUP)]
		[ReadOnly]
#endif
		public ResourceType resourceType;


#if ODIN_INSPECTOR
        [BoxGroup(BASE_GROUP)]
#endif
		[Tooltip("Whether this config should be considered when processing assets.")]
		public bool IsEnabled = true;

#if ODIN_INSPECTOR
        [BoxGroup(BASE_GROUP)]
#endif
		[Tooltip(
			"Some assets will not be preprocessed again if they already match certain criteria. Enable this to always force preprocessing.")]
		public bool ForcePreprocess;

#if ODIN_INSPECTOR
        [BoxGroup(BASE_GROUP)]
#endif
		[Tooltip("Configs with lower sort order are checked first.")]
		public int ConfigSortOrder = 10;

		[Header("Platforms")] public List<ProcessorPlatform> PlatformsRegexList = new List<ProcessorPlatform>
		{
			ProcessorPlatform.Android,
			ProcessorPlatform.Iphone
		};

		[Header("Asset Path Regex Matching")]
		[Tooltip("Config will be used if asset path matches regex list. Use a * wildcard to match all.")]
		public List<string> MatchRegexList = new List<string>();

		[Tooltip("Any asset path matching the ignore regex list will be ignored.")]
		public List<string> IgnoreRegexList = new List<string>();
#if ODIN_INSPECTOR
		[BoxGroup(BASE_GROUP)] 
#endif
		public bool EnableVerboseLogging;

		#region Func

		public virtual bool ShouldUseConfigForAssetImporter(AssetImporter assetImporter)
		{
			if (!IsEnabled) return false;

			var assetPath = assetImporter.assetPath;

			if (!AssetPreprocessorUtils.DoesRegexStringListMatchString(
				    FilterOutBadRegexStrings(MatchRegexList, nameof(MatchRegexList)), assetPath)) return false;

			// Check that the Asset path does NOT match any ignore regex strings.
			if (AssetPreprocessorUtils.DoesRegexStringListMatchString(
				    FilterOutBadRegexStrings(IgnoreRegexList, nameof(IgnoreRegexList)), assetPath)) return false;

			return true;
		}

		private List<string> FilterOutBadRegexStrings(List<string> regexStrings, string fieldName)
		{
			// Create a copy so that we don't mutate the original list.
			regexStrings = regexStrings.ToList();

			for (var i = 0; i < regexStrings.Count; i++)
			{
				var regexString = regexStrings[i];

				if (regexString == string.Empty)
				{
					Debug.LogError(
						$" - {fieldName} - Regex string at index: {i} is empty. Ignoring, otherwise it will match everything.");

					regexStrings.RemoveAt(i);

					i--;
				}
			}

			return regexStrings;
		}


		public BaseProcessorSettings(ResourceType resourceType)
		{
			this.resourceType = resourceType;
		}

		public static BaseProcessorSettings Get(ResourceType type)
		{
			switch (type)
			{
				case ResourceType.Texture:
					return new TextureProcessorSettings();
				case ResourceType.Model:
					return new ModelProcessorSettings();
				case ResourceType.Audio:
					return new AudioProcessorSettings();
			}

			return null;
		}

		#endregion
	}
}