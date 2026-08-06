using UnityEngine;

public class Player_Rifle : MonoBehaviour
{
    [Header("Punti di Sparo per Direzione")]
    public Transform launchPointSide;
    public Transform launchPointUp;
    public Transform launchPointDown;

    [Header("Prefab Proiettile e Configurazione")]
    public GameObject rifleBulletPrefab;

    [Header("Effetto Fiammata (Muzzle Flash)")]
    public GameObject muzzleFlashPrefab;
    public float flashDuration = 0.05f;
    public float flashRotationOffset = 0f;

    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;

    [Header("Blocco Movimento Sparo")]
    public float shootStopDuration = 0.2f; // Per quanti secondi il player rimane fermo quando spara
    private float stopTimer;

    // Proprietà pubbliche per leggere lo stato dall'esterno
    public bool IsShooting => stopTimer > 0;
    public Vector2 AimDirection => aimDirection; // <-- RIGA AGGIUNTA QUI

    private Player_Combat combat;

    void Start()
    {
        combat = GetComponent<Player_Combat>();
    }

    void Update()
    {
        // Gestione timer per il blocco movimento
        if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime;
        }

        shootTimer -= Time.deltaTime;

        // Se il player NON ha il fucile equipaggiato, blocca la mira e lo sparo
        if (combat != null && !combat.hasRifle) return;

        HandleAiming();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Rifle_Shoot") && shootTimer <= 0 && h == 0 && v == 0)
        {
            Shoot(); 
        }
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Snap a sole 4 Direzioni Cardinali (Niente Diagonali)
        if (horizontal != 0 || vertical != 0)
        {
            if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
            {
                aimDirection = new Vector2(Mathf.Sign(horizontal), 0f); // Solo Destra (+1) o Sinistra (-1)
            }
            else
            {
                aimDirection = new Vector2(0f, Mathf.Sign(vertical)); // Solo Su (+1) o Giù (-1)
            }
        }
    }

    private Transform GetActiveLaunchPoint()
    {
        if (aimDirection.y > 0)
        {
            return launchPointUp != null ? launchPointUp : transform;
        }
        else if (aimDirection.y < 0)
        {
            return launchPointDown != null ? launchPointDown : transform;
        }
        else
        {
            return launchPointSide != null ? launchPointSide : transform;
        }
    }

    public void Shoot()
    {
        Transform activePoint = GetActiveLaunchPoint();

        if (activePoint == null || rifleBulletPrefab == null) return;

        // Blocca il personaggio per la durata dello sparo
        stopTimer = shootStopDuration;

        // 1. Spawna e orienta il proiettile
        Bullet rifleBullet = Instantiate(rifleBulletPrefab, activePoint.position, Quaternion.identity).GetComponent<Bullet>();
        if (rifleBullet != null)
        {
            rifleBullet.direction = aimDirection;
        }

        // 2. Spawna la fiammata con l'offset di rotazione
        if (muzzleFlashPrefab != null)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + flashRotationOffset;
            Quaternion flashRotation = Quaternion.Euler(0, 0, angle);

            GameObject flash = Instantiate(muzzleFlashPrefab, activePoint.position, flashRotation);
            Destroy(flash, flashDuration);
        }

        shootTimer = shootCooldown;
    }
}