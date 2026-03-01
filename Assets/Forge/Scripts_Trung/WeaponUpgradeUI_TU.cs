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
    public Button btnUpgrade;
    public Button btnClose;

    [Header("Panels")]
    public GameObject confirmPanel;
    public Button btnYes;
    public Button btnNo;

    [Header("Dialogs")]
    public InfoDialog_TU dialogInfo;
    public InfoDialog_TU dialogSuccess;

    [Header("Effect")]
    public GameObject upgradeEffect;
    public float upgradeDuration = 1.2f;

    ItemRuntime selectedWeapon;
    ItemRuntime selectedStone;

    int weaponLevel = 0;

    void Start()
    {
        btnUpgrade.onClick.AddListener(OnClickUpgrade);
        btnYes.onClick.AddListener(DoUpgrade);
        btnNo.onClick.AddListener(() => confirmPanel.SetActive(false));
        btnClose.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
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

    // Khi click slot A/B
    public void OnSlotClicked(WeaponUpgradeSlotCell_TU slot)
    {
        if (slot.slotType == WeaponUpgradeSlotCell_TU.SlotType.Weapon)
        {
            selectedWeapon = slot.currentItem;
            weaponLevel = selectedWeapon.upgradeLevel;
        }
        else if (slot.slotType == WeaponUpgradeSlotCell_TU.SlotType.Stone)
        {
            selectedStone = slot.currentItem;
        }
    }

    void OnClickUpgrade()
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

        confirmPanel.SetActive(true);
    }

    void DoUpgrade()
    {
        confirmPanel.SetActive(false);
        StartCoroutine(CoUpgrade());
    }

    IEnumerator CoUpgrade()
    {
        if (upgradeEffect)
            upgradeEffect.SetActive(true);

        yield return new WaitForSeconds(upgradeDuration);

        if (upgradeEffect)
            upgradeEffect.SetActive(false);

        // 🔥 Tăng level
        weaponLevel++;
        selectedWeapon.upgradeLevel = weaponLevel;
        selectedWeapon.ApplyUpgradeVisual();
        selectedWeapon.itemData.GetFinalStats(
        selectedWeapon.upgradeLevel,
        out float finalAtk,
        out float finalDef,
        out float finalCrit);

        selectedWeapon.atk = finalAtk;
        selectedWeapon.def = finalDef;

        // 🔥 Xóa vũ khí khỏi túi (tạm thời)
        RemoveItemFromInventory(selectedWeapon);

        // 🔥 Preview ở slot C
        slotC.SetItem(selectedWeapon, weaponLevel);

        // 🔥 Xóa slot A/B
        slotA.Clear();
        slotB.Clear();

        dialogSuccess.Show(
            $"Cường hóa thành công +{weaponLevel}!",
            () =>
            {
                // Khi nhấn OK
                slotC.Clear();

                // Thêm lại vào inventory
                AddItemToInventory(selectedWeapon);

                FindObjectOfType<InventoryUI>().RefreshAll();
            });
    }

    void RemoveItemFromInventory(ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        foreach (var container in player.Inventory.Values)
        {
            if (container.items.Contains(item))
            {
                container.items.Remove(item);
                break;
            }
        }
    }

    void AddItemToInventory(ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        char prefix = item.itemID[0];

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
