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

namespace Sample.Sample3
{
	public readonly partial struct TurretAspect : IAspect
	{
		readonly RefRO<Turret> m_Turret;
		public Entity CannonBallPrefab => m_Turret.ValueRO.CannonBallPrefab;
		public Entity CannonBallSpawn => m_Turret.ValueRO.CannonBallSpawn;
	}
}