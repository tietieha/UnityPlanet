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
using Unity.Entities;
using Unity.Mathematics;

namespace Sample.Sample3
{
	[BurstCompile]
	public partial struct CannonBallSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
		}
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
			var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
			var cannonBallJob = new CannonBallJob
			{
				ECB = ecb.AsParallelWriter(),
                DeltaTime = SystemAPI.Time.DeltaTime
            };
			cannonBallJob.ScheduleParallel();
		}
	}
	
	[BurstCompile]
	public partial struct CannonBallJob : IJobEntity
	{
		public EntityCommandBuffer.ParallelWriter ECB;
		public float DeltaTime;
		
		public void Execute([ChunkIndexInQuery] int chunkIndex, CannonBallAspect cannonBall)
		{
			var gravity = new float3(0f, -9.82f, 0f);
			var invertY = new float3(1f, -1f, 1f);
			cannonBall.Position += cannonBall.Speed * DeltaTime;
			if (cannonBall.Position.y < 0f)
			{
				cannonBall.Position *= invertY;
				cannonBall.Speed *= invertY * 0.8f;
			}
			cannonBall.Speed += gravity * DeltaTime;
			var speed = math.lengthsq(cannonBall.Speed);
			if (speed < 0.01f)
			{
				ECB.DestroyEntity(chunkIndex, cannonBall.Entity);
			}
		}
	}
}