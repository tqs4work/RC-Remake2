using UnityEngine;
using TMPro;

public class BetAmountUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject panel;
    [SerializeField] public TMP_InputField betInputField;

    private string betType;

    public void Show(string type, int maxGold)
    {
        betType = type;
        betInputField.text = "";
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void Confirm()
    {
        if (!int.TryParse(betInputField.text, out int amount))
            return;

        BettingManager.Instance.ReceiveBet(betType, amount);
        Hide();
    }

    public void Cancel()
    {
        Hide();
    }

    public void AllIn()
    {
        betInputField.text = BettingManager.Instance.GetCurrentGold().ToString();
    }

    public void Multiply(int value)
    {
        if (int.TryParse(betInputField.text, out int current))
        {
            int newValue = current * value;

            int max = BettingManager.Instance.GetCurrentGold();
            if (newValue > max)
                newValue = max;

            betInputField.text = newValue.ToString();
        }
    }
}
