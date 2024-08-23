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
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Sample.Sample3
{
	[BurstCompile]
    partial struct TurretRotationSystem : ISystem
    {
	    [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
	        quaternion rotation = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);
	        foreach (var transform in SystemAPI.Query<RefRW<LocalToWorld>>().WithAll<Turret>())
	        {
		        Quaternion q = rotation;
		        Vector3 angel = q.eulerAngles;
		        q = transform.ValueRW.Rotation;
		        angel += q.eulerAngles;
		        rotation = Quaternion.Euler(angel);
		        LocalTransform localTransform = new LocalTransform();
		        localTransform.Position = transform.ValueRW.Position;
		        localTransform.Rotation = rotation;
		        localTransform.Scale = 1;
		        float4x4 mat = localTransform.ToMatrix();
		        float3 scale = transform.ValueRW.Value.Scale();
		        mat.c0 *= scale.x;
		        mat.c1 *= scale.y;
		        mat.c2 *= scale.z;
		        transform.ValueRW.Value = mat;
	        }
        }
        
    }
}