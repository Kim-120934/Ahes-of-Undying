using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public enum BossPhase { Idle, SwordAttack, Vulnerable, LaserAttack, Dead }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float attackRange = 3f;

    [Header("Sword Attack")]
    [SerializeField] private int swordComboCount = 3;
    [SerializeField] private float timeBetweenSwings = 0.8f;
    [SerializeField] private float vulnerableTime = 3f;
    [SerializeField] private Vector2 swordHitboxSize = new Vector2(5f, 4f);
    [SerializeField] private float swordHitboxRange = 1f;
    [SerializeField] private int swordDamage = 1;
    [SerializeField] private GameObject shockwavePrefab;

    [Header("Laser Attack")]
    [SerializeField] private int laserCount = 3;
    [SerializeField] private float laserSpread = 8f;
    [SerializeField] private float laserWarningTime = 1.5f;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float timeBetweenLasers = 0.4f;
    [SerializeField] private float laserCooldown = 8f;

    [Header("Phase")]
    [SerializeField] private int swordAttacksBeforeLaser = 2;

    [Header("References")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BossPersistence bossPersistence;

    private BossPhase _phase = BossPhase.Idle;
    private Transform _player;
    private EnemyHealth _enemyHealth;
    private Animator _anim;
    private Rigidbody2D _rb;
    private bool _facingRight = true;
    private int _swordAttacksDone = 0;
    private float _lastLaserTime = -99f;
    private bool _isActing = false;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>()?.transform;
        Debug.Log($"Player encontrado: {_player}");
    }

    private void Update()
    {
        if (_enemyHealth != null && _enemyHealth.IsDead) return;
        if (_player == null || _isActing) return;

        float dist = Vector2.Distance(transform.position, _player.position);


        if (dist > detectionRange) return;

        FacePlayer();

        switch (_phase)
        {
            case BossPhase.Idle:
                _phase = BossPhase.SwordAttack;
                break;

            case BossPhase.SwordAttack:
                if (dist <= attackRange)
                    StartCoroutine(SwordCombo());
                else
                    Walk();
                break;

            case BossPhase.LaserAttack:
                StartCoroutine(LaserSequence());
                break;
        }
        
    }
    [Header("Contact")]
    [SerializeField] private float contactDamageInterval = 1f;
    private float _lastContactDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= _lastContactDamageTime + contactDamageInterval)
            {
                HollowKnightMovement player = collision.gameObject.GetComponent<HollowKnightMovement>();
                if (player != null)
                    player.TakeDamage(1, transform.position);
                _lastContactDamageTime = Time.time;
            }
        }
    }
    private void Walk()
    {
        float dir = _facingRight ? 1f : -1f;
        _rb.linearVelocity = new Vector2(dir * walkSpeed, _rb.linearVelocity.y);
        if (_anim != null) _anim.SetBool("isWalking", true);
    }

    private IEnumerator SwordCombo()
    {
        FacePlayer();
        Debug.Log("SwordCombo iniciado");
        _isActing = true;
        _rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.SetBool("isWalking", false);

        for (int i = 0; i < swordComboCount; i++)
        {
            // Animación de ataque espada
            if (_anim != null) _anim.SetTrigger("AttackSword");
            yield return new WaitForSeconds(0.3f);

            // Hitbox del golpe
            Vector2 hitPos = new Vector2(transform.position.x + (_facingRight ? swordHitboxRange : -swordHitboxRange),
              _player.position.y
             );
            Debug.Log($"Hitbox en: {hitPos} tamaño: {swordHitboxSize}");
            Debug.Log($"Jugador en: {_player.position} Hitbox en: {hitPos}");
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitPos, swordHitboxSize, 0f);
            Debug.Log($"Hits detectados: {hits.Length}");
            foreach (Collider2D hit in hits)
            {
                HollowKnightMovement player = hit.GetComponent<HollowKnightMovement>();
                if (player != null) player.TakeDamage(swordDamage, transform.position);
            }

            // Onda expansiva
            if (shockwavePrefab != null)
            {
                Vector3 spawnPos = new Vector3(transform.position.x + (_facingRight ? 1f : -1f),transform.position.y - 3f,0f);
                GameObject sw = Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
                sw.GetComponent<Shockwave>()?.Init(_facingRight);
                if (!_facingRight)
                {
                    Vector3 scale = sw.transform.localScale;
                    scale.x *= -1;
                    sw.transform.localScale = scale;
                }
            }

            yield return new WaitForSeconds(timeBetweenSwings);
        }

        // Espada atrapada - fase vulnerable
        if (_anim != null) _anim.SetTrigger("SwordStuck");
        _phase = BossPhase.Vulnerable;
        yield return new WaitForSeconds(vulnerableTime);

        _swordAttacksDone++;

        // Decidir siguiente fase
        if (_swordAttacksDone >= swordAttacksBeforeLaser && Time.time >= _lastLaserTime + laserCooldown)
        {
            _swordAttacksDone = 0;
            _phase = BossPhase.LaserAttack;
        }
        else
        {
            _phase = BossPhase.SwordAttack;
        }

        _isActing = false;
    }

    private IEnumerator LaserSequence()
    {
        _isActing = true;
        _rb.linearVelocity = Vector2.zero;
        _lastLaserTime = Time.time;

        if (_anim != null) _anim.SetTrigger("LaserAbility");
        yield return new WaitForSeconds(0.5f);

        // Spawnear rayos en posiciones aleatorias alrededor del jugador
        for (int i = 0; i < laserCount; i++)
        {
            float randomX = _player.position.x + Random.Range(-laserSpread, laserSpread);
            Vector3 laserPos = new Vector3(randomX, _player.position.y + 10f, 0f);

            if (laserPrefab != null)
                Instantiate(laserPrefab, laserPos, Quaternion.identity);

            yield return new WaitForSeconds(timeBetweenLasers);
        }

        // Animación uso rayo
        if (_anim != null) _anim.SetTrigger("LaserUse");
        yield return new WaitForSeconds(1f);

        _phase = BossPhase.SwordAttack;
        _isActing = false;
    }

    private void FacePlayer()
    {
        bool shouldFaceRight = _player.position.x > transform.position.x;
        if (shouldFaceRight == _facingRight) return;
        _facingRight = shouldFaceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnDeath()
    {
        _phase = BossPhase.Dead;
        if (bossPersistence != null) bossPersistence.OnBossDefeated();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
