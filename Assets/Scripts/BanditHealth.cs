using Unity.VisualScripting;
using UnityEngine;

public class BanditHealth : MonoBehaviour
{
    public int banditHealth = 100;
    public GameObject Player;
    public GameObject Bandit;
    public bool isDead = false;
    public AudioSource banditGrunts;
    public AudioClip banditGrunt1, banditGrunt2, banditGrunt3, banditGrunt4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "projectile":
                banditHealth -= Player.GetComponent<ThirdPersonController>().currentDamage;
                banditGrunts.loop = false;
                int grunt = Random.Range(1, 5);
                switch (grunt)
                {
                    case 1:
                        banditGrunts.clip = banditGrunt1;
                        banditGrunts.Play();
                        break;
                    case 2:
                        banditGrunts.clip = banditGrunt2;
                        banditGrunts.Play();
                        break;
                    case 3:
                        banditGrunts.clip = banditGrunt3;
                        banditGrunts.Play();
                        break;
                    case 4:
                        banditGrunts.clip = banditGrunt4;
                        banditGrunts.Play();
                        break;
                }
                if (banditHealth <= 0)
                {
                    CheckDestroy();
                }

                

                Destroy(other.gameObject);
                return;

        }


    }
    void DestroyBandit()
    {
        Destroy(transform.parent.gameObject);
    }
    private void Update()
    {

        CheckDestroy();
    }



    void CheckDestroy()
    {
        if (banditHealth <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                gameObject.GetComponentInChildren<BanditEnemy>().BanditAnimator.SetTrigger("isDead");
                Invoke("DestroyBandit", 5f);

            }
        }
    }
}
