using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebtCollectorTrigger : MonoBehaviour
{
    public delegate void GameEventDelegate();

    [SerializeField] private GameObject innerColliders;
    [SerializeField] private Animator npcAnim;

    private PlayerController player;

    private static int chances = 3;
    private static bool paid = false;

    public event GameEventDelegate GameEndEvent;

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    private void Awake()
    {
        //Subscribe anonymous method to event
        GameEndEvent = delegate 
        { 
            chances = 3; //reset chances to 3 for next game
            paid = false;
            SceneManager.LoadScene(0); //reload the scene
        };
        GameEndEvent += GameManager.ResetStaticValues; //subscribe GameManager method to event
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (player != null) //if script is tracking player
        {
            if (player.CanMove == true) //if player can move
            {
                if (Input.GetKeyDown(KeyCode.F) == true) //if input pressed
                {
                    player.CanMove = false; //turn player movement off
                    if (paid == false) //if first quest incomplete
                    {
                        if (GameManager.ActualCurrencyValue >= 100000) //first objective complete
                        {
                            paid = true;
                            innerColliders.SetActive(false); //deactivate inner bounds
                            StartCoroutine(DialogueResponse_ObjectiveComplete(3f)); //start positive dialogue coroutine
                        }
                        else //was unable to complete first objective
                        {
                            chances--;
                            if (chances < 0) //out of chances
                            {
                                GameEndEvent.Invoke(); //end the game
                            }
                            else
                            {
                                StartCoroutine(DialogueResponse_ObjectiveIncomplete(3f)); //start negative dialogue coroutine
                            }
                        }
                    }
                    else
                    {
                        //if target has been retrieved, game has been won
                    }
                }
            }
        }
    }

    /// <summary>
    /// Executes on the frame that a collider enters this object's trigger collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (player == null)
        {
            if (other.TryGetComponent(out player) == true)
            {
                player.UpdateInteractionText("Press F to talk to the debt collector.");
            }
        }
    }

    /// <summary>
    /// Executes on the frame that a collider leaves this object's trigger collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            player.UpdateInteractionText(null);
            player = null;
        }
    }

    /// <summary>
    /// Coroutine used to change dialogue text.
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="nextMessage"></param>
    /// <returns></returns>
    private IEnumerator DialogueResponse_ObjectiveIncomplete(float delay)
    {
        npcAnim.SetBool("talking", true);
        player.Anim.SetBool("talking", true);
        player.UpdateInteractionText("Licks remaining: " + string.Format("{0:n0}", 100000 - GameManager.ActualCurrencyValue));
        yield return new WaitForSeconds(delay);
        player.UpdateInteractionText("Chances remaining: " + chances);
        yield return new WaitForSeconds(delay);
        npcAnim.SetBool("talking", false);
        player.Anim.SetBool("talking", false);
        player.CanMove = true;
        player.UpdateInteractionText("Press F to talk to the debt collector.");
    }

    /// <summary>
    /// Coroutine used to change dialogue text.
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="nextMessage"></param>
    /// <returns></returns>
    private IEnumerator DialogueResponse_ObjectiveComplete(float delay)
    {
        npcAnim.SetBool("talking", true);
        player.Anim.SetBool("talking", true);
        player.UpdateInteractionText("Your debt has been settled.");
        yield return new WaitForSeconds(delay);
        player.UpdateInteractionText(""); //quest text
        yield return new WaitForSeconds(delay);
        npcAnim.SetBool("talking", false);
        player.Anim.SetBool("talking", false);
        player.CanMove = true;
        //start quest timer
        player.UpdateInteractionText("Press F to talk to the debt collector.");
    }
}
