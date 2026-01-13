using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerRuntime.Instance.Player.Gold += 10;
            Debug.Log("Gold +10");

            PlayerRuntime.Instance.Player.Mp -= 10;
            Debug.Log("Mp -10");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerRuntime.Instance.Player.Hp -= 5;
            Debug.Log("Hp -5");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerRuntime.Instance.Player.Exp += 10;
            Debug.Log("Exp +10");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddTestItem();
        }
    }

    private void AddTestItem()
    {
        var player = PlayerRuntime.Instance.Player;
        if (player == null) return;

        // ?? T?O ITEM TEST
        Item testItem = new Item
        {
            itemID = "sword_test",
            itemName = "Test Sword",
            atk = 10,
            def = 0
        };

        player.Inventory.Add(testItem);

        Debug.Log("Added Test Sword to inventory");
    }
}
