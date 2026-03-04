using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    public void ExitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Thoát Play Mode trong Unity Editor
#else
        Application.Quit(); // Thoát khi build game
#endif
    }

    public void LoadFarm()
    {
        SceneManager.LoadScene("Farm");
    }
    public void LoadCity()
    {
        SceneManager.LoadScene("City");
    }
    public void LoadForge()
    {
        SceneManager.LoadScene("Forge");
    }
    public void LoadDungeon()
    {
        SceneManager.LoadScene("Dungeon");
    }
}
