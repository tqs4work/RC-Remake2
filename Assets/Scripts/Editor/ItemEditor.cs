using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    SerializedProperty itemType;
    SerializedProperty itemID;
    SerializedProperty itemName;
    SerializedProperty isStackable;
    SerializedProperty maxStack;
    SerializedProperty description;
    SerializedProperty icon;

    SerializedProperty price;
    SerializedProperty level;

    SerializedProperty quantity;
    SerializedProperty hpAmount;
    SerializedProperty mpAmount;

    SerializedProperty atk;
    SerializedProperty def;
    SerializedProperty durability;

    SerializedProperty growTime;

    SerializedProperty bonus;


    SerializedProperty crit;
    SerializedProperty useDurability;
    SerializedProperty maxRepairRateOfBuyPrice;

    SerializedProperty addATKPerLevel;
    SerializedProperty addDEFPerLevel;
    SerializedProperty addCRITPerLevel;
    SerializedProperty stoneLevel;
    SerializedProperty upgradePriceToNext; // dành cho Stone
    SerializedProperty upgradePrice; // dành cho Weapon
    SerializedProperty upgradeIcons;

    void OnEnable()
    {
        itemType = serializedObject.FindProperty("itemType");
        itemID = serializedObject.FindProperty("itemID");
        itemName = serializedObject.FindProperty("itemName");
        isStackable = serializedObject.FindProperty("isStackable");
        maxStack = serializedObject.FindProperty("maxStack");
        description = serializedObject.FindProperty("description");
        icon = serializedObject.FindProperty("icon");

        price = serializedObject.FindProperty("price");
        level = serializedObject.FindProperty("level");

        quantity = serializedObject.FindProperty("quantity");
        hpAmount = serializedObject.FindProperty("hpAmount");
        mpAmount = serializedObject.FindProperty("mpAmount");

        atk = serializedObject.FindProperty("atk");
        def = serializedObject.FindProperty("def");
        durability = serializedObject.FindProperty("durability");

        growTime = serializedObject.FindProperty("growTime");
        upgradeIcons = serializedObject.FindProperty("upgradeIcons");
        bonus = serializedObject.FindProperty("bonus");
        stoneLevel = serializedObject.FindProperty("stoneLevel");
        upgradePriceToNext = serializedObject.FindProperty("upgradePriceToNext");
        upgradePrice = serializedObject.FindProperty("upgradePrice");


        crit = serializedObject.FindProperty("crit");
        useDurability = serializedObject.FindProperty("useDurability");
        maxRepairRateOfBuyPrice = serializedObject.FindProperty("maxRepairRateOfBuyPrice");
        addATKPerLevel = serializedObject.FindProperty("addATKPerLevel");
        addDEFPerLevel = serializedObject.FindProperty("addDEFPerLevel");
        addCRITPerLevel = serializedObject.FindProperty("addCRITPerLevel");
    }

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();        

    //    EditorGUILayout.PropertyField(itemType);
    //    EditorGUILayout.PropertyField(itemID);
    //    EditorGUILayout.PropertyField(itemName);
    //    EditorGUILayout.PropertyField(icon);
    //    EditorGUILayout.PropertyField(description);

    //    EditorGUILayout.PropertyField(isStackable);
    //    EditorGUILayout.PropertyField(price);
    //    EditorGUILayout.PropertyField(level);

    //    EditorGUILayout.Space();

    //    ItemType type = (ItemType)itemType.enumValueIndex;

    //    if (type == ItemType.Sword || type == ItemType.Armor || type == ItemType.Shovel || type == ItemType.Bow || type == ItemType.Axe || type == ItemType.Pickaxe)
    //    {            
    //        EditorGUILayout.PropertyField(atk);
    //        EditorGUILayout.PropertyField(def);
    //        EditorGUILayout.PropertyField(durability);


    //        EditorGUILayout.PropertyField(crit);
    //        EditorGUILayout.PropertyField(useDurability);
    //        EditorGUILayout.Space();
    //        EditorGUILayout.PropertyField(addATKPerLevel, true);
    //        EditorGUILayout.PropertyField(addDEFPerLevel, true);
    //        EditorGUILayout.PropertyField(addCRITPerLevel, true);


    //    }

    //    if(type == ItemType.Consumable)
    //    {            
    //        EditorGUILayout.PropertyField(hpAmount);
    //        EditorGUILayout.PropertyField(mpAmount);
    //    }


    //    if (isStackable.boolValue == true)
    //    {
    //        EditorGUILayout.PropertyField(quantity);
    //        EditorGUILayout.PropertyField(maxStack);
    //    }


    //    if(type == ItemType.Seed)
    //    {            
    //        EditorGUILayout.PropertyField(growTime);
    //    }

    //    EditorGUILayout.PropertyField(bonus);
    //    serializedObject.ApplyModifiedProperties();
    //}
    public override void OnInspectorGUI()
    {
        if (serializedObject == null)
            return;

        serializedObject.Update();

        // ===== SAFETY CHECK =====
        if (itemType == null)
        {
            EditorGUILayout.HelpBox("itemType property not found. Check field name in Item class.", MessageType.Error);
            return;
        }

        EditorGUILayout.PropertyField(itemType);
        EditorGUILayout.PropertyField(itemID);
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.PropertyField(isStackable);
        EditorGUILayout.PropertyField(price);
        if ((ItemType)itemType.enumValueIndex != ItemType.Stone)
        {
            EditorGUILayout.PropertyField(level);
        }

        EditorGUILayout.Space();

        ItemType type = (ItemType)itemType.enumValueIndex;
        // ===== Stone =====
        if (type == ItemType.Stone)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stone Settings", EditorStyles.boldLabel);

            if (stoneLevel != null)
                EditorGUILayout.PropertyField(stoneLevel);

            if (upgradePriceToNext != null)
                EditorGUILayout.PropertyField(upgradePriceToNext);
        }

        // ===== Equipment =====
        if (type == ItemType.Sword || type == ItemType.Armor ||
            type == ItemType.Shovel || type == ItemType.Bow ||
            type == ItemType.Axe || type == ItemType.Pickaxe)
        {
            if (atk != null) EditorGUILayout.PropertyField(atk);
            if (def != null) EditorGUILayout.PropertyField(def);
            if (durability != null) EditorGUILayout.PropertyField(durability);

            if (crit != null) EditorGUILayout.PropertyField(crit);
            if (useDurability != null) EditorGUILayout.PropertyField(useDurability);
            EditorGUILayout.PropertyField(upgradeIcons, true);
            EditorGUILayout.PropertyField(upgradePrice, true);

            EditorGUILayout.Space();

            if (addATKPerLevel != null) EditorGUILayout.PropertyField(addATKPerLevel, true);
            if (addDEFPerLevel != null) EditorGUILayout.PropertyField(addDEFPerLevel, true);
            if (addCRITPerLevel != null) EditorGUILayout.PropertyField(addCRITPerLevel, true);
        }

        // ===== Consumable =====
        if (type == ItemType.Consumable)
        {
            if (hpAmount != null) EditorGUILayout.PropertyField(hpAmount);
            if (mpAmount != null) EditorGUILayout.PropertyField(mpAmount);
        }

        // ===== Stackable =====
        if (isStackable != null && isStackable.boolValue)
        {
            if (quantity != null) EditorGUILayout.PropertyField(quantity);
            if (maxStack != null) EditorGUILayout.PropertyField(maxStack);
        }

        // ===== Seed =====
        if (type == ItemType.Seed)
        {
            if (growTime != null) EditorGUILayout.PropertyField(growTime);
        }

        if (bonus != null)
            EditorGUILayout.PropertyField(bonus);

        serializedObject.ApplyModifiedProperties();
    }
}
