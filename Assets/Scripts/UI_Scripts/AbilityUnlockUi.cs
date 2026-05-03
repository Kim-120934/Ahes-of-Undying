using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class AbilityUnlockUI : MonoBehaviour
{
    public static AbilityUnlockUI instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;

    private bool _isOpen = false;
    private System.Action _onClose;

    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    public void Show(System.Action onClose = null)
    {
        _onClose = onClose;
        _isOpen = true;
        panel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(Fade(0f, 1f));
    }

    private void Update()
    {
        if (!_isOpen) return;
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Close());
    }

    private IEnumerator Close()
    {
        _isOpen = false;
        yield return StartCoroutine(Fade(1f, 0f));
        panel.SetActive(false);
        Time.timeScale = 1f;
        _onClose?.Invoke();
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