using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoDialog_TU : MonoBehaviour
{
    public TMP_Text messageText;
    public Button okButton;

    private Action _onOk;

    void Awake()
    {
        gameObject.SetActive(false);
        if (okButton) okButton.onClick.AddListener(() => { Hide(); _onOk?.Invoke(); });
    }

    public void Show(string message, Action onOk = null)
    {
        _onOk = onOk;
        if (messageText) messageText.text = message;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        ItemTooltipUI_TU.Ensure()?.Hide();
    }

    public void Hide() => gameObject.SetActive(false);
}
