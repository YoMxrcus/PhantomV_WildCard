using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float stamina = 100f;
    public float sprintDepletionRate = 20f;
    public float staminaRegenRate = 5f;
    [SerializeField] bool allowSprint = true;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;


    public float dashSpeed = 20f;
    public float dashDuration = .25f;
    public float dashCooldown = 3f;
    private bool isDashing = false;
    private bool canDash = true;


    public bool aiming = false;

    public GameObject PuzzleManager;
    public GameObject PasswordManager;
    public GameObject HeatMinigameManager;
    public GameObject OxygenMinigameManager;
    public bool InPuzzle = false;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Called automatically by PlayerInput
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        if (Mouse.current.rightButton.isPressed)
        {
            aiming = true;
            RotateTowardMouse();

        }
        else { aiming = false; }
    }


    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && canDash && stamina >= 30)
        {
            StartCoroutine(Dash());
        }
    }


    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        stamina -= 30;
        
        Vector3 dashDirection = transform.forward;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
           
             controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            yield return null; // Wait for the next frame
        }

        // Dash ends
        isDashing = false;

        // Start cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }



    private void HandleMovement()
    {
        if (stamina <= 5)
        {
            allowSprint = false;
            Invoke("ResetSprint", 3f);
        }

        if (Keyboard.current.shiftKey.isPressed && stamina > 0 && allowSprint)
        {
            moveSpeed = 9;
            stamina -= sprintDepletionRate * Time.deltaTime;
        }

        else if (stamina < 100f)
        { 
            moveSpeed = 6;
            stamina += staminaRegenRate * Time.deltaTime;

            // Clamp the stamina to make sure it doesn't go over 100
            stamina = Mathf.Min(stamina, 100f);
        }

        // Ground check
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to stay grounded
        }

        // Convert input to world direction
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (inputDirection.magnitude >= 0.1f && !isDashing)
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
            if (!aiming)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else { }

                controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);


       
    }

    void RotateTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;

            Vector3 direction = (lookPoint - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)

        {
            case "TestTrigger":

                int randomNum = Random.Range(1, 5);

                switch (randomNum)
                {
                    case 1:
                        if (InPuzzle == false)
                        {
                        PuzzleManager.GetComponent<Puzzles>().BeginMinigame();
                            InPuzzle = true;
                        }
                        return;

                    case 2:
                        if (InPuzzle == false)
                        {
                        PasswordManager.GetComponent<PasswordMinigameManager>().BeginMinigame();
                            InPuzzle = true;
                        }
                        return;

                    case 3:
                        if (InPuzzle == false)
                        {
                        HeatMinigameManager.GetComponent<HeatMeter>().BeginMinigame();
                            InPuzzle = true;
                        }
                        return;

                    case 4:
                        if (InPuzzle == false)
                        {
                            OxygenMinigameManager.GetComponent<OxygenLevelsPuzzle>().BeginMinigame();
                            InPuzzle = true;
                        }
                        return;
                }

               
                return;
        }
    }

    private void ResetSprint()
    {
        allowSprint = true;
    }

}
