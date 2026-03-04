using UnityEngine;

public class CitySoundManager : MonoBehaviour
{
    public static CitySoundManager Instance { get; private set; }

    [Header("City Sounds")]
    [SerializeField] private AudioSource cityMusic;

    [Header("Door Sounds")]
    [SerializeField] private AudioSource doorOpen;
    [SerializeField] private AudioSource doorClose;

    [Header("Minigame Sounds")]
    [SerializeField] private AudioSource minigameMusic;
    [SerializeField] private AudioSource minigameDoorOpen;
    [SerializeField] private AudioSource minigameDoorClose;

    [Header("Pharmacy Sounds")]
    [SerializeField] private AudioSource pharmacyMusic;

    [Header("Random Machine Sounds")]
    [SerializeField] private AudioSource randomMachineMusic;
    [SerializeField] private AudioSource prizeMachineMusic;

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
        PlayCityMusic();
    }

    private void Update()
    {
        // Nếu đang phát nhạc khác và nó kết thúc → quay lại CityMusic
        if (currentMusic != null && currentMusic != cityMusic)
        {
            if (!currentMusic.isPlaying)
            {
                PlayCityMusic();
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

    public void PlayCityMusic()
    {
        PlayMusic(cityMusic);
    }

    public void StopCityMusic()
    {
        if (cityMusic != null)
            cityMusic.Stop();
    }

    public void ResumeCityMusic()
    {
        PlayMusic(cityMusic);
    }

    // ======================
    // OTHER MUSIC
    // ======================

    public void PlayMinigameMusic()
    {
        PlayMusic(minigameMusic);
    }

    public void PlayPharmacyMusic()
    {
        PlayMusic(pharmacyMusic);
    }

    public void PlayRandomMachineMusic()
    {
        PlayMusic(randomMachineMusic);
    }

    public void PlayPrizeMachineMusic()
    {
        PlayMusic(prizeMachineMusic);
    }

    // ======================
    // SOUND EFFECTS (không ảnh hưởng cityMusic)
    // ======================

    public void PlayDoorOpen()
    {
        if (doorOpen != null)
            doorOpen.Play();
    }

    public void PlayDoorClose()
    {
        if (doorClose != null)
            doorClose.Play();
    }

    public void PlayMinigameDoorOpen()
    {
        if (minigameDoorOpen != null)
            minigameDoorOpen.Play();
    }

    public void PlayMinigameDoorClose()
    {
        if (minigameDoorClose != null)
            minigameDoorClose.Play();
    }
}