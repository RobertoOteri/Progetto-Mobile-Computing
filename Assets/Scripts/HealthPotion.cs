using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Identificativo Univoco")]
    [Tooltip("Lascialo vuoto se vuoi che venga generato in automatico dalla posizione")]
    public string itemID;

    public int healAmount = 2; 

    [Header("--- Audio ---")]
    [SerializeField] private AudioClip healSFX;

    private void Awake()
    {
        // Se l'ID è vuoto, ne genera uno univoco con scena, nome e coordinate
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = gameObject.scene.name + "_" + gameObject.name + "_" + transform.position.x.ToString("F2") + "_" + transform.position.y.ToString("F2");
        }
    }

    private void Start()
    {
        // Se stiamo caricando una partita salvata e questo oggetto era già stato consumato, distruggilo subito
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsItemConsumed(itemID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Cura il giocatore solo se ha vita mancante
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.ChangeHealth(healAmount);

                    if (healSFX != null && AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(healSFX);
                    }

                    // Registra l'oggetto come consumato nel SaveSystem
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.RegisterConsumedItem(itemID);
                    }
                    
                    // Distrugge l'oggetto dalla mappa
                    Destroy(gameObject);
                }
            }
        }
    }
}