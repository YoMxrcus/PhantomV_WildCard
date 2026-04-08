using Unity.VisualScripting;
using UnityEngine;

public class BanditHealth : MonoBehaviour
{
    public int banditHealth = 100;
    public GameObject Player;
    public GameObject Bandit;
    public bool isDead = false;
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
