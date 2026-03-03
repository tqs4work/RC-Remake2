using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject quitButton;
    public GameObject Pharmacy;

    public void Start()
    {
        Pharmacy.SetActive(false);
    }

    public void closePharmacy()
    {
        Pharmacy.SetActive(false);
    }
}
