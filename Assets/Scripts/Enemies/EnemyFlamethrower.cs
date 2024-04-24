using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlamethrower : MonoBehaviour
{
    public float projectileDamage = 20f;
    public float maxTimer = 2f;
    public float currentTimer;

    private void Start()
    {
        // Inicializa o timer no valor máximo
        currentTimer = maxTimer;
    }

    private void OnTriggerStay(Collider other)
    {
        // Verifica se o objeto dentro do trigger é o jogador
        if (other.CompareTag("Player"))
        {
            // Reduz o timer a cada quadro
            currentTimer -= Time.deltaTime;

            // Verifica se o timer atingiu zero ou menos
            if (currentTimer <= 0f) Destroy(gameObject);
            

            // Obtém o componente do jogador e aplica dano
            Xingu playerScript = other.GetComponentInParent<Xingu>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(projectileDamage);
            }
                        
        }
    }
}
