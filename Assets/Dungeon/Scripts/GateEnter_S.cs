using UnityEngine;
using UnityEngine.SceneManagement;

public class GateEnter_S : MonoBehaviour
{
    public GateSO gate;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(gate.gateName == "farmGate")
            {
                SceneManager.LoadScene("Farm");
            }
            else if(gate.gateName == "cityGate")
            {
                SceneManager.LoadScene("City");
            }
            else if(gate.gateName == "forgeGate")
            {
                SceneManager.LoadScene("Forge");
            }
            else if (gate.gateName == "dungeonGate")
            {
                SceneManager.LoadScene("Dungeon");
            }
        }
    }
}
