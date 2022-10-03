using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerB : MonoBehaviour
{
    public TriggerA trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            if(trigger.Active == true)
            {
                Debug.Log("Game victory!");
            }
            else
            {
                Debug.Log("Game lost!");
            }

            SceneManager.LoadScene(0);
        }
    }
}
