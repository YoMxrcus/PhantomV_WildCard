using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyDetect : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    [SerializeField] private bool lockEnemyHeight = true;
    [SerializeField] private float lockedHeightOffset = 1f;

    float speed = 2f;
    public bool canLunge = true;
    public bool canMove = true;
    public Animator SnakeAnimator;
    //public GameObject[] enemyStance = new GameObject[4];
    //public float animationSpeed = 1f; // Time between stance swaps

    //private Coroutine stanceRoutine;
    //private bool isMoving = false;
    public ThirdPersonController playerScript;

    //public GameObject EnemyAttack1, EnemyAttack2;

    //public bool CancelAnimation = false;

    //public Animator GoblinAnim;

    private bool hasLockedHeight;
    private float lockedWorldY;

    private void Awake()
    {
        CacheLockedHeight();
    }

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
                    if (SnakeAnimator.GetBool("LungingFast") != true)
                    {
                        SetAllBoolsFalse();
                        SnakeAnimator.SetBool("LungingFast", true);
                    }
                    Invoke("SlowSnake", 3f);
                    Invoke("DisallowLunge", .25f);
                    Invoke("AllowLunge", 5f);
                }
                Vector3 targetPosition = player.transform.position;
                targetPosition.y = GetLockedWorldY();

                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, speed * Time.deltaTime);
                ApplyHeightLock();
                //GoblinAnim.SetBool("Attack", false);
                //GoblinAnim.SetBool("Walking", true);

                // Start the loop if we aren't already moving

            }
            else
            {

                if (SnakeAnimator.GetBool("Attacking") != true)
                {
                    SetAllBoolsFalse();
                    SnakeAnimator.SetBool("Attacking", true);
                }

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
    void SlowSnake()
    {
        if (SnakeAnimator.GetBool("SlitheringSlow") != true)
        {
            SetAllBoolsFalse();
            SnakeAnimator.SetBool("SlitheringSlow", true);
        }
    }

    public void SetAllBoolsFalse()
    {
        if (SnakeAnimator == null) return;

        // Loop through all parameters in the Animator
        foreach (AnimatorControllerParameter param in SnakeAnimator.parameters)
        {
            // Only set bool parameters
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                SnakeAnimator.SetBool(param.name, false);
            }
        }

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
        ApplyHeightLock();
    }

    void AllowEnemyMovement()
    {
       canMove = true;

    }

    private void CacheLockedHeight()
    {
        if (enemy == null)
        {
            return;
        }

        lockedWorldY = enemy.transform.position.y + lockedHeightOffset;
        hasLockedHeight = true;
    }

    private float GetLockedWorldY()
    {
        if (!lockEnemyHeight)
        {
            return player != null ? player.transform.position.y : transform.position.y;
        }

        if (!hasLockedHeight)
        {
            CacheLockedHeight();
        }

        return hasLockedHeight ? lockedWorldY : transform.position.y;
    }

    private void ApplyHeightLock()
    {
        if (!lockEnemyHeight || enemy == null)
        {
            return;
        }

        Vector3 lockedPosition = enemy.transform.position;
        lockedPosition.y = GetLockedWorldY();
        enemy.transform.position = lockedPosition;
    }
}
