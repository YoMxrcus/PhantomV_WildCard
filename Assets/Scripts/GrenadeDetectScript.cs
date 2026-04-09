using UnityEngine;

public class GrenadeDetectScript : MonoBehaviour
{
    [SerializeField] bool PlayerInField = false;
    public GameObject Player;
    [SerializeField] GameObject ParentGrenade;
    public ParticleSystem Explosion;
    public AudioSource enemyGrenade;
    public AudioClip grenadeBoom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        Invoke("BoomCountdown", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerInField = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerInField = false;
                break;
        }
    }

    void DestroyGrenade()
    {
        Destroy(transform.parent.gameObject);
    }

    void BoomCountdown()
    {
        if (PlayerInField)
        {
            Player.GetComponent<ThirdPersonController>().health -= 20;
           
        }

        Explosion.Play();
        enemyGrenade.PlayOneShot(grenadeBoom);
        if (transform.parent != null)
        {
            Invoke("DestroyGrenade", .5f);
        }
    }
}
