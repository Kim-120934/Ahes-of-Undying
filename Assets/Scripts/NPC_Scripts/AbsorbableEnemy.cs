using UnityEngine;
using System.Collections;

/// <summary>
/// Añadir al enemigo junto a EnemyHealth.
/// Cuando EnemyHealth llama a TriggerStun(), el enemigo entra en stun,
/// muestra el icono F, y espera a que el jugador absorba o se acabe el tiempo.
/// </summary>
public class AbsorbableEnemy : MonoBehaviour
{

    public enum AbsorbUnlockType { Projectile, DoubleJump, Dash, None, Souls }
    [Header("Soul Reward")]
    [SerializeField] private int soulReward = 0;
    [Header("Boss Options")]
    [SerializeField] private bool showAbilityUI = false;
    [SerializeField] private DasherPersistence persistence;
    [Header("Absorción")]
    [SerializeField] private AbsorbUnlockType unlockType = AbsorbUnlockType.None;
    [SerializeField] private float stunDuration = 6f;       // Tiempo antes de morir solo
    [SerializeField] private float absorbRange = 2f;        // Rango para que el jugador pueda absorber

    [Header("Referencias")]
    [SerializeField] private InteractUI interactUI;          // El componente InteractUI del enemigo

    // Estado
    public bool IsStunned { get; private set; } = false;
    public bool IsAbsorbed { get; private set; } = false;

    private EnemyHealth _enemyHealth;
    private HollowKnightMovement _player;
    private Animator _anim;
    private Coroutine _stunCoroutine;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>();

        // Si no se asignó en inspector, buscar en hijos
        if (interactUI == null)
            interactUI = GetComponentInChildren<InteractUI>();
    }

    private void Update()
    {
        if (!IsStunned || IsAbsorbed) return;

        // Detectar input F si el jugador está en rango
        if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= absorbRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(AbsorbSequence());
            }
        }
    }

    /// <summary>
    /// Llamado desde EnemyHealth cuando la vida llega a 0.
    /// </summary>
    public void TriggerStun()
    {
        if (IsStunned) return;
        IsStunned = true;

        // Desactivar patrol para que no haga daño
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunSequence());
    }

    private IEnumerator StunSequence()
    {
        // Activar animación de stun
        if (_anim != null)
            _anim.SetTrigger("Stun");

        // Mostrar icono F
        if (interactUI != null)
            interactUI.Show();

        // Esperar stunDuration o hasta que sea absorbido
        float timer = 0f;
        while (timer < stunDuration && !IsAbsorbed)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Si no fue absorbido, morir directamente
        if (!IsAbsorbed)
        {
            if (interactUI != null)
                interactUI.Hide();

            _enemyHealth.ForceDie(wasAbsorbed: false);
        }
    }

    private IEnumerator AbsorbSequence()
    {
        IsAbsorbed = true;

        // Ocultar icono F
        if (interactUI != null)
            interactUI.Hide();

        // Animación absorb en el jugador (si existe)
        // _player puede tener un trigger "Absorb" en su Animator
        if (_player != null)
            _player.PlayAbsorbAnimation();

        // Animación absorb en el enemigo
        if (_anim != null)
            _anim.SetTrigger("Absorb");

        // Esperar duración de la animación de absorb
        yield return new WaitForSeconds(1.5f);
        _player.SetAnimatorBool("isAbsorbing", false);

        // Desbloquear habilidad
        if (_player != null)
        {
            switch (unlockType)
            {
                case AbsorbUnlockType.Projectile:
                    _player.UnlockProjectile();
                    break;
                case AbsorbUnlockType.DoubleJump:
                    _player.UnlockDoubleJump();
                    break;
                case AbsorbUnlockType.None:
                    break;
                case AbsorbUnlockType.Dash:
                    _player.UnlockDash();
                    break;

            }
        }
        if (soulReward > 0 && _player != null)
            _player.GainSoul(soulReward);
        // Boss options
        if (persistence != null)
            persistence.OnDasherDefeated();

        if (showAbilityUI && AbilityUnlockUI.instance != null)
            AbilityUnlockUI.instance.Show();

        // Morir con animación
        _enemyHealth.ForceDie(wasAbsorbed: true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, absorbRange);
    }
}

