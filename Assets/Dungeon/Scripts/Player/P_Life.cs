using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_Life : MonoBehaviour
{
    public bool isDead = false;

    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
        InvokeRepeating("mpRestore", 0, 3f);
    }
    private void Update()
    {
        player.GetComponent<Animator>().SetBool("isDead", isDead);
        if (PlayerRuntime.Instance.Player.Hp <= 0)
        {
            isDead = true;
            CancelInvoke("mpRestore");
        }
        
    }

    void mpRestore()
    {         
        if (PlayerRuntime.Instance.Player.Mp < 100)
        {
            PlayerRuntime.Instance.Player.Mp += 1;
        }
    }
}
