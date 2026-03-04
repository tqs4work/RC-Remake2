using System.Collections;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab;

    void Start()
    {
        StartCoroutine(AutoSaveRoutine());
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    GetCurrentChickenData();
        //}
    }
    private void Awake()
    {
        var chicList = PlayerRuntime.Instance.Player.ChickenWrapper;
        // Kiểm tra xem danh sách có dữ liệu không mới đẻ
        if (chicList != null && chicList.Count > 0)
        {
            foreach (ChickenData data in chicList)
            {
                SpawnChickenFromData(data); // Đẻ từng con một
            }
            Debug.Log("Đã load thành công đàn gà từ PlayerRuntime!");
        }
        else
        {
            Debug.Log("Chưa có dữ liệu gà để load.");
        }
    }
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
        PlayerRuntime.Instance.Player.ChickenWrapper = wrapper.chickens; // Cập nhật dữ liệu vào PlayerRuntime
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


    IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            GetCurrentChickenData();
            
        }
    }
}
