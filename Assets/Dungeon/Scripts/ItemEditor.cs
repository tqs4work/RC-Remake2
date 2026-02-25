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

        bonus = serializedObject.FindProperty("bonus");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        InitStyles();

        //EditorGUILayout.LabelField("Th�ng tin c? b?n", vietnameseBold);
        EditorGUILayout.PropertyField(itemType);
        EditorGUILayout.PropertyField(itemID);
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.PropertyField(isStackable);
        EditorGUILayout.PropertyField(price);
        EditorGUILayout.PropertyField(level);
        
        EditorGUILayout.Space();

        ItemType type = (ItemType)itemType.enumValueIndex;

        if (type == ItemType.Weapon || type == ItemType.Armor)
        {
            //EditorGUILayout.LabelField("Ch? s? chi?n ??u", vietnameseBold);
            EditorGUILayout.PropertyField(atk);
            EditorGUILayout.PropertyField(def);
            EditorGUILayout.PropertyField(durability);


        }

        if(type == ItemType.Consumable)
        {
            //EditorGUILayout.LabelField("Ch? s? h?i ph?c", vietnameseBold);
            EditorGUILayout.PropertyField(hpAmount);
            EditorGUILayout.PropertyField(mpAmount);
        }


        if (isStackable.boolValue == true)
        {
            EditorGUILayout.PropertyField(quantity);
            EditorGUILayout.PropertyField(maxStack);
        }


        if(type == ItemType.Seed)
        {
            //EditorGUILayout.LabelField("Th?i gian ph�t tri?n", vietnameseBold);
            EditorGUILayout.PropertyField(growTime);
        }

        EditorGUILayout.PropertyField(bonus);
        serializedObject.ApplyModifiedProperties();
    }

    private GUIStyle vietnameseBold;

    void InitStyles()
    {
        if (vietnameseBold != null) return;

        vietnameseBold = new GUIStyle(EditorStyles.label);
        vietnameseBold.fontStyle = FontStyle.Bold;
        vietnameseBold.fontSize = 12;
    }
}
