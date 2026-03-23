using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyDetect : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    float speed = 2f;
    public bool canLunge = true;
    public bool canMove = true;
    //public GameObject[] enemyStance = new GameObject[4];
    //public float animationSpeed = 1f; // Time between stance swaps

    //private Coroutine stanceRoutine;
    //private bool isMoving = false;
    public ThirdPersonController playerScript;

    //public GameObject EnemyAttack1, EnemyAttack2;

    //public bool CancelAnimation = false;

    //public Animator GoblinAnim;
  

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canMove == true)
            {
                // Movement Logic
                if (canLunge)
                {
                    AllowLunge();
                    Invoke("DisallowLunge", .25f);
                    Invoke("AllowLunge", 5f);
                }
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, speed * Time.deltaTime);
                //GoblinAnim.SetBool("Attack", false);
                //GoblinAnim.SetBool("Walking", true);

                // Start the loop if we aren't already moving

            }
            else
            {

                //GoblinAnim.SetBool("Attack", true);
                StopMovement();
                Invoke("AllowEnemyMovement", 3f);
            }
        }
    }
    void AllowLunge()
    {
        canLunge = true;
        speed = 10;
    }    
    void DisallowLunge()
    {
        canLunge = false;
        speed = 2;
    }    

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoblinAnim.SetBool("Walking", false);
            GoblinAnim.SetBool("Attack", false);
            //StopMovement();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoblinAnim.SetBool("Walking", true);
            GoblinAnim.SetBool("Attack", false);
            //CancelAnimation = true;

        }
    }

    

    void EnemySecondAttack()
    {
        if (CancelAnimation)
        {
            EnemyAttack1.SetActive(false);
            EnemyAttack2.SetActive(false);
            enemyStance[0].SetActive(true);
            CancelAnimation = false;
        }
        else
        {
            EnemyAttack1.SetActive(false);
            EnemyAttack2.SetActive(true);
        }
            
    }

    void EndEnemyAttack()
    {
        if (CancelAnimation)
        {
            EnemyAttack1.SetActive(false);
            EnemyAttack2.SetActive(false);
            enemyStance[0].SetActive(true);
            CancelAnimation = false;
        }

        else
        {
            EnemyAttack2.SetActive(false);
            enemyStance[0].SetActive(true);
        }

            
    }*/
    void StopMovement()
    {
        enemy.transform.position = transform.position;
    }

    void AllowEnemyMovement()
    {
       canMove = true;

    }
}