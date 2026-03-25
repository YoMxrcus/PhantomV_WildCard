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
    public int health = 100;
    public bool canMove = true;
    [SerializeField] bool allowSprint = true;
    public Vector3 moveDirection;
    public int heatMeter = 0;
    public Slider heatBar, staminaBar, healthBar;
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
    public int pistolAmmo = 50;
    public int assaultAmmo = 75;
    public int shotgunAmmo = 30;
    public int activeGun = 0;
    public int pistolMag = 15;
    public int assaultMag = 25;
    public int shotgunMag = 10;
    public TMP_Text MagNum, MaxAmmo;

    private bool isCocked = true;
    // Pattern for shotgun spread
    private float[] shotgunSpreadPattern = { -0.4f, -0.2f, 0f, 0.2f, 0.4f };

    private float lastFireTime;
    public float fireRate = 0.12f; //Time between shots
    public float bloom = 0.20f; //recoil offset

    public int condensorHealth = 0;
    public Texture StimIMAGE, GrenadeIMAGE, EmptyIMAGE,CondensorImage, PistolImage, AssaultRifleIMAGE, ShotgunIMAGE;
    public GameObject invSlotOne, invSlotTwo, invSlotThree, invSlotFour;
    public GameObject[] InventorySelects;
    public TMP_Text CondensorNum, StimNum, GrenadeNum;

    [SerializeField] Coroutine heatCoroutine, condensorHealthCoroutine, condensorActiveCoroutine;

    public int CurrentInvSelect = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public IEnumerator HeatCoroutine()
    {
        HeatDecrement = true;
        while (HeaterUp < 100)
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
        
      

        UpdateUI();
    }


    void Update()
    {
        if (Keyboard.current[Key.Digit1].wasPressedThisFrame && !InPuzzle)
        {
            if (CurrentInvSelect == 0)
            {
                activeGun++;
                if (activeGun > 2)
                {
                    activeGun = 0;
                }
            }
            CurrentInvSelect = 0;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame && !InPuzzle)
        {
            CurrentInvSelect = 1;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame && !InPuzzle)
        {
            CurrentInvSelect = 2;
            UpdateUI();
        }
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame && !InPuzzle)
        {
            CurrentInvSelect = 3;
            UpdateUI();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && canDash && stamina >= 30 && !InPuzzle)
        {
            StartCoroutine(Dash());
        }

        else if (Keyboard.current.cKey.isPressed && InPuzzle)
        {
            StartCoroutine(Dash());
        }
        CheckLoot();
        CheckReload();
        StimActivate();
        CondensorActivate();

        switch (activeGun)
        {
            case 0:
                Shoot();
                return;

            case 1:
                AssaultRifle();
                return;

            case 2:
                Shotgun();
                return;
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
        if (!InPuzzle || DashOut)
        {

            if (stamina <= 5)
            {
                allowSprint = false;
                Invoke("ResetSprint", 3f);
            }

            if (Keyboard.current.shiftKey.isPressed && stamina > 0 && allowSprint)
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

        if (Keyboard.current.cKey.isPressed && InPuzzle)
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

            if (direction != Vector3.zero)
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
                    if (interactTextNum == 1)
                    {
                        interactTextNum = -1;
                        InteractText.SetActive(false);
                        DashOut = false;
                    }

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
                    if (interactTextNum == 1)
                    {
                        interactTextNum = -1;
                        InteractText.SetActive(false);
                        DashOut = false;
                    }

                    lootPanel.SetActive(true);

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

            case "Enemy":

                health -= 10;
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
    public void UpdateUI()
    {
        heatBar.value = HeaterUp;
        arrow.transform.rotation = Quaternion.Euler(0,0, ((360/100) * HeaterUp) * -1);
        staminaBar.value = stamina;
        healthBar.value = health;

        switch (activeGun)
        {
            case 0:
                invSlotOne.GetComponent<RawImage>().texture = PistolImage;
                MagNum.text = pistolMag.ToString();
                MaxAmmo.text = pistolAmmo.ToString();
                break;

            case 1:
                invSlotOne.GetComponent<RawImage>().texture = AssaultRifleIMAGE;
                MagNum.text = assaultMag.ToString();
                MaxAmmo.text = assaultAmmo.ToString();
                break;

            case 2:
                invSlotOne.GetComponent<RawImage>().texture = ShotgunIMAGE;
                MagNum.text = shotgunMag.ToString();
                MaxAmmo.text = shotgunAmmo.ToString();
                break;
        }



        if (condensors == 0)
        {
            invSlotTwo.GetComponent<RawImage>().texture = EmptyIMAGE;
        }

        else if(condensors > 0)
        {
            invSlotTwo.GetComponent<RawImage>().texture = CondensorImage;
        }

        if (stims == 0)
        {
            invSlotThree.GetComponent<RawImage>().texture = EmptyIMAGE;
        }

        else if (stims > 0)
        {
            invSlotThree.GetComponent<RawImage>().texture = StimIMAGE;
        }

        if (grenades == 0)
        {
            invSlotFour.GetComponent<RawImage>().texture = EmptyIMAGE;
        }

        else if (grenades > 0)
        {
            invSlotFour.GetComponent<RawImage>().texture = GrenadeIMAGE;
        }

        CondensorNum.text = condensors.ToString();
        StimNum.text = stims.ToString();
        GrenadeNum.text = grenades.ToString(); 

        for (int i = 0; i < InventorySelects.Length; i++)
        {
            InventorySelects[i].SetActive(false);
        }

        InventorySelects[CurrentInvSelect].SetActive(true);

    }
   
    void ExitAllMinigames()
    {
        PuzzleManager.GetComponent<Puzzles>().CloseMinigame();
        PasswordManager.GetComponent<PasswordMinigameManager>().CloseMinigame();
        HeatMinigameManager.GetComponent<HeatMeter>().CloseMinigame();
        OxygenMinigameManager.GetComponent<OxygenLevelsPuzzle>().CloseMinigame();
    }
    

    void CondensorActivate()
    {
        

        if(CurrentInvSelect == 1 && condensors > 0 && !InPuzzle && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (condensorActive)
            {
                condensorHealth = 50;
                condensors--;
                return;
            }

            condensorActive = true;

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
            }

            else
            {
                pistolAmmo -= 15;
                pistolMag += 15;
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && assaultMag == 0)
        {
            if (assaultAmmo < 25)
            {
                assaultMag += assaultAmmo;
                assaultAmmo = 0;
            }

            else
            {
                assaultAmmo -= 25;
                assaultMag += 25;
            }

        }

        if (Keyboard.current.rKey.wasPressedThisFrame && shotgunMag == 0)
        {
            if (shotgunAmmo < 10)
            {
                shotgunMag += shotgunAmmo;
                shotgunAmmo = 0;
            }

            else
            {
                shotgunAmmo -= 10;
                shotgunMag += 10;
            }
        }
    }

    void CheckLoot()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && InLoot)
        {
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



            }
            if (currentLootBox.GetComponent<LootBox>().CurrentLoot.Count == 0)
            {
                currentLootBox.GetComponent<LootBox>().IsLooted = true;
                lootPanel.SetActive(false);

            }
        }
    }

}
