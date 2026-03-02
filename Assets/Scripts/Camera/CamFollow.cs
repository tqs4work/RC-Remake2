using Unity.Cinemachine;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    GameObject player;
    public CinemachineCamera cam;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        cam.Follow = player.transform;
    }
}
