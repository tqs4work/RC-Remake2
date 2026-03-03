using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct RecipeIngredient
{
    public Item item;
    public int quantity;
}
[CreateAssetMenu(fileName = "Recipe_", menuName = "Farm/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Ingredients")]
    public List<RecipeIngredient> ingredients;
    [Header("Result")]
    public Item resultItem;
    public int resultQuantity;

    [Header("Crafting Time")]
    public float craftingTime; // Thời gian chế tạo (giây)
}