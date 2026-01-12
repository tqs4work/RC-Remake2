using UnityEngine;

public class P_Move : MonoBehaviour
{
    public float moveSpeed;
    
    public float lastX;
    public float lastY;
    bool isRun;    
    bool isAction;
    Rigidbody2D rb;
    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {        
        isAction = GetComponent<P_UseTools>().isAction;
        Animate();
        if(!isAction)
        {
            Move();
        }   
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRun", false);
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        animator.SetBool("isRun", x != 0 || y != 0);
        if (x == 0 && y == 0)
        {

        }
        else
        {
            lastX = x;
            lastY = y;
        }

        rb.linearVelocity = new Vector2 (x, y).normalized * moveSpeed;
    }

    void Animate()
    {
        
        animator.SetFloat("X", lastX);
        animator.SetFloat("Y", lastY);
    }    
}
