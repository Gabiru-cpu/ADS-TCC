using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float projectileDamage = 20f;
    public float maxTimer = 2f;
    private float currentTimer;

    private void Start()
    {
        // Inicializa o timer no valor máximo
        currentTimer = maxTimer;
    }

    private void Update()
    {
        if (currentTimer <= 0f) Destroy(gameObject);

        currentTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se colidiu com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtém o componente do jogador e aplica dano
            Xingu playerScript = collision.gameObject.GetComponentInParent<Xingu>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(projectileDamage);
            }

            // Destroi o projetil
            Destroy(gameObject);
        }        
    }
}
