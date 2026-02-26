using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyDetect : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public GameObject[] enemyStance = new GameObject[4];
    public float animationSpeed = 1f; // Time between stance swaps

    private Coroutine stanceRoutine;
    private bool isMoving = false;
    //private SpellCasting playerSpellScript;

    public GameObject EnemyAttack1, EnemyAttack2;

    public bool CancelAnimation = false;

    public Animator GoblinAnim;
    void Start()
    {
        // calling the script for better performance
        //playerSpellScript = player.GetComponent<SpellCasting>();

        // Ensures only the first stance is active at start
       
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Player"))
        {
            if (playerSpellScript.canMove)
            {
                // Movement Logic
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, 5f * Time.deltaTime);
                GoblinAnim.SetBool("Attack", false);
                GoblinAnim.SetBool("Walking", true);

                // Start the loop if we aren't already moving

            }
            else
            {

                GoblinAnim.SetBool("Attack", true);
                //StopMovement();
            }
        }*/
    }

    private void OnTriggerExit(Collider other)
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

            
    }
}