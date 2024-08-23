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

using UnityEditor;
using UnityEngine;

namespace AssetProcessor
{
	public class AudioProcessorSettings : BaseProcessorSettings
	{
		private const string CONFIG_GROUP = RIGHT_VERTICAL_GROUP + "/Audio";

#if ODIN_INSPECTOR
		[VerticalGroup(RIGHT_VERTICAL_GROUP)]
		[BoxGroup(CONFIG_GROUP)]
#endif
		public bool ForceToMono = true;

#if ODIN_INSPECTOR
		[BoxGroup(CONFIG_GROUP)]
#endif
		public bool Normalize = true;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		public bool LoadInBackground;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		public bool Ambisonic;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		[Space] public AudioClipLoadType AudioClipLoadType = AudioClipLoadType.CompressedInMemory;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		public bool PreloadAudioData;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		public AudioCompressionFormat AudioCompressionFormat = AudioCompressionFormat.Vorbis;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		[Range(0, 100)] public float Quality = 70;

#if ODIN_INSPECTOR
        [BoxGroup(CONFIG_GROUP)]
#endif
		public AudioSampleRateSetting AudioSampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;

		#region Func

		public AudioProcessorSettings() : base(ResourceType.Audio)
		{
		}

		#endregion
	}
}