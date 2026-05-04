using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private string spawnPointID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (SaveManager.instance != null)
        {
            SaveManager.instance.nextSpawnID = spawnPointID;
            SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
        }

        if (SceneTransition.instance != null)
            SceneTransition.instance.LoadScene(nextScene);
        else
            SceneManager.LoadScene(nextScene);
    }
}