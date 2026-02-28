using System.Collections;
using UnityEngine;

public class P_Action : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    P_Move move;

    Vector3 direct;

    public bool isAction;
    bool isRolling;
    bool isRangedAiming;

    [Header("References")]
    [SerializeField] HotbarUI hotbar;
    [SerializeField] GameObject arrowDir;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject arrowPrefab;

    [Header("Stone Drop")]
    [SerializeField] GameObject stone1;
    [SerializeField] GameObject stone2;
    [SerializeField] GameObject stone3;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        move = GetComponent<P_Move>();
    }

    void Update()
    {
        if (isAction) return;

        HandleInput();

        if (isRangedAiming)
            Aim();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UseCurrentItem();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling)
        {
            StartCoroutine(Roll());
        }
    }

    void UseCurrentItem()
    {
        ItemRuntime item = hotbar.GetSelectedItem();
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                if (EnoughMP(5))
                    StartCoroutine(MeleeAttack());
                break;

            case ItemType.Arrow:
                if (EnoughMP(5))
                    StartCoroutine(RangedAttack());
                break;

            case ItemType.Consumable:
                Consume(item);
                break;

            case ItemType.Seed:
                StartCoroutine(Dig());
                break;

            case ItemType.Stone:
                StartCoroutine(Mining());
                break;
        }
    }

    bool EnoughMP(int cost)
    {
        if (PlayerRuntime.Instance.Player.Mp < cost)
            return false;

        PlayerRuntime.Instance.Player.Mp -= cost;
        return true;
    }

    #region Consumable

    void Consume(ItemRuntime item)
    {
        if (item.quantity <= 0) return;

        var player = PlayerRuntime.Instance.Player;

        player.Hp += item.hpAmount;
        player.Mp += item.mpAmount;

        item.quantity--;

        if (item.quantity <= 0)
            player.Inventory.Remove(item);

        hotbar.Refresh();
    }

    #endregion

    #region Melee

    IEnumerator MeleeAttack()
    {
        isAction = true;

        animator.SetTrigger("MA");
        yield return new WaitForSeconds(0.4f);

        DoMeleeHit();

        yield return new WaitForSeconds(0.2f);
        isAction = false;
    }

    void DoMeleeHit()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            1.5f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var c in enemies)
        {
            if (!c.CompareTag("Enemy")) continue;

            c.GetComponent<Animator>()?.SetTrigger("Hurt");

            Rigidbody2D enemyRb = c.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 forceDir = (c.transform.position - transform.position).normalized;
                enemyRb.AddForce(forceDir * 5f, ForceMode2D.Impulse);
            }
        }
    }

    #endregion

    #region Ranged

    IEnumerator RangedAttack()
    {
        isAction = true;
        isRangedAiming = true;

        animator.SetTrigger("RA");

        yield return new WaitForSeconds(0.4f);

        ShootArrow();

        isRangedAiming = false;

        yield return new WaitForSeconds(0.2f);
        isAction = false;
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(
            arrowPrefab,
            shootPoint.position,
            Quaternion.LookRotation(Vector3.forward, direct) * Quaternion.Euler(0, 0, 90)
        );

        arrow.GetComponent<Rigidbody2D>().linearVelocity = direct * 15f;
        Destroy(arrow, 3f);
    }

    void Aim()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direct = (mouse - transform.position).normalized;

        arrowDir.SetActive(true);
        arrowDir.transform.rotation =
            Quaternion.LookRotation(Vector3.forward, direct) * Quaternion.Euler(0, 0, 90);

        move.lastX = direct.x;
        move.lastY = direct.y;

        animator.SetFloat("X", direct.x);
        animator.SetFloat("Y", direct.y);
    }

    #endregion

    #region Mining / Dig

    IEnumerator Mining()
    {
        isAction = true;

        animator.SetTrigger("Mining");
        yield return new WaitForSeconds(0.6f);

        DropStone();

        yield return new WaitForSeconds(0.2f);
        isAction = false;
    }

    void DropStone()
    {
        Collider2D[] stones = Physics2D.OverlapCircleAll(
            attackPoint.position,
            1.5f,
            LayerMask.GetMask("Stone")
        );

        foreach (var c in stones)
        {
            Vector3 dropPos = transform.position + Random.insideUnitSphere * 0.5f;
            dropPos.z = 0;

            if (c.CompareTag("Stone1"))
                Instantiate(stone1, dropPos, Quaternion.identity);

            if (c.CompareTag("Stone2"))
                Instantiate(stone2, dropPos, Quaternion.identity);

            if (c.CompareTag("Stone3"))
                Instantiate(stone3, dropPos, Quaternion.identity);
        }
    }

    IEnumerator Dig()
    {
        isAction = true;
        animator.SetTrigger("Dig");
        yield return new WaitForSeconds(1f);
        isAction = false;
    }

    #endregion

    IEnumerator Roll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");
        yield return new WaitForSeconds(1f);
        isRolling = false;
    }
}