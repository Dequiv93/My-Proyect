using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = -5f;
    public float coneOfVision = 120f;
    private NavMeshAgent agent;
    [SerializeField]private Animator animator;
    public int maxHealth = 100;
    private int currentHealth;
    private Rigidbody rb;
    private float timer;
    public float wanderRadius = 10;
    public float wanderTimer = 5;
    public bool isDead = false;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private float attackCooldown = 3f;
    private float lastAttackTime = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance< attackRange)
        {
            Attack();
        }

        else if (distance <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle <= coneOfVision / 2f)
            {
                agent.SetDestination(player.position);
                return;
            }
        }
        timer += Time.deltaTime;
        if (timer> wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
      
        }

    }
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return; // Exit the method if still in cooldown
        }

        Collider[] hitPlayer = Physics.OverlapSphere(attackPosition.position, attackRange);
        foreach (var player in hitPlayer)
        {
            CharacterCrontroller characterController = player.GetComponent<CharacterCrontroller>();
            if (characterController != null && !characterController.isDead) // Solo atacar si no está muerto.
            {
                characterController.TakeDamage(25);

            }
        }
        // Update the last attack time
        lastAttackTime = Time.time + attackCooldown;
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return; // No hacer nada si ya está muerto.
        currentHealth -= damage;
        Debug.Log(" Enemy took " + damage + " damage.");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true; // Marcar al enemigo como muerto.
            Died();
        }
    }

    void Died()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}
