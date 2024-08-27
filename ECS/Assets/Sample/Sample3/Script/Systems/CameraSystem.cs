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

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sample.Sample3
{
	[UpdateInGroup(typeof(LateSimulationSystemGroup))]
	public partial class CameraSystem : SystemBase
	{
		private Entity m_Target;
		private Random m_Random;
		private EntityQuery m_TanksQuery;

		protected override void OnCreate()
		{
			m_Random = Random.CreateFromIndex(1234);
			m_TanksQuery = GetEntityQuery(typeof(Tank));
			RequireForUpdate(m_TanksQuery);
		}

		protected override void OnUpdate()
		{
			if (m_Target == Entity.Null || Input.GetKeyDown(KeyCode.Space))
			{
				var tanks = m_TanksQuery.ToEntityArray(Allocator.Temp);
				m_Target = tanks[m_Random.NextInt(0, tanks.Length)];
			}

			var cameraTransform = CameraSingleton.Instance.transform;
			var tankTransform = SystemAPI.GetComponent<LocalToWorld>(m_Target);
			cameraTransform.position = tankTransform.Position - 10.0f * tankTransform.Forward +
			                           new float3(0.0f, 5.0f, 0.0f);
			cameraTransform.LookAt(tankTransform.Position, new float3(0.0f, 1.0f, 0.0f));
		}
	}
}