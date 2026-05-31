using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int EnemyHealth = 100;
    public GameObject Player;
    private GameObject SnakeShellGameObject;
    public GameObject HitboxGameObject;
    public GameObject SpawnerRef;
    

    private void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        Transform SnakeShellTransform = transform.Find("SnakeShell");
        if (SnakeShellTransform != null)
        {
            SnakeShellGameObject = SnakeShellTransform.gameObject;
            Transform HitboxTransform = SnakeShellGameObject.transform.Find("Hitbox");
            if (HitboxTransform != null)
            {
                HitboxGameObject = HitboxTransform.gameObject;
                HitboxGameObject.GetComponent<EnemyDetect>().player = Player;
                HitboxGameObject.GetComponent<EnemyDetect>().playerScript = Player.GetComponent<ThirdPersonController>();
            }
        }
        

      
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            

            case "projectile":
                EnemyHealth -= Player.GetComponent<ThirdPersonController>().currentDamage;

                if (EnemyHealth <= 0)
                {
                    DestroyEnemy();
                }

                Destroy(other.gameObject);
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
        if (SpawnerRef != null)
        {

        SpawnerRef.GetComponent<EnemySpawnerScript>().CurrentSpawnAmount--;
        }
        Destroy(gameObject);
        
    }
}

