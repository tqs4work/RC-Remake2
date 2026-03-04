using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.VFX;

public class CitySoundManager : MonoBehaviour
{
    public static CitySoundManager Instance { get; set; }

    [Header("City Sounds")]
    public AudioSource CityMusic;

    [Header("Door Sounds")]
    public AudioSource DoorOpen;
    public AudioSource DoorClose;

    [Header("Minigame Sounds")]
    public AudioSource MinigameSoundtrack;
    public AudioSource MinigameSoundtrackDoorOpen;
    public AudioSource MinigameSoundtrackDoorClose;

    [Header("Pharmacy Sounds")]
    public AudioSource PharmacySoundtrack;

    [Header("Random Machine Sounds")]
    public AudioSource RandomMachineSoundtrack;

    public void Awake()
    {
        Instance = this;
    }

    public void PlayCityMusic()
    {
        CityMusic.Play();
    }

    public void PlayDoorOpen()
    {
        DoorOpen.Play();
    }

    public void PlayDoorClose()
    {
        DoorClose.Play();
    }

    public void PlayMinigameMusic()
    {
        MinigameSoundtrack.Play();
    }

    public void PlayMinigameDoorOpen()
    {
        MinigameSoundtrackDoorOpen.Play();
    }

    public void PlayMinigameDoorClose()
    {
        MinigameDoorClose.Play();
    }

    public void PlayPharmacyMusic()
    {
        PharmacySoundtrack.Play();
    }

    public void PlayRandomMachineMusic()
    {
        RandomMachineSoundtrack.Play();
    }


}
