// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-08-23       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using Unity.Entities;
using UnityEngine;

namespace Sample.Sample3
{
	public class TurretAuthoring : MonoBehaviour
	{
		public GameObject CannonBallPrefab;
		public Transform CannonBallSpawn;
	}

	public class TurretBaker : Baker<TurretAuthoring>
	{
		public override void Bake(TurretAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Turret
			{
				CannonBallPrefab = GetEntity(authoring.CannonBallPrefab, TransformUsageFlags.Dynamic),
				CannonBallSpawn = GetEntity(authoring.CannonBallSpawn, TransformUsageFlags.Dynamic),
			});
			
		}
	}
}