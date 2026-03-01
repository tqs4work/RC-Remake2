using UnityEngine;

public class PlayerFarm : MonoBehaviour
{
    public FarmController farmController;
    bool isFarm;
    private void Start()
    {
        
    }
    void Update()
    {
        GameObject obj = GameObject.Find("FarmController");
        if (obj == null)
        {
            return;
        }
        if (farmController == null)
        {
            farmController = GameObject.Find("FarmController").GetComponent<FarmController>();            
        }
        farmController.UpdateSelectorTile(isFarm,transform);
        farmController.CheckHole();
        farmController.Shovel();
        farmController.Water();
        if(Input.GetKeyDown(KeyCode.K))
        {
            farmController.SaveGameData();
            Debug.Log("Save");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Farm"))
        {
            isFarm = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Farm"))
        {
            isFarm = false;
        }
    }
}
