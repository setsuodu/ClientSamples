using System.Collections.Generic;
using UnityEngine;
using Peril.Physics;

public class DemoScript : MonoBehaviour
{

    public enum CollisionSystemType
    {
        Brute,
        QuadTree
    }

    public DemoPhysicsBody DemoPhysicsBody;

    [Header("CollisionSystem Settings")]
    public CollisionSystemType CSType;
    public int MaxBodies = 500;

    [Header("QuadTree Settings")]
    public Vector2 WorldSize = new Vector2(200, 200);
    public int BodiesPerNode = 6;
    public int MaxSplits = 6;

    public QuadTree _quadTree;
    private List<IQuadTreeBody> _quadTreeBodies = new List<IQuadTreeBody>();
    private CollisionSystemQuadTree _csQuad;
    private CollisionSystemBrute _csBrute = new CollisionSystemBrute();

    private void Start()
    {
        _quadTree = new QuadTree(new Rect(0, 0, WorldSize.x, WorldSize.y), BodiesPerNode, MaxSplits);
        _csQuad = new CollisionSystemQuadTree(_quadTree);

        for(int i = 0; i < MaxBodies; i++)
        {
            var body = GameObject.Instantiate<DemoPhysicsBody>(DemoPhysicsBody);
            body.transform.position = new Vector3(Random.Range(0, WorldSize.x), 0, Random.Range(0, WorldSize.y));
            _csBrute.AddBody(body);// add body to CollisionSystemBrute
            _csQuad.AddBody(body);
            _quadTree.AddBody(body); // add body to QuadTree
            _quadTreeBodies.Add(body); // cache bodies so we can refresh the tree in update
        }
    }

    private void Update()
    {
        switch(CSType)
        {
            case CollisionSystemType.Brute:
                _csBrute.Step();
                break;
            case CollisionSystemType.QuadTree:
                _csQuad.Step();
                break;
        }

        // refresh QuadTree each frame if bodies can move
        _quadTree.Clear();
        foreach(var b in _quadTreeBodies)
        {
            _quadTree.AddBody(b);
        }
    }

    private void OnDrawGizmos()
    {
        if (_quadTree == null) return;
        _quadTree.DrawGizmos();
    }

}
