using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HotFix;

public class GameManager : MonoBehaviour
{
    //public static Present present;

    void Awake()
    {

    }

    // °ó¶¨×é¼þ
    void BindAssets()
    {
        GameObject uiManager = new GameObject("UIManager");
        uiManager.transform.SetParent(this.transform);
        uiManager.AddComponent<UIManager>();
    }

    void CheckUpdate()
    {
    
    }
}