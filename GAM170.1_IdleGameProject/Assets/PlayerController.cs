using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private Text interactionText;

    private CharacterController controller;
    private float velocity = 0;

    public bool CanMove { get; set; } = true;

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    private void Awake()
    {
        if(TryGetComponent(out controller) == true)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// May run more than once per frame
    /// </summary>
    private void FixedUpdate()
    {
        if(controller.isGrounded == true)
        {
            velocity = -gravity * Time.deltaTime;
        }
        else
        {
            velocity -= gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        Vector3 motionStep = Vector3.zero;
        float inputX = Input.GetAxisRaw("Horizontal");

        if (CanMove == true)
        {
            if (controller.isGrounded == true)
            {
                if (Input.GetButtonDown("Jump") == true)
                {
                    velocity = jumpForce;
                }
            }

            motionStep += transform.right * inputX * moveSpeed;
        }
        motionStep.y += velocity;
        motionStep.z = 0;
        controller.Move(motionStep * Time.deltaTime);
    }

    /// <summary>
    /// Toggles the interaction text and updates it to displayed the passed message.
    /// </summary>
    /// <param name="message">The new message the interaction text will display. If null, turns text off.</param>
    public void UpdateInteractionText(string message)
    {
        if (message == null)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = message;
        }
    }
}
