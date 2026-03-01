using UnityEngine;
using UnityEngine.UI;

public class HarvestPopupUI : MonoBehaviour
{
    public Button harvestBtn;
    private Vector3Int cropPosition;
    private FarmController_TA farmManager;

    // Khi cây chín sẽ có cái nút hiện ra và hàm này sẽ chạy ngay lúc ấy 
    public void Setup(Vector3Int pos, FarmController_TA farm) // truyền vị trí và script farm vô 
    {
        cropPosition = pos;
        farmManager = farm;

    }

    public void OnHarvestClicked()
    {
        // gọi cái lệnh thu hoạch bên cái FarmController_TA 
        farmManager.HarvestCrop(cropPosition);
        Debug.Log("Thu hoạch thành công");

        // xóa ui khi thu hoạch xong 
        Destroy(gameObject);
    }
}