using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseIngredient : MonoBehaviour
{
    [Header("UI References/Ingredient")]
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    

    public void SetUpForIngredient(RecipeIngredient ingredient)
    {
        quantityText.text = "x" + ingredient.quantity.ToString();
        if (ingredient.item.icon != null)
        {
            iconImage.sprite = ingredient.item.icon;
        }
        
    }
}
