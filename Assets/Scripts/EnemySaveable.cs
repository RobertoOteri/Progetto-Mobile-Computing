using UnityEngine;

public class EnemySaveable : MonoBehaviour
{
    [Header("ID Univoco Nemico")]
    public string enemyID;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = gameObject.name + "_" + transform.GetSiblingIndex();
        }

        // CONTROLLO PERSISTENZA: se il nemico è già stato eliminato in precedenza, disattivalo subito
        if (PlayerPrefs.GetInt("EnemyDead_" + enemyID, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void MarkAsPermanentlyDead()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = gameObject.name + "_" + transform.GetSiblingIndex();
        }

        PlayerPrefs.SetInt("EnemyDead_" + enemyID, 1);
        PlayerPrefs.Save();
    }

    public EnemySaveData GetSaveData()
    {
        EnemySaveData data = new EnemySaveData();
        data.enemyID = enemyID;
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        data.isDead = !gameObject.activeSelf || PlayerPrefs.GetInt("EnemyDead_" + enemyID, 0) == 1;

        // Nemici
        Enemy_Health hp = GetComponent<Enemy_Health>();
        if (hp != null) data.currentHealth = hp.currentHealth;

        // Boss
        DemonBoss_Movement boss = GetComponent<DemonBoss_Movement>();
        if (boss != null)
        {
            boss.FillBossSaveData(data);
        }

        return data;
    }

    public void LoadData(EnemySaveData data)
    {
        if (data.isDead || PlayerPrefs.GetInt("EnemyDead_" + enemyID, 0) == 1)
        {
            MarkAsPermanentlyDead();
            gameObject.SetActive(false);
            return;
        }

        Vector3 targetPos = new Vector3(data.posX, data.posY, transform.position.z);
        transform.position = targetPos;

        if (rb != null)
        {
            rb.position = targetPos;
            rb.linearVelocity = Vector2.zero;
        }

        // Ripristina la vita
        Enemy_Health hp = GetComponent<Enemy_Health>();
        if (hp != null) hp.currentHealth = data.currentHealth;

        // Ripristina la fase e lo stato di movimento del boss
        DemonBoss_Movement boss = GetComponent<DemonBoss_Movement>();
        if (boss != null)
        {
            boss.LoadBossData(data);
        }
    }
}