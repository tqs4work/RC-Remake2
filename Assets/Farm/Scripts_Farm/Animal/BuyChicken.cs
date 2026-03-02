using UnityEngine;

public class BuyChicken : MonoBehaviour
{
    [Header("Shop settings")]
    public GameObject chickenPrefab; // Prefab của gà
    public int chickenPrice = 500; 
    public Transform[] spawnPoint; // Điểm xuất hiện của gà sau khi mua
    [Header("UI Hint")]
    public GameObject hintUI;
    public int index = 0; // Chỉ số điểm xuất hiện gà, mặc định là 0
    private bool isPlayerNear = false;

    private void Start()
    {
        if(hintUI!= null)
        {
            hintUI.SetActive(false);
        }
    }
    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            BuyChic();
        }
    }
    private void BuyChic()
    {
        if (PlayerRuntime.Instance.Player.Gold >= chickenPrice && index <6) // chỉ cho mua 6 con gà 0-5
        {
            PlayerRuntime.Instance.Player.Gold -= chickenPrice;
            Debug.Log("Đã mua gà thành công! Tiền còn lại: " + PlayerRuntime.Instance.Player.Gold);

            if (chickenPrefab != null && spawnPoint != null)
            {
                Instantiate(chickenPrefab, spawnPoint[index].position, Quaternion.identity);
            }
        }
        else if(index >= 6)
        {
            Debug.Log("Đã mua tối đa số gà! Không thể mua thêm.");
        }
        else
        {
            Debug.Log(" không đủ " + chickenPrice + " vàng để mua gà!");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (hintUI != null) hintUI.SetActive(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (hintUI != null) hintUI.SetActive(false); 
        }
    }


}
