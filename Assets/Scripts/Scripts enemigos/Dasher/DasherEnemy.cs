using UnityEngine;
using System.Collections;

public class DasherEnemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float dashRange = 4f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float telegraphDuration = 0.5f; // Pausa antes del dash
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float retreatDistance = 3f;

    [Header("Attack")]
    [SerializeField] private Vector2 attackHitboxSize = new Vector2(1f, 1f);
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float hitStunWindow = 0.5f; // Ventana para contraatacar

    [Header("Effects")]
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private LayerMask playerLayer;
    private GhostTrail _ghostTrail;

    private enum State { Idle, Walk, Telegraph, Dash, Attack, Retreat, HitStun }
    private State _state = State.Idle;

    private Transform _player;
    private Rigidbody2D _rb;
    private EnemyHealth _enemyHealth;
    private Animator _anim;
    private SpriteRenderer _sr;
    private bool _facingRight = true;
    private float _lastDashTime = -99f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _ghostTrail = GetComponent<GhostTrail>();
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

        switch (_state)
        {
            case State.Idle:
                _rb.linearVelocity = Vector2.zero;
                if (dist <= detectionRange)
                    _state = State.Walk;
                break;

            case State.Walk:
                WalkTowardsPlayer();
                if (dist <= dashRange && Time.time >= _lastDashTime + dashCooldown)
                    StartCoroutine(TelegraphAndDash());
                break;
        }
    }

    private void WalkTowardsPlayer()
    {
        float dir = Mathf.Sign(_player.position.x - transform.position.x);
        _rb.linearVelocity = new Vector2(dir * walkSpeed, _rb.linearVelocity.y);
        FaceDirection(dir > 0);
        if (_anim != null) _anim.SetBool("isWalking", true);
    }

    private IEnumerator TelegraphAndDash()
    {
        _state = State.Telegraph;
        _rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.SetBool("isWalking", false);

        // Parpadeo de telegraph
        float timer = 0f;
        while (timer < telegraphDuration)
        {
            timer += Time.deltaTime;
            _sr.color = timer % 0.1f < 0.05f ? Color.yellow : Color.white;
            yield return null;
        }
        _sr.color = Color.white;

        // Dash hacia el jugador
        _state = State.Dash;
        _lastDashTime = Time.time;
        float dashDir = Mathf.Sign(_player.position.x - transform.position.x);
        FaceDirection(dashDir > 0);

        if (dashTrail != null) dashTrail.emitting = true;
        if (_ghostTrail != null) _ghostTrail.StartTrail();
        if (_anim != null) _anim.SetTrigger("Dash");

        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            _rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            yield return null;
        }

        if (dashTrail != null) dashTrail.emitting = false;
        if (_ghostTrail != null) _ghostTrail.StopTrail();
        _rb.linearVelocity = Vector2.zero;

        // Ventana de hitstun donde el jugador puede contraatacar
        _state = State.HitStun;
        if (_anim != null) _anim.SetTrigger("HitStun");
        yield return new WaitForSeconds(hitStunWindow);

        // Ataque melee
        yield return StartCoroutine(PerformAttack());

        // Retreat
        yield return StartCoroutine(Retreat());

        _state = State.Walk;
    }

    private IEnumerator PerformAttack()
    {
        _state = State.Attack;
        if (_anim != null) _anim.SetTrigger("Attack");

        // Detectar jugador con hitbox
        Vector2 attackPos = (Vector2)transform.position + Vector2.right * (_facingRight ? attackRange : -attackRange);
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, attackHitboxSize, 0f, playerLayer);
        foreach (Collider2D hit in hits)
        {
            HollowKnightMovement player = hit.GetComponent<HollowKnightMovement>();
            if (player != null)
                player.TakeDamage(attackDamage, transform.position);
        }

        yield return new WaitForSeconds(attackDuration);
    }

    private IEnumerator Retreat()
    {
        _state = State.Retreat;
        if (_anim != null) _anim.SetTrigger("Retreat");

        float retreatDir = _facingRight ? -1f : 1f;
        float timer = 0f;
        float retreatTime = retreatDistance / walkSpeed;

        while (timer < retreatTime)
        {
            timer += Time.deltaTime;
            _rb.linearVelocity = new Vector2(retreatDir * walkSpeed, _rb.linearVelocity.y);
            yield return null;
        }

        _rb.linearVelocity = Vector2.zero;
    }

    private void FaceDirection(bool faceRight)
    {
        if (faceRight == _facingRight) return;
        _facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashRange);
    }
}
