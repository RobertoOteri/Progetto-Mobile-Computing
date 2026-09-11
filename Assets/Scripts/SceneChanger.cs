using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    [Header("Impostazioni Spawn Point")]
    public string targetSpawnPointName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Combat combat = collision.GetComponent<Player_Combat>();
            if (combat != null)
            {
                combat.SaveWeaponData();
            }

            if (!string.IsNullOrEmpty(targetSpawnPointName))
            {
                PlayerPrefs.SetString("TargetSpawnPoint", targetSpawnPointName);
            }

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