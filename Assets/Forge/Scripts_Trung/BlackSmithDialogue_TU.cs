using UnityEngine;
/*
* ScriptableObjct: dùng để lưu trữ dữ liệu hội` thoại của thợ rèn
* -> Không gắn lên GameObject
* -> Tạo asset trong Project (Right Click -> Create -> Scriptable Objects -> BlackSmithDialogue_TU)
*/

// tên file mặc định khi tạo
// menu Create trong Project
[CreateAssetMenu(fileName = "BlackSmithDialogue_TU", menuName = "Scriptable Objects/BlackSmithDialogue_TU")]
public class BlackSmithDialogue_TU : ScriptableObject
{
    // Thông tin mặc định của thợ rèn
    [Header("Default Speker")]
    public string npcName = "BlackSmith"; // Tên NPc hiển thị trên UI
    public Sprite npcPortrait; // Ảnh đại diện NPC hiển thị trên UI

    [Header("Defaults")] // Hội thoại mặc định
    public float defautTypeSpeed = 0.05f; // Tốc độ gõ chữ mặc định
    public AudioClip defaultVoiceSound; // Âm thanh giọng nói mặc định
    public float defaultVoicePitch = 1f; // Cao độ giọng nói mặc định
    public float defaultAutodelay = 1.5f; // Delay mặc định khi tự động chuyển câu(nếu bật auto)
    // Danh sách các hội thoại
    [Header("Lines")]
    public DialogueLine[] lines;
}
/*Dữ liệu của 1 câu hội thoại
*[System.Serializable]  // Hiển thị trong Inspector 
*/

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string lineText; // Nội dung câu hội thoại
    public Sprite linePortrait; // Ảnh đại diện cho câu hội thoại này (nếu khác với mặc định)
    public AudioClip voiceSound; // Âm thanh giọng nói cho câu hội thoại này
    [Range(0.2f, 2f)]
    public float voicePitch = 1f; // Cao độ giọng nói cho câu hội thoại này
    [Range(0.05f, 0.2f)]
    public float typeSpeed = 0.05f; // Tốc độ gõ chữ cho câu hội thoại này
    public bool autoProgress; // Có tự động chuyển câu sau delay không?
    // -1 = dùng giá trị mặc định defaultAutoDelay
    public float autoDelay = -1f; // Delay trước khi tự động chuyển câu (nếu autoProgress = true)
    // Mở rộng sau này (tùy): lựa chọn, trigger, tag, v.v. 
    // public DialogueChoice[] choices;
}