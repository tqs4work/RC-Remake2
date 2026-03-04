using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoneUpgradeUI_TU : MonoBehaviour
{
    [Header("Info Dialog")]
    public InfoDialog_TU infoDialog;
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
    public int priceL1toL2 = 200;
    public int priceL2toL3 = 500;

    [Header("Close")]
    public Button btnClose;

    [Header("FX")]
    public GameObject fxObject;
    public float mergeTime = 1f;

    [Header("Sound")]
    public AudioSource mergeSound;

    void Awake()
    {
        Instance = this;

        btnYes.onClick.AddListener(OnConfirmYes);
        btnNo.onClick.AddListener(OnConfirmNo);
        btnClose.onClick.AddListener(Close);
        if (fxObject)
        fxObject.SetActive(false);
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
        confirmRoot.SetActive(false);
        onClose?.Invoke();
    }

    // ===============================
    // DOUBLE CLICK GỌI VÀO ĐÂY
    // ===============================

    public void TryPlaceStone(ItemRuntime runtime)
    {
        if (!gameObject.activeSelf) return;
        if (IsBlockingUI()) return;

        if (runtime.itemType != ItemType.Stone)
        {
            infoDialog?.Show("Chỉ có thể đặt ĐÁ vào đây!");
            return;
        }

        // 🔥 Chặn max cấp ngay từ đầu
        if (runtime.upgradeLevel >= 3)
        {
            infoDialog?.Show("Đá đã đạt cấp cao nhất!");
            return;
        }

        if (slotA.IsEmpty())
        {
            PlaceStone(runtime, slotA);
            return;
        }

        if (slotB.IsEmpty())
        {
            // chỉ cần check cùng cấp
            if (slotA.StoneLevel != runtime.upgradeLevel)
            {
                infoDialog?.Show("Phải dùng 2 viên đá cùng cấp!");
                return;
            }

            PlaceStone(runtime, slotB);
            return;
        }
    }

    void RefreshUI()
    {
        int result = GetTargetLevel();

        if (result == 0)
        {
            confirmRoot.SetActive(false);
            return;
        }

        if (result == -1)
        {
            infoDialog?.Show("Chỉ được ghép đá cùng loại và cùng cấp!");
            slotB.SetEmpty();
            return;
        }

        if (result == -2)
        {
            infoDialog?.Show("Đá đã đạt cấp tối đa!");
            slotA.SetEmpty();
            slotB.SetEmpty();
            return;
        }


        int price = GetPrice(result);

        confirmRoot.SetActive(true);
        confirmText.text = $"Ghép Lv{result - 1} → Lv{result}\nGiá: {price}G";
    }

    int GetTargetLevel()
    {
        if (slotA.IsEmpty() || slotB.IsEmpty())
        return 0;

        int currentLevel = slotA.StoneLevel;

        if (currentLevel >= 3)
            return -2;

        return currentLevel + 1;
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
            infoDialog?.Show("Không đủ vàng");
            return;
        }

        player.Gold -= price;

        confirmRoot.SetActive(false);

        // 🔥 XÓA ĐÁ CŨ NGAY
        RemoveStoneFromInventory(slotA.Item);
        RemoveStoneFromInventory(slotB.Item);

        StartCoroutine(CoUpgrade(targetLv));
    }

    IEnumerator CoUpgrade(int newLevel)
    {
        slotA.SetEmpty();
        slotB.SetEmpty();

        // ===== PLAY FX =====
        if (fxObject)
        {
            fxObject.SetActive(true);

            

            // Nếu là ParticleSystem → reset sạch
            var ps = fxObject.GetComponent<ParticleSystem>();
            if (ps)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }
        if (mergeSound && mergeSound.clip != null)
            {
                mergeSound.PlayOneShot(mergeSound.clip);
            }

        yield return new WaitForSeconds(mergeTime);

        // ===== TẮT FX NGAY =====
        if (fxObject)
        {
            var ps = fxObject.GetComponent<ParticleSystem>();
            if (ps)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            fxObject.SetActive(false);
        }

        // ===== TẠO ĐÁ MỚI =====
        Item baseItem = ItemDatabase.Instance.GetItemByID("D_Stone_Lv" + newLevel);

        if (baseItem == null)
        {
            Debug.LogError("Stone ID không tồn tại!");
            yield break;
        }

        ItemRuntime newStone = ItemRuntime.FromItem(baseItem);

        slotC.SetItem(newStone);

        infoDialog?.Show(
            $"Nâng cấp thành công! Đá Lv{newLevel}",
            () =>
            {
                var player = PlayerRuntime.Instance.Player;

                player.Inventory[InventoryContainerType.Dungeon].AddItem(newStone);

                slotC.SetEmpty();

                FindFirstObjectByType<InventoryUI>()?.RefreshAll();
            });
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
    public bool IsBlockingUI()
    {
        return confirmRoot.activeSelf ||
            (infoDialog != null && infoDialog.gameObject.activeSelf);
    }
    void PlaceStone(ItemRuntime source, StoneSlotCell targetSlot)
    {
        if (source.isStackable && source.quantity > 1)
        {
            source.quantity--;

            ItemRuntime clone = ItemRuntime.FromItem(source.itemData);

            //QUAN TRỌNG
            clone.upgradeLevel = source.upgradeLevel;
            clone.icon = source.icon;

            targetSlot.SetItem(clone);
        }
        else
        {
            RemoveStoneFromInventory(source);
            targetSlot.SetItem(source);
        }

        FindFirstObjectByType<InventoryUI>()?.RefreshAll();
        RefreshUI();
    }
}