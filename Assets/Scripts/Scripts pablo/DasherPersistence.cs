using UnityEngine;

/// <summary>
/// Gestiona la persistencia del Dasher y la puerta.
/// Añadir a un GameObject vacío en la escena del Dasher.
/// </summary>
public class DasherPersistence : MonoBehaviour
{
    public static DasherPersistence instance;

    [SerializeField] private GameObject dasherGameObject;
    [SerializeField] private PersistentDoor door;

    public bool IsDefeated { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Comprobar si ya fue derrotado en el save
        if (SaveManager.instance != null && SlotMenu.CurrentSlot > 0)
        {
            SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
            if (data != null && data.dasherDefeated)
            {
                IsDefeated = true;
                if (dasherGameObject != null) dasherGameObject.SetActive(false);
                if (door != null) door.OpenInstant();
            }
        }
    }

    public void OnDasherDefeated()
    {
        IsDefeated = true;
        if (door != null) door.OpenAnimated();

        // Guardar
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
    }
}
