using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class RandomMachine : MonoBehaviour
{
    [Header("Item Config")]
    [SerializeField] private Item[] items;
    [SerializeField] private Image[] slots;        // Đổi GameObject[] thành Image[]
    [SerializeField] private Image resultImage;
    private int playCost = 50;

    [Header("Animation Config")]
    [SerializeField] private float rollDuration = 3f;
    [SerializeField] private float rollSpeed = 0.1f;

    private bool isRolling = false;

    public void Play()
    {
        if (isRolling) return;

        if (!CurrencyMana.Instance.SpendCoin(playCost))
        {
            Debug.Log("Không đủ coin!");
            return;
        }
        Debug.Log("còn lại: " + PlayerRuntime.Instance.Player.Gold + " coin");

        StartCoroutine(PlayAnimation());

    }

    public void AddItem (Item item)
    {
        ItemRuntime newItem = ItemRuntime.FromItem(item);
        var player = PlayerRuntime.Instance.Player;

        InventoryContainerType containerType =
            GetContainerType(newItem.itemID);

        var container = player.Inventory[containerType];

        // ======================================================
        // ===== 1?? KI?M TRA STACK TR??C =======================
        // ======================================================

        foreach (var invItem in container.items)
        {
            if (invItem.itemID == newItem.itemID &&
                invItem.isStackable)
            {
                if(invItem.quantity + newItem.quantity > invItem.maxStack)
                {
                    int spaceLeft = invItem.maxStack - invItem.quantity;
                    invItem.quantity += spaceLeft;
                    newItem.quantity -= spaceLeft;
                    newItem.itemID += " " + Random.Range(0f, 100f).ToString();
                    Debug.Log($"Picked up: {newItem.itemName} (Partially Stacked)");
                }
                else
                {
                    invItem.quantity += newItem.quantity;                    
                    Debug.Log($"Picked up: {newItem.itemName} (Stacked)");
                    //Destroy(gameObject);
                    return;
                }                
                
            }
            else if (invItem.itemID == newItem.itemID &&
                     !invItem.isStackable)
            {
                newItem.itemID += " " + Random.Range(0f,100f).ToString();
                //Destroy(gameObject);                
            }
        }

        // ======================================================
        // ===== 2?? KI?M TRA FULL TR??C KHI ADD ================
        // ======================================================

        if (container.maxSize > 0 &&
            container.items.Count >= container.maxSize)
        {
            Debug.Log("Inventory Full! Cannot pick up.");
            return; // ? KH�NG Destroy ? v?n n?m tr�n ??t
        }

        // ======================================================
        // ===== 3?? ADD ITEM M?I ===============================
        // ======================================================

        container.items.Add(newItem);

        Debug.Log($"Picked up: {newItem.itemName}");
        //Destroy(gameObject);
    }

    private InventoryContainerType GetContainerType(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return InventoryContainerType.Tool;

        char prefix = itemID[0];

        return prefix switch
        {
            'T' => InventoryContainerType.Tool,
            'F' => InventoryContainerType.Farm,
            'C' => InventoryContainerType.City,
            'D' => InventoryContainerType.Dungeon,
            _ => InventoryContainerType.Tool
        };
    }

    private IEnumerator PlayAnimation()
    {
        isRolling = true;

        float timer = 0f;

        while (timer < rollDuration)
        {
            foreach (var slot in slots)
            {
                slot.color = new Color(
                    Random.value,
                    Random.value,
                    Random.value
                );
            }

            yield return new WaitForSeconds(rollSpeed);
            timer += rollSpeed;
        }

        // Reset màu về trắng
        foreach (var slot in slots)
        {
            slot.color = Color.white;
        }

        // Random kết quả thật
        int randomIndex = Random.Range(0, items.Length);
        Item selectedItem = items[randomIndex];

        resultImage.sprite = selectedItem.icon;
        resultImage.enabled = true;

        CitySoundManager.Instance.PlayPrizeMachineMusic();

        Debug.Log($"Bạn nhận được: {selectedItem.itemName}");

        isRolling = false;

        AddItem(selectedItem);
    }

    
}