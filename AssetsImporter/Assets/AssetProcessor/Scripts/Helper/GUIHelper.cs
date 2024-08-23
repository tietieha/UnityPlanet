// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-07-03       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using UnityEditor;
using UnityEngine;

namespace AssetProcessor
{
	public class GUIHelper
	{
		public static readonly GUIStyle CenterLargeLabel = new GUIStyle(EditorStyles.largeLabel)
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 50,
		};
        
		public static bool Button(string label, Color color, params GUILayoutOption[] options)
		{
			Color preColor = GUI.color;
			GUI.color = color;
			if (GUILayout.Button(label, options))
			{
				return true;
			}

			GUI.color = preColor;
			return false;
		}
	}
}