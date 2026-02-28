using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    void Awake()
    {
        // N?u ?ã có Instance r?i thì xóa object m?i
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Gi? object khi ??i scene
    }
}
