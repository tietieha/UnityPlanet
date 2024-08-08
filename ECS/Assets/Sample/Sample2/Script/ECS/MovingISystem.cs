// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-08-05       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample.Sample2
{
    [BurstCompile]
    public partial struct MovingISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // non Job
            // var deltaTime = SystemAPI.Time.DeltaTime;
            // foreach (MovingAspect aspect in SystemAPI.Query<MovingAspect>())
            // {
            //     aspect.Move(deltaTime);
            // }

            new MoveJob { deltaTime = SystemAPI.Time.DeltaTime }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float deltaTime;

        [BurstCompile]
        public void Execute(MovingAspect aspect)
        {
            aspect.Move(deltaTime);
        }
    }

    public readonly partial struct MovingAspect : IAspect
    {
        private readonly Entity entity;
        private readonly RefRO<Speed> speed;
        private readonly RefRW<LocalTransform> transform;

        public void Move(float deltaTime)
        {
            transform.ValueRW.Position += new float3(1, 0, 0) * speed.ValueRO.value * deltaTime;
        }
    }
}