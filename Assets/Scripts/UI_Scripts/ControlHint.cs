using UnityEngine;
using System.Collections;

public class ControlHint : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer hintIcon;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private KeyCode keyToPress;

    [Header("Parpadeo")]
    [SerializeField] private float blinkSpeed = 3f;

    private bool _shown = false;
    private bool _completed = false;
    private Coroutine _blinkCoroutine;

    private void Update()
    {
        if (_completed) return;

        if (_shown && Input.GetKeyDown(keyToPress))
        {
            _completed = true;
            Hide();
        }

        if (hintIcon != null)
            hintIcon.transform.position = transform.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_completed) return;
        if (!other.CompareTag("Player")) return;
        Show();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_completed) return;
        if (!other.CompareTag("Player")) return;
        Hide();
    }

    private void Show()
    {
        if (_shown) return;
        _shown = true;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink());
    }

    private void Hide()
    {
        _shown = false;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        if (hintIcon != null) hintIcon.color = Color.clear;
    }

    private IEnumerator Blink()
    {
        while (_shown)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;
            if (hintIcon != null)
                hintIcon.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }
    }
}
