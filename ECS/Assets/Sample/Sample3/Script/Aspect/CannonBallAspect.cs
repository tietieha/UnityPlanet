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
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample.Sample3
{
	public readonly partial struct CannonBallAspect : IAspect
	{
		public readonly Entity Entity;

		private readonly RefRW<CannonBall> _cannonBall;
		private readonly RefRW<LocalTransform> _transform;

		public float3 Position
		{
			get => _transform.ValueRO.Position;
			set => _transform.ValueRW.Position = value;
		}

		public float3 Speed
		{
			get => _cannonBall.ValueRO.Speed;
            set => _cannonBall.ValueRW.Speed = value;
		}
	}
}