using UnityEngine;

/// <summary>
/// Controller del boss 2D top-down: gestisce lo stato dell'IA (idle, inseguimento,
/// ritirata, attacco, danno, morte) e pilota l'Animator impostando dei parametri
/// (Enraged, Inverted, Moving, Running, Retreating, Spawn, Attack, Hurt, Death).
///
/// NB: i nomi dei parametri qui sotto sono un'ipotesi ragionevole basata sugli stati
/// visti nell'Animator (walk / walk inverted / walk back / walk back inv, run e varianti,
/// attack e varianti, idle "arrabbiato", spawn, hurt, death). Se nel tuo Animator i
/// parametri si chiamano diversamente, basta cambiare le stringhe negli StringToHash
/// qui sotto: il resto dello script non cambia.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    public enum BossState { Spawn, Idle, Chase, Retreat, Attack, Hurt, Dead }

    [Header("Riferimenti")]
    [Tooltip("Di solito il Transform del player")]
    [SerializeField] private Transform target;

    private Animator animator;
    private Rigidbody2D rb;

    [Header("Statistiche")]
    [SerializeField] private int maxHealth = 300;
    [SerializeField, Range(0f, 1f)] private float enrageThreshold = 0.5f;

    [Header("Movimento")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float retreatRange = 0.8f;

    [Header("Attacco")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackDuration = 0.8f;

    [Header("Timing di fallback (usati anche senza Animation Event collegati)")]
    [SerializeField] private float spawnDuration = 1f;
    [SerializeField] private float hurtDuration = 0.4f;

    [Header("Debug (sola lettura)")]
    [SerializeField] private BossState currentState = BossState.Spawn;

    private int currentHealth;
    private bool isEnraged;
    private bool isDead;
    private float attackTimer;
    private float stateTimer;
    private Vector2 moveDirection;
    private float currentSpeed;

    // Parametri Animator: usare gli hash è più efficiente di passare stringhe ogni frame
    private static readonly int ParamEnraged  = Animator.StringToHash("Enraged");
    private static readonly int ParamInverted = Animator.StringToHash("Inverted");
    private static readonly int ParamMoving   = Animator.StringToHash("Moving");
    private static readonly int ParamRunning  = Animator.StringToHash("Running");
    private static readonly int ParamRetreat  = Animator.StringToHash("Retreating");
    private static readonly int TrigSpawn     = Animator.StringToHash("Spawn");
    private static readonly int TrigAttack    = Animator.StringToHash("Attack");
    private static readonly int TrigHurt      = Animator.StringToHash("Hurt");
    private static readonly int TrigDeath     = Animator.StringToHash("Death");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        ChangeState(BossState.Spawn);
    }

    private void Update()
    {
        if (isDead) return;

        UpdateFacing();

        switch (currentState)
        {
            case BossState.Spawn:   TickSpawn();   break;
            case BossState.Idle:    TickIdle();    break;
            case BossState.Chase:   TickChase();   break;
            case BossState.Retreat: TickRetreat(); break;
            case BossState.Attack:  TickAttack();  break;
            case BossState.Hurt:    TickHurt();    break;
            // Dead: nessuna logica da far girare, si esce già sopra con isDead
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (stateTimer > 0f)
            stateTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        bool canMove = currentState == BossState.Chase || currentState == BossState.Retreat;
        Vector2 velocity = canMove ? moveDirection * currentSpeed : Vector2.zero;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    // ---------------- LOGICA DEGLI STATI ----------------

    private void TickIdle()
    {
        animator.SetBool(ParamMoving, false);

        if (target == null) return;

        if (Vector2.Distance(transform.position, target.position) <= chaseRange)
            ChangeState(BossState.Chase);
    }

    private void TickChase()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= retreatRange)
        {
            ChangeState(BossState.Retreat);
            return;
        }
        if (distance <= attackRange && attackTimer <= 0f)
        {
            ChangeState(BossState.Attack);
            return;
        }
        if (distance > chaseRange)
        {
            ChangeState(BossState.Idle);
            return;
        }

        SetMovement(target.position - transform.position, forward: true);
    }

    private void TickRetreat()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > retreatRange)
        {
            ChangeState(BossState.Chase);
            return;
        }

        SetMovement(target.position - transform.position, forward: false);
    }

    // Questi tre Tick sono il fallback a tempo: fanno avanzare la FSM da soli,
    // anche se non hai ancora collegato nessun Animation Event nell'Animator.
    // Se colleghi OnSpawnAnimationEnd/OnAttackAnimationEnd/OnHurtAnimationEnd,
    // quelli scattano prima (timing preciso) e questi diventano ridondanti ma innocui.

    private void TickSpawn()
    {
        if (stateTimer <= 0f)
            ChangeState(BossState.Idle);
    }

    private void TickAttack()
    {
        if (stateTimer <= 0f)
            ChangeState(BossState.Chase);
    }

    private void TickHurt()
    {
        if (stateTimer <= 0f)
            ChangeState(BossState.Chase);
    }

    /// <summary>
    /// Imposta direzione/velocità di movimento e i parametri Animator corrispondenti.
    /// forward = true  -> il boss avanza verso il target (walk / run)
    /// forward = false -> il boss arretra (walk back / run back)
    /// </summary>
    private void SetMovement(Vector2 towardTarget, bool forward)
    {
        moveDirection = (forward ? towardTarget : -towardTarget).normalized;
        currentSpeed = isEnraged ? runSpeed : walkSpeed;

        animator.SetBool(ParamMoving, true);
        animator.SetBool(ParamRunning, isEnraged);
        animator.SetBool(ParamRetreat, !forward);
    }

    private void UpdateFacing()
    {
        if (target == null) return;
        // true quando il target è a sinistra del boss -> usa le clip "inverted"
        animator.SetBool(ParamInverted, target.position.x < transform.position.x);
    }

    // ---------------- TRANSIZIONI DI STATO ----------------

    private void ChangeState(BossState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BossState.Spawn:
                animator.SetTrigger(TrigSpawn);
                stateTimer = spawnDuration;
                break;
            case BossState.Attack:
                animator.SetBool(ParamMoving, false);
                animator.SetTrigger(TrigAttack);
                attackTimer = attackCooldown;
                stateTimer = attackDuration;
                break;
            case BossState.Hurt:
                animator.SetBool(ParamMoving, false);
                animator.SetTrigger(TrigHurt);
                stateTimer = hurtDuration;
                break;
            case BossState.Dead:
                animator.SetBool(ParamMoving, false);
                animator.SetTrigger(TrigDeath);
                break;
        }
    }

    // ---------------- ANIMATION EVENTS (opzionali) ----------------
    // La FSM ora avanza da sola grazie al fallback a tempo (spawnDuration /
    // attackDuration / hurtDuration + stateTimer), quindi il boss funziona anche
    // senza questi eventi collegati. Aggiungili solo quando vuoi un timing preciso
    // sincronizzato ai frame delle clip (click destro sulla clip in finestra
    // Animation -> Add Animation Event): se scattano prima del timer, hanno la
    // precedenza; se non li colleghi, ci pensa il timer.

    /// <summary>Da agganciare all'ultimo frame della clip "spawn".</summary>
    public void OnSpawnAnimationEnd()
    {
        if (currentState == BossState.Spawn)
            ChangeState(BossState.Idle);
    }

    /// <summary>Da agganciare al frame di impatto delle clip "attack".</summary>
    public void OnAttackHit()
    {
        if (target == null) return;

        if (Vector2.Distance(transform.position, target.position) <= attackRange * 1.2f)
        {
            // Sostituisci con la tua interfaccia/metodo di danno per il player, es.:
            // target.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
        }
    }

    /// <summary>Da agganciare all'ultimo frame delle clip "attack".</summary>
    public void OnAttackAnimationEnd()
    {
        if (currentState == BossState.Attack)
            ChangeState(BossState.Chase);
    }

    /// <summary>Da agganciare all'ultimo frame della clip "hurt".</summary>
    public void OnHurtAnimationEnd()
    {
        if (currentState == BossState.Hurt && !isDead)
            ChangeState(BossState.Chase);
    }

    // ---------------- DANNO / MORTE ----------------

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (!isEnraged && currentHealth <= maxHealth * enrageThreshold)
        {
            isEnraged = true;
            animator.SetBool(ParamEnraged, true);
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        ChangeState(BossState.Hurt);
    }

    private void Die()
    {
        isDead = true;
        ChangeState(BossState.Dead);
        // Es: disabilita collider / questo componente dopo l'animazione tramite
        // un ultimo Animation Event, oppure Destroy(gameObject, delay).
    }

    // ---------------- DEBUG ----------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, retreatRange);
    }
}