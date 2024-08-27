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
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sample.Sample3
{
	[BurstCompile]
	public partial struct TankSpawningSystem : ISystem
	{
		EntityQuery m_BaseColorQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			// 保证Config存在,系统才运行
			state.RequireForUpdate<Config>();

			m_BaseColorQuery =
				state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var random = Random.CreateFromIndex(1234);
			var hue = random.NextFloat();
			URPMaterialPropertyBaseColor RandomColor()
			{
				hue = (hue + 0.618034005f) % 1;
				var color = Color.HSVToRGB(hue, 1.0f, 1.0f);
				return new URPMaterialPropertyBaseColor() { Value = (Vector4)color };
			}

			/// 初始化内容
			/// 1.为什么不放在OnCreate里
			///		因为OnCreate的时候 Config还不存在
			/// 2.如果烘焙函数比这个OnUpdate还要晚怎么办？
			///		OnCreate中加 state.RequireForUpdate<Config>();
			var config = SystemAPI.GetSingleton<Config>();
			var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
			var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
			var vehicles =
				CollectionHelper.CreateNativeArray<Entity>(config.TankCount, Allocator.Temp);
			ecb.Instantiate(config.TankPrefab, vehicles);

			var queryMask = m_BaseColorQuery.GetEntityQueryMask();
			foreach (var vehicle in vehicles)
			{
				ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor());
			}
			
			
			state.Enabled = false;
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
}