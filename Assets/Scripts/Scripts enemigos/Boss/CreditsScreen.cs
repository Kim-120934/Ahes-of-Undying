using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayDuration = 8f;

    private void Start()
    {
        StartCoroutine(CreditsSequence());
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
    private IEnumerator CreditsSequence()
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, 1f));

        // Esperar con opción de salir
        float timer = 0f;
        while (timer < displayDuration)
        {
            timer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape) || Input.anyKeyDown)
            {
                SceneManager.LoadScene(0);
                yield break;
            }
            yield return null;
        }

        // Fade out y volver al menú
        yield return StartCoroutine(Fade(1f, 0f, 1f));
        SceneManager.LoadScene(0);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        canvasGroup.alpha = from;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}