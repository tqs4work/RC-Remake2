using UnityEngine;

public class DungeonAudio : MonoBehaviour
{
    public GameObject cam2;

    public AudioSource bg1;
    public AudioSource bg2;


    //Enemy audio sources
    public AudioSource frog;
    public AudioSource slime;
    public AudioSource fire;
    public AudioSource tengu1;
    public AudioSource tengu2;

    void Start()
    {
        bg1.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            cam2.SetActive(!cam2.activeSelf);
        }

    }

    public void SwitchAudio()
    {
        if (bg1.isPlaying)
        {
            bg1.Stop();
            bg2.Play();
        }
        else
        {
            bg2.Stop();
            bg1.Play();
        }
    }
}
