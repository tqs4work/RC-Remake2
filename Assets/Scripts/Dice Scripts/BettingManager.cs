using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BettingManager : MonoBehaviour
{
    public static BettingManager Instance;

    [SerializeField] private int playerCoins = 20;
    [SerializeField] private Button BetOddButton;
    [SerializeField] private Button BetEvenButton;

    [Header("Panels")]
    [SerializeField] private GameObject WelcomePanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private CanvasGroup ScorePanel;

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
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ScorePanel.gameObject.SetActive(false);
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
        if (playerCoins >= 1)
        {
            playerBetType = type;
            playerCoins -= 1;
            hasBet = true;

            Debug.Log($"🎯 Bet: {type} | Coins left: {playerCoins}");
            WelcomePanel.SetActive(false);
        }
        else
        {
            Debug.Log("💀 Not enough coins!");
        }
    }

    public void CheckResult(int total)
    {
        if (!hasBet) return;

        Debug.Log($"🎲 Dice result: {total}");

        bool isEven = (total % 2 == 0);

        if (isEven)
        {
            Debug.Log("➡ Result: EVEN");
            if (playerBetType == "Even") WinBet();
            else LoseBet();
        }
        else
        {
            Debug.Log("➡ Result: ODD");
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
        Debug.Log("🏆 You Win!");
    }

    public void LoseBet()
    {
        playerCoins -= 2;
        playerWon = false;
        Debug.Log("😢 You Lose!");
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
            WinPanel.SetActive(true);
        else
            LosePanel.SetActive(true);
    }
}
