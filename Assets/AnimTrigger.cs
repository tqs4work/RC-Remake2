using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    public SpriteSwap DoorOpen;

    private bool playerInRange;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
        DoorOpen.CloseDoor();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DoorOpen.OpenDoor();
        }
    }
}
