using UnityEngine;
using System.Collections;

public class BossLaserColumn : MonoBehaviour
{
    [SerializeField] private GameObject warningObject;
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float damageDuration = 0.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2 hitboxSize = new Vector2(1f, 8f);

    private void Start()
    {
        if (warningObject != null) warningObject.SetActive(false);
        if (laserObject != null) laserObject.SetActive(false);
        StartCoroutine(LaserSequence());
    }

    private IEnumerator LaserSequence()
    {
        // Warning
        if (warningObject != null) warningObject.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        if (warningObject != null) warningObject.SetActive(false);

        // Laser activo
        if (laserObject != null) laserObject.SetActive(true);
        yield return new WaitForSeconds(0.5f); // espera a que aparezca el rayo en la animación

        float timer = 0f;
        while (timer < damageDuration)
        {
            timer += Time.deltaTime;
            HollowKnightMovement playerDebug = FindFirstObjectByType<HollowKnightMovement>();
            Debug.Log($"Jugador en: {playerDebug?.transform.position} Laser en: {transform.position}");
            Vector2 checkPos = new Vector2(transform.position.x, transform.position.y - hitboxSize.y / 2f);
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPos, hitboxSize, 0f, playerLayer);
            Debug.Log($"Laser hits: {hits.Length}");
            foreach (Collider2D hit in hits)
            {
                HollowKnightMovement player = hit.GetComponent<HollowKnightMovement>();
                if (player != null) player.TakeDamage(damage, transform.position);
            }
            yield return new WaitForSeconds(0.2f);
        }

        if (laserObject != null) laserObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, hitboxSize);
    }
}