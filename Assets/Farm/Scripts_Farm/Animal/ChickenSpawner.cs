using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab;
    public ChickenListWrapper GetCurrentChickenData()
    {
        //bỏ vô wrapper để lưu firebase
        ChickenListWrapper wrapper = new ChickenListWrapper();
        ChicManager[] allChickens = FindObjectsByType<ChicManager>(FindObjectsSortMode.None);
        foreach (var chic in allChickens)
        {
            // Gọi hàm SaveChic() bên trong con gà để lấy dữ liệu (ChickenData)
            ChickenData data = chic.SaveChic();

            // Nhét dữ liệu đó vào danh sách
            wrapper.chickens.Add(data);
        }
        return wrapper;
    }
   

    // firebase gọi cái này để tạo gà mới từ data đã lưu
    public void SpawnChickenFromData(ChickenData data)
    {
        Vector3 pos = new Vector3(data.posX, data.posY, data.posZ);
        GameObject newChicken = Instantiate(chickenPrefab, pos, Quaternion.identity);

        // Gọi cái LoadData bên trong con gà để nó tự biến hình thành Baby/Teen
        newChicken.GetComponent<ChicManager>().LoadData(data);
    }

    // xóa gà cũ 
    public void ClearAllChickens()
    {
        ChicManager[] chickens = FindObjectsByType<ChicManager>(FindObjectsSortMode.None);
        foreach (var c in chickens) Destroy(c.gameObject);
    }
}
