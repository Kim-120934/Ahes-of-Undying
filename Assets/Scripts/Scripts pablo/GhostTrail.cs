using UnityEngine;
using System.Collections;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private float ghostInterval = 0.05f;
    [SerializeField] private float ghostDuration = 0.2f;
    [SerializeField] private Color ghostColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Material ghostMaterial;

    private SpriteRenderer _sr;
    private bool _active = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void StartTrail()
    {
        if (!_active)
            StartCoroutine(SpawnGhosts());
    }

    public void StopTrail()
    {
        _active = false;
    }

    private IEnumerator SpawnGhosts()
    {
        _active = true;
        while (_active)
        {
            SpawnGhost();
            yield return new WaitForSeconds(ghostInterval);
        }
    }

    private void SpawnGhost()
    {
        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        ghostSr.sprite = _sr.sprite;
        ghostSr.color = ghostColor;
        ghostSr.sortingLayerName = _sr.sortingLayerName;
        ghostSr.sortingOrder = _sr.sortingOrder - 1;
        if (ghostMaterial != null) ghostSr.material = ghostMaterial;

        Destroy(ghost, ghostDuration);
        StartCoroutine(FadeGhost(ghostSr));
    }

    private IEnumerator FadeGhost(SpriteRenderer sr)
    {
        float timer = 0f;
        Color startColor = sr.color;
        while (timer < ghostDuration && sr != null)
        {
            timer += Time.deltaTime;
            if (sr != null)
                sr.color = Color.Lerp(startColor, Color.clear, timer / ghostDuration);
            yield return null;
        }
    }
}