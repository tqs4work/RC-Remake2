using System.Collections;
using UnityEngine;

public class Frog_Attack : MonoBehaviour
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
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (!isAction && isDetectPlayer() && !isAttackPlayer())
        {
            StartCoroutine(MoveToPlayer());
        }
        else if (!isAction && isAttackPlayer())
        {
            StartCoroutine(AttackToPlayer());
        }
        if(player != null)
        {            
            if (player.transform.position.x > transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
            if (player.transform.position.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
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
