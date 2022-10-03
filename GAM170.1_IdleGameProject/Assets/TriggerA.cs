using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerA : MonoBehaviour
{
    private bool active = false;

    public bool Active 
    { 
        get 
        { 
            return active;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true && active == false)
        {
            active = true;
            Debug.Log("Trigger A has been activated!");
        }
    }
}
