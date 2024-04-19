using UnityEngine;
using Peril.Physics;

public class DemoPhysicsBody : MonoBehaviour, ICollisionBody, IQuadTreeBody
{

    private ICollisionShape _shape;
    private Color _gizmoColor = Color.green;

    private void Awake()
    {
        var collider = GetComponent<BoxCollider>();
        _shape = new BoxShape(collider.bounds, false);
        _shape.Center = transform.position;
    }

    private void Update()
    {
        _shape.Center = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 1.25f);
    }

    #region ICollisionBody

    public int RefId { get; set; }

    public bool Sleeping { get { return false; } }

    public ICollisionShape CollisionShape { get { return _shape; } }

    public void OnCollision(CollisionResult result, ICollisionBody other)
    {
        _gizmoColor = result.Type == CollisionType.Exit ? Color.green : Color.red;
    }

    #endregion

    #region IQuadTreeBody

    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.z); } }
    public bool QuadTreeIgnore { get { return false; } }

    #endregion

}
