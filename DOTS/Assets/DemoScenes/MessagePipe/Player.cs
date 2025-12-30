using MessagePipe;
using UnityEngine;
using VContainer;

public class Player : MonoBehaviour
{
    [Inject] IPublisher<PlayerAttackData> attackPublisher;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attackPublisher.Publish(new PlayerAttackData
            {
                Position = transform.position,
                Radius = 5f,
                Damage = 10
            });
        }
    }
}