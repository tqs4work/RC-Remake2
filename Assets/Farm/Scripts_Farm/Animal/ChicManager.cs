using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

public class ChicManager : MonoBehaviour
{
    /*Quản lý gà 
     * 1. Gà có 3 trạng thái: Baby, Teen, Adult
     * 2. Trạng thái baby: gà sẽ đói sau 3s, sau khi được cho ăn 3 lần sẽ chuyển sang trạng thái teen
     * 3. Trạng thái teen: gà sẽ đói sau 5s, sau khi được cho ăn 3 lần sẽ chuyển sang trạng thái adult, mỗi lần cho ăn sẽ cho 1 trứng
     * 4. Trạng thái adult: gà sẽ tiến vào giai đoạn trưởng thành, xuất hiện button bán gà, bán gà sẽ được tiền và thịt
    */
    public enum ChicState
    {
        Baby,
        Teen,
        Adult
    }
    public ChicState currentState = ChicState.Baby;

    [Header("Baby Settings")]
    public float hungerInterval = 3f; // thời gian gà sẽ đói (trong 3s)
    private float hungerTimer;
    private bool isHungry = false;
    private int timesFed = 0;//số lần cho ăn 

    [Header("Teen Settings")]
    public float teenHungerInterval = 5f; // thời gian gà sẽ đói (trong 5s)
    private float teenHungerTimer;
    private bool isTeenHungry = false;
    private int teenTimesFed = 0;//số lần cho ăn
    private int eggsProduced = 0;//số trứng đã sản xuất
    public GameObject eggPrefab; // Prefab của trứng

    [Header("Adult Settings")]
    public GameObject adultUIPopup; // UI popup khi gà trưởng thành
    public GameObject meatPrefab; // Prefab của thịt gà
    public int moneyReward = 1000; // Số tiền nhận được khi bán gà trưởng thành

    [Header("Chic model")]
    public GameObject babyModel;
    public GameObject teenModel;
    public GameObject AdultModel;

    [Header("UI")]
    public Image food;
    public Image sellIcon;
    public Button Interact;

    [Header("Movement Settings")]
    public float moveSpeed = 1.5f; // Tốc độ đi bộ
    public float moveDistance = 2f; // Khoảng cách đi tối đa từ vị trí ban đầu
    public float minIdleTime = 1f; // Thời gian đứng chơi ít nhất
    public float maxIdleTime = 3f; // Thời gian đứng chơi lâu nhất
    private Vector3 startPos; // Lưu vị trí lúc mới sinh ra để gà không đi quá xa
    private bool isWalking = false;

    public AudioSource AudioSource;
    public AudioClip eatSound;
    public Transform chicVisuals;

