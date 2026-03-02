using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-200)]
public class ItemTooltipUI_TU : MonoBehaviour
{
    // Singleton an toàn: có thể tìm thấy cả khi GameObject đang INACTIVE
    public static ItemTooltipUI_TU I { get; private set; }

    public enum FollowMode { Anchor, Cursor }

    [Header("Behavior")]
    public FollowMode followMode = FollowMode.Anchor;
    public Vector2 offset = new Vector2(12, -12);

    [Header("UI Refs")]
    public RectTransform root;     // Panel chính của tooltip (thường chính GameObject này)
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text statText;
    public TMP_Text descText;
    public Canvas parentCanvas;

    [Header("Camera (optional)")]
    public Camera uiCamera;        // dùng nếu Canvas là ScreenSpace-Camera/World

    RectTransform anchorTarget;
    bool visible;

    // --------- Bootstrap: đảm bảo I != null kể cả khi GameObject đang tắt ----------
    public static ItemTooltipUI_TU Ensure()
    {
        if (I == null)
        I = FindObjectOfType<ItemTooltipUI_TU>(true);

        return I;

    }

    void Awake()
    {
        I = this;

        parentCanvas = GetComponentInParent<Canvas>();
        root = GetComponent<RectTransform>();

        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();

        cg.blocksRaycasts = false;
        cg.interactable = false;

        transform.SetAsLastSibling();

        Hide();   // luôn reset khi scene load
    }

    void LateUpdate()
    {
        if (!visible) return;
        if (followMode == FollowMode.Anchor) FollowAnchor();
        else FollowCursor();
    }

    public void Show(Item item, int upgradeLevel, RectTransform anchor, int durabilityPercent = 100, bool showRepairPrice = false)
    {
        if (!item) return;
        Ensure(); // bảo đảm đã init
        anchorTarget = anchor;

        // icon
        if (iconImage)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = (item.icon != null);
        }

        // tên + màu theo cấp
        Color col = Item.GetColorByLevel(Mathf.Clamp(upgradeLevel, 0, 5));
        string hex = ColorUtility.ToHtmlStringRGB(col);
        string plus = upgradeLevel > 0 ? $" <color=#{hex}>(+{upgradeLevel})</color>" : "";

        if (nameText)
        {
            nameText.textWrappingMode = TextWrappingModes.Normal;
            nameText.richText = true;
            nameText.text = $"<color=#{hex}>{item.itemName}</color>{plus}";
        }

        // Nội dung
        bool isStone = (item.itemType == ItemType.Stone);

        if (!isStone)
        {
            item.GetFinalStats(upgradeLevel, out float fATK, out float fDEF, out float fCRIT);
            item.GetAddedAtLevel(upgradeLevel, out int aATK, out int aDEF, out float aC);

            if (item.itemType != ItemType.Sword && item.itemType != ItemType.Armor && item.itemType != ItemType.Bow)
            { aATK = aDEF = 0; aC = 0f; }

            string PlusInt(int v) => v > 0 ? $" <color=#{hex}>(+{v})</color>" : "";
            string PlusPct(float v) => v > 0.0001f ? $" <color=#{hex}>(+{v:0.#}%)</color>" : "";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"ATK: {fATK}{PlusInt(aATK)}");
            sb.AppendLine($"DEF: {fDEF}{PlusInt(aDEF)}");
            sb.AppendLine($"CRIT: {fCRIT:0.#}%{PlusPct(aC)}");

            if (item.useDurability && item.itemType != ItemType.Arrow)
            {
                durabilityPercent = Mathf.Clamp(durabilityPercent, 0, 100);
                string broken = durabilityPercent <= 0 ? "<color=#FF5555>(Hỏng)</color>" : "";
                sb.AppendLine($"Độ bền: {durabilityPercent}/{item.maxDurability}{broken}");
                if (showRepairPrice) sb.AppendLine($"Sửa: {item.GetRepairCost(durabilityPercent)}G");
            }

            if (statText)
            {
                statText.textWrappingMode = TextWrappingModes.Normal;
                statText.richText = true;
                statText.text = sb.ToString();
            }
        }
        else
        {
            if (statText)
            {
                statText.textWrappingMode = TextWrappingModes.Normal;
                statText.richText = true;
                statText.text = $"Đá cường hoá Lv{item.stoneLevel}";
            }
        }

        if (descText)
        {
            descText.textWrappingMode = TextWrappingModes.Normal;
            descText.richText = true;
            descText.text = item.description;
        }

        transform.SetAsLastSibling();
        root.gameObject.SetActive(true);
        visible = true;

        if (followMode == FollowMode.Anchor) FollowAnchor();
        else FollowCursor();
    }

    public void Hide()
    {
        visible = false;
        anchorTarget = null;
        if (root) root.gameObject.SetActive(false);
    }

    // ===== positioning =====
    Camera EffectiveCamera()
    {
        if (!parentCanvas || parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        if (uiCamera) return uiCamera;
        return parentCanvas.worldCamera;
    }

    void FollowAnchor()
    {
        if (!anchorTarget || !parentCanvas) return;
        RectTransform canvasRT = (RectTransform)parentCanvas.transform;
        Camera cam = EffectiveCamera();

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, anchorTarget.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screen + offset, cam, out Vector2 local);
        root.anchoredPosition = ClampToCanvas(local, canvasRT, root.sizeDelta);
    }

    void FollowCursor()
    {
        if (!parentCanvas) return;
        RectTransform canvasRT = (RectTransform)parentCanvas.transform;
        Camera cam = EffectiveCamera();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, Input.mousePosition + (Vector3)offset, cam, out Vector2 local);
        root.anchoredPosition = ClampToCanvas(local, canvasRT, root.sizeDelta);
    }

    Vector2 ClampToCanvas(Vector2 pos, RectTransform canvasRT, Vector2 size)
    {
        Vector2 min = canvasRT.rect.min + new Vector2(8, 8);
        Vector2 max = canvasRT.rect.max - new Vector2(size.x + 8, size.y + 8);
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y + size.y, max.y + size.y);
        return pos;
    }
}