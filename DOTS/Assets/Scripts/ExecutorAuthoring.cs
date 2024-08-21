using UnityEngine;
using Unity.Entities;

public class ExecutorAuthoring : MonoBehaviour
{
    public bool isOn;

    class ExecutorBaker : Baker<ExecutorAuthoring>
    {
        public override void Bake(ExecutorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None); //空物体None即可

            if (authoring.isOn)
            {
                //AddComponent(entity, new TestData{});
                AddComponent<ExecutorData>(entity);
            }
        }
    }
}

public struct ExecutorData : IComponentData
{

}