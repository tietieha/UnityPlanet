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

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Sample.Sample3
{
    public partial struct TurretRotationSystem : ISystem
    {
        public void Update(ref SystemState state)
        {
            var rotation = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);
            foreach (var transform in SystemAPI.Query<LocalTransform>())
            {
                transform.Rotate(rotation);
            }
        }
    }
}