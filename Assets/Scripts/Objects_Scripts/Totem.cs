using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Totem : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D totemLight;
    [SerializeField] private GameObject savePanel;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Fast Travel")]
    [SerializeField] private GameObject travelPanel;         // Panel separado dentro del savePanel
    [SerializeField] private Transform travelButtonContainer; // El layout donde se crean los botones
    [SerializeField] private GameObject travelButtonPrefab;  // Prefab de botón con un Text/TextMeshPro
    [SerializeField] private List<TotemDestination> destinations; // Los destinos configurados en Inspector

    private CanvasGroup _canvasGroup;
    private bool _playerInRange = false;
    private bool _panelOpen = false;
    private bool _travelPanelOpen = false;

    private void Awake()
    {
        _canvasGroup = savePanel.GetComponent<CanvasGroup>();
        if (travelPanel != null) travelPanel.SetActive(false);
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
            CloseTravelPanel();
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

        // Abrir/cerrar mapa de viaje solo si el panel del totem está abierto
        if (_panelOpen && Input.GetKeyDown(KeyCode.M))
        {
            if (_travelPanelOpen) CloseTravelPanel();
            else OpenTravelPanel();
        }

        if (_panelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            if (_travelPanelOpen)
                CloseTravelPanel();
            else
            {
                ClosePanel();
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
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
        CloseTravelPanel();
        _panelOpen = false;
        StartCoroutine(Fade(1f, 0f, () => savePanel.SetActive(false)));
    }

    private void OpenTravelPanel()
    {
        _travelPanelOpen = true;
        travelPanel.SetActive(true);

        // Limpiar botones anteriores
        foreach (Transform child in travelButtonContainer)
            Destroy(child.gameObject);

        // Crear un botón por destino
        foreach (TotemDestination dest in destinations)
        {
            GameObject btn = Instantiate(travelButtonPrefab, travelButtonContainer);
            // Soporte para Text y TextMeshPro
            var txt = btn.GetComponentInChildren<Text>();
            if (txt != null) txt.text = dest.zoneName;
            var tmpTxt = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpTxt != null) tmpTxt.text = dest.zoneName;

            TotemDestination captured = dest; // capturar para el lambda
            btn.GetComponentInChildren<Button>().onClick.AddListener(() => TravelTo(captured));
        }
    }

    private void CloseTravelPanel()
    {
        _travelPanelOpen = false;
        if (travelPanel != null) travelPanel.SetActive(false);
    }

    private void TravelTo(TotemDestination dest)
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.nextSpawnID = dest.spawnID;
            SaveManager.instance.lastSafeScene = dest.sceneName;
            SaveManager.instance.lastSpawnID = dest.spawnID;
        }

        Time.timeScale = 1f;

        if (SceneTransition.instance != null)
            SceneTransition.instance.LoadScene(dest.sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(dest.sceneName);
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