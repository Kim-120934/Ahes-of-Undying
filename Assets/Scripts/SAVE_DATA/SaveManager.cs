using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private string _pendingName = "Jugador";
    public string nextSpawnID;
    public string lastSafeScene = "";  
    public string lastSpawnID = "";

    public void SetPendingName(string name)
    {
        _pendingName = name;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetSavePath(int slot)
    {
        return Application.persistentDataPath + "/save_" + slot + ".json";
    }

    public void SaveGame(int slot)
    {
        
        HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
        if (player == null) return;
        
        SaveData data = new SaveData();
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        data.respawnX = player.respawnPoint != null ? player.respawnPoint.position.x : player.transform.position.x;
        data.respawnY = player.respawnPoint != null ? player.respawnPoint.position.y : player.transform.position.y;
        data.coins = CoinManager.instance.GetCoins();
        data.currentLives = player.currentLives;
        data.currentHits = player.currentHits;
        data.currentSoul = player.currentSoul;
        data.hasProjectile = player.hasProjectile;
        data.hasDoubleJump = player.hasDoubleJump;
        data.hasDash = player.hasDash;
        data.dasherDefeated = DasherPersistence.instance != null ?
    DasherPersistence.instance.IsDefeated :
    (LoadGame(slot)?.dasherDefeated ?? false);
        SaveData existingData = LoadGame(slot);
        data.playerName = (existingData != null && !string.IsNullOrEmpty(existingData.playerName))
            ? existingData.playerName
            : _pendingName;
        if (existingData != null && existingData.destroyedWalls != null)
        {
            data.destroyedWalls = existingData.destroyedWalls;
            
        }
        // Mantener lista de paredes destruidas
        if (existingData != null && existingData.destroyedWalls != null)
            data.destroyedWalls = existingData.destroyedWalls;

        // Mantener dasherDefeated si no hay DasherPersistence en escena
        data.dasherDefeated = DasherPersistence.instance != null ?
            DasherPersistence.instance.IsDefeated :
            (existingData?.dasherDefeated ?? false);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
       
        data.bossDefeated = BossPersistence.instance != null ?
         BossPersistence.instance.IsDefeated :
         (existingData?.bossDefeated ?? false);
    }

    public SaveData LoadGame(int slot)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path))
        {
            Debug.Log("No hay partida guardada en slot " + slot);
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Partida cargada del slot " + slot);
        return data;
    }

    public bool SlotExists(int slot)
    {
        string path = GetSavePath(slot);
        Debug.Log("RUTA COMPROBACION: " + path);
        return File.Exists(path);
    }

    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Partida borrada del slot " + slot);
        }
    }

}