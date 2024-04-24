using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour
{
    public GameObject enterArenaTrigger;  // Cubo de colis�o para entrar na arena
    public GameObject lockArenaObject;    // Barreira para impedir a sa�da da arena
    public GameObject curvedRoof;         // Objeto que deve subir e girar

    public float roofMoveSpeed = 1.0f;
    public float roofRotateSpeed = 30.0f;
    public float maxHeight = 5.0f; // Altura m�xima desejada
    public float minHeight = 0.0f; // Altura m�nima desejada

    private bool arenaEntered = false;
    private bool isMovingUp = true;
    private float initialYPosition;

    void Start()
    {
        // Salva a posi��o inicial em Y do objeto
        initialYPosition = curvedRoof.transform.position.y;
    }

    void Update()
    {
        if (!arenaEntered)
        {
            // Verifique se o jogador entrou na arena
            CheckArenaEntry();
        }
        else
        {
            // Ative a barreira
            lockArenaObject.SetActive(true);

            // Fa�a o objeto CurvedRoof girar e mover verticalmente
            MoveAndRotateCurvedRoof();
        }
    }

    [SerializeField] private AudioSource MainCameraAudioSource;

    [SerializeField] private AudioClip MusicaAmbienteAudioClip;
    [SerializeField] private AudioClip MusicaBossAudioClip;

    void CheckArenaEntry()
    {
        // Use a tag "Player" ou ajuste conforme necess�rio
        Collider[] colliders = Physics.OverlapBox(enterArenaTrigger.transform.position, enterArenaTrigger.transform.localScale / 2, Quaternion.identity);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // O jogador entrou na arena
                arenaEntered = true;
                MainCameraAudioSource.clip = MusicaBossAudioClip;
                MainCameraAudioSource.Play();
                break;
            }
        }
    }

    void MoveAndRotateCurvedRoof()
    {
        // Movimento vertical
        float newY = curvedRoof.transform.position.y;

        if (isMovingUp)
        {
            newY = Mathf.Min(newY + roofMoveSpeed * Time.deltaTime, initialYPosition + maxHeight);
            if (newY >= initialYPosition + maxHeight)
                isMovingUp = false;
        }
        else
        {
            newY = Mathf.Max(newY - roofMoveSpeed * Time.deltaTime, initialYPosition + minHeight);
            if (newY <= initialYPosition + minHeight)
                isMovingUp = true;
        }

        // Aplica a movimenta��o vertical
        curvedRoof.transform.position = new Vector3(curvedRoof.transform.position.x, newY, curvedRoof.transform.position.z);

        // Rota��o
        curvedRoof.transform.Rotate(Vector3.up, roofRotateSpeed * Time.deltaTime);
    }
}
