using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int EnemyHealth = 100;
    public GameObject Player;
    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "projectile":
                EnemyHealth -= 20;

                if (EnemyHealth <= 0)
                {
                    DestroyEnemy();
                }
                return;
            
        }

        
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "LightningCloud":
                //Player.GetComponent<SpellCasting>().Hitmarker.SetActive(true);
                //Player.GetComponent<SpellCasting>().Invoke("ResetHitmarker", .2f);
                EnemyHealth -= 100;
                Invoke("DestroyEnemy", 1f);
                break;
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}

