using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    [Header("Recipe Data")]
    public List<Recipe> allRecipeInGame; //  file recipe 
    public GameObject RecipeItemPrefab;      //  Prefab recipe 
    public Transform contentParent;        //  'Content' của Scroll View 

    void Start()
    {
        foreach (Recipe p in allRecipeInGame)
        {
            // Tạo nút mới bên trong Content
            GameObject newItem = Instantiate(RecipeItemPrefab, contentParent);

            // Gọi hàm Setup để hiển thị đúng tên/giá của công thức nấu ăn đó
            newItem.GetComponent<ChooseRecipe>().Setup(p);
  
        }
    }
}
