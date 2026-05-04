using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    private bool _shown = false;
    [SerializeField] private string tutorialID;

    private void Start()
    {
        if (SaveManager.instance == null) return;
        SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
        if (data != null && System.Array.Exists(data.destroyedWalls, id => id == tutorialID))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_shown) return;
        if (!other.CompareTag("Player")) return;
        _shown = true;
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(Fade(0f, 1f));
    }

    private void Update()
    {
        if (!_shown) return;
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(FadeAndClose());
    }

    private IEnumerator FadeAndClose()
    {
        yield return StartCoroutine(Fade(1f, 0f));
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        SaveData data = SaveManager.instance.LoadGame(SlotMenu.CurrentSlot);
        if (data != null)
        {
            var list = new System.Collections.Generic.List<string>(data.destroyedWalls);
            if (!list.Contains(tutorialID))
                list.Add(tutorialID);
            data.destroyedWalls = list.ToArray();
            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(
                Application.persistentDataPath + "/save_" + SlotMenu.CurrentSlot + ".json", json);
        }
        Destroy(gameObject);
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