using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        Time.timeScale = 1f; // ??m b?o th?i gian ch?y bình th??ng khi vào menu
        player = GameObject.Find("PlayerRuntime");
    }

    public void LogoutButton()
    {
        SceneManager.LoadScene("Login");
    }

    public void LoadTest()
    {
        player.GetComponent<PlayerRuntime>().indexScene = 0;
        SceneManager.LoadScene("GameLoad");
    }

    public void LoadFarm()
    {
        player.GetComponent<PlayerRuntime>().indexScene = 1;
        SceneManager.LoadScene("GameLoad");
    }
    public void LoadCity()
    {
        player.GetComponent<PlayerRuntime>().indexScene = 2;
        SceneManager.LoadScene("GameLoad");
    }
    public void LoadForge()
    {
        player.GetComponent<PlayerRuntime>().indexScene = 3;
        SceneManager.LoadScene("GameLoad");
    }
    public void LoadDungeon()
    {
        player.GetComponent<PlayerRuntime>().indexScene = 4;
        SceneManager.LoadScene("GameLoad");
    }
}
