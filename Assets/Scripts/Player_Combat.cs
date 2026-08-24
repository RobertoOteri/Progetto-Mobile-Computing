using System.Collections;
using UnityEngine;

public enum WeaponType { Sword = 0, Hammer = 1, Rifle = 2, Gun = 3, Bomb = 4, None = 5 }

public class Player_Combat : MonoBehaviour
{   
    [Header("Tasto Equipaggia / Riponi")]
    public KeyCode toggleWeaponKey = KeyCode.E;

    [Header("Arma Posseduta")]
    public WeaponType storedWeapon = WeaponType.Gun;
    public bool isWeaponDrawn = false;

    [Header("Parametri Attacco Melee")]
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float knockbackForce = 25f;
    public float knockbackTime = 0.15f;
    public float stunTime = 0.3f;
    public LayerMask enemyLayer;
    public int damage = 1;

    [Header("Riferimenti")]
    public Animator anim;
    public Player_Rifle playerRifle;
    public Player_Gun playerGun;
    private PlayerMovement playerMovement;

    [Header("Oggetti Figli nel Player")]
    public GameObject swordObject;  
    public GameObject hammerObject; 
    public GameObject rifleObject;
    public GameObject gunObject;
    public GameObject bombObject;

    [Header("Stato Equipaggiamento (In Mano)")]
    public bool hasSword = false;
    public bool hasHammer = false;
    public bool hasRifle = false;
    public bool hasGun = false;
    public bool hasBomb = false;

    [Header("Impostazioni Lancio Bomba")]
    public GameObject bombProjectilePrefab;
    public float bombThrowForce = 7f;
    public float bombThrowDuration = 0.4f;

    public bool IsThrowingBomb { get; private set; } = false;

