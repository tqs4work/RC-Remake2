using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingManager : MonoBehaviour
{
    [Header("Recipe Data")]
    public List<Recipe> allRecipeInGame; //  file recipe 
    public GameObject RecipeItemPrefab;      //  Prefab recipe 
    public Transform contentParent;        //  'Content' của Scroll View 
    private bool isCooking = false;//u can just cook only 1 dish at a time 
    public TextMeshProUGUI notiText; 
    //phát ra âm thanh khi nấu
    public AudioSource AudioSource;
    public AudioClip cookingAudioClip;
    void Start()
    {
        notiText.text = "";
        foreach (Recipe p in allRecipeInGame)
        {
            // Tạo nút mới bên trong Content
            GameObject newItem = Instantiate(RecipeItemPrefab, contentParent);

            // Gọi hàm Setup để hiển thị đúng tên/giá của công thức nấu ăn đó
            newItem.GetComponent<ChooseRecipe>().Setup(p,TryCook);
  
        }
    }
    public void TryCook(Recipe recipe, ChooseRecipe uiSlot) // truyen cai uiSlot chu yeu de co slider cua chooseRecipe
    {
        if (isCooking) return; //neu dang nau thi k nau nua 
        // Kiểm tra xem có đủ nguyên liệu trong túi không?
        if (CheckIfCanCook(recipe))
        {
            StartCoroutine(CookCoroutine(recipe,uiSlot));
        }
        else
        {
            StartCoroutine(ShowNotification("Không đủ nguyên liệu để nấu!", 1f));
            Debug.Log("Không đủ nguyên liệu để nấu!");
        }
    }
    IEnumerator CookCoroutine(Recipe recipe, ChooseRecipe uiSlot)
    {
        isCooking = true;
        ConsumeIngredients(recipe);
        if (AudioSource != null && cookingAudioClip != null)
        {
            AudioSource.clip = cookingAudioClip;
            AudioSource.loop = true; // Cho phép lặp lại nếu thời gian nấu dài hơn file âm thanh
            AudioSource.Play();
        }
        Slider targetSlider = uiSlot.timeslider;
        if (targetSlider != null)
        {
            targetSlider.gameObject.SetActive(true); // Bật lên
            targetSlider.maxValue = recipe.craftingTime;
            targetSlider.value = 0;
        }
        float timer = 0f;
        while (timer < recipe.craftingTime)
        {
            timer += Time.deltaTime;
            if (targetSlider != null) targetSlider.value = timer;
            yield return null;
        }
        if (AudioSource != null)
        {
            AudioSource.Stop();
        }
        AddResultItem(recipe);
        if (targetSlider != null) targetSlider.gameObject.SetActive(false); // Tắt đi sau khi nấu xong
        isCooking = false;
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
    private bool CheckIfCanCook(Recipe recipe)
    {
        var player = PlayerRuntime.Instance.Player;

        foreach (RecipeIngredient ing in recipe.ingredients)
        {
            // Chuyển Item ScriptableObject sang ItemRuntime để lấy ID và check loại túi
            ItemRuntime tempRuntime = ItemRuntime.FromItem(ing.item);
            InventoryContainerType type = GetContainerType(tempRuntime.itemID);

            // Lấy đúng cái túi chứa loại đồ đó
            var container = player.Inventory[type];

            // Tìm item trong túi xem có không (Dùng Linq tìm theo ID)
            var itemInBag = container.items.FirstOrDefault(x => x.itemID == tempRuntime.itemID);

            // Nếu không tìm thấy item HOẶC tìm thấy nhưng số lượng ít hơn yêu cầu -> Return false (Không nấu được)
            if (itemInBag == null || itemInBag.quantity < ing.quantity)
            {
                return false;
            }
        }
        return true; // Duyệt qua hết danh sách mà đủ cả thì trả về True
    }
    private void ConsumeIngredients(Recipe recipe)
    {
        var player = PlayerRuntime.Instance.Player;

        foreach (RecipeIngredient ing in recipe.ingredients)
        {
            ItemRuntime tempRuntime = ItemRuntime.FromItem(ing.item);
            InventoryContainerType type = GetContainerType(tempRuntime.itemID);
            var container = player.Inventory[type];

            // Tìm item trong túi để trừ
            var itemInBag = container.items.FirstOrDefault(x => x.itemID == tempRuntime.itemID);

            if (itemInBag != null)
            {
                itemInBag.quantity -= ing.quantity; // Trừ số lượng

                // Nếu xài hết sạch (về 0) thì xóa luôn khỏi list túi đồ cho gọn
                if (itemInBag.quantity <= 0)
                {
                    container.items.Remove(itemInBag);
                }
            }
        }
    }

    // 3. Hàm Thêm món ăn vào túi (Copy logic từ ItemPickup của bạn)
    private void AddResultItem(Recipe recipe)
    {

        // Tạo ItemRuntime mới từ món ăn thành phẩm
        ItemRuntime newItem = ItemRuntime.FromItem(recipe.resultItem);
        newItem.quantity = recipe.resultQuantity; // Gán số lượng nhận được

        var player = PlayerRuntime.Instance.Player;
        InventoryContainerType containerType = GetContainerType(newItem.itemID);
        var container = player.Inventory[containerType];

        // --- Logic Cộng dồn (Stacking) ---
        foreach (var invItem in container.items)
        {
            if (invItem.itemID == newItem.itemID && invItem.isStackable)
            {
                invItem.quantity += newItem.quantity;
                StartCoroutine(ShowNotification($"Bạn đã nấu được: {newItem.itemName} x{newItem.quantity}", 1f));
                return; // Cộng xong thì thoát hàm luôn
            }
            // Logic cho item không stack (tạo ID riêng để không bị trùng)
            else if (invItem.itemID == newItem.itemID && !invItem.isStackable)
            {
                newItem.itemID += " " + Random.Range(0f, 100f).ToString();
            }
        }

        // --- Logic Kiểm tra túi đầy ---
        if (container.maxSize > 0 && container.items.Count >= container.maxSize)
        {
            StartCoroutine(ShowNotification("Túi đầy rồi! Không nhận được món ăn.", 1f));
            Debug.Log("Túi đầy rồi! Không nhận được món ăn.");
            return;
        }

        // --- Thêm món mới vào túi ---
        container.items.Add(newItem);
        StartCoroutine(ShowNotification($"Bạn đã nấu được: {newItem.itemName} x{newItem.quantity}", 1f));


    }
    IEnumerator ShowNotification(string message, float duration)
    {
        notiText.text = message;
        yield return new WaitForSeconds(duration);
        notiText.text = "";
    }
}
