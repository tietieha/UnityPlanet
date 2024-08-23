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

using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

namespace Sample.Sample3
{
	public struct Turret : IComponentData
	{
		public Entity CannonBallPrefab;
		public Entity CannonBallSpawn;
	}
}