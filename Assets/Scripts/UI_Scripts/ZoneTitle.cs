using UnityEngine;
using System.Collections;
using TMPro;

public class  ZoneTitle : MonoBehaviour
{
    [SerializeField] private string zoneName;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        titleText.text = zoneName;
        StartCoroutine(ShowTitle());
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(0.5f); // pequeño delay al cargar

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}