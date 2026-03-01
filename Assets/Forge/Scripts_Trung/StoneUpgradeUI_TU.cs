using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoneUpgradeUI_TU : MonoBehaviour
{
    public System.Action onClose;
    public static StoneUpgradeUI_TU Instance;

    [Header("Slots")]
    public StoneSlotCell slotA;
    public StoneSlotCell slotB;
    public StoneSlotCell slotC;

    [Header("Confirm UI")]
    public GameObject confirmRoot;
    public TMP_Text confirmText;
    public Button btnYes;
    public Button btnNo;

    [Header("Price")]
    public TMP_Text priceText;
    public int priceL1toL2 = 200;
    public int priceL2toL3 = 500;

    [Header("Close")]
    public Button btnClose;

    [Header("FX")]
    public GameObject fxObject;
    public float mergeTime = 1f;

    void Awake()
    {
        Instance = this;

        btnYes.onClick.AddListener(OnConfirmYes);
        btnNo.onClick.AddListener(OnConfirmNo);
        btnClose.onClick.AddListener(Close);

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);

        slotA.SetEmpty();
        slotB.SetEmpty();
        slotC.SetEmpty();

        RefreshUI();
    }
    public void Close()
    {
        gameObject.SetActive(false);

        slotA.SetEmpty();
        slotB.SetEmpty();
        slotC.SetEmpty();

        onClose?.Invoke();
    }

    // ===============================
    // DOUBLE CLICK GỌI VÀO ĐÂY
    // ===============================

    public void TryPlaceStone(ItemRuntime runtime)
    {
        if (!gameObject.activeSelf)
            return;

        if (runtime.itemType != ItemType.Stone)
            return;

        if (runtime.upgradeLevel >= 3)
        {
            Debug.Log("Đá max cấp.");
            return;
        }

        if (slotA.IsEmpty())
        {
            slotA.SetItem(runtime);
        }
        else if (slotB.IsEmpty())
        {
            if (slotA.StoneLevel != runtime.upgradeLevel)
            {
                Debug.Log("Cần cùng cấp.");
                return;
            }

            slotB.SetItem(runtime);
        }

        RefreshUI();
    }

    void RefreshUI()
    {
        int targetLv = GetTargetLevel();

        if (targetLv == 0)
        {
            confirmRoot.SetActive(false);
            priceText.text = "-";
        }
        else
        {
            confirmRoot.SetActive(true);
            int price = GetPrice(targetLv);
            confirmText.text = $"Ghép Lv{targetLv - 1} → Lv{targetLv}";
            priceText.text = price + " G";
        }
    }

    int GetTargetLevel()
    {
        if (slotA.IsEmpty() || slotB.IsEmpty())
            return 0;

        if (slotA.StoneLevel != slotB.StoneLevel)
            return 0;

        if (slotA.StoneLevel == 1)
            return 2;

        if (slotA.StoneLevel == 2)
            return 3;

        return 0;
    }

    int GetPrice(int targetLv)
    {
        if (targetLv == 2) return priceL1toL2;
        if (targetLv == 3) return priceL2toL3;
        return 0;
    }

    void OnConfirmYes()
    {
        int targetLv = GetTargetLevel();
        if (targetLv == 0) return;

        int price = GetPrice(targetLv);

        var player = PlayerRuntime.Instance.Player;

        if (player.Gold < price)
        {
            Debug.Log("Không đủ vàng");
            return;
        }

        player.Gold -= price;

        StartCoroutine(CoUpgrade(targetLv));
    }

    IEnumerator CoUpgrade(int newLevel)
    {
        if (fxObject) fxObject.SetActive(true);

        yield return new WaitForSeconds(mergeTime);

        if (fxObject) fxObject.SetActive(false);

        var player = PlayerRuntime.Instance.Player;

        // XÓA 2 viên cũ
        RemoveStoneFromInventory(slotA.Item);
        RemoveStoneFromInventory(slotB.Item);

        // TẠO VIÊN MỚI
        ItemRuntime newStone = new ItemRuntime();
        newStone.itemType = ItemType.Stone;
        newStone.upgradeLevel = newLevel;
        newStone.itemID = "D_Stone_Lv" + newLevel;
        newStone.itemName = "Stone Lv" + newLevel;

        player.Inventory[InventoryContainerType.Dungeon].AddItem(newStone);

        slotA.SetEmpty();
        slotB.SetEmpty();
        slotC.SetItem(newStone);

        RefreshUI();
    }

    void RemoveStoneFromInventory(ItemRuntime item)
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

    void OnConfirmNo()
    {
        slotA.SetEmpty();
        slotB.SetEmpty();
        RefreshUI();
    }
    
}