using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WeaponUpgradeUI_TU : MonoBehaviour
{
    public System.Action onClose;
     [Header("Slots")]
    public WeaponUpgradeSlotCell_TU slotA;
    public WeaponUpgradeSlotCell_TU slotB;
    public WeaponUpgradeSlotCell_TU slotC;

    [Header("Buttons")]
    public Button btnClose;

    [Header("Panels")]
    public GameObject confirmPanel;
    public TMP_Text confirmText;
    public Button btnYes;
    public Button btnNo;

    [Header("Dialogs")]
    public InfoDialog_TU dialogInfo;
    public InfoDialog_TU dialogSuccess;

    [Header("Effect")]
    public GameObject upgradeEffect;
    public float upgradeDuration = 1.2f;
    [Header("Sound")]
    public AudioSource upgradeSound;

    ItemRuntime selectedWeapon;
    ItemRuntime selectedStone;

    int weaponLevel = 0;

    void Start()
    {
        btnYes.onClick.AddListener(DoUpgrade);
        btnNo.onClick.AddListener(() => confirmPanel.SetActive(false));
        btnClose.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        if (upgradeEffect)
            upgradeEffect.SetActive(false); 
        ClearAll();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        ClearAll();
        onClose?.Invoke();
    }

    void ClearAll()
    {
        slotA.Clear();
        slotB.Clear();
        slotC.Clear();

        selectedWeapon = null;
        selectedStone = null;
        weaponLevel = 0;

        confirmPanel.SetActive(false);
    }

    public void OnSlotClicked(WeaponUpgradeSlotCell_TU slot)
    {
        if (slot.slotType == WeaponUpgradeSlotCell_TU.SlotType.Weapon)
        {
            if (slot.currentItem.itemType != ItemType.Sword &&
            slot.currentItem.itemType != ItemType.Armor && slot.currentItem.itemType != ItemType.Bow)
            {
                dialogInfo.Show("Chỉ có thể đặt vũ khí hoặc áo giáp.");
                return;
            }

            if (slot.currentItem.upgradeLevel >= 5)
            {
                dialogInfo.Show("Vật phẩm đã đạt cấp tối đa.");
                return;
            }

            selectedWeapon = slot.currentItem;
            weaponLevel = selectedWeapon.upgradeLevel;

            TryShowConfirm(); // gọi luôn
        }
        else if (slot.slotType == WeaponUpgradeSlotCell_TU.SlotType.Stone)
        {
            if (slot.currentItem.itemType != ItemType.Stone)
            {
                dialogInfo.Show("Chỉ có thể đặt đá cường hóa.");
                return;
            }

            selectedStone = slot.currentItem;

            // nếu đã có weapon thì kiểm tra upgrade luôn
            if (selectedWeapon != null)
            {
                TryShowConfirm();
            }
        }
    }
    void TryShowConfirm()
    {
        if (selectedWeapon == null)
        {
            dialogInfo.Show("Hãy chọn vũ khí.");
            return;
        }

        if (weaponLevel >= 5)
        {
            dialogInfo.Show("Vũ khí đã đạt cấp tối đa.");
            return;
        }

        if (selectedStone == null)
        {
            dialogInfo.Show("Hãy chọn đá nâng cấp.");
            return;
        }

        if (!CheckStoneRequirement())
            return;

        int price = selectedWeapon.itemData.upgradePrice[weaponLevel];

        if (PlayerRuntime.Instance.Player.Gold < price)
        {
            dialogInfo.Show("Không đủ vàng để cường hóa.");
            return;
        }

        confirmText.text =
        $"Cường hóa {selectedWeapon.itemName} +{weaponLevel} → +{weaponLevel+1}\n" +
        $"Chi phí: {price} Gold\nBạn có muốn tiếp tục?";

        confirmPanel.SetActive(true);
    }

    bool CheckStoneRequirement()
    {
        int[] stoneNeed = { 1, 2, 1, 2, 1 };
        int[] stoneLevel = { 1, 1, 2, 2, 3 };

        int needStone = stoneNeed[weaponLevel];
        int needLevel = stoneLevel[weaponLevel];
        if (selectedStone.upgradeLevel != needLevel)
        {
            dialogInfo.Show($"Cần đá cấp {needLevel}.");
            return false;
        }

        if (selectedStone.quantity < needStone)
        {
            dialogInfo.Show($"Cần {needStone} đá.");
            return false;
        }

        return true;
    }
    void DoUpgrade()
    {
        confirmPanel.SetActive(false);
        StartCoroutine(CoUpgrade());
    }

    IEnumerator CoUpgrade()
    {
        int price = selectedWeapon.itemData.upgradePrice[weaponLevel];
        PlayerRuntime.Instance.Player.Gold -= price;
        if (upgradeEffect)
        {
            upgradeEffect.SetActive(true);
        }

        if (upgradeSound && upgradeSound.clip != null)
        {
            upgradeSound.PlayOneShot(upgradeSound.clip);
        }
        yield return new WaitForSeconds(upgradeDuration);

        if (upgradeEffect)
        upgradeEffect.SetActive(false);

        int[] stoneNeed = { 1, 2, 1, 2, 1 };
        int[] stoneLevel = { 1, 1, 2, 2, 3 };

        int needStone = stoneNeed[weaponLevel];
        int needLevel = stoneLevel[weaponLevel];

        // Trừ stone
        selectedStone.quantity -= needStone;

        if (selectedStone.quantity <= 0)
        {
          RemoveItemFromInventory(selectedStone);
            slotB.Clear();
            selectedStone = null;
        }
        else
        {
            slotB.Clear();
        }
        weaponLevel++;
        selectedWeapon.upgradeLevel = weaponLevel;
        selectedWeapon.icon = selectedWeapon.itemData.upgradeIcons[weaponLevel];
        selectedWeapon.ApplyUpgradeVisual();
        selectedWeapon.itemData.GetFinalStats(
        selectedWeapon.upgradeLevel,
        out float finalAtk,
        out float finalDef,
        out float finalCrit);

        selectedWeapon.atk = finalAtk;
        selectedWeapon.def = finalDef;

        // Xóa vũ khí khỏi túi (tạm thời)
        RemoveItemFromInventory(selectedWeapon);

        // Preview ở slot C
        slotC.SetItem(selectedWeapon, weaponLevel);

        // Xóa slot A/B
        slotA.Clear();
        slotB.Clear();

        ItemRuntime upgradedWeapon = selectedWeapon;

        dialogSuccess.Show(
            $"Cường hóa thành công +{weaponLevel}!",
            () =>
            {
                slotC.Clear();

                AddItemToInventory(upgradedWeapon);   // ✅ dùng biến tạm

                FindObjectOfType<InventoryUI>().RefreshAll();
            });

        selectedWeapon = null;
        selectedStone = null;
    }

    void RemoveItemFromInventory(ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        foreach (var container in player.Inventory.Values)
        {
            if (container.items.Contains(item))
            {
                // Nếu item stackable thì chỉ xóa khi quantity = 0
                if (item.isStackable)
                {
                    if (item.quantity <= 0)
                        container.items.Remove(item);
                }
                else
                {
                    container.items.Remove(item);
                }

                break;
            }
        }
    }

    void AddItemToInventory(ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        char prefix = item.itemID[0];
        if (item == null) return;

        InventoryContainerType type = prefix switch
        {
            'T' => InventoryContainerType.Tool,
            'F' => InventoryContainerType.Farm,
            'C' => InventoryContainerType.City,
            'D' => InventoryContainerType.Dungeon,
            _ => InventoryContainerType.Tool
        };

        player.Inventory[type].items.Add(item);
    }
    
}
