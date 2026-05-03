using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtPoint = 1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;

    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageInterval = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;
    private Animator _anim;

    private enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Patrol;

    private Transform currentTarget;
    private float waitTimer;
    private bool isWaiting;
    private Transform player;
    private float lastDamageTime;
    private bool facingRight = true;
    private bool _isWaitingAfterAttack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        currentTarget = pointA;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead) return;
        if (_isWaitingAfterAttack || (enemyHealth != null && enemyHealth.IsHitStunned))
        {
            rb.linearVelocity = Vector2.zero;
            if (_anim != null) _anim.SetBool("isMoving", false);
            if (_anim != null) _anim.SetBool("isAttacking", false);
            return;
        }

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chase;
                    isWaiting = false;
                }
                break;

            case EnemyState.Chase:
                Chase();
                if (distanceToPlayer <= attackRange)
                    currentState = EnemyState.Attack;
                else if (distanceToPlayer > detectionRange * 1.5f)
                    currentState = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                Attack();
                if (distanceToPlayer > attackRange * 1.2f)
                    currentState = EnemyState.Chase;
                break;
        }
    }

    private void Patrol()
    {
        if (isWaiting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (_anim != null) _anim.SetBool("isMoving", false);
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        if (distance < 0.3f)
        {
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
            rb.linearVelocity = Vector2.zero;
            Vector3 targetPos = currentTarget.position;
            transform.position = new Vector3(targetPos.x, transform.position.y, transform.position.z);
            return;
        }

        MoveTowards(currentTarget.position, patrolSpeed);
        if (_anim != null) _anim.SetBool("isAttacking", false);
    }

    private void Chase()
    {
        if (player == null) { currentState = EnemyState.Patrol; return; }
        MoveTowards(player.position, chaseSpeed);
        if (_anim != null) _anim.SetBool("isAttacking", false);
    }

    private void Attack()
    {
        if (enemyHealth != null && enemyHealth.IsHitStunned)
        {
            rb.linearVelocity = Vector2.zero;
            if (_anim != null) _anim.SetBool("isAttacking", false);
            return;
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (player != null)
        {
            bool shouldFaceRight = player.position.x > transform.position.x;
            if (shouldFaceRight != facingRight) Flip();
        }
        if (_anim != null) _anim.SetBool("isMoving", false);
        if (_anim != null) _anim.SetBool("isAttacking", true);
    }

    private void MoveTowards(Vector2 targetPosition, float speed)
    {
        if (!IsGroundAhead())
        {
            if (currentState == EnemyState.Patrol)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
        }

        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        bool shouldFaceRight = direction > 0;
        if (shouldFaceRight != facingRight) Flip();
        if (_anim != null) _anim.SetBool("isMoving", true);
    }

    private bool IsGroundAhead()
    {
        Vector2 rayOrigin = (Vector2)groundCheck.position + Vector2.right * (facingRight ? 0.5f : -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
            if (Time.time >= lastDamageTime + damageInterval)
                DamagePlayer(collision.gameObject);
        }
    }

    private void DamagePlayer(GameObject playerObj)
    {
        if (enemyHealth != null && enemyHealth.IsHitStunned) return;
        if (enemyHealth != null && enemyHealth.IsDead) return;

        HollowKnightMovement playerMovement = playerObj.GetComponent<HollowKnightMovement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(contactDamage, transform.position);
            lastDamageTime = Time.time;
            StartCoroutine(PauseAfterAttack());
        }
    }

    private IEnumerator PauseAfterAttack()
    {
        _isWaitingAfterAttack = true;
        rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.SetBool("isMoving", false);
        if (_anim != null) _anim.SetBool("isAttacking", false);
        yield return new WaitForSeconds(damageInterval);
        _isWaitingAfterAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (pointA != null) { Gizmos.color = Color.green; Gizmos.DrawSphere(pointA.position, 0.3f); }
        if (pointB != null) { Gizmos.color = Color.blue; Gizmos.DrawSphere(pointB.position, 0.3f); }
        if (pointA != null && pointB != null) { Gizmos.color = Color.cyan; Gizmos.DrawLine(pointA.position, pointB.position); }
        if (groundCheck != null)
        {
            Gizmos.color = Color.magenta;
            Vector2 rayOrigin = (Vector2)groundCheck.position + Vector2.right * (facingRight ? 0.5f : -0.5f);
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * groundCheckDistance);
        }
    }
}