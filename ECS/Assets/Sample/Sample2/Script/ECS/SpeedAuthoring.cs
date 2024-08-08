using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sample.Sample2
{
    public class SpeedAuthoring : MonoBehaviour
    {
        public float value;
    }

    public struct Speed : IComponentData
    {
        public float value;
    }

    public class SpeedBaker : Baker<SpeedAuthoring>
    {
        public override void Bake(SpeedAuthoring authoring)
        {
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Speed { value = authoring.value });
        }
    }
}