using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;

    void Start()
    {
        pannelloPrincipale.SetActive(true);
        pannelloImpostazioni.SetActive(false);
    }

    public void Gioca() { SceneManager.LoadScene("Intro"); }
    public void Esci() { Application.Quit(); }

    public void ApriImpostazioni() 
    { 
        pannelloPrincipale.SetActive(false); 
        pannelloImpostazioni.SetActive(true); 
    }

    public void ChiudiImpostazioni() 
    { 
        pannelloImpostazioni.SetActive(false); 
        pannelloPrincipale.SetActive(true); 
    }
}