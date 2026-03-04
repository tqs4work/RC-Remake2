using UnityEditor;
using UnityEngine;

public class LoginScene : MonoBehaviour
{
    public void ExitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Thoát Play Mode trong Unity Editor
#else
        Application.Quit(); // Thoát khi build game
#endif
    }
}
