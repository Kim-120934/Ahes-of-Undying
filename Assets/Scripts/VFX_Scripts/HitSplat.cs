using UnityEngine;

public class HitSplat : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        float duration = _anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, duration);
    }
}