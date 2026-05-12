using UnityEngine;
using System.Collections;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public static VictoryUI instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float displayDuration = 4f;

    private void Awake()
    {
        instance = this;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        Time.timeScale = 0f;

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));

        // Esperar
        yield return new WaitForSecondsRealtime(displayDuration);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        Time.timeScale = 1f;

        // Cargar créditos
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    private IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        canvasGroup.alpha = from;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
