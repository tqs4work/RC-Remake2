using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dice2Scripts : MonoBehaviour
{
    Rigidbody body; // Reference to the Rigidbody component
    [SerializeField] private float maxRandomForceValue, startRollingForce; // Max random force value for torque and initial upward force
    private float forceX, forceY, forceZ; // Random torque values
    public int diceFaceNum;

    private void Awake()
    {
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        if (body != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RollDice();
                
            }
        }
    }

    private void RollDice()
    {
        body.isKinematic = false;

        // Generate random torque values
        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);

        // Apply upward force and torque to the dice
        body.AddForce(Vector3.up * startRollingForce);
        body.AddTorque(forceX, forceY, forceZ);
    }

    private void Initialized()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0); // Random initial rotation
    }
}

