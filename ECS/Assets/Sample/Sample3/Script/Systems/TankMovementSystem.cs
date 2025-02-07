﻿// **********************************************************
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

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample.Sample3
{
	public partial class TankMovementSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var dt = SystemAPI.Time.DeltaTime;
			Entities.WithAll<Tank>().ForEach((Entity entity, ref LocalTransform transform) =>
			{
				var pos = transform.Position;
				pos.y = entity.Index;
				var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
				var dir = float3.zero;
				math.sincos(angle, out dir.x, out dir.z);
				transform.Position += dir * dt * 5.0f;
				transform.Rotation = quaternion.RotateY(angle);
			}).ScheduleParallel();
		}
	}
}