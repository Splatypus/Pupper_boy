using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallObjective : MonoBehaviour
{
    public FallBubbleGameManager manager;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            manager.CompleteObjective(this);
        }
    }
}
