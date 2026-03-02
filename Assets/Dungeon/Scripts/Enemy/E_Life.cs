using System.Collections;
using UnityEngine;

public class E_Life : MonoBehaviour
{
    public float hp;
    public float timeDeath;
    public float expDrop;
    public bool isDead;
    [SerializeField] private GameObject expPre;
    void Start()
    {
        
    }
    
    void Update()
    {
        if(hp <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Death());
        }
    }

    IEnumerator DropExp()
    {
        if (expPre != null && expDrop > 0) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 1) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 2) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 3) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 4) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 5) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 6) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 7) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 8) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        if (expPre != null && expDrop > 9) Instantiate(expPre, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Death()
    {
        yield return StartCoroutine(DropExp());        
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);

    }
}
