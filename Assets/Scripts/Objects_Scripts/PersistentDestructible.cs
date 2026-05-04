using UnityEngine;

public class PersistentDestructible : MonoBehaviour
{
    [SerializeField] private string wallID;

    private void Start()
    {
        if (SaveManager.instance == null) return;
        SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
        if (data != null && System.Array.Exists(data.destroyedWalls, id => id == wallID))
        {
            Debug.Log($"Destruyendo pared {wallID}");
            Destroy(gameObject);
        }
    }

    public void OnDestroyed()
    {
        if (SaveManager.instance == null) return;
        SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
        if (data == null) return;

        var list = new System.Collections.Generic.List<string>(data.destroyedWalls);
        if (!list.Contains(wallID))
            list.Add(wallID);
        data.destroyedWalls = list.ToArray();

        // Guardar directamente el JSON
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(
            Application.persistentDataPath + "/save_" + SlotMenu.CurrentSlot + ".json", json);
    }
}