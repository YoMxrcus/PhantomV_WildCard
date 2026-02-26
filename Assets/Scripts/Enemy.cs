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
            /*case "Fireball":
                if (Player.GetComponent<SpellCasting>().EnhancementActive)
                {
                    EnemyHealth -= 100;
                    Player.GetComponent<SpellCasting>().EnhancementActive = false;
                    Player.GetComponent<SpellCasting>().Hitmarker.SetActive(true);
                    Player.GetComponent<SpellCasting>().Invoke("ResetHitmarker", .2f);
                }
                else { EnemyHealth -= 50; Player.GetComponent<SpellCasting>().Hitmarker.SetActive(true); Player.GetComponent<SpellCasting>().Invoke("ResetHitmarker", .2f); }

                if (EnemyHealth <= 0)
                {
                    Destroy(gameObject);
                }
                break;
            */
            
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

