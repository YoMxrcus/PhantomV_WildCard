using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Called automatically by PlayerInput
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Ground check
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to stay grounded
        }

        // Convert input to world direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (inputDirection.magnitude >= 0.1f)
        {
            // Camera-relative movement
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

            // Rotate toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
