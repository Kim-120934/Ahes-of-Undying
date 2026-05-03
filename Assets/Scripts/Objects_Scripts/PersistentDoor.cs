using UnityEngine;
using System.Collections;

public class PersistentDoor : MonoBehaviour
{
    [SerializeField] private float openDistance = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _closedPos = transform.position;
        _openPos = _closedPos + Vector3.up * openDistance;
    }

    public void OpenInstant()
    {
        transform.position = _openPos;
        if (_col != null) _col.enabled = false;
    }

    public void OpenAnimated()
    {
        StartCoroutine(SlideUp());
    }

    private IEnumerator SlideUp()
    {
        if (_col != null) _col.enabled = false;
        while (Vector3.Distance(transform.position, _openPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _openPos, openSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = _openPos;
    }
}
