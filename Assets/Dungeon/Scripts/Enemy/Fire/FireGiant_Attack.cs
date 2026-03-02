using System.Collections;
using UnityEngine;

public class FireGiant_Attack : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    GameObject player;
    public GameObject aPos;
    bool isAction;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject bPos;
    [SerializeField] float ballSpeed;
    [SerializeField] float aRange;
    [SerializeField] float bRange;
    [SerializeField] bool isBig;
    [SerializeField] GameObject fireEPre;
    float hp;    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(Bigger());
    }


    void Update()
    {
        hp = GetComponent<E_Life>().hp;

        if (isBig)
        {
            if (!isAction && isDetectPlayer() && !isAttackPlayer())
            {
                StartCoroutine(MoveToPlayer());
            }
            else if (!isAction && isAttackPlayer())
            {
                StartCoroutine(AttackToPlayer());
            }
        }
        else
        {
            if (!isAction && isDetectPlayer())
            {
                StartCoroutine(MoveToPlayer());
            }
        }

        //if (player != null)
        //{
        //    if (player.transform.position.x > transform.position.x) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        //    if (player.transform.position.x < transform.position.x) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        //}
        if(isBig)
        {
            transform.localScale = new Vector3(2, 2, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (isBig && hp <= 0)
        {
            isBig = false;
            if (fireEPre != null) Instantiate(fireEPre, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            if (fireEPre != null) Instantiate(fireEPre, transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
        }
        
    }

    IEnumerator Bigger()
    {
        yield return new WaitForSeconds(0.1f);
        if (isBig)
        {
            transform.localScale = new Vector3(2, 2, 1);
            GetComponent<E_Life>().hp = 7;
        }
    }

    IEnumerator MoveToPlayer()
    {
        isAction = true;
        if (player != null) rb.linearVelocity = (player.transform.position - transform.position).normalized * 1f;
        anim.SetBool("isMove", true);
        yield return new WaitForSeconds(0.5f);
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMove", false);
        isAction = false;
    }

    IEnumerator AttackToPlayer()
    {
        isAction = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMove", false);
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        GameObject b = Instantiate(ball, bPos.transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody2D>().linearVelocity = (player.transform.position - bPos.transform.position).normalized * ballSpeed;
        Destroy(b, 3f);
        yield return new WaitForSeconds(0.5f);
        isAction = false;
    }

    bool isDetectPlayer()
    {
        Collider2D p = Physics2D.OverlapCircle(aPos.transform.position, aRange, LayerMask.GetMask("Player"));
        if (p != null)
        {
            player = p.gameObject;
            return true;
        }
        else
        {
            player = null;
            return false;
        }
    }

    bool isAttackPlayer()
    {
        return Physics2D.OverlapCircle(aPos.transform.position, bRange, LayerMask.GetMask("Player"));

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aPos.transform.position, aRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aPos.transform.position, bRange);
    }
}
