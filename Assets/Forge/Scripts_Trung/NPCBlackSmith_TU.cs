using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class NPCBlackSmith_TU : MonoBehaviour
{
    public static bool DialogueOpen { get; private set; }

    [Header("Dialogue Data")]
    public BlackSmithDialogue_TU dialogueData;

    [Header("Dialogue UI")]
    public GameObject dialogueUI;
    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public Image npcPortraitImage;

    [Header("Choice UI")]
    public GameObject choicesUI;
    public Button buttonTrade;
    public Button buttonRepair;
    public Button buttonStone;
    public Button buttonUpgrade;
    public Button buttonExit;

    [Header("Panels")]
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject repairPanel;
    [SerializeField] GameObject stonePanel;
    [SerializeField] GameObject weaponUpgradePanel;
    [SerializeField] GameObject inventoryPanel;

    ShopUI_TU shopUI;
    RepairUI_TU repairUI;
    StoneUpgradeUI_TU stoneUI;
    WeaponUpgradeUI_TU weaponUI;

    [Header("Player")]
    public Transform playerTransform;
    public float talkRange = 3f;

    [Header("Hammer SFX")]
    public AudioClip hammerSound;
    public AudioSource hammerSource;
    public float hearRange = 3.2f;
    public float hitInterval = 0.9f;
    public Vector2 intervalJitter = new Vector2(-0.08f, 0.08f);
    [Range(0f,1f)] public float hitVolume = 0.5f;

    Coroutine hammerCo;
    bool hammerPaused;

    int index;
    bool isTyping;
    Coroutine typingCo;
    bool waitingExternal;
    bool exitLine;
    static float reopenBlock;

    void Awake()
    {
        dialogueUI.SetActive(false);
        choicesUI.SetActive(false);

        shopPanel?.SetActive(false);
        repairPanel?.SetActive(false);
        stonePanel?.SetActive(false);
        weaponUpgradePanel?.SetActive(false);
        inventoryPanel?.SetActive(false);

        buttonTrade?.onClick.AddListener(OpenShop);
        buttonRepair?.onClick.AddListener(OpenRepair);
        buttonStone?.onClick.AddListener(OpenStone);
        buttonUpgrade?.onClick.AddListener(OpenWeaponUpgrade);
        buttonExit?.onClick.AddListener(ShowExitLine);

        shopUI = shopPanel?.GetComponent<ShopUI_TU>();
        repairUI = repairPanel?.GetComponent<RepairUI_TU>();
        stoneUI = stonePanel?.GetComponent<StoneUpgradeUI_TU>();
        weaponUI = weaponUpgradePanel?.GetComponent<WeaponUpgradeUI_TU>();

        if (repairUI != null) repairUI.onClose += ResumeDialogue;
        if (shopUI != null) shopUI.onShopClosed += ResumeDialogue;
    }

    void Update()
    {
        HandleHammer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
            return;
        }

        if (!dialogueUI.activeSelf || waitingExternal) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                FinishTyping();
            else if (exitLine)
                CloseDialogue();
        }
    }

    void OnMouseDown()
    {
        if (Time.time < reopenBlock) return;

        if (Vector2.Distance(transform.position, playerTransform.position) > talkRange)
            return;

        if (!DialogueOpen)
            StartDialogue();
    }

    void StartDialogue()
    {
        if (!dialogueData || dialogueData.lines.Length == 0) return;

        DialogueOpen = true;
        index = 0;
        exitLine = false;

        dialogueUI.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        var line = dialogueData.lines[index];

        npcNameText.text = dialogueData.npcName;
        dialogueText.text = "";

        npcPortraitImage.sprite = line.linePortrait 
            ? line.linePortrait 
            : dialogueData.npcPortrait;

        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = StartCoroutine(TypeLine(line.lineText));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;

        if (index == 0)
            choicesUI.SetActive(true);
    }

    void FinishTyping()
    {
        StopCoroutine(typingCo);
        dialogueText.text = dialogueData.lines[index].lineText;
        isTyping = false;
        choicesUI.SetActive(true);
    }

    void OpenShop()
    {
        OpenExternal(shopPanel);
    }

    void OpenRepair()
    {
        OpenExternal(repairPanel);
        repairUI?.Open();
    }

    void OpenStone()
    {
        OpenExternal(stonePanel);
        stoneUI?.Open();
    }

    void OpenWeaponUpgrade()
    {
        OpenExternal(weaponUpgradePanel);
        weaponUI?.Open();
    }

    void OpenExternal(GameObject panel)
    {
        waitingExternal = true;
        choicesUI.SetActive(false);
        dialogueUI.SetActive(false);

        inventoryPanel?.SetActive(true);
        panel?.SetActive(true);

        PauseHammer();
    }

    void ResumeDialogue()
    {
        shopPanel?.SetActive(false);
        repairPanel?.SetActive(false);
        stonePanel?.SetActive(false);
        weaponUpgradePanel?.SetActive(false);
        inventoryPanel?.SetActive(false);

        waitingExternal = false;
        dialogueUI.SetActive(true);
        choicesUI.SetActive(true);

        ResumeHammer();
    }

    void ShowExitLine()
    {
        choicesUI.SetActive(false);
        exitLine = true;
        index = Mathf.Min(1, dialogueData.lines.Length - 1);
        ShowLine();
    }

    void CloseDialogue()
    {
        dialogueUI.SetActive(false);
        choicesUI.SetActive(false);

        DialogueOpen = false;
        exitLine = false;
        waitingExternal = false;

        reopenBlock = Time.time + 0.25f;
    }

    void HandleHammer()
    {
        if (!hammerSound || !hammerSource || !playerTransform || hammerPaused)
            return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= hearRange && hammerCo == null)
            hammerCo = StartCoroutine(HammerLoop());
        else if (dist > hearRange && hammerCo != null)
            StopHammer();
    }

    IEnumerator HammerLoop()
    {
        while (true)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);

            if (dist > hearRange) break;

            float volume = hitVolume * (1 - (dist / hearRange));
            hammerSource.PlayOneShot(hammerSound, volume);

            float wait = hitInterval + Random.Range(intervalJitter.x, intervalJitter.y);
            yield return new WaitForSeconds(wait);
        }

        hammerCo = null;
    }

    void StopHammer()
    {
        if (hammerCo != null)
            StopCoroutine(hammerCo);

        hammerCo = null;
    }

    void PauseHammer()
    {
        hammerPaused = true;
        StopHammer();
    }

    void ResumeHammer()
    {
        hammerPaused = false;
    }
}
