using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;

public class BettingManager : MonoBehaviour
{
    public static BettingManager Instance;

    [SerializeField] private int playerCoins = PlayerRuntime.Instance.Player.Gold;
    [SerializeField] private Button BetOddButton;
    [SerializeField] private Button BetEvenButton;
    [SerializeField] private GameObject allPanel;

    [Header("Panels")]
    [SerializeField] private GameObject WelcomePanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private CanvasGroup ScorePanel;
    [SerializeField] private CanvasGroup instructionsPanel;

    private string playerBetType;
    private bool hasBet = false;
    private bool playerWon = false; // ✔ thêm biến xác định thắng thua

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WelcomePanel.SetActive(true);
        allPanel.SetActive(true);
        instructionsPanel.gameObject.SetActive(false);
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ScorePanel.gameObject.SetActive(false);
    }

    public void Update()
    {
        PlayerRuntime.Instance.Player.Gold = playerCoins; // Đồng bộ vàng với PlayerRuntime
    }

    public void play()
    {    
        WelcomePanel.SetActive(false);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BetOdd()
    {
        PlaceBet("Odd");
    }

    public void BetEven()
    {
        PlaceBet("Even");   
    }

    public void PlaceBet(string type)
    {
        StartCoroutine(showInstructions());

        if (playerCoins >= 1)
        {
            playerBetType = type;
            playerCoins -= 1;
            hasBet = true;

            Debug.Log($"🎯 Bet: {type} | Coins left: {playerCoins}");
            allPanel.SetActive(false);
        }
        else
        {
            Debug.Log("💀 Not enough coins!");
        }
    }

    public void CheckResult(int total)
    {
        if (!hasBet) return;

        bool isEven = (total % 2 == 0);

        if (isEven)
        {
            if (playerBetType == "Even") WinBet();
            else LoseBet();
        }
        else
        {
            if (playerBetType == "Odd") WinBet();
            else LoseBet();
        }

        StartCoroutine(ShowScorePanel());
        hasBet = false;
    }

    // ✔ Không bật panel ở đây nữa — chỉ tính xu + lưu kết quả
    public void WinBet()
    {
        playerCoins += 2;
        playerWon = true;
    }

    public void LoseBet()
    {
        playerCoins -= 2;
        playerWon = false;
    }

    IEnumerator showInstructions()
    {
        instructionsPanel.gameObject.SetActive(true);

    // 🚫 Khóa tương tác khi bắt đầu
    instructionsPanel.interactable = true;
    instructionsPanel.blocksRaycasts = true;

    // Fade IN
    instructionsPanel.alpha = 0;
    float t = 0;

    while (t < 0.3f)
    {
        t += Time.deltaTime;
        instructionsPanel.alpha = t / 0.3f;
        yield return null;
    }

    instructionsPanel.alpha = 1;

    // ✅ Khi alpha == 1 → mở tương tác
    instructionsPanel.interactable = false;
    instructionsPanel.blocksRaycasts = false;

    yield return new WaitForSeconds(2f);

    // 🚫 Khóa lại trước khi fade out
    instructionsPanel.interactable = false;
    instructionsPanel.blocksRaycasts = false;

    // Fade OUT
    t = 0;
    while (t < 0.3f)
    {
        t += Time.deltaTime;
        instructionsPanel.alpha = 1 - (t / 0.3f);
        yield return null;
    }

    instructionsPanel.alpha = 0;
    instructionsPanel.interactable = true;
    instructionsPanel.blocksRaycasts = true;

    instructionsPanel.gameObject.SetActive(false);
    }

    // ✔ ScorePanel → wait → fade out → rồi mới Win/Lose panel
    IEnumerator ShowScorePanel()
    {
        ScorePanel.gameObject.SetActive(true);

        // Fade IN
        ScorePanel.alpha = 0;
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            ScorePanel.alpha = t / 0.3f;
            yield return null;
        }

        ScorePanel.alpha = 1;

        // Wait
        yield return new WaitForSeconds(2f);

        // Fade OUT
        t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            ScorePanel.alpha = 1 - (t / 0.3f);
            yield return null;
        }

        ScorePanel.gameObject.SetActive(false);

        // ✔ Sau khi score biến mất → hiện win/lose
        if (playerWon)
            StartCoroutine(winPanelTimer());
        else
            StartCoroutine(losePanelTimer());
    }

    IEnumerator winPanelTimer()
    {
        WinPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        WinPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        
        RestartGame();
    }

    IEnumerator losePanelTimer()
    {
        LosePanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        LosePanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        
        RestartGame();
    }
}
