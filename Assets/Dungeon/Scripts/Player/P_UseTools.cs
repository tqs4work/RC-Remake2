using System.Collections;
using UnityEngine;

public class P_UseTools : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    Vector3 mousePos;
    Vector3 mouseWPos;
    Vector3 direct;
    bool isDetectE;
    bool isDetectM;
    public bool isAction;
    bool isRoll;
    bool isRA;
    public int numberTool;
    public GameObject select;
    [SerializeField] GameObject arrowDir;
    [SerializeField] GameObject aPos;
    [SerializeField] GameObject bPos;
    [SerializeField] GameObject arrowPre;

    [SerializeField] GameObject stone1;
    [SerializeField] GameObject stone2;
    [SerializeField] GameObject stone3;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollMouse();

        arrowDir.SetActive(isRA && isAction);


        if(isRA)
        {
            Aim();
        }

        CheckE();

    }
    void ScrollMouse()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        select.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250f + (numberTool - 1) * 100f, 60);
        if (scrollInput > 0)
        {
            numberTool++;
            if (numberTool > 6) numberTool = 1;

        }
        else if (scrollInput < 0)
        {
            numberTool--;
            if (numberTool < 1) numberTool = 6;
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isRoll)
        {
            StartCoroutine(Roll());
        }
        if (Input.GetMouseButtonDown(0) && !isAction)
        {
            switch (numberTool)
            {
                case 1:
                    StartCoroutine(Axe());
                    break;
                case 2:
                    StartCoroutine(Minning());
                    break;
                case 3:
                    StartCoroutine(Dig());
                    break;
                case 4:
                    StartCoroutine(Water());
                    break;
                case 5:
                    StartCoroutine(MA());
                    break;
                case 6:
                    StartCoroutine(RA());
                    break;
                default:
                    break;
            }
        }
    }    
    public IEnumerator Water()
    {        
        animator.SetTrigger("Water");
        isAction = true;
        yield return new WaitForSeconds(1f);
        isAction = false;
    }

    public IEnumerator Axe()
    {        
        animator.SetTrigger("Axe");
        isAction = true;
        yield return new WaitForSeconds(1f);
        isAction = false;
    }

    public IEnumerator Hammer()
    {
        animator.SetTrigger("Hammer");
        isAction = true;
        yield return new WaitForSeconds(1f);
        isAction = false;
    }
    public IEnumerator Minning()
    {        
        if (isDetectM)
        {
            TurnBody();
        }
        animator.SetTrigger("Mining");
        isAction = true;
        yield return new WaitForSeconds(4 / 6f);
        Hit2();
        yield return new WaitForSeconds(2 / 6f);
        isAction = false;
    }

    public IEnumerator Dig()
    {        
        animator.SetTrigger("Dig");
        isAction = true;
        yield return new WaitForSeconds(1f);
        isAction = false;
    }

    public IEnumerator Roll()
    {        
        animator.SetTrigger("Roll");
        isRoll = true;
        yield return new WaitForSeconds(1f);
        isRoll = false;
    }

    public IEnumerator MA()
    {
        PlayerRuntime.Instance.Player.Mp -= 5;
        if (isDetectE)
        {
            TurnBody();
        }
        animator.SetTrigger("MA");
        isAction = true;
        yield return new WaitForSeconds(4/6f);
        Hit1();
        yield return new WaitForSeconds(2/6f);
        isAction = false;
    }

    public IEnumerator RA()
    {      
        PlayerRuntime.Instance.Player.Mp -= 5;
        animator.SetTrigger("RA");
        isRA = true;
        isAction = true;
        yield return new WaitForSeconds(4/6f);
        GameObject a = Instantiate(arrowPre, bPos.transform.position, Quaternion.LookRotation(Vector3.forward, direct) * Quaternion.Euler(0, 0, 90));
        a.GetComponent<Rigidbody2D>().linearVelocity = direct * 15f;
        Destroy(a, 3f);
        isRA = false;
        yield return new WaitForSeconds(2/6f);        
        isAction = false;
    }

    void Aim()
    {
        mousePos = Input.mousePosition;
        mouseWPos = Camera.main.ScreenToWorldPoint(mousePos);
        direct = (mouseWPos - transform.position).normalized;
        arrowDir.transform.rotation = Quaternion.LookRotation(Vector3.forward, direct) * Quaternion.Euler(0, 0, 90);
        GetComponent<P_Move>().lastX = direct.x;
        GetComponent<P_Move>().lastY = direct.y;
        animator.SetFloat("X", direct.x);
        animator.SetFloat("Y", direct.y);
    }

    void TurnBody()
    {
        GetComponent<P_Move>().lastX = direct.x;
        GetComponent<P_Move>().lastY = direct.y;
        animator.SetFloat("X", direct.x);
        animator.SetFloat("Y", direct.y);
    }
    void CheckE()
    {
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(aPos.transform.position, new Vector2(3, 3), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Enemy"));
        foreach (Collider2D c in enemies)
        {
            if (enemies.Length == 0)
            {
                isDetectE = false;                
            }
            else
            {
                isDetectE = true;
                direct = (c.transform.position - transform.position).normalized;
            }
        }
    }

    void CheckM()
    {
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(aPos.transform.position, new Vector2(3, 3), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Stone"));
        foreach (Collider2D c in enemies)
        {
            if (enemies.Length == 0)
            {
                isDetectM = false;
            }
            else
            {
                isDetectM = true;
                direct = (c.transform.position - transform.position).normalized;
            }
        }
    }

    void Hit1()
    {
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(aPos.transform.position, new Vector2(3, 3), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Enemy"));

        foreach (Collider2D c in enemies)
        {
            if (c.CompareTag("Enemy"))
            {
                c.GetComponent<Animator>().SetTrigger("Hurt");
                //c.GetComponent<E_Life>().hp -= 1;
                c.GetComponent<Rigidbody2D>().AddForce((c.transform.position - transform.position) * 5f, ForceMode2D.Impulse);
            }
        }
    }

    void Hit2()
    {
        Collider2D[] enemies = Physics2D.OverlapCapsuleAll(aPos.transform.position, new Vector2(3, 3), CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Stone"));

        foreach (Collider2D c in enemies)
        {
            if (c.CompareTag("Stone1"))
            {
                Instantiate(stone1, transform.position + new Vector3(Random.Range(-0.5f,0.5f), Random.Range(-0.5f, 0.5f),0), Quaternion.identity);
            }
            if (c.CompareTag("Stone2"))
            {
                Instantiate(stone2, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), Quaternion.identity);
            }
            if (c.CompareTag("Stone3"))
            {
                Instantiate(stone3, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), Quaternion.identity);
            }
        }
    }
}
