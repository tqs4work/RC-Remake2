using System.Collections.Generic;
using UnityEngine;

public class SeedSet : MonoBehaviour
{
    public List<PlantData> allPlantsInGame; //  file PlantData 
    public GameObject seedItemPrefab;      //  Prefab ChooseSeedui 
    public Transform contentParent;        //  'Content' của Scroll View 

    void Start()
    {
        foreach (PlantData p in allPlantsInGame)
        {
            // Tạo nút mới bên trong Content
            GameObject newItem = Instantiate(seedItemPrefab, contentParent);

            // Gọi hàm Setup để hiển thị đúng tên/giá của cây đó
            newItem.GetComponent<ChooseSeedui>().Setup(p);
        }
    }
}
