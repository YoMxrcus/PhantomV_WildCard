using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Vector3 targetDirection; //Stores Target Direction
    private float speed = 20f;
    public GameObject Player;
    public float initialYPos;
    public AudioSource playerSounds;
    public AudioClip grunt1, grunt2, grunt3, grunt4;

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
        if (other.gameObject.tag == null || other.gameObject.tag == "CoverObject")
        {
            Destroy(gameObject);
        }
   

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ThirdPersonController>().health -= 5;
            Destroy(gameObject);
            playerSounds.loop = false;
            int grunt = Random.Range(1, 5);
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

    }
}
