using System.Collections;
using UnityEngine;

// Ordine originale esatto per non sfasare l'Inspector di Unity
public enum WeaponType { Sword = 0, Hammer = 1, Rifle = 2, Gun = 3, Bomb = 4, None = 5 }

public class Player_Combat : MonoBehaviour
{   
    [Header("Tasto Equipaggia / Riponi")]
    public KeyCode toggleWeaponKey = KeyCode.E;

    [Header("Arma Posseduta")]
    public WeaponType storedWeapon = WeaponType.Gun; // Parte con la Pistola memorizzata
    public bool isWeaponDrawn = false; // Parte a mani nude

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
        if (playerRifle == null) playerRifle = GetComponent<Player_Rifle>();
        if (playerGun == null) playerGun = GetComponent<Player_Gun>();
    }

    private void Start()
    {
        // Assicurati che parta a mani nude ma con la pistola memorizzata
        storedWeapon = WeaponType.Gun;
        isWeaponDrawn = false;
        ApplyWeaponState();
    }

    private void Update()
    {
        // Tasto Toggle per estrarre/riporre
        if (Input.GetKeyDown(toggleWeaponKey) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            ToggleWeapon();
        }
    }

    public void ToggleWeapon()
    {
        if (storedWeapon == WeaponType.None) return;

        isWeaponDrawn = !isWeaponDrawn;
        ApplyWeaponState();
    }

    public void EquipNewWeapon(WeaponType newWeapon)
    {
        storedWeapon = newWeapon;
        isWeaponDrawn = true; // Quando raccogli, la metti subito in mano
        ApplyWeaponState();
    }

    private void ApplyWeaponState()
    {
        // 1. Resetta tutte le variabili
        hasSword = false;
        hasHammer = false;
        hasRifle = false;
        hasGun = false;
        hasBomb = false;

        // 2. Se l'arma è estratta, attiva la specifica arma memorizzata
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
    }

    public void Attack(float vInput, float hInput, float lastV, float lastH)
    {
        if (anim == null || IsThrowingBomb) return;
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