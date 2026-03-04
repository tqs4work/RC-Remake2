using UnityEngine;
using System.Collections;

public class CurrencyMana : MonoBehaviour
{
    public static CurrencyMana Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
  
    }

    public bool SpendCoin(int amount)
    {
        if (PlayerRuntime.Instance.Player.Gold >= amount)
        {
            PlayerRuntime.Instance.Player.Gold -= amount;
            return true;
        }

        return false;
    }
}
