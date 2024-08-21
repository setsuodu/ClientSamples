using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class TestAuthoring : MonoBehaviour
{
    public float number;

    // �決 Mono���� �� DOTS����
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

// �̳�IComponentData����ͨ�ṹ�� ��Ϊ DOTS�Ľṹ��
// �ṹ������ʵ��Runtime�ϵ��������ΪDOTS�����ȫ��������ṹ�幹�ɵ�
public struct TestData : IComponentData
{
    public float m_Number;
}

// System������������
// System�����������е�Object����ʹû�д���Ҳ������������
public partial struct TestSystem : ISystem
{
    // 
    public void OnCreate(ref SystemState state)
    {
    
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        // ��ȡ���������е�TestData
        // RefRO=ֻ����RefRW=��д�����޸�
        // ��һ��������
        //foreach (var test in SystemAPI.Query<RefRO<TestData>>())
        //{
        //    float speed = test.ValueRO.m_Number;
        //}

        // Scene�����ֵ���й����ͣ����ʵ�Ч
        //float delta = Time.deltaTime;
        float deltaS = SystemAPI.Time.DeltaTime;

        // ˫����������
        foreach (var (test,trans) in SystemAPI.Query<RefRO<TestData>, RefRO<LocalTransform>>())
        {
            float speed = test.ValueRO.m_Number;
            trans.ValueRO.RotateY(speed * deltaS);
        }
    }
}