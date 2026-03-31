using UnityEngine;

public class BanditHealth : MonoBehaviour
{
    int banditHealth = 100;
    public GameObject Player;
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
                    Destroy(transform.parent.gameObject);
                }

                Destroy(other.gameObject);
                return;

        }


    }

}
