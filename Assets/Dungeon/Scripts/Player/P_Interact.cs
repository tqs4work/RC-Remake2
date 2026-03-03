using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Interact : MonoBehaviour
{
    public bool isImmune = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Enemy") && !isImmune)
        {
            GetComponent<Animator>().SetTrigger("Hurt");
            PlayerRuntime.Instance.Player.Hp -= 10;
            isImmune = true;
            StartCoroutine(ResetImmune());
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        //if (c.gameObject.CompareTag("Enemy") && !isImmune)
        //{
        //    GetComponent<Animator>().SetTrigger("Hurt");
        //    PlayerRuntime.Instance.Player.Hp -= 10;
        //    isImmune = true;
        //    StartCoroutine(ResetImmune());
        //}

        if (c.gameObject.CompareTag("Exp"))
        {
            PlayerRuntime.Instance.Player.Exp += 10;
            Destroy(c.gameObject);
        }
        if (c.gameObject.CompareTag("E_Bullet") && !isImmune)
        {
            GetComponent<Animator>().SetTrigger("Hurt");
            PlayerRuntime.Instance.Player.Hp -= 10;
            Destroy(c.gameObject);
            isImmune = true;
            StartCoroutine(ResetImmune());
        }        

    }
    private void OnTriggerStay2D(Collider2D c)
    {        

    }

    IEnumerator ResetImmune()
    {
        yield return new WaitForSeconds(1f);
        isImmune = false;
    }
    //IEnumerator Load()
    //{
    //    BBG.gameObject.SetActive(true);
    //    BBG.CrossFadeAlpha(1, 0.1f, false);
    //    Portal.SetActive(true);
    //    yield return new WaitForSeconds(3f);
    //    Portal.SetActive(false);
    //    BBG.CrossFadeAlpha(0, 12f, false);
    //    yield return new WaitForSeconds(4f);
    //    BBG.CrossFadeAlpha(0, 1f, false);
    //}
}
