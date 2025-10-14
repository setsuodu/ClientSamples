using UnityEngine;

public class CauseException : MonoBehaviour
{
    void Update()
    {
        // Press 'N' to trigger NullReferenceException
        if (Input.GetKeyDown(KeyCode.N))
        {
            GameObject nullObj = null;
            // Accessing a null object's property
            nullObj.transform.position = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            var obj = Resources.Load("Prefabs/xp");
            Instantiate(obj);
        }
    }
}