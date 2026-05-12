using UnityEngine;
using System.Collections;

public class BossPersistence : MonoBehaviour
{
    public static BossPersistence instance;

    [SerializeField] private GameObject bossGameObject;
    [SerializeField] private PersistentDoor exitDoor;
    [SerializeField] private VictoryUI victoryUI;

    public bool IsDefeated { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (SaveManager.instance != null && SlotMenu.CurrentSlot > 0)
        {
            SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
            if (data != null && data.bossDefeated)
            {
                IsDefeated = true;
                if (bossGameObject != null) bossGameObject.SetActive(false);
                if (exitDoor != null) exitDoor.OpenInstant();
            }
        }
    }

    public void OnBossDefeated()
    {
        IsDefeated = true;
        if (exitDoor != null) exitDoor.OpenAnimated();
        if (victoryUI != null) victoryUI.Show();
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
        StartCoroutine(GoToCredits());
    }

    private IEnumerator GoToCredits()
    {
        yield return new WaitForSeconds(4f); // tiempo para ver la puerta abrirse y el victoryUI
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits"); // cambia "Credits" por el nombre exacto de tu escena
    }
}
