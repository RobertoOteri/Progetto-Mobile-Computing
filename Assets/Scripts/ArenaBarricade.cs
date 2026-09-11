using UnityEngine;

public class ArenaBarricade : MonoBehaviour
{
    public static ArenaBarricade Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (NPCTriggerDialogue.IsBossDefeated)
        {
            OpenBarricade();
            return;
        }

        OpenBarricade();
    }

    public void CloseBarricade()
    {
        if (NPCTriggerDialogue.IsBossDefeated)
        {
            return;
        }

        gameObject.SetActive(true);
    }


    public void OpenBarricade()
    {
        gameObject.SetActive(false);
       
    }
}