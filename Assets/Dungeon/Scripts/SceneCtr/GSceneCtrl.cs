using System.Collections;
using UnityEngine;

public class GSceneCtrl_S : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.Find("PlayerRuntime");
        StartCoroutine(LoadScene());
    }    

    IEnumerator LoadScene()
    {
        if (player != null)
        {
            if (player.GetComponent<PlayerRuntime>().indexScene ==0)
            {
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
            else if (player.GetComponent<PlayerRuntime>().indexScene == 1)
            {
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Farm");
            }
            else if (player.GetComponent<PlayerRuntime>().indexScene == 2)
            {
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("City");
            }
            else if (player.GetComponent<PlayerRuntime>().indexScene == 3)
            {
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Forge");
            }
            else if (player.GetComponent<PlayerRuntime>().indexScene == 4)
            {
                yield return new WaitForSeconds(2f);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon");
            }
        }
    }
}