    public TextMeshProUGUI noti;
    private void Start()
    {
        //Khởi tạo timer cho trạng thái baby
        hungerTimer = hungerInterval;
        babyModel.SetActive(true);
        teenModel.SetActive(false);
        AdultModel.SetActive(false);
        adultUIPopup.SetActive(false);
        Interact.gameObject.SetActive(false);
        food.enabled = false;
        sellIcon.enabled = false;

        startPos = transform.position; // Lưu lại mốc tọa độ ban đầu
        StartCoroutine(RoamRoutine()); // Bắt đầu tiến trình đi dạo
        noti.text = "";
    }
    private void Update()
    {
        if (currentState == ChicState.Baby)
        {
            if (isHungry)
            {
                ShowHungryUI();
            }
            else
            {
                hungerTimer -= Time.deltaTime;
                if (hungerTimer <= 0f)
                {
                    isHungry = true;
                    Debug.Log("Gà đói rồi! Hãy cho ăn!");
                    
                }
            }
        }
        else if (currentState == ChicState.Teen)
        {
            if (isTeenHungry)
            {
                ShowHungryUI();
            }
            else
            {
                teenHungerTimer -= Time.deltaTime;
                if (teenHungerTimer <= 0f)
                {
                    isTeenHungry = true;
                }
            }
        }
    }
    public void InteractWithChic()
    {
        switch(currentState)
        {
            case ChicState.Baby:
                {
                    if(isHungry)
                    {
                        FeedBaby();
                        break;
                    }
                    else
                    {
                        Debug.Log("Gà baby không đói!");
                    }
                    break;
                }
                case ChicState.Teen:
                {
                    if(isTeenHungry)
                    {
                        FeedTeen();
                        break;
                    }
                    else
                    {
                        Debug.Log("Gà teen không đói!");
                    }
                    break;
                }
                case ChicState.Adult:
                {
                    ShowAdultPopup();
                    break;
                }
        }    
    }
    private bool Feed()
    {
        var player = PlayerRuntime.Instance.Player;
        var farmInventory = player.Inventory[InventoryContainerType.Farm];
        var wheatItem = farmInventory.items.Find(x => x.itemID == "F1"); //  ID của thức ăn wheat là F1
        if (wheatItem == null || wheatItem.quantity <= 0)
        {
            Debug.Log("Bạn không có thức ăn để cho gà con ăn!");
            StartCoroutine(NotiMess("Bạn không có thức ăn để cho gà con ăn!", 2));
            return false;
        }
        wheatItem.quantity--; // Trừ 1 thức ăn
        Debug.Log("Đã sử dụng 1 thức ăn. Còn lại: " + wheatItem.quantity);
        StartCoroutine(NotiMess("Đã sử dụng 1 thức ăn. Còn lại: " + wheatItem.quantity, 2));
        if (wheatItem.quantity <= 0)
        {
            farmInventory.items.Remove(wheatItem); // Xóa khỏi túi nếu hết
            Debug.Log("Bạn đã dùng hết thức ăn!");
            return true;
        }
        return true;
    }    
    private void FeedBaby()
    {
        if (!Feed()) return;
        AudioSource.PlayOneShot(eatSound);
        Debug.Log("Sound of chic!");
        isHungry = false;
        hungerTimer = hungerInterval; // Reset lại đồng hồ đếm đói
        timesFed++;
        Debug.Log("Đã cho gà con ăn. Lần: " + timesFed);
        Interact.gameObject.SetActive(false);
        food.enabled = false;
        sellIcon.enabled = false;
        if (timesFed >= 3)
        {
            GrowToTeen();
        }
    }
    private void GrowToTeen()
    {
        currentState = ChicState.Teen;
        Debug.Log("Gà đã lớn thành Teen!");
        babyModel.SetActive(false);
        teenModel.SetActive(true);
        teenHungerTimer = teenHungerInterval;
        isTeenHungry = false;
    }
    private void FeedTeen()
    {
        if (!Feed()) return;
        AudioSource.PlayOneShot(eatSound);
        Debug.Log("Sound of chic!");
        isTeenHungry = false;
        teenHungerTimer = teenHungerInterval; // Reset lại đồng hồ đếm đói
        teenTimesFed++;
        Debug.Log("Đã cho gà teen ăn. Lần: " + teenTimesFed);
        Interact.gameObject.SetActive(false);
        food.enabled = false;
        sellIcon.enabled = false;
        // Tạo trứng mỗi lần cho ăn
        Instantiate(eggPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity); 
        eggsProduced++;
        Debug.Log("Gà đẻ 1 trứng. Tổng: " + eggsProduced);
        if (teenTimesFed >= 3)
        {
            GrowToAdult();
        }
    }
    private void GrowToAdult()
    {
        currentState = ChicState.Adult;
        Debug.Log("Gà đã trưởng thành! Không đẻ trứng nữa.");
        babyModel.SetActive(false);
        teenModel.SetActive(false);
        AdultModel.SetActive(true);
        Interact.gameObject.SetActive(false);
        food.enabled = false;
        sellIcon.enabled = false;
        StartCoroutine(WaitToEnableAdultInteract());
    }
    private IEnumerator WaitToEnableAdultInteract()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("Gà trưởng thành đã sẵn sàng để bán!");
        Interact.gameObject.SetActive(true);
        sellIcon.enabled = true;
    }
    private void ShowAdultPopup()
    {
        if (adultUIPopup != null)
        {
            adultUIPopup.SetActive(true);
        }
    }
    //Viết cho nút button nè
    public void SellAdultChic()
    {
        for(int i=0; i < eggsProduced; i++)
        {
            Instantiate(meatPrefab, transform.position + new Vector3(0.5f * i, 0, 0), Quaternion.identity); // Tạo thịt gà
        }
        PlayerRuntime.Instance.Player.Gold += moneyReward; // Thêm tiền vào tài khoản người chơi
        Debug.Log("Đã bán gà! Rớt 3 thịt và 1000 vàng.");
        //Tắt UI xóa con gà
        if(adultUIPopup != null)
        {
            adultUIPopup.SetActive(false);
        }
        Destroy(gameObject); // Xóa con gà khỏi game
    }
    public void CancelSell()
    {
        if (adultUIPopup != null) adultUIPopup.SetActive(false);
    }
    private IEnumerator RoamRoutine()
    {
        while (true) // Vòng lặp chạy liên tục đến khi gà bị bán hoặc xóa
        {
            // Đứng yên nghỉ ngơi một khoảng thời gian ngẫu nhiên
            isWalking = false;
            float waitTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(waitTime);

            // Đi ngẫu ngẫu nhiên (Trái hoặc Phải)
            // Chọn ngẫu nhiên -1 (trái) hoặc 1 (phải)
            float randomDir = Random.Range(0, 2) == 0 ? -1f : 1f;

            // Tính toán điểm đến dự kiến
            Vector3 targetPos = transform.position + new Vector3(randomDir * moveDistance, 0, 0);

            // Giới hạn không cho gà đi vượt quá moveDistance so với tâm điểm (startPos)
            if (targetPos.x > startPos.x + moveDistance) targetPos.x = startPos.x + moveDistance;
            if (targetPos.x < startPos.x - moveDistance) targetPos.x = startPos.x - moveDistance;

            // 3. Lật mặt con gà (Flip) để nhìn đúng hướng di chuyển
            // Chú ý: Giả định model gốc của bạn đang quay mặt sang phải. X = 1 là phải, -1 là trái.
            if (targetPos.x < transform.position.x)
            {
                chicVisuals.localScale = new Vector3(-1, 1, 1); // Lật sang trái
            }
            else if (targetPos.x > transform.position.x)
            {
                chicVisuals.localScale = new Vector3(1, 1, 1); // Lật sang phải
            }

            // 4. Bắt đầu di chuyển từ từ đến targetPos
            isWalking = true;
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                // Dùng MoveTowards để dịch chuyển mượt mà từng frame
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                // Tạm dừng Coroutine chờ đến frame tiếp theo rồi mới chạy tiếp vòng lặp while này
                yield return null;
            }
        }
    }
    public ChickenData SaveChic()
    {
        ChickenData data = new ChickenData();
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        data.posZ = transform.position.z;

        // Lưu trạng thái
        data.state = currentState;
        data.eggsProduced = eggsProduced;

        if (currentState == ChicState.Baby)
        {
            data.currentTimer = hungerTimer;      
            data.isHungryStatus = isHungry;       
            data.feedCount = timesFed;            
        }
        else if (currentState == ChicState.Teen)
        {
            data.currentTimer = teenHungerTimer;  
            data.isHungryStatus = isTeenHungry;   
            data.feedCount = teenTimesFed;        
        }
        // Adult thì không cần lưu timer hay đói, chỉ cần vị trí và state là đủ
        return data;
    }
    public void LoadData(ChickenData data)
    {
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        startPos = transform.position; // Cập nhật lại điểm gốc đi dạo

        // Khôi phục các chỉ số chung
        currentState = data.state;
        eggsProduced = data.eggsProduced;

        // Tắt hết model để bật lại cái đúng
        babyModel.SetActive(false);
        teenModel.SetActive(false);
        AdultModel.SetActive(false);
        Interact.gameObject.SetActive(false); // Mặc định tắt nút tương tác

        if (currentState == ChicState.Baby)
        {
            babyModel.SetActive(true);
            hungerTimer = data.currentTimer;
            isHungry = data.isHungryStatus;
            timesFed = data.feedCount;
            if (isHungry) ShowHungryUI();
        }
        else if (currentState == ChicState.Teen)
        {
            teenModel.SetActive(true);
            teenHungerTimer = data.currentTimer;
            isTeenHungry = data.isHungryStatus;
            teenTimesFed = data.feedCount;
            if (isTeenHungry) ShowHungryUI();
        }
        else if (currentState == ChicState.Adult)
        {
            AdultModel.SetActive(true);
            StartCoroutine(WaitToEnableAdultInteract());
        }
    }

    void ShowHungryUI()
    {
        Interact.gameObject.SetActive(true);
        food.enabled = true;
        sellIcon.enabled = false;
    }
    IEnumerator NotiMess(string message, int time)
    {
        noti.text = message;
        yield return new WaitForSeconds(time);
        noti.text = ""; 
    }    
}
