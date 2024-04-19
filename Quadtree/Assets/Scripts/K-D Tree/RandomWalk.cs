using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    public Vector4 bound = new Vector4(-100, 100, -100, 100);
    public float moveSpeed = 5;
    Vector3 newPos;

    void Start()
    {
        transform.position = new Vector3(Random.Range(bound.x, bound.y), 0, Random.Range(bound.z, bound.w));

        InvokeRepeating("SetPos", -1, 3);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * moveSpeed);
    }

    void SetPos()
    {
        Vector3 oldPos = transform.position;
        Vector3 v = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
        newPos = oldPos + v;
    }
}
