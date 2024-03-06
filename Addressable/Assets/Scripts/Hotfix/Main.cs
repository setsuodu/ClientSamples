using UnityEngine;

namespace HotFix
{
    public class Main
    {
        public static void Init()
        {
            GameObject uiManager = new GameObject("UIManager");
            uiManager.transform.SetParent(GameManager.Get.gameObject.transform);
            uiManager.AddComponent<UIManager>();

            // ���ص�һ��UI
            UIManager.Get().Push<UI_Login>();
        }
    }
}