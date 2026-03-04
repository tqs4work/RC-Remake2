using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomMachine : MonoBehaviour
{
    [Header("Item Config")]
    [SerializeField] private Item[] items;
    [SerializeField] private Image[] slots;        // Đổi GameObject[] thành Image[]
    [SerializeField] private Image resultImage;
    private int playCost = 50;

    [Header("Animation Config")]
    [SerializeField] private float rollDuration = 3f;
    [SerializeField] private float rollSpeed = 0.1f;

    private bool isRolling = false;

    public void Play()
    {
        if (isRolling) return;

        if (!CurrencyMana.Instance.SpendCoin(playCost))
        {
            Debug.Log("Không đủ coin!");
            return;
        }
        Debug.Log("còn lại: " + PlayerRuntime.Instance.Player.Gold + " coin");

        StartCoroutine(PlayAnimation());
    }

    public void addItem()
    {
        
    }

    private IEnumerator PlayAnimation()
    {
        isRolling = true;

        float timer = 0f;

        while (timer < rollDuration)
        {
            foreach (var slot in slots)
            {
                slot.color = new Color(
                    Random.value,
                    Random.value,
                    Random.value
                );
            }

            yield return new WaitForSeconds(rollSpeed);
            timer += rollSpeed;
        }

        // Reset màu về trắng
        foreach (var slot in slots)
        {
            slot.color = Color.white;
        }

        // Random kết quả thật
        int randomIndex = Random.Range(0, items.Length);
        Item selectedItem = items[randomIndex];

        resultImage.sprite = selectedItem.icon;
        resultImage.enabled = true;

        Debug.Log($"Bạn nhận được: {selectedItem.itemName}");

        isRolling = false;
    }
}