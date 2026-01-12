using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class P_Interact : MonoBehaviour
{
    [SerializeField] GameObject cam1;
    [SerializeField] GameObject cam2;
    [SerializeField] GameObject cam3;
    [SerializeField] Image BBG;
    [SerializeField] GameObject Portal;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Enemy"))
        {
            GetComponent<Animator>().SetTrigger("Hurt");
            GetComponent<P_Life>().hp -= 10;
            c.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }        
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Enemy"))
        {
            c.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Exp"))
        {
            GetComponent<P_Lv>().exp += 10;
            Destroy(c.gameObject);
        }
        if (c.gameObject.CompareTag("E_Bullet"))
        {
            GetComponent<Animator>().SetTrigger("Hurt");
            GetComponent<P_Life>().hp -= 10;
            Destroy(c.gameObject);
        }
        if (c.gameObject.CompareTag("E_Bullet"))
        {
            GetComponent<Animator>().SetTrigger("Hurt");
            GetComponent<P_Life>().hp -= 10;
            Destroy(c.gameObject);
        }

    }
    private void OnTriggerStay2D(Collider2D c)
    {        

    }
    IEnumerator Load()
    {
        BBG.gameObject.SetActive(true);
        BBG.CrossFadeAlpha(1, 0.1f, false);
        Portal.SetActive(true);
        yield return new WaitForSeconds(3f);
        Portal.SetActive(false);
        BBG.CrossFadeAlpha(0, 12f, false);
        yield return new WaitForSeconds(4f);
        BBG.CrossFadeAlpha(0, 1f, false);
    }
}
