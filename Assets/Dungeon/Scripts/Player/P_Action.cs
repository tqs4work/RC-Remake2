using System.Collections;
using FirebaseAdmin.Messaging;
using Unity.VisualScripting;
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

    public bool isOpenInventory;

    [Header("References")]
    [SerializeField] HotbarUI hotbar;
    [SerializeField] GameObject arrowDir;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject arrowPrefab;

    [Header("Objects Drop")]
    [SerializeField] GameObject stone1;
    [SerializeField] GameObject stone2;
    [SerializeField] GameObject stone3;
    [SerializeField] GameObject wood;

    P_Audio audioPlayer;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        move = GetComponent<P_Move>();
    }

    void Start()
    {
        audioPlayer = GetComponent<P_Audio>();
    }

    void Update()
    {
        if(GameObject.Find("Canvas").transform.Find("InventoryUI") == null) return;

        isOpenInventory = GameObject.Find("Canvas").transform.Find("InventoryUI").GetComponent<InventoryUI>().isPanelOpen;

        if (hotbar == null)
        {
            hotbar = GameObject.Find("Canvas").transform.Find("HotBarManager").GetComponent<HotbarUI>();
        }
        if (isAction) return;

        HandleInput();

        if (isRangedAiming)
            Aim();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isOpenInventory && !isRolling)
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
            case ItemType.Sword:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(MeleeAttack());
                    audioPlayer.sword.Play();
                    item.durability -= 5;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.Bow:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(RangedAttack());
                    audioPlayer.bow.Play();
                    item.durability -= 5;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.Shovel:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(Dig());
                    audioPlayer.shovel.Play();
                    item.durability -= 5;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.Axe:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(Axe());
                    audioPlayer.axe.Play();
                    item.durability -= 5;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.Pickaxe:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(Mining());  
                    audioPlayer.pickaxe.Play();
                    item.durability -= 5;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.WateringCan:
                if (EnoughMP(5) && item.durability > 0)
                {
                    PlayerRuntime.Instance.Player.Mp -= 5;
                    StartCoroutine(Water());
                    audioPlayer.water.Play();
                    item.durability -= 1;
                    hotbar.Refresh();
                }
                else
                {
                    Debug.Log("Not enough MP or durability!");
                }
                break;

            case ItemType.Consumable:
                StartCoroutine(Doing());
                Consume(item);
                break;

            case ItemType.Seed:
                //StartCoroutine(Doing());
                break;

            case ItemType.Stone:
                //
                break;
        }
    }

    bool EnoughMP(int cost)
    {
        if (PlayerRuntime.Instance.Player.Mp < cost)
            return false;
        
        return true;
    }

    #region Consumable    

    void Consume(ItemRuntime item)
    {
        if (item == null || item.quantity <= 0)
            return;

        var player = PlayerRuntime.Instance.Player;


        // HP / MP
        player.Hp += item.hpAmount;
        player.Mp += item.mpAmount;

        item.quantity--;

        if (item.quantity <= 0)
        {
            // ===== CHANGED: Xóa kh?i Hotbar container =====
            var hotbarContainer =
                player.Inventory[InventoryContainerType.Hotbar];

            hotbarContainer.items.Remove(item);
        }



        hotbar.Refresh();
    }

    IEnumerator Doing()
    {
        isAction = true;

        animator.SetTrigger("Doing");
        yield return new WaitForSeconds(1f);

        isAction = false;
    }

    #endregion

    #region Melee

    IEnumerator MeleeAttack()
    {
        isAction = true;

        animator.SetTrigger("MA");
        yield return new WaitForSeconds(4 / 6f);

        DoMeleeHit();

        yield return new WaitForSeconds(1 / 3f);
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

        yield return new WaitForSeconds(5 / 6f);

        ShootArrow();

        isRangedAiming = false;


        yield return new WaitForSeconds(1 / 6f);

        arrowDir.SetActive(false);
        isAction = false;
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(
            arrowPrefab,
            shootPoint.position,
            Quaternion.LookRotation(Vector3.forward, direct) * Quaternion.Euler(0, 0, 90)
        );

        //arrow.GetComponent<Rigidbody2D>().linearVelocity = direct.normalized * 15f;
        arrow.GetComponent<Rigidbody2D>().AddForce(direct * 10f, ForceMode2D.Impulse);

        Destroy(arrow, 3f);

    }

    void Aim()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = Mathf.Abs(Camera.main.transform.position.z); // kho?ng cách t? camera t?i world

        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mouse);

        direct = ((Vector2)worldMouse - (Vector2)transform.position).normalized;

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

        yield return new WaitForSeconds(0.4f);
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
            Vector3 vec = new Vector3(0, 0.5f, 0);
            Vector3 dropPos = c.transform.position - vec + Random.insideUnitSphere * 0.5f;
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

    #region Axe / Watering

    public IEnumerator Axe()
    {
        animator.SetTrigger("Axe");
        isAction = true;
        yield return new WaitForSeconds(1f);
        DropWood();
        isAction = false;
    }

    void DropWood()
    {
        Collider2D[] Woods = Physics2D.OverlapCircleAll(
            attackPoint.position,
            1.5f,
            LayerMask.GetMask("Tree")
        );

        foreach (var c in Woods)
        {
            Vector3 vec = new Vector3(0, 1.5f, 0);
            Vector3 dropPos = c.transform.position - vec - Random.insideUnitSphere * 0.5f; 
            dropPos.z = 0;

            if (c.CompareTag("Tree"))
                Instantiate(wood, dropPos, Quaternion.identity);
            
        }
    }

    public IEnumerator Water()
    {
        animator.SetTrigger("Water");
        isAction = true;
        yield return new WaitForSeconds(1f);
        isAction = false;
    }

    #endregion

    IEnumerator Roll()
    {
        if (EnoughMP(10))
        {
            isRolling = true;
            PlayerRuntime.Instance.Player.Mp -= 10;
            animator.SetTrigger("Roll");
            GetComponent<P_Interact>().isImmune = true;
            yield return new WaitForSeconds(1f);
            GetComponent<P_Interact>().isImmune = false;
            isRolling = false;
        }
        else
        {
            Debug.Log("Not enough MP to roll!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.transform.position, 1.5f);        
    }
}

