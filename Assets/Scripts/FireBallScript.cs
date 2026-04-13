using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public Vector3 targetPos; //Stores Target Location
    private float speed = 30f;
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
        if(targetPos.sqrMagnitude > 0.01f)
        {
            Vector3 movement = targetPos * speed * Time.fixedDeltaTime;

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
        if (other.gameObject.tag != "PlayerTag" && other.gameObject.tag != "Player" && other.gameObject.tag != "MainCamera" && other.gameObject.tag != "Enemy" && other.gameObject.tag != "BanditHitbox" && other.gameObject.tag != "CoverObject" && other.gameObject.tag != "DioTrigger" && other.gameObject.tag != "Building" && other.gameObject.tag != "EnemySpawner" && other.gameObject.tag != "Hitbox" && other.gameObject.tag != "projectile")
        {
            Debug.Log("Object Name"+ other.gameObject.name);
            Debug.Log(other.gameObject.tag);
            Destroy(gameObject);
        }

    }


}
