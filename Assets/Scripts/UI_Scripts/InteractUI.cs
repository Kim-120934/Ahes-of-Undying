using UnityEngine;
using System.Collections;

/// <summary>
/// Muestra una imagen parpadeante (tecla F) encima del objeto.
/// Añadir como componente en el mismo GameObject o en un hijo.
/// Asignar el SpriteRenderer que muestra la imagen en el Inspector.
/// </summary>
public class InteractUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SpriteRenderer keyIcon; // SpriteRenderer con el PNG de la tecla F
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Header("Parpadeo")]
    [SerializeField] private float blinkSpeed = 3f;

    private bool _visible = false;
    private Coroutine _blinkCoroutine;

    private void Awake()
    {
        if (keyIcon == null)
            keyIcon = GetComponentInChildren<SpriteRenderer>();

        // Desvincular del padre para que no herede el scale
        if (keyIcon != null)
            keyIcon.transform.SetParent(null);

        Hide();
    }

    private void Update()
    {
        if (keyIcon != null)
        {
            keyIcon.transform.position = transform.position + offset;
            // Mantener escala positiva siempre
            Vector3 scale = keyIcon.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            keyIcon.transform.localScale = scale;
        }
    }

    public void Show()
    {
        if (_visible) return;
        _visible = true;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink());
    }

    public void Hide()
    {
        _visible = false;
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        if (keyIcon != null)
            keyIcon.color = Color.clear;
    }

    private IEnumerator Blink()
    {
        while (_visible)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0 a 1
            if (keyIcon != null)
                keyIcon.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }
    }
}
