using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        if (PlayerCharacter.Instance != null)
        {
            PlayerCharacter.Instance.transform.position = spawnPoint.position;
        }
    }
}