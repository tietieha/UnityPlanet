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

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Sample.Sample3
{
	public partial struct TurretShootingSytem : ISystem
	{
		ComponentLookup<LocalToWorld> m_LocalToWorldFromEntity;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
			m_LocalToWorldFromEntity = state.GetComponentLookup<LocalToWorld>(true);
		}
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			m_LocalToWorldFromEntity.Update(ref state);
			var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
			var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
			var turretShootingJob = new TurretShootingJob
			{
				LocalToWorldEntity = m_LocalToWorldFromEntity,
				ECB = ecb
			};
			turretShootingJob.Schedule();
		}
	}

	
	[WithAll(typeof(Shooting))]
	[BurstCompile]
	public partial struct TurretShootingJob : IJobEntity
	{
		[ReadOnly]
		public ComponentLookup<LocalToWorld> LocalToWorldEntity;
		public EntityCommandBuffer ECB;
		
		public void Execute(TurretAspect turret)
		{
			var instance = ECB.Instantiate(turret.CannonBallPrefab);
			var spawnLocalToWorld = LocalToWorldEntity[turret.CannonBallSpawn];
			var cannonBallTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position);
			var prefabInfo = LocalToWorldEntity[turret.CannonBallPrefab];
			float4x4 val = prefabInfo.Value;
			ECB.SetComponent(instance, cannonBallTransform);
			ECB.AddComponent(instance, new PostTransformMatrix
			{
				Value = val
			});
			ECB.SetComponent(instance, new CannonBall
			{
				Speed = spawnLocalToWorld.Forward * 20.0f
			});
			ECB.SetComponent(instance, new URPMaterialPropertyBaseColor
			{
				Value = turret.Color
			});
		}
	}
}