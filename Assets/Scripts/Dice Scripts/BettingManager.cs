using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BettingManager : MonoBehaviour
{
    public static BettingManager Instance;

    // PLAYER DATA

    [SerializeField] private int playerCoins;          // Số vàng hiện tại
    private int currentBetAmount;                      // Số tiền đang cược

    // BETTING BUTTONS
    [Header("Betting Buttons")]
    [SerializeField] private Button BetOddButton;
    [SerializeField] private Button BetEvenButton;

    // UI PANELS
    [Header("Panels")]
    [SerializeField] private GameObject WelcomePanel;  // Panel play ban đầu
    [SerializeField] private GameObject WinPanel;      // Panel thắng
    [SerializeField] private GameObject LosePanel;     // Panel thua
    [SerializeField] private CanvasGroup ScorePanel;   // Panel hiển thị kết quả
    [SerializeField] private CanvasGroup instructionsPanel; // Panel hướng dẫn
    [SerializeField] private GameObject allPanel;      // Panel chứa nút cược


    // BET AMOUNT UI (SCRIPT KHÁC)
    [Header("Betting UI")]
    [SerializeField] private BetAmountUI betAmountUI;

    // TEXT
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private string playerBetType;  // Odd hoặc Even
    private bool hasBet = false;   // Đã đặt cược chưa
    private bool playerWon = false;// Kết quả thắng/thua

    // NHÀ CÁI
    private int winStreak = 0;
    private int forcedLoseCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Lấy vàng từ hệ thống PlayerRuntime
        playerCoins = PlayerRuntime.Instance.Player.Gold;
        UpdateMoneyUI();

        // Reset UI ban đầu
        WelcomePanel.SetActive(true);
        allPanel.SetActive(true);
        instructionsPanel.gameObject.SetActive(false);
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ScorePanel.gameObject.SetActive(false);
        betAmountUI.Hide();
    }

    public void Play()
    {
        WelcomePanel.SetActive(false);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("City");
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateMoneyUI()
    {
        moneyText.text = playerCoins.ToString();
        PlayerRuntime.Instance.Player.Gold = playerCoins;
    }

    public int GetCurrentGold()
    {
        return playerCoins;
    }

    public void BetOdd()
    {
        betAmountUI.Show("Odd", playerCoins);
    }

    public void BetEven()
    {
        betAmountUI.Show("Even", playerCoins);
    }

    // NHẬN DỮ LIỆU TỪ BetAmountUI
    public void ReceiveBet(string type, int amount)
    {
        if (type == "Odd" || type == "Even")
        {
            betAmountUI.Show(type, playerCoins);
            if (amount > playerCoins)
                return;

            playerBetType = type;
            currentBetAmount = amount;
            hasBet = true;

            // Trừ tiền ngay khi đặt cược
            playerCoins -= amount;
            UpdateMoneyUI();
            allPanel.SetActive(false);

            StartCoroutine(showInstructions());
        }
    }

    // CHECK RESULT (được gọi từ hệ thống xúc xắc)
    public void CheckResult(int total)
    {
        if (!hasBet) return;

        bool isEven = (total % 2 == 0);

        playerWon =
            (isEven && playerBetType == "Even") ||
            (!isEven && playerBetType == "Odd");

        hasBet = false;

        StartCoroutine(ShowScorePanel());
    }

    // ÁP DỤNG KẾT QUẢ (chỉ cộng tiền nếu thắng)
    void ApplyResult()
    {
        // RIG SYSTEM: 2 WIN → 5 LOSE

        if (forcedLoseCount > 0)
        {
            playerWon = false;
            forcedLoseCount--;
        }
        else
        {
            if (playerWon)
            {
                winStreak++;

                if (winStreak >= 2)
                {
                    winStreak = 0;
                    forcedLoseCount = 5;
                    Debug.Log("Forced Lose Activated (5 rounds)");
                }
            }
            else
            {
                winStreak = 0;
            }
        }
        // APPLY MONEY
        if (playerWon)
        {
            playerCoins += currentBetAmount * 2;
        }

        UpdateMoneyUI();
    }

    // INSTRUCTION PANEL (Fade in/out)
    IEnumerator showInstructions()
    {
        instructionsPanel.gameObject.SetActive(true);

        instructionsPanel.alpha = 0;
        float t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            instructionsPanel.alpha = t / 0.3f;
            yield return null;
        }

        instructionsPanel.alpha = 1;

        yield return new WaitForSeconds(2f);

        t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            instructionsPanel.alpha = 1 - (t / 0.3f);
            yield return null;
        }

        instructionsPanel.gameObject.SetActive(false);
    }

    // SCORE PANEL + WIN/LOSE FLOW
    IEnumerator ShowScorePanel()
    {
        ScorePanel.gameObject.SetActive(true);

        ScorePanel.alpha = 0;
        float t = 0;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            ScorePanel.alpha = t / 0.3f;
            yield return null;
        }

        ScorePanel.alpha = 1;

        yield return new WaitForSeconds(2f);

        ApplyResult(); // Cộng tiền nếu thắng

        ScorePanel.gameObject.SetActive(false);

        if (playerWon)
            StartCoroutine(winPanelTimer());
        else
            StartCoroutine(losePanelTimer());
    }


    // WIN / LOSE SEQUENCE
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