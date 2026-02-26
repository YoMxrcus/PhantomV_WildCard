using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public GameObject hitbox, player;
    public int lives = 100;
    public GameObject Player;
    public Animator WizardAnim;
    public void Update()
    {
        Player = GameObject.Find("PlayerMain");
    }

    private void FixedUpdate()
    {
        if (hitbox.GetComponent<RangedEnemyDetect>().canMove == true)
        {
            WizardAnim.SetBool("WalkingBool", true);
            WizardAnim.SetBool("AttackBool", false);
            Vector3 moveEnemy = Vector3.MoveTowards(transform.position, player.transform.position, 4f * Time.deltaTime);
            transform.position = moveEnemy;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Fireball":
                //Player.GetComponent<SpellCasting>().Hitmarker.SetActive(true);
                //Player.GetComponent<SpellCasting>().Invoke("ResetHitmarker", .2f);
                lives -= 35;
                if (lives <= 0)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "LightningCloud":
                //Player.GetComponent<SpellCasting>().Hitmarker.SetActive(true);
                //Player.GetComponent<SpellCasting>().Invoke("ResetHitmarker", .2f);
                lives -= 100;
                Invoke("DestroyEnemy", 1f);
                break;
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}

