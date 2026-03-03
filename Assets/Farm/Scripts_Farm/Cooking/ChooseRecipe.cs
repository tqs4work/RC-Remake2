using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseRecipe : MonoBehaviour
{
    [Header("UI References/Result")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeToCook;
    public Button cookButton;
    private Recipe rep;

    [Header("Ingredient Display")]
    public Transform ingredientContainer; // dưới cái scroll view, nơi hiển thị nguyên liệu
    public GameObject ingredientPrefab;
    public void Setup(Recipe result)
    {
        rep = result;
        nameText.text = rep.resultItem.itemName;
        timeToCook.text = rep.craftingTime.ToString() + "s";
        if (rep.resultItem.icon != null)
        {
            iconImage.sprite = rep.resultItem.icon;
        }
        SpawnIngredients();
    }
    void SpawnIngredients()
    {
        // Xóa sạch icon cũ 
        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject);
        }

        // tìm món ăn xong duyệt nguyên liệu của món ăn 
        foreach (RecipeIngredient ing in rep.ingredients)
        {
            // Đẻ ra ô nguyên liệu con nằm trong cái khung Container
            GameObject ingSlot = Instantiate(ingredientPrefab, ingredientContainer);

            // Cài đặt hình ảnh và số lượng cho nó
            ingSlot.GetComponent<ChooseIngredient>().SetUpForIngredient(ing);
        }
    }

    // Gán vào sự kiện onclick của Button trong prefab ChooseRecipe
    public void OnSelect()
    {
        Debug.Log("Nút nấu ăn đã được nhấn!");
    }
}
