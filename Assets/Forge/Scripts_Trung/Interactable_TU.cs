using UnityEngine;
using UnityEngine.Events;
/* Script dùng cho mọi tương tác trong game (nhặt đồ, mở cửa, nói chuyện...)
* mà Player có thể tương tác được.(Nhấn E/Click chuột)
* Script này:
* ✔ Bắt input tương tác
* ✔ Kiểm tra player có ở gần không
* ✔ Gọi hành động thông qua UnityEvent

* Script này KHÔNG:
* ✘ Không chứa logic NPC / cửa / shop
* ✘ Không xử lý UI
*/
[RequireComponent(typeof(Collider2D))] //Bắt buộc phải có Collider2D (để phát hiện vùng tương tác)
public class Interactable_TU : MonoBehaviour
{
   [Header("Activation")]
   public KeyCode interactKey = KeyCode.E; //Phím tương tác (mặc định là E)
   [Tooltip("Dùng Trigger Zone để xác định player đang ở gần.")]
   public bool useTriggerZone = true; //true: dùng OnTriggerEnter2D/Exit2D; false: dùng OnCollisionEnter2D/Exit2D
   [Tooltip("Nếu không dùng Trigger Zone, sẽ kiểm tra bán kính tương tác mỗi frame.")]
   public float interactRadius = 2.0f; //Bán kính tương tác (nếu không dùng Trigger Zone)
   [Header("References")]
   [Tooltip("Transform của player để kiểm tra khoảng cách.")]
   public Transform playerTransform; //Tham chiếu Player để tinh khoảng cách
   [Header("Events")] 
   public UnityEvent onInteract; //UnityEvent cho gắn hành động tương tác
   bool playerInRange = false; //Player có đang ở gần không
   Collider2D col2D; //Tham chiếu Collider2D của object này

   void Awake()
   {
       // Lấy Collider2D gắn trên object này
       col2D = GetComponent<Collider2D>();
       //Nếu dùng Trigger Zone -> bật isTrigger
       if (useTriggerZone)
       {
           col2D.isTrigger = true;
       }
       //Nếu chưa gắn playerTransform -> tìm player theo tag "Player"
       if (playerTransform == null)
       {
           var playerObj = GameObject.FindGameObjectWithTag("Player");
           if (playerObj != null)
           {
               playerTransform = playerObj.transform;
           }
           else
           {
               Debug.LogError("Interactable_TU: Không tìm thấy Player trong scene. Vui lòng gán playerTransform thủ công.");
           }
       }
   }
   void Update()
   {
       //Nhấn phím E tương tác
       // + Dialogue hiện tại chưa mở
       // + Player đang ở gần
       if (Input.GetKeyDown(interactKey) && !FindObjectOfType<NPCBlackSmith_TU>().DialogueOpen && IsPlayerCloseEnough())
       {
           Interact();
       }
   }
   public void Interact()
   {
       //Gọi toàn bộ hàm đã gắn trong Inspector
       onInteract.Invoke();
   }
   bool IsPlayerCloseEnough()
   {
       //Nếu dùng Trigger Zone -> kiểm tra biến playerInRange
       if (useTriggerZone)
       {
           return playerInRange;
       }
       else //Nếu không dùng Trigger Zone -> kiểm tra khoảng cách mỗi frame
       {
           if (playerTransform == null) return false;
           float dist = Vector2.Distance(playerTransform.position, transform.position);
           return dist <= interactRadius;
       }
   }
   //Player vào vùng Trigger
   void OnTriggerEnter2D(Collider2D other)
   {
       // Nếu không dùng Trigger Zone thì không xử lý
       if (!useTriggerZone) return;
       if (other.transform == playerTransform)
       {
           playerInRange = true;
       }
   }
   //Player rời vùng Trigger
   void OnTriggerExit2D(Collider2D other)
   {
       // Nếu không dùng Trigger Zone thì không xử lý
       if (!useTriggerZone) return;
       if (other.transform == playerTransform)
       {
           playerInRange = false;
       }
   }
   #if UNITY_EDITOR
   //Vẽ bán kính tương tác trong Scene view
   void OnDrawGizmosSelected()
   {
       //Chỉ vẽ nếu không dùng Trigger Zone
       if (!useTriggerZone)
       {
           Gizmos.color = Color.yellow;
           Gizmos.DrawWireSphere(transform.position, interactRadius);
       }
   }
   #endif
}