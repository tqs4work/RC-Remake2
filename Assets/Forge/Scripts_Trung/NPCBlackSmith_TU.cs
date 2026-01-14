using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
/* Script dùng cho NPC thợ rèn trong game
* Có dialogue khi tương tác
* Có âm thanh búa đập khi player ở gần
*/

[RequireComponent(typeof(Collider2D))] //Bắt buộc phải có Collider2D (để phát hiện vùng tương tác)
public class NPCBlackSmith_TU : MonoBehaviour
{
    //Biến static để báo cho toàn game biết Dialogue đang mở hay không
    public static bool DialogueOpen { get; private set; } = false;
    [Header("Data")]
    public BlackSmithDialogue_TU dialogueData; //ScriptableObject chứa dữ liệu hội thoại thợ rèn
    [Header("UI - Dialogue")]
    public GameObject dialogueUI; //Panel UI hội thoại
    public TMP_Text npcNameText;
    public TMP_Text dialogueText; //Text nội dung hội thoại
    public Image npcPortraitImage; //Image ảnh đại diện NPC
    [Header("UI - Choices")]
    public GameObject choicesUI; //Panel UI lựa chọn
    public Button buttonTrade;
    public Button buttonRepair;
    public Button buttonStone;
    public Button buttonUpgrade;
    public Button buttonExit;
    [Header("External Panels")]
    [SerializeField] GameObject ShopPanel;
    [SerializeField] GameObject RepairPanel;
    [SerializeField] GameObject StonePanel;
    [SerializeField] GameObject WeaponUpgradePanel;
    [SerializeField] GameObject InventoryPanel;
    //Refence runtime (Lấy component trong runtime)
    ShopUI_TU shopUI; // Tham chiếu UI shop

    RepairUI_TU repairUI; // Tham chiếu UI repair
    StoneUpgradeUI_TU stoneUpgradeUI;   // Tham chiếu UI stone upgrade
    WeaponUpgradeUI_TU weaponUpgradeUI; // Tham chiếu UI weapon upgrade
    [Header("Player dections")] // Phát hiện player gần
    public Transform playerTransform; 
    public float talkRange = 3.0f;
    [Header("SFX - Hammer (One Shot)")]
    public AudioClip hammerSound; //Clip tiếng búa đập
    public AudioSource hammerSource; //AudioSource phát âm thanh búa đập
    public float hearRange = 3.2f; //Khoảng cách nghe thấy tiếng búa đập
    public float hitInterval = 0.9f; //Khoảng thời gian giữa các lần búa đập
    public Vector2 intervalJitter = new Vector2(-0.08f, 0.08f); //Độ nhiễu thời gian giữa các lần búa đập
    [Range(0f, 1f)] public float hitVolume = 0.5f; //Âm lượng tiếng búa đập
    float hitTimer = 0f; //Bộ đếm thời gian giữa các lần búa đập
    bool hammerPausedByPanel = false; //Có tạm dừng búa đập do mở panel khác không?
    Coroutine hitsCo; 
    [Header("Dialogue State (Runtime)")]
    int index = 0; 
    bool isTyping = false; 
    Coroutine typingCo; 
    bool suppressAdvance = false; //Có đang chặn tự động chuyển câu không (do mở menu lựa chọn)
    bool waitingExternal = false; //Có đang chờ đóng panel bên ngoài không
    bool exitLineShowing = false; //Có đang hiển thị dòng exit không
    static float reopenBlockUntil = 0f; //Thời gian chặn mở lại hội thoại (sau khi đóng)

