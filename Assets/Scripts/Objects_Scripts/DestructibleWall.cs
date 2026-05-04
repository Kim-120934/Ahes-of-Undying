using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
   
    public void DestroyWall()
    {
        GetComponent<PersistentDestructible>()?.OnDestroyed();
        AudioManager.instance.PlaySFX(AudioManager.instance.wallBreakSFX);
        Destroy(gameObject);

    }
}

