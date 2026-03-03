using UnityEngine;
using UnityEngine.UI;

public class ScoreTrigger : MonoBehaviour
{
    Dice1Scripts dice;
    Dice2Scripts dice2;
    Dice3Scripts dice3;
    [SerializeField] private Text ScoreText;

    private void Awake()
    {
        dice = FindAnyObjectByType<Dice1Scripts>();    
        dice2 = FindAnyObjectByType<Dice2Scripts>();
        dice3 = FindAnyObjectByType<Dice3Scripts>();
    }

    private void Update()
    {
        ScoreCheck();
    }

    public void ScoreCheck()
    {
        if (dice != null && dice2 != null && ScoreText != null)
        {
            if (dice.diceFaceNum != 0 && dice2.diceFaceNum != 0 && dice3.diceFaceNum != 0)
            {
                int tong = dice.diceFaceNum + dice2.diceFaceNum + dice3.diceFaceNum;
                ScoreText.text = tong.ToString();

                if (tong >= 3 && tong <= 10)
                {
                    
                }
                else if (tong >= 11 && tong <= 18)
                {

                }

                if (BettingManager.Instance != null)
                {
                    BettingManager.Instance.CheckResult(tong);
                }
            }
            
        }      
    
    }
}