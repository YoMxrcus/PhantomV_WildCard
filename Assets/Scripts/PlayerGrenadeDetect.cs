using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadeDetect : MonoBehaviour
{
    public AudioSource playerGrenade;
    public AudioClip grenadeBoom;
    [SerializeField] bool PlayerInField = false;
    public GameObject Player;
    [SerializeField] GameObject ParentGrenade;
    public List<GameObject> activeEnemies = new List<GameObject>();
    public ParticleSystem Explosion;
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
            case "BanditHitbox":

                if (!activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Add(other.gameObject);
                }

                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "BanditHitbox":
                activeEnemies.RemoveAll(item => item == null);

                if (activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Remove(other.gameObject);
                }
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "BanditHitbox":
                activeEnemies.RemoveAll(item => item == null);

                if (!activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Add(other.gameObject);
                }
                break;
        }
    }


void DestroyGrenade()
    {
        Destroy(transform.parent.gameObject);
    }
    void BoomCountdown()
    {

        for (int i = 0; i < activeEnemies.Count; i++) 
        {
            if (activeEnemies[i].name.Contains("Bandit"))
            {
                activeEnemies[i].GetComponentInChildren<BanditHealth>().banditHealth -= 75;
            }

            else
            {
                activeEnemies[i].GetComponent<Enemy>().EnemyHealth -= 75;
            }
        }

        Explosion.Play();
        playerGrenade.PlayOneShot(grenadeBoom);

        if (transform.parent != null)
        {
            Invoke("DestroyGrenade", .5f);
        }
    }
}
