using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public Vector3 targetPos; //Stores Target Location

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 8f);
        CheckDestroy();
    }


    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Enemy"))
       {
            Destroy(gameObject);
       }

      
    }

    void CheckDestroy()
    {
        if (transform.position == targetPos) 
        {
            Destroy(gameObject);
        }

        
    }
}
