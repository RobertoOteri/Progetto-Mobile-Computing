using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    [Header("Impostazioni Spawn Point")]
    [Tooltip("Scrivi il nome esatto dell'oggetto SpawnPoint di destinazione nella nuova scena")]
    public string targetSpawnPointName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 🔹 NUOVO: Salva l'arma e il suo stato prima di cambiare scena
            Player_Combat combat = collision.GetComponent<Player_Combat>();
            if (combat != null)
            {
                combat.SaveWeaponData();
            }

            // 1. Salva il nome dello SpawnPoint in cui il player apparirà
            if (!string.IsNullOrEmpty(targetSpawnPointName))
            {
                PlayerPrefs.SetString("TargetSpawnPoint", targetSpawnPointName);
            }

            // 2. Registra che l'intro del gioco è ormai superata
            PlayerPrefs.SetInt("GameIntroCompleted", 1);
            PlayerPrefs.Save();

            if (fadeAnim != null)
            {
                fadeAnim.Play("FadeToWhite");
            }

            StartCoroutine(DelayFade());
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}