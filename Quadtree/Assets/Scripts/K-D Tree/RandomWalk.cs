using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector3 newPos;

    void Start()
    {
        transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));

        InvokeRepeating("SetPos", -1, 3);
    }

    void Update()
    {
        //Vector3 v = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + v, Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * moveSpeed);
    }

    void SetPos()
    {
        Vector3 oldPos = transform.position;
        Vector3 v = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
        newPos = oldPos + v;
        //Debug.Log($"{oldPos} ¡ú {newPos}");
    }
}
