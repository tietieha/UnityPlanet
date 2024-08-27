// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-08-27       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using Unity.Entities;
using UnityEngine;

namespace Sample.Sample3
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public GameObject TankPrefab;
		public int TankCount;
		public float SafeZoneRadius;
	}
	
	public class ConfigAuthoringBaker : Baker<ConfigAuthoring>
	{
		public override void Bake(ConfigAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Config
			{
				TankPrefab = GetEntity(authoring.TankPrefab, TransformUsageFlags.Dynamic),
				TankCount = authoring.TankCount,
				SafeZoneRadius = authoring.SafeZoneRadius
			});
		}
	}
}