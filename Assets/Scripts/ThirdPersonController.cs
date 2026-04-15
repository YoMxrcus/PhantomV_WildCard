using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.UI;

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
    public bool canMove = true;
    [SerializeField] bool allowSprint = true;
    public Vector3 moveDirection;
    public int heatMeter = 0;
    public Slider staminaBar;
    public GameObject projectile;
    public GameObject enemyDamage;

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

    public int HeaterUp = 0; //heat
    public bool HeatDecrement;
    public int health = 100;
    public Slider healthSlider;

    public bool aiming = false;

    public GameObject PuzzleManager;
    public GameObject PasswordManager;
    public GameObject HeatMinigameManager;
    public GameObject OxygenMinigameManager;
    public bool InPuzzle = false;
    public bool InLoot = false;
    public bool InLootTrigger = false;
    public bool condensorActive = false;
    public bool stimActive = false;
    public bool isSprinting = false;
    public bool CondensorAnimation = false;
    public bool Grenading = false;
    public bool isDead = false;
    
    

    public GameObject CurrentGenerator;
    public GameObject InteractText;
    public int interactTextNum;
    public bool DashOut = false;

    public LayerMask layerMask;

    public GameObject currentLootBox;
    public GameObject lootPanel;
    public GameObject arrow, dial;

    public int condensors = 0;
    public int stims = 0;
    public int grenades = 0;
    public int bandages = 0;
    public int pistolAmmo = 50;
    public int assaultAmmo = 75;
    public int shotgunAmmo = 30;
    public int activeGun = 0;
    public int pistolMag = 15;
    public int assaultMag = 25;
    public int shotgunMag = 10;
    public TMP_Text MagNum, MaxAmmo;
    public int currentDamage = 20;

    private bool isCocked = true;
    // Pattern for shotgun spread
    private float[] shotgunSpreadPattern = { -0.4f, -0.2f, 0f, 0.2f, 0.4f };

    private float lastFireTime;
    public float fireRate = 0.12f; //Time between shots
    public float bloom = 0.20f; //recoil offset

    public int condensorHealth = 0;
    public Sprite StimIMAGE, GrenadeIMAGE, EmptyIMAGE,CondensorImage, PistolImage, AssaultRifleIMAGE, ShotgunIMAGE, BandageIMAGE;
    public GameObject gunSlot, invSlotTwo, invSlotThree, invSlotFour, invSlotFive;
    public GameObject[] InventorySelects;
    public GameObject PistolObject, RifleObject, ShotgunObject;
    public TMP_Text CondensorNum, StimNum, GrenadeNum, BandageNum;

    [SerializeField] Coroutine heatCoroutine, condensorHealthCoroutine, condensorActiveCoroutine;

    public int CurrentInvSelect = 0;

    public GameObject GrenadePrefab;
    [SerializeField] Transform throwPoint;
    //Vector3 targetPosition ;

    public AudioSource playerSounds;
    public AudioClip walking, running, shotgunReload, shotgunRecock, shotgunShot, pistolReload, pistolShot, rifleReload, rifleShot, dash, grunt1, grunt2, grunt3, grunt4, switchWeapons, syringe, bandage, useCondensor,throwGrenade, lootTrashcan, looting2, generatorStart, generatorStop, tickingSound;
    public Animator PlayerAnimator;

    //heartbeatsensor
    public Animator heartBeatAnimator;
    public Image targetImageLine;
    public TMP_Text heartBeatText;
    public GameObject DeathPanel;

    //canvas groups 
    public CanvasGroup UICanvasGroup;
    public CanvasGroup HeatWarningCanvasGroup;

    //
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerSounds.loop = false;
    }

    public IEnumerator HeatCoroutine()
    {
        HeatDecrement = true;
        while (HeaterUp < 120)
        {
            yield return new WaitForSeconds(1f);

            HeaterUp++;

            
        }
    }
    public IEnumerator CondensorCoroutine()
    {
        HeatDecrement = true;
        while (HeaterUp > 0)
        {
            yield return new WaitForSeconds(1f);

            HeaterUp--;
         
        }
    }

    public IEnumerator CondensorDecrementCoroutine()
    {
        HeatDecrement = true;
        while (condensorHealth > 0)
        {

            yield return new WaitForSeconds(2f);

            condensorHealth--;

        }
            
                condensorHealth = 0;
                condensorActive = false;
                StopCondensorCoroutine();
                StartHeatCoroutine();
           
        condensorHealthCoroutine = null;
    }

    private void Start()
    {
        StartHeatCoroutine(); 
    }

    // Called automatically by PlayerInput
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 0)
        {
            if (!playerSounds.isPlaying)
            {
                if (!Keyboard.current.shiftKey.isPressed)
                {
                    playerSounds.clip = walking;
                }
                playerSounds.loop = true;
                playerSounds.Play();
            }
        }
        else { playerSounds.clip = null; }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            HandleMovement();
        }
        if (Mouse.current.rightButton.isPressed)
        {
            aiming = true;
            RotateTowardMouse(); 

        }
        else { aiming = false; }

        UpdateUI();

        if (HeaterUp >=120)
        { health = 0; }
    }

    public void SetAllBoolsFalse()
    {
        if (PlayerAnimator == null) return;

        // Loop through all parameters in the Animator
        foreach (AnimatorControllerParameter param in PlayerAnimator.parameters)
        {
            // Only set bool parameters
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                PlayerAnimator.SetBool(param.name, false);
            }
        }

    }


    public void CheckIdleAnimations()
    {
        if (CurrentInvSelect != 0 && moveInput.sqrMagnitude == 0)
        {
            if (PlayerAnimator.GetBool("isIdle") != true && !InLoot && !InPuzzle && !CondensorAnimation && !Grenading)
            {
            SetAllBoolsFalse();
            PlayerAnimator.SetBool("isIdle", true && !InLoot && !InPuzzle);
            }
        }

        else if (CurrentInvSelect == 0 && moveInput.sqrMagnitude == 0)
        {
            switch (activeGun) 
            {
                case 0:
                    if (PlayerAnimator.GetBool("isPistolIdle") != true && !InLoot && !InPuzzle)
                    {
                        SetAllBoolsFalse();
                        PlayerAnimator.SetBool("isPistolIdle", true);
                    }
                    break;

                case 1:
                    if (!aiming)
                    {
                        if (PlayerAnimator.GetBool("IsRifleIdle") != true && !InLoot && !InPuzzle)
                        {
                        SetAllBoolsFalse();
                        PlayerAnimator.SetBool("IsRifleIdle", true);
                        }
                    }

                    else if (aiming)
                    {
                        if (PlayerAnimator.GetBool("RifleAimIdle") != true && !InLoot && !InPuzzle)
                        {
                            SetAllBoolsFalse();
                            PlayerAnimator.SetBool("RifleAimIdle", true);
                        }
                    }

                        break;
                case 2:
                    if (!aiming)
                    {
                        if (PlayerAnimator.GetBool("IsRifleIdle") != true && !InLoot && !InPuzzle)
                        {
                            SetAllBoolsFalse();
                            PlayerAnimator.SetBool("IsRifleIdle", true);
                        }
                    }

                    else if (aiming)
                    {
                        if (PlayerAnimator.GetBool("RifleAimIdle") != true && !InLoot && !InPuzzle)
                        {
                            SetAllBoolsFalse();
                            PlayerAnimator.SetBool("RifleAimIdle", true);
                        }
                    }
                    break;
            }

        }
    }


    public void CheckWalkAnimations()
    {
        if (CurrentInvSelect != 0 && moveInput.sqrMagnitude > 0)
        {
            if (!isSprinting)
            {
                if (PlayerAnimator.GetBool("isWalking") != true)
                {
                    SetAllBoolsFalse();
                    PlayerAnimator.SetBool("isWalking", true);
                }
            }
          
        }

        else if (CurrentInvSelect == 0 && moveInput.sqrMagnitude > 0)
        {
            if (!isSprinting)
            {
                switch (activeGun)
                {
                    case 0:
                        if (PlayerAnimator.GetBool("isPistolWalk") != true)
                        {
                            SetAllBoolsFalse();
                            PlayerAnimator.SetBool("isPistolWalk", true);
                        }
                        break;

                    case 1:
                        if (!aiming)
                        {
                            if (PlayerAnimator.GetBool("isRifleWalk") != true)
                            {
                                SetAllBoolsFalse();
                                PlayerAnimator.SetBool("isRifleWalk", true);
                            }
                        }

                        else if (aiming)
                        {
                            if (PlayerAnimator.GetBool("RifleAimWalk") != true)
                            {
                                SetAllBoolsFalse();
                                PlayerAnimator.SetBool("RifleAimWalk", true);
                            }
                        }

                        break;
                    case 2:
                        if (!aiming)
                        {
                            if (PlayerAnimator.GetBool("isRifleWalk") != true)
                            {
                                SetAllBoolsFalse();
                                PlayerAnimator.SetBool("isRifleWalk", true);
                            }
                        }

                        else if (aiming)
                        {
                            if (PlayerAnimator.GetBool("RifleAimWalk") != true)
                            {
                                SetAllBoolsFalse();
                                PlayerAnimator.SetBool("RifleAimWalk", true);
                            }
                        }
                        break;
                }

            }

        }
    }


    void StopGame()
    {
        Time.timeScale = 0;
        DeathPanel.SetActive(true);
    }
    void Update()
    {
        CheckIdleAnimations();
        CheckWalkAnimations();

        if (health <= 0)
        {
            
            if (!isDead)
            {
                SetAllBoolsFalse();
                PlayerAnimator.SetTrigger("isDead");
                isDead = true;
                Invoke("StopGame", 7f);

            }
        }

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            //playerSounds.Stop();
            playerSounds.clip = running;

            isSprinting = true;
                

                if (CurrentInvSelect != 0 && moveInput.sqrMagnitude > 0 && !isDead)
                {
                    
                        if (PlayerAnimator.GetBool("isRunning") != true)
                        {
                            SetAllBoolsFalse();
                            PlayerAnimator.SetBool("isRunning", true);
                            playerSounds.Play();
                        }
                    

                }

                else if (CurrentInvSelect == 0 && moveInput.sqrMagnitude > 0 && !isDead)
                {
                        switch (activeGun)
                        {
                            case 0:
                                SetAllBoolsFalse();
                                PlayerAnimator.SetBool("IsPistolRunning", true);
                                isSprinting = true;
                                break;

                            case 1:
                                if (!aiming)
                                {
                                    if (PlayerAnimator.GetBool("IsRifleRunning") != true)
                                    {
                                        SetAllBoolsFalse();
                                        PlayerAnimator.SetBool("IsRifleRunning", true);
                                        isSprinting = true;
                                    }
                                }

                                else if (aiming)
                                {
                                    if (PlayerAnimator.GetBool("RifleAimRun") != true)
                                    {
                                        SetAllBoolsFalse();
                                        PlayerAnimator.SetBool("RifleAimRun", true);
                                        isSprinting = true;
                                    }
                                }
                                break;

                            case 2:
                                if (!aiming)
                                {
                                    if (PlayerAnimator.GetBool("IsRifleRunning") != true)
                                    {
                                        SetAllBoolsFalse();
                                        PlayerAnimator.SetBool("IsRifleRunning", true);
                                        isSprinting = true;
                                    }
                                }

                                else if (aiming)
                                {
                                    if (PlayerAnimator.GetBool("RifleAimRun") != true)
                                    {
                                        SetAllBoolsFalse();
                                        PlayerAnimator.SetBool("RifleAimRun", true);
                                        isSprinting = true;
                                    }
                                }
                                    break;
                        }
                    
                }





            
        }
        else if (Keyboard.current.shiftKey.wasReleasedThisFrame && !isDead)
        {
            //playerSounds.Stop();
            playerSounds.clip = walking;
            isSprinting = false;
            if (moveInput.sqrMagnitude > 0)
            {
                playerSounds.Play();
                
            }
        }

        if (Keyboard.current[Key.Digit1].wasPressedThisFrame && !InPuzzle && !isDead)
        {
            if (CurrentInvSelect == 0)
            {
                playerSounds.PlayOneShot(switchWeapons);
                activeGun++;
                if (activeGun > 2)
                {
                    activeGun = 0;
                }
            }
            playerSounds.PlayOneShot(switchWeapons);
            CurrentInvSelect = 0;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame && !InPuzzle && !isDead)
        {
            playerSounds.PlayOneShot(switchWeapons);
            CurrentInvSelect = 1;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame && !InPuzzle && !isDead)
        {
            playerSounds.PlayOneShot(switchWeapons);
            CurrentInvSelect = 2;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame && !InPuzzle && !isDead)
        {
            playerSounds.PlayOneShot(switchWeapons);
            CurrentInvSelect = 3;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit5].wasPressedThisFrame && !InPuzzle && !isDead)
        {
            playerSounds.PlayOneShot(switchWeapons);
            CurrentInvSelect = 4;
            UpdateUI();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash && stamina >= 30 && !InPuzzle && !isDead)
        {
            StartCoroutine(Dash());
            playerSounds.PlayOneShot(dash);
        }

        else if (Keyboard.current.cKey.isPressed && InPuzzle && !isDead)
        {
            StartCoroutine(Dash());
            playerSounds.PlayOneShot(dash);
            playerSounds.PlayOneShot(generatorStop);
        }

        if (!isDead)
        {
            CheckGrenade();
            CheckBandage();
            CheckLoot();
            CheckReload();
            StimActivate();
            CondensorActivate();
        }

        if (!isDead)
        {

            switch (activeGun)
            {
                case 0:
                    currentDamage = 35;
                    Shoot();
                    if (CurrentInvSelect == 0 && !InLoot && !isDead)
                    {
                        RifleObject.SetActive(false);
                        PistolObject.SetActive(true);
                        ShotgunObject.SetActive(false);
                    }


                    else
                    {
                        RifleObject.SetActive(false);
                        PistolObject.SetActive(false);
                        ShotgunObject.SetActive(false);
                    }
                        return;

                case 1:
                    currentDamage = 15;
                    AssaultRifle();
                    if (CurrentInvSelect == 0 && !InLoot && !isDead)
                    {
                        RifleObject.SetActive(true);
                        PistolObject.SetActive(false);
                        ShotgunObject.SetActive(false);
                    }

                    else
                    {
                        RifleObject.SetActive(false);
                        PistolObject.SetActive(false);
                        ShotgunObject.SetActive(false);
                    }
                    return;

                case 2:
                    currentDamage = 45;
                    Shotgun();
                    if (CurrentInvSelect == 0 && !InLoot && !isDead)
                    {
                        RifleObject.SetActive(false);
                        PistolObject.SetActive(false);
                        ShotgunObject.SetActive(true);
                    }

                    else
                    {
                        RifleObject.SetActive(false);
                        PistolObject.SetActive(false);
                        ShotgunObject.SetActive(false);
                    }
                    return;
            }
        }

        

    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        stamina -= 30;
        
        Vector3 dashDirection = moveDirection;

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
        if (!InPuzzle || DashOut && !isDead)
        {

            if (stamina <= 5)
            {
                allowSprint = false;
                Invoke("ResetSprint", 3f);
            }

            if (Keyboard.current.shiftKey.isPressed && stamina > 0 && allowSprint && moveInput.sqrMagnitude > 0.01f)
            {
                moveSpeed = 9;
                if (!stimActive)
                {
                    stamina -= sprintDepletionRate * Time.deltaTime;
                }
               
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

                moveDirection = forward * moveInput.y + right * moveInput.x;

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

        if (Keyboard.current.cKey.isPressed && InPuzzle && !isDead)
        {
            ExitAllMinigames();
            lootPanel.SetActive(false);
            DashOut = true;

        }
    }

    void RotateTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;

            Vector3 direction = (lookPoint - transform.position).normalized;

            if (direction != Vector3.zero && !isDead)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }
        }

    }


    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)

        {
            case "TestTrigger":

                CurrentGenerator = other.gameObject;

                if (interactTextNum == 0 && CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete < 3)
                {
                    interactTextNum++;
                    InteractText.SetActive(true);
                }

                if (Keyboard.current.fKey.isPressed == true && CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete < 3) 
                
                {
                    if (!InPuzzle)
                    {
                        playerSounds.PlayOneShot(generatorStart);
                        
                    }
                    if (interactTextNum == 1)
                    {
                        interactTextNum = -1;
                        InteractText.SetActive(false);
                        DashOut = false;
                    }

                    int randomNum = Random.Range(1, 5);


                    UITransparent();

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


                }
                    return;

            case "LootTrigger":
                
                currentLootBox = other.gameObject;
                InLootTrigger = true;

                if (interactTextNum == 0 && currentLootBox.GetComponent<LootBox>().IsLooted == false)
                {
                    interactTextNum++;
                    InteractText.SetActive(true);
                }

                if (Keyboard.current.fKey.isPressed == true && currentLootBox.GetComponent<LootBox>().IsLooted == false && !InLoot)
                {
                    InLoot = true;
                    if (PlayerAnimator.GetBool("isRummage") != true)
                    {
                        SetAllBoolsFalse();
                        PlayerAnimator.SetBool("isRummage", true);
                    }
                    if (interactTextNum == 1)
                    {
                        interactTextNum = -1;
                        InteractText.SetActive(false);
                        DashOut = false;
                    }

                    lootPanel.SetActive(true);
                    playerSounds.PlayOneShot(lootTrashcan);

                    if (currentLootBox.GetComponent<LootBox>().Randomized == false) 
                    {
                        currentLootBox.GetComponent<LootBox>().RandomizeLoot();
                        currentLootBox.GetComponent<LootBox>().Randomized = true;
                    }

                   else if (currentLootBox.GetComponent<LootBox>().Randomized == true)
                    {
                        currentLootBox.GetComponent<LootBox>().AssignImages();
                    }

                }

               

                    return;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)

        {
            case "DeathPlane":
                health = 0;
                break;
            case "Enemy":
                if (health > 0)
                { 
                    health -= 5;
                    playerSounds.loop = false;
                    int grunt = Random.Range(1,5);
                    switch (grunt)
                    {
                        case 1:
                            playerSounds.clip = grunt1;
                            playerSounds.Play();
                            break;
                        case 2:
                            playerSounds.clip = grunt2;
                            playerSounds.Play();
                            break;
                        case 3:
                            playerSounds.clip = grunt3;
                            playerSounds.Play();
                            break;
                        case 4:
                            playerSounds.clip = grunt4;
                            playerSounds.Play();
                            break;
                    }
                }
                enemyDamage = other.gameObject;
                enemyDamage.GetComponent<Enemy>().HitboxGameObject.GetComponent<EnemyDetect>().canMove = false;
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)

        {
            case "TestTrigger":

                InteractText.SetActive(false);
                interactTextNum = 0;
                break;

            case "LootTrigger":

                InteractText.SetActive(false);
                interactTextNum = 0;
                InLootTrigger = false;
                lootPanel.SetActive(false);
                InLoot = false;
                break;

        }
    }
  

    private void ResetSprint()
    {
        allowSprint = true;
    }


    public void Shoot()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && aiming && CurrentInvSelect == 0 && pistolMag > 0)

        { 
            

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);

                RaycastHit hit;

                float raycastDistance = 100f;

                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    playerSounds.PlayOneShot(pistolShot);
                    Debug.Log($"Raycast hit at: {hit.point}");

                    Vector3 impact = (hit.point - transform.position).normalized;

                    if (impact.sqrMagnitude > 0.01f)
                    {
                        GameObject projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
                        projectileInstance.GetComponent<FireBallScript>().targetPos = impact;
                        pistolMag--;

                        Debug.Log($"Projectile Direction: {impact}");

                    }

                    else
                    {
                    Debug.LogWarning("Calculated direction is too small, not spawning projectile.");
                    }

                }

            else
            {
                Debug.LogWarning("Raycast did not hit anything.");
            }

        }
            

    }


    public void Shotgun()
    {
        // Cocking mechanic
        if (Keyboard.current.rKey.wasPressedThisFrame && !isCocked)
        {
            playerSounds.PlayOneShot(shotgunRecock);
            isCocked = true;
            Debug.Log("Gun Cocked");
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && aiming && CurrentInvSelect == 0 && shotgunMag > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);

            RaycastHit hit;

            float raycastDistance = 100f;

            if (!isCocked)
            {
                Debug.Log("Needs recocking!");
                return;
            }
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                playerSounds.PlayOneShot(shotgunShot);
                Vector3 impact = (hit.point - transform.position).normalized;
                impact.y = 0;

                if (impact.sqrMagnitude > 0.01f)
                {
                    Vector3 rightDir = Camera.main.transform.right;
                    rightDir.y = 0;
                    rightDir.Normalize();

                    for (int i = 0; i < shotgunSpreadPattern.Length; i++)
                    {
                        Vector3 spreadDirection = (impact + (rightDir * shotgunSpreadPattern[i])).normalized;

                        GameObject projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);

                        FireBallScript script = projectileInstance.GetComponent<FireBallScript>();
                        script.targetPos = spreadDirection;
                        //script.damage = 30f;
                    }

                    isCocked = false;
                    shotgunMag--;
                }
            }
        }
    }

    public void AssaultRifle()
    {
        if (Mouse.current.leftButton.isPressed && aiming && CurrentInvSelect == 0 && assaultMag > 0)
        {
            //handles fire rate
            if (Time.time >= lastFireTime + fireRate)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
                RaycastHit hit;
                float raycastDistance = 100f;

                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    playerSounds.PlayOneShot(rifleShot);
                    //handles direction
                    Vector3 impact = (hit.point - transform.position).normalized;
                    impact.y = 0;

                    if (impact.sqrMagnitude > 0.01f)
                    {
                        //handles bloom
                        Vector3 rightDir = Camera.main.transform.right;
                        rightDir.y = 0;
                        rightDir.Normalize();

                        //Apply bloom
                        float randomBloom = Random.Range(-bloom, bloom);
                        Vector3 finalDirection = (impact + (rightDir * randomBloom)).normalized;

                        //Spawn Projectile
                        GameObject projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
                        FireBallScript script = projectileInstance.GetComponent<FireBallScript>();

                        script.targetPos = finalDirection;
                        //script.damage = 8f; 

                        //Update timer
                        lastFireTime = Time.time;

                        Debug.Log("AR Fired");
                        assaultMag--;
                    }
                }
            }
        }
    }
    void ThrowGrenade(Vector3 targetPosition)
    {

            GameObject grenade = Instantiate(GrenadePrefab, throwPoint.position, Quaternion.identity);

            Rigidbody rb = grenade.GetComponent<Rigidbody>();


            float timeToTarget = 2.5f; //  tweak this for arc shape

            Vector3 velocity = CalculateLaunchVelocity(throwPoint.position, targetPosition, timeToTarget);
            
            Debug.Log("Applying velocity: " + velocity);

            rb.linearVelocity = velocity; 

        
    }


    void ResetGrenading()
    {
        Grenading = false;
    }

    void CheckGrenade()
    {
       
        if (Mouse.current.leftButton.wasPressedThisFrame && aiming && CurrentInvSelect == 3 && grenades > 0)
        {
            if (PlayerAnimator.GetBool("isGrenadingStill") != true && moveInput.magnitude == 0)
            {
                SetAllBoolsFalse();
                PlayerAnimator.SetBool("isGrenadingStill", true);
                Grenading = true;
                Invoke("ResetGrenading", 1f);
            }


            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);

            RaycastHit hit;

            float raycastDistance = 100f;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                
                ThrowGrenade(hit.point);
                grenades--;
            }
        }

        
    }
    void CheckBandage()
    {
       
        if (Mouse.current.leftButton.wasPressedThisFrame && CurrentInvSelect == 4 && bandages > 0)
        {
            health += 25;
            playerSounds.PlayOneShot(bandage);
            bandages--;
        }

        
    }
    public static Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float time)
    {
        Vector3 displacement = target - start;

        // Horizontal velocity (x and z)
        Vector3 velocity = new Vector3( displacement.x / time, 0f,displacement.z / time );

        // Vertical velocity (y)
        velocity.y = (displacement.y / time) - (0.5f * Physics.gravity.y * time);

        return velocity;
    }
    public void UpdateUI()
    {
       
        arrow.transform.rotation = Quaternion.Euler(0, 0, ((360 / 100) * HeaterUp) * -1);
        staminaBar.value = stamina;

        float HeatValueFloat = HeaterUp;
        Debug.Log(HeatValueFloat);
        Debug.Log(HeatValueFloat/100f);
        if (HeatValueFloat >= 60)
        {
            HeatWarningCanvasGroup.alpha = ((HeatValueFloat - 60) / 100);
        }
        

        if (health > 100) { health = 100; }

        //heartbeat UI
        if (health >= 80)
        {
            heartBeatAnimator.speed = 0.4f;
            heartBeatText.text = "80";
            targetImageLine.color = Color.cyan;
        }
        else if (health >= 60)
        {
            heartBeatAnimator.speed = 0.6f;
            heartBeatText.text = "90";
            targetImageLine.color = Color.cyan;
        }
        else if (health >= 40)
        {
            heartBeatAnimator.speed = 0.8f;
            heartBeatText.text = "100";
            targetImageLine.color = Color.orange;
        }
        else if (health >= 20)
        {
            heartBeatAnimator.speed = 1.0f;
            heartBeatText.text = "110";
            targetImageLine.color = Color.orangeRed;
        }
        else if (health < 20)
        {
            heartBeatAnimator.speed = 1.2f;
            heartBeatText.text = "120";
            targetImageLine.color = Color.red;
        }

        switch (activeGun)
        {
            case 0:
                gunSlot.GetComponent<Image>().sprite = PistolImage;
                MagNum.text = pistolMag.ToString();
                MaxAmmo.text = pistolAmmo.ToString();
                break;

            case 1:
                gunSlot.GetComponent<Image>().sprite = AssaultRifleIMAGE;
                MagNum.text = assaultMag.ToString();
                MaxAmmo.text = assaultAmmo.ToString();
                break;

            case 2:
                gunSlot.GetComponent<Image>().sprite = ShotgunIMAGE;
                MagNum.text = shotgunMag.ToString();
                MaxAmmo.text = shotgunAmmo.ToString();
                break;
        }

        invSlotTwo.SetActive(condensors > 0);
        invSlotThree.SetActive(stims > 0);
        invSlotFour.SetActive(grenades > 0);
        invSlotFive.SetActive(bandages > 0);

        CondensorNum.text = condensors.ToString();
        StimNum.text = stims.ToString();
        GrenadeNum.text = grenades.ToString(); 
        BandageNum.text = bandages.ToString();

        for (int i = 0; i < InventorySelects.Length; i++)
        {
            InventorySelects[i].SetActive(false);
        }

        InventorySelects[CurrentInvSelect].SetActive(true);

    }
   
    void ExitAllMinigames()
    {
        UIOpaque();
        PuzzleManager.GetComponent<Puzzles>().CloseMinigame();
        PasswordManager.GetComponent<PasswordMinigameManager>().CloseMinigame();
        HeatMinigameManager.GetComponent<HeatMeter>().CloseMinigame();
        OxygenMinigameManager.GetComponent<OxygenLevelsPuzzle>().CloseMinigame();
    }
    

    void ResetCondensorAnimation()
    {
        CondensorAnimation = false;
    }

    void CondensorActivate()
    {
        

        if(CurrentInvSelect == 1 && condensors > 0 && !InPuzzle && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CondensorAnimation = true;
            Invoke("ResetCondensorAnimation", 1f);
            if (condensorActive)
            {
                condensorHealth = 50;
                condensors--;
                return;
            }

            if (PlayerAnimator.GetBool("isCondensor") != true)
            {
                SetAllBoolsFalse();
                PlayerAnimator.SetBool("isCondensor", true);
                condensorActive = true;
                playerSounds.PlayOneShot(useCondensor);
               
            }


            condensors--;
            condensorHealth = 50;
            StopHeatCoroutine();
            StopCondensorCoroutine();
            StopCondensorDecrementCoroutine();

            condensorHealthCoroutine = StartCoroutine(CondensorDecrementCoroutine());
            moveSpeed = moveSpeed / 2;
            CancelInvoke("CondensorSecure");
            Invoke("CondensorSecure", 1.75f);
        }
    }


    void StimActivate()
    {
        if (CurrentInvSelect == 2 && stims > 0 && !InPuzzle && Mouse.current.leftButton.wasPressedThisFrame)
        {
            stims--;
            playerSounds.PlayOneShot(syringe);
            stamina = 100;
            stimActive = true;
            CancelInvoke("ResetStim");
            Invoke("ResetStim", 10f);
        }
    }

    void CondensorSecure()
    {
        moveSpeed = moveSpeed * 2;
        condensorActiveCoroutine = StartCoroutine(CondensorCoroutine());
       
    }

    void StopCondensorCoroutine()
    {
        if (condensorActiveCoroutine != null)
        {
            StopCoroutine(condensorActiveCoroutine);
            condensorActiveCoroutine = null;
        }
    }
    void StopCondensorDecrementCoroutine()
    {
        if (condensorHealthCoroutine != null)
        {
            StopCoroutine(condensorHealthCoroutine);
            condensorHealthCoroutine = null;
        }

        
    }
    void StopHeatCoroutine()
    {
        if (heatCoroutine != null)
        {
            StopCoroutine(heatCoroutine);
            heatCoroutine = null;
        }
    }

    void StartCondensorHealthCoroutine()
    {
        condensorHealthCoroutine = StartCoroutine(CondensorDecrementCoroutine());
    }

    void StartHeatCoroutine()
    {
        heatCoroutine = StartCoroutine(HeatCoroutine());
    }

    void ResetStim()
    {
        stimActive = false;
    }


    void CheckReload()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame && pistolMag == 0)
        {
            if (pistolAmmo < 15)
            {
                pistolMag += pistolAmmo;
                pistolAmmo = 0;
                playerSounds.PlayOneShot(pistolReload);
            }

            else
            {
                pistolAmmo -= 15;
                pistolMag += 15;
                playerSounds.PlayOneShot(pistolReload);
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && assaultMag == 0)
        {
            if (assaultAmmo < 25)
            {
                assaultMag += assaultAmmo;
                assaultAmmo = 0;
                playerSounds.PlayOneShot(rifleReload);
            }

            else
            {
                assaultAmmo -= 25;
                assaultMag += 25;
                playerSounds.PlayOneShot(rifleReload);
            }

        }

        if (Keyboard.current.rKey.wasPressedThisFrame && shotgunMag == 0)
        {
            if (shotgunAmmo < 10)
            {
                shotgunMag += shotgunAmmo;
                shotgunAmmo = 0;
                playerSounds.PlayOneShot(shotgunReload);
            }

            else
            {
                shotgunAmmo -= 10;
                shotgunMag += 10;
                playerSounds.PlayOneShot(shotgunReload);
            }
        }
    }

    public void UITransparent()
    {
        UICanvasGroup.alpha = 0.4f;
    }

    public void UIOpaque()
    {
        UICanvasGroup.alpha = 1f;
    }

    void CheckLoot()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && InLoot)
        {
            playerSounds.PlayOneShot(looting2);
            switch (currentLootBox.GetComponent<LootBox>().CurrentLoot[0])
            {
                case "Stim":
                    stims++;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;

                case "Grenade":
                    grenades++;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;

                case "PistolAmmo":
                    pistolAmmo += 25;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;

                case "AssaultAmmo":
                    assaultAmmo += 50;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;

                case "ShotgunAmmo":
                    shotgunAmmo += 10;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;

                case "Bandage":
                    bandages += 2;
                    currentLootBox.GetComponent<LootBox>().CurrentLoot.RemoveAt(0);
                    currentLootBox.GetComponent<LootBox>().ResetImages();
                    currentLootBox.GetComponent<LootBox>().AssignImages();
                    UpdateUI();
                    break;



            }
            if (currentLootBox.GetComponent<LootBox>().CurrentLoot.Count == 0)
            {
                currentLootBox.GetComponent<LootBox>().IsLooted = true;
                lootPanel.SetActive(false);

            }
        }
    }

}
