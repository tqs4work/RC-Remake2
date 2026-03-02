using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PlayerRuntime.Instance.Player.Gold += 10;
            PlayerRuntime.Instance.Player.Hp += 5;
            PlayerRuntime.Instance.Player.Exp += 10;
            PlayerRuntime.Instance.Player.Mp += 30;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayerRuntime.Instance.Player.Gold -= 10;
            PlayerRuntime.Instance.Player.Hp -= 5;
            PlayerRuntime.Instance.Player.Exp -= 10;
            PlayerRuntime.Instance.Player.Mp -= 30;
        }

    }

}
