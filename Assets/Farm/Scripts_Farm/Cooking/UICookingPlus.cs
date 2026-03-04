using UnityEngine;
using UnityEngine.Rendering;

public class UICookingPlus : MonoBehaviour
{
    public GameObject cookingPanel; // Panel chứa giao diện nấu ăn
    private void Start()
    {
        HideCookingPanel(); // Ẩn panel khi bắt đầu
    }
    private void Update()
    {
        GameObject.Find("Player").GetComponent<P_Action>().isFarm = cookingPanel.activeSelf; // Khi mở panel nấu ăn thì player sẽ không thể di chuyển
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
