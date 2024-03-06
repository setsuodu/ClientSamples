using System.Collections;
using UnityEngine;

public class ILGlobal : MonoBehaviour
{
    public static ILGlobal Get;

    void Awake()
    {
        Get = this;
    }

    public void GlobalInit()
    {
        StartCoroutine(LoadHotFixAssembly());
    }

    // 从ab包加载dll
    IEnumerator LoadHotFixAssembly()
    {
        yield return null;

        //byte[] dll = ABManager.LoadDLL();
        //var fs = new MemoryStream(dll);
        //appdomain.LoadAssembly(fs, null, null);

        //InitializeILRuntime();
        OnHotFixLoaded();
    }

    void OnHotFixLoaded()
    {
        // IL热更加载UI
        //appdomain.Invoke("HotFix.Main", "Init", gameObject, null); //static方法
        HotFix.Main.Init();
    }
}