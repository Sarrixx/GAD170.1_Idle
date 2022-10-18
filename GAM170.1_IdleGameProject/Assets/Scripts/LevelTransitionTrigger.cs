using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionTrigger : MonoBehaviour
{
    [SerializeField] private Animator canvasAnim;

    private bool loading = false;
    private PlayerController player;

    private void Update()
    {
        if (player != null)
        {
            if (player.CanMove == true)
            {
                if (Input.GetKeyDown(KeyCode.F) == true && loading == false)
                {
                    loading = true;
                    StartCoroutine(LoadIdle());
                }
            }
        }
    }
    private IEnumerator LoadIdle()
    {
        canvasAnim.SetTrigger("fade");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player == null)
        {
            if (other.TryGetComponent(out player) == true)
            {
                player.UpdateInteractionText("Press F to lick the window some more.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            player.UpdateInteractionText(null);
            player = null;
        }
    }
}
