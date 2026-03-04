using UnityEngine;
using UnityEngine.UI;

public class RandomMachine : MonoBehaviour
{
    [Header("Item Config")]
    [SerializeField] private Item[] items;     // 4 ScriptableObject
    [SerializeField] private Image resultImage;  // Ô trắng hiển thị kết quả
    [SerializeField] private int playCost = 50;

    public void Play()
    {
        // 1. Kiểm tra tiền
        if (!CurrencyMana.Instance.SpendCoin(playCost))
        {
            Debug.Log("Không đủ coin!");
            return;
        }

        // 2. Random item
        int randomIndex = Random.Range(0, items.Length);
        Item selectedItem = items[randomIndex];
        Debug.Log($"Bạn nhận được: {selectedItem.itemName}");

        // 3. Hiển thị icon
        resultImage.sprite = selectedItem.icon;
        resultImage.enabled = true;
        Debug.Log("Kết quả đã hiển thị trên UI.");
    }
}