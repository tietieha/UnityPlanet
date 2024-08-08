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

using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sample.Sample2
{
    public class SpeedManager : MonoBehaviour
    {
        public float globalSpeed;

        private void Update()
        {
            var entities = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(Speed)).ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var speed = World.DefaultGameObjectInjectionWorld.EntityManager
                    .GetComponentData<Speed>(entity);
                speed.value = globalSpeed;
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, speed);
            }
        }
    }
}