    // tắt
    void Awake()
    {
        //Tắt toàn bộ UI khi vừa load scene
        if (ShopPanel) ShopPanel.SetActive(false);
        if (RepairPanel) RepairPanel.SetActive(false);
        if (StonePanel) StonePanel.SetActive(false);
        if (WeaponUpgradePanel) WeaponUpgradePanel.SetActive(false);   
        if (InventoryPanel) InventoryPanel.SetActive(false);
        if (dialogueUI) dialogueUI.SetActive(false);
        if (choicesUI) choicesUI.SetActive(false); 
        
        if(buttonTrade != null)
        {
            buttonTrade.onClick.AddListener(OnChooseTrade);
        }
        if(buttonRepair != null)
        {
            buttonRepair.onClick.AddListener(OnChooseRepair);
        }
        if(buttonStone != null)
        {
            buttonStone.onClick.AddListener(OnChooseStone);
        }
        if(buttonUpgrade != null)
        {
            buttonUpgrade.onClick.AddListener(OnChooseWeaponUpgrade);
        }
        if(buttonExit != null)
        {
            buttonExit.onClick.AddListener(OnChooseExit);
        }
        //Bắt sự kiện đóng panel bên ngoài
        if(ShopPanel)
        {
            shopUI = ShopPanel.GetComponent<ShopUI_TU>();
            if (shopUI != null)
            {
                shopUI.onShopClosed.AddListener(ResumeAfterExternalPanel);
            }
        }
        if(RepairPanel)
        {
            repairUI = RepairPanel.GetComponent<RepairUI_TU>();
            if (repairUI != null)
            {
                repairUI.onRepairClosed.AddListener(ResumeAfterExternalPanel);
            }
        }
        if(StonePanel)
        {
            stoneUpgradeUI = StonePanel.GetComponent<StoneUpgradeUI_TU>();
            if (stoneUpgradeUI != null)
            {
                stoneUpgradeUI.onStoneUpgradeClosed.AddListener(ResumeAfterExternalPanel);
            }
        }
        if(WeaponUpgradePanel)
        {
            weaponUpgradeUI = WeaponUpgradePanel.GetComponent<WeaponUpgradeUI_TU>();
            if (weaponUpgradeUI != null)
            {
                weaponUpgradeUI.onWeaponUpgradeClosed.AddListener(ResumeAfterExternalPanel);
            }
        }
        //AudioSource cho PlayOneShot búa đập
        if (hammerSource == null)
        {
            hammerSource.playOnAwake = false;//Không phát âm thanh khi load
            hammerSource.spatialBlend = 0f; //2D sound
            hammerSource.loop = false; 
            hammerSource.minDistance = 0.5f;//Khoảng cách bắt đầu giảm âm lượng
            hammerSource.maxDistance = hearRange; //Khoảng cách tối đa nghe thấy âm thanh
        }
        //Đảm bảo nghe tiếng búa >= khoảng cách nói chuyện
        if (hearRange < talkRange)
        {
            hearRange = talkRange;
        }
    }

