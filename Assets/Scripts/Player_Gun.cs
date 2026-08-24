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

    public float shootCooldown = 0.3f;
    private float shootTimer;

    [Header("Blocco Movimento Sparo")]
    public float shootStopDuration = 0.15f; 
    private float stopTimer;

    public bool IsShooting => stopTimer > 0;
    public Vector2 AimDirection => aimDirection;

    private Player_Combat combat;
    private PlayerMovement playerMovement;

    void Awake()
    {
        combat = GetComponent<Player_Combat>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime;
        }

        shootTimer -= Time.deltaTime;

        if (combat != null && !combat.hasGun) return;

        HandleAiming();

        // Tasto J per sparare da PC
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot(); 
        }
    }

    public void HandleAiming()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Se usi il Joystick Mobile, mira nella direzione della levetta
        if (playerMovement != null && playerMovement.movementJoystick != null &&
            (Mathf.Abs(playerMovement.movementJoystick.Horizontal) > 0.1f || Mathf.Abs(playerMovement.movementJoystick.Vertical) > 0.1f))
        {
            horizontal = playerMovement.movementJoystick.Horizontal;
            vertical = playerMovement.movementJoystick.Vertical;
        }
        else
        {
            // Altrimenti leggi da tastiera (WASD / Frecce)
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

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
        else if (playerMovement != null)
        {
            if (playerMovement.lastHorizontal != 0 || playerMovement.lastVertical != 0)
            {
                if (Mathf.Abs(playerMovement.lastHorizontal) >= Mathf.Abs(playerMovement.lastVertical))
                {
                    aimDirection = new Vector2(Mathf.Sign(playerMovement.lastHorizontal), 0f);
                }
                else
                {
                    aimDirection = new Vector2(0f, Mathf.Sign(playerMovement.lastVertical));
                }
            }
        }
    }

    private Transform GetActiveLaunchPoint()
    {
        if (aimDirection.y > 0 && launchPointUp != null)
        {
            return launchPointUp;
        }
        else if (aimDirection.y < 0 && launchPointDown != null)
        {
            return launchPointDown;
        }
        else if (launchPointSide != null)
        {
            return launchPointSide;
        }

        return transform;
    }

    public void Shoot()
    {
        if (shootTimer > 0) return;

        Transform activePoint = GetActiveLaunchPoint();

        if (gunBulletPrefab == null)
        {
            Debug.LogError("[Player_Gun] ERRORE: Gun Bullet Prefab non assegnato nell'Inspector!");
            return;
        }

        stopTimer = shootStopDuration;

        GameObject bulletObj = Instantiate(gunBulletPrefab, activePoint.position, Quaternion.identity);
        Bullet gunBullet = bulletObj.GetComponent<Bullet>();
        if (gunBullet != null)
        {
            gunBullet.direction = aimDirection;
        }

        if (AudioManager.Instance != null && AudioManager.Instance.gunShootSFX != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(AudioManager.Instance.gunShootSFX, 0.2f);
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