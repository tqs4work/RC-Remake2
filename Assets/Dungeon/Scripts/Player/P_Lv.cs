using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_Lv : MonoBehaviour
{
    [SerializeField] Image lvExp;
    [SerializeField] TextMeshProUGUI lvNum;

    public int lv;
    public int exp;
    void Start()
    {
        
    }
    
    void Update()
    {
        lvNum.text = lv.ToString();
        lvExp.fillAmount = (float)exp / (lv * 100);
        LvUp();
    }

    /*
     EXP for level up:
        Lv 1 > 2 : 100
        Lv 2 > 3 : 200
        Lv 3 > 4 : 300
        Lv 4 > 5 : 400
        Lv 5 > 6 : 500
        Lv 6 > 7 : 600
        Lv 7 > 8 : 700
        Lv 8 > 9 : 800
        Lv 9 > 10 : 900
        Lv 10 > 11 : 1000
     */

    void LvUp()
    {
        if (exp >= lv * 100)
        {
            exp -= lv * 100;
            lv++;
            GetComponent<P_Life>().hp = GetComponent<P_Life>().hp0;
        }
    }

}
