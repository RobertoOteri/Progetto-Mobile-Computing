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
    public GameObject muzzleFlashPrefab; // Trascina qui MuzzleFlash_rifle
    public float flashDuration = 0.05f;   // Durata fiammata
    public float flashRotationOffset = 0f; // Offset in gradi per correggere la rotazione (es. 180 se è al contrario)

    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;

    private Player_Combat combat;

    void Start()
    {
        combat = GetComponent<Player_Combat>();
    }

    void Update()
    {
        // Se il player NON ha il fucile equipaggiato, blocca lo sparo
        if (combat != null && !combat.hasRifle) return;

        shootTimer -= Time.deltaTime;

        HandleAiming();

        if (Input.GetButtonDown("Rifle_Shoot") && shootTimer <= 0)
        {
            Shoot(); 
        }
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
        }
    }

    private Transform GetActiveLaunchPoint()
    {
        if (Mathf.Abs(aimDirection.y) > Mathf.Abs(aimDirection.x))
        {
            if (aimDirection.y > 0)
            {
                return launchPointUp != null ? launchPointUp : transform;
            }
            else
            {
                return launchPointDown != null ? launchPointDown : transform;
            }
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