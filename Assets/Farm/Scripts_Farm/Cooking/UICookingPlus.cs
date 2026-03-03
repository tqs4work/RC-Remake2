using UnityEngine;

public class UICookingPlus : MonoBehaviour
{
    public GameObject cookingPanel; // Panel chứa giao diện nấu ăn
    private void Start()
    {
        HideCookingPanel(); // Ẩn panel khi bắt đầu
    }

    public void ShowCookingPanel()
    {
        cookingPanel.SetActive(true);
    }
    public void HideCookingPanel()
    {
        cookingPanel.SetActive(false);
    }
}
