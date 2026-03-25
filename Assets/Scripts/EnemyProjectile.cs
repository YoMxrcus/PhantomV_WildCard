using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Vector3 targetDirection; //Stores Target Direction
    private float speed = 20f;
    public GameObject Player;
    public float initialYPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        initialYPos = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(targetDirection.sqrMagnitude > 0.01f)
        {
            Vector3 movement = targetDirection * speed * Time.fixedDeltaTime;

            movement.y = initialYPos;

            transform.position += movement;

            transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, -10000f, 10000f),  // Clamp horizontal X
                    initialYPos,                                  // Keep Y constant
                    Mathf.Clamp(transform.position.z, -10000f, 10000f)   // Clamp horizontal Z
                );

        }
        else
        {
            Debug.LogWarning("Direction vector is zero, projectile will not move.");
        }
           

    }


    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Player"))
       {
            Destroy(gameObject);
       }

      
    }

   
}
