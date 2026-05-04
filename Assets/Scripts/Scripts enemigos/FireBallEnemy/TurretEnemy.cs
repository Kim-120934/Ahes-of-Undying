using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    public enum FireMode { Straight, Tracking }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;

    [Header("Shooting")]
    [SerializeField] private FireMode fireMode = FireMode.Straight;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;

    [Header("Tracking")]
    [SerializeField] private float trackingError = 1.5f; // Margen de error para que sea justo

    private Transform _player;
    private EnemyHealth _enemyHealth;
    private SpriteRenderer _sr;
    private bool _facingRight = true;
    private float _lastFireTime = 0f;
    private bool _playerInRange = false;
    private Animator _anim;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_enemyHealth != null && _enemyHealth.IsDead) return;
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        _playerInRange = dist <= detectionRange;

        if (_playerInRange)
        {
            // Flip hacia el jugador
            bool shouldFaceRight = _player.position.x > transform.position.x;
            if (shouldFaceRight != _facingRight)
            {
                _facingRight = shouldFaceRight;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }

            // Disparar
            if (Time.time >= _lastFireTime + fireRate)
            {
                Fire();
                _lastFireTime = Time.time;
            }
        }
    }

    private void Fire()
    {
        if (_anim != null) _anim.SetTrigger("Fire");
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 direction = Vector2.zero;

        if (fireMode == FireMode.Straight)
        {
            direction = _facingRight ? Vector2.right : Vector2.left;
        }
        else if (fireMode == FireMode.Tracking)
        {
            // Dirección aproximada al jugador con margen de error
            Vector2 targetPos = (Vector2)_player.position + Random.insideUnitCircle * trackingError;
            direction = (targetPos - (Vector2)firePoint.position).normalized;
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        TurretProjectile tp = proj.GetComponent<TurretProjectile>();
        if (tp != null) tp.Init(direction, projectileSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
