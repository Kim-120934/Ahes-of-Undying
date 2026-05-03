using UnityEngine;
using System.Collections;

public class BreakableChest : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int hitsToBreak = 3;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 3;
    [SerializeField] private int maxCoins = 8;

    private int _currentHits;
    private Animator _anim;

    private void Awake()
    {
        _currentHits = hitsToBreak;
        _anim = GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        _currentHits--;
        if (_anim != null) _anim.SetTrigger("Hit");

        if (_currentHits <= 0)
            StartCoroutine(Break());
    }

    private IEnumerator Break()
    {
        if (_anim != null)
        {
            _anim.SetTrigger("Break");
            yield return new WaitForSeconds(0.5f);
        }

        int amount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < amount; i++)
        {
            if (coinPrefab != null)
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    
}