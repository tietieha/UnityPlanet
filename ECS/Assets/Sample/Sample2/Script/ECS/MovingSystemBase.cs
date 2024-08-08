// // **********************************************************
// // *		                .-"""-.							*
// // *		               / .===. \			            *
// // *		               \/ 6 6 \/			            *
// // *		     ______ooo__\__=__/_____________			*
// // *		    / @author     Leon			   /			*
// // *		   / @Modified   2024-08-05       /			    *
// // *		  /_____________________ooo______/			    *
// // *		  			    |_ | _|			                *
// // *		  			    /-'Y'-\			                *
// // *		  			   (__/ \__)			            *
// // **********************************************************
//
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
//
// namespace Sample.Sample2
// {
//     public partial class MovingSystemBase : SystemBase
//     {
//         
//         protected override void OnUpdate()
//         {
//             var deltaTime = SystemAPI.Time.DeltaTime;
//             Entities.ForEach((ref LocalTransform transform, in Speed speed) =>
//             {
//                 transform.Position += new float3(1, 0, 0) * speed.value * deltaTime;
//             }).ScheduleParallel();
//         }
//     }
// }