using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameUI : MonoBehaviour
{
    public GameObject minigamePanel;

    public void Start()
    {
        minigamePanel.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            minigamePanel.SetActive(true);
        }
    }

    public void ShowMinigameUI()
    {
        SceneManager.LoadScene("DiceRoll");
        Debug.Log("Minigame UI is now visible!");
    }
}
