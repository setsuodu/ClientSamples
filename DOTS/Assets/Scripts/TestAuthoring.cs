using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class TestAuthoring : MonoBehaviour
{
    public float number;

    // 烘焙 Mono数据 → DOTS数据
    class TestBaker : Baker<TestAuthoring>
    {
        public override void Bake(TestAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TestData
            {
                m_Number = authoring.number,
            });
        }
    }
}

// 继承IComponentData，普通结构体 变为 DOTS的结构体
// 结构体是现实在Runtime上的组件，因为DOTS本身就全是组件化结构体构成的
public struct TestData : IComponentData
{
    public float m_Number;
}

// System控制生命周期
// System不依赖场景中的Object，即使没有创建也会走生命周期
// System只有OnUpdate函数，用到LateUpdate，FixedUpdate需要加[UpdateInGroup]标签
//[UpdateInGroup(typeof(SimulationSystemGroup))] //→ Update
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] //→ FixedUpdate
public partial struct TestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TestData>(); //确保当前场景内有TestData，才执行Update。

        state.RequireForUpdate<ExecutorData>(); //外部开关
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        // 获取场景中所有的TestData
        // RefRO=只读，RefRW=读写，可修改
        // 单一条件搜索
        //foreach (var test in SystemAPI.Query<RefRO<TestData>>())
        //{
        //    float speed = test.ValueRO.m_Number;
        //}

        // Scene世界的值，托管类型，访问低效
        //float delta = Time.deltaTime;
        float deltaS = SystemAPI.Time.DeltaTime;

        // 双个条件搜索
        foreach (var (test,trans) in SystemAPI.Query<RefRO<TestData>, RefRW<LocalTransform>>())
        {
            float speed = test.ValueRO.m_Number;
            trans.ValueRW = trans.ValueRO.RotateY(speed * deltaS);
            //Debug.Log($"OnUpdate : {speed * deltaS}");
        }
    }
}