    void Start()
    {
        PlayHammerIfNear();
    }
    void Update()
    {
        // Kiểm soát one-shot theo khoảng cách
        HandleHammerSFXByProximity();

        // chặn nói chuyện khi đứng xa
        if (playerTransform && !DialogueOpen)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist > talkRange) return;
        }

        // ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
            return;
        }

        if (!dialogueUI || !dialogueUI.activeSelf) return;
        if (waitingExternal) return;

        bool pressed = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0);
        if (!pressed || suppressAdvance) return;

        if (isTyping)
        {
            FinishTypingInstant();
            return;
        }

        if (exitLineShowing)
        {
            ClosePanel();
            return;
        }
    }
    void OnMouseDown()
    {
        if (Time.time < reopenBlockUntil) return;

        if (playerTransform)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist > talkRange) return;
        }

        if (!DialogueOpen) StartDialogue();
        else
        {
            if (isTyping) FinishTypingInstant();
            else if (exitLineShowing) ClosePanel();
        }
    }
    public void StartDialogue()
    {
        if (DialogueOpen) return;
        if (Time.time < reopenBlockUntil) return;
        if (!dialogueData || dialogueData.lines == null || dialogueData.lines.Length == 0) return;

        index = 0;
        exitLineShowing = false;

        OpenPanel();
        ShowCurrentLine();

        // mở hội thoại vẫn nghe búa (trừ khi mở panel con)
        hammerPausedByPanel = false;
        PlayHammerIfNear();
    }
    // 
    void ShowCurrentLine()
    {
        // Nếu
        var line = dialogueData.lines[Mathf.Clamp(index, 0, dialogueData.lines.Length - 1)];
        if (npcNameText) npcNameText.text = dialogueData.npcName;

        if (npcPortraitImage)
        {
            var p = line.linePortrait ? line.linePortrait : dialogueData.npcPortrait;
            npcPortraitImage.sprite = p;
            npcPortraitImage.enabled = (p != null);
        }

        if (typingCo != null) StopCoroutine(typingCo);
        float spd = (line.typeSpeed > 0f) ? line.typeSpeed : dialogueData.defaultTypeSpeed;
        typingCo = StartCoroutine(TypeRoutine(line.lineText, spd));
    }
    IEnumerator TypeRoutine(string text, float charDelay)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        isTyping = false;

        if (index == 0 && !waitingExternal && !exitLineShowing)
            ShowChoices();
    }
    void FinishTypingInstant()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = null;
        dialogueText.text = dialogueData.lines[Mathf.Clamp(index, 0, dialogueData.lines.Length - 1)].lineText;
        isTyping = false;

        if (index == 0 && !waitingExternal && !exitLineShowing)
            ShowChoices();
    }

    void ShowChoices() { if (choicesUI) choicesUI.SetActive(true); }
    void HideChoices() { if (choicesUI) choicesUI.SetActive(false); }
    void OnChooseTrade()
    {
        HideChoices();
        waitingExternal = true;
        PauseHammerForPanel();

        if (dialogueUI)  dialogueUI.SetActive(false);
        if (ShopPanel)  ShopPanel.SetActive(true);
        if (InventoryPanel) InventoryPanel.SetActive(true);

        if (shopUI == null && ShopPanel)
        {
            shopUI = shopUI.GetComponent<ShopUI_TU>();
            if (shopUI != null) shopUI.onShopClosed = ResumeAfterExternalPanel;
        }
    }
    void OnChooseRepair()
    {
        HideChoices();
        waitingExternal = true;
        PauseHammerForPanel();

        if (dialogueUI)  dialogueUI.SetActive(false);
        if (InventoryPanel) InventoryPanel.SetActive(true);
        if (RepairPanel) RepairPanel.SetActive(true);

        if (repairUI == null && RepairPanel)
        {
            repairUI = RepairPanel.GetComponent<RepairUI_TU>();
            if (repairUI != null) repairUI.onClose = ResumeAfterExternalPanel;
        }

        if (repairUI)
        {
            repairUI.Open();
            InventorySlotCell.RepairUIRef = repairUI;
        }
    }
    void OnChooseStone()
    {
        HideChoices();
        waitingExternal = true;
        PauseHammerForPanel();

        if (dialogueUI)  dialogueUI.SetActive(false);
        if (InventoryPanel) InventoryPanel.SetActive(true);
        if (StonePanel)  StonePanel.SetActive(true);

        if (stoneUpgradeUI == null && StonePanel)
        {
            stoneUpgradeUI = StonePanel.GetComponent<StoneUpgradeUI_TU>();
            if (stoneUpgradeUI != null) stoneUpgradeUI.onClose.AddListener(ResumeAfterExternalPanel);
        }

        if (stoneUpgradeUI)
        {
            stoneUpgradeUI.Open();
        }
    }
    void OnChooseWeaponUpgrade()
    {
        HideChoices();
        waitingExternal = true;
        PauseHammerForPanel();

        if (dialogueUI)  dialogueUI.SetActive(false);
        if (InventoryPanel) InventoryPanel.SetActive(true);
        if (WeaponUpgradePanel) WeaponUpgradePanel.SetActive(true);

        if (weaponUpgradeUI == null && weaponUpgradeUI)
        {
            weaponUpgradeUI = WeaponUpgradePanel.GetComponent<WeaponUpgradeUI_TU>();
            if (weaponUpgradeUI != null) weaponUpgradeUI.onClose.AddListener(ResumeAfterExternalPanel);
        }

        if (weaponUpgradeUI)
        {
            weaponUpgradeUI.Open();
        }
    }
    void OnChooseExit()
    {
        HideChoices();
        exitLineShowing = true;
        index = Mathf.Min(1, dialogueData.lines.Length - 1);
        ShowCurrentLine();

        suppressAdvance = true;
        StartCoroutine(ReleaseAdvanceGuard());
        // Không mute búa ở lựa chọn "Thoát".
    }
    void SayOnce(string t)
    {
        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = null;
        dialogueText.text = t;
        isTyping = false;
    }
    void ReturnToGreeting()
    {
        index = 0;
        exitLineShowing = false;
        ShowCurrentLine();
    }
    public void ResumeAfterExternalPanel()
    {
        if (ShopPanel)      ShopPanel.SetActive(false);
        if (InventoryPanel) InventoryPanel.SetActive(false);
        if (RepairPanel)    RepairPanel.SetActive(false);
        if (StonePanel)     StonePanel.SetActive(false);
        if (WeaponUpgradePanel) WeaponUpgradePanel.SetActive(false);

        InventorySlotCell.RepairUIRef = null;
        InventorySlotCell.StoneUpgradeUIRef = null;
        InventorySlotCell.WeaponUpgradeUIRef = null;

        waitingExternal = false;

        if (dialogueUI) dialogueUI.SetActive(true);
        ReturnToGreeting();

        hammerPausedByPanel = false;
        PlayHammerIfNear();

        suppressAdvance = true;
        StartCoroutine(ReleaseAdvanceGuard());
    }
    void OpenPanel()
    {
        if (dialogueUI) dialogueUI.SetActive(true);
        DialogueOpen = true;
        suppressAdvance = true;
        StartCoroutine(ReleaseAdvanceGuard());
    }
    IEnumerator ReleaseAdvanceGuard()
    {
        yield return null;
        while (Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
            yield return null;
        suppressAdvance = false;
    }
    void ClosePanel()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = null;

        if (ShopPanel)      ShopPanel.SetActive(false);
        if (RepairPanel)    RepairPanel.SetActive(false);
        if (StonePanel)     StonePanel.SetActive(false);
        if (WeaponUpgradePanel) WeaponUpgradePanel.SetActive(false);
        if (InventoryPanel) InventoryPanel.SetActive(false);
        if (choicesUI)    choicesUI.SetActive(false);
        if (dialogueUI)  dialogueUI.SetActive(false);

        InventorySlotCell.RepairUIRef = null;
        InventorySlotCell.StoneUpgradeUIRef = null;
        InventorySlotCell.WeaponUpgradeUIRef = null;

        isTyping = false;
        waitingExternal = false;
        exitLineShowing = false;
        DialogueOpen = false;

        reopenBlockUntil = Time.time + 0.25f;
        hammerPausedByPanel = false;
    }
    void HandleHammerSFXByProximity()
    {
        if (hammerPausedByPanel) return;
        if (!playerTransform || !hammerSource || !hammerSound) { StopHammerHits(); return; }

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= hearRange)
            PlayHammerIfNear();
        else
            StopHammerHits();
    }
    void PauseHammerForPanel()
    {
        hammerPausedByPanel = true;
        StopHammerHits();
    }


    //Phát âm thanh búa đập nếu player gần
    void PlayHammerIfNear()
    {
        // Nếu playerTransform, hammerSource hoặc hammerSound chưa gán thì thoát
        if (playerTransform == null || hammerSource == null || hammerSound == null) return;
        // Tính khoảng cách từ NPC đến player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);    
        // Nếu trong khoảng nghe thấy thì phát âm thanh với âm lượng giảm dần theo khoảng cách
        if (distanceToPlayer <= hearRange)
        {
            float volume = hitVolume * (1 - (distanceToPlayer / hearRange));
            hammerSource.PlayOneShot(hammerSound, volume);
        }
    }

    // Coroutine búa đập liên tục khi player gần
    IEnumerator Co_Hits()
    {
        // Chạy vòng lặp vô hạn
        while (playerTransform &&
           Vector2.Distance(transform.position, playerTransform.position) <= hearRange)
        {
            PlayHammerIfNear();
            float jitter = Random.Range(intervalJitter.x, intervalJitter.y);
            float waitTime = Mathf.Max(0.1f, hitInterval + jitter);
            yield return new WaitForSeconds(waitTime);
        }
        hitsCo = null;
    }
    void StopHammerHits()
    {
        if (hitsCo != null)
        {
            StopCoroutine(hitsCo);
            hitsCo = null;
        }
    }

}
