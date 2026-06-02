using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn_Point_Script : MonoBehaviour
{
    [SerializeField] private string spawnID; // <- añadir este campo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();
            if (player != null)
            {
                player.SetRespawnPoint(transform);
                if (SaveManager.instance != null)
                {
                    SaveManager.instance.lastSafeScene = SceneManager.GetActiveScene().name;
                    SaveManager.instance.lastSpawnID = spawnID; // <- usar el ID, no el nombre
                    SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
                }
            }
        }
    }
}