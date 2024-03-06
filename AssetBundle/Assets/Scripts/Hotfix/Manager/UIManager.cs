using System.Collections.Generic;
using UnityEngine;

namespace HotFix
{
    public class UIManager : MonoBehaviour
    {
        static UIManager _instance;
        public static UIManager Get()
        {
            return _instance;
        }

        private Transform Parent;
        //private Transform Top;

        // UI存储栈
        private Dictionary<string, UIBase> stack; //显示的
        private Dictionary<string, UIBase> recyclePool; //隐藏的

        void Awake()
        {
            _instance = this;
            Parent = GameObject.Find("Canvas").transform;
            stack = new Dictionary<string, UIBase>();
            recyclePool = new Dictionary<string, UIBase>();
        }

        public UIBase GetActiveUI()
        {
            var child = Parent.GetChild(Parent.childCount - 1);
            //Debug.Log($"GetActive: {child.name}");
            string scriptName = child.name;

            UIBase ui = null;
            if (stack.TryGetValue(scriptName, out ui) == false)
            {
                Debug.LogError($"还没有创建：{scriptName}");
                return null;
            }
            return ui.GetComponent<UIBase>();
        }

        public T GetUI<T>() where T : UIBase
        {
            string scriptName = typeof(T).ToString().Replace("HotFix.", "");
            //Debug.Log($"GetUI: {scriptName}");
            UIBase ui = null;
            if (stack.TryGetValue(scriptName, out ui) == false)
            {
                if (recyclePool.TryGetValue(scriptName, out ui) == false)
                {
                    Debug.LogError($"还没有创建：{scriptName}");
                    return null;
                }
                else
                {
                    Debug.Log($"{scriptName}处于未激活状态");
                }
            }
            return ui.GetComponent<T>();
        }

        public T Push<T>(int layer = 1) where T : UIBase
        {
            string fullName = typeof(T).ToString();
            string scriptName = string.Empty;
            if (fullName.Contains("."))
            {
                scriptName = fullName.Split('.')[1];
            }
            else
            {
                scriptName = fullName;
            }
            //Debug.Log($"Push<{scriptName}>");
            UIBase ui = null;
            if (stack.TryGetValue(scriptName, out ui))
            {
                ui.transform.SetAsLastSibling(); //提到最前面显示
                return ui.GetComponent<T>();
            }
            if (recyclePool.TryGetValue(scriptName, out ui))
            {
                recyclePool.Remove(scriptName);
                stack.Add(scriptName, ui);
                //Debug.Log($"<color=yellow>[ReUse]{scriptName} stack:{stack.Count}/recycle:{recyclePool.Count}</color>");
                ui.gameObject.SetActive(true);
                ui.transform.SetAsLastSibling(); //提到最前面显示
                return ui.GetComponent<T>();
            }
            else
            {
                GameObject prefab = ABManager.LoadPrefab($"UI/{scriptName}"); //iOS区分大小写？
                GameObject obj = Instantiate(prefab, Parent);
                obj.transform.localPosition = Vector3.zero;
                obj.name = scriptName;

                if (obj.GetComponent<T>() == false)
                    obj.AddComponent<T>();
                var script = obj.GetComponent<T>();
                stack.Add(scriptName, script);
                //Debug.Log($"<color=yellow>[New]{scriptName} stack:{stack.Count}/recycle:{recyclePool.Count}</color>");
                return script;
            }
        }

        public void Pop(UIBase ui)
        {
            string scriptName = ui.name;
            if (ui == null)
            {
                Debug.LogError("没有需要销毁的UI");
                return;
            }
            if (stack.ContainsKey(scriptName) == false)
            {
                Debug.LogError($"没有需要销毁的UI：{scriptName}");
                return;
            }
            stack.Remove(scriptName);
            recyclePool.Add(scriptName, ui);
            ui.gameObject.SetActive(false);
        }
        public void PopAll()
        {
            foreach (var item in stack)
            {
                //Debug.Log($"{item.Key}---{item.Value.gameObject}");
                recyclePool.Add(item.Key, item.Value);
                item.Value.gameObject.SetActive(false);
            }
            stack.Clear();
        }
    }
}