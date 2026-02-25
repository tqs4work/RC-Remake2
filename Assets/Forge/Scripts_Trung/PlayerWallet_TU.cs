using UnityEngine;
using UnityEngine.Events;
using TMPro;

[DefaultExecutionOrder(-100)] // chạy sớm để UI khác có thể đọc được Gold
public class PlayerWallet : MonoBehaviour
{
    [Header("Gold")]
    [SerializeField] private int startGold = 0;
    [SerializeField] private int gold;
    public int Gold => gold;

    [Header("UI (optional)")]
    [Tooltip("TextMeshPro để hiển thị vàng. Bỏ trống nếu bạn tự cập nhật ở nơi khác.")]
    [SerializeField] private TMP_Text goldText;

    [Header("Format")]
    [SerializeField] private string prefix = "Vàng: ";
    [SerializeField] private bool padWithZeros = true;
    [SerializeField] private int zeroPadWidth = 4;   // 0000, 0010, ...

    [Header("Events")]
    public UnityEvent<int> OnGoldChanged; // tham số = gold mới

    /*==================== Lifecycle ====================*/

    private void Awake()
    {
        gold = Mathf.Max(0, startGold);
        RefreshUI();
        OnGoldChanged?.Invoke(gold);
    }

    /*==================== API công khai ====================*/

    /// <summary>Thêm vàng (>=0).</summary>
    public void Add(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        Changed();
    }

    /// <summary>Trừ vàng nếu đủ. Trả về true nếu thành công.</summary>
    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        gold -= amount;
        Changed();
        return true;
    }

    /// <summary>Đặt vàng về giá trị cụ thể (không âm).</summary>
    public void Set(int newGold)
    {
        gold = Mathf.Max(0, newGold);
        Changed();
    }

    /*==================== Helpers ====================*/

    private void Changed()
    {
        RefreshUI();
        OnGoldChanged?.Invoke(gold);
    }

    private void RefreshUI()
    {
        if (!goldText) return;

        string body = padWithZeros ? gold.ToString().PadLeft(zeroPadWidth, '0')
                                   : gold.ToString();
        goldText.text = prefix + body;
    }

    /*==================== Menu tiện test ====================*/

    [ContextMenu("Add 100 Gold")]
    private void CM_Add100() => Add(100);

    [ContextMenu("Spend 50 Gold")]
    private void CM_Spend50() => TrySpend(50);
}