using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator animator;

    public float health;
    public Slider healthBar;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float attackDamage = 20;
    public GameObject damageArea;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("ThirdPersonPlayer").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Set animator parameters based on the current state
        animator.SetBool("patrol", !playerInSightRange && !playerInAttackRange);
        animator.SetBool("chase", playerInSightRange && !playerInAttackRange);
        animator.SetBool("attack", playerInAttackRange && playerInSightRange);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();


        healthBar.value = health;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
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

    private void AttackPlayer()
{
    // Make sure enemy doesn't move
    agent.SetDestination(transform.position);

    // Salve a rotação atual ao longo do eixo X
    float originalRotationX = transform.rotation.eulerAngles.x;

    // Faça com que o inimigo melee olhe para a direção do jogador sem mexer na rotação do eixo X
    transform.LookAt(player);

    // Restaure a rotação original ao longo do eixo X
    Quaternion originalRotation = transform.rotation;
    originalRotation.eulerAngles = new Vector3(originalRotationX, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
    transform.rotation = originalRotation;

    if (!alreadyAttacked)
    {            
        

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
}
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip attackAudioClip;
        private void ActiveHitbox()
    {
        attackAudioSource.PlayOneShot(attackAudioClip);
        damageArea.SetActive(true);
    }

    private void DeactivatedHitbox()
    {
        damageArea.SetActive(false);
    }

    [SerializeField] private AudioSource passosCorrendoAudioSource;
    [SerializeField] private AudioClip passosCorrendoAudioClip;    
    private void MinionPassoCorrendo()
    {
        passosCorrendoAudioSource.PlayOneShot(passosCorrendoAudioClip);
    }

    [SerializeField] private AudioClip gritoCorrendoAudioClip;
    private void MinionGritoCorrendo()
    {
        passosCorrendoAudioSource.PlayOneShot(gritoCorrendoAudioClip);
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
