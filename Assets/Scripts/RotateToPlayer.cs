using UnityEngine;



public class RotateToPlayer : MonoBehaviour
{
    public Transform playerTransform; // Assign the player's Transform in the Inspector
    public float rotationSpeed = 5f; // Adjust the rotation speed

    public bool playerInTrigger = false;
    public GameObject Player;
    public bool IsBandit; 

    private void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        if (Player != null) { playerTransform = Player.transform; }
        if (gameObject.GetComponentInChildren<BanditHealth>() != null)
        {
            IsBandit = true;
        }

        else { IsBandit = false; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player GameObject has the "Player" tag
        {
            playerInTrigger = true;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    void Update()
    {
        if (playerInTrigger && playerTransform != null && !gameObject.GetComponentInChildren<BanditHealth>().isDead)
        {
            // Calculate the direction to the player
            if (!gameObject.GetComponentInChildren<BanditEnemy>().moveTime)
            {
                Vector3 directionToPlayer = playerTransform.position - transform.position;
                directionToPlayer.y = 0; // Keep the rotation on the Y-axis (horizontal)

                // Create a target rotation looking at the player
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            }


            else if (gameObject.GetComponentInChildren<BanditEnemy>().moveTime && !gameObject.GetComponentInChildren<BanditHealth>().isDead)
            {
                Vector3 directionToCover = gameObject.GetComponentInChildren<BanditEnemy>().movePos - transform.position;
                directionToCover.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(directionToCover);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}