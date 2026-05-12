using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;

    [Header("Hit Stun")]
    [SerializeField] private float hitStunDuration = 0.5f;
    public bool IsHitStunned { get; private set; } = false;

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 2;
    [SerializeField] private int maxCoins = 10;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AbsorbableEnemy _absorbable;
    private HitEffect _hitEffect;

    public bool IsDead { get; internal set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _absorbable = GetComponent<AbsorbableEnemy>();
        _hitEffect = GetComponent<HitEffect>();
        
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockbackDirection.x, 0f) * knockbackForce, ForceMode2D.Impulse);
        }

        if (_hitEffect != null)
            _hitEffect.PlayHitEffect(transform.position);

        AudioManager.instance.PlaySFX(AudioManager.instance.hitEnemySFX);

        if (currentHealth <= 0)
        {
            spriteRenderer.color = Color.white;
            Die();
        }
        else
        {
            StartCoroutine(DamageFlash());
            StartCoroutine(HitStun());
        }
    }

    private IEnumerator DamageFlash()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Hit");
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator HitStun()
    {
        IsHitStunned = true;
        yield return new WaitForSeconds(hitStunDuration);
        IsHitStunned = false;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
        if (player != null)
            player.GainSoul(1);

        AudioManager.instance.PlaySFX(AudioManager.instance.killEnemySFX);

        if (_absorbable != null)
            _absorbable.TriggerStun();
        else
            StartCoroutine(DieSequence());
    }

    public void ForceDie(bool wasAbsorbed)
    {
        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Death");
            yield return new WaitForSeconds(1f);
        }

        int coinAmount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinAmount; i++)
        {
            if (coinPrefab != null)
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        GetComponent<BossController>()?.OnDeath();
        Destroy(gameObject);
    }
}