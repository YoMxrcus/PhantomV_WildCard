using UnityEngine;
using UnityEngine.AI;

public class ArenaZombieEnemy : MonoBehaviour
{
    public enum ZombieKind
    {
        Walker,
        Runner
    }

    [Header("Zombie Type")]
    [SerializeField] private ZombieKind kind = ZombieKind.Walker;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 80;
    [SerializeField] private float walkerSpeed = 2.2f;
    [SerializeField] private float runnerSpeed = 4.4f;
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private int attackDamage = 7;
    [SerializeField] private float attackCooldown = 1.15f;
    [SerializeField] private float retargetInterval = 0.2f;
    [SerializeField] private float destroyAfterDeathSeconds = 1.8f;

    [Header("Animation State Names")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string walkState = "Walk";
    [SerializeField] private string runState = "Run";
    [SerializeField] private string attackState = "Attack";
    [SerializeField] private string hitState = "Hit";
    [SerializeField] private string dyingState = "Dying";

    private NavMeshAgent agent;
    private Animator animator;
    private ThirdPersonController player;
    private int currentHealth;
    private float nextAttackTime;
    private float nextRetargetTime;
    private bool isDead;
    private string currentAnimationState;

    public ZombieKind Kind => kind;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        currentHealth = maxHealth;
        ConfigureAgent();
    }

    private void Start()
    {
        ResolvePlayer();
        PlayState(idleState, 0.05f);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (player == null)
        {
            ResolvePlayer();
            PlayState(idleState, 0.1f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }

        if (other.CompareTag("projectile"))
        {
            int damage = player != null ? player.currentDamage : 20;
            ApplyDamage(damage);
            Destroy(other.gameObject);
        }
    }

    public void ApplyDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        PlayState(hitState, 0.03f);
    }

    public void SetKind(ZombieKind newKind)
    {
        kind = newKind;
        ConfigureAgent();
    }

    private void ChasePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            FacePlayer();
            PlayMovementAnimation();
            return;
        }

        if (Time.time >= nextRetargetTime)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            nextRetargetTime = Time.time + retargetInterval;
        }

        PlayMovementAnimation();
    }

    private void AttackPlayer()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        FacePlayer();
        PlayState(attackState, 0.05f);

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

    private void Die()
    {
        isDead = true;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider zombieCollider in colliders)
        {
            zombieCollider.enabled = false;
        }

        PlayState(dyingState, 0.05f);
        Destroy(gameObject, destroyAfterDeathSeconds);
    }

    private void ConfigureAgent()
    {
        if (agent == null)
        {
            return;
        }

        agent.speed = kind == ZombieKind.Runner ? runnerSpeed : walkerSpeed;
        agent.acceleration = kind == ZombieKind.Runner ? 18f : 10f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = attackRange * 0.8f;
        agent.radius = 0.02f;
        agent.height = 0.08f;
        agent.autoBraking = false;
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

    private void PlayMovementAnimation()
    {
        if (kind == ZombieKind.Runner)
        {
            PlayState(runState, 0.1f);
        }
        else
        {
            PlayState(walkState, 0.1f);
        }
    }

    private void PlayState(string stateName, float transitionDuration)
    {
        if (animator == null || string.IsNullOrEmpty(stateName) || currentAnimationState == stateName)
        {
            return;
        }

        animator.CrossFade(stateName, transitionDuration);
        currentAnimationState = stateName;
    }
}
