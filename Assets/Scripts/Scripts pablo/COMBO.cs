using System.Collections;
using UnityEngine;

public class COMBO : MonoBehaviour
{
    private Animator _ani;
    private HollowKnightMovement _player;

    private string[] _triggers = { "Attack1", "Attack2", "Attack3" };

    public int combo = 0;
    public bool atacando = false;
    private bool _inputBuffered = false;
    private float _comboResetTimer = 0f;
    [SerializeField] private float _comboResetTime = 0.8f;

    void Start()
    {
        _ani = GetComponent<Animator>();
        _player = GetComponent<HollowKnightMovement>();
    }

    void Update()
    {
        if (combo > 0 && !atacando)
        {
            _comboResetTimer += Time.deltaTime;
            if (_comboResetTimer >= _comboResetTime)
            {
                combo = 0;
                _comboResetTimer = 0f;
            }
        }
        else
        {
            _comboResetTimer = 0f;
        }
    }

    public void TryAttack()
    {
        if (!atacando)
        {
            atacando = true;
            _comboResetTimer = 0f;
            StopAllCoroutines();

            // Si está en el aire, ataque aéreo
            if (!_player.IsGrounded())
            {
                _ani.SetTrigger("AttackAir");
            }
            else
            {
                _ani.SetTrigger(_triggers[combo]);
            }
        }
        else if (!_inputBuffered)
        {
            _inputBuffered = true;
        }
    }

    public void OnHitFrame()
    {
        _player.DetectAndHitEnemies();
        PlayAttackSFX();
    }

    public void AnimEnd()
    {
        if (combo < 2)
            combo++;
        else
            combo = 0;

        if (_inputBuffered && combo > 0)
        {
            _inputBuffered = false;
            StopAllCoroutines();
            StartCoroutine(DelayedTrigger(combo));
        }
        else
        {
            atacando = false;
            _inputBuffered = false;
        }
    }
    public void AnimEndAir()
    {
        atacando = false;
        _inputBuffered = false;
    }
    private IEnumerator DelayedTrigger(int comboIndex)
    {
        yield return new WaitForSeconds(0.05f);
        _ani.SetTrigger(_triggers[comboIndex]);
    }

    public void ResetCombo()
    {
        StopAllCoroutines();
        atacando = false;
        _inputBuffered = false;
        combo = 0;
        _comboResetTimer = 0f;
    }

    private void PlayAttackSFX()
    {
        if (AudioManager.instance != null && AudioManager.instance.attackSFX != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.attackSFX);
    }
}