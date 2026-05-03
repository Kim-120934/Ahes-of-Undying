using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D totemLight;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private float fadeDuration = 0.3f;
    private CanvasGroup _canvasGroup;
    private bool _playerInRange = false;
    private bool _panelOpen = false;

    private void Awake()
    {
        _canvasGroup = savePanel.GetComponent<CanvasGroup>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            ClosePanel();
        }
    }

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_panelOpen) ClosePanel();
            else OpenPanel();
        }

        if (_panelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    private void OpenPanel()
    {
        if (totemLight != null) totemLight.intensity = 4f;
        _panelOpen = true;
        savePanel.SetActive(true);
        HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
        if (player != null)
        {
            player.currentLives = player.maxLives;
            player.currentHits = player.maxHitsPerLife;
            player.currentSoul = player.maxSoul;
        }
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame(SlotMenu.CurrentSlot);
        StartCoroutine(Fade(0f, 1f));
    }

    private void ClosePanel()
    {
        _panelOpen = false;
        StartCoroutine(Fade(1f, 0f, () => savePanel.SetActive(false)));
    }

    private IEnumerator Fade(float from, float to, System.Action onComplete = null)
    {
        float timer = 0f;
        _canvasGroup.alpha = from;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = to;
        onComplete?.Invoke();
    }
}