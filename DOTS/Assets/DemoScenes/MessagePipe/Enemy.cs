using MessagePipe;
using System;
using UnityEngine;
using VContainer;

public class Enemy : MonoBehaviour
{
    [Inject] ISubscriber<PlayerAttackData> attackSubscriber;

    private IDisposable subscription;

    void Start()
    {
        var bag = DisposableBag.CreateBuilder();
        attackSubscriber.Subscribe(attack =>
        {
            if (Vector3.Distance(transform.position, attack.Position) <= attack.Radius)
            {
                Debug.Log($"Enemy 受到伤害: {attack.Damage}");
                // 处理伤害逻辑
            }
        }).AddTo(bag);

        subscription = bag.Build();
    }

    void OnDestroy()
    {
        subscription?.Dispose();
    }
}