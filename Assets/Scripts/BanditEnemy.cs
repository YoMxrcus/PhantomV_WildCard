using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;


public class BanditEnemy : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public GameObject playerMover;
    public GameObject fireBall;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float speed = 3f;
    public bool canShoot = true;
    public bool canMove = true;
    public int activeActionNum;
    public List<string> possibleActions = new List<string>() {"Hide", "Fight", "Grenade", "Heal"};
    public List<GameObject> activeCovers = new List<GameObject>();
    public bool hiding, fighting, healing, grenading;
    bool collectingInitial = true;
    private Rigidbody rb;
    public Vector3 movePos;
    public bool moveTime = false;
    public bool moveRandom = false;
    public int shootFunction = 1;
    public Vector3 playerPos;
    Vector3 randomPos;
    public Vector3 direction;
    public Coroutine ShootCoroutine, MoveRandomCoroutine, StartActionCoroutine;
    public string currentActionStr;
    public bool ActionStarted = false;
    float yPosition;
    public GameObject ActiveCoverObject;

    public GameObject GrenadePrefab;
    [SerializeField] Transform throwPoint;
    [SerializeField] float launchAngle = 45f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        StartCoroutine(IntialScan());
        player = GameObject.Find("MainPlayer");
        playerMover = GameObject.Find("ThirdPersonTEST");
        Vector3 playerPos = playerMover.transform.position;
        firePoint = enemy.transform; 
        hiding = false;
        fighting = false;
        healing = false;
        grenading = false;
    }

    IEnumerator IntialScan()
    {
        yield return new WaitForSeconds(1f); // wait one frame
        collectingInitial = false;
    }
    private void Update()
    {
        yPosition = player.transform.position.y;
        playerPos = playerMover.transform.position;
        
    }
    private void FixedUpdate()
    {
       
       

        if (moveTime)
        {
            Vector3 MoveFunction = Vector3.MoveTowards(enemy.transform.position, new Vector3 (movePos.x, yPosition, movePos.z), speed * Time.deltaTime);
            enemy.transform.position = MoveFunction;
            
        }

        if (moveRandom)
        {
            EndShootCoroutine();
            Vector3 MoveFunction = Vector3.MoveTowards(enemy.transform.position, randomPos, speed * Time.deltaTime);
            enemy.transform.position = MoveFunction;
            if (enemy.transform.position == randomPos)
            {
                moveRandom = false;
                shootFunction = 1;
            }
        }

        if (!moveRandom && fighting && shootFunction == 1)
        {
            ShootCoroutine = StartCoroutine(ShootProjectile(firePoint));
            shootFunction = 0;
        }
    }

    public IEnumerator SwitchActionCoroutine(string currentAction)
    {
        CheckAction(currentAction); 

        while (true)
        {
            Debug.Log("reached here");
            yield return new WaitForSeconds(8f);

            Debug.Log("reached here2");
            activeActionNum = Random.Range(0, 3);
            currentActionStr = possibleActions[activeActionNum];
            Debug.Log(currentActionStr);
            CheckAction(currentActionStr);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                
                    activeActionNum = Random.Range(0, 3);
                    currentActionStr = possibleActions[activeActionNum];
                    StartActionCoroutine = StartCoroutine(SwitchActionCoroutine(currentActionStr));
                    
                
                break;

            case "CoverObject":

                activeCovers.RemoveAll(item => item == null);
                
                  if (!activeCovers.Contains(other.gameObject))
                  {
                      activeCovers.Add(other.gameObject);
                  }
                
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
       
        switch (other.gameObject.tag)
        {
            case "Player":
                if (canShoot)
                {
                  
                }
                break;

            case "CoverObject":
                activeCovers.RemoveAll(item => item == null);

                if (collectingInitial)
                {
                    if (!activeCovers.Contains(other.gameObject))
                    {
                        activeCovers.Add(other.gameObject);
                    }
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag) 
        {
            case "CoverObject":

                activeCovers.RemoveAll(item => item == null);

                if (activeCovers.Contains(other.gameObject))
                {
                    activeCovers.Remove(other.gameObject);
                }

                break;

            case "Player":
                StopAllCoroutines();
                StartActionCoroutine = null;
                ShootCoroutine = null;
                MoveRandomCoroutine = null;
                break;
        }
    }


    public IEnumerator RandomMoveCoroutine()
    {
        moveRandom = true;
        randomPos = new Vector3(transform.position.x + Random.Range(-3, 4), yPosition, transform.position.z + Random.Range(-3, 4));

        while (fighting)
        {
            yield return new WaitForSeconds(10f);
            moveRandom = true;
            randomPos = new Vector3(transform.position.x + Random.Range(-3, 4), yPosition, transform.position.z + Random.Range(-3, 4));
        }

    }


    public IEnumerator ShootProjectile(Transform firePoint)
    {
        while (!moveRandom)
        {

            yield return new WaitForSeconds(1f);

            playerPos = playerMover.transform.position;

            var projectileObj = Instantiate(fireBall, firePoint.position, Quaternion.identity); 
            direction = (playerPos - firePoint.position).normalized;
            projectileObj.GetComponent<EnemyProjectile>().targetDirection = direction;
            Destroy(projectileObj, 5f);

        }
    }
    

    void Hide()
    {
        EndShootCoroutine(); // Stops shooting when hiding
        for (int i = 0; i < activeCovers.Count; i++)
        {
            if (!activeCovers[i].GetComponentInChildren<CoverPointScript>().isCovered)
            {
                if (ActiveCoverObject != null)
                {
                    ActiveCoverObject.GetComponentInChildren<CoverPointScript>().isCovered = false;
                }
                ActiveCoverObject = activeCovers[i];
                ActiveCoverObject.GetComponentInChildren<CoverPointScript>().isCovered = true;
                movePos = ActiveCoverObject.GetComponentInChildren<CoverPointScript>().gameObject.transform.position;
                Debug.Log(movePos);
                moveTime = true;
                Invoke("ResetMoveTime", 5f);
                activeCovers.Remove(ActiveCoverObject);
                activeCovers.Add(ActiveCoverObject);
                break;
            }

            else
            {
                Debug.Log("GameBroken");
                moveTime = false;
                continue;
            }
        }

    }

    void ResetMoveTime()
    {
        moveTime = false;
    }

    void EndShootCoroutine()
    {
        if (ShootCoroutine != null)
        {
            Debug.Log("Stop Shoot");
            StopCoroutine(ShootCoroutine);
            ShootCoroutine = null;
            Debug.Log("Stopped Shoot");
        }
    }
    void EndActionCoroutine()
    {
        if (StartActionCoroutine != null)
        {
            StopCoroutine(StartActionCoroutine);
            StartActionCoroutine = null;
        }
    }
    void EndRandomMoveCoroutine()
    {
        if (MoveRandomCoroutine != null)
        {
            StopCoroutine(MoveRandomCoroutine);
            MoveRandomCoroutine = null;
        }
    }

    void ResetActions()
    {
        fighting = false;
        hiding = false;
        healing = false;
        grenading = false;
       
    }

    void CheckAction(string currentAction)
    {
        switch (currentAction)
        {
            case "Hide":
                ResetActions();
                if (!hiding)
                {
                    Invoke("Hide", Random.Range(1f, 2f));
                    hiding = true;
                }
                break;

            case "Fight":
                ResetActions();
                if (!fighting)
                {
                    MoveRandomCoroutine = StartCoroutine(RandomMoveCoroutine());
                    fighting = true;
                }
                break;

            case "Heal":
                ResetActions();
                if (!healing)
                {
                    healing = true;
                }
                break;

            case "Grenade":
                ResetActions();
                if (!grenading)
                {
                    grenading = true;
                    ThrowGrenade(playerPos);
                    grenading = false;
                }
                break;
        }
    }

    public static Vector3 CalculateLaunchVelocity(
     Vector3 start,
     Vector3 target,
     float time
 )
    {
        Vector3 displacement = target - start;

        // Horizontal velocity (x and z)
        Vector3 velocity = new Vector3(
            displacement.x / time,
            0f,
            displacement.z / time
        );

        // Vertical velocity (y)
        velocity.y = (displacement.y / time) - (0.5f * Physics.gravity.y * time);

        return velocity;
    }

    void ThrowGrenade(Vector3 targetPosition)
    {
        EndShootCoroutine();

        GameObject grenade = Instantiate(GrenadePrefab, throwPoint.position, Quaternion.identity);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        float timeToTarget = 2.5f; //  tweak this for arc shape

        Vector3 velocity = CalculateLaunchVelocity(
            throwPoint.position,
            targetPosition,
            timeToTarget
        );

        Debug.Log("Applying velocity: " + velocity);

        rb.linearVelocity = velocity; // direct and reliable
    }
}