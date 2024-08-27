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

using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Sample.Sample3
{
	public partial struct SafeZoneSystem : ISystem
	{
		private ComponentLookup<Shooting> m_TurretActiveFromEntity;
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_TurretActiveFromEntity = state.GetComponentLookup<Shooting>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
			const float debugRenderStepInDegrees = 20;
			for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees)
			{
				var a = float3.zero;
				var b = float3.zero;
				math.sincos(math.radians(angle), out a.x, out a.z);
				math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
				Debug.DrawLine(a * radius, b * radius);
			}

			m_TurretActiveFromEntity.Update(ref state);
			var saveZoneJob = new SafeZoneJob()
			{
				TurrentActiveFromEntity = m_TurretActiveFromEntity,
				SquaredRadius = radius * radius
			};
			saveZoneJob.ScheduleParallel();
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}

	[WithAll(typeof(Turret))]
	[BurstCompile]
	public partial struct SafeZoneJob : IJobEntity
	{
		[NativeDisableContainerSafetyRestriction]
		public ComponentLookup<Shooting> TurrentActiveFromEntity;
		
		public float SquaredRadius;
		
		void Execute(Entity entity, LocalToWorld transform)
		{
			TurrentActiveFromEntity.SetComponentEnabled(entity,
				math.lengthsq(transform.Position) > SquaredRadius);
		}
	}
}