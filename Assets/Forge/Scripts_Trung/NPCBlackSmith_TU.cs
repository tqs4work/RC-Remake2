using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using NUnit.Framework;
/* Script dùng cho NPC thợ rèn trong game
* Gắn trên NPC thợ rèn
* Có dialogue khi tương tác
* có menu lựa chọn (Trade / Repair / Stone / Upgrade / Exit)
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
    public TMP_Text npcNameText; //Text tên NPC
    public TMP_Text dialogueText; //Text nội dung hội thoại
    public Image npcPortraitImage; //Image ảnh đại diện NPC
    [Header("UI - Choices")]
    public GameObject choicesUI; //Panel UI lựa chọn
    public Button buttonTrade;
    public Button buttonRepair;
    public Button buttonStone;
    public Button buttonUpgrade;
    public Button buttonExit;
    [Header("External Panels")] // Các panel khác mở từ lựa chọn
    [SerializeField] GameObject ShopPanel;
    [SerializeField] GameObject RepairPanel;
    [SerializeField] GameObject StonePanel;
    [SerializeField] GameObject weaponUpgradePanel;
    [SerializeField] GameObject inventoryPanel;
    //Refence runtime (Lấy component trong runtime)
    ShopUI_TU shopUI; // Tham chiếu UI shop

    RepairUI_TU repairUI; // Tham chiếu UI repair
    StoneUpgradeUI_TU stoneUpgradeUI;   // Tham chiếu UI stone upgrade
    WeaponUpgradeUI_TU weaponUpgradeUI; // Tham chiếu UI weapon upgrade
    [Header("Player dections")] // Phát hiện player gần
    public Transform playerTransform; 
    public float talkRadius = 3.0f; //Bán kính nói chuyện
    [Header("SFX - Hammer (One Shot)")]
    public AudioClip hammerSound; //Clip tiếng búa đập
    public AudioSource hammerSource; //AudioSource phát âm thanh búa đập
    public float hearRange = 3.2f; //Khoảng cách nghe thấy tiếng búa đập
    public float hitInterval = 0.9f; //Khoảng thời gian giữa các lần búa đập
    public Vector2 intervalJitter = new Vector2(-0.08f, 0.08f); //Độ nhiễu thời gian giữa các lần búa đập
    [Range(0f, 1f)] public float hitVolume = 0.5f; //Âm lượng tiếng búa đập
    float hitTimer = 0f; //Bộ đếm thời gian giữa các lần búa đập
    bool hammerPausedByPanel = false; //Có tạm dừng búa đập do mở panel khác không?
    Coroutine hitsCoroutine; //Tham chiếu Coroutine búa đập
    int index = 0; //Dòng hội thoại hiện tại
    bool isTyping = false; //Đang gõ chữ hay không
    Coroutine typingCoroutine; //Tham chiếu Coroutine gõ chữ
    bool suppressAdvance = false; //Có đang chặn tự động chuyển câu không (do mở menu lựa chọn)
    bool waitingExternal = false; //Có đang chờ đóng panel bên ngoài không
    bool exitLineShowing = false; //Có đang hiển thị dòng exit không
    static float reopenBlockUntil = 0f; //Thời gian chặn mở lại hội thoại (sau khi đóng)






    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
