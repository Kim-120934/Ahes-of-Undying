using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageInterval = 1f;
    [Header("Effects")]
    [SerializeField] private float shakeForce = 0.3f;

    private Transform _currentTarget;
    private float _lastDamageTime;
    private bool _facingRight = true;
    private EnemyHealth _enemyHealth;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        _currentTarget = pointB;
    }

    private void Update()
    {
        if (_enemyHealth != null && _enemyHealth.IsDead) return;

        MoveTowards(_currentTarget.position);

        if (Vector2.Distance(transform.position, _currentTarget.position) < 0.2f)
            _currentTarget = (_currentTarget == pointA) ? pointB : pointA;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        bool shouldFaceRight = dir.x > 0;
        if (shouldFaceRight != _facingRight)
        {
            _facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            DamagePlayer(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= _lastDamageTime + damageInterval)
                DamagePlayer(collision.gameObject);
        }
    }

    private void DamagePlayer(GameObject playerObj)
    {
        if (_enemyHealth != null && _enemyHealth.IsHitStunned) return;
        if (_enemyHealth != null && _enemyHealth.IsDead) return;

        HollowKnightMovement player = playerObj.GetComponent<HollowKnightMovement>();
        if (player != null)
        {
            player.TakeDamage(contactDamage, transform.position);
            _lastDamageTime = Time.time;
        }
        CinemachineImpulseSource impulse = GetComponent<CinemachineImpulseSource>();
        if (impulse != null)
            impulse.GenerateImpulse(shakeForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null) { Gizmos.color = Color.green; Gizmos.DrawSphere(pointA.position, 0.2f); }
        if (pointB != null) { Gizmos.color = Color.blue; Gizmos.DrawSphere(pointB.position, 0.2f); }
        if (pointA != null && pointB != null) { Gizmos.color = Color.cyan; Gizmos.DrawLine(pointA.position, pointB.position); }
    }
}