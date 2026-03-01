using System.Collections.Generic;
using TMPro;
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

    bool isPanelOpen = false;

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
        isPanelOpen = (toolPanel.activeSelf ||
                       farmPanel.activeSelf ||
                       cityPanel.activeSelf ||
                       dungeonPanel.activeSelf);

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

    void HideAllPanels()
    {
        toolPanel.SetActive(false);
        farmPanel.SetActive(false);
        cityPanel.SetActive(false);
        dungeonPanel.SetActive(false);
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

                if (item.isStackable && item.quantity > 1)
                {
                    bgImg.gameObject.SetActive(true);
                    amount.text = item.quantity.ToString();
                }

                var itemCopy = item;
                var typeCopy = type;

                slot.onClick.RemoveAllListeners();
                slot.onClick.AddListener(() =>
                {
                    MoveItemToHotbar(typeCopy, itemCopy);
                });

            }
        }
    }
    void MoveItemToHotbar(InventoryContainerType fromContainer, ItemRuntime item)
    {
        var player = PlayerRuntime.Instance.Player;

        for (int i = 0; i < player.Hotbar.slots.Length; i++)
        {
            if (player.Hotbar.slots[i] == null)
            {
                player.MoveToHotbar(fromContainer, item, i);
                break;
            }
        }

        hotbarUI.Refresh();
        RefreshAll();
    }

    public void RefreshAll()
    {
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