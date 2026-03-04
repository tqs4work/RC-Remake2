using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour
{
    public List<GameObject> giftItems; // Danh sách các item có thể nhận được từ gift
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerRuntime.Instance.Player.Gold += 1000; 
                    foreach (GameObject item in giftItems)
                    {
                        Instantiate(item, transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject); 
                    break;
                }
            }
        }
    }

    
}
