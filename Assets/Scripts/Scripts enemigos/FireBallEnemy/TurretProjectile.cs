using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 4f;

    private Vector2 _direction;
    private float _speed;

    public void Init(Vector2 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
        Destroy(gameObject, lifetime);

        // Rotar el sprite según dirección
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HollowKnightMovement player = other.GetComponent<HollowKnightMovement>();
            if (player != null)
                player.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
