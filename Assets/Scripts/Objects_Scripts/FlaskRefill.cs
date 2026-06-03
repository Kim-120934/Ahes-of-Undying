using UnityEngine;
public class FlaskRefill : MonoBehaviour
{
    [SerializeField] private Sprite emptySprite;  // arrastra aquí el sprite de fuente vacía
    [SerializeField] private int cost = 10;        // coste en monedas

    private bool _playerInRange = false;
    private bool _used = false;
    private Animator _anim;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
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
            // Comprobar monedas
            if (CoinManager.instance == null || CoinManager.instance.GetCoins() < cost)
            {
                Debug.Log("No tienes suficientes monedas"); // sustituye por UI si tienes
                return;
            }

            HollowKnightMovement player = FindFirstObjectByType<HollowKnightMovement>();
            if (player != null)
            {
                CoinManager.instance.SpendCoins(cost); // necesitas este método, ver abajo
                player.currentSoul = player.maxSoul;
                _used = true;

                // Cambiar sprite
                if (emptySprite != null && _sr != null)
                {
                    if (_anim != null) _anim.enabled = false; // desactivar animator para que no sobreescriba el sprite
                    _sr.sprite = emptySprite;
                }
                else if (_anim != null)
                {
                    _anim.SetTrigger("Used");
                }
            }
        }
    }
}