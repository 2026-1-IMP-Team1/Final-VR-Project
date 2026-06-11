using System.Collections;
using UnityEngine;

public class CustomFallRespawn : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravity = -9.81f;

    [Header("Respawn Settings")]
    public float fallThreshold = -10f;

    private Vector3 startPosition;
    private Vector3 velocity;
    
    private CharacterController characterController;

    void Start()
    {
        startPosition = transform.position;
        
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);

        if (transform.position.y < fallThreshold)
        {
            characterController.enabled = false;
            transform.rotation = Quaternion.identity;
            transform.position = startPosition;
            velocity = Vector3.zero;
            characterController.enabled = true;
            Physics.SyncTransforms(); 
        }
    }
}
