using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopInv : MonoBehaviour
{
    //Inventory
    public GameObject inventorySlotsParent; //Panel chứa các slot trong inventory
    public Image IconDrag;

    //Miêu tả item
    public GameObject itemDesciptionParent;
    //public GameObject worldItemInfoPrefab;
    public Image itemDescriptionIcon;
    public TextMeshProUGUI itemDescriptionName;
    public TextMeshProUGUI itemDescription;

    //Danh sách slot
    //public List<Slots> inventorySlots = new List<Slots>(); //Danh sách các slot trong inventory, sẽ được khởi tạo từ các slot con của inventorySlotsPanel
    //public List<Slots> allSlots = new List<Slots>(); //Danh sách tất cả các slot trong game, bao gồm cả các slot trong inventory và các slot khác (như slot của nhân vật, slot của cửa hàng, v.v.)

    //private Slots draggedSlot = null; //Biến tạm để lưu slot đang được kéo thả, sẽ được sử dụng trong các sự kiện kéo thả để xác định slot nguồn và slot đích
    private bool isDragging = false; //Kiểm tra xem có đang kéo item hay không
}
