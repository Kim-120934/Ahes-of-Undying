using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelMenu : MonoBehaviour
{
    public void TravelTo_Zona1() => TravelTo("Sala_2", "Sala_2.1");
    public void TravelTo_Zona2() => TravelTo("Sala_3", "Sala_3.1");
    public void TravelTo_Zona3() => TravelTo("Sala_4", "Sala_4.1");
    // añade uno por cada totem

    private void TravelTo(string sceneName, string spawnID)
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.nextSpawnID = spawnID;
            SaveManager.instance.lastSafeScene = sceneName;
            SaveManager.instance.lastSpawnID = spawnID;
        }
        Time.timeScale = 1f;
        if (SceneTransition.instance != null)
            SceneTransition.instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}