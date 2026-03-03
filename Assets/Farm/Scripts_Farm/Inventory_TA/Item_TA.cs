using UnityEngine;

public enum ItemType
{
    Shovel,
    Axe,
    Pickaxe,
    WateringCan,
    Bow,
    Sword,
    Armor,
    Consumable,
    Seed,
    Material, 
    Arrow,
    Stone
}
[CreateAssetMenu(fileName = "Item_", menuName = "Inventory/NewItem")]
public class Item : ScriptableObject
{
    [Header("Thông tin vật phẩm")]    
    public ItemType itemType;
    public string itemID;
    public string itemName;    
    
    [TextArea]
    public string description;

    [Header("Hình ảnh")]
    public Sprite icon;

    [Header("Chỉ số vật phẩm")]
    public bool isStackable;
    public int maxStack;
    public int price;
    public bool stackable = false;
    public int level;

    [Header("Vật phẩm tiêu thụ")]
    public int quantity;
    public int hpAmount;
    public int mpAmount;

    [Header("Vật phẩm chiến đấu")]
    public float atk;
    public float def;
    public float crit = 0f;
     [Header("Durability")]
    [Tooltip("Áp dụng cho Weapon/Armor. Arrow/Stone không dùng độ bền.")]
     public bool useDurability = true;
    public float durability;

    [Header("Upgrade (+1..+5): cộng THÊM mỗi cấp")]
    [Tooltip("Độ dài <= 5. Mỗi phần tử là L1..L5 (cộng thêm so với cấp trước).")]
    public int[] addATKPerLevel = new int[5];
    public int[] addDEFPerLevel = new int[5];
    public float[] addCRITPerLevel = new float[5];

    [Range(1, 200)] public float maxDurability = 100;
    [Tooltip("Tối đa % giá mua để sửa từ 0% -> full. Ví dụ 0.5 = 50% giá mua.")]
    [Range(0f, 1f)] public float maxRepairRateOfBuyPrice = 0.5f;


    [Header("Hạt giống")]
    public float growTime;    

    [Header("Chỉ số đặc biệt")]
    public float bonus;    

    [Header("Stone (only when type = Stone)")]
    [Range(1, 3)] public int stoneLevel = 1;                 // 1=trắng, 2=xanh lá, 3=tím
    [Tooltip("Giá nâng từ stoneLevel hiện tại -> stoneLevel+1 (nếu có)")]
    public int upgradePriceToNext = 0;
    [Header("Upgrade Icons (0-5)")]
    public Sprite[] upgradeIcons = new Sprite[6];

    // ===== Helpers =====
    public bool IsStone => itemType == ItemType.Stone;

    /// <summary>Tổng cộng thêm từ +1..+level. level clamp 0..5</summary>
    public void GetAddedAtLevel(int level, out int addAtk, out int addDef, out float addCrit)
    {
        level = Mathf.Clamp(level, 0, 5);
        addAtk = addDef = 0; addCrit = 0f;
        for (int i = 0; i < level; i++)
        {
            if (i < addATKPerLevel.Length)  addAtk  += addATKPerLevel[i];
            if (i < addDEFPerLevel.Length)  addDef  += addDEFPerLevel[i];
            if (i < addCRITPerLevel.Length) addCrit += addCRITPerLevel[i];
        }
    }

    /// <summary>Chỉ số cuối cùng cho tooltip (base + add)</summary>
    public void GetFinalStats(int level, out float Atk, out float Def, out float Crit)
    {
        GetAddedAtLevel(level, out int aA, out int aD, out float aC);
        Atk  = atk + aA;
        Def  = def + aD;
        Crit = crit + aC;
    }

    /// <summary>Màu theo cấp (dùng cho tên + highlight)</summary>
    public static Color GetColorByLevel(int level)
    {
        level = Mathf.Clamp(level, 0, 5);
        switch (level)
        {
            case 0: return Color.white;                        // +0
            case 1: return Color.white;                        // +1 vẫn trắng
            case 2: return new Color(0.33f, 0.85f, 0.33f);     // +2: green
            case 3: return new Color(0.35f, 0.55f, 1.00f);     // +3: blue
            case 4: return new Color(1.00f, 0.85f, 0.20f);     // +4: yellow
            case 5: return new Color(1.00f, 0.35f, 0.35f);     // +5: red
            default: return Color.white;
        }
    }

    /// <summary>Giá sửa theo % độ bền (0..100)</summary>
    public int GetRepairCost(int durabilityPercent)
    {
        if (!useDurability || itemType == ItemType.Arrow || IsStone) return 0;
        durabilityPercent = Mathf.Clamp(durabilityPercent, 0, 100);
        float missing = 1f - (durabilityPercent / 100f);
        float maxCost = price * maxRepairRateOfBuyPrice;
        return Mathf.CeilToInt(maxCost * missing);
    }

#if UNITY_EDITOR
    // Bảo đảm dữ liệu “hợp lệ” mỗi lần chỉnh trong Inspector
    void OnValidate()
    {
        ClampArray(ref addATKPerLevel);
        ClampArray(ref addDEFPerLevel);
        ClampArray(ref addCRITPerLevel);

        if (IsStone)
        {
            // Đá: chồng được, không có độ bền, reset stat
            stackable = true;
            useDurability = false;
            atk = def = 0;
            crit = 0f;

            stoneLevel = Mathf.Clamp(stoneLevel, 1, 3);
            if (stoneLevel >= 3) upgradePriceToNext = 0; // Lv3 là max -> không có giá nâng tiếp
        }
    }

    void ClampArray<T>(ref T[] arr)
    {
        if (arr == null) { arr = new T[5]; return; }
        if (arr.Length > 5)
        {
            var tmp = new T[5];
            for (int i = 0; i < 5; i++) tmp[i] = arr[i];
            arr = tmp;
        }
    }
#endif
}
    
