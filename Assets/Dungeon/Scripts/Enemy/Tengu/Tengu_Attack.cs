using System.Collections;
using UnityEngine;

public class Tengu_Attack : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    BoxCollider2D box;
    GameObject player;
    bool isAction;
    [SerializeField] GameObject aPos;
    [SerializeField] GameObject bPos;
    
    [SerializeField] float aRange;
    [SerializeField] float bRange;

    [SerializeField] GameObject redZone1;
    [SerializeField] GameObject redZone2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        if (PlayerRuntime.Instance.Player.Hp <= 0) return;

        if (!isAction && isDetectPlayer() && !isAttackPlayer())
        {
            StartCoroutine(MoveToPlayer());
        }
        else if (!isAction && isAttackPlayer())
        {
            StartCoroutine(A1());
        }
        if (player != null)
        {
            if (player.transform.position.x > transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
            if (player.transform.position.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        }
    }


    IEnumerator MoveToPlayer()
    {
        isAction = true;
        if (player != null)
        {
            anim.SetTrigger("MD");
            yield return new WaitForSeconds(1f);
            sr.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(0.5f);
            transform.position = player.transform.position;
            GameObject r = Instantiate(redZone1, player.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            r.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            r.GetComponent<CircleCollider2D>().enabled = true;
            yield return new WaitForSeconds(0.2f);
            Destroy(r);
            sr.enabled = true;
            anim.SetTrigger("MU");
            yield return new WaitForSeconds(0.8f);
            box.enabled = true;
            yield return new WaitForSeconds(1f);
            isAction = false;
        }
        else
        {
            yield return new WaitForSeconds(0f);
            isAction = false;
        }
    }    

    IEnumerator A1()
    {
        isAction = true;
        anim.SetTrigger("Jump");
        yield return new WaitForSeconds(0.25f);

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.transform.position;

        GameObject r = Instantiate(redZone2, targetPos, Quaternion.identity);        

        box.enabled = false;

        float distance = Vector2.Distance(startPos, targetPos);

        // ?? Ch?m l?i g?p 2
        float duration = Mathf.Clamp(distance / 6f *1.5f, 0.3f * 1.5f, 0.8f * 1.5f); //:2, *2, *2

        float jumpHeight = 2.5f;

        float timer = 0f;
        bool fallTriggered = false;

        while (timer < duration)
        {
            float t = timer / duration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            float heightOffset = 4 * jumpHeight * t * (1 - t);

            transform.position = pos + new Vector3(0, heightOffset, 0);

            // ?? Khi b?t ??u ?i xu?ng (qua ??nh)
            if (!fallTriggered && t >= 0.5f)
            {
                anim.SetBool("isFall", true);
                fallTriggered = true;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        anim.SetBool("isFall", false);
        
        box.enabled = true;

        r.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        r.GetComponent<CircleCollider2D>().enabled = true;

        yield return new WaitForSeconds(0.2f);
        Destroy(r);

        yield return new WaitForSeconds(2f);
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
