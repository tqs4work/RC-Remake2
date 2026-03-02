using System.Collections;
using UnityEngine;

public class EnemyInteract : MonoBehaviour
{
    [SerializeField] GameObject player;
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.position.y > player.transform.position.y)
        //{
        //    sr.sortingOrder = -1;
        //}
        //else
        //{
        //    sr.sortingOrder = 1;
        //}
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("P_Bullet"))
        {
            GetComponent<E_Life>().hp -= 1;
            GetComponent<Animator>().SetTrigger("Hurt");
            Destroy(other.gameObject);
        }
    }   

}
