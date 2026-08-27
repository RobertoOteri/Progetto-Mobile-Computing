using UnityEngine;

public class BossController : MonoBehaviour
{
    // Stati logici del Boss
    public enum BossState { Spawning, Idle, Chasing, Running, Attacking, Hurt, Dead }
    public BossState currentState = BossState.Spawning;

    [Header("Target e Movimento")]
    public Transform player;
    public float attackRange = 2f;
    public float aggroRange = 10f;
    public float walkSpeed = 2f;
    public float runSpeed = 4.5f;

    [Header("Fasi Boss")]
    public bool isAngry = false; // Se true, userà "idle angry" e correrà invece di camminare

    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 facingDirection = Vector2.down; // Direzione in cui guarda (di default in basso)
    
    // Variabile per tenere traccia dell'animazione attualmente in riproduzione
    private string currentAnimName = "";

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if(p != null) player = p.transform;
        }

        // Iniziamo con l'animazione di spawn
        ChangeState(BossState.Spawning);
        // Simuliamo la fine dello spawn dopo 2 secondi (modifica in base alla durata reale dell'animazione)
        Invoke("FinishSpawn", 2f); 
    }

    void Update()
    {
        if (currentState == BossState.Dead || currentState == BossState.Spawning || currentState == BossState.Hurt) 
            return;

        // Calcola la direzione in cui si trova il player per sapere dove guardare
        if (player != null)
        {
            facingDirection = (player.position - transform.position).normalized;
        }
    }

    void FixedUpdate()
    {
        if (currentState == BossState.Dead || currentState == BossState.Spawning || 
            currentState == BossState.Attacking || currentState == BossState.Hurt) 
        {
            UpdateAnimation(); // Aggiorna solo l'aspetto visivo
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            ChangeState(BossState.Attacking);
            Attack();
        }
        else if (distance < aggroRange)
        {
            // Se il boss è arrabbiato corre, altrimenti cammina
            ChangeState(isAngry ? BossState.Running : BossState.Chasing);
            
            float currentSpeed = isAngry ? runSpeed : walkSpeed;
            Vector2 newPosition = rb.position + facingDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            ChangeState(BossState.Idle);
        }

        UpdateAnimation();
    }

    // --- GESTORE DELLE ANIMAZIONI ---
    // Questa funzione decide quale stringa chiamare in base allo stato e alla direzione
    void UpdateAnimation()
    {
        string animToPlay = "";

        // Condizioni base (non dipendono dalla direzione)
        if (currentState == BossState.Dead) { animToPlay = "DEATH"; }
        else if (currentState == BossState.Hurt) { animToPlay = "hurt"; }
        else if (currentState == BossState.Spawning) { animToPlay = "spawn"; }
        else
        {
            // Determiniamo la direzione principale
            // Usiamo 0.1f e -0.1f come soglie per capire se si sta muovendo prevalentemente su/giù o destra/sinistra
            bool isFacingBack = facingDirection.y > 0.1f; // Sta guardando in SU
            bool isFacingInverted = facingDirection.x < -0.1f; // Sta guardando a SINISTRA

            switch (currentState)
            {
                case BossState.Idle:
                    animToPlay = isAngry ? "idle angry" : "idle";
                    break;

                case BossState.Chasing: // Camminata
                    if (isFacingBack && isFacingInverted) animToPlay = "walk back inv";
                    else if (isFacingBack) animToPlay = "walk back";
                    else if (isFacingInverted) animToPlay = "walk inverted";
                    else animToPlay = "walk";
                    break;

                case BossState.Running: // Corsa
                    if (isFacingBack && isFacingInverted) animToPlay = "run back inverted";
                    else if (isFacingBack) animToPlay = "run back";
                    else if (isFacingInverted) animToPlay = "run inverted";
                    else animToPlay = "run";
                    break;

                case BossState.Attacking:
                    // Nell'immagine non c'è "attack back inv", quindi se guarda su e sinistra usiamo "attack back"
                    if (isFacingBack) animToPlay = "attack back";
                    else if (isFacingInverted) animToPlay = "attack inv";
                    else animToPlay = "attack";
                    break;
            }
        }

        // Riproduce l'animazione solo se non è già in riproduzione
        PlayAnimation(animToPlay);
    }

    // Metodo fondamentale per non resettare l'animazione ad ogni frame
    void PlayAnimation(string newAnimName)
    {
        if (currentAnimName == newAnimName) return; // Se sta già riproducendo questa animazione, fermati

        anim.Play(newAnimName);
        currentAnimName = newAnimName;
    }

    // --- FUNZIONI DI SERVIZIO ---
    
    void ChangeState(BossState newState)
    {
        currentState = newState;
    }

    void FinishSpawn()
    {
        ChangeState(BossState.Idle);
    }

    void Attack()
    {
        // Ferma il movimento durante l'attacco
        rb.linearVelocity = Vector2.zero; 
        
        // Aspetta la fine dell'attacco prima di tornare a muoversi
        // Modifica 1.5f con la durata reale della tua animazione di attacco
        Invoke("ResetAfterAction", 1.5f); 
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead || currentState == BossState.Spawning) return;

        // Logica vita... (es. health -= damage)
        // Se health < 50%, puoi settare: isAngry = true;

        ChangeState(BossState.Hurt);
        CancelInvoke("ResetAfterAction"); // Ferma eventuali timer di attacco
        Invoke("ResetAfterAction", 0.5f); // Modifica 0.5f con la durata dell'animazione "hurt"
    }

    void ResetAfterAction()
    {
        if (currentState != BossState.Dead)
        {
            ChangeState(BossState.Idle);
        }
    }
}