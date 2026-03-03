using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePan : MonoBehaviour
{
    // Pause Panel    
    public RectTransform panel;

    public float openX = 800f;
    public float closeX = 1109f;
    public float speed = 800f;

    private bool isOpen = false;
    private bool isMoving = false;
    void Start()
    {
        panel = GameObject.Find("Canvas").transform.Find("PausePan").GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(1109f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    // Pause Panel
    public async void TogglePanel()
    {
        if (isMoving) return;

        isMoving = true;

        float targetX = isOpen ? closeX : openX;

        while (Mathf.Abs(panel.anchoredPosition.x - targetX) > 0.1f)
        {
            float newX = Mathf.MoveTowards(
                panel.anchoredPosition.x,
                targetX,
                speed * Time.unscaledDeltaTime
            );

            panel.anchoredPosition = new Vector2(newX, panel.anchoredPosition.y);
            await Task.Yield();
        }

        panel.anchoredPosition = new Vector2(targetX, panel.anchoredPosition.y);

        isOpen = !isOpen;
        isMoving = false;

        // Pause / Resume game
        if (Mathf.Approximately(targetX, openX))
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    // ===== BUTTON LOGIN =====
    public void GoToLogin()
    {
        if (!IsPanelOpen()) return;

        Time.timeScale = 1;
        SceneManager.LoadScene("Login");
    }

    public void ContinueGame()
    {
        if (!IsPanelOpen()) return;

        TogglePanel();
    }

    private bool IsPanelOpen()
    {
        return Mathf.Approximately(panel.anchoredPosition.x, openX);
    }
}