    private Collider2D playerCollider;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        playerMovement = GetComponent<PlayerMovement>();
        if (anim == null) anim = GetComponent<Animator>();
        if (playerRifle == null) playerRifle = GetComponent<Player_Rifle>();
        if (playerGun == null) playerGun = GetComponent<Player_Gun>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("SavedWeapon"))
        {
            storedWeapon = (WeaponType)PlayerPrefs.GetInt("SavedWeapon");
            isWeaponDrawn = PlayerPrefs.GetInt("SavedIsDrawn", 0) == 1;
        }
        ApplyWeaponState();
    }

    private void Update()
    {
        // Tasti Toggle per estrarre/riporre su PC (E oppure 1)
        if (Input.GetKeyDown(toggleWeaponKey) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            ToggleWeapon();
        }

        // Tasto K per Spada/Martello su PC
        if (Input.GetKeyDown(KeyCode.K) && (hasSword || hasHammer))
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            float lastV = playerMovement != null ? playerMovement.lastVertical : 0f;
            float lastH = playerMovement != null ? playerMovement.lastHorizontal : 1f;
            Attack(v, h, lastV, lastH);
        }

        // Tasto Q per lanciare Bomba su PC
        if (Input.GetKeyDown(KeyCode.Q) && hasBomb)
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            float lastV = playerMovement != null ? playerMovement.lastVertical : 0f;
            float lastH = playerMovement != null ? playerMovement.lastHorizontal : 1f;
            Attack(v, h, lastV, lastH);
        }
    }

    public void SaveWeaponData()
    {
        PlayerPrefs.SetInt("SavedWeapon", (int)storedWeapon);
        PlayerPrefs.SetInt("SavedIsDrawn", isWeaponDrawn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleWeapon()
    {
        if (storedWeapon == WeaponType.None) return;

        isWeaponDrawn = !isWeaponDrawn;
        ApplyWeaponState();
        SaveWeaponData();
    }

    public void EquipNewWeapon(WeaponType newWeapon)
    {
        storedWeapon = newWeapon;
        isWeaponDrawn = true;
        ApplyWeaponState();
        SaveWeaponData();
    }

    public void ApplyWeaponState()
    {
        hasSword = false;
        hasHammer = false;
        hasRifle = false;
        hasGun = false;
        hasBomb = false;

        if (isWeaponDrawn)
        {
            switch (storedWeapon)
            {
                case WeaponType.Sword: hasSword = true; break;
                case WeaponType.Hammer: hasHammer = true; break;
                case WeaponType.Rifle: hasRifle = true; break;
                case WeaponType.Gun: hasGun = true; break;
                case WeaponType.Bomb: hasBomb = true; break;
            }
        }

        UpdateWeaponVisibility();
        UpdateAnimatorBools();
        SyncGunScripts();
        UpdateMobileUI();
    }

    public void UpdateMobileUI()
    {
        if (AttackButtonUI.Instance == null) return;

        if (hasSword || hasHammer)
        {
            AttackButtonUI.Instance.SetMeleeMode();
        }
        else if (hasGun || hasRifle)
        {
            AttackButtonUI.Instance.SetRangedMode();
        }
        else if (hasBomb)
        {
            AttackButtonUI.Instance.SetBombMode();
        }
        else
        {
            AttackButtonUI.Instance.SetMeleeMode();
        }
    }

    // Eseguito dal pulsante UI Attack/Shoot/Launch
    public void ExecuteCurrentWeaponAction(float vInput, float hInput, float lastV, float lastH)
    {
        if (!isWeaponDrawn) return;

        // 1. Se ha la Pistola
        if (hasGun && playerGun != null)
        {
            playerGun.Shoot();
            return;
        }

        // 2. Se ha il Fucile
        if (hasRifle && playerRifle != null)
        {
            playerRifle.Shoot();
            return;
        }

        // 3. Se ha Spada, Martello o Bomba
        if (hasSword || hasHammer || hasBomb)
        {
            Attack(vInput, hInput, lastV, lastH);
        }
    }

    public void Attack(float vInput, float hInput, float lastV, float lastH)
    {
        if (anim == null || IsThrowingBomb || !isWeaponDrawn) return;
        if (!hasSword && !hasHammer && !hasBomb) return;

        float vert = 0f;
        float horiz = 0f;

        float targetV = (vInput != 0) ? vInput : lastV;
        float targetH = (hInput != 0) ? hInput : lastH;

        if (targetV != 0 && hInput == 0)
        {
            vert = targetV > 0 ? 1f : -1f;
        }
        else
        {
            horiz = targetH < 0 ? -1f : 1f;
        }

        Vector2 throwDirection = new Vector2(horiz, vert).normalized;

        if (attackPoint != null)
        {
            float offset = 0.8f;
            attackPoint.position = transform.position + new Vector3(throwDirection.x * offset, throwDirection.y * offset, 0f);
        }

        if (hasBomb)
        {
            StartCoroutine(ThrowBombRoutine(throwDirection, vert, horiz));
            return;
        }

        anim.SetFloat("vertical", vert);
        anim.SetFloat("horizontal", horiz);
        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("isAttacking", true);

        if (AudioManager.Instance != null)
        {
            if (hasSword)
            {
                AudioManager.Instance.PlaySFXWithVolume(AudioManager.Instance.swordAttackSFX, 0.3f);
            }
            else if (hasHammer)
            {
                AudioManager.Instance.PlaySFXWithVolume(AudioManager.Instance.hammerAttackSFX, 0.3f);
            }
        }
    }

    private IEnumerator ThrowBombRoutine(Vector2 direction, float vert, float horiz)
    {
        IsThrowingBomb = true;

        anim.SetFloat("horizontal", 0f);
        anim.SetFloat("vertical", 0f);

        storedWeapon = WeaponType.None;
        isWeaponDrawn = false;
        ApplyWeaponState();
        SaveWeaponData();

        if (vert > 0)
            anim.Play("Bomb_throw_up", 0, 0f);
        else if (vert < 0)
            anim.Play("Bomb_throw_down", 0, 0f);
        else
            anim.Play("Bomb_throw_side", 0, 0f);

        if (bombProjectilePrefab != null && attackPoint != null)
        {
            GameObject bomb = Instantiate(bombProjectilePrefab, attackPoint.position, Quaternion.identity);

            Collider2D bombCollider = bomb.GetComponent<Collider2D>();
            if (bombCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(bombCollider, playerCollider, true);
            }

            BombProjectile bp = bomb.GetComponent<BombProjectile>();
            if (bp != null)
            {
                bp.Launch(direction, bombThrowForce);
            }
            else
            {
                Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * bombThrowForce;
                }
            }
        }

        yield return new WaitForSeconds(bombThrowDuration);

        IsThrowingBomb = false;
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            Enemy_Health health = enemies[0].GetComponent<Enemy_Health>();
            if (health != null) health.ChangeHealth(-damage);

            Enemy_Knockback kb = enemies[0].GetComponent<Enemy_Knockback>();
            if (kb != null) kb.Knockback(transform, knockbackForce, knockbackTime, stunTime);
        }
    }

    public void FinishAttack()
    {
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }

    public void EquipSword() => EquipNewWeapon(WeaponType.Sword);
    public void EquipHammer() => EquipNewWeapon(WeaponType.Hammer);
    public void EquipRifle() => EquipNewWeapon(WeaponType.Rifle);
    public void EquipGun() => EquipNewWeapon(WeaponType.Gun);
    public void EquipBomb() => EquipNewWeapon(WeaponType.Bomb);

    private void UpdateWeaponVisibility()
    {
        if (swordObject != null) swordObject.SetActive(hasSword);
        if (hammerObject != null) hammerObject.SetActive(hasHammer);
        if (rifleObject != null) rifleObject.SetActive(hasRifle);
        if (gunObject != null) gunObject.SetActive(hasGun);
        if (bombObject != null) bombObject.SetActive(hasBomb);
    }

    public void HideWeapons()
    {
        if (swordObject != null) swordObject.SetActive(false);
        if (hammerObject != null) hammerObject.SetActive(false);
        if (rifleObject != null) rifleObject.SetActive(false);
        if (gunObject != null) gunObject.SetActive(false);
        if (bombObject != null) bombObject.SetActive(false);
    }

    public void RestoreWeapons()
    {
        UpdateWeaponVisibility();
    }

    private void UpdateAnimatorBools()
    {
        if (anim == null) return;

        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("hasRifle", hasRifle);
        anim.SetBool("hasGun", hasGun);
        anim.SetBool("hasBomb", hasBomb);
    }

    private void SyncGunScripts()
    {
        if (playerRifle != null) playerRifle.enabled = hasRifle;
        if (playerGun != null) playerGun.enabled = hasGun;
    }
}