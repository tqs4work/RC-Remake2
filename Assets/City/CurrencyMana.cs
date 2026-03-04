using UnityEngine;
using System.Collections;

public class CurrencyMana : MonoBehaviour
{
    public static CurrencyMana Instance;

    private int currentCoin;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        currentCoin = PlayerRuntime.Instance.Player.Gold;
    }

    public bool SpendCoin(int amount)
    {
        if (currentCoin >= amount)
        {
            currentCoin -= amount;
            return true;
        }

        return false;
    }
}
