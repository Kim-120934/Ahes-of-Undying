using UnityEngine;
using System.Collections;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    private bool _movingRight = true;

    public void Init(bool movingRight)
    {
        _movingRight = movingRight;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        float dir = _movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();
            if (player != null)
                player.TakeDamage(damage, transform.position);
        }
    }
}