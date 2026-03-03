using UnityEngine;
using System.Collections;
using Mono.Cecil.Cil;

public class FaceDetector : MonoBehaviour
{
    Dice1Scripts dice;
    Dice2Scripts dice2;
    Dice3Scripts dice3;
    private bool hasDetectedDice1 = false;
    private bool hasDetectedDice2 = false;
    private bool hasDetectedDice3 = false;

    private void Awake()
    {
        dice = FindAnyObjectByType<Dice1Scripts>(); 
        dice2 = FindAnyObjectByType<Dice2Scripts>();
        dice3 = FindAnyObjectByType<Dice3Scripts>();
    }

    private void OnTriggerStay(Collider other)
    {
        // Kiểm tra Dice 1
        var dice1 = other.GetComponentInParent<Dice1Scripts>();
        if (dice1 != null)
        {
            Rigidbody rb1 = dice1.GetComponent<Rigidbody>();
            if (rb1.linearVelocity.magnitude < 0.1f && rb1.angularVelocity.magnitude < 0.1f)
            {
                if (int.TryParse(other.gameObject.name.Replace("Face", ""), out int faceNumber))
                {
                    if (!hasDetectedDice1)
                    {
                        dice1.diceFaceNum = faceNumber;
                        hasDetectedDice1 = true;
                    }
                }
            }
        }

        // Kiểm tra Dice 2
        var dice2 = other.GetComponentInParent<Dice2Scripts>();
        if (dice2 != null)
        {
            Rigidbody rb2 = dice2.GetComponent<Rigidbody>();
            if (rb2.linearVelocity.magnitude < 0.1f && rb2.angularVelocity.magnitude < 0.1f)
            {
                if (int.TryParse(other.gameObject.name.Replace("Face", ""), out int faceNumber))
                {
                    if (!hasDetectedDice2)
                    {
                        dice2.diceFaceNum = faceNumber;
                        hasDetectedDice2 = true;
                    }
                }
            }
        }

        // Kiểm tra Dice 3
        var dice3 = other.GetComponentInParent<Dice3Scripts>();
        if (dice3 != null)
        {
            Rigidbody rb3 = dice3.GetComponent<Rigidbody>();
            if (rb3.linearVelocity.magnitude < 0.1f && rb3.angularVelocity.magnitude < 0.1f)
            {
                if (int.TryParse(other.gameObject.name.Replace("Face", ""), out int faceNumber))
                {
                    if (!hasDetectedDice3)
                    {
                        dice3.diceFaceNum = faceNumber;
                        hasDetectedDice3 = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        hasDetectedDice1 = false;
        hasDetectedDice2 = false;
        hasDetectedDice3 = false;
    }
}
