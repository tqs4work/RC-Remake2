using UnityEngine;
using System.Collections;

public class SpriteSwap : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject pharmacyPanel;

    [Header("Animation Frames")]
    [SerializeField] public Sprite[] openAnimation;

    [Header("Settings")]
    [SerializeField] public float frameRate = 0.1f; // Time between frames in seconds

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    [SerializeField] public bool isOpen = false;
    [SerializeField] public bool isAnimating = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        pharmacyPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public IEnumerator OpenDoor()
    {
        if (!isAnimating)
        {
            StartCoroutine(PlayAnimation());
            yield return new WaitForSeconds(0.5f); // Wait for animation to complete
            pharmacyPanel.SetActive(true);           
        }
    }

    public void CloseDoor()
{
    if (isOpen && !isAnimating)
    {
        StartCoroutine(PlayAnimation());
        pharmacyPanel.SetActive(false);
    }
}

    public IEnumerator PlayAnimation()
    {
        isAnimating = true;

        if (!isOpen)
        {
            for (int i = 0; i < openAnimation.Length; i++) // Play open animation
            {
                spriteRenderer.sprite = openAnimation[i];
                yield return new WaitForSeconds(frameRate);
            }
        }
        else
        {
            for (int i = openAnimation.Length - 1; i >= 0; i--) // Play close animation in reverse
            {
                spriteRenderer.sprite = openAnimation[i];
                yield return new WaitForSeconds(frameRate);
            }
        }

        isOpen = !isOpen;
        doorCollider.enabled = !isOpen;

        isAnimating = false;
    }
}