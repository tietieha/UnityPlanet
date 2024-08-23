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
	public class TankAuthoring : MonoBehaviour
	{
		
	}

	public class TankBaker : Baker<TankAuthoring>
	{
		public override void Bake(TankAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Tank());
		}
	}
}