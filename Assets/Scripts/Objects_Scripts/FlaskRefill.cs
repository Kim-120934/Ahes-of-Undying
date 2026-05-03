using UnityEngine;

public class FlaskRefill : MonoBehaviour
{
    private bool _playerInRange = false;
    private bool _used = false;
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }

    private void Update()
    {
        if (_used || !_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
            if (player != null)
            {
                player.currentSoul = player.maxSoul;
                _used = true;
                if (_anim != null) _anim.SetTrigger("Used");
            }
        }
    }
}