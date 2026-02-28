using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot")]
    public Button slotPrefab;
    public int maxSlots = 25;

    [Header("Panels")]
    public GameObject toolPanel;
    public GameObject farmPanel;
    public GameObject cityPanel;
    public GameObject dungeonPanel;

    [Header("Containers")]
    public Transform toolContainer;
    public Transform farmContainer;
    public Transform cityContainer;
    public Transform dungeonContainer;

    private List<ItemRuntime> inventory;

    void Start()
    {
        inventory = PlayerRuntime.Instance.Player.Inventory;        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (toolPanel.activeSelf || farmPanel.activeSelf || cityPanel.activeSelf || dungeonPanel.activeSelf)
                HideAllPanels();
            else
                ShowToolPanel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {            
            ShowToolPanel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowFarmPanel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowCityPanel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ShowDungeonPanel();
        }
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
        RenderItems(toolPanel);
    }

    public void ShowFarmPanel()
    {
        HideAllPanels();
        farmPanel.SetActive(true);
        RenderItems(farmPanel);
    }

    public void ShowCityPanel()
    {
        HideAllPanels();
        cityPanel.SetActive(true);
        RenderItems(cityPanel);
    }

    public void ShowDungeonPanel()
    {
        HideAllPanels();
        dungeonPanel.SetActive(true);
        RenderItems(dungeonPanel);
    }

    void RenderItems(GameObject targetPanel)
    {
        inventory = PlayerRuntime.Instance.Player.Inventory;

        Transform parent = GetContainer(targetPanel);

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        List<ItemRuntime> filteredItems = FilterItemsByPanel(targetPanel);

        for (int i = 0; i < maxSlots; i++)
        {
            var slot = Instantiate(slotPrefab, parent);
            slot.gameObject.SetActive(true);

            var bgImg = slot.transform.Find("BG").GetComponent<Image>();
            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            var amount = slot.transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            bgImg.gameObject.SetActive(false);
            iconImg.sprite = null;
            amount.text = string.Empty;

            if (i < filteredItems.Count)
            {
                var item = filteredItems[i];
                iconImg.sprite = item.icon;
                amount.text = (item.quantity >= 1 && item.isStackable)
                    ? item.quantity.ToString()
                    : string.Empty;

                bgImg.gameObject.SetActive(item.quantity >= 1 && item.isStackable);
                iconImg.enabled = true;
            }
            else
            {
                iconImg.enabled = false;
            }
        }
    }
    Transform GetContainer(GameObject panel)
    {
        if (panel == toolPanel) return toolContainer;
        if (panel == farmPanel) return farmContainer;
        if (panel == cityPanel) return cityContainer;
        if (panel == dungeonPanel) return dungeonContainer;

        return null;
    }

    List<ItemRuntime> FilterItemsByPanel(GameObject panel)
    {
        List<ItemRuntime> result = new List<ItemRuntime>();

        foreach (var item in inventory)
        {
            if (panel == toolPanel && item.itemID.StartsWith("T"))
                result.Add(item);

            else if (panel == farmPanel && item.itemID.StartsWith("F"))
                result.Add(item);

            else if (panel == cityPanel && item.itemID.StartsWith("C"))
                result.Add(item);

            else if (panel == dungeonPanel && item.itemID.StartsWith("D"))
                result.Add(item);
        }

        return result;
    }
}