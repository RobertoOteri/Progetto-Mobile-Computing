using UnityEngine;

public class Player_Gun : MonoBehaviour
{
    [Header("Punti di Sparo per Direzione")]
    public Transform launchPointSide;
    public Transform launchPointUp;
    public Transform launchPointDown;

    [Header("Prefab Proiettile e Configurazione")]
    public GameObject gunBulletPrefab;

    [Header("Effetto Fiammata (Muzzle Flash)")]
    public GameObject muzzleFlashPrefab;
    public float flashDuration = 0.05f;
    public float flashRotationOffset = 0f;

    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .3f; // Cooldown pistola
    private float shootTimer;

    [Header("Blocco Movimento Sparo")]
    public float shootStopDuration = 0.15f; 
    private float stopTimer;

    // Proprietà pubbliche per PlayerMovement
    public bool IsShooting => stopTimer > 0;
    public Vector2 AimDirection => aimDirection;

    private Player_Combat combat;

    void Start()
    {
        combat = GetComponent<Player_Combat>();
    }

    void Update()
    {
        if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime;
        }

        shootTimer -= Time.deltaTime;

        // Verifica se il player ha la pistola equipaggiata (assicurati che in Player_Combat ci sia hasGun o simile)
        if (combat != null && !combat.hasGun) return;

        HandleAiming();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Per la pistola usiamo GetButtonDown (sparo singolo per ogni click)
        if (Input.GetButtonDown("Shoot") && shootTimer <= 0 && h == 0 && v == 0)
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
            if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
            {
                aimDirection = new Vector2(Mathf.Sign(horizontal), 0f);
            }
            else
            {
                aimDirection = new Vector2(0f, Mathf.Sign(vertical));
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

        if (activePoint == null || gunBulletPrefab == null) return;

        stopTimer = shootStopDuration;

        Bullet gunBullet = Instantiate(gunBulletPrefab, activePoint.position, Quaternion.identity).GetComponent<Bullet>();
        if (gunBullet != null)
        {
            gunBullet.direction = aimDirection;
        }

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