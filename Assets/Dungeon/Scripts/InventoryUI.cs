using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot")]
    public Button slotPrefab;
    public int maxSlots = 27;

    [Header("Panels")]
    public GameObject toolPanel;
    public GameObject farmPanel;
    public GameObject cityPanel;
    public GameObject dungeonPanel;

    public bool isPanelOpen = false;

    [Header("Containers")]
    public Transform toolContainer;
    public Transform farmContainer;
    public Transform cityContainer;
    public Transform dungeonContainer;

    [Header("Reference")]
    public HotbarUI hotbarUI;


    void Start()
    {
        hotbarUI = Object.FindFirstObjectByType<HotbarUI>();
    }

    private void Update()
    {        
        isPanelOpen = toolPanel.activeSelf ||
                       farmPanel.activeSelf ||
                       cityPanel.activeSelf ||
                       dungeonPanel.activeSelf;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPanelOpen)
                HideAllPanels();
            else
                ShowToolPanel();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && isPanelOpen)
            ShowToolPanel();

        if (Input.GetKeyDown(KeyCode.Alpha2) && isPanelOpen)
            ShowFarmPanel();

        if (Input.GetKeyDown(KeyCode.Alpha3) && isPanelOpen)
            ShowCityPanel();

        if (Input.GetKeyDown(KeyCode.Alpha4) && isPanelOpen)
            ShowDungeonPanel();
    }

    public void HideAllPanels()
    {
        toolPanel.SetActive(false);
        farmPanel.SetActive(false);
        cityPanel.SetActive(false);
        dungeonPanel.SetActive(false);
        ItemTooltipUI_TU.Ensure()?.Hide();
    }

    public void ShowToolPanel()
    {
        HideAllPanels();
        toolPanel.SetActive(true);
        RenderItems(InventoryContainerType.Tool, toolContainer);
    }

    public void ShowFarmPanel()
    {
        HideAllPanels();
        farmPanel.SetActive(true);
        RenderItems(InventoryContainerType.Farm, farmContainer);
    }

    public void ShowCityPanel()
    {
        HideAllPanels();
        cityPanel.SetActive(true);
        RenderItems(InventoryContainerType.City, cityContainer);
    }

    public void ShowDungeonPanel()
    {
        HideAllPanels();
        dungeonPanel.SetActive(true);
        RenderItems(InventoryContainerType.Dungeon, dungeonContainer);
    }


    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f; // 0.3 gi�y

    void RenderItems(InventoryContainerType type, Transform parent)
    {        
        var playerInventory = PlayerRuntime.Instance.Player.Inventory;

        if (!playerInventory.ContainsKey(type))
            return;

        List<ItemRuntime> items = playerInventory[type].items;

        // Clear slot c?
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        for (int i = 0; i < maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, parent);
            slot.gameObject.SetActive(true);

            var bgImg = slot.transform.Find("BG").GetComponent<Image>();
            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            var amount = slot.transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            bgImg.gameObject.SetActive(false);
            iconImg.sprite = null;
            iconImg.enabled = false;
            amount.text = string.Empty;

            if (i < items.Count)
            {
                var item = items[i];

                iconImg.sprite = item.icon;
                iconImg.enabled = true;
                var hover = slot.GetComponent<ItemHoverHandler_TU>();
                if (hover == null)
                   hover = slot.gameObject.AddComponent<ItemHoverHandler_TU>();

                hover.Setup(item.itemData, item.upgradeLevel);
                hover.SetDurability(Mathf.RoundToInt(item.durability));

                hover.SetAnchor(slot.GetComponent<RectTransform>());

                if (item.isStackable && item.quantity >= 1)
                {
                    bgImg.gameObject.SetActive(true);
                    amount.text = item.quantity.ToString();
                }

                var itemCopy = item;
                var typeCopy = type;

                slot.onClick.RemoveAllListeners();

                slot.onClick.AddListener(() =>
                 {
                    if (Time.time - lastClickTime <= doubleClickThreshold)
                        {
                            Debug.Log("DOUBLE CLICK DETECTED");

                            ShopUI_TU shop = FindObjectOfType<ShopUI_TU>();

                            if (shop != null && shop.gameObject.activeSelf && shop.CurrentMode == ShopUI_TU.Mode.Sell)
                            {
                                shop.OpenSellPopup(itemCopy, typeCopy);
                                return;
                            }

                            if (RepairUI_TU.Instance != null &&
                                RepairUI_TU.Instance.gameObject.activeInHierarchy)
                            {
                                RepairUI_TU.Instance.repairSlotCell.SetItem(itemCopy);
                                return;
                            }

                            if (StoneUpgradeUI_TU.Instance != null &&
                                StoneUpgradeUI_TU.Instance.gameObject.activeInHierarchy)
                            {
                                StoneUpgradeUI_TU.Instance.TryPlaceStone(itemCopy);
                                return;
                            }

                            var upgradeUI = FindObjectOfType<WeaponUpgradeUI_TU>();
                            if (upgradeUI != null && upgradeUI.gameObject.activeInHierarchy)
                            {
                                if (itemCopy.itemType == ItemType.Stone)
                                {
                                    upgradeUI.slotB.SetItem(itemCopy);
                                    upgradeUI.OnSlotClicked(upgradeUI.slotB);
                                }
                                else if (itemCopy.itemType == ItemType.Sword ||
                                        itemCopy.itemType == ItemType.Armor ||
                                        itemCopy.itemType == ItemType.Bow)
                                {
                                    upgradeUI.slotA.SetItem(itemCopy, itemCopy.upgradeLevel);
                                    upgradeUI.OnSlotClicked(upgradeUI.slotA);
                                }
                                return;
                            }

                            MoveItemToHotbar(typeCopy, itemCopy);
                        }

                     lastClickTime = Time.time;
                 });

            }
        }
    }

    // ===== CHANGED: Rewrite theo container system =====
    void MoveItemToHotbar(InventoryContainerType fromContainer, ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        var hotbarContainer =
            player.Inventory[InventoryContainerType.Hotbar];

        // ===== CHANGED: ki?m tra c�n ch? tr?ng (maxSize = 6) =====
        if (hotbarContainer.maxSize > 0 &&
            hotbarContainer.items.Count >= hotbarContainer.maxSize)
        {
            Debug.Log("Hotbar Full");
            return;
        }

        // ===== CHANGED: d�ng MoveItem thay v� MoveToHotbar =====
        player.MoveItem(fromContainer,
                        InventoryContainerType.Hotbar,
                        item);

        hotbarUI.Refresh();
        RefreshAll();
    }
    //

    public void RefreshAll()
    {
        ItemTooltipUI_TU.Ensure()?.Hide();   
        if (toolPanel.activeSelf)
            ShowToolPanel();

        else if (farmPanel.activeSelf)
            ShowFarmPanel();

        else if (cityPanel.activeSelf)
            ShowCityPanel();

        else if (dungeonPanel.activeSelf)
            ShowDungeonPanel();
    }
}