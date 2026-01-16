using UnityEngine;

public class DebugPlayer : MonoBehaviour
{
    void Start()
    {
        Player player = PlayerRuntime.Instance.Player;

        Debug.Log($"Player: {player.Name} - HP: {player.Hp}");
        Debug.Log($"Inventory count: {player.Inventory.Count}");
    }
}
