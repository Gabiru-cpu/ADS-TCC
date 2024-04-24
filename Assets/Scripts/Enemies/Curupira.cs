using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DigitalRuby.PyroParticles;

public class Curupira : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;
    public Animator animator;

    public float health;
    public Slider healthBar;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject lancePrefab; // Lance objeto    
    public Transform attackPoint;  // Ponto de origem do ataque

    //skills
    public GameObject[] skills;

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

        animator.SetBool("idle", !playerInSightRange && !playerInAttackRange);
        animator.SetBool("chase", playerInSightRange && !playerInAttackRange);
        animator.SetBool("prepareAttack", playerInAttackRange && playerInSightRange);

        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        healthBar.value = health;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        attackPoint.transform.LookAt(player);

        // Salve a rotação atual ao longo do eixo X
        float originalRotationX = transform.rotation.eulerAngles.x;
        // Faça com que o inimigo olhe para a direção do jogador sem mexer na rotação do eixo X
        transform.LookAt(player);

        // Restaure a rotação original ao longo do eixo X
        Quaternion originalRotation = transform.rotation;
        originalRotation.eulerAngles = new Vector3(originalRotationX, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
        transform.rotation = originalRotation;

        if (!alreadyAttacked)
        {
            // Randomly choose an attack
            int randomAttack = Random.Range(1, 4); // Generates a random number between 1 and 3

            switch (randomAttack)
            {
                case 1:
                    animator.SetBool("at1", true);
                    //LanceAttack();
                    break;
                case 2:
                    animator.SetBool("at2", true);
                    //FireAttack();
                    break;
                case 3:
                    animator.SetBool("at3", true);
                    //WaveAttack();
                    break;
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    [SerializeField] private AudioSource CurupiraAudioSource;
    [SerializeField] private AudioClip passosCurupiraAudioClip;
    private void CurupiraCorrendo()
    {
        CurupiraAudioSource.PlayOneShot(passosCurupiraAudioClip);
    }

    [SerializeField] private AudioClip tauntCurupiraAudioClip;
    private void CurupiraTaunt()
    {
        CurupiraAudioSource.PlayOneShot(tauntCurupiraAudioClip);
    }

    [SerializeField] private AudioClip fireballCurupiraAudioClip;
    private void LanceAttack()
    {
        // Attack code for lance
        CurupiraAudioSource.PlayOneShot(fireballCurupiraAudioClip);
        GameObject lanceObject = Instantiate(lancePrefab, attackPoint.transform.position, Quaternion.identity);

        // Ajuste a rotação da lança
        lanceObject.transform.rotation = Quaternion.LookRotation(attackPoint.transform.forward);

        // Obtenha o Rigidbody da lança
        Rigidbody rb = lanceObject.GetComponent<Rigidbody>();

        // Adicione forças ao Rigidbody
        rb.AddForce(attackPoint.transform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(attackPoint.transform.up * 3f, ForceMode.Impulse);
    }

    private void LanceAttackEnd()
    {
        animator.SetBool("at1", false);
    }

    private void FireAttack()
    {
        Debug.Log("Ataque do fogo");
        // Attack code for fire
        BeginEffect(0);
    }
    private void FireAttackEnd()
    {
        animator.SetBool("at2", false);
    }

    private void WaveAttack()
    {
        Debug.Log("Ataque da onda de fogo");
        // Attack code for wave
        BeginEffect(1);
    }

    private void WaveAttackEnd()
    {
        animator.SetBool("at3", false);
    }

    private void BeginEffect(int prefabIndex)
    {
        // Lógica para iniciar o efeito/skill aqui, usando o prefab com o índice fornecido
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;

        GameObject currentPrefabObject = GameObject.Instantiate(skills[prefabIndex]);
        FireBaseScript currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();

        if (currentPrefabScript == null)
        {
            currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
            if (currentPrefabScript.IsProjectile)
            {
                rotation = transform.rotation;
                pos = transform.position + forward + right + up;
            }
            else
            {
                pos = transform.position + (forwardY * 10.0f);
            }
        }
        else
        {
            pos = transform.position + (forwardY * 5.0f);
            rotation = transform.rotation;
            pos.y = 0.0f;
        }

        FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
        }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = rotation;
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
        float damage = projectile.damage;
        TakeDamage(damage);
        Destroy(projectile.gameObject);
    }
    public LoadingScreenBarSystem loadingScreen;
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            loadingScreen.loadingScreen(4); // colocar 4 quando tiver tela de fim de demo
            Invoke(nameof(DestroyBoss), 0.5f);
        }
    }

    private void DestroyBoss()
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
