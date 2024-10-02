using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float playerSpeed = 5.0f, playerRunSpeed = 8;
    [SerializeField]
    private float jumpHeight = 1.25f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float flySpeed = 2;

    private Vector3 playerVelocity;

    [Header("Grounded check parameters:")]
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float rayDistance = 0.1f;
    [field: SerializeField]
    public bool IsGrounded { get; private set; }
    [SerializeField]
    private float rayThickness = 1f;
    public RaycastHit m_hit;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private Vector3 GetMovementDirection(Vector3 movementInput)
    {
        return transform.right * movementInput.x + transform.forward * movementInput.z;
    }

    public void Fly(Vector3 movementInput, bool ascendInput, bool descendInput)
    {
        Vector3 movementDirection = GetMovementDirection(movementInput);

        if (ascendInput)
        {
            movementDirection += Vector3.up * flySpeed;
        }
        else if (descendInput)
        {
            movementDirection -= Vector3.up * flySpeed;
        }
        controller.Move(movementDirection * playerSpeed * Time.deltaTime);
    }

    public void Walk(Vector3 movementInput, bool runningInput)
    {
        Vector3 movementDirection = GetMovementDirection(movementInput);
        float speed = runningInput ? playerRunSpeed : playerSpeed;
        controller.Move(movementDirection * Time.deltaTime * speed);
    }

    public void HandleGravity(bool isJumping)
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        if (isJumping && IsGrounded)
            AddJumpForce();
        ApplyGravityForce();
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void AddJumpForce()
    {
        //playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        playerVelocity.y = jumpHeight;
    }

    private void ApplyGravityForce()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        playerVelocity.y = Mathf.Clamp(playerVelocity.y, gravityValue, 10);
    }

    private void Update()
    {
        // Slightly below the player’s center
        Vector3 boxCenter = transform.position; // + Vector3.down * 1.69f;
        boxCenter.y += 0.1f;
        // Half-extent of the box (x, y, z) - y should be small as it's the thickness of the box.
        Vector3 boxHalfExtents = new Vector3(rayThickness, 0.1f, rayThickness);

        // Check for collision below
        IsGrounded = Physics.BoxCast(
            boxCenter,                      // Center of the box
            boxHalfExtents,                 // Half-size of the box
            Vector3.down,                   // Direction to cast (downwards)
            out m_hit,                      // Store the hit information
            Quaternion.identity,            // No rotation to ensure the box is aligned with world axes
            rayDistance,                    // Distance to cast
            groundMask                      // Layer mask to define what is ground
        );
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Slightly below the player’s center, matching the box center in Update
        Vector3 boxCenter = transform.position; // + Vector3.down * 1.69f;
        boxCenter.y += 0.1f;
        Vector3 boxHalfExtents = new Vector3(rayThickness, 0.1f, rayThickness);

        // Draw the box cast at its starting position
        //Gizmos.DrawWireCube(boxCenter, boxHalfExtents * 2); // Multiply by 2 because extents are half-size

        // If grounded, visualize the hit point
        if (IsGrounded)
        {
            // Draw a box at the point of collision
            Gizmos.DrawWireCube(m_hit.point, boxHalfExtents * 2);
        }
        else
        {
            // Draw a box at the maximum cast distance
            Gizmos.DrawWireCube(boxCenter + Vector3.down * rayDistance, boxHalfExtents * 2);
        }
    }

}
