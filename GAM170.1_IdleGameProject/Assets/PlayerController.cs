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

    private void Awake()
    {
        TryGetComponent(out controller);
    }

    // May run more than once per frame
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

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (controller.isGrounded == true)
        {
            if(Input.GetButtonDown("Jump") == true)
            {
                velocity = jumpForce;
            }
        }

        Vector3 motionStep = Vector3.zero;
        motionStep += transform.right * inputX * moveSpeed;
        motionStep.y += velocity;
        motionStep.z = 0;
        controller.Move(motionStep * Time.deltaTime);
    }

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
