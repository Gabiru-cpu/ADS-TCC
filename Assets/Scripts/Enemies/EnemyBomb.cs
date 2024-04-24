using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyBomb : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public Slider healthBar;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeToAttack = 5.0f;
    public float attackTimer = 0.0f;
    bool alreadyAttacked;
    public float attackDamage = 20;
    public GameObject damageArea;
    public GameObject explosionEffect;
    //public GameObject model;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("ThirdPersonPlayer").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            attackTimer = 0.0f;  // Resetar o temporizador quando fora de alcance
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            attackTimer = 0.0f;  // Resetar o temporizador quando fora de alcance
        }
        
        if (playerInAttackRange && playerInSightRange)
        {
            // Se estiver no alcance de ataque, comece a contar até o tempo de ataque
            attackTimer += Time.deltaTime;

            // Se o temporizador atingir o tempo de ataque, ataque o jogador
            if (attackTimer >= timeToAttack)
            {                
                AttackPlayer();
                if (attackTimer >= timeToAttack + .3) Invoke(nameof(DestroyEnemy), 0.5f);
            }
        }

        healthBar.value = health;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);        
    }

    [SerializeField] private AudioSource BombAudioSource;
    [SerializeField] private AudioClip ExplodiuAudioClip;

    private void AttackPlayer()
    {        
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        // Save the current rotation along the X axis
        float originalRotationX = transform.rotation.eulerAngles.x;

        // Make the enemy melee look at the player's direction without affecting the X-axis rotation
        transform.LookAt(player);

        // Restore the original rotation along the X axis
        Quaternion originalRotation = transform.rotation;
        originalRotation.eulerAngles = new Vector3(originalRotationX, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
        transform.rotation = originalRotation;

        if (!alreadyAttacked)
        {
            damageArea.SetActive(true);
            explosionEffect.SetActive(true);
            BombAudioSource.PlayOneShot(ExplodiuAudioClip);
            //model.SetActive(false);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeToAttack);  // Corrigido aqui
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("PlayerHit"))
        {
            Xingu playerScript = other.GetComponentInParent<Xingu>();

            float damage = playerScript.attackDamage;
            TakeDamage(damage);            
        }
        else if (other.CompareTag("Bullet"))
        {
            ProjectileHit(other.GetComponent<Projectile>());
        }
    }

    private void ProjectileHit(Projectile projectile)
    {
        // Assuming Projectile script has a damage value
        float damage = projectile.damage;

        TakeDamage(damage);

        // Destroy the projectile
        Destroy(projectile.gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
