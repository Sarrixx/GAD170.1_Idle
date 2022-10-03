using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebtCollectorTrigger : MonoBehaviour
{
    public delegate void GameEventDelegate();

    private bool activePlayer = false;
    private bool transitioning = false;

    private static int chances = 0;
    private static bool paid = false;

    public event GameEventDelegate GameEndEvent;

    private void Awake()
    {
        GameEndEvent = delegate 
        { 
            chances = 0;
            SceneManager.LoadScene(0);
        };
        GameEndEvent += GameManager.ResetStaticValues;
    }

    private void Update()
    {
        if(activePlayer == true)
        {
            if (transitioning == false)
            {
                if (Input.GetKeyDown(KeyCode.F) == true)
                {
                    if (paid == false)
                    {
                        if (GameManager.ActualCurrencyValue >= 100000)
                        {
                            paid = true;
                            //activate timer
                            //deactivate inner bounds
                        }
                        else
                        {
                            chances--;
                            if (chances == 0)
                            {
                                GameEndEvent.Invoke();
                            }
                            else
                            {
                                //tell player how many licks until the window is clean (100000 - GameManager.ActualCurrencyValue) 
                                //tell player how many chances they have
                                transitioning = true;
                            }
                        }
                    }
                    else
                    {
                        //if target has been retrieved, game has been won
                    }
                }
            }
            else
            {
                if(Input.anyKeyDown == true)
                {
                    //send player back to idle level
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController controller) == true)
        {
            activePlayer = true;
            controller.UpdateInteractionText("Press F to talk to the debt collector.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController controller) == true)
        {
            activePlayer = false;
            controller.UpdateInteractionText(null);
        }
    }
}
