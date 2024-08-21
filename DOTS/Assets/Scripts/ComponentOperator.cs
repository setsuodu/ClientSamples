using UnityEngine;
using Unity.Entities;

public class ComponentOperator : MonoBehaviour
{
    class ComponentOperatorBaker : Baker<ComponentOperator>
    {
        public override void Bake(ComponentOperator authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new GameObj());

            AddBuffer<AddDataElem>(entity);
        }
    }
}

public struct GameObj : IComponentData
{
    public float test;
}

// 用来测试运行时添加的组件
public struct AddData : IComponentData
{
    public float test;
}

// 数组（DOTS中为了排列内存，不能用List/Array等变长数据）
//[InternalBufferCapacity(128)] //不写时，默认是128字节，但一般是浪费的
[InternalBufferCapacity(8)] //尽量设置成合适的值
public struct AddDataElem : IBufferElementData
{
    public float test;
}

public partial struct ComOpSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameObj>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; //关闭/让函数只执行一次

        var obj = SystemAPI.GetSingletonEntity<GameObj>();

        //1.用entity manager添加（不推荐）
        //EntityManager em = state.EntityManager;
        //em.AddComponent<AddData>(obj);

        //2.用cmd添加（推荐，在各个Group生命周期之间，见缝插针执行缓冲区的命令）
        var sing = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //指定在BeginSimulationEntity之前插入命令
        EntityCommandBuffer buffer = sing.CreateCommandBuffer(state.WorldUnmanaged);
        buffer.AddComponent<AddData>(obj);
        //buffer.Playback(state.EntityManager); //单线程不用加。

        // 删除组件
        buffer.RemoveComponent<AddData>(obj);
    }
}