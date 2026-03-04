using UnityEngine;

public class DiceSound : MonoBehaviour
{
    public static DiceSound Instance { get; private set;}
    public AudioSource MainMusic;
    public AudioSource Dice;
    private AudioSource currentMusic;
    private void Awake()
    {
        // Singleton protection
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMainMusic();
    }

    private void Update()
    {
        // Nếu đang phát nhạc khác và nó kết thúc → quay lại CityMusic
        if (currentMusic != null && currentMusic != MainMusic)
        {
            if (!currentMusic.isPlaying)
            {
                PlayMainMusic();
            }
        }
    }

    // ======================
    // CORE MUSIC CONTROLLER
    // ======================

    private void PlayMusic(AudioSource newMusic)
    {
        if (newMusic == null) return;

        if (currentMusic != null && currentMusic.isPlaying)
        {
            currentMusic.Stop();
        }

        currentMusic = newMusic;
        currentMusic.Play();
    }

    public void PlayMainMusic()
    {
        PlayMusic(MainMusic);
    }

    public void StopMainMusic()
    {
        if (MainMusic != null)
            MainMusic.Stop();
    }

    public void ResumeMainMusic()
    {
        PlayMusic(MainMusic);
    }

    public void PlayDice()
    {
        if (Dice != null)
            Dice.Play();
    }
    public void StopDice()
    {
        if (Dice != null) Dice.Stop();
    }
}
