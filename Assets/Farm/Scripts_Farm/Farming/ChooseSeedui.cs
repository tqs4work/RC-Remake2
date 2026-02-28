using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSeedui : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button chooseButton;
    private PlantData data;

    public void Setup(PlantData plantData)
    {
        data = plantData;
        nameText.text = data.seedName;
        priceText.text = data.buyPrice.ToString() + "G";
        if (data.seedIcon != null)
        {
            iconImage.sprite = data.seedIcon;
        }
        chooseButton.onClick.AddListener(OnSelect);
    }

    // Gán vào sự kiện onclick của Button trong prefab ChooseSeedui
    public void OnSelect()
    {
        Debug.Log("BUTTON ĐÃ ĐƯỢC NHẤN!");
        FarmController_TA player = FindAnyObjectByType<FarmController_TA>();

        if (player != null && data != null  ) //&&PlayerRuntime.Instance.Player.Gold > data.buyPrice - đổi con player trong login mới check đc 
        {
            //trừ tiền 
            //PlayerRuntime.Instance.Player.Gold -= data.buyPrice;


            //Lấy pos
            Vector3Int cellPos = player.GetCurrentTargetCell(); 
            Debug.Log("Vị trí hiện tại để gieo hạt: " + cellPos);
            // Gieo 
            player.PlantSeedAtCurrentPos(data);
            Debug.Log("Đã gieo hạt: " + data.seedName + " tại vị trí " + cellPos);

            // đóng menu
            player.seedMenuPanel.SetActive(false);
        }
    }
}
