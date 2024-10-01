using UnityEngine;

public class AABB : MonoBehaviour
{
    private Color defaultColor = new Color(0, 1, 0, 0.5f);
    private Color intersectColor = new Color(1, 0, 0, 0.5f);

    public Transform[] objs;

    //public static bool Intersecting(Rect a, Rect b)
    //{
    //    // Basic AABB collision detection. All of these must be true for there to be a collision.
    //    bool comp1 = a.yMin > b.yMax;
    //    bool comp2 = a.yMax < b.yMin;
    //    bool comp3 = a.xMin < b.xMax;
    //    bool comp4 = a.xMax > b.xMin;

    //    // This will only return true if all are true.
    //    return comp1 && comp2 && comp3 && comp4;
    //}
    public bool Intersect(Rect a, Rect b)
    {
        return (a.xMin <= b.xMax && a.xMax >= b.xMin) &&
               (a.yMin <= b.yMax && a.yMax >= b.yMin);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = defaultColor;
        //Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0));

        for (int i = 0; i < objs.Length; i++)
        {
            for (int t = i + 1; t < objs.Length; t++)
            {
                var bodyA = Transform2Rect(objs[i]);
                var bodyB = Transform2Rect(objs[t]);

                if (Intersect(bodyA, bodyB))
                {
                    Gizmos.color = intersectColor;
                }
            }
        }

        for (int i = 0; i < objs.Length; i++)
        {
            DrawRect(objs[i], new Vector2(1, 1), 1f);
        }
    }

    private void DrawRect(Transform trans, Vector2 size, float thikness)
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = trans.localToWorldMatrix;

        Gizmos.DrawCube(new Vector3(1, 1, 0) * size.x / 2, new Vector3(thikness, size.y, 0.01f));

        Gizmos.matrix = matrix;
    }

    Rect Transform2Rect(Transform obj)
    {
        var rect = new Rect();
        rect.position = obj.position;
        rect.width = 1;
        rect.height = 1;
        //Debug.Log($"[{obj.name}] rect: {rect}, pos: {rect.position}, center: {rect.center}");
        return rect;
    }
}