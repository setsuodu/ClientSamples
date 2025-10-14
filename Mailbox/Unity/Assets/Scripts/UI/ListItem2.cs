using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem2 : MonoBehaviour
    {
        public Text mNameText;
        public Text mTimeText;
        public Image mIcon;
        public Button mSelfBtn;
        public GameObject mContentRootObj;
        int mItemDataIndex = -1;
        public LoopListView2 mLoopListView;

        public void Init()
        {
            //Debug.Log($"Init: {gameObject.name}"); //只有第一次执行
            mSelfBtn.onClick.RemoveAllListeners();
            mSelfBtn.onClick.AddListener(OnItemBtnClicked);
        }

        public void SetItemData(ItemData itemData, int itemIndex)
        {
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mTimeText.text = itemData.mTime;
        }

        void OnItemBtnClicked()
        {
            Debug.Log("阅读邮件");
        }
    }
}