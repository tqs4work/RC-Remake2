using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpdateCanvas : MonoBehaviour
{
    public string Name;
    public int Hp;
    public int Mp;
    public int Exp;
    public int Lv;
    public int Gold;

    public GameObject canvas;
    public GameObject player;
    [SerializeField] Image hpBar;
    [SerializeField] TextMeshProUGUI hpNum;
    [SerializeField] Image mpBar;
    [SerializeField] TextMeshProUGUI mpNum;
    [SerializeField] TextMeshProUGUI usernameText;
    [SerializeField] TextMeshProUGUI lvNum;
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI goldNum;

    public bool isDead;
    public int hp0;
    public int mp0 = 100;

    void Start()
    {
        canvas = GameObject.Find("Canvas").transform.Find("Main Panel").transform.Find("PlayerInfo").gameObject;
        player = GameObject.Find("Player");
    }
    private void Update()
    {
        Name = PlayerRuntime.Instance.Player.Name;
        Hp = PlayerRuntime.Instance.Player.Hp;
        Mp = PlayerRuntime.Instance.Player.Mp;
        Exp = PlayerRuntime.Instance.Player.Exp;
        Lv = PlayerRuntime.Instance.Player.Lv;
        Gold = PlayerRuntime.Instance.Player.Gold;

        hpBar = canvas.transform.Find("HP_MP").transform.Find("HpBar").GetComponent<Image>();
        hpNum = canvas.transform.Find("HP_MP").transform.Find("HpNum").GetComponent<TextMeshProUGUI>();
        mpBar = canvas.transform.Find("HP_MP").transform.Find("MpBar").GetComponent<Image>();
        mpNum = canvas.transform.Find("HP_MP").transform.Find("MpNum").GetComponent<TextMeshProUGUI>();
        usernameText = canvas.transform.Find("Username").GetComponent<TextMeshProUGUI>();
        lvNum = canvas.transform.Find("LV_EXP").transform.Find("LvNum").GetComponent<TextMeshProUGUI>();
        expBar = canvas.transform.Find("LV_EXP").transform.Find("ExpBar").GetComponent<Image>();
        goldNum = canvas.transform.Find("GOLD").transform.Find("GoldNum").GetComponent<TextMeshProUGUI>();
        
        UpdateHPMP();
        UpdateUELG();
    }

    void UpdateUELG()
    {
        usernameText.text = Name;
        lvNum.text = Lv.ToString();
        expBar.fillAmount = (float)Exp / (Lv * 100);
        LvUp();
        goldNum.text = Gold.ToString();
    }
    void UpdateHPMP()
    {
        hp0 = 100 + (Lv - 1) * 10;
        player.GetComponent<Animator>().SetBool("isDead", isDead);

        if (Hp > hp0)
        {
            Hp = hp0;
        }
        if (Mp > mp0)
        {
            Mp = mp0;
        }

        hpBar.fillAmount = (float)Hp / hp0;
        mpBar.fillAmount = (float)Mp / mp0;

        if (Hp <= 0)
        {
            PlayerRuntime.Instance.Player.Hp = 0;
            hpNum.text = "0 / " + hp0.ToString();
            isDead = true;
        }
        else
        {
            hpNum.text = Hp.ToString("F0") + " / " + hp0.ToString();
            isDead = false;
        }

        if (Mp <= 0)
        {
            PlayerRuntime.Instance.Player.Mp = 0;
            mpNum.text = "0 / " + mp0.ToString();
            //CancelInvoke("PlusSta");            
        }
        else
        {
            mpNum.text = Mp.ToString("F0") + " / " + mp0.ToString();
            //InvokeRepeating("PlusSta", 0, 1f);
        }
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
        if (Exp >= Lv * 100)
        {
            Exp -= Lv * 100;
            PlayerRuntime.Instance.Player.Lv++;
            Hp = hp0;
        }
    }

}
