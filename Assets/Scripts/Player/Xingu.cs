using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Xingu : MonoBehaviour
{
    public CharacterController characterController;
    private Vector3 moveDirection;
    public Transform cameraTransform;
    public Animator animator;
    public float health;
    public float healthRegenRate = 20f;
    private float timeSinceLastDamage;
    public Slider healthBar;
    public float stamina;
    public float staminaRegenRate = 10f;
    private float lastRunTime;
    public Slider staminaBar;
    public float moveSpeed = 6f;
    private float gravity = 20f;
    private float jumpForce = 8f;
    public int jumpCount = 0;
    public int maxJumpCount = 2;
    
    public Transform groundChecker; // Referência ao objeto groundChecker
    public float groundCheckRadius = 0.2f;

    public float rotationSpeed = 10f; // Ajuste a velocidade de rotação conforme necessário

    public bool isGrounded = false; // verifica se está no chao
    public bool isCrouch = false; // indica se esta agaixado ou não
    public float timeOnGround = 0f;
    public float timeOnFall = 0f;

    // varaveis de ataque
    public int nooOfClicks = 0;
    float lastClickedTime = 0;
    public float maxComboDelay = 0.9f;    
    public bool isAttacking = false;
    private float horizontalInput = 0f; // travar a movimentação HORIZONTAL se estiver atacando
    private float verticalInput = 0f; // travar a movimentação VERTICAL se estiver atacando
    
    public LayerMask enemyLayers;
    public GameObject hitBoxDamage;
    public GameObject hitBigBoxDamage;
    public float attackDamage = 20;

    //variaveis ataque forte
    private float pressStartTime;
    private float pressDuration;

    // Variaveis de arco e flecha
    public bool archerMode = false; // variavel que muda da lança para o arco
    public GameObject lance;
    public GameObject bow;

    //variaveis do arco parte 2
    public GameObject arrowPrefab;  // Prefab da flecha
    public float shootForce = 30f;  // Força do disparo
    public float shootDelay = 1.0f;  // Atraso entre os disparos
    private bool canShoot = true;  // Variável para controlar o atraso entre disparos
    private Camera mainCamera; // Referência para a câmera principal

    public Transform arrowSpawnPoint;
    private Quaternion cameraRotation; // Variável para armazenar a rotação da câmera
    public GameObject mira;

    public float RangeArrow;

    //RETURN DO TIRO
    public void returnShoot()
    {
        isAttacking = false;
        animator.SetBool("shoot", false);
    }


    [SerializeField] private AudioSource flechaAudioSource;
    [SerializeField] private AudioClip flechaAudioClip;

    private void somShot() { flechaAudioSource.PlayOneShot(flechaAudioClip); }

    // SURGIR O TIRO
    public void shoot()
    {
        // Obtenha a direção da câmera
        Vector3 cameraForward = cameraTransform.forward;

        somShot();
        // Instancie uma nova flecha a partir do prefab
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

        // Defina a rotação da flecha para corresponder à direção da câmera
        arrow.transform.forward = cameraForward;

        //gambiarra para a flecha nao ser jogada de pé nao consegui arrumar de um jeito melhor
        //Porém com certeza existe um jeito melhor
        arrow.transform.rotation = Quaternion.Euler(-90, arrowSpawnPoint.rotation.eulerAngles.y, 0);

        // Obtenha o Rigidbody da flecha
        Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();

        // Aplique uma força ao Rigidbody para disparar a flecha na direção da câmera
        arrowRigidbody.AddForce(cameraForward * shootForce, ForceMode.Impulse);

        // Defina canShoot para false para criar um atraso entre os disparos
        canShoot = false;

        // Inicie uma rotina para redefinir canShoot após o atraso especificado
        StartCoroutine(ResetShootDelay());
    }


    IEnumerator ResetShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public DialogueSystem dialogueSystem;//chamar sistema de dialogo
    
    // dava pra fazer um array... Claro que dava, mas foda-se o codigo e meu eu faço oq quiser
    public GameObject npc;
    public GameObject npc2;
    public GameObject npc3;
    public GameObject npc4;
    public GameObject npc5;
    public GameObject saci;
    public bool talk;    

    public LoadingScreenBarSystem loadingScreen;
    public void TakeDamage(float damage)
    {
        health -= damage;
        timeSinceLastDamage = Time.time;

        if(health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            loadingScreen.loadingScreen(3); // colocar 3 quando tiver tela de game over         
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHit"))
        {
            EnemyMelee enemyScript = other.GetComponentInParent<EnemyMelee>();

            float damage = enemyScript.attackDamage;
            TakeDamage(damage);
        }
        if (other.CompareTag("EnemyProjectile"))
        {
            // Obtenha o componente EnemyProjectile do tiro inimigo
            EnemyProjectile enemyProjectile = other.GetComponent<EnemyProjectile>();
            
            // Chame a função TakeDamage do jogador com base no dano do projétil inimigo
            TakeDamage(enemyProjectile.projectileDamage);

            // Destrua o tiro inimigo após atingir o jogador
            Destroy(other.gameObject);
        }
    }

    [SerializeField] private AudioSource passosAudioSource;
    [SerializeField] private AudioClip[] passosAudioClip;
    private void SomPassos()
     {
        passosAudioSource.PlayOneShot(passosAudioClip[Random.Range(0, passosAudioClip.Length)]);
    }

    [SerializeField] private AudioSource passosCorrendoAudioSource;
    [SerializeField] private AudioClip[] passosCorrendoAudioClip;
    private void SomPassosCorrendo()
    {
        passosCorrendoAudioSource.PlayOneShot(passosCorrendoAudioClip[Random.Range(0, passosCorrendoAudioClip.Length)]);
    }

    [SerializeField] private AudioSource pousoAudioSource;
    [SerializeField] private AudioClip[] pouseAudioClip;
    private void somPouso()
    {
        pousoAudioSource.PlayOneShot(pouseAudioClip[Random.Range(0, pouseAudioClip.Length)]);
    }


    private void Update()
    {
        //atualizar vida
        healthBar.value = health;

        if (Time.time - timeSinceLastDamage > 10f && health < 100f)
        {
            // Regenerate health over time
            health = Mathf.Clamp(health + healthRegenRate * Time.deltaTime, 0f, 100f);
        }

        //atualizar estamina
        staminaBar.value = stamina;
        // Recarrega a stamina após um período sem correr
        if (Time.time - lastRunTime > 4f && stamina < 100)
        {
            stamina = Mathf.Clamp(stamina + staminaRegenRate * Time.deltaTime, 0f, 100f);
        }

        //CHAMAR DIALOGO 1
        if (Vector3.Distance(transform.position, npc.transform.position) < 5f)
        {
            // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
            dialogueSystem.SetDialogueData(npc.GetComponent<NPC>().dialogueData);

            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(npc.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //CHAMAR DIALOGO 2
        if (Vector3.Distance(transform.position, npc2.transform.position) < 5f)
        {
            dialogueSystem.SetDialogueData(npc2.GetComponent<NPC>().dialogueData);
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(npc2.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //CHAMAR DIALOGO 3
        if (Vector3.Distance(transform.position, npc3.transform.position) < 5f)
        {
            // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
            dialogueSystem.SetDialogueData(npc3.GetComponent<NPC>().dialogueData);

            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(npc3.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //CHAMAR DIALOGO 4
        if (Vector3.Distance(transform.position, npc4.transform.position) < 5f)
        {
            dialogueSystem.SetDialogueData(npc4.GetComponent<NPC>().dialogueData);
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(npc4.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //CHAMAR DIALOGO 5
        if (Vector3.Distance(transform.position, npc5.transform.position) < 5f)
        {
            dialogueSystem.SetDialogueData(npc5.GetComponent<NPC>().dialogueData);
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(npc5.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //CHAMAR DIALOGO Saci
        if (Vector3.Distance(transform.position, saci.transform.position) < 5f)
        {
            dialogueSystem.SetDialogueData(saci.GetComponent<NPC>().dialogueData);
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.finished && talk)
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                dialogueSystem.End();
            }

            if (Input.GetKeyDown(KeyCode.E) && !dialogueSystem.finished && !talk)
            {
                // Acesse o dialogueData do NPC e atribua-o ao dialogueSystem
                dialogueSystem.SetDialogueData(saci.GetComponent<NPC>().dialogueData);
                dialogueSystem.Next();
            }
        }

        //      -----------------------     SISTEMA DE ARQUEIRO     -----------------------
        if (arrowSpawnPoint != null && mainCamera != null)
        {
            // Obtenha a rotação da câmera
            cameraRotation = mainCamera.transform.rotation;

            // Ajuste a rotação do arrowSpawnPoint para a rotação da câmera
            arrowSpawnPoint.rotation = cameraRotation;
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking 
            && !animator.GetBool("shoot") && !animator.GetBool("1") && !animator.GetBool("2") && !animator.GetBool("3") 
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("heavyAttack"))
        {
            // Inverta a ativação do Mesh Renderer
            archerMode = !archerMode;
            lance.SetActive(!lance.activeSelf);
            bow.SetActive(!bow.activeSelf);
            mira.SetActive(!mira.activeSelf);
        }


        if (archerMode && Input.GetButtonDown("Fire1") && isGrounded && !isCrouch && canShoot)
        {
            isAttacking = true;
            animator.SetBool("shoot", true);
        }

        if (archerMode && isAttacking) // Verifica se o archerMode está ativado e se a câmera principal está definida
        {
            // Obtenha a rotação da câmera
            Quaternion cameraRotation = mainCamera.transform.rotation;

            // Ajuste a rotação do personagem para a rotação da câmera, apenas na direção horizontal (y)
            Quaternion targetRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);

            // Interpole suavemente a rotação atual para a nova rotação
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);           

        }


        //      -----------------------     SISTEMA DE ATAQUES     -----------------------
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            nooOfClicks = 0;            
        }

        if (Input.GetButtonDown("Fire1") && isGrounded && !isCrouch && !archerMode)
        {
            lastClickedTime = Time.time;
            nooOfClicks++;
            hitBoxDamage.SetActive(true);
            if (nooOfClicks == 1)
            {
                isAttacking = true;
                animator.SetBool("1", true);
                animator.SetBool("walk", false);
                animator.SetBool("run", false);      
                
            }
            nooOfClicks = Mathf.Clamp(nooOfClicks, 0, 3);
        }

        if (isAttacking)
        {
            // Defina a entrada horizontal e vertical como zero para interromper o movimento durante o ataque
            horizontalInput = 0f;
            verticalInput = 0f;
        }
        else
        {
            // Obtenha as entradas horizontal e vertical normalmente
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        //ATAQUE FORTE
        if (Input.GetButton("Fire1") && !archerMode)
        {
            // Se não estiver atacando, registra o tempo inicial do pressionamento
            if (!isAttacking)
            {
                pressStartTime = Time.time;
                isAttacking = true; // Marca que estamos atacando
            }
            // Calcula a duração do pressionamento
            pressDuration = Time.time - pressStartTime;

            // Verifica se a duração do pressionamento é maior que 3 segundos
            if (pressDuration > 1.2f)
            {
                isAttacking = true;
                archerMode = false;
                // Ativa o ataque "4"                
                animator.SetBool("4", true);
                animator.SetBool("walk", false);
                animator.SetBool("run", false);
            }            
        }
        else
        {
            // Se o botão foi solto, reinicia as variáveis
            isAttacking = false;
            animator.SetBool("4", false);
        }

        /* Antigo ataque Forte
        if (Input.GetButton("Fire1") && isGrounded && !archerMode)
        {
            holdClick++; // ver como contar por segundo dentro do jogo
        }
        if (Input.GetButtonUp("Fire1") && isGrounded)
        {
            holdClick = 0;
        }

        if (holdClick > 100)
        {
            isAttacking = true;
            animator.SetBool("4", true);
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
        }*/

        //ATAQUE NO AR
        if (Input.GetButtonDown("Fire1") && !isGrounded && timeOnFall > .55f && !archerMode)
        {
            isAttacking = true;
            animator.SetBool("fallingAttack", true);
        }

        if (isAttacking && !isGrounded)
        {
            gravity = 30f;
        }
        //      -----------------------    FIM SISTEMA DE ATAQUES     -----------------------

        //      -----------------------     MOVIMENTAÇÃO     -----------------------
        // Verifique se o personagem está no chão usando Physics.CheckSphere
        int groundLayerMask = LayerMask.GetMask("Ground", "Default");
        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckRadius, groundLayerMask);
        //isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckRadius, LayerMask.GetMask("Ground"));


        if (isCrouch)
        {
            animator.SetBool("crouch", true);
            characterController.height = 1.25f;
            characterController.center = new Vector3(0, -0.41f, 0);
        }
        else
        {
            animator.SetBool("crouch", false);
            characterController.height = 2f;
            characterController.center = new Vector3(0, 0f, 0);
        }


        // Verifique se o personagem está no chão
        if (characterController.isGrounded)
        {            

            // Resetar o contador de pulos quando estiver no chão            
            jumpCount = 0; jumpForce = 8f;

            // Obtém a direção da câmera em relação ao mundo
            Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraRight = cameraTransform.right;

            // Use horizontalInput e verticalInput para controlar o movimento
            // Calcula a direção de movimento com base na entrada do jogador e a direção da câmera            
            moveDirection = horizontalInput * cameraRight + verticalInput * cameraForward;
            moveDirection = moveDirection.normalized * moveSpeed;



            if (moveDirection != Vector3.zero)
            {               
                // Calcula a rotação desejada com base na direção do movimento
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Interpola suavemente a rotação atual para a nova rotação
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Ativar a animação de caminhar
                animator.SetBool("walk", true);
                // se ele estiver andando e apertar o Shift
                if (Input.GetKey(KeyCode.LeftShift) && !isCrouch && stamina > 0)
                {
                    stamina -= Time.deltaTime * 10f;
                    moveSpeed = 10f;
                    animator.SetBool("run", true);
                    lastRunTime = Time.time;
                }
                else
                {
                    moveSpeed = 4f;
                    animator.SetBool("run", false);                    
                }
            }
            else
            {
                // Desativar a animação de caminhar quando o personagem não está se movendo
                animator.SetBool("run", false);
                animator.SetBool("walk", false);
            }

        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (isGrounded)
        {
            // se apertar o C alterar n o valor da variavel de agaixado
            if (Input.GetKeyDown(KeyCode.C)) //Input.GetKeyDown(KeyCode.LeftControl)
            {
                isCrouch = !isCrouch;
            }

            // esse if e para resolver o problema q qnd ele descia rampas ou escadas ele ia virando travando ao inves de virar de fato
            if (Input.GetAxis("Vertical") < 0 && !isAttacking) // o !isAttacking esta resolvendo o bug dele olhar para frente enquanto ataca apertando S
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

                // Mantenha os ângulos de rotação nos outros eixos
                float currentX = transform.rotation.eulerAngles.x;
                float currentZ = transform.rotation.eulerAngles.z;
                targetRotation = Quaternion.Euler(currentX, targetRotation.eulerAngles.y, currentZ);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }


            timeOnFall = 0f;
            // Contador que é usado para fazer o pouso
            timeOnGround += Time.deltaTime;
            animator.SetBool("falling", false);
            animator.SetBool("jumping", false);

            if (timeOnGround < .2f)
            {
                // Faça a transição para a animação de "landing"
                animator.SetBool("landing", true);
            }
            else
            {
                animator.SetBool("landing", false);
            }

        }
        else
        {
            timeOnFall += Time.deltaTime;
            timeOnGround = 0f; // Resetar o contador quando não estiver no chão

            animator.SetBool("falling", true);
            isCrouch= false;

            if (timeOnFall < .1f)
            {
                // Faça a transição para a animação de "jumping"
                animator.SetBool("jumping", true);
            }
            else
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }

        }

        // Permitir o pulo duplo
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount && !isAttacking && !isCrouch)
        {
            timeOnFall = 0f;            
            moveDirection.y = jumpForce;
            jumpForce = 11f;
            jumpCount++;

        }

        // Movimentar o personagem
        characterController.Move(moveDirection * Time.deltaTime);
        //      -----------------------    FIM MOVIMENTAÇÃO     -----------------------        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Trava o cursor no meio da tela OBS: para destravar troque para .None;
        Cursor.visible = false;  // Torna o cursor invisível OBS: para deixar visivel troque por true
        mainCamera = Camera.main;
    }


    // PARTE 1 DO COMBO EM PE
    public void return1()
    {
        hitBoxDamage.SetActive(true);
        if (nooOfClicks >= 2)
        {
            isAttacking = true;
            animator.SetBool("2", true);
        }
        else
        {
            hitBoxDamage.SetActive(false);
            isAttacking = false;
            animator.SetBool("1", false);
            nooOfClicks = 0;
        }
    }
    // PARTE 2 DO COMBO EM PE
    public void return2()
    {
        hitBoxDamage.SetActive(true);
        if (nooOfClicks >= 3)
        {
            isAttacking = true;
            animator.SetBool("3", true);
        }
        else
        {
            hitBoxDamage.SetActive(false);
            isAttacking = false;
            animator.SetBool("2", false);
            animator.SetBool("1", false);
            nooOfClicks = 0;
        }
    }
    // PARTE 3 DO COMBO EM PE
    public void return3()
    {
        hitBoxDamage.SetActive(false);
        isAttacking = false;
        animator.SetBool("1", false);
        animator.SetBool("2", false);
        animator.SetBool("3", false);
        nooOfClicks = 0;
    }
    // ATAQUE FORTE
    [SerializeField] private AudioClip bigAttackAudioClip;

    public void createBigAttack()
    {
        flechaAudioSource.PlayOneShot(bigAttackAudioClip);
        hitBigBoxDamage.SetActive(true);
        //createBigAttack
    }

    [SerializeField] private AudioClip att1AudioClip;
    [SerializeField] private AudioClip att2AudioClip;
    [SerializeField] private AudioClip att3AudioClip;

    private void somAtt1() { flechaAudioSource.PlayOneShot(att1AudioClip); }
    private void somAtt2() { flechaAudioSource.PlayOneShot(att2AudioClip); }
    private void somAtt3() { flechaAudioSource.PlayOneShot(att3AudioClip); }

    public void return4()
    {
        hitBigBoxDamage.SetActive(false);
        isAttacking = false;
        animator.SetBool("4", false);
    }
    // ATAQUE NO AR
    public void return5()
    {
        isAttacking = false;
        gravity = 20f;
        animator.SetBool("fallingAttack", false);
    }
}

