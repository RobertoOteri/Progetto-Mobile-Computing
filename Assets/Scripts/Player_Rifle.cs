using UnityEngine;

public class Player_Rifle : MonoBehaviour
{

    public Transform launchPoint;
    public GameObject rifleBulletPrefab;

    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;

    // Update is called once per frame
    void Update()
    {
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

        if(horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
        }
    }

    public void Shoot()
    {
        Bullet rifleBullet = Instantiate(rifleBulletPrefab, launchPoint.position, Quaternion.identity).GetComponent<Bullet>();
        rifleBullet.direction = aimDirection;
        shootTimer = shootCooldown;
    }


}
