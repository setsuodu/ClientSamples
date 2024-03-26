using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosest : MonoBehaviour
{
    public GameObject whitePrefab;
    public GameObject blackPrefab;
    public int whiteCoumt = 1000;
    public int blackCoumt = 1000;
    //public List<RandomWalk> whiteBalls = new List<RandomWalk>();
    //public List<RandomWalk> blackBalls = new List<RandomWalk>();
    public KdTree<RandomWalk> whiteBalls = new KdTree<RandomWalk>();
    public KdTree<RandomWalk> blackBalls = new KdTree<RandomWalk>();

    void Start()
    {
        for (int i = 0; i < whiteCoumt; i++)
        {
            var obj = Instantiate(whitePrefab);
            var script = obj.GetComponent<RandomWalk>();
            whiteBalls.Add(script);
        }

        for (int i = 0; i < whiteCoumt; i++)
        {
            var obj = Instantiate(blackPrefab);
            var script = obj.GetComponent<RandomWalk>();
            blackBalls.Add(script);
        }
    }

    void Update()
    {
        //foreach (var whiteBall in whiteBalls)
        //{
        //    var nearestDist = float.MaxValue;
        //    RandomWalk nearestObj = null;

        //    foreach (var blackBall in blackBalls)
        //    {
        //        var dist = Vector3.Distance(whiteBall.transform.position, blackBall.transform.position);
        //        if (dist < nearestDist)
        //        {
        //            nearestDist = dist;
        //            nearestObj = blackBall;
        //        }
        //    }

        //    Debug.DrawLine(whiteBall.transform.position, nearestObj.transform.position, Color.red);
        //}

        blackBalls.UpdatePositions();

        foreach (var whiteBall in whiteBalls)
        {
            RandomWalk nearestObj = blackBalls.FindClosest(whiteBall.transform.position);

            Debug.DrawLine(whiteBall.transform.position, nearestObj.transform.position, Color.red);
        }
    }
}
