using UnityEngine;
using TMPro;
using System.Collections;
public class Dialog : MonoBehaviour
{
    [SerializeField] GameObject nextDialogBox;
    [SerializeField] TextMeshProUGUI nameTag; 
    public TextMeshProUGUI textComponent;
    public string tagName;
    public string characterName;
    public string[] lines;
    public float textSpeed;    
    public int index;

    [Header("Audio")]
    [SerializeField] AudioSource TypingSound;
    void Start()
    {
        if(nameTag != null) nameTag.text = tagName.ToString();
        textComponent.text = string.Empty;
        StartDialog();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == GetProcessedLine(lines[index]))
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = GetProcessedLine(lines[index]);
                TypingSound.Stop();
            }
        }        
    }

    void StartDialog()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        TypingSound.Play();
        foreach (char c in lines[index].ToCharArray())
        {
            if(c != '$') textComponent.text += c;
            else textComponent.text += characterName.ToString();
            yield return new WaitForSeconds(textSpeed);
        }
        TypingSound.Stop();
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            index++;
            gameObject.SetActive(false);
            if (nextDialogBox != null)
            {
                nextDialogBox.SetActive(true);
            }
        }
    }
    // Hàm xử lý ký tự $
    private string GetProcessedLine(string line)
    {
        return line.Replace("$", characterName);
    }
}
