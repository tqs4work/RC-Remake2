using System.Collections;
using UnityEngine;

public class Slime_Attack : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    GameObject player;
    public GameObject aPos;    
    bool isAction;
    [SerializeField] GameObject warn;
    [SerializeField] GameObject audioManager;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //
        audioManager = GameObject.Find("AudioManager");
    }

    
    void Update()
    {
        if(!isAction && isDetectPlayer())
        {
            StartCoroutine(MoveToPlayer());
        }
        warn.SetActive(isDetectPlayer());
    }


    IEnumerator MoveToPlayer()
    {
        isAction = true;
        if (player != null) rb.linearVelocity = (player.transform.position - transform.position).normalized * 1f;
        anim.SetBool("isMove", true);
        //
        audioManager.GetComponent<DungeonAudio>().slime.Play();
        //
        yield return new WaitForSeconds(0.5f);
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMove", false);
        isAction = false;
    }



    bool isDetectPlayer()
    {
        Collider2D p =  Physics2D.OverlapCircle(aPos.transform.position, 5f, LayerMask.GetMask("Player"));        
        if(p != null)
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aPos.transform.position, 5f);
    }
}
