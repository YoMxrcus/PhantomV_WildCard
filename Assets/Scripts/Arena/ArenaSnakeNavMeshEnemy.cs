using UnityEngine;
using UnityEngine.AI;

public class ArenaSnakeNavMeshEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float retargetInterval = 0.2f;

    private NavMeshAgent agent;
    private Animator animator;
    private ThirdPersonController player;
    private float nextAttackTime;
    private float nextRetargetTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        animator = GetComponentInChildren<Animator>();
        ConfigureAgent();
        DisableLegacySnakeMovement();
    }

    private void Start()
    {
        ResolvePlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            ResolvePlayer();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            return;
        }

        ChasePlayer();
    }

    private void ConfigureAgent()
    {
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = attackRange * 0.85f;
        agent.autoBraking = false;
    }

    private void DisableLegacySnakeMovement()
    {
        EnemyDetect[] legacyDetectors = GetComponentsInChildren<EnemyDetect>(true);

        foreach (EnemyDetect detector in legacyDetectors)
        {
            detector.enabled = false;
        }
    }

    private void ChasePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            FacePlayer();
            PlayAnimation("SlitheringSlow");
            return;
        }

        if (Time.time >= nextRetargetTime)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            nextRetargetTime = Time.time + retargetInterval;
        }

        PlayAnimation("SlitheringSlow");
    }

    private void AttackPlayer()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        FacePlayer();
        PlayAnimation("Attacking");

        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (player.health > 0 && !player.GodMode)
        {
            player.health -= attackDamage;
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
    }

    private void PlayAnimation(string boolName)
    {
        if (animator == null || string.IsNullOrEmpty(boolName))
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, parameter.name == boolName);
            }
        }
    }

    private void ResolvePlayer()
    {
        GameObject playerObject = GameObject.Find("ThirdPersonTEST");

        if (playerObject == null)
        {
            ThirdPersonController playerController = FindFirstObjectByType<ThirdPersonController>();
            playerObject = playerController != null ? playerController.gameObject : null;
        }

        player = playerObject != null ? playerObject.GetComponent<ThirdPersonController>() : null;
    }
}
