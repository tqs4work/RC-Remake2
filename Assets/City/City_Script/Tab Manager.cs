using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header("=== DANH SÁCH TAB ===")]
    public Image[] tabsImages;
    public GameObject[] pages;

    // THÊM DÒNG NÀY – RẤT QUAN TRỌNG!
    //[Header("=== KẾT NỐI VỚI ITEM MANAGER ===")]
    //public ItemManager itemManager; // Kéo ItemManager vào đây trong Inspector

    void Start()
    {
        ActiveTab(0);
    }

    public void ActiveTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == tabNo); // gọn hơn
            tabsImages[i].color = i == tabNo ? Color.white : Color.gray;
        }

        // THÊM ĐOẠN NÀY – CHỈ 4 DÒNG THÔI!
        // if (itemManager != null)
        // {
        //     if (tabNo == 1) 
        //         itemManager.SwitchToBuySection();
        //     else if (tabNo == 2) 
        //         itemManager.SwitchToSellSection();
        // }
    }
}
