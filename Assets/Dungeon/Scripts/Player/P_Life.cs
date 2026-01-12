using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P_Life : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI hpNum;
    [SerializeField] Image staBar;
    [SerializeField] TextMeshProUGUI staNum;
    public float hp;
    public float sta;
    public bool isDead;
    public float hp0;
    public float sta0 = 100;
    float lv;
    void Start()
    {
        
    }

    void Update()
    {        
        lv = GetComponent<P_Lv>().lv;
        if(hp > hp0)
        {
            hp = hp0;
        }
        if(sta > sta0)
        {
            sta = sta0;
        }
        hpBar.fillAmount = hp / 100 + (lv - 1)*10;
        staBar.fillAmount = sta / sta0;
        hp0 = 100 + (lv - 1) * 10;
        GetComponent<Animator>().SetBool("isDead", isDead);
        if (hp <= 0)
        {
            hpNum.text = "0 / " + hp0.ToString();
            isDead = true;
        }
        else
        {
            hpNum.text = hp.ToString("F0") + " / " + hp0.ToString();
            isDead = false;
        }

        if (sta <= 0)
        {
            staNum.text = "0 / " + sta0.ToString();
            //CancelInvoke("PlusSta");            
        }
        else
        {
            staNum.text = sta.ToString("F0") + " / " + sta0.ToString();
            //InvokeRepeating("PlusSta", 0, 1f);
        }
    }

    void PlusSta()
    {
        sta += 1;
    }
